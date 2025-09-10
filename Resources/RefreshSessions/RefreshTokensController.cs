using Microsoft.AspNetCore.Http;
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
            /*(string accessToken, string refreshToken)*/UserPayload data = await _authService.Login(credentialsDTO);


            var session = HttpContext.Session;
            var cookie = HttpContext.Response.Cookies;

            if (session != null)
            {
                session.SetString("accessToken", data.AccessToken!);
                //session.SetString("refreshToken", data.refreshToken);
            }

            HttpContext.Response.Cookies.Append("accessToken", data.AccessToken!);

            Console.WriteLine(session!.GetString("accessToken"));
            //Console.WriteLine(session!.GetString("refreshToken"));

            return Ok(/*new { data.accessToken, data.refreshToken }*/ data );

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
