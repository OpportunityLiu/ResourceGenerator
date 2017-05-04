param($installPath, $toolsPath, $package, $project)

if (Get-Module 'ResourceGenerator')
{
    Remove-Module 'ResourceGenerator'
}

Import-Module (Join-Path $PSScriptRoot 'ResourceGenerator.psd1') -DisableNameChecking
Write-Host 'Model installation finished, start regenerating resources classes'
Write-Host
Convert-Resource
