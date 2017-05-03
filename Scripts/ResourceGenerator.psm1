$ErrorActionPreference = 'Stop'

Register-TabExpansion Create-Config @{
    Project = { GetProjects }
    Name = { "LocalizedStrings", "Strings" }
}
Register-TabExpansion Generate-Resource @{
    Project = { GetProjects }
}


function Create-Config
{
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

function Generate-Resource
{
    [CmdletBinding(PositionalBinding = $false)]
    param(
        [Parameter(Position = 0, Mandatory = $false)]
        [string] $Project)
    if(!$Project)
    {
        GetProjects | ForEach-Object{Generate-Resource -Project $_}
    }
    else
    {
        $dteProject = GetProject($Project)
        $tool = ToolPath
        $configFiles = GetConfigFiles($dteProject)
        $toolArgs = $dteProject.FullName;
        Write-Host "==== Project" $Project "===="
        if(!$configFiles)
        {
            Write-Host "No config files found, skipped."
            return
        }
        Start-Process -FilePath $tool -ArgumentList ($toolArgs +" " +( $configFiles -join " ")) -NoNewWindow -Wait
        $i = 1
        $configFiles | ForEach-Object{
            $file = ClassFileName($_)
            Write-Host $i ">" $_ 
            Write-Host $i ">" "`t=>" $file
            $dteProject.ProjectItems.AddFromFile($file) | Out-Null
            $dte.ItemOperations.OpenFile($file)| Out-Null
            $i = $i + 1
        }
        Write-Host ($i - 1) "file(s) generated."
    }
}

function ClassFileName($configFileName)
{
    $className = [System.IO.Path]::GetFileNameWithoutExtension($configFileName)
    return [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($configFileName), $className + ".cs");
}

function ToolPath
{
    return Join-Path $PSScriptRoot "Opportunity.ResourceGenerator.Generator.exe"
}

function GetFile
{
    return @"
    {
      // TODO: 修改相关配置。

      // 检索 resw 的路径，默认为 "Strings"。
      "ResourcePath": "/Strings",

      // 检索 resw 并生成注释时使用的语言相对 ResourcePath 的路径。
      //"SourceLanguagePath": "en-Us",

      // 生成辅助类的命名空间，默认使用 "<ProjectDefaultNamespace>"。
      //"LocalizedStringsNamespace": "MyNamespace",

      // 生成辅助类的相关接口的命名空间，默认使用 "<ProjectDefaultNamespace>.<ProjectDefaultNamespace>_ResourceInfo"。
      //"InterfacesNamespace": "MyNamespace.MyNamespace_ResourceInfo",

      // 生成辅助类的修饰符
      "Modifier": "internal",

      // 是否为默认工程，决定是否需要显式定义资源路径。
      "IsDefaultProject": true,

      // 是否调试生成的代码。
      "DebugGeneratedCode": false
    }
"@
}

function GetProjects
{
    return Get-Project -All | ForEach-Object{ if(IsUWP($_)){ $_.ProjectName} }
}

function GetConfigFiles($dteProject)
{
    $path = Split-Path $dteProject.FullName -Parent
    Get-ChildItem -Path $path -Filter "*.resgenconfig" -Recurse | ForEach-Object{$_.FullName}
}

function GetProjectTypes($project)
{
    $solution = Get-VSService 'Microsoft.VisualStudio.Shell.Interop.SVsSolution' 'Microsoft.VisualStudio.Shell.Interop.IVsSolution'
    $hierarchy = $null
    $hr = $solution.GetProjectOfUniqueName($project.UniqueName, [ref] $hierarchy)
    [Runtime.InteropServices.Marshal]::ThrowExceptionForHR($hr)

    $aggregatableProject = Get-Interface $hierarchy 'Microsoft.VisualStudio.Shell.Interop.IVsAggregatableProject'
    if (!$aggregatableProject)
    {
        return $project.Kind
    }

    $projectTypeGuidsString = $null
    $hr = $aggregatableProject.GetAggregateProjectTypeGuids([ref] $projectTypeGuidsString)
    [Runtime.InteropServices.Marshal]::ThrowExceptionForHR($hr)

    return $projectTypeGuidsString.Split(';')
}

function IsUWP($project)
{
    $types = GetProjectTypes $project

    return $types -contains '{A5A43C5B-DE2A-4C0C-9213-0A381AF9435A}'
}

function GetProject($projectName)
{
    if (!$projectName)
    {
        return Get-Project
    }

    return Get-Project $projectName
}