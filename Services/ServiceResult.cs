using System.Net;

namespace Services;

public class ServiceResult<T>
{
	public HttpStatusCode HttpResponse { get; private set; }
	public string? Message { get; private set; }
	public bool Success { get; private set; }
	public T? Data { get; private set; }
	
	public ServiceResult(bool success, HttpStatusCode httpResponse, string? message, T? data = default)
	{
		Success      = success;
		HttpResponse = httpResponse;
		Message      = message;
		Data         = data;
	}
}