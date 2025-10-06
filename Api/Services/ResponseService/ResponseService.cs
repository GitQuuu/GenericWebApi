using System.Net;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Services.ResponseService;

/// <summary>
/// Handling the service result response
/// This service should be used in the BLL layer to return the proper ObjectResult to the controller. It maps the HTTP status code in ServiceResult&lt;T&gt; to an appropriate IActionResult.
/// </summary>
public class ResponseService : IResponseService
{
	///  <summary>
	///  This service is meant to be used in the BLL layer it return the proper ObjectResult to the controller, it takes in a ServiceResult&lt;T&gt; object, checks its HttpResponse property, and returns an appropriate error response based on that HTTP status code, along with the error message provided in the ServiceResult&lt;T&gt;
	///  </summary>
	///  <param name="result">The response from ServiceResult&lt;T&gt;</param>
	///  <typeparam name="T">The expected return type</typeparam>
	///  <returns>A specific object result type</returns>
	///  <example>
	/// 		var result = await _authenticationService.LoginAsync(dto);
	/// 		return _responseService(result.Data)
	/// 		
	///  </example>
	///  <remarks>Foreach new case added here create a test for it in HandleResultUnitTests</remarks>
	public Task<IActionResult> HandleResultAsync<T>(ServiceResult<T>? result) where T : class
	{
        ArgumentNullException.ThrowIfNull(result);

        return Task.FromResult<IActionResult>(result.HttpResponse switch
											  {
												  HttpStatusCode.OK           => new OkObjectResult(result),
												  HttpStatusCode.NoContent    => new NoContentResult(),
												  HttpStatusCode.NotFound     => new NotFoundObjectResult(result),
												  HttpStatusCode.BadRequest   => new BadRequestObjectResult(result),
												  HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(result),
												  HttpStatusCode.Forbidden => new ObjectResult(new { error = result.Message })
												  {
													  StatusCode = StatusCodes.Status403Forbidden,
												  },
												  _                           => new ConflictObjectResult(result)
											  });
	}
}