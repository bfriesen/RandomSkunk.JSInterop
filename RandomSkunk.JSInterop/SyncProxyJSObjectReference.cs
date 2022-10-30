namespace RandomSkunk.JSInterop;

/// <summary>
/// A dynamic proxy object that invokes JavaScript methods synchronously on an instance of a JavaScript object.
/// </summary>
public sealed class SyncProxyJSObjectReference : Abstract.SyncProxy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SyncProxyJSObjectReference"/> class.
    /// </summary>
    /// <param name="jsObject">The backing JS object reference.</param>
    public SyncProxyJSObjectReference(IJSInProcessObjectReference jsObject)
    {
        JSObject = jsObject ?? throw new ArgumentNullException(nameof(jsObject));
    }

    /// <summary>
    /// Gets the backing JS object reference.
    /// </summary>
    /// <returns>The backing <see cref="IJSInProcessObjectReference"/>.</returns>
    public IJSInProcessObjectReference JSObject { get; }

    /// <summary>
    /// Gets an async version of this proxy object.
    /// </summary>
    /// <returns>An equivalent <see cref="AsyncProxyJSObjectReference"/>.</returns>
    public AsyncProxyJSObjectReference Async() => new(JSObject);

    /// <inheritdoc/>
    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (binder.Type.IsAssignableFrom(typeof(IJSInProcessObjectReference)))
        {
            result = JSObject;
            return true;
        }

        result = null;
        return false;
    }

    /// <inheritdoc/>
    protected override TValue Invoke<TValue>(string identifier, params object?[]? args) =>
        JSObject.Invoke<TValue>(identifier, args);
}
