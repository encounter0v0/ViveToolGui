using System.Security.Principal;

namespace ViveToolGui.Helpers;

/// <summary>
/// 判断当前进程是否以管理员身份运行。
/// </summary>
public static class AdminHelper
{
    public static bool IsRunningAsAdmin()
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }
}
