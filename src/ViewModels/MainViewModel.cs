using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ViveToolGui.Helpers;
using ViveToolGui.Models;
using ViveToolGui.Services;

namespace ViveToolGui.ViewModels;

/// <summary>
/// 主窗口视图模型：维护功能列表、搜索过滤、以及启用/禁用/重置操作。
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IViveToolService _viveTool;

    [ObservableProperty]
    private ObservableCollection<FeatureEntry> _features = new();

    [ObservableProperty]
    private ObservableCollection<FeatureEntry> _filteredFeatures = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "点击「刷新」加载功能列表";

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isAdmin;

    [ObservableProperty]
    private bool _viveToolMissing;

    public MainViewModel() : this(new ViveToolService())
    {
    }

    public MainViewModel(IViveToolService viveTool)
    {
        _viveTool = viveTool;
        _isAdmin = AdminHelper.IsRunningAsAdmin();
        _viveToolMissing = !_viveTool.Exists;
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    public async Task RefreshAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        StatusMessage = "正在查询功能列表...";
        try
        {
            var list = await _viveTool.QueryFeaturesAsync();
            Features.Clear();
            foreach (var f in list)
                Features.Add(f);

            ApplyFilter();
            ViveToolMissing = false;
            StatusMessage = $"共 {Features.Count} 个功能";
        }
        catch (System.Exception ex)
        {
            StatusMessage = $"查询失败：{ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task EnableAsync(FeatureEntry? entry)
    {
        if (entry is null)
            return;
        await ApplyAsync(entry, "/enable", "启用");
    }

    public async Task DisableAsync(FeatureEntry? entry)
    {
        if (entry is null)
            return;
        await ApplyAsync(entry, "/disable", "禁用");
    }

    public async Task ResetAsync(FeatureEntry? entry)
    {
        if (entry is null)
            return;
        await ApplyAsync(entry, "/reset", "重置");
    }

    private async Task ApplyAsync(FeatureEntry entry, string verb, string actionLabel)
    {
        if (IsBusy)
            return;

        IsBusy = true;
        StatusMessage = $"正在{actionLabel}功能 {entry.Id}...";
        try
        {
            var (ok, msg) = verb switch
            {
                "/enable" => await _viveTool.EnableAsync(entry.Id),
                "/disable" => await _viveTool.DisableAsync(entry.Id),
                _ => await _viveTool.ResetAsync(entry.Id)
            };

            if (ok)
            {
                StatusMessage = $"{actionLabel}功能 {entry.Id} 成功。";
                await RefreshAsync();
            }
            else
            {
                StatusMessage = msg;
            }
        }
        catch (System.Exception ex)
        {
            StatusMessage = $"{actionLabel}失败：{ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyFilter()
    {
        FilteredFeatures.Clear();
        var q = (SearchText ?? string.Empty).Trim().ToLowerInvariant();
        foreach (var f in Features)
        {
            if (string.IsNullOrEmpty(q) ||
                f.Id.ToString().Contains(q) ||
                (f.Name ?? string.Empty).ToLowerInvariant().Contains(q))
            {
                FilteredFeatures.Add(f);
            }
        }
    }
}
