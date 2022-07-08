
# Hyland Rocket RPA API Trigger Example

This is a (not so) simple example of how to trigger a new task from a UnityScript and anywhere you can use them within OnBase (like workflows)

**Consider this script as a work in progress!**

## The OnBaseRocketUnityScriptTrigger
Find the file here:
https://github.com/marchorst/Hyland.Rocket.RPA.ApiHelper/example/OnBaseRocketUnityScriptTrigger.cs

## How to use it
### Quickstart
Create a new IClientWorkflowScript (or any other you want to use, just make sure you adapt the interface) and name it 'OnBaseRocketUnityScriptTrigger' - after that you can just copy the content into the OnBase Studio Editor, add some references and that's it.

### A bit more details
![1 Step]https://raw.githubusercontent.com/marchorst/Hyland.Rocket.RPA.ApiHelper/main/example/1.JPG)
First click on "Scripts+"

![2 Step]https://raw.githubusercontent.com/marchorst/Hyland.Rocket.RPA.ApiHelper/main/example/2.JPG)
Set the correct name 'OnBaseRocketUnityScriptTrigger'

![3 Step]https://raw.githubusercontent.com/marchorst/Hyland.Rocket.RPA.ApiHelper/main/example/3.JPG)
Select the type of the script 'IClientWorkflowScript' - The script is ready for the type IClientWorkflowScript, but can be quickly adapted in the script for other types as well (As long as you know what you are doing and why ;-) )

![4 Step]https://raw.githubusercontent.com/marchorst/Hyland.Rocket.RPA.ApiHelper/main/example/4.JPG)
Edit the parameters in the first section.

![5 Step]https://raw.githubusercontent.com/marchorst/Hyland.Rocket.RPA.ApiHelper/main/example/5.JPG)
Be sure to include all those references.

For more information about embedding the DLL, see the main readme.