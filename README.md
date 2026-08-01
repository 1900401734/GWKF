# GWKF 开发 MES 快速交接手册

## 1. 项目定位

本项目是基于 .NET Framework 4.7.2 的 WinForms MES 现场程序，负责连接 PLC、MES、扭力控制器、串口扭力仪和可选的 Codesoft/LabelManager 打标软件，完成设备生产数据采集、流程验证、产品过站、标签打印和设备状态上报。

程序主窗体为 `MesDatas/Views/Form1.cs`，大部分生产运行逻辑、PLC 轮询、MES 交互和后台任务都从这里启动。

当前稳定发布版本：`v1.1.0`。

## 2. 启动顺序

程序启动后，主流程大致如下：

```mermaid
flowchart TD
    A[启动开发MES.exe] --> B[InitializeVariables]
    B --> C[加载系统配置和生产配置]
    C --> D[加载检测项和本地缓存]
    D --> E[读取设备名称与用户权限]
    E --> F[设置当前工单]
    F --> G[启动 Permanent Task]
    G --> H[启动 Dynamic Task]
    H --> I[PLC/MES/扭力/打印业务运行]
```

`Form1` 窗体加载阶段主要完成：

1. 加载 `SystemInfo` 生产配置。
2. 加载产品检测项、PLC 地址和设备配置。
3. 加载最近的 Weight MES 状态缓存。
4. 初始化 MES Token 和 HTTP 客户端。
5. 设置设备名称、权限、工单和界面数据。
6. 启动永久任务和动态任务。

## 3. Permanent Task 与 Dynamic Task

### 3.1 Permanent Task

永久任务在程序启动后持续运行，生命周期通常与主窗体一致：

| 任务 | 主要职责 |
| --- | --- |
| `InterfaceHeatBeat` | 按配置周期调用设备心跳接口，更新设备在线状态。 |
| `_plcManager.StartConnectionTaskAsync` | 建立和维护 PLC 连接，向业务层提供 PLC 读写对象。 |
| `Recovery` | 监听 PLC 复位信号，触发报警复位和错误处理。 |
| `DeviceStatusUpload` | 读取设备 RUN/IDLE/STOP 等状态，状态变化时或超过约 5 分钟未上传时上报 MES。 |
| `StartMesOutboxRetryTask` | “先反馈再上传”模式下，后台重试本地 MES 补传记录。 |

永久任务不应依赖单次产品过站才能运行。排查 PLC 或 MES 长连接问题时，优先检查这些任务的异常日志和连接状态。

### 3.2 Dynamic Task

动态任务由 `SetDynamicTaskStart` 启动，切换生产配置或重启任务时会取消并重新创建：

| 任务 | 主要职责 |
| --- | --- |
| `ProcessPlc_ReadBarcode` | 读取产品条码、工装条码等 PLC 信号并触发对应流程。 |
| `ProcessPlc_ReadValue` | 非装配机设备读取生产数据、工单号和测试结果。 |
| `ProcessPlc_ReadValue1/2/3` | 装配机不同工序读取生产数据、工单号和测试结果。 |
| `CallKeyArgsInterface` | 上传关键参数。 |
| `ReadDeviceArgsRealtime` | 实时读取设备运行参数。 |
| `MonitorModelSwitchFromPlc` | 监听 PLC 型号变化并更新当前工单/型号配置。 |
| `CallDeviceErrorUpload` | 监听并上传设备预警、报警信息。 |
| `CallRealtimeArgsInterface` | 按配置周期上传实时生产参数。 |
| `InitTorqueSystem` | 启动两个扭力控制器和 PLC 扭力转发流程。 |
| `InitSerialTorqueSystem` | 启动串口扭力仪和点检扭力采集。 |
| `CallPrintBarCode` | 仅在定义 `UseCodesoft` 时启动，负责模板标签打印。 |

动态任务停止时会取消 `CancellationTokenSource`，等待任务退出，并关闭 PLC 管理器。新增后台任务时必须考虑取消、重启和异常退出行为。

## 4. 主要业务流程

### 4.1 流程检查

流程检查用于在产品过站前验证当前产品是否允许进入后续工序，入口主要在产品条码读取后的 PLC 流程中。

基本流程：

```mermaid
sequenceDiagram
    participant PLC
    participant MES
    participant APP as 开发MES

    PLC->>APP: 触发流程检查
    APP->>APP: 读取产品条码
    APP->>MES: CHECKROUTE/流程检查请求
    MES-->>APP: PASS/FAIL/null
    APP->>PLC: 写入反馈 1 或 2
    APP->>APP: 记录流程检查日志
```

处理重点：

- PLC 触发地址和反馈地址来自界面配置的 `PlcAddressInfo`，不要在代码或现场文档中写死具体 D 地址。
- 支持条码规则验证、型号校验、拼版条码获取和 MES 流程检查。
- PASS 后只有在 PLC 写入反馈成功时才记录最终通过结果。
- FAIL、MES 无响应、返回 `null`、条码校验失败和本地规则失败进入现有错误处理流程。
- 阻塞错误需要人工清除；非阻塞错误由后台完成 PLC 反馈。
- 流程日志 UI 与本地日志文件使用相同的完整文本，便于按时间排查现场问题。

### 4.2 产品过站

产品过站由 `GetProductResult` 和 `SendResultToMes` 等逻辑协作完成，主要步骤如下：

1. 从 PLC 读取产品结果和产品条码。
2. 读取测试项实际值、上下限和结果。
3. 按配置执行本地规则、型号、条码和拼版校验。
4. 组装产品过站数据并请求 MES 过站接口。
5. 根据 MES PASS/FAIL/null 和产品过站模式决定 PLC 反馈值。
6. 记录产品过站日志、MES 交互日志和数据异常日志。
7. 在“先反馈再上传”模式下，先写 PLC 反馈，再由 MES Outbox 后台补传。

常见产品过站模式包括：

- 显示 NG 且阻塞。
- 显示 NG 且不阻塞。
- 强制过站或屏蔽上传配置。
- 流程检查关闭或离线模式。
- 先反馈再上传。

产品过站主日志只记录关键节点；MES 请求报文、响应报文和异常原因写入 MES 交互或数据异常日志，避免现场主日志过度膨胀。

### 4.3 标签打印

标签打印由 `CallPrintBarCode` 和 `printTest_Click` 等逻辑负责，使用第三方 `Interop.LabelManager2.dll` 调用 Codesoft/LabelManager 软件。

打印前置条件：

1. 现场安装并正确注册 Codesoft/LabelManager 打标软件。
2. 项目引用的 `LocalLibs/Interop.LabelManager2.dll` 存在。
3. 生产配置中启用打印模板。
4. 配置模板文件路径和打印机名称。
5. 模板文件可以被打标软件正常打开。
6. 装配机使用包含 `UseCodesoft` 的编译配置。

程序通过条件编译控制打印任务：

```csharp
#if UseCodesoft
    // 启动打印模板标签任务
    CallPrintBarCode();
#endif
```

如果没有安装打标软件或当前构建没有定义 `UseCodesoft`，对应打印代码不会参与编译或运行。非装配机现场不应强行启用打标任务。

### 4.4 扭力采集与 PLC 转发

扭力流程由两个 TCP 扭力控制器和 PLC 握手地址组成，两个工位分别处理：

- `Scan_ASSY`：装配工序 1。
- `Screw_BA`：装配工序 3/实际工位 5 动作。

正常流程：

```mermaid
sequenceDiagram
    participant TC as 扭力控制器
    participant APP as 开发MES
    participant PLC

    TC->>APP: MID 0061 原始扭力报文
    APP->>APP: 解析扭力、上下限和 OK/NG
    APP->>PLC: 写入扭力值、上下限、结果
    APP->>PLC: Req=1
    PLC-->>APP: Ack=1
    APP->>PLC: 等待 Req/Ack 回零
    APP-->>APP: 记录转发完成
```

关键规则：

- 扭力控制器地址和端口来自生产配置。
- PLC 扭力值、上下限、结果、Req、Ack 地址来自界面维护的 PLC 地址配置。
- 两个工位有独立转发锁，避免互相覆盖 PLC 握手地址。
- ACK 超时会清 Req 并报警，不无限等待。
- 互锁信号按控制器连接状态写入 PLC；连续写入失败达到阈值时报警。
- 正常扭力日志 UI 与本地文件格式一致。
- 扭力日志分别写入：
  - `D:\KaiFaLogs\扭力检测\Scan_ASSY\yyyy-MM-dd.log`
  - `D:\KaiFaLogs\扭力检测\Screw_BA\yyyy-MM-dd.log`

## 5. 其它核心业务

### MES 交互

MES 交互由 `HttpClientUtil`、`RequestMes` 和 `SendResultToMes` 等模块完成，负责 Token、流程检查、产品过站、关键参数、实时参数、设备状态、预警和标签相关接口调用。

排查 MES 问题时，按以下顺序查看：

1. 程序配置中的 MES URL、Token 和超时时间。
2. `MES交互` 日志中的请求、响应和耗时。
3. `数据异常` 日志中的异常原因。
4. 产品过站或流程检查主日志中的流程节点。

### 设备状态上传

设备状态通常由 PLC 信号转换为 RUN、IDLE、STOP 等状态：

- 状态变化时上传。
- 状态未变化但超过约 5 分钟未上传时上传一次。
- `UNKNOWN` 状态不上传。
- 上传失败写入数据异常日志，并等待下一轮处理。

### 关键参数与实时参数

- 关键参数：读取配置的检测项和产品参数，按启用选项上传 MES。
- 实时参数：根据配置周期读取设备运行数据并上传。
- 两者都依赖当前设备数据库中的产品配置和检测项定义。

### 报警与复位

- PLC 复位信号由永久任务持续监听。
- 报警按照阻塞/非阻塞模式处理。
- 阻塞报警显示在错误区域，通常需要人工清除。
- 非阻塞报警进入后台反馈流程。
- PLC 写入失败不能伪造为成功，需在数据异常日志中记录地址、值和失败原因。

## 6. 数据库与部署目录

### 6.1 当前代码实际使用的数据库格式

当前代码使用 Microsoft Jet OLEDB 4.0 连接 Access 数据库，代码实际拼接的文件扩展名是 `.mdb`：

```text
SystemDateBase.mdb
<database_name>.mdb
```

虽然现场有时将设备数据库简称为 `xxxx.db`，但当前程序不会自动把 `.db` 当作 `.mdb` 处理。若现场提供的是 `xxxx.db`，必须在交付前确认：

- 是否实际为 Access 数据库文件；
- 是否需要重命名为 `xxxx.mdb`；
- 或者是否需要另行修改数据库加载代码。

不要仅修改扩展名而不确认文件格式，避免程序启动时出现 Jet/OLEDB 连接错误。

### 6.2 多设备数据库关系

程序启动时先加载：

```text
SystemDateBase.mdb
```

然后读取其中 `SystemDataBase` 表 `id=1` 记录的 `database_name` 字段，再加载当前设备数据库：

```text
<database_name>.mdb
```

因此多个设备可以使用多个独立设备数据库文件，设备切换的关键是 `SystemDateBase.mdb` 中的 `database_name` 配置。

交接时建议维护以下设备映射表：

| 设备/产线 | 引导数据库 | 当前设备数据库 | 用途 |
| --- | --- | --- | --- |
| 设备 A | `SystemDateBase.mdb` | `<database_name>.mdb` | 系统配置、生产配置、PLC 地址、检测项 |
| 设备 B | `SystemDateBase.mdb` | `<database_name>.mdb` | 系统配置、生产配置、PLC 地址、检测项 |
| 设备 C | `SystemDateBase.mdb` | `<database_name>.mdb` | 系统配置、生产配置、PLC 地址、检测项 |

现场交付时应把实际文件名和设备名称补入此表，不要只交付一个未标识的数据库文件。

### 6.3 数据库必须放在运行目录

程序通过 `AppDomain.CurrentDomain.BaseDirectory` 拼接数据库路径，因此数据库必须放在程序实际运行目录：

```text
MesDatas\bin\Debug\SystemDateBase.mdb
MesDatas\bin\Debug\<database_name>.mdb
```

如果运行 Release，则对应放在：

```text
MesDatas\bin\Release\SystemDateBase.mdb
MesDatas\bin\Release\<database_name>.mdb
```

数据库不在运行目录时，常见表现包括：

- 启动时无法读取系统数据库；
- 无法查询当前设备数据库名；
- 生产配置页面为空或报错；
- 登录、工单、PLC 地址、检测项加载失败。

数据库连接使用的主要参数：

```text
Provider: Microsoft.Jet.OLEDB.4.0
User: admin
Database password: byd
```

现场机器需要安装可用的 Jet OLEDB 运行环境，并注意程序平台位数与数据库驱动兼容。Debug 配置当前为 x86，通常更适合现场旧版 Jet/COM 组件环境。

## 7. 编译与运行

### 7.1 必备环境

- Visual Studio 2022。
- .NET Framework 4.7.2 Developer Pack/运行环境。
- Microsoft Jet OLEDB 4.0 或现场可用的 Access 数据库驱动。
- PLC 通讯组件和现场网络配置。
- MES 服务地址、Token 和接口配置。
- 装配机需要 Codesoft/LabelManager 及 `LocalLibs/Interop.LabelManager2.dll`。

### 7.2 Debug 编译

Debug 当前定义：

```text
TRACE;DEBUG;UseCodesoft
```

平台为 x86，适合现场调试和旧版 Jet/打标 COM 组件联调。

使用 Visual Studio 开发者 PowerShell：

```powershell
MSBuild.exe MesDatas.sln `
  /t:Build `
  /p:Configuration=Debug `
  /p:Platform="Any CPU" `
  /m:1 `
  /v:minimal
```

运行前确认数据库文件位于：

```text
MesDatas\bin\Debug\
```

### 7.3 Release 编译

Release 当前定义：

```text
TRACE;USE_LABLEMANAGER;UseCodesoft
```

`UseCodesoft` 用于确保装配机标签代码参与编译，`USE_LABLEMANAGER` 保留兼容现有项目符号。

```powershell
MSBuild.exe MesDatas.sln `
  /t:Build `
  /p:Configuration=Release `
  /p:Platform="Any CPU" `
  /m:1 `
  /v:minimal
```

Release 输出目录：

```text
MesDatas\bin\Release\
```

### 7.4 条件编译规则

装配机相关代码必须使用：

```csharp
#if UseCodesoft
    // LabelManager/Codesoft 相关逻辑
#endif
```

不要直接删除 `#if`，也不要在未安装打标软件的非装配机环境中强制执行打标初始化。

## 8. 日志位置

主要日志目录由 `MesDatas/Log4net.config` 配置：

| 日志 | 默认目录 |
| --- | --- |
| MES 交互 | `D:\KaiFaLogs\MES交互\` |
| 产品过站 | `D:\KaiFaLogs\产品过站\` |
| 流程检查 | `D:\KaiFaLogs\流程检查\` |
| 扭力检测 Scan_ASSY | `D:\KaiFaLogs\扭力检测\Scan_ASSY\` |
| 扭力检测 Screw_BA | `D:\KaiFaLogs\扭力检测\Screw_BA\` |
| 数据异常 | `D:\KaiFaLogs\数据异常\` |

排查原则：

- 业务顺序和 PLC 反馈：先看产品过站/流程检查日志。
- 请求、响应和接口耗时：看 MES 交互日志。
- PLC、数据库、线程和写入错误：看数据异常日志。
- 扭力控制器和 Req/Ack：看对应工位扭力日志。

## 9. 现场快速交接清单

### 文件

- [ ] `开发MES.exe` 与依赖 DLL 来自同一构建输出目录。
- [ ] `SystemDateBase.mdb` 已放入运行目录。
- [ ] `database_name` 指向的设备数据库已放入运行目录。
- [ ] 实际设备数据库文件名已登记在交接表中。
- [ ] `Log4net.config` 已复制到 EXE 同级目录。
- [ ] 装配机已安装 Codesoft/LabelManager。
- [ ] `LocalLibs/Interop.LabelManager2.dll` 存在并与项目引用匹配。

### 配置

- [ ] PLC IP、端口和连接类型正确。
- [ ] PLC 地址维护页面中的条码、流程检查、过站、扭力 Req/Ack 地址正确。
- [ ] MES URL、Token、心跳周期和超时时间正确。
- [ ] 工单和产品型号配置正确。
- [ ] 标签模板路径、打印机名称和打印份数正确。
- [ ] 扭力控制器 IP/端口、串口号和 ACK 超时时间正确。

### 验证

- [ ] 程序能够启动并成功加载设备数据库。
- [ ] PLC 连接状态正常，复位信号可用。
- [ ] 流程检查 PASS/FAIL 均能得到正确 PLC 反馈。
- [ ] 产品过站 PASS/FAIL 和 MES 异常路径均已观察。
- [ ] 标签测试打印成功，实际模板打印成功。
- [ ] 两个扭力工位均能收到数据并完成 PLC Req/Ack 转发。
- [ ] 日志目录能够生成并按工位区分。
- [ ] 重启程序后工单和最近工单记录能够恢复。

## 10. 常见问题

### 启动时报数据库错误

优先检查：

1. `SystemDateBase.mdb` 是否位于 EXE 同级目录。
2. `SystemDataBase` 表中 `database_name` 是否有有效值。
3. `<database_name>.mdb` 是否位于同一目录。
4. 文件是否真的是 Access/Jet 可打开的数据库。
5. 进程位数是否与 Jet OLEDB 驱动匹配。

### 装配机没有标签打印任务

优先检查：

1. 当前构建是否包含 `UseCodesoft`。
2. Release 是否使用项目中的 `TRACE;USE_LABLEMANAGER;UseCodesoft`。
3. `Interop.LabelManager2.dll` 是否存在。
4. Codesoft/LabelManager 是否安装并可启动。
5. 模板文件和打印机名称是否正确。

### 只看到 PLC 连接但没有产品过站

优先检查：

1. 当前设备是否正确加载了 PLC 地址数据库。
2. 条码读取任务是否启动。
3. PLC 触发地址是否配置正确。
4. 工单、型号和条码规则是否通过。
5. 流程检查或产品上传是否被配置屏蔽。
6. 产品过站、流程检查和数据异常日志中的同一时间段记录。

## 11. 版本和历史

- `v1.1.0`：工单历史、产品过站/流程检查日志优化、扭力日志分工位落盘、电批互锁写入容错等。
- README 中的 Git 版本号用于代码发布追踪；现场旧版软件名称或客户版本号如仍在使用，应在交接单中单独登记。