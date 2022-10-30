namespace RandomSkunk.JSInterop;

/// <summary>
/// Extensions for <see cref="IJSRuntime"/>.
/// </summary>
public static class JSRuntimeExtensions
{
    /// <summary>
    /// Gets an async dynamic proxy object for the <see cref="IJSRuntime"/> instance.
    /// </summary>
    /// <param name="jsRuntime">The <see cref="IJSRuntime"/> to get a dynamic project object for.</param>
    /// <returns>An instance of <see cref="AsyncProxyJSRuntime"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="jsRuntime"/> is null.</exception>
    public static dynamic AsDynamic(this IJSRuntime jsRuntime)
    {
        if (jsRuntime is null)
            throw new ArgumentNullException(nameof(jsRuntime));

        return new AsyncProxyJSRuntime(jsRuntime);
    }
}
