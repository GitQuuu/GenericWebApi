using Api.Services.IdentityProviderService;
using Api.Services.TokenService;
using Services.ResponseService;

namespace Api.Services.Authentication;

/// <summary>
/// 
/// </summary>
public partial class AuthenticationOrchestrator : IAuthenticationOrchestrator
{
	private readonly IIdentityProviderService _identityProviderService;
	private readonly ITokenService _tokenService;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IResponseService _responseService;

	public AuthenticationOrchestrator(IIdentityProviderService identityProviderService,
									  ITokenService tokenService,
									  IHttpContextAccessor httpContextAccessor,
									  IResponseService responseService)
	{
		_identityProviderService = identityProviderService;
		_tokenService            = tokenService;
		_httpContextAccessor     = httpContextAccessor;
		_responseService		 = responseService;
	}
	
	
	
	
}