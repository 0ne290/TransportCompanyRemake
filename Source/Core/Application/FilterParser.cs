using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Application;

public static class FilterParser
{
    public static LambdaExpression Parse<T>(string filterCode) =>
        DynamicExpressionParser.ParseLambda(typeof(T), typeof(bool), filterCode);
}