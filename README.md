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
    
1.  **Create config file**   
    To create default config file of ResourceGenerator, execute following command.  
    ```powershell
    New-Config Resources/Strings
    ``` 
    You can also specify paths other than `Resources/Strings`.   
    Use `-Project <ProjectName>` to create config file in project other than default project.
    
1.  **Edit config file**      
    After step 2, you'll get a `.resgenconfig` file with following content.
    ```js
    {
      // Path for resource files (*.resw & *.resjson).
      // Default value is "/Strings".
      "ResourcePath": "/Strings",
      
      // Default language of resources, will be detected automatically if unset.
      //"SourceLanguagePath": "en-Us",
      
      // Namespace for resource visitor class.
      // Default value is "<ProjectDefaultNamespace>".
      //"LocalizedStringsNamespace": "MyNamespace",
      
      // Namespace for resource visitor interfaces.
      // Default value is "<ProjectDefaultNamespace>.<ProjectDefaultNamespace>_ResourceInfo".
      //"InterfacesNamespace": "MyNamespace.MyNamespace_ResourceInfo",
      
      // Modifier for resource visitor class and interfaces.
      "Modifier": "internal",
      
      // Specifies whether this project is the default project or not.
      // Determines if it is necessary to contains project name in the resource path.
      "IsDefaultProject": true,
      
      // Specifies whether the tool generates code that is debuggable.
      "DebugGeneratedCode": false
    }
    ```
    Edit this file to control properties of generated classes.
1.  **Generate resource class**   
    Run following command to generate resource class.
    ```powershell
    Convert-Resource -Project <ProjectName>
    ```
    To generate resource classes in all projects, run `Convert-Resource` without arguments.

If you edited your resource file (`.resw` & `.resjson`), re-generate resource classes as the last step.
    
