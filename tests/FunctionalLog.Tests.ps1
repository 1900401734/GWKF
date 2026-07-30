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

function Assert-NotContains {
    param(
        [string]$Content,
        [string]$Text,
        [string]$Message
    )

    if ($Content.Contains($Text)) {
        throw $Message
    }
}

function New-Text {
    param([int[]]$Codes)
    return [string]::Concat(($Codes | ForEach-Object { [char]$_ }))
}

$configPath = Join-Path $RepoRoot 'MesDatas\Log4net.config'
$helperPath = Join-Path $RepoRoot 'MesDatas\Utility\Log4netUtil.cs'
$httpPath = Join-Path $RepoRoot 'MesDatas\Utility\HttpClientUtil.cs'
$formPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'
$writeLogPath = Join-Path $RepoRoot 'MesDatas\Utility\WriteLog.cs'

$config = [System.IO.File]::ReadAllText($configPath)
$helper = [System.IO.File]::ReadAllText($helperPath)
$http = [System.IO.File]::ReadAllText($httpPath)
$form = [System.IO.File]::ReadAllText($formPath)
$writeLog = [System.IO.File]::ReadAllText($writeLogPath)

foreach ($oldAppender in @('DebugLog', 'ErrorLog', 'InfoLog', 'WarnLog')) {
    Assert-NotContains $config $oldAppender "Log4net.config must remove old level appender '$oldAppender'."
}

$mes = New-Text @(0x0044,0x003A,0x005C,0x004B,0x0061,0x0069,0x0046,0x0061,0x004C,0x006F,0x0067,0x0073,0x005C,0x004D,0x0045,0x0053,0x4EA4,0x4E92,0x005C)
$print = New-Text @(0x0044,0x003A,0x005C,0x004B,0x0061,0x0069,0x0046,0x0061,0x004C,0x006F,0x0067,0x0073,0x005C,0x6807,0x7B7E,0x6253,0x5370,0x005C)
$pass = New-Text @(0x0044,0x003A,0x005C,0x004B,0x0061,0x0069,0x0046,0x0061,0x004C,0x006F,0x0067,0x0073,0x005C,0x4EA7,0x54C1,0x8FC7,0x7AD9,0x005C)
$route = New-Text @(0x0044,0x003A,0x005C,0x004B,0x0061,0x0069,0x0046,0x0061,0x004C,0x006F,0x0067,0x0073,0x005C,0x6D41,0x7A0B,0x68C0,0x67E5,0x005C)
$torque = New-Text @(0x0044,0x003A,0x005C,0x004B,0x0061,0x0069,0x0046,0x0061,0x004C,0x006F,0x0067,0x0073,0x005C,0x626D,0x529B,0x68C0,0x6D4B,0x005C)
$exception = New-Text @(0x0044,0x003A,0x005C,0x004B,0x0061,0x0069,0x0046,0x0061,0x004C,0x006F,0x0067,0x0073,0x005C,0x6570,0x636E,0x5F02,0x5E38,0x005C)
$uploadAfterFeedbackCreateText = New-Text @(0x5148,0x53CD,0x9988,0x518D,0x4E0A,0x4F20,0x8BB0,0x5F55,0x5DF2,0x521B,0x5EFA,0xFF0C,0x7B49,0x5F85,0x004D,0x0045,0x0053,0x540E,0x53F0,0x786E,0x8BA4)
$oldOutboxQueueText = New-Text @(0x004D,0x0045,0x0053,0x8FC7,0x7AD9,0x8BB0,0x5F55,0x5DF2,0x5199,0x5165,0x672C,0x5730,0x8865,0x4F20,0x961F,0x5217)
$sendTimeText = New-Text @(0x53D1,0x9001,0x65F6,0x95F4)
$receiveTimeText = New-Text @(0x63A5,0x6536,0x65F6,0x95F4)
$mesPrimaryKeyConflictText = New-Text @(0x004D,0x0045,0x0053,0x6570,0x636E,0x5E93,0x4E3B,0x952E,0x51B2,0x7A81)
$duplicateKeyValueText = New-Text @(0x91CD,0x590D,0x952E,0x503C)

foreach ($path in @($mes, $print, $pass, $route, $torque, $exception)) {
    Assert-Contains $config $path "Log4net.config must contain function log path '$path'."
}

foreach ($method in @('LogMesInteraction', 'LogLabelPrint', 'LogProductPass', 'LogRouteCheck', 'LogTorque', 'LogDataException')) {
    Assert-Contains $helper $method "Log4netHelper must expose $method."
}

Assert-Contains $helper 'enum LogArea' 'Log4netHelper must define LogArea enum.'
Assert-Contains $helper 'FormatFlowLog' 'Log4netHelper must format logs as time-based Chinese flow lines.'
Assert-Contains $helper 'LogMesInteractionBlock' 'MES interaction raw blocks must bypass generic field formatting.'
Assert-Contains $helper 'MES_SYNC_CONFIRMED_PASS' 'Log4netHelper must format normal synchronous MES PASS logs.'
Assert-Contains $helper 'MES_SYNC_CONFIRMED_FAIL' 'Log4netHelper must format normal synchronous MES FAIL logs.'
Assert-Contains $helper 'PRINT_BLOCKED_BY_WEIGHT' 'Log4netHelper must format local print-block logs.'
Assert-Contains $helper $uploadAfterFeedbackCreateText 'Outbox create text must be limited to upload-after-feedback wording.'
Assert-NotContains $helper 'FormatKeyValueLog' 'Log4netHelper must not emit old key-value log body formatting.'
Assert-NotContains $helper 'level=' 'Log body must not include level= fields.'
Assert-NotContains $helper 'area=' 'Log body must not include area= fields.'
Assert-NotContains $helper 'action=' 'Log body must not include action= fields.'
Assert-NotContains $helper $oldOutboxQueueText 'Old outbox queue wording must not appear in log helper.'
Assert-NotContains $writeLog 'Log4netHelper.Debug' 'WriteLog.AppendToComponent must not write UI logs to Debug files.'

$messageOnlyPatternCount = [regex]::Matches($config, '<conversionPattern value="%m%n" />').Count
if ($messageOnlyPatternCount -ne 6) {
    throw "All function appenders must use message-only conversionPattern. Actual count: $messageOnlyPatternCount"
}

Assert-NotContains $config '%d{yyyy-MM-dd HH:mm:ss.fff} %m%n' 'Log4net layout must not prepend a second timestamp.'
Assert-Contains $config $mes 'MES interaction logs must stay in the single date-file folder.'
Assert-Contains $config '<param name="DatePattern" value="yyyy-MM-dd&quot;.log&quot;" />' 'MES interaction logs must keep one date file per day.'

foreach ($text in @('RequestId', 'Function', 'SN', $sendTimeText, $receiveTimeText, 'PayloadHash')) {
    Assert-Contains $http $text "MES interaction logs must include $text."
}

foreach ($method in @('WriteMesRequestLog', 'WriteMesResponseLog', 'WriteMesExceptionLog', 'BuildMesInteractionBlock', 'FormatMesXmlForLog', 'TryFormatEmbeddedJson', 'ExtractMesLogContext')) {
    Assert-Contains $http $method "HttpClientUtil must implement MES log helper $method."
}

Assert-Contains $http 'Log4netHelper.LogMesInteractionBlock' 'MES raw request/response blocks must be written as raw log blocks.'
Assert-NotContains $http 'Log4netHelper.LogMesInteraction("SEND"' 'MES request payload must not go through generic field formatting.'
Assert-NotContains $http 'Log4netHelper.LogMesInteraction("RECEIVE"' 'MES response payload must not go through generic field formatting.'
Assert-NotContains $http 'Log4netHelper.LogMesInteraction("REQUEST_ERROR"' 'MES exception payload must not go through generic field formatting.'
Assert-NotContains $http '{ "payload", requestXml }' 'Raw MES request XML must not be stored as a generic payload field.'
Assert-NotContains $http '{ "payload", recvXml }' 'Raw MES response XML must not be stored as a generic payload field.'
Assert-NotContains $http 'SaveResultTransientRetryCount' 'SAVERESULT is a write operation and must not be auto-retried by HttpClientUtil.'
Assert-NotContains $http 'maxAttemptCount = IsSaveResultFunction(function)' 'SAVERESULT must be sent once per explicit operator/background action.'
Assert-NotContains $http 'Thread.Sleep(attempt' 'HttpClientUtil must not delay and blindly retry SAVERESULT.'

Assert-Contains $form $mesPrimaryKeyConflictText 'Product pass logs must classify MES primary-key conflicts explicitly.'
Assert-Contains $form 'PK_rt_PrdSNTrace_MOInput' 'Primary-key conflict detection must include the MES table key.'
Assert-Contains $form 'PRIMARY KEY' 'Primary-key conflict detection must include SQL PRIMARY KEY text.'
Assert-Contains $form $duplicateKeyValueText 'Primary-key conflict detection must capture duplicate key text.'

$productionFiles = Get-ChildItem -Path (Join-Path $RepoRoot 'MesDatas') -Recurse -Filter '*.cs' |
    Where-Object { $_.FullName -notlike '*\Utility\Log4netUtil.cs' }

$forbiddenPattern = 'Log4netHelper\.(Info|Debug|Warn|Fatal)\s*\('
foreach ($file in $productionFiles) {
    $content = [System.IO.File]::ReadAllText($file.FullName)
    if ([regex]::IsMatch($content, $forbiddenPattern)) {
        throw "Production file still calls level logger: $($file.FullName)"
    }
}

Write-Host 'Functional log source checks passed.'
