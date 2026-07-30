param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'

function Assert-Contains {
    param(
        [string]$Content,
        [string]$Text,
        [string]$Message
    )

    if (-not $Content.Contains($Text)) {
        throw $Message
    }
}

function Assert-RegexMatch {
    param(
        [string]$Content,
        [string]$Pattern,
        [string]$Message
    )

    if (-not [regex]::IsMatch($Content, $Pattern)) {
        throw $Message
    }
}

$formPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'
$designerPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.Designer.cs'
$form = [System.IO.File]::ReadAllText($formPath)
$designer = [System.IO.File]::ReadAllText($designerPath)

Assert-Contains $designer '"Int16"' 'PLC test type list must include Int16.'
Assert-Contains $designer '"Int32"' 'PLC test type list must include Int32.'
Assert-Contains $designer '"String"' 'PLC test type list must include String.'
Assert-Contains $designer 'this.cmbType.SelectedIndex = 0;' 'PLC test must default to Int16.'
Assert-Contains $designer 'this.btnRead.Click += new System.EventHandler(this.btnRead_Click);' 'PLC test read button must be wired.'
Assert-Contains $designer 'this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);' 'PLC test type changes must update length input state.'

Assert-Contains $form '_readWriteNet.ReadInt16(address)' 'PLC test must read Int16 values.'
Assert-Contains $form '_readWriteNet.ReadInt32(address)' 'PLC test must read Int32 values.'
Assert-Contains $form '_readWriteNet.ReadString(address, stringLength)' 'PLC test must read a String with the requested length.'
Assert-Contains $form 'ushort.TryParse(txtPlcTestStringLength.Text.Trim(), out ushort stringLength)' 'String length must be constrained to UInt16 input.'
Assert-Contains $form 'stringLength == 0' 'String length zero must be rejected.'
Assert-Contains $form 'CodeNum.CleanString(stringResult.Content)' 'PLC String results must reuse the existing cleanup rule.'
Assert-Contains $form '!isPlcConnected || _readWriteNet == null' 'PLC test must reject reads while disconnected.'
Assert-Contains $form 'string.IsNullOrWhiteSpace(address)' 'PLC test must reject an empty address.'
Assert-Contains $form 'ReportPlcTestReadFailure(address, dataType' 'PLC test must route failed reads through the failure reporter.'
Assert-Contains $form 'if (!int16Result.IsSuccess)' 'PLC test must verify Int16 read success before using Content.'
Assert-Contains $form 'if (!int32Result.IsSuccess)' 'PLC test must verify Int32 read success before using Content.'
Assert-Contains $form 'if (!stringResult.IsSuccess)' 'PLC test must verify String read success before using Content.'
Assert-RegexMatch $form 'private void ReportPlcTestReadFailure\([\s\S]*?PLC_TEST_READ_FAILED' 'PLC test read failures must be logged.'

Write-Host 'PLC test read source checks passed.'
