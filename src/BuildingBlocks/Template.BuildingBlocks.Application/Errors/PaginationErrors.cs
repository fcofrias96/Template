using Template.SharedKernel.Results;

namespace Template.BuildingBlocks.Application.Errors
{
    public static class PaginationErrors
    {
        public static readonly Error InvalidLimit = Error.Validation(
            "Pagination.InvalidLimit",
            "The 'limit' value must be greater than zero.");

        public static Error OffsetNotMultipleOfLimit(int offset, int limit) => Error.Validation(
            "Pagination.InvalidOffset",
            $"The 'offset' value ({offset}) must be either zero or a multiple of the 'limit' value ({limit}).");

        public static Error OffsetOutOfRange(int total, int limit) => Error.Validation(
            "Pagination.OffsetOutOfRange",
            $"The 'offset' value must be either zero or less than or equal to 'total' value ({total}) and a multiple of the 'limit' value ({limit}).");
    }
}
