# ResourceGenerator
A tool to generate classes for UWP string resources.

[![NuGet](https://img.shields.io/nuget/v/Opportunity.ResourceGenerator.svg)](https://www.nuget.org/packages/Opportunity.ResourceGenerator/)
[![Build status](https://ci.appveyor.com/api/projects/status/m9bn4ub78r62aw1e?svg=true)](https://ci.appveyor.com/project/OpportunityLiu/resourcegenerator)

## How-To
1.  **Install package**    
    To install this package, execute following command in 
    [package manager console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console).
    ```powershell
    Install-Package Opportunity.ResourceGenerator
    ```
1.  **Create config file**    
    To create default config file of ResourceGenerator, execute following command.  
    ```powershell
    New-Config Resources/Strings
    ```
    You can also specify paths other than `Resources/Strings`.   
    Use `-Project <ProjectName>` to create config file in project other than default project.    
1.  **Edit config file**      
    After step 2, you'll get a `.resgenconfig` file with following content.  
    ```js
    {
      "`$schema": "https://raw.githubusercontent.com/OpportunityLiu/ResourceGenerator/master/resgenconfig.json?version=1.3.2",
      // Path for resource files (*.resw & *.resjson).
      // Default value is "/Strings".
      "ResourcePath": "/Strings",
  
      // Default language of resources, will be detected automatically if unset.
      //"SourceLanguagePath": "en-Us",
  
      // Namespace for resource visitor class.
      // Default value is "<ProjectDefaultNamespace>".
      //"LocalizedStringsNamespace": "MyNamespace",
  
      // Namespace for resource visitor interfaces.   
      // Default value is "<ProjectDefaultNamespace>.ResourceInfo".
      //"InterfacesNamespace": "MyNamespace.ResourceInfo",
  
      // Modifier for resource visitor class and interfaces.
      "Modifier": "internal",
  
      // Specifies whether this project is the default project or not.
      // Determines if it is necessary to contains project name in the resource path.
      "IsDefaultProject": true,

      // Regard resource strings whose name starts with '$' as format string.
      // Default value is false.
      //"IsFormatStringEnabled": true,
  
      // Specifies whether the tool generates code that is debuggable.
      "DebugGeneratedCode": false
    }
    ```  
    Edit this file to control properties of generated classes.      
1.  **Generate resource class**    
    Run following command to generate resource class.  
    ```powershell
    Convert-Resource -Project <ProjectName>
    ```   
    To generate resource classes in all projects, run `Convert-Resource` without arguments.    

If you edited your resource file (`.resw` & `.resjson`), re-generate resource classes as the last step.

## Features
Takes following `.resjson` file as an example:
```js
//File `Resources.resjosn`:
{
  "AppName": "TestName",
  "ContentTextBox": {
    "Header": "Header",
    "Text": "Content",
    "ToolTipService/ToolTip": "A simple text box."
  },
  "$FileNotFound": "Line {line:g}: Can not find file with name \"{name}\" in \"{path}\""
}
```

1.  **Nested resource strings** (`.` in `.resw` file or `/` in `.resjson` file)    
    For example, you can visit the tool tip with expression `Strings.Resources.ContentTextBox.ToolTipService.ToolTip`.
1.  **Format resource strings** (resource strings whose name starts with a `$`)  
    To enable this function, you should set `IsFormatStringEnabled` to `true` in `.resgenconfig` file.  
    You can find a generated function `string Strings.Resources.FileNotFound(object line, object name, object path)` used for format strings.
1.  **Dynamic visit support**    
    You should use a pair of parentheses to end visiting with a string result.  
    Dynamic version of the first example look like following:  
    ```cs
    dynamic resources = Strings.Resources;
    string tooltip1 = (string)resources.ContentTextBox.ToolTipService.ToolTip();
    string tooltip2 = (string)resources.ContentTextBox["ToolTipService"].ToolTip();
    string tooltip3 = (string)resources.ContentTextBox["ToolTipService/ToolTip"]();
    ```
