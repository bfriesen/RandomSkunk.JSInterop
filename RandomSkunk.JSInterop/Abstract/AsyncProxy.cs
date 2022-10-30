namespace RandomSkunk.JSInterop.Abstract;

/// <summary>
/// Provides a base class for dynamic proxy objects that invoke JavaScript methods asynchronously.
/// </summary>
public abstract class AsyncProxy : DynamicObject
{
    private static readonly MethodInfo _invokeAsyncMethod =
        typeof(AsyncProxy).GetMethod(nameof(InvokeAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;

    internal AsyncProxy()
    {
    }

    /// <inheritdoc/>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        var jsObjectTask = InvokeAsync<IJSObjectReference>(binder.Name);
        result = GetTaskOfProxyJSObject(jsObjectTask);
        return true;
    }

    /// <inheritdoc/>
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        if (binder.GetTypeArguments() is { Length: > 0 } typeArguments)
        {
            if (typeArguments.Length > 1)
                throw new InvalidOperationException("Type arguments must have exactly one item.");

            result = NonGenericInvokeAsync(typeArguments[0], binder.Name, args);
            return true;
        }
        else
        {
            var jsObjectTask = InvokeAsync<IJSObjectReference>(binder.Name, args);
            result = GetTaskOfProxyJSObject(jsObjectTask);
            return true;
        }
    }

    /// <summary>
    /// Invokes the specified JavaScript function asynchronously.
    /// </summary>
    /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments.</param>
    /// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-deserializing the return value.</returns>
    protected abstract Task<TValue> InvokeAsync<TValue>(string identifier, params object?[]? args);

    private static Task<AsyncProxyJSObjectReference> GetTaskOfProxyJSObject(Task<IJSObjectReference> jsObjectTask)
    {
        var tcs = new TaskCompletionSource<AsyncProxyJSObjectReference>();

        jsObjectTask.ContinueWith(objectTask =>
        {
            if (objectTask.IsCompletedSuccessfully)
                tcs.TrySetResult(new AsyncProxyJSObjectReference(objectTask.Result));
            if (objectTask.IsFaulted)
                tcs.TrySetException(objectTask.Exception!);
            if (objectTask.IsCanceled)
                tcs.TrySetCanceled();
        });

        return tcs.Task;
    }

    private object? NonGenericInvokeAsync(Type type, string identifier, params object?[]? args)
    {
        // TODO: This could use some optimizing.
        var invokeAsyncMethod = _invokeAsyncMethod.MakeGenericMethod(type);
        return invokeAsyncMethod.Invoke(this, new object?[] { identifier, args });
    }
}
