namespace RandomSkunk.JSInterop;

/// <summary>
/// A dynamic proxy object that invokes JavaScript methods asynchronously on an instance of a JavaScript object.
/// </summary>
public sealed class AsyncProxyJSObjectReference : Abstract.AsyncProxy
{
    private readonly IJSObjectReference _jsObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncProxyJSObjectReference"/> class.
    /// </summary>
    /// <param name="jsObject">The backing JS object reference.</param>
    public AsyncProxyJSObjectReference(IJSObjectReference jsObject)
    {
        _jsObject = jsObject ?? throw new ArgumentNullException(nameof(jsObject));
    }

    /// <summary>
    /// Gets the backing JS object reference.
    /// </summary>
    /// <returns>The backing <see cref="IJSObjectReference"/>.</returns>
    public IJSObjectReference JSObject() => _jsObject;

    /// <summary>
    /// Gets a sync version of this proxy object.
    /// </summary>
    /// <returns>An equivalent <see cref="SyncProxyJSObjectReference"/>.</returns>
    public SyncProxyJSObjectReference Sync()
    {
        if (_jsObject is not IJSInProcessObjectReference inProcessObject)
            throw new InvalidOperationException();

        return new(inProcessObject);
    }

    /// <inheritdoc/>
    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (binder.Type.IsAssignableFrom(typeof(IJSInProcessRuntime)))
        {
            result = _jsObject;
            return true;
        }

        result = null;
        return false;
    }

    /// <inheritdoc/>
    protected override Task<TValue> InvokeAsync<TValue>(string identifier, params object?[]? args) =>
        _jsObject.InvokeAsync<TValue>(identifier, args).AsTask();
}
