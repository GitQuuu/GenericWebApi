using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Services.ResponseService;

public interface IResponseService
{
	/// <summary>
	/// Handles the error response based on the provided ServiceResult.
	/// </summary>
	/// <typeparam name="T">The expected return type.</typeparam>
	/// <param name="result">The response from ServiceResult&lt;T&gt;.</param>
	/// <returns>An ActionResult representing the error response.</returns>
	Task<IActionResult> HandleResultAsync<T>(ServiceResult<T>? result) where T : class?;
}