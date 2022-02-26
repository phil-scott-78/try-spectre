using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Spectre.Console;
using Spectre.Console.Rendering;
using XtermBlazor;

namespace TrySpectre;


class NoopExclusivityMode : IExclusivityMode
{
    public T Run<T>(Func<T> func)
    {
        return func();
    }

    public async Task<T> RunAsync<T>(Func<Task<T>> func)
    {
        return await func().ConfigureAwait(false);
    }
}

class XtermTextWriter : TextWriter
{
    private readonly Xterm _xterm;

    public XtermTextWriter(Xterm xterm)
    {
        _xterm = xterm;
    }

    public override Encoding Encoding => Encoding.Unicode;

    public override void Write(string? value)
    {
        if (value == null)
            return;

        var t = _xterm.Write(value);
    }
}

class XTermAnsiInput : IAnsiConsoleInput
{
    private readonly ConcurrentQueue<ConsoleKeyInfo> _keyQueue;

    public XTermAnsiInput(ConcurrentQueue<ConsoleKeyInfo> keyQueue)
    {
        _keyQueue = keyQueue;
    }

   
    public bool IsKeyAvailable()
    {
        return !_keyQueue.IsEmpty;
    }

    public ConsoleKeyInfo? ReadKey(bool intercept)
    {
        if (_keyQueue.TryDequeue(out var result))
        {
            return result;
        }

        return null;
    }

    public Task<ConsoleKeyInfo?> ReadKeyAsync(bool intercept, CancellationToken cancellationToken)
    {
        return Task.FromResult(ReadKey(intercept));
    }
}
