namespace ViveToolGui.Models;

/// <summary>
/// 单个 Windows 功能开关（Feature Gate）的展示模型。
/// </summary>
public class FeatureEntry
{
    /// <summary>功能 ID，例如 26079239。</summary>
    public int Id { get; set; }

    /// <summary>功能名称（vivetool 可能为空）。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>vivetool 输出的原始状态文本，例如 "Enabled (2)"。</summary>
    public string StateText { get; set; } = string.Empty;

    /// <summary>解析后的枚举状态。</summary>
    public FeatureState State { get; set; } = FeatureState.Unknown;

    /// <summary>用于界面展示的中文状态标签。</summary>
    public string StateLabel => State switch
    {
        FeatureState.Enabled => "已启用",
        FeatureState.Disabled => "已禁用",
        FeatureState.Default => "默认",
        _ => string.IsNullOrWhiteSpace(StateText) ? "未知" : StateText
    };
}
