using System.Reflection;

namespace iDi.Plus.Test.Extensions;

public static class Reflection
{
    public static TReturn? InvokeNonPublic<TReturn>(this object target, string nonPublicMethodName, params object[]? parameters)
    {
        var method = target.GetType().GetMethod(nonPublicMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
                throw new Exception("Non-public method not found in object.");

            return (TReturn) method.Invoke(target, parameters)!;
    }
    
    public static TReturn? InvokeNonPublic<TReturn>(this object target, string nonPublicMethodName)
    {
        return target.InvokeNonPublic<TReturn>(nonPublicMethodName, null);
    }

    public static void InvokeNonPublicNoReturn(this object target, string nonPublicMethodName, params object[]? parameters)
    {
        var method = target.GetType().GetMethod(nonPublicMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (method == null)
            throw new Exception("Non-public method not found in object.");

        method.Invoke(target, parameters);
    }

    public static void InvokeNonPublicNoReturn(this object target, string nonPublicMethodName)
    {
        target.InvokeNonPublicNoReturn(nonPublicMethodName,null);
    }
}