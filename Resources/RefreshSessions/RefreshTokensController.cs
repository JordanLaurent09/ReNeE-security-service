using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using security_service.Database.Entities;
using security_service.Resources.RefreshSessions.Interfaces;
using security_service.Services;
using security_service.Utils.Classes;

namespace security_service.Resources.RefreshSessions
{
    [Route("auth")]
    public class RefreshTokensController : Controller
    {
        IRefreshTokenService _refreshTokenService;
        
        ITempDataDictionaryFactory _tempDataDictionaryFactory;
        TokenService _tokenService;
        IHttpClientFactory _httpClientFactory;
        AuthService _authService;

        public RefreshTokensController(IRefreshTokenService refreshTokenService, ITempDataDictionaryFactory tempDataDictionaryFactory, TokenService tokenService, IHttpClientFactory httpClientFactory, AuthService authService)

        {
            _refreshTokenService = refreshTokenService;            
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
            _authService = authService;
           
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] CredentialsDTO credentialsDTO)
        {
            (string accessToken, string refreshToken) data = await _authService.Login(credentialsDTO);
            
            return Ok(new { data.accessToken, data.refreshToken});
            
        }

        [HttpPost]
        [Route("refreshToken")]
        public IActionResult RefreshToken([FromBody] RefreshData refreshToken)
        {           
            (string accessToken, string refreshToken) data = _authService.RefreshToken(refreshToken.RefreshToken);

            return Ok(new { data.accessToken, data.refreshToken });
        }


        [HttpGet]
        [Route("all")]
        public IActionResult All()
        {
            return Ok(_refreshTokenService.GetAll());
        }        
    }
}
