$ErrorActionPreference = 'Stop'

Register-TabExpansion New-Config @{
    Project = { GetProjects }
    Name = { "LocalizedStrings", "Strings", "Strings/LocalizedStrings", "Resources/Strings" }
}
Register-TabExpansion Convert-Resource @{
    Project = { GetProjects }
}


function New-Config {
    [CmdletBinding(PositionalBinding = $false)]
    param(
        [Parameter(Position = 0, Mandatory = $true)]
        [string] $Name,
        [string] $Project)
    $dteProject = GetProject($Project)
    $path = Split-Path $dteProject.FullName -Parent
    $file = Join-Path $path ($Name + ".resgenconfig")
    GetFile | Out-File -FilePath $file
    $dteProject.ProjectItems.AddFromFile($file) | Out-Null
    $dte.ItemOperations.OpenFile($file)| Out-Null
}


function Convert-Resource {
    [CmdletBinding(PositionalBinding = $false)]
    param(
        [Parameter(Position = 0, Mandatory = $false)]
        [string] $Project)
    if (!$Project) {
        GetProjects | ForEach-Object { Convert-Resource -Project $_ }
    }
    else {
        $dteProject = GetProject($Project)
        $tool = ToolPath
        $configFiles = GetConfigFiles($dteProject)
        $toolArgs = $dteProject.FullName;
        Write-Host "============= Project $Project ============="
        if (!$configFiles) {
            Write-Host "No config files found, skipped."
            Write-Host
            return
        }
        $p = Start-Process -FilePath $tool -ArgumentList "`"$toolArgs`"" -WindowStyle Hidden -Wait -PassThru
        if($p.ExitCode -ne 0){
            Write-Warning "Failed to generate resources for this project (ExitCode $($p.ExitCode))" 
            Write-Host 
            return
        }
        $i = 1
        $configFiles | ForEach-Object {
            $file = ClassFileName($_)
            Write-Host $i ">" $_ 
            Write-Host $i ">" "`t=>" $file
            $dteProject.ProjectItems.AddFromFile($file) | Out-Null
            $i = $i + 1
        }
        if ($i -eq 2) {
            Write-Host "1 file generated."
        }
        else {
            Write-Host ($i - 1) "file(s) generated."
        }
        Write-Host
    }
}

function ClassFileName($configFileName) {
    $className = [System.IO.Path]::GetFileNameWithoutExtension($configFileName)
    return [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($configFileName), $className + ".g.cs");
}

function ToolPath {
    return Join-Path $PSScriptRoot "Opportunity.ResourceGenerator.Generator.exe"
}

function GetFile {
    return @"
{
  "`$schema": "https://raw.githubusercontent.com/OpportunityLiu/ResourceGenerator/master/resgenconfig.json?version=1.3.9",
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

  // Format function used for format strings, must be replaceble with string.Format.
  //"FormatStringFunction": "string.Format",

  // Excluded resource paths.
  //"Exclude": [],

  // Included resource paths, has higher priority than Exclude.
  //"Include": [],
  
  // Specifies whether the tool generates code that is debuggable.
  "DebugGeneratedCode": false
}
"@
}

function GetProjects {
    return Get-Project -All | ForEach-Object { if (IsUWP($_)) { $_.ProjectName} }
}

function GetConfigFiles($dteProject) {
    $path = Split-Path $dteProject.FullName -Parent
    Get-ChildItem -Path $path -Filter "*.resgenconfig" -Recurse | ForEach-Object {$_.FullName}
}

function GetProjectTypes($project) {
    $solution = Get-VSService 'Microsoft.VisualStudio.Shell.Interop.SVsSolution' 'Microsoft.VisualStudio.Shell.Interop.IVsSolution'
    $hierarchy = $null
    $hr = $solution.GetProjectOfUniqueName($project.UniqueName, [ref] $hierarchy)
    [Runtime.InteropServices.Marshal]::ThrowExceptionForHR($hr)

    $aggregatableProject = Get-Interface $hierarchy 'Microsoft.VisualStudio.Shell.Interop.IVsAggregatableProject'
    if (!$aggregatableProject) {
        return $project.Kind
    }

    $projectTypeGuidsString = $null
    $hr = $aggregatableProject.GetAggregateProjectTypeGuids([ref] $projectTypeGuidsString)
    [Runtime.InteropServices.Marshal]::ThrowExceptionForHR($hr)

    return $projectTypeGuidsString.Split(';')
}

function IsUWP($project) {
    $types = GetProjectTypes $project

    return $types -contains '{A5A43C5B-DE2A-4C0C-9213-0A381AF9435A}'
}

function GetProject($projectName) {
    if (!$projectName) {
        return Get-Project
    }

    return Get-Project $projectName
}