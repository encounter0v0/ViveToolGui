# ViveTool GUI

> 🔗 Gitee 仓库：https://gitee.com/mccurios/ViveToolGui

基于 **WinUI 3（Windows App SDK）** 的 ViveTool 图形化管理工具。原生桌面应用，**非 WebView 套壳**，用于启用 / 禁用 / 重置 Windows 系统的功能开关（Feature Gates / Feature IDs）。

> ViveTool 是社区用于开关 Windows 隐藏功能的命令行工具，本工具为其提供图形界面。

## ✨ 功能特性

- 📋 一键查询系统中所有功能开关（Feature ID、名称、状态）
- 🔍 按 Feature ID 或名称实时搜索过滤
- ✅ 启用 / ⛔ 禁用 / 🔄 重置 任意功能开关
- 🛡️ 自动检测管理员权限、缺失 vivetool.exe 时给出引导
- 🎨 原生 Fluent 设计（Mica 材质背景），非 HTML 套壳
- 🪟 非打包（unpackaged）模式，生成独立 exe，无需 MSIX 安装

## 🧱 技术栈

- C# + .NET 8（`net8.0-windows`）
- WinUI 3 / Windows App SDK 1.6
- WinUIEx（窗口管理）
- CommunityToolkit.Mvvm（MVVM）
- 非打包（unpackaged）部署

## 📋 环境要求

- Windows 10 2004 (19041) 或更高 / Windows 11
- Visual Studio 2022 17.8+（需勾选「.NET 桌面开发」与「Windows 应用开发」workload）
- .NET 8 SDK
- Windows App SDK 1.6 运行时（随 VS workload 安装）

## 🔧 获取 ViveTool

本仓库**不包含** `vivetool.exe`（二进制不进版本库）。请下载后放入 `tools/` 目录：

1. 打开 https://github.com/thebookisclosed/ViVe/releases
2. 下载最新 Release 压缩包，解压得到 `vivetool.exe`
3. 复制到 `tools/vivetool.exe`

目录结构：

```
tools/
└── vivetool.exe
```

应用启动时会依次在 `tools/vivetool.exe`、程序所在目录、系统 `PATH` 中查找。

## 🚀 编译与运行

1. 用 Visual Studio 2022 打开 `ViveToolGui.sln`
2. 配置：`Debug | x64`（或 `arm64`）
3. **以管理员身份运行 Visual Studio**（否则 vivetool 调用会因权限不足失败）
4. 按 `F5` 启动调试

发布独立 exe：

```powershell
dotnet publish src/ViveToolGui.csproj -c Release -r win-x64 --self-contained true
```

产物位于 `src/bin/Release/net8.0-windows10.0.19041.0/win-x64/publish/`。

## 🖱️ 使用说明

1. 首次运行点击「刷新」加载功能列表。
2. 顶部搜索框输入 Feature ID（如 `26079239`）或名称，实时过滤。
3. 每行右侧按钮：
   - **启用**：`vivetool /enable /id:<ID>`
   - **禁用**：`vivetool /disable /id:<ID>`
   - **重置**：`vivetool /reset /id:<ID>`
4. 操作后会自动刷新该行状态。

## 📁 目录结构

```
ViveToolGui/
├── ViveToolGui.sln
├── tools/                 # 放置 vivetool.exe（见 tools/README.md）
├── LICENSE
├── README.md
└── src/
    ├── ViveToolGui.csproj
    ├── app.manifest       # 声明 requireAdministrator
    ├── App.xaml / App.xaml.cs
    ├── MainWindow.xaml / MainWindow.xaml.cs
    ├── Models/            # FeatureEntry, FeatureState
    ├── Services/          # ViveToolService（调用与解析）
    ├── ViewModels/        # MainViewModel
    └── Helpers/           # AdminHelper, BoolNegationConverter
```

## ⚠️ 注意事项

- 修改 Windows 功能开关可能影响系统稳定性，**请谨慎操作，后果自负**。
- 必须以管理员身份运行。
- 部分功能需重启（或注销）后生效，部分需特定 Windows 预览版。
- 本项目与微软官方无关，ViveTool 版权归原作者所有。

## 📄 许可证

[MIT](LICENSE)
