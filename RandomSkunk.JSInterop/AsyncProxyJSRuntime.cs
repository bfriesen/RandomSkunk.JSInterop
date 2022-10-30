namespace RandomSkunk.JSInterop;

/// <summary>
/// A dynamic proxy object that invokes JavaScript methods asynchronously on the JavaScript <c>window</c> object.
/// </summary>
public sealed class AsyncProxyJSRuntime : Abstract.AsyncProxy
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
    /// Gets the backing JS runtime.
    /// </summary>
    /// <returns>The backing <see cref="IJSRuntime"/>.</returns>
    public IJSRuntime JSRuntime { get; }

    /// <summary>
    /// Gets a sync version of this proxy runtime.
    /// </summary>
    /// <returns>An equivalent <see cref="SyncProxyJSRuntime"/>.</returns>
    public SyncProxyJSRuntime Sync()
    {
        if (JSRuntime is not IJSInProcessRuntime inProcessRuntime)
            throw new InvalidOperationException();

        return new(inProcessRuntime);
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
