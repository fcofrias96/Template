using System.Collections.Concurrent;
using System.Linq.Expressions;
using Template.Application.Contracts;

namespace Template.Application.Utilities
{
    public static class SpecificationHelper
    {
        // Cache: por cada tipo de Specification guardamos un delegado de creación
        private static readonly ConcurrentDictionary<Type, Delegate> _factoryCache = new();

        public static TSpecification CreatorInstance<TSpecification, TEntity>(
            IQueryParametersBase request,
            bool applyPagination = true
        )
            where TSpecification : ISpecification<TEntity>
        {
            var specType = typeof(TSpecification);

            // Intentamos obtener el factory ya compilado

            var factoryDelegate = _factoryCache.GetOrAdd(specType, value => CreateFactory<TSpecification>(value));

            var factory = (Func<IQueryParametersBase, bool, TSpecification>)factoryDelegate;

            return factory(request, applyPagination);
        }

        private static Delegate CreateFactory<TSpecification>(Type specType)
        {
            // Buscar constructor compatible (no solo exacto)
            var ctor = specType.GetConstructors()
                .FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    return parameters.Length == 2
                        && typeof(IQueryParametersBase).IsAssignableFrom(parameters[0].ParameterType)
                        && parameters[1].ParameterType == typeof(bool);
                });

            if (ctor == null)
            {
                throw new InvalidOperationException(
                    $"Specification '{specType.Name}' must have a constructor " +
                    $"with signature (IQueryParametersBase or derived, bool, ILogger or derived).");
            }

            // Params del lambda
            var requestParam = Expression.Parameter(typeof(IQueryParametersBase), "request");
            var applyPaginationParam = Expression.Parameter(typeof(bool), "applyPagination");

            // Cast explícito al tipo concreto que espera el constructor
            var requestConverted = Expression.Convert(requestParam, ctor.GetParameters()[0].ParameterType);

            // new TSpecification((TQueryParams)request, applyPagination
            var newExpr = Expression.New(ctor, requestConverted, applyPaginationParam);

            // Func<IQueryParametersBase, bool, ILogger, TSpecification>
            var lambda = Expression.Lambda<Func<IQueryParametersBase, bool, TSpecification>>(
                newExpr,
                requestParam,
                applyPaginationParam
            );

            return lambda.Compile();
        }
    }
}