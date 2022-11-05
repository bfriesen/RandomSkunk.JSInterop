using RandomSkunk.JSInterop.Abstract;

namespace RandomSkunk.JSInterop;

/// <summary>
/// A dynamic proxy object that invokes JavaScript methods asynchronously on an instance of a JavaScript object.
/// </summary>
public sealed class AsyncProxyJSObjectReference : AsyncProxy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncProxyJSObjectReference"/> class.
    /// </summary>
    /// <param name="jsObject">The backing JS object reference.</param>
    public AsyncProxyJSObjectReference(IJSObjectReference jsObject)
    {
        JSObject = jsObject ?? throw new ArgumentNullException(nameof(jsObject));
    }

    /// <summary>
    /// Gets the backing JavaScript object reference.
    /// </summary>
    /// <returns>The backing <see cref="IJSObjectReference"/>.</returns>
    public IJSObjectReference JSObject { get; }

    /// <summary>
    /// Gets a sync version of this proxy object.
    /// </summary>
    /// <returns>A <see cref="SyncProxyJSObjectReference"/> with the same backing <see cref="IJSObjectReference"/> as this
    ///     instance of <see cref="AsyncProxyJSObjectReference"/>.</returns>
    /// <exception cref="InvalidOperationException">The value of the <see cref="JSObject"/> property does not implement
    ///     <see cref="IJSInProcessObjectReference"/>.</exception>
    public SyncProxyJSObjectReference AsSync()
    {
        if (JSObject is not IJSInProcessObjectReference inProcessObject)
        {
            throw new InvalidOperationException(
                "Sync() cannot be called when the backing IJSReferenceObject does not also implement IJSInProcessObjectReference.");
        }

        return new SyncProxyJSObjectReference(inProcessObject);
    }

    /// <inheritdoc/>
    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (binder.Type.IsAssignableFrom(typeof(IJSObjectReference)))
        {
            result = JSObject;
            return true;
        }

        result = null;
        return false;
    }

    /// <inheritdoc/>
    protected override Task<TValue> InvokeAsync<TValue>(string identifier, params object?[]? args) =>
        JSObject.InvokeAsync<TValue>(identifier, args).AsTask();
}
