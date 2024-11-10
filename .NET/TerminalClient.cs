using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Antelcat.AspNetCore.WebSocket;

namespace InteractiveServer;

public class TerminalClient : Client
{
    private readonly ConcurrentDictionary<string, Session> sessions = [];

    protected override async Task OnReceivedTextAsync(string text, CancellationToken token)
    {
        var payload = JsonSerializer.Deserialize(text, AppSerializerContext.MyOption.TerminalPayload);
        if (payload is null) return;
        if (sessions.TryGetValue(payload.Id, out var session))
        {
            if (payload.Type is "close")
            {
                sessions.Remove(payload.Id, out _);
                await session.DisposeAsync();
            }
            else
            {
                await session.WriteLineAsync(payload.Message);
            }
        }
        else
        {
            CreateSession(payload);
        }
    }

    private void CreateSession(TerminalPayload payload)
    {
        sessions.GetOrAdd(payload.Id, id =>
        {
            var ret = new Session(payload.Message);
            ret.Output += async s =>
            {
                await this.SendAsync(new TerminalPayload(id, s, "out").Serialize());
            };
            ret.Error += async s =>
            {
                await this.SendAsync(new TerminalPayload(id, s, "error").Serialize());
            };
            return ret;
        });
    }

    private class Session : IAsyncDisposable
    {
        private static readonly string Command = 
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd" :
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "bash" 
                                                                : throw new ArgumentOutOfRangeException();
        public Session(string workDir)
        {
            process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName               = Command,
                WorkingDirectory       = workDir,
                CreateNoWindow         = true,
                UseShellExecute        = false,
                RedirectStandardError  = true,
                RedirectStandardInput  = true,
                RedirectStandardOutput = true
            };
            if (process.Start())
            {
                tasks =
                [
                    Task.Run(async () =>
                    {
                        while (!cancel.IsCancellationRequested && !process.HasExited)
                        {
                            var line = await process.StandardOutput.ReadLineAsync(cancel.Token);
                            if (line is null || Output is null) continue;
                            await Output(line);
                        }
                    }),
                    Task.Run(async () =>
                    {
                        while (!cancel.IsCancellationRequested && !process.HasExited)
                        {
                            var line = await process.StandardError.ReadLineAsync(cancel.Token);
                            if (line is null || Error is null) continue;
                            await Error(line);
                        }
                    })
                ];
            }
        }

        private readonly Process                 process;
        private readonly CancellationTokenSource cancel = new();
        private readonly Task[]                  tasks  = [];

        public event Func<string, Task>? Output;
        public event Func<string, Task>? Error;

        public async Task WriteLineAsync(string line) => await process.StandardInput.WriteLineAsync(line);

        public async ValueTask DisposeAsync()
        {
            await cancel.CancelAsync();
            try
            {
                await Task.WhenAll(tasks);
            }
            catch
            {
                //
            }
            process.Dispose();
            cancel.Dispose();
        }
    }
}

file static class WebSocketExtensions
{
    public static ValueTask SendAsync(this Client client, string message)
    {
        return client.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage);
    }

    public static string Serialize(this TerminalPayload payload) =>
        JsonSerializer.Serialize(payload, AppSerializerContext.MyOption.TerminalPayload);
}

