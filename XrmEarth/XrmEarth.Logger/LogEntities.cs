using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmEarth.Logger.Common;

namespace XrmEarth.Logger
{
    public class LogEntities
    {
        public class Application : LogEntity
        {
            public Application(string logicalName) : base(logicalName, "Application", "Applications")
            {
                Schema.Add(Fields.Identifier, Tuple.Create("Tanımlayıcı", AttributeTypeCode.String));
                Schema.Add(Fields.Name, Tuple.Create("Uygulama Adı", AttributeTypeCode.String));
                Schema.Add(Fields.Description, Tuple.Create("Açıklama", AttributeTypeCode.String));
                Schema.Add(Fields.Version, Tuple.Create("Versiyon", AttributeTypeCode.String));
                Schema.Add(Fields.Namespace, Tuple.Create("İsim uzayı", AttributeTypeCode.String));
            }

            public override KeyValuePair<string, Tuple<string, AttributeTypeCode>> PrimaryAttribute()
            {
                return Schema.Single(s => s.Key == Fields.Name);
            }

            public class Fields
            {
                public const string Identifier = "new_identifier";
                public const string Name = "new_name";
                public const string Description = "new_description";
                public const string Version = "new_version";
                public const string Namespace = "new_namespace";
            }
        }

        public class ApplicationInstance : LogEntity
        {
            public ApplicationInstance(string logicalName) : base(logicalName, "Application Instance", "Application Instances")
            {
                Schema.Add(Fields.AppicationID, Tuple.Create("Uygulama", AttributeTypeCode.Lookup));
                Schema.Add(Fields.Path, Tuple.Create("Çalıştığı dizin", AttributeTypeCode.String));
                Schema.Add(Fields.StartAt, Tuple.Create("Çalışma zamanı", AttributeTypeCode.DateTime));
                Schema.Add(Fields.FinishAt, Tuple.Create("Kapanma zamanı", AttributeTypeCode.DateTime));
                Schema.Add(Fields.Parameters, Tuple.Create("Başlangıç parametreleri", AttributeTypeCode.String));
                Schema.Add(Fields.Result, Tuple.Create("Sonuç", AttributeTypeCode.String));
                Schema.Add(Fields.Summary, Tuple.Create("Özet", AttributeTypeCode.String));

                EntityReferences.Add(new EntityReferenceContainer
                {
                    AttributeName = Fields.AppicationID,
                    IsInternal = true,
                    ReferenceInternalType = typeof(Application),
                    RelationType = RelationType.ManyToOne,
                    GetRelation = (metadata, selfLogicalName, refLogicalName) =>
                    {
                        return metadata.ManyToOneRelationships.FirstOrDefault(mto => mto.ReferencingEntity == selfLogicalName && mto.ReferencedEntity == refLogicalName);
                    }
                });
            }

            public override KeyValuePair<string, Tuple<string, AttributeTypeCode>> PrimaryAttribute()
            {
                return Schema.Single(s => s.Key == Fields.Path);
            }

            public class Fields
            {
                public const string AppicationID = "new_applicationid";
                public const string Path = "new_path";
                public const string StartAt = "new_startat";
                public const string FinishAt = "new_finishat";
                public const string Parameters = "new_parameters";
                public const string Result = "new_result";
                public const string Summary = "new_summary";
            }
        }

        public class ApplicationLog : LogEntity
        {
            public ApplicationLog(string logicalName) : base(logicalName, "Application Log", "Application Logs")
            {
                Schema.Add(Fields.ApplicationInstanceID, Tuple.Create("Uygulama Örneği", AttributeTypeCode.Lookup));
                Schema.Add(Fields.ParentCallerMember, Tuple.Create("Üst çağıran metod", AttributeTypeCode.String));
                Schema.Add(Fields.CallerMember, Tuple.Create("Çağıran metod", AttributeTypeCode.String));
                Schema.Add(Fields.Message, Tuple.Create("Mesaj", AttributeTypeCode.String));
                Schema.Add(Fields.Type, Tuple.Create("Tip", AttributeTypeCode.Picklist));
                Schema.Add(Fields.LogLevel, Tuple.Create("Log Seviyesi", AttributeTypeCode.Integer));
                Schema.Add(Fields.Tag1, Tuple.Create("Etiket 1", AttributeTypeCode.String));
                Schema.Add(Fields.Tag2, Tuple.Create("Etiket 2", AttributeTypeCode.String));
                
                OptionSets.Add(new OptionSetMetadata(new OptionMetadataCollection
                {
                    new OptionMetadata(CreateLabel("Bilgi"), 1),
                    new OptionMetadata(CreateLabel("Uyarı"), 2),
                    new OptionMetadata(CreateLabel("Hata"), 4),
                    new OptionMetadata(CreateLabel("Nesne"), 8),
                    new OptionMetadata(CreateLabel("Durum"), 16),
                    new OptionMetadata(CreateLabel("Detay Bilgi"), 32),
                })
                {
                    DisplayName = CreateLabel("Log Tipi"),
                    IsGlobal = false,
                    OptionSetType = OptionSetType.Picklist,
                    Name = Fields.Type
                });

                EntityReferences.Add(new EntityReferenceContainer
                {
                    AttributeName = Fields.ApplicationInstanceID,
                    IsInternal = true,
                    ReferenceInternalType = typeof(ApplicationInstance),
                    RelationType = RelationType.ManyToOne,
                    GetRelation = (metadata, selfLogicalName, refLogicalName) =>
                    {
                        return metadata.ManyToOneRelationships.FirstOrDefault(mto => mto.ReferencingEntity == selfLogicalName && mto.ReferencedEntity == refLogicalName);
                    }
                });
            }

