namespace DotSharp.Abstractions.Services;

/// <summary>
/// Provides methods to get the current user's information and claims.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's name.
    /// </summary>
    /// <returns>The name of the current user, or null if not available.</returns>
    string? GetCurrentUserName();

    /// <summary>
    /// Gets the current user's email.
    /// </summary>
    /// <returns>The email of the current user, or null if not available.</returns>
    string? GetCurrentUserEmail();

    /// <summary>
    /// Gets the current user's authentication token.
    /// </summary>
    /// <returns>The authentication token of the current user.</returns>
    string GetCurrentUserToken();

    /// <summary>
    /// Gets a claim value by its name for the current user.
    /// </summary>
    /// <param name="claimName">The name of the claim to retrieve.</param>
    /// <returns>The claim value, or null if the claim is not found.</returns>
    string? GetClaimByName(string claimName);
}