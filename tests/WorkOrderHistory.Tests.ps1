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

$storePath = Join-Path $RepoRoot 'MesDatas\Services\WorkOrderHistoryStore.cs'
$accessPath = Join-Path $RepoRoot 'MesDatas\Utility\AccessHelper.cs'
$form1Path = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'
$form3Path = Join-Path $RepoRoot 'MesDatas\Views\Form3.cs'
$projectPath = Join-Path $RepoRoot 'MesDatas\MesDatas.csproj'

Assert-FileContains $storePath 'MaxRecentOrderCount = 10' 'Work order history must retain 10 recent orders.'
Assert-FileContains $storePath 'ORDER BY id DESC' 'Work order history must use deterministic newest-first ordering.'
Assert-FileContains $storePath 'MaxRecentOrderCount - 1' 'Saving must retain nine other orders before inserting the current order.'
Assert-FileContains $storePath 'string.IsNullOrWhiteSpace(existingOrderNo)' 'Legacy blank orders must not consume a recent-order slot.'
Assert-FileContains $storePath 'ExecuteTransaction(deleteSql, insertSql)' 'History cleanup and insert must share one transaction.'
Assert-FileContains $storePath 'Replace("''", "''''")' 'Work order SQL values must escape apostrophes.'
Assert-FileContains $accessPath 'OleDbTransaction' 'AccessHelper must support an OleDb transaction.'
Assert-FileContains $form3Path 'SaveRecentOrder(orderNo, oper, orderQuantity)' 'Confirming an order must promote it to the newest history entry.'
Assert-FileContains $form3Path 'GetRecentOrders(Global.Instance.LoginMessage.WorkId)' 'The history dropdown must load the current operator recent orders.'
Assert-FileContains $projectPath 'Services\WorkOrderHistoryStore.cs' 'The project must compile WorkOrderHistoryStore.cs.'

$form1Content = [System.IO.File]::ReadAllText($form1Path)
$setOrderIndex = $form1Content.IndexOf('SetOrderMessage();', [StringComparison]::Ordinal)
$startTaskIndex = $form1Content.IndexOf('StartPermanentTask();', [StringComparison]::Ordinal)
if ($setOrderIndex -lt 0 -or $startTaskIndex -lt 0 -or $setOrderIndex -gt $startTaskIndex) {
    throw 'The latest work order must be restored before background tasks start.'
}

Assert-FileContains $form1Path 'new WorkOrderHistoryStore(curDb).GetLatestOrder()' 'Startup must restore the device latest work order.'

Write-Host 'Work order history source checks passed.'
