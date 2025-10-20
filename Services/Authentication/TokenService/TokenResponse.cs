namespace Services.Authentication.TokenService;

public sealed record TokenResponse(string AccessToken, DateTimeOffset ExpiresAt);
