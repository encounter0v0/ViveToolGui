using Microsoft.UI.Xaml.Data;
using System;

namespace ViveToolGui.Helpers;

/// <summary>
/// 将 bool 取反，用于 XAML 绑定（如 IsBusy -> 按钮 IsEnabled）。
/// </summary>
public sealed class BoolNegationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => !(value is bool b && b);

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => !(value is bool b && b);
}
