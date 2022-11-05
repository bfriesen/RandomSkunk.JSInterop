using RandomSkunk.JSInterop.Abstract;

namespace RandomSkunk.JSInterop;

/// <summary>
/// A dynamic proxy object that invokes JavaScript methods asynchronously on the JavaScript <c>window</c> object.
/// </summary>
public sealed class AsyncProxyJSRuntime : AsyncProxy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncProxyJSRuntime"/> class.
    /// </summary>
    /// <param name="jsRuntime">The backing JS runtime.</param>
    public AsyncProxyJSRuntime(IJSRuntime jsRuntime)
    {
        JSRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    /// <summary>
    /// Gets the backing JavaScript runtime.
    /// </summary>
    /// <returns>The backing <see cref="IJSRuntime"/>.</returns>
    public IJSRuntime JSRuntime { get; }

    /// <summary>
    /// Gets a sync version of this proxy runtime.
    /// </summary>
    /// <returns>A <see cref="SyncProxyJSRuntime"/> with the same backing <see cref="IJSRuntime"/> as this instance of
    ///     <see cref="AsyncProxyJSRuntime"/>.</returns>
    /// <exception cref="InvalidOperationException">The value of the <see cref="JSRuntime"/> property does not implement
    ///     <see cref="IJSInProcessRuntime"/>.</exception>
    public SyncProxyJSRuntime AsSync()
    {
        if (JSRuntime is not IJSInProcessRuntime inProcessRuntime)
        {
            throw new InvalidOperationException(
                "Sync() cannot be called when the backing IJSRuntime does not also implement IJSInProcessRuntime.");
        }

        return new SyncProxyJSRuntime(inProcessRuntime);
    }

    /// <inheritdoc/>
    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (binder.Type.IsAssignableFrom(typeof(IJSRuntime)))
        {
            result = JSRuntime;
            return true;
        }

        result = null;
        return false;
    }

    /// <inheritdoc/>
    protected override Task<TValue> InvokeAsync<TValue>(string identifier, params object?[]? args) =>
        JSRuntime.InvokeAsync<TValue>(identifier, args).AsTask();
}
