param($installPath, $toolsPath, $package, $project)

if (Get-Module 'ResourceGenerator')
{
    Remove-Module 'ResourceGenerator'
}

Import-Module (Join-Path $PSScriptRoot 'ResourceGenerator.psd1') -DisableNameChecking
Write-Host 'Model installation finished, use `New-Config` to create .resgenconfig files, use `Convert-Resource` to generate csharp files.'
Write-Host
Convert-Resource
