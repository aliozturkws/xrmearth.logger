using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XrmEarth.Logger
{
    public class InitializerNew : XrmEarth.Logger.Initializer.ILogEnvironmentInitializer
    {
        public InitializerNew(CrmConnection connection)
        {
            Connection = connection;
        }

        public CrmConnection Connection { get; private set; }
        private readonly List<string> _entities = new List<string>();

        public string InitializeEnvironment()
        {
            if (Connection == null)
                throw new NullReferenceException("Connection boş olamaz.");

            var sb = new StringBuilder();

            InitEntities();

            //var entities = CheckEntities(sb);
            var entityReports = CheckEntities();

            foreach (var entityReport in entityReports)
            {
                sb.Append("-->");
                BindReport(entityReport, sb);
                foreach (var childReport in entityReport.ChildReports)
                {
                    BindReport(childReport, sb);
                }
                sb.Append("-->");
            }

            foreach (var entityReport in entityReports)
            {
                RecursivelyRepair(entityReport);
            }
            return sb.ToString();
        }

        private void BindReport(MetadataReport report, StringBuilder stringBuilder)
        {
            switch (report.MetadataType)
            {
                case MetadataType.Entity:
                    {
                        var entityReport = (EntityMetadataReport)report;
                        stringBuilder
                            .Append("<- Varlık ->")
                            .AppendLine()
                            .Append(entityReport.Schema.GetType().Name).Append(" - ").Append(entityReport.Schema.LogicalName)
                            .AppendLine();
                    }
                    break;
                case MetadataType.Attribute:
                    stringBuilder
                        .AppendLine()
                        .Append(report.LogicalName)
                        .Append(" - ");
                    break;
                case MetadataType.OptionSet:
                    stringBuilder
                        .AppendLine()
                        .Append(report.LogicalName)
                        .Append(" - ");
                    break;
                case MetadataType.Relation:
                    stringBuilder
                        .AppendLine()
                        .Append(report.LogicalName)
                        .Append(" - ");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (report.IsValid)
            {
                stringBuilder
                    .Append(report.MetadataType)
                    .Append(" geçerli.");
            }
            else
            {
                var repOptTxt = string.Empty;
                switch (report.RepairOption)
                {
                    case RepairOption.None:
                        repOptTxt = "Yok";
                        break;
                    case RepairOption.Create:
                        repOptTxt = "Oluşturma";
                        break;
                    case RepairOption.DeleteCreate:
                        repOptTxt = "Mevcudu silip yeniden oluşturma";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                stringBuilder
                    .Append(report.MetadataType)
                    .Append(" geçersiz. Onarım Türü: ")
                    .Append(repOptTxt);
            }
        }

        private void RecursivelyRepair(MetadataReport report)
        {
            if (report.MetadataType == MetadataType.Entity)
            {
                var entityReport = report as EntityMetadataReport;
                if (entityReport == null)
                    throw new NullReferenceException(
                        "Rapor beklenen şekilde değildi. Entity raporlarının EntityMetadataReport tipinde olması gerekir.");

                RepairEntity(entityReport);

                foreach (var childReport in entityReport.ChildReports)
                {
                    RecursivelyRepair(childReport);
                }
            }
            else if (report.MetadataType == MetadataType.Attribute)
            {
                RepairAttribute(report);
            }
            else if (report.MetadataType == MetadataType.OptionSet)
            {
                RepairOptionSet(report);
            }
            else if (report.MetadataType == MetadataType.Relation)
            {
                RepairRelation(report);
            }
        }

        private void RepairRelation(MetadataReport report)
        {
            RepairAttribute(report);
        }

        private void RepairOptionSet(MetadataReport report)
        {
            RepairAttribute(report);
        }

        private void RepairAttribute(MetadataReport report)
        {
            if (!report.IsValid)
            {
                var entityReport = report.GetTopReport() as EntityMetadataReport;
                if (entityReport == null)
                    throw new NullReferenceException("Geçersiz rapor yapısı.");

                var schema = entityReport.Schema;
                var field = schema.Schema.Single(s => s.Key == report.LogicalName);
                if (report.RepairOption == RepairOption.DeleteCreate)
                {
                    Connection.Service.Execute(new DeleteAttributeRequest
                    {
                        EntityLogicalName = schema.LogicalName,
                        LogicalName = report.LogicalName
                    });

                    if (field.Value.Item2 == AttributeTypeCode.Lookup)
                    {
                        var relation = schema.EntityReferences.Single(er => er.AttributeName == field.Key);
                        var relationName =
                            schema.LogicalName
                            + "_"
                            + (relation.IsInternal
                            ? LogEntities.GetLogicalName(relation.ReferenceInternalType)
                            : relation.ReferenceEntityLogicalName);
                        Connection.Service.Execute(new DeleteRelationshipRequest
                        {
                            Name = relationName,
                        });
                    }
                }

                OrganizationRequest request = null;
                if (field.Value.Item2 == AttributeTypeCode.Lookup)
                {
                    request = CreateLookupRequest(field, entityReport.Schema);
                }
                else
                {
                    request = CreateAttributeRequest(field, entityReport.Schema);
                }

                Connection.Service.Execute(request);
            }
        }

        private void RepairEntity(EntityMetadataReport entityReport)
        {
            if (!entityReport.IsValid)
            {
                if (entityReport.RepairOption == RepairOption.DeleteCreate)
                {
                    var deleteResponse =
                        Connection.Service.Execute(
                                new DeleteEntityRequest { LogicalName = entityReport.Schema.LogicalName }) as
                            DeleteEntityResponse;
                    if (deleteResponse == null)
                        throw new NullReferenceException(
                            "Cevap beklenen şekilde değildi. Initializer>RepairEntity>ServiceExecute_DeleteEntityResponse");
                }

                var fields = entityReport.Schema.Schema;

                var entityRequest = CreateEntityRequest(entityReport.Schema);

                var entityResponse = Connection.Service.Execute(entityRequest) as CreateEntityResponse;
                if (entityResponse == null)
                    throw new NullReferenceException(
                        "Cevap beklenen şekilde değildi. Initializer>RepairEntity>ServiceExecute_CreateEntityResponse");

                var primaryAttribute = entityReport.Schema.PrimaryAttribute();
                foreach (var field in fields)
                {
                    if (field.Key == primaryAttribute.Key)
                        continue;

                    OrganizationRequest request;
                    if (field.Value.Item2 == AttributeTypeCode.Lookup)
                    {
                        request = CreateLookupRequest(field, entityReport.Schema);
                    }
                    else
                    {
                        request = CreateAttributeRequest(field, entityReport.Schema);
                    }

                    var attributeResponse = Connection.Service.Execute(request);

                    if (attributeResponse == null)
                        throw new NullReferenceException(
                            "Cevap beklenen şekilde değildi. Initializer>RepairEntity>ServiceExecute_CreateAttributeResponse");
                }
            }
        }

        private CreateEntityRequest CreateEntityRequest(LogEntities.LogEntity entity)
        {
            var primaryAttribute = entity.PrimaryAttribute();
            return new CreateEntityRequest
            {
                Entity = new EntityMetadata
                {
                    SchemaName = entity.LogicalName,
                    DisplayName = LogEntities.CreateLabel(entity.DisplayName),
                    DisplayCollectionName = LogEntities.CreateLabel(entity.PluralName),
                    OwnershipType = OwnershipTypes.UserOwned,
                    IsActivity = false
                },
                PrimaryAttribute = new StringAttributeMetadata
                {
                    SchemaName = primaryAttribute.Key,
                    FormatName = StringFormatName.Text,
                    MaxLength = 2000,
                    DisplayName = LogEntities.CreateLabel(primaryAttribute.Value.Item1)
                }
            };
        }

        private OrganizationRequest CreateLookupRequest(KeyValuePair<string, Tuple<string, AttributeTypeCode>> field, LogEntities.LogEntity entity)
        {
            var relation = entity.EntityReferences.FirstOrDefault(os => os.AttributeName == field.Key);
            if (relation == null)
                throw new NullReferenceException(field.Key + " isminde relation tanımı bulunamadı.");

            var selfLogicalName = entity.LogicalName;
            var refLogicalName = relation.IsInternal
                ? LogEntities.GetLogicalName(relation.ReferenceInternalType)
                : relation.ReferenceEntityLogicalName;
            var refPrimaryAttributeName = LogEntities.GetPrimaryAttributeName(relation.ReferenceInternalType);
            var schemaName = selfLogicalName + "_" + refLogicalName;


            switch (relation.RelationType)
            {
                case RelationType.OneToOne:
                    break;
                case RelationType.OneToMany:
                    break;
                case RelationType.ManyToOne:
                    {
                        return new CreateOneToManyRequest
                        {
                            Lookup = new LookupAttributeMetadata
                            {
                                SchemaName = field.Key,
                                DisplayName = LogEntities.CreateLabel(field.Value.Item1),
                            },
                            OneToManyRelationship = new OneToManyRelationshipMetadata
                            {
                                AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                                {
                                    Behavior = AssociatedMenuBehavior.UseCollectionName,
                                    Group = AssociatedMenuGroup.Details,
                                    Label = LogEntities.CreateLabel(entity.PluralName),
                                    Order = 10000
                                },
                                CascadeConfiguration = new CascadeConfiguration
                                {
                                    Assign = CascadeType.NoCascade,
                                    Delete = CascadeType.RemoveLink,
                                    Merge = CascadeType.NoCascade,
                                    Reparent = CascadeType.NoCascade,
                                    Share = CascadeType.NoCascade,
                                    Unshare = CascadeType.NoCascade
                                },
                                ReferencedEntity = refLogicalName,
                                ReferencedAttribute = refPrimaryAttributeName,
                                ReferencingEntity = entity.LogicalName,
                                SchemaName = schemaName
                            }
                        };
                    }
                    break;
                case RelationType.ManyToMany:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return null;
        }

        private CreateAttributeRequest CreateAttributeRequest(
            KeyValuePair<string, Tuple<string, AttributeTypeCode>> field, LogEntities.LogEntity entity)
        {
            var typeCode = field.Value.Item2;
            switch (typeCode)
            {
                case AttributeTypeCode.Boolean:
                    return new CreateAttributeRequest
                    {
                        EntityName = entity.LogicalName,
                        Attribute = new BooleanAttributeMetadata
                        {
                            SchemaName = field.Key,
                            DisplayName = LogEntities.CreateLabel(field.Value.Item1),
                        }
                    };
                case AttributeTypeCode.DateTime:
                    return new CreateAttributeRequest
                    {
                        EntityName = entity.LogicalName,
                        Attribute = new DateTimeAttributeMetadata
                        {
                            SchemaName = field.Key,
                            DisplayName = LogEntities.CreateLabel(field.Value.Item1)
                        }
                    };
                case AttributeTypeCode.Decimal:
                    return new CreateAttributeRequest
                    {
                        EntityName = entity.LogicalName,
                        Attribute = new DecimalAttributeMetadata
                        {
                            SchemaName = field.Key,
                            DisplayName = LogEntities.CreateLabel(field.Value.Item1)
                        }
                    };
                case AttributeTypeCode.Double:
                    return new CreateAttributeRequest
                    {
                        EntityName = entity.LogicalName,
                        Attribute = new DoubleAttributeMetadata
                        {
                            SchemaName = field.Key,
                            DisplayName = LogEntities.CreateLabel(field.Value.Item1)
                        }
                    };
                case AttributeTypeCode.Integer:
                    return new CreateAttributeRequest
                    {
                        EntityName = entity.LogicalName,
                        Attribute = new IntegerAttributeMetadata
                        {
                            SchemaName = field.Key,
                            DisplayName = LogEntities.CreateLabel(field.Value.Item1)
                        }
                    };
                case AttributeTypeCode.String:
                    return new CreateAttributeRequest
                    {
                        EntityName = entity.LogicalName,
                        Attribute = new StringAttributeMetadata
                        {
                            SchemaName = field.Key,
                            DisplayName = LogEntities.CreateLabel(field.Value.Item1),
                            MaxLength = 2000
                        }
                    };
                case AttributeTypeCode.BigInt:
                    return new CreateAttributeRequest
                    {
                        EntityName = entity.LogicalName,
                        Attribute = new BigIntAttributeMetadata
                        {
                            SchemaName = field.Key,
                            DisplayName = LogEntities.CreateLabel(field.Value.Item1)
                        }
                    };
                case AttributeTypeCode.Memo:
                    return new CreateAttributeRequest
                    {
                        EntityName = entity.LogicalName,
                        Attribute = new MemoAttributeMetadata
                        {
                            SchemaName = field.Key,
                            DisplayName = LogEntities.CreateLabel(field.Value.Item1)
                        }
                    };
                case AttributeTypeCode.Money:
                    return new CreateAttributeRequest
                    {
                        EntityName = entity.LogicalName,
                        Attribute = new MoneyAttributeMetadata
                        {
                            SchemaName = field.Key,
                            DisplayName = LogEntities.CreateLabel(field.Value.Item1)
                        }
                    };
                case AttributeTypeCode.Uniqueidentifier:
                    return new CreateAttributeRequest
                    {
                        EntityName = entity.LogicalName,
                        Attribute = new UniqueIdentifierAttributeMetadata
                        {
                            SchemaName = field.Key,
                            DisplayName = LogEntities.CreateLabel(field.Value.Item1)
                        }
                    };
                case AttributeTypeCode.Picklist:
                    {
                        var optionSet = entity.OptionSets.FirstOrDefault(os => os.Name == field.Key);
                        if (optionSet == null)
                            throw new NullReferenceException(field.Key + " isminde optionset tanımı bulunamadı.");

                        return new CreateAttributeRequest
                        {
                            EntityName = entity.LogicalName,
                            Attribute = new PicklistAttributeMetadata
                            {
                                SchemaName = field.Key,
                                DisplayName = LogEntities.CreateLabel(field.Value.Item1),
                                OptionSet = optionSet
                            }
                        };
                    }

                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.PartyList:
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                case AttributeTypeCode.CalendarRules:
                case AttributeTypeCode.Virtual:
                case AttributeTypeCode.ManagedProperty:
                case AttributeTypeCode.EntityName:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private List<EntityMetadataReport> CheckEntities()
        {
            var results = new List<EntityMetadataReport>();

            var applicationResult = CheckEntity<LogEntities.Application>();
            results.Add(applicationResult);

            var applicationInstanceResult = CheckEntity<LogEntities.ApplicationInstance>();
            results.Add(applicationInstanceResult);

            var applicationLogResult = CheckEntity<LogEntities.ApplicationLog>();
            results.Add(applicationLogResult);

            return results;
        }

        private EntityMetadataReport CheckEntity<T>(string logicalName = null)
            where T : LogEntities.LogEntity
        {
            if (string.IsNullOrEmpty(logicalName))
                logicalName = LogEntities.GetLogicalName(typeof(T));

            var instance = Activator.CreateInstance(typeof(T), logicalName) as T;
            var entityMetadata = Load(instance);

            var entityReport = ValidateMetadata(instance, entityMetadata);
            return entityReport;
        }

        private EntityMetadataReport ValidateMetadata<T>(T instance, EntityMetadata entityMetadata)
            where T : LogEntities.LogEntity
        {
            var entityReport = new EntityMetadataReport
            {
                Schema = instance,
                LogicalName = instance.LogicalName,
                IsValid = entityMetadata != null
            };

            if (!entityReport.IsValid)
            {
                entityReport.RepairOption = RepairOption.Create;
                return entityReport;
            }


            foreach (var field in instance.Schema)
            {
                var attributeMetadata = entityMetadata.Attributes.SingleOrDefault(a => a.LogicalName == field.Key);

                var attrReport = new MetadataReport(MetadataType.Attribute)
                {
                    LogicalName = field.Key,
                    IsValid = true
                };
                if (attributeMetadata != null)
                {
                    if (attributeMetadata.AttributeType.HasValue)
                    {
                        if (field.Value.Item2 == AttributeTypeCode.Picklist)
                        {
                            if (attributeMetadata.AttributeType.Value == AttributeTypeCode.Picklist)
                            {
                                MetadataReport optionSetReport = null;
                                var optionSet = instance.OptionSets.Single(os => os.Name == field.Key);
                                var picklistAttributeMetadata = (PicklistAttributeMetadata)attributeMetadata;
                                foreach (var option in optionSet.Options.Where(o => o.Value.HasValue))
                                {
                                    var crmOption =
                                        picklistAttributeMetadata.OptionSet.Options.FirstOrDefault(
                                            o => o.Value.HasValue && o.Value.Value == option.Value.Value);
                                    if (crmOption == null)
                                    {
                                        //Olması gereken option mevcut değilse.
                                        optionSetReport = new MetadataReport(MetadataType.OptionSet)
                                        {
                                            LogicalName = optionSet.Name,
                                            RepairOption = RepairOption.DeleteCreate,
                                            IsValid = false
                                        };
                                        break;
                                    }
                                }

                                if (optionSetReport != null)
                                {
                                    entityReport.AddChild(optionSetReport);
                                    //OptionSet'te sorun oluğu için alanla birlikte tekrar oluşturulmalı
                                    attrReport.RepairOption = RepairOption.DeleteCreate;
                                    attrReport.IsValid = false;
                                }
                            }
                            else
                            {
                                //Özellik var fakat optionset değil
                                attrReport.RepairOption = RepairOption.DeleteCreate;
                                attrReport.IsValid = false;
                            }
                        }
                        else if (field.Value.Item2 == AttributeTypeCode.Lookup)
                        {
                            if (attributeMetadata.AttributeType.Value == AttributeTypeCode.Lookup)
                            {
                                var container = instance.EntityReferences.Single(er => er.AttributeName == field.Key);

                                var relation = container.FindRelation(entityMetadata, instance);

                                if (relation == null)
                                {
                                    //Özellik lookup fakat düzgün relation kurulmamış
                                    attrReport.RepairOption = RepairOption.DeleteCreate;
                                    attrReport.IsValid = false;
                                }
                            }
                            else
                            {
                                //Özellik var fakat lookup değil
                                attrReport.RepairOption = RepairOption.DeleteCreate;
                                attrReport.IsValid = false;
                            }
                        }
                        else
                        {
                            if (attributeMetadata.AttributeType.Value != field.Value.Item2)
                            {
                                //Özellik var fakat tip uygun değil
                                attrReport.RepairOption = RepairOption.DeleteCreate;
                                attrReport.IsValid = false;
                            }
                        }
                    }
                    else
                    {
                        //Özellik var fakat tipi belli değil
                        attrReport.RepairOption = RepairOption.DeleteCreate;
                        attrReport.IsValid = false;
                    }
                }
                else
                {
                    //Özellik yok
                    attrReport.RepairOption = RepairOption.Create;
                    attrReport.IsValid = false;
                }
                entityReport.AddChild(attrReport);
            }

            return entityReport;
        }

        private EntityMetadata Load(LogEntities.LogEntity logEntity)
        {
            if (_entities.All(e => e != logEntity.LogicalName))
                return null;

            var entityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships,
                LogicalName = logEntity.LogicalName,
            };

            var entityResponse = Connection.Service.Execute(entityRequest) as RetrieveEntityResponse;
            if (entityResponse == null)
                throw new Exception(
                    "Cevap beklenen şekilde değildi. Initializer>Load_EntityMedata>ServiceExecute_RetrieveEntityResponse");

            return entityResponse.EntityMetadata;
        }

        private void InitEntities()
        {
            var entitiesRequest = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };
            var response = Connection.Service.Execute(entitiesRequest) as RetrieveAllEntitiesResponse;
            if (response == null)
                throw new Exception(
                    "Cevap beklenen şekilde değildi. Initializer>Load_EntityMedata>ServiceExecute_RetrieveAllEntitiesRequest");

            _entities.Clear();
            foreach (var entityMetadata in response.EntityMetadata)
            {
                _entities.Add(entityMetadata.LogicalName);
            }
        }
    }

    public class EntityMetadataReport : MetadataReport
    {
        public EntityMetadataReport() : base(MetadataType.Entity)
        {
            ChildReports = new List<MetadataReport>();
        }

        public LogEntities.LogEntity Schema { get; set; }

        public List<MetadataReport> ChildReports { get; private set; }

        public void ValidateChild(MetadataReport metadataReport)
        {
            metadataReport.Parent = this;
        }

        public void AddChild(MetadataReport metadataReport)
        {
            ChildReports.Add(metadataReport);
            ValidateChild(metadataReport);
        }
    }

    public class MetadataReport
    {
        public MetadataReport(MetadataType metadataType)
        {
            MetadataType = metadataType;
        }

        public string LogicalName { get; set; }
        public MetadataType MetadataType { get; private set; }
        public RepairOption RepairOption { get; set; }
        public bool IsValid { get; set; }
        public object Tag { get; set; }

        public object Parent { get; set; }

        public MetadataReport GetTopReport()
        {
            MetadataReport validParent = null;
            var pReport = Parent as MetadataReport;
            while (pReport != null)
            {
                validParent = pReport;
                pReport = pReport.Parent as MetadataReport;
            }
            return validParent;
        }
    }

    public enum MetadataType
    {
        Entity,
        Attribute,
        OptionSet,
        Relation
    }

    public enum RepairOption
    {
        None = 0,
        Create = 1,
        //Update,
        DeleteCreate = 2
    }
}