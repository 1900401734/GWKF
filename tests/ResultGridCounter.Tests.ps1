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

function Assert-FileNotContains {
    param(
        [string]$Path,
        [string]$Text,
        [string]$Message
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "File does not exist: $Path"
    }

    $content = [System.IO.File]::ReadAllText($Path)
    if ($content.Contains($Text)) {
        throw $Message
    }
}

$formCode = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'

Assert-FileNotContains $formCode 'nameof(uploadManagerEntity.Name)' 'Result grid counters must not use nameof(uploadManagerEntity.Name), because it always returns "Name".'
Assert-FileContains $formCode 'uploadManagerEntity.Name.ToString()' 'Result grid counters must use the actual process name as the counter key.'
Assert-FileNotContains $formCode 'Rows.RemoveAt(dgvResult1.Rows.Count - 1)' 'Result grid row trimming must not use dgvResult1 when trimming other result grids.'
Assert-FileContains $formCode 'gridView.Rows.RemoveAt(gridView.Rows.Count - 1)' 'Result grid row trimming must use the current gridView row count.'

Write-Host 'ResultGridCounter source checks passed.'
