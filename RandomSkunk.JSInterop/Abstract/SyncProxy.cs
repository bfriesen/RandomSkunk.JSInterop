namespace RandomSkunk.JSInterop.Abstract;

/// <summary>
/// Provides a base class for dynamic proxy objects that invoke JavaScript methods synchronously.
/// </summary>
public abstract class SyncProxy : DynamicObject
{
    private static readonly MethodInfo _invokeMethod = typeof(SyncProxy).GetMethod(nameof(Invoke), BindingFlags.NonPublic | BindingFlags.Instance)!;

    internal SyncProxy()
    {
    }

    /// <inheritdoc/>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        var jsObject = Invoke<IJSInProcessObjectReference>(binder.Name);
        result = new SyncProxyJSObjectReference(jsObject);
        return true;
    }

    /// <inheritdoc/>
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        if (binder.GetTypeArguments() is { Length: > 1 } typeArguments)
        {
            if (typeArguments.Length > 1)
                throw new InvalidOperationException("Type arguments must have exactly one item.");

            result = NonGenericInvoke(typeArguments[0], binder.Name, args);
            return true;
        }
        else
        {
            var jsObject = Invoke<IJSInProcessObjectReference>(binder.Name, args);
            result = new SyncProxyJSObjectReference(jsObject);
            return true;
        }
    }

    /// <summary>
    /// Invokes the specified JavaScript function synchronously.
    /// </summary>
    /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments.</param>
    /// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-deserializing the return value.</returns>
    protected abstract TValue Invoke<TValue>(string identifier, params object?[]? args);

    private object? NonGenericInvoke(Type type, string identifier, params object?[]? args)
    {
        var invokeMethod = _invokeMethod.MakeGenericMethod(type);
        return invokeMethod.Invoke(this, new object?[] { identifier, args });
    }
}
