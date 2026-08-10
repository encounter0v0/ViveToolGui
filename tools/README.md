# tools 目录

本目录用于放置 `vivetool.exe`（ViveTool 命令行本体）。

## 下载

从 ViveTool 官方发布页下载最新版，把解压出来的 `vivetool.exe` 放到本目录：

- GitHub Releases：https://github.com/thebookisclosed/ViVe/releases
- Gitee 镜像（如 GitHub 访问不畅）：https://gitee.com/mirrors/ViVe

放置后的目录结构应类似：

```
tools/
└── vivetool.exe
```

## 说明

- 应用启动时会按以下顺序查找 `vivetool.exe`：
  1. `tools/vivetool.exe`（程序所在目录下的 tools）
  2. 程序所在目录下的 `vivetool.exe`
  3. 系统 `PATH` 中的 `vivetool.exe`
- 若未找到，主界面会提示下载，功能操作将不可用。
- 由于 `.gitignore` 已忽略 `tools/*.exe`，该二进制不会进入 Git 版本库。
- `vivetool.exe` 自身需要管理员权限运行；本 GUI 已在 `app.manifest` 中声明
  `requireAdministrator`，因此以管理员身份启动后调用 vivetool 即可正常工作。
