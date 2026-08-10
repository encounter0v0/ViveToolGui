using System.Collections.Generic;
using System.Threading.Tasks;
using ViveToolGui.Models;

namespace ViveToolGui.Services;

/// <summary>
/// ViveTool 命令行封装服务接口。
/// </summary>
public interface IViveToolService
{
    /// <summary>当前环境是否找到了 vivetool.exe。</summary>
    bool Exists { get; }

    /// <summary>查询系统中的所有功能开关。</summary>
    Task<List<FeatureEntry>> QueryFeaturesAsync();

    /// <summary>启用指定功能。</summary>
    Task<(bool ok, string message)> EnableAsync(int id);

    /// <summary>禁用指定功能。</summary>
    Task<(bool ok, string message)> DisableAsync(int id);

    /// <summary>重置指定功能（恢复系统默认）。</summary>
    Task<(bool ok, string message)> ResetAsync(int id);
}
