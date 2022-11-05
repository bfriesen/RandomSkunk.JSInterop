using RandomSkunk.JSInterop.Abstract;

namespace RandomSkunk.JSInterop;

/// <summary>
/// A dynamic proxy object that invokes JavaScript methods synchronously on the JavaScript <c>window</c> object.
/// </summary>
public sealed class SyncProxyJSRuntime : SyncProxy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SyncProxyJSRuntime"/> class.
    /// </summary>
    /// <param name="jsRuntime">The backing JS runtime.</param>
    public SyncProxyJSRuntime(IJSInProcessRuntime jsRuntime)
    {
        JSRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    /// <summary>
    /// Gets the backing JavaScript runtime.
    /// </summary>
    /// <returns>The backing <see cref="IJSInProcessRuntime"/>.</returns>
    public IJSInProcessRuntime JSRuntime { get; }

    /// <summary>
    /// Gets an async version of this proxy runtime.
    /// </summary>
    /// <returns>An <see cref="AsyncProxyJSRuntime"/> with the same backing <see cref="IJSRuntime"/> as this instance of
    ///     <see cref="SyncProxyJSRuntime"/>.</returns>
    public AsyncProxyJSRuntime AsAsync() => new(JSRuntime);

    /// <inheritdoc/>
    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (binder.Type.IsAssignableFrom(typeof(IJSInProcessRuntime)))
        {
            result = JSRuntime;
            return true;
        }

        result = null;
        return false;
    }

    /// <inheritdoc/>
    protected override TValue Invoke<TValue>(string identifier, params object?[]? args) =>
        JSRuntime.Invoke<TValue>(identifier, args);
}
