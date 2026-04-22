using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Template.Application.Contracts;

namespace Template.Application.Extensions
{
    public static class AutoMapperMappingExtensions
    {
        public static IMappingExpression<TSource, TDestination> ApplyPatchConvention<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mappingExpression)
            where TSource : class, IPatchDto
            where TDestination : class
        {
            mappingExpression.ForMember("Id", opt => opt.Ignore());

            mappingExpression.ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));

            return mappingExpression;
        }

        public static IMappingExpression<TSource, TDestination> ApplyUpdateConvention<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mappingExpression)
            where TSource : class, IUpdateDto
            where TDestination : class
        {
            mappingExpression.ForMember("Id", opt => opt.Ignore());

            return mappingExpression;
        }
    }
}
