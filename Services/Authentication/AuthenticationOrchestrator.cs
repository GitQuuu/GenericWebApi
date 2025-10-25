using Microsoft.AspNetCore.Http;
using Services.Authentication.IdentityProviderService;
using Services.Authentication.TokenService;
using Services.Authentication.UserService;
using Services.ResponseService;

namespace Services.Authentication;

/// <summary>
/// 
/// </summary>
public partial class AuthenticationOrchestrator : IAuthenticationOrchestrator
{
	private readonly IIdentityProviderService _identityProviderService;
	private readonly ITokenService _tokenService;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IResponseService _responseService;
	private readonly IUserService _userService;

	public AuthenticationOrchestrator(IIdentityProviderService identityProviderService,
									  ITokenService tokenService,
									  IHttpContextAccessor httpContextAccessor,
									  IResponseService responseService,
									  IUserService userService)
	{
		_identityProviderService = identityProviderService;
		_tokenService            = tokenService;
		_httpContextAccessor     = httpContextAccessor;
		_responseService         = responseService;
		_userService             = userService;
	}
}