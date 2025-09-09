namespace Api.Services.TokenService;

public sealed record TokenResponse(string AccessToken, DateTimeOffset ExpiresAt);
