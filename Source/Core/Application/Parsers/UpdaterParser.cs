using System.Linq.Dynamic.Core;

namespace Application;

public static class UpdaterParser
{
    public static IEnumerable<Action<T>> Parse<T>(string updaterCode)
    {
        updaterCode = updaterCode.Trim();
        
        var actionCodes = updaterCode.Split(";");
        
        return actionCodes.Select(ac =>
            (Action<T>)DynamicExpressionParser.ParseLambda(typeof(T), typeof(void), ac).Compile());
    }
}