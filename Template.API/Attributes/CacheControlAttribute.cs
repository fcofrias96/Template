using Microsoft.AspNetCore.Mvc;
using Template.Core.Application.Filter;

namespace Template.Api.Attributes
{
    public class CacheControlAttribute : TypeFilterAttribute
    {
        public CacheControlAttribute() : base(typeof(CacheControlFilter))
        {
        }
    }
}