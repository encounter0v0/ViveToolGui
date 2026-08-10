using Microsoft.UI.Xaml;
using ViveToolGui;

namespace ViveToolGui;

/// <summary>
/// 应用入口（WinUI 3 非打包模式），由 Windows App SDK 自动生成的 Main 调用。
/// </summary>
public partial class App : Application
{
    private Window? _window;

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
