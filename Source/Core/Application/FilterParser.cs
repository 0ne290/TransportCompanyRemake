using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Application;

public static class FilterParser
{
    public static Expression<Func<T, bool>> Parse<T>(string filterCode) =>
        (Expression<Func<T, bool>>)DynamicExpressionParser.ParseLambda(typeof(T), typeof(bool), filterCode);
}