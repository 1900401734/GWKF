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
$tracePath = Join-Path $RepoRoot 'MesDatas\Utility\ProductPassTraceContext.cs'
$routeTracePath = Join-Path $RepoRoot 'MesDatas\Utility\RouteCheckTraceContext.cs'

$config = [System.IO.File]::ReadAllText($configPath)
$helper = [System.IO.File]::ReadAllText($helperPath)
$http = [System.IO.File]::ReadAllText($httpPath)
$form = [System.IO.File]::ReadAllText($formPath)
$writeLog = [System.IO.File]::ReadAllText($writeLogPath)
$trace = [System.IO.File]::ReadAllText($tracePath)
$routeTrace = [System.IO.File]::ReadAllText($routeTracePath)

foreach ($oldAppender in @('DebugLog', 'ErrorLog', 'InfoLog', 'WarnLog')) {
    Assert-NotContains $config $oldAppender "Log4net.config must remove old level appender '$oldAppender'."
}

$mes = New-Text @(0x0044,0x003A,0x005C,0x004B,0x0061,0x0069,0x0046,0x0061,0x004C,0x006F,0x0067,0x0073,0x005C,0x004D,0x0045,0x0053,0x4EA4,0x4E92,0x005C)
$print = New-Text @(0x0044,0x003A,0x005C,0x004B,0x0061,0x0069,0x0046,0x0061,0x004C,0x006F,0x0067,0x0073,0x005C,0x6807,0x7B7E,0x6253,0x5370,0x005C)
$pass = New-Text @(0x0044,0x003A,0x005C,0x004B,0x0061,0x0069,0x0046,0x0061,0x004C,0x006F,0x0067,0x0073,0x005C,0x4EA7,0x54C1,0x8FC7,0x7AD9,0x005C)
$route = New-Text @(0x0044,0x003A,0x005C,0x004B,0x0061,0x0069,0x0046,0x0061,0x004C,0x006F,0x0067,0x0073,0x005C,0x6D41,0x7A0B,0x68C0,0x67E5,0x005C)
$torque = New-Text @(0x0044,0x003A,0x005C,0x004B,0x0061,0x0069,0x0046,0x0061,0x004C,0x006F,0x0067,0x0073,0x005C,0x626D,0x529B,0x68C0,0x6D4B,0x005C)`r`n$scanAssyTorque = $torque + 'Scan_ASSY\'`r`n$screwBaTorque = $torque + 'Screw_BA\'
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

Assert-Contains $helper 'LogRouteCheckLine(string fullLine)' 'Route check logs must expose a raw full-line writer.'
Assert-Contains $helper 'LogTorqueLine(ProcessName processName, string fullLine)' 'Torque logs must expose a station-aware raw full-line writer.'
Assert-Contains $helper 'ScanAssyTorqueLogger.Info(fullLine);' 'Scan_ASSY torque lines must route to the Scan_ASSY logger.'
Assert-Contains $helper 'ScrewBaTorqueLogger.Info(fullLine);' 'Screw_BA torque lines must route to the Screw_BA logger.'
Assert-Contains $helper 'TorqueLogger.Info(fullLine);' 'Torque lines without a station must retain the shared logger fallback.'
Assert-Contains $config $scanAssyTorque 'Scan_ASSY torque logs must use their own folder.'
Assert-Contains $config $screwBaTorque 'Screw_BA torque logs must use their own folder.'
Assert-Contains $form 'string fullLine = $"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {msg}";' 'Torque UI and file logs must share one complete timestamped line.'
Assert-Contains $form 'Log4netHelper.LogTorqueLine(processName, fullLine);' 'Torque local logs must reuse the complete UI line.'
Assert-Contains $form 'rtbASSYLog.AppendRaw(fullLine);' 'Scan_ASSY UI must append the complete line without a second timestamp.'
Assert-Contains $form 'rtbBALog.AppendRaw(fullLine);' 'Screw_BA UI must append the complete line without a second timestamp.'
Assert-NotContains $form 'rtbASSYLog.AppendToComponent($"[{processName}] {msg}")' 'Scan_ASSY UI must not add a legacy process prefix or second timestamp.'
Assert-NotContains $form 'rtbBALog.AppendToComponent($"[{processName}] {msg}")' 'Screw_BA UI must not add a legacy process prefix or second timestamp.'

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
Assert-Contains $writeLog 'public DateTime OccurredAt' 'UI log queue must retain the original occurrence time.'
Assert-Contains $writeLog 'EnqueueUiLog(richtextBox, logMessage, maxLineCount, DateTime.Now);' 'Normal UI logs must always use the shared queue.'
Assert-Contains $writeLog 'EnqueueUiLog(richtextBox, line ?? string.Empty, maxLineCount, DateTime.Now, isRaw: true);' 'Raw UI logs must always use the shared queue.'
Assert-NotContains $writeLog '_WriteAppendToComponent(richtextBox, logMessage, maxLineCount, occurredAt);' 'Normal UI logs must not bypass the shared queue.'
Assert-NotContains $writeLog '_WriteRawToComponent(richtextBox, line ?? string.Empty, maxLineCount);' 'Raw UI logs must not bypass the shared queue.'

Assert-Contains $trace ': $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";' 'Product pass flow lines must use one timestamp without an extra process prefix.'
Assert-Contains $trace 'UiSink?.Invoke(line);' 'Product pass UI must receive the same full line used by the local log.'
Assert-Contains $trace 'Log4netHelper.LogProductPassLine(line);' 'Product pass context must reuse one full line for local logging.'
Assert-Contains $trace 'WriteFlowCore(string.Empty);' 'Completed product passes must append a blank separator line.'
Assert-Contains $trace 'CompleteCore($"{result}，反馈{FeedbackPoint}={value}，总耗时={_totalWatch.ElapsedMilliseconds}ms");' 'Successful PLC writes must create the final product-pass result line.'
Assert-Contains $trace 'WriteFlowCore($"{result}，但反馈{FeedbackPoint}={value}写入失败，总耗时={_totalWatch.ElapsedMilliseconds}ms");' 'Failed PLC writes must be recorded explicitly.'
Assert-NotContains $trace 'LogFlowFailure(' 'Product pass main flow must not expose failure-reason nodes.'
Assert-NotContains $trace 'LogFlowElapsedFailure(' 'MES failure details must stay out of the concise product pass flow.'

$triggerLine = 'trace.LogFlow($"PLC触发产品过站，{uploadManager.triggerPoint}={triggerValue}");'
$triggerLineCount = [regex]::Matches($form, [regex]::Escape($triggerLine)).Count
if ($triggerLineCount -ne 4) {
    throw "All product-pass entry points must use the configured trigger address without extra spaces. Actual count: $triggerLineCount"
}

Assert-Contains $form 'trace?.LogFlowElapsed("产品信息读取完成", productInfoWatch' 'Product pass flow must retain the product-information node.'
Assert-Contains $form 'trace?.LogFlowElapsed("测试数据读取完成", testDataWatch);' 'Product pass flow must retain the test-data node.'
Assert-Contains $form 'trace?.LogFlowElapsed("MES请求-响应完成", httpWatch);' 'Every completed MES request must use the same concise request-response node.'
Assert-NotContains $form 'LogFlowFailure(' 'Product pass main flow must not include detailed failure-reason nodes.'
Assert-NotContains $form 'LogFlowElapsedFailure(' 'Product pass MES failure details must stay in diagnostic logs.'
Assert-NotContains $form 'trace?.LogFlowElapsed("请求构造完成"' 'Product pass flow must not log the request-build detail node.'
Assert-NotContains $form 'trace?.LogFlow("发起过站请求")' 'Product pass flow must not log the request-start detail node.'
Assert-NotContains $form '请求MES流程开始' 'Production UI must not log the old MES request-start line.'
Assert-NotContains $form '请求MES流程结束' 'Production UI must not log the old MES request-end line.'
foreach ($legacyProductLine in @(
    'UploadMes.AppendToComponent($"[{uploadEntity.Name}] 读取到条码：{prdSN}");',
    'UploadMes.AppendToComponent($"[{uploadEntity.Name}] 准备读取测试数据");',
    'UploadMes.AppendToComponent($"[{uploadEntity.Name}] 测试数据读取完成");',
    'UploadMes.AppendToComponent($"[{uploadEntity.Name}] 开始执行数据上传流程 <-");',
    'UploadMes.AppendToComponent($"[{uploadEntity.Name}] -> 数据上传流程执行结束");',
    'UploadMes.AppendToComponent($"[{uploadEntity.Name}] 过站成功，反馈{uploadEntity.feedbackPoint} = 1");'
)) {
    Assert-NotContains $form $legacyProductLine "Product pass UI must remove legacy verbose line: $legacyProductLine"
}
Assert-NotContains $form 'trace?.LogFlowFailure("数据采集完成"' 'Product pass flow must not use the old data-collection result wording.'
Assert-NotContains $form 'Log4netHelper.LogProductPass("MES_OUTBOX_' 'Outbox status must not be mixed into the product pass flow file.'
Assert-NotContains $form 'Log4netHelper.LogProductPass("OFFLINE_BYPASS"' 'Offline status must not be duplicated in the product pass flow file.'
Assert-Contains $form 'Log4netHelper.LogMesInteraction("MES_OUTBOX_CREATE"' 'Outbox status must be written to the MES interaction log.'
Assert-Contains $form 'Log4netHelper.LogMesInteraction("OFFLINE_BYPASS"' 'Offline MES status must be written to the MES interaction log.'
Assert-Contains $form 'uploadEntity.ProductResult' 'Product result reads must use UploadManagerEntity configuration.'
Assert-Contains $form 'uploadEntity.BarcodeToUpload' 'Barcode reads must use UploadManagerEntity configuration.'
Assert-Contains $form 'uploadEntity.BarcodeToUploadLength' 'Barcode length must use UploadManagerEntity configuration.'
Assert-Contains $form 'uploadEntity.feedbackPoint' 'PLC feedback must use UploadManagerEntity configuration.'
Assert-NotContains $form 'D7116写入失败' 'Product pass feedback logs must not contain the old fixed PLC address.'
Assert-Contains $form 'ProductPassTrace = productPassTrace' 'Blocking errors must retain the product-pass trace context.'
Assert-Contains $form 'productPassTrace?.HandOffToError();' 'Product-pass traces must not finish before queued errors are handled.'
Assert-Contains $form 'currentError.ProductPassTrace?.CompleteFeedback(passed: false, value: feedbackValue)' 'Manual clear must finish the trace only after PLC feedback succeeds.'
Assert-Contains $form 'errorData.ProductPassTrace?.CompleteFeedback(passed: false, value: Convert.ToInt16(errorData.FeedbackValue))' 'Non-blocking errors must finish the trace only after PLC feedback succeeds.'
Assert-Contains $form 'currentError.ProductPassTrace?.LogFeedbackWriteFailed(passed: false, value: feedbackValue, canRetry: true)' 'Manual-clear feedback failures must remain retryable.'
Assert-Contains $form 'errorData.ProductPassTrace?.LogFeedbackWriteFailed(passed: false, value: Convert.ToInt16(errorData.FeedbackValue), canRetry: false)' 'Non-blocking feedback failures must close as failed writes.'
Assert-Contains $form 'if (feedbackResult.IsSuccess)' 'PLC feedback success must be inspected before a flow is completed.'
Assert-Contains $form 'trace?.CompleteFeedback(passed: true, value: 1);' 'PASS feedback must be logged only through the completion helper.'
Assert-Contains $form 'trace?.LogFeedbackWriteFailed(passed: true, value: 1, canRetry: false);' 'Failed PASS feedback must not be recorded as successful.'

Assert-Contains $form 'addrInfo.HasBarcodeTag,' 'Route check trace must use the configured PLC trigger address.'
Assert-Contains $form 'addrInfo.BarcodeVerifyTag,' 'Route check trace must use the configured PLC feedback address.'
Assert-NotContains $form 'D7000' 'Route check flow must not contain a fixed PLC trigger address.'
Assert-NotContains $form 'D7001' 'Route check flow must not contain a fixed PLC feedback address.'
Assert-Contains $form 'line => rtbReadBarCode.AppendRaw(line)' 'Route check UI must receive the same full line used by the local log.'
Assert-Contains $routeTrace 'Log4netHelper.LogRouteCheckLine(fullLine)' 'Route check context must reuse one full line for local logging.'
Assert-Contains $routeTrace '_uiSink?.Invoke(fullLine)' 'Route check context must reuse one full line for UI logging.'
Assert-Contains $routeTrace 'WriteLine(string.Empty)' 'Completed route checks must append a blank separator line.'
Assert-Contains $form 'routeCheckTrace?.LogElapsed("拼版MES请求-响应完成", mesWatch)' 'Panelization MES requests must record their own elapsed time.'
Assert-Contains $form 'routeCheckTrace?.LogElapsed("流程检查MES请求-响应完成", mesWatch)' 'Route-check MES requests must record their own elapsed time.'
Assert-Contains $form 'OperateResult feedbackResult = _readWriteNet.Write(addrInfo.BarcodeVerifyTag, 1);' 'Route-check success must inspect the configured PLC write result.'
Assert-Contains $form 'if (feedbackResult.IsSuccess)' 'Route-check success must only be recorded after the PLC write succeeds.'
Assert-Contains $form 'routeCheckTrace?.LogFeedbackWriteFailed(passed: true, value: 1, canRetry: false)' 'A failed PASS feedback write must be logged as failed.'
Assert-Contains $form 'currentError.RouteCheckTrace?.CompleteFeedback(passed: false, value: feedbackValue)' 'Blocking failures must finish only after manual PLC feedback succeeds.'
Assert-Contains $form 'errorData.RouteCheckTrace?.CompleteFeedback(passed: false, value: Convert.ToInt16(errorData.FeedbackValue))' 'Non-blocking failures must finish only after PLC feedback succeeds.'
Assert-Contains $form 'CompleteWithoutFeedback(passed: false)' 'Paths without a PLC feedback opportunity must be recorded as not fed back.'
Assert-Contains $form 'result.IsSuccess ? "PANELIZATION_SEND" : "PANELIZATION_SEND_FAILED"' 'Panelization PLC diagnostics must distinguish write failure without changing control flow.'
Assert-NotContains $form '开始访问MES流程检查' 'Product route-check UI must remove the old MES request-start line.'
Assert-NotContains $form '收到MES流程检查反馈' 'Product route-check UI must remove the old MES response line.'
Assert-Contains $form 'if (barcodeType == 2)' 'Tooling barcode behavior must remain isolated in its existing branch.'
Assert-Contains $form 'Log4netHelper.LogRouteCheck("TOOLING_BARCODE_FEEDBACK"' 'Tooling barcode feedback logging must remain unchanged.'

foreach ($legacyRouteAction in @('PANELIZATION_NULL', 'PANELIZATION_FAIL', 'PANELIZATION_EMPTY', 'PANELIZATION_PASS', 'CHECKROUTE_NULL', 'CHECKROUTE_FAIL')) {
    Assert-NotContains $form "Log4netHelper.LogRouteCheck(`"$legacyRouteAction`"" "Product diagnostics must not pollute the simplified route-check flow log: $legacyRouteAction."
}

$messageOnlyPatternCount = [regex]::Matches($config, '<conversionPattern value="%m%n" />').Count
if ($messageOnlyPatternCount -ne 8) {
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
