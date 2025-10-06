using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Authentication;

public interface IAuthenticationOrchestrator
{
	Task<IActionResult> HandleEntraLoginAsync(CancellationToken ctx = default);
	Task<IActionResult> HandleEntraGoogleAsync(CancellationToken ctx = default);
}