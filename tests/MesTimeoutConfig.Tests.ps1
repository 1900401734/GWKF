param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'

function Assert-FileContains {
    param(
        [string]$Path,
        [string]$Text,
        [string]$Message
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "File does not exist: $Path"
    }

    $content = [System.IO.File]::ReadAllText($Path)
    if (-not $content.Contains($Text)) {
        throw $Message
    }
}

$systemInfoPath = Join-Path $RepoRoot 'MesDatas\DataAccess\SystemInfo.cs'
$systemInfoServerPath = Join-Path $RepoRoot 'MesDatas\DataAccess\SystemInfoServer.cs'
$formPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'
$designerPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.Designer.cs'
$httpPath = Join-Path $RepoRoot 'MesDatas\Utility\HttpClientUtil.cs'

Assert-FileContains $systemInfoPath 'MesSaveResultTimeoutSeconds' 'SystemInfo must persist MES SAVERESULT timeout seconds.'
Assert-FileContains $systemInfoServerPath 'EnsureMesSaveResultTimeoutColumn' 'SystemInfoServer must add MES timeout column for existing Access databases.'
Assert-FileContains $designerPath 'txtMesSaveResultTimeoutSeconds' 'Production config UI must expose MES timeout textbox.'
Assert-FileContains $designerPath 'labelMesSaveResultTimeoutSeconds' 'Production config UI must label MES timeout clearly.'
Assert-FileContains $formPath 'txtMesSaveResultTimeoutSeconds.Text = systemInfo.MesSaveResultTimeoutSeconds' 'Production config load must populate MES timeout.'
Assert-FileContains $formPath 'systemInfo.MesSaveResultTimeoutSeconds = NormalizeMesSaveResultTimeoutSeconds' 'Production config save must validate and persist MES timeout.'
Assert-FileContains $formPath 'HttpClientUtil.ConfigureSaveResultTimeoutSeconds' 'Saving production config must update runtime MES timeout.'
Assert-FileContains $httpPath 'ConfigureSaveResultTimeoutSeconds' 'HttpClientUtil must expose runtime MES timeout configuration.'
Assert-FileContains $httpPath 'GetSaveResultTimeoutSeconds' 'HttpClientUtil must read SAVERESULT timeout through a helper.'
Assert-FileContains $httpPath 'DefaultSaveResultTimeoutSeconds' 'HttpClientUtil must keep 30 seconds as the fallback default.'

Write-Host 'MES timeout config source checks passed.'
