# Welcome to Hyland Rocket RPA ApiHelper
With this dll you are able to connect to the Hyland RPA API.
## Dependencies

| Name | Version | Target Framework | Download
|--|--|--|--|
| Newtonsoft.Json | 11.0.2 | net452 |https://nuget.info/packages/Newtonsoft.Json/11.0.2 |
| RestSharp | 106.15.0 | net452 | https://nuget.info/packages/RestSharp/106.15.0 |

You can use any other version from the dependencies as well, but it is yet tested only with these versions.

## Connect throw the API

    // Initialize the API Connection
    var rpaApiObject = new RpaApi("https://your.domain.without.ending.slash", "CLIENTID", "USERNAME", "PASSWORD");

## Create a new Task
To create a new task you can either use the Create Method with some parameters or create a new "NewTask" object and use this as your parameter.

### Example create a new task with a DataTable
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
    	checkDiversity = true,
    	diversity = "Test",
    	inputData = inputData,
    	processId = 1,
    	redoable = true,
    	type = RpaTaskType.PRO
    };
    
    ITask task2 = rpaApiObject.TasksRoute().Create(newTaskObject);
## Get a Task by TaskID
To get a Task by the TaskID you can either use an ITask object or use the TaskID as an integer.

    ITask task = rpaApiObject.TasksRoute().Get(task);
