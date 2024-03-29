---
layout: default
title: Welcome to Hyland Rocket RPA API Helper
---
# Welcome to Hyland Rocket RPA API Helper
With this dll you are able to connect to the Hyland RPA API.
- [Getting started](#gettingstarted)
    - [Download](#download)
    - [Dependencies](#dependencies)
        - [External references](#externalrefs)
        - [Preconditions](#preconditions)
    - [Connect through the API](#connect)
    - [Create a new Task](#createtask)
        - [Example create a new Task with a DataTable](#createtaskdatatable)
    - [Get Task](#gettask)
- [Helpers](#helpers)
    - [Serialize DataTable](#serializedt)
    - [Serialize JSON](#serializejson)
    - [Serialize other](#serializeother)
- [Additional hints](#additional)
    - [OnBase Studio](#additionalstudio)

---
<a name="gettingstarted"></a>
## Getting started
<a name="download"></a>
### Download
You can get the latest version in the releases here:\
https://github.com/marchorst/Hyland.Rocket.RPA.ApiHelper/releases/latest
<a name="dependencies"></a>
### Dependencies
<a name="externalrefs"></a>
#### External references
**Newtonsoft.Json (Version: 11.0.2, net452)**\
https://nuget.info/packages/Newtonsoft.Json/11.0.2

**RestSharp (Version: 106.15.0, net452)**\
https://nuget.info/packages/RestSharp/106.15.0

You can use any other version from the dependencies as well, but it is yet tested only with these versions.
<a name="preconditions"></a>
#### Preconditions
Be sure to follow the documentation for direct API Access here:\
<a href="https://docs.hyland.com/RPA/en_US/22/1/RPAP/RPA_Platform.htm" target="_blank">https://docs.hyland.com/RPA/en_US/22/1/RPAP/RPA_Platform.htm</a> \
(Administration->Direct API requests with Postman)
<a name="connect"></a>
### Connect through the API
#### Access token
```
// Initialize the API Connection
var rpaApiObject = new RpaApi("https://your.domain.without.ending.slash/heart", "https://your.domain.without.ending.slash/identity", "AccessToken");
```
Hint: You can generate an "Access token" in your "Hyland RPA Manager Account Settings".
#### Username and password (Deprecated)
```
// Initialize the API Connection
var rpaApiObject = new RpaApi("https://your.domain.without.ending.slash/heart", "https://your.domain.without.ending.slash/identity", "Heart's Client ID", "USERNAME", "PASSWORD");
```
Hint: Heart's Client ID Note: The ID can be found in Heart's appsettings.json file at HeartServer:Swagger:ClientId
<a name="createtask"></a>
### Create a new Task
To create a new task you can either use the Create Method with some parameters or create a new "NewTask" object and use this as your parameter.
<a name="createtaskdatatable"></a>
#### Example create a new Task with a DataTable
```
// Create a dummy DataTable if you want
var dt = new DataTable("Table");
dt.Columns.Add("ColName");
dt.Rows.Add("Test");

// Serialize it with the RpaHelper
var inputData = RpaHelper.SerializeDataTable(dt);

// Create a new Task
// 1.
ITask task = rpaApiObject.Tasks.Create(1, inputData);

// rpaApiObject.TasksRoute() and rpaApiObject.Tasks are the same
// You can create a new task either with the method above or use a "NewTask" Object as parameter.

// 2.
var newTaskObject = new NewTask()
{
    CheckDiversity = true,
    Diversity = "Test",
    InputData = inputData,
    ProcessId = 1,
    Redoable = true,
    Type = RpaTaskType.PRO
};

ITask task2 = rpaApiObject.TasksRoute().Create(newTaskObject);
```
<a name="gettask"></a>    
### Get a Task by TaskID
```
To get a Task by the TaskID you can either use an ITask object or use the TaskID as an integer.
ITask task = rpaApiObject.TasksRoute().Get(task);
```
<a name="helpers"></a>
## Helpers
<a name="serializedt"></a>
### Serialize a DataTable
```
RpaHelper.SerializeDataTable(DataTable table)
```
<a name="serializejson"></a>
### Serialize to JSON
```
RpaHelper.ToJson(object o)
```
<a name="serializeother"></a>
### Serialize any other
```
RpaHelper.Serialize<T>(T obj)
```
<a name="additional"></a>
## Additional hints
<a name="additionalstudio"></a>
### OnBase Studio
When trying to import the Dlls' to the OnBase Studio, you have to be sure that all three Dlls' are in the same folder from where you import them.
