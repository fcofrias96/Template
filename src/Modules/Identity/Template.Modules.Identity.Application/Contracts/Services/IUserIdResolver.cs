namespace Template.Modules.Identity.Application.Contracts.Services
{
    /// <summary>
    /// Maps the public id carried by the JWT (<c>sub</c>) to the internal user id used by the database and cache keys.
    /// </summary>
    public interface IUserIdResolver
    {
        /// <returns>The internal id, or <c>null</c> when the value is not a public id of an active user.</returns>
        Task<int?> GetInternalUserIdAsync(string? userPublicId, CancellationToken cancellationToken);
    }
}
