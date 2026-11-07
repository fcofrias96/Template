using Template.SharedKernel.Results;

namespace Template.BuildingBlocks.Application.Errors
{
    /// <summary>Errors raised while turning query-string parameters into a specification.</summary>
    public static class QueryErrors
    {
        public static readonly Error SortByRequired = Error.Validation(
            "Query.SortByRequired",
            "The sort field cannot be empty.");

        public static Error UnknownProperty(string propertyName, string typeName) => Error.Validation(
            "Query.UnknownProperty",
            $"Property '{propertyName}' was not found on type '{typeName}'.");

        public static Error InvalidValue(string value, string typeName) => Error.Validation(
            "Query.InvalidValue",
            $"Value '{value}' is not valid for type '{typeName}'.");
    }
}
