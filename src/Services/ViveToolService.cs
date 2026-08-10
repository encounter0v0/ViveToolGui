using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ViveToolGui.Models;

namespace ViveToolGui.Services;

/// <summary>
/// ViveTool 命令行封装：定位 vivetool.exe 并调用其 /query、/enable、/disable、/reset 子命令。
/// </summary>
public class ViveToolService : IViveToolService
{
    private readonly string _exePath;

    public ViveToolService()
    {
        _exePath = FindViveTool();
    }

    public bool Exists => File.Exists(_exePath);

    private static string FindViveTool()
    {
        var baseDir = AppContext.BaseDirectory;
        var candidates = new[]
        {
            Path.Combine(baseDir, "tools", "vivetool.exe"),
            Path.Combine(baseDir, "vivetool.exe"),
            "vivetool.exe"
        };

        foreach (var c in candidates)
        {
            try
            {
                if (File.Exists(c))
                    return Path.GetFullPath(c);
            }
            catch { /* 忽略无权访问的路径 */ }
        }

        var fromPath = FindInPath("vivetool.exe");
        if (fromPath is not null)
            return fromPath;

        // 默认回退路径：Exists 会返回 false，由界面提示用户放置。
        return candidates[0];
    }

    private static string? FindInPath(string fileName)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? "";
        foreach (var dir in pathEnv.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            try
            {
                var full = Path.Combine(dir, fileName);
                if (File.Exists(full))
                    return full;
            }
            catch { /* 忽略 */ }
        }
        return null;
    }

    public async Task<List<FeatureEntry>> QueryFeaturesAsync()
    {
        if (!Exists)
            throw new InvalidOperationException("未找到 vivetool.exe，请将其放入 tools 目录或确保其在 PATH 中。");

        var (exitCode, output) = await RunAsync("/query");
        if (exitCode != 0)
            throw new InvalidOperationException($"vivetool 返回错误（退出码 {exitCode}）：\n{output}");

        return ParseQuery(output);
    }

    public Task<(bool ok, string message)> EnableAsync(int id) => ApplyAsync("/enable", id);
    public Task<(bool ok, string message)> DisableAsync(int id) => ApplyAsync("/disable", id);
    public Task<(bool ok, string message)> ResetAsync(int id) => ApplyAsync("/reset", id);

    private async Task<(bool ok, string message)> ApplyAsync(string verb, int id)
    {
        if (!Exists)
            return (false, "未找到 vivetool.exe，请将其放入 tools 目录或确保其在 PATH 中。");

        var (exitCode, output) = await RunAsync($"{verb} /id:{id}");
        if (exitCode == 0)
            return (true, $"功能 {id} 操作成功。");

        return (false, $"操作失败（退出码 {exitCode}）：\n{output}");
    }

    private async Task<(int exitCode, string output)> RunAsync(string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _exePath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException("无法启动 vivetool.exe，请确认文件未被占用或已正确下载。");

        var stdOut = await process.StandardOutput.ReadToEndAsync();
        var stdErr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return (process.ExitCode, stdOut + Environment.NewLine + stdErr);
    }

    private static List<FeatureEntry> ParseQuery(string output)
    {
        var result = new List<FeatureEntry>();
        if (string.IsNullOrWhiteSpace(output))
            return result;

        // vivetool /query 以空行分隔每个功能块。
        var blocks = output.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var block in blocks)
        {
            int? id = null;
            var name = string.Empty;
            var stateText = string.Empty;

            foreach (var rawLine in block.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var line = rawLine.Trim();
                if (line.Length == 0)
                    continue;

                var idx = line.IndexOf(':');
                if (idx < 0)
                    continue;

                var key = line[..idx].Trim();
                var value = line[(idx + 1)..].Trim();

                if (key.Equals("Feature ID(s)", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("Id", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(value, out var parsed))
                        id = parsed;
                }
                else if (key.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    name = value;
                }
                else if (key.Equals("State", StringComparison.OrdinalIgnoreCase))
                {
                    stateText = value;
                }
            }

            if (id.HasValue)
            {
                result.Add(new FeatureEntry
                {
                    Id = id.Value,
                    Name = name,
                    StateText = stateText,
                    State = ParseState(stateText)
                });
            }
        }

        result.Sort((a, b) => a.Id.CompareTo(b.Id));
        return result;
    }

    private static FeatureState ParseState(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return FeatureState.Unknown;

        var lower = text.ToLowerInvariant();
        if (lower.Contains("enabled"))
            return FeatureState.Enabled;
        if (lower.Contains("disabled"))
            return FeatureState.Disabled;
        if (lower.Contains("default"))
            return FeatureState.Default;

        // 兼容形如 "Enabled (2)" / "Disabled (1)" / "Default (0)"。
        var open = text.IndexOf('(');
        var close = text.IndexOf(')');
        if (open >= 0 && close > open)
        {
            var num = text.Substring(open + 1, close - open - 1).Trim();
            if (int.TryParse(num, out var n))
            {
                return n switch
                {
                    0 => FeatureState.Default,
                    1 => FeatureState.Disabled,
                    2 => FeatureState.Enabled,
                    _ => FeatureState.Unknown
                };
            }
        }

        return FeatureState.Unknown;
    }
}
