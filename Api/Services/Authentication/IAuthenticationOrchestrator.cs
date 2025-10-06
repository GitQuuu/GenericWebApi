using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Authentication;

public interface IAuthenticationOrchestrator
{
	Task<IActionResult> HandleEntraLoginAsync();
	Task<IActionResult> HandleEntraGoogleAsync();
}