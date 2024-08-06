using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Application.Parsers;

public static class FilterParser
{
    public static LambdaExpression Parse<T>(string filterCode) =>
        DynamicExpressionParser.ParseLambda(typeof(T), typeof(bool), filterCode);
}