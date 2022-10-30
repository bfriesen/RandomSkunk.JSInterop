namespace RandomSkunk.JSInterop;

internal static class InternalExtensions
{
    private static Func<InvokeMemberBinder, Type[]> _getTypeArguments = binder => Type.EmptyTypes;

    static InternalExtensions()
    {
        InitializeGetTypeArgumentsField();
    }

    public static Type[] GetTypeArguments(this InvokeMemberBinder binder) =>
        _getTypeArguments(binder);

    private static void InitializeGetTypeArgumentsField()
    {
        var csharpBinderType = Type.GetType("Microsoft.CSharp.RuntimeBinder.CSharpInvokeMemberBinder, Microsoft.CSharp");
        if (csharpBinderType is null)
            return;

        var typeArgumentsProperty = csharpBinderType.GetProperty("TypeArguments");
        if (typeArgumentsProperty is null)
            return;

        var uses = 0;
        const int compileAtThisNumberOfUses = 50;

        _getTypeArguments = binder =>
        {
            // Monitor uses of the reflection function. When it reaches
            // the threshold, compile the function in the background.
            if (Interlocked.Increment(ref uses) == compileAtThisNumberOfUses)
                ThreadPool.QueueUserWorkItem(state => CompileGetTypeArgumentsFunction());

            return binder.GetType().IsAssignableTo(csharpBinderType)
                ? (Type[])typeArgumentsProperty.GetValue(binder)!
                : Type.EmptyTypes;
        };

        void CompileGetTypeArgumentsFunction()
        {
            var binderParameter = Expression.Parameter(typeof(InvokeMemberBinder), "binder");

            var condition = Expression.Condition(
                test: Expression.TypeIs(binderParameter, csharpBinderType),
                ifTrue: Expression.Property(Expression.Convert(binderParameter, csharpBinderType), typeArgumentsProperty),
                ifFalse: Expression.Field(null, typeof(Type), nameof(Type.EmptyTypes)));

            var lambda = Expression.Lambda<Func<InvokeMemberBinder, Type[]>>(condition, binderParameter);
            _getTypeArguments = lambda.Compile();
        }
    }
}
