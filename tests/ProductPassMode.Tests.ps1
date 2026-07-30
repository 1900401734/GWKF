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
        throw "文件不存在: $Path"
    }

    $content = [System.IO.File]::ReadAllText($Path)
    if (-not $content.Contains($Text)) {
        throw $Message
    }
}

function Assert-FileNotContains {
    param(
        [string]$Path,
        [string]$Text,
        [string]$Message
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "文件不存在: $Path"
    }

    $content = [System.IO.File]::ReadAllText($Path)
    if ($content.Contains($Text)) {
        throw $Message
    }
}

$formCode = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'
$formDesigner = Join-Path $RepoRoot 'MesDatas\Views\Form1.Designer.cs'
$snapshotCode = Join-Path $RepoRoot 'MesDatas\Models\ProductUploadSnapshot.cs'
$projectFile = Join-Path $RepoRoot 'MesDatas\MesDatas.csproj'

$modeText = [string]::Concat([char]0x5148, [char]0x53CD, [char]0x9988, [char]0x518D, [char]0x4E0A, [char]0x4F20)
$labelText = [string]::Concat([char]0x8FC7, [char]0x7AD9, [char]0x6A21, [char]0x5F0F, [char]0xFF1A)
$oldOutboxQueueText = [string]::Concat(
    [char]0x004D, [char]0x0045, [char]0x0053,
    [char]0x8FC7, [char]0x7AD9, [char]0x8BB0, [char]0x5F55,
    [char]0x5DF2, [char]0x5199, [char]0x5165,
    [char]0x672C, [char]0x5730,
    [char]0x8865, [char]0x4F20, [char]0x961F, [char]0x5217
)

Assert-FileContains $formDesigner """$modeText""" 'ComboBox must include upload-after-feedback mode.'
Assert-FileContains $formDesigner "this.label21.Text = ""$labelText"";" 'Label text must be changed to product pass mode.'
Assert-FileContains $formCode 'ProductModeUploadAfterFeedback' 'Form1 must define upload-after-feedback mode constant.'
Assert-FileContains $formCode 'StartMesUploadAfterFeedbackAsync' 'Form1 must include a background MES upload entry point.'
Assert-FileContains $formCode 'handleMesFailure' 'SendResultToMes must allow background mode to skip PLC NG feedback.'
Assert-FileContains $formCode 'bool useOutboxRecord = !handleMesFailure && mesOutboxRecord != null;' 'SendResultToMes must only use outbox records in background upload mode.'
Assert-FileContains $formCode 'LogMesSyncResult' 'Normal synchronous pass must log MES results without creating outbox records.'
Assert-FileNotContains $formCode 'mesOutboxRecord = mesOutboxRecord ?? CreateMesOutboxRecord' 'SendResultToMes must not create outbox records unconditionally.'
Assert-FileNotContains $formCode $oldOutboxQueueText 'Normal logs must not contain old local outbox queue wording.'
Assert-FileContains $snapshotCode 'class ProductUploadSnapshot' 'ProductUploadSnapshot must be added for background upload copies.'
Assert-FileContains $projectFile 'Models\ProductUploadSnapshot.cs' 'Project file must include ProductUploadSnapshot.cs.'

$formContent = [System.IO.File]::ReadAllText($formCode)
if (-not [regex]::IsMatch($formContent, 'IsUploadAfterFeedbackMode\(\)[\s\S]{0,1600}CreateMesOutboxRecord')) {
    throw 'CreateMesOutboxRecord must be created only inside the upload-after-feedback branch.'
}

$createOutboxCallCount = [regex]::Matches($formContent, 'CreateMesOutboxRecord\s*\(').Count
if ($createOutboxCallCount -ne 2) {
    throw "CreateMesOutboxRecord must have exactly one call site plus its method definition. Actual count: $createOutboxCallCount"
}

Write-Host 'ProductPassMode source checks passed.'