            public override KeyValuePair<string, Tuple<string, AttributeTypeCode>> PrimaryAttribute()
            {
                return Schema.Single(s => s.Key == Fields.Message);
            }

            public class Fields
            {
                public const string ApplicationInstanceID = "new_applicationinstanceid";
                public const string ParentCallerMember = "new_parentcallermember";
                public const string CallerMember = "new_callermember";
                public const string Message = "new_message";
                public const string Type = "new_type";
                public const string LogLevel = "new_loglevel";
                public const string Tag1 = "new_tag1";
                public const string Tag2 = "new_tag2";
            }
        }

        public class LogEntity
        {
            protected LogEntity(string logicalName, string displayName, string pluralName)
            {
                LogicalName = logicalName;
                DisplayName = displayName;
                PluralName = pluralName;
            }
            public string LogicalName { get; private set; }
            public string DisplayName { get; private set; }
            public string PluralName { get; private set; }

            public virtual KeyValuePair<string, Tuple<string, AttributeTypeCode>> PrimaryAttribute()
            {
                return Schema.FirstOrDefault();
            }

            public readonly Dictionary<string, Tuple<string, AttributeTypeCode>> Schema = new Dictionary<string, Tuple<string, AttributeTypeCode>>();
            public readonly List<OptionSetMetadata> OptionSets = new List<OptionSetMetadata>();
            public readonly List<EntityReferenceContainer> EntityReferences = new List<EntityReferenceContainer>();
        }

        public class EntityReferenceContainer
        {
            public string AttributeName { get; set; }
            
            public string ReferenceEntityLogicalName { get; set; }

            public bool IsInternal { get; set; }
            public Type ReferenceInternalType { get; set; }
            public RelationType RelationType { get; set; }

            public RelationshipMetadataBase FindRelation(EntityMetadata metadata, LogEntity logEntity)
            {
                //var container = logEntity.EntityReferences.Single(er => er.AttributeName == AttributeName);
                var selfLogicalName = logEntity.LogicalName;
                var refLogicalName = IsInternal
                    ? GetLogicalName(ReferenceInternalType)
                    : ReferenceEntityLogicalName;

                return GetRelation(metadata, selfLogicalName, refLogicalName);
            }
            public Func<EntityMetadata, string, string, RelationshipMetadataBase> GetRelation { get; set; }
        }

        public static Label CreateLabel(string title)
        {
            return new Label(title, LanguageCode.Turkey_Turkish.GetHashCode());
        }
        
        public static string GetLogicalName(Type type)
        {
            if (type == typeof(LogEntities.Application))
            {
                return CrmLogger.ApplicationLogicalName;
            }
            if (type == typeof(LogEntities.ApplicationInstance))
            {
                return CrmLogger.ApplicationInstanceLogicalName;
            }
            if (type == typeof(LogEntities.ApplicationLog))
            {
                return CrmLogger.ApplicationLogLogicalName;
            }
            return null;
        }

        public static string GetPrimaryAttributeName(Type type)
        {
            return GetLogicalName(type) + "id";
        }
    }

    public enum RelationType
    {
        OneToOne,
        OneToMany,
        ManyToOne,
        ManyToMany
    }

    public static class CrmExtensions
    {
        public static string GetTitle(this Label self)
        {
            return self.UserLocalizedLabel.Label;
        }
    }
}
