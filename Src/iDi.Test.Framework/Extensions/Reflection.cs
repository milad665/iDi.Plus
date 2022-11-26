using System.Reflection;

namespace iDi.Test.Framework.Extensions;

public static class Reflection
{
    public static TReturn? InvokeNonPublic<TReturn>(this object target, string nonPublicMethodName, params object[]? parameters)
    {
        var method = target.GetType().GetMethod(nonPublicMethodName, BindingFlags.NonPublic);
            if (method == null)
                throw new Exception("Non-public method not found in object.");

            return (TReturn) method.Invoke(target, parameters)!;
    }
    
    public static void InvokeNonPublicNoReturn(this object target, string nonPublicMethodName, params object[]? parameters)
    {
        var method = target.GetType().GetMethod(nonPublicMethodName, BindingFlags.NonPublic);
        if (method == null)
            throw new Exception("Non-public method not found in object.");

        method.Invoke(target, parameters);
    }
}