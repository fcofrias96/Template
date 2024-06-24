using System.Linq.Expressions;
using System.Reflection;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Identity.Entities;

namespace Template.Identity.Specification.V1
{
    public class UserSpecification
    {
        public Expression<Func<User, bool>>? Criteria { get; protected set; }
        public bool Descending { get; private set; }

        public Expression<Func<User, object>> SortBy { get; set; }

        public int Skip { get; private set; }
        public int Take { get; private set; }

        public void ApplySortBy(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(User), "p");
            var propertyInfo = typeof(User).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo == null)
            {
                throw new BadRequestException($"Could not find {propertyName} on {typeof(User).Name}");
            }

            var property = Expression.Property(parameter, propertyInfo);
            var conversion = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<User, object>>(conversion, parameter);

            SortBy = lambda;
        }

        public UserSpecification(UserFilterRequest request)
        {
            Skip = request.Offset;
            Take = request.Limit;
            Descending = request.Descending;
            ApplySortBy(request.SortBy ?? "FirstName");

            #region Criteria

            #region FirstName

            if (request.FirstName != null)
            {
                Expression<Func<User, bool>> expression = User => User.FirstName.Contains(request.FirstName);

                Criteria = Criteria is null ? expression : Criteria.And(expression);
            }

            #endregion FirstName

            #region LastName

            if (request.LastName != null)
            {
                Expression<Func<User, bool>> expression = User => User.LastName.Contains(request.LastName);

                Criteria = Criteria is null ? expression : Criteria.And(expression);
            }

            #endregion LastName

            #region Email

            if (request.Email != null)
            {
                Expression<Func<User, bool>> expression = User => User.Email.Contains(request.Email);

                Criteria = Criteria is null ? expression : Criteria.And(expression);
            }

            #endregion Email

            #region UserName

            if (request.UserName != null)
            {
                Expression<Func<User, bool>> expression = User => User.UserName.Contains(request.UserName);

                Criteria = Criteria is null ? expression : Criteria.And(expression);
            }

            #endregion UserName

            #endregion Criteria
        }
    }
}