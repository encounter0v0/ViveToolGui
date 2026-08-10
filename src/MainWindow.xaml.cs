using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ViveToolGui.Models;
using ViveToolGui.ViewModels;

namespace ViveToolGui;

/// <summary>
/// 主窗口：基于 WinUI 3（Windows App SDK）的原生界面，非 WebView 套壳。
/// </summary>
public sealed partial class MainWindow : WinUIEx.WindowEx
{
    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        this.InitializeComponent();
        ViewModel = new MainViewModel();

        this.Title = "ViveTool GUI";
        this.Width = 1100;
        this.Height = 760;
        this.CenterOnScreen();
        this.SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();

        // 根据运行环境提示管理员权限 / vivetool 缺失。
        AdminBar.IsOpen = !ViewModel.IsAdmin;
        MissingBar.IsOpen = ViewModel.ViveToolMissing;

        // 关闭主窗口即退出应用（非打包模式）。
        this.Closed += (_, _) => Application.Current.Exit();
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.RefreshAsync();
    }

    private async void EnableButton_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.CommandParameter is FeatureEntry fe)
            await ViewModel.EnableAsync(fe);
    }

    private async void DisableButton_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.CommandParameter is FeatureEntry fe)
            await ViewModel.DisableAsync(fe);
    }

    private async void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.CommandParameter is FeatureEntry fe)
            await ViewModel.ResetAsync(fe);
    }
}
