using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using security_service.Database.Entities;
using security_service.Resources.RefreshSessions.Interfaces;
using security_service.Services;
using security_service.Utils.Classes;


namespace security_service.Resources.RefreshSessions
{
    [ApiController]
    [Route("auth")]
    public class RefreshTokensController : ControllerBase
    {
        IRefreshTokenService _refreshTokenService;

        ITempDataDictionaryFactory _tempDataDictionaryFactory;
        TokenService _tokenService;
        IHttpClientFactory _httpClientFactory;
        AuthService _authService;


        public RefreshTokensController(
            IRefreshTokenService refreshTokenService,
            ITempDataDictionaryFactory tempDataDictionaryFactory,
            TokenService tokenService,
            IHttpClientFactory httpClientFactory,
            AuthService authService)

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
            /*(string accessToken, string refreshToken)*/
            UserPayload data = await _authService.Login(credentialsDTO);


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

            return Ok(/*new { data.accessToken, data.refreshToken }*/ data);

        }

        [HttpGet]
        [Route("allUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {

            IEnumerable<User> users = await _authService.GetAllUsers();

            return Ok(users);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            string result = await _authService.RegisterUser(user);

            return Ok(result);
        }

        [HttpGet]
        [Route("users/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            User user = await _authService.GetUserById(id);

            return Ok(user);
        }

        [HttpPatch]
        [Route("users/update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            try
            {
                await _authService.UpdateUser(user);
                return Ok("Пользователь успешно изменен");
            }
            catch (Exception ex) 
            {
                return BadRequest("Ошибки при изменении пользователя");
            }
        }

        [HttpDelete]
        [Route("users/{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _authService.DeleteUser(id);
                return Ok("Пользователь успешно удален");
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка при удалении пользователя");
            }
        }

        [HttpPost]
        [Route("addPerformer")]
        [Authorize]
        public async Task<IActionResult> AddPerformer([FromBody] UsersPerformers data)
        {
            string result = await _authService.AddFavoritePerformer(data);

            return Ok(result);
        }

        [HttpGet]
        [Route("performers/{userId:int}")]
        [Authorize]
        public async Task<IActionResult> GetPerformersIds(int userId)
        {
            IEnumerable<int> ids = await _authService.GetPerformersIds(userId);

            return Ok(ids);
        }

        [HttpDelete]
        [Route("deletePerformer")]
        [Authorize]
        public async Task<IActionResult> DeleteFavoritePerformer([FromQuery]int userId, [FromQuery]int performerId)
        {
            try
            {
                await _authService.DeleteFavoritePerformer(userId, performerId);
                return Ok("Исполнитель успешно удален");
            }
            catch (Exception ex)
            {
                return BadRequest("Не удалось удалить исполнителя");
            }
        }

        [HttpGet]
        [Route("photos")]
        [Authorize]
        public async Task<IActionResult> GetPerformerPhotos([FromQuery] int userId, [FromQuery] int performerId)
        {
            IEnumerable<Photo> photos = await _authService.GetPerformerPhotos(userId, performerId);

            return Ok(photos);
        }

        [HttpPost]
        [Route("addPhoto")]
        [Authorize]
        public async Task<IActionResult> AddNewPhoto([FromBody] Photo photo)
        {
            string result = await _authService.AddPhoto(photo);

            return Ok(result);
        }

        [HttpDelete]
        [Route("deletePhoto/{photoId:int}")]
        [Authorize]
        public async Task<IActionResult> RemovePerformerPhoto(int photoId)
        {
            try
            {
                await _authService.RemovePhoto(photoId);

                return Ok("Фото успешно удалено");
            }
            catch
            {
                return BadRequest("Не удалось удалить фотографию");
            }
        }

        [HttpGet]
        [Route("albums")]
        [Authorize]
        public async Task<IActionResult> GetAlbumsIds([FromQuery] int userId, [FromQuery] int performerId)
        {
            IEnumerable<int> ids = await _authService.Albums(userId, performerId);

            return Ok(ids);
        }

        [HttpPost]
        [Route("addAlbum")]
        [Authorize]
        public async Task<IActionResult> AddAlbum([FromBody] UsersAlbums album)
        {
            string result = await _authService.AddAlbum(album);

            return Ok(result);
        }

        [HttpDelete]
        [Route("deleteAlbum")]
        [Authorize]
        public async Task<IActionResult> RemoveAlbum([FromQuery]int albumId, [FromQuery] int userId)
        {
            try
            {
                await _authService.RemoveAlbum(albumId, userId);
                return Ok("Альбом успешно удален");
            }
            catch
            {
                return BadRequest("Не удалось удалить альбои");
            }
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
