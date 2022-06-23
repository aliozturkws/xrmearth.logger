# :pencil2: Xrm Earth Logger :clipboard:

* Provides custom log writing in Dynamics CRM :+1:

## Description

* It provides flexible assembly developments that you can use in your Dynamics CRM processes. :running:
* Functions in this assembly aim to increase productivity by reducing development loads. :star:
* Can be used in plugin, workflow assembly, windows console app, windows service, web service. :sunglasses:

## Getting Started

### Dependencies

* Dynamics CRM V9 Recommended :heart:

### Installing

* Download solution [xrmearthlogger.zip](https://drive.google.com/file/d/14_sOnTFyQybD69Yr0KXw1Po36aGu58nr/view?usp=sharing) and add it to crm. :floppy_disk:
* then customize it by reviewing the [example here](https://drive.google.com/drive/folders/1DntcAAWgUx5mLaH5bJKiofuVbUa93bJP?usp=sharing) :point_left:

## Example Parameters :bell:

### CRM Logger :train: (outside the sandbox)
```
public IOrganizationService Service { get; set; }
var crmConnection = new CrmConnection(Service);
var crmLogger = LogManager.CreateLogger(crmConnection);
LogManager.RegisterAll(crmLogger);
```

### CRM Logger :key: (in the plugin,workflow)
```
public IOrganizationService Service { get; set; }
InitConfiguration.InjectApplication = false; // must be sandbox value=false
InitConfiguration.OverrideAssembly = typeof(IPlugin).Assembly;
var crmConnection = new CrmConnection(service);
var crmLogger = LogManager.CreateLogger(crmConnection);
LogManager.RegisterAll(CrmLogger);
```

### Usage :floppy_disk:
```
crmLogger.Info("Test Log Message", 15);
crmLogger.Error("Test Error", 1501, "Tag1 Value","Tag2 Value");
crmLogger.Info("Test Info", 1502, "Tag1 Value", "Tag2 Value");
crmLogger.Warning("Test Warning", 1502, "Tag1 Value", "Tag2 Value");
```
### Install Nuget Package :blush:
```
Install-Package XrmEarth.Logger -Version 1.0.0
```

