param($installPath, $toolsPath, $package, $project)

if (Get-Module 'ResourceGenerator')
{
    Remove-Module 'ResourceGenerator'
}

Import-Module (Join-Path $PSScriptRoot 'ResourceGenerator.psd1') -DisableNameChecking
