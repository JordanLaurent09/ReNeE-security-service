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
    [Produces("application/json")]
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


        /// <summary>
        /// Authorize user
        /// </summary>
        /// <param name="credentialsDTO"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// {
        ///     credential: "testMail@mail.ru",
        ///     password: "password"    
        /// }
        /// 
        /// </remarks>
        /// <response code="200">Authorize specific user </response>
        /// <response code="400">If param is absent</response>
        /// <response code="404">If specific user not found</response>
        /// <response code="500">If server error occurs</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] CredentialsDTO credentialsDTO)
        {            
            UserPayload data = await _authService.Login(credentialsDTO);


            var session = HttpContext.Session;
            var cookie = HttpContext.Response.Cookies;

            if (session != null)
            {
                session.SetString("accessToken", data.AccessToken!);
            }

            HttpContext.Response.Cookies.Append("accessToken", data.AccessToken!);

            Console.WriteLine(session!.GetString("accessToken"));          

            return Ok(data);

        }

        /// <summary>
        /// Gets all existing users
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all users</response>
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="500">If server error occurs</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]      
        [Route("allUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {

            IEnumerable<User> users = await _authService.GetAllUsers();

            return Ok(users);
        }


        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        ///  {
        ///     "login": "fred21",
        ///     "firstname": "Fred",
        ///     "lastname": "Johnson",
        ///     "email": "freddie22@mail.com",
        ///     "sex": "MALE",
        ///     "password": "iamfreddie"
        ///  }
        /// 
        /// </remarks>
        /// <response code="201">Returns newly created user</response>
        /// <response code="400">If entity is null</response>
        /// <response code="500">If server error occurs</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("register")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            string result = await _authService.RegisterUser(user);

            return Ok(result);
        }


        /// <summary>
        /// Gets specific user by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns specific user by id</response>
        /// <response code="400">If param is absent</response>
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="404">If specific user not found</response>
        /// <response code="500">If server error occurs</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("users/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            User user = await _authService.GetUserById(id);

            return Ok(user);
        }


        /// <summary>
        /// Changes specific user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// {
        ///     "id": 10,
        ///     "login": "fred21",
        ///     "firstname": "Fred",
        ///     "lastname": "Johnson",
        ///     "email": "freddie22@mail.com",
        ///     "sex": "MALE",
        ///     "password": "iamfreddie"
        ///  }
        /// 
        /// </remarks>
        /// <response code="200">Returns updated user</response>
        /// <response code="400">If entity is null</response>
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="404">If user not found</response>
        /// <response code="500">If server error occurs</response>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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



        /// <summary>
        /// Deletes specific user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Deletes specific user by id</response>
        /// <response code="400">If param is absence</response>
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="404">If user not found</response>
        /// <response code="500">If server error occurs</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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





        ///<summary>
        /// Add performer to user's favorites
        ///</summary>
        ///<param name="data"></param>
        ///<returns></returns>
        ///<remarks>
        /// 
        /// {
        ///     "userId": 1,
        ///     "performerId": 1
        /// }
        /// 
        ///</remarks>
        ///<response code="200">Returns success message</response>
        ///<response code="400">If uncorrect request occurs</response>
        ///<response code="401">If attempt of unauthorized access occurs</response>
        ///<response code="500">If server error occurs</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("addPerformer")]
        [Authorize]
        public async Task<IActionResult> AddPerformer([FromBody] UsersPerformers data)
        {
            string result = await _authService.AddFavoritePerformer(data);

            return Ok(result);
        }




        /// <summary>
        /// Get indexes of user's favorite performers
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="200">Returns indexes array</response>
        /// <response code="400">If bad response occurs</response>
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="404">If values didn't find</response>
        /// <response code="500">If server error occurs</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("performers/{userId:int}")]
        [Authorize]
        public async Task<IActionResult> GetPerformersIds(int userId)
        {
            IEnumerable<int> ids = await _authService.GetPerformersIds(userId);

            return Ok(ids);
        }


        /// <summary>
        /// Removes favorite user's performer
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="performerId"></param>
        /// <returns></returns>
        /// <response code="200">Returns success message</response>
        /// <response code="400">If bad response occurs</response>
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="404">If favorite performer didn't find</response>
        /// <response code="500">If server error occurs</response>
        /// 
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
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

        /// <summary>
        /// Return's array with user's photos
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="performerId"></param>
        /// <returns></returns>
        /// <response code="200">Returns photo array</response>
        /// <response code="400">If bad response occurs</response>
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="404">If photos didn't find</response>
        /// <response code="500">If server error occurs</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("photos")]
        [Authorize]
        public async Task<IActionResult> GetPerformerPhotos([FromQuery] int userId, [FromQuery] int performerId)
        {
            IEnumerable<Photo> photos = await _authService.GetPerformerPhotos(userId, performerId);

            return Ok(photos);
        }



        /// <summary>
        /// Add new user's photo of favorite performer
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        ///<remarks>
        ///
        /// {
        ///     performerId: 1,
        ///     userId: 1,
        ///     image: "text view of picture file"
        /// }
        /// 
        ///</remarks> 
        ///
        /// <response code="200"> Returns success message</response>
        /// <response code="400">If bad response occurs</response>
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="500">If server error occurs</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("addPhoto")]
        [Authorize]
        public async Task<IActionResult> AddNewPhoto([FromBody] Photo photo)
        {
            string result = await _authService.AddPhoto(photo);

            return Ok(result);
        }



        /// <summary>
        /// Removes chosen photo
        /// </summary>
        /// <param name="photoId"></param>
        /// <returns></returns>
        /// <response code="200"> Returns success message</response>
        /// <response code="400">If bad response occurs</response>
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="404">If chosen photo didn't find</response>
        /// <response code="500">If server error occurs</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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


        /// <summary>
        /// Get user's favorite albums indexes
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="performerId"></param>
        /// <returns></returns>
        /// <response code="200"> Returns array of albums indexes</response>
        /// <response code="400">If bad response occurs</response>
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="404">If chosen photo didn't find</response>
        /// <response code="500">If server error occurs</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("albums")]
        [Authorize]
        public async Task<IActionResult> GetAlbumsIds([FromQuery] int userId, [FromQuery] int performerId)
        {
            IEnumerable<int> ids = await _authService.Albums(userId, performerId);

            return Ok(ids);
        }


        /// <summary>
        /// Add album to user's favorites
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// {
        ///     albumId: 1,
        ///     userId: 1,
        ///     performerId: 1
        ///     
        /// }
        ///
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="400">If uncorrect request occurs</response> 
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="500">If server error occurs</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("addAlbum")]
        [Authorize]
        public async Task<IActionResult> AddAlbum([FromBody] UsersAlbums album)
        {
            string result = await _authService.AddAlbum(album);

            return Ok(result);
        }



        /// <summary>
        /// Removes user's favorite album
        /// </summary>
        /// <param name="albumId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="200"> Returns success message</response>
        /// <response code="400">If bad response occurs</response>
        /// <response code="401">If attempt of unauthorized access occurs</response>
        /// <response code="404">If chosen album didn't find</response>
        /// <response code="500">If server error occurs</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
