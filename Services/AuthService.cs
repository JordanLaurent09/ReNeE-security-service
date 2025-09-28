using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration.UserSecrets;
using security_service.Database.Entities;
using security_service.Resources.RefreshSessions.Interfaces;
using security_service.Utils.Classes;
using System.Security.Cryptography;

namespace security_service.Services
{
    public class AuthService
    {
        private TokenService _tokenService;
        private ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private IHttpClientFactory _httpClientFactory;
        private IRefreshTokenService _refreshTokenService;

        public AuthService(TokenService tokenService, ITempDataDictionaryFactory tempDataDictionaryFactory, IHttpClientFactory httpClientFactory, IRefreshTokenService refreshTokenService)
        {
            _tokenService = tokenService;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _httpClientFactory = httpClientFactory;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<User> GetUser(CredentialsDTO credentials)
        {
            var client = _httpClientFactory.CreateClient();
            using var response = await client.PostAsJsonAsync("http://users-service:5077/users/login", credentials);
            User? user = await response.Content.ReadFromJsonAsync<User>();

            return user;
        }

        // Методы для обработки взаимодействия с сервисом пользователей

        // Получить всех пользователей
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetFromJsonAsync<List<User>>("http://users-service:5077/users/all-user");

            return response!.AsEnumerable();
        }

        // Зарегистрировать пользователя
        public async Task<string> RegisterUser(User user)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("http://users-service:5077/users/new", user);

            return await response.Content.ReadAsStringAsync();
        }

        // Получить пользователя по идентификатору
        public async Task<User> GetUserById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var user = await client.GetFromJsonAsync<User>($"http://users-service:5077/users/{id}");

            return user;
        }

        // Обновить пользователя
        public async Task UpdateUser(User user)
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                await client.PatchAsJsonAsync("http://users-service:5077/users/change-info", user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }

        // Удалить пользователя
        public async Task DeleteUser(int id)
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                await client.DeleteFromJsonAsync<User>($"http://users-service:5077/users/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Добавить любимого исполнителя
        public async Task<string> AddFavoritePerformer(UsersPerformers data)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsJsonAsync("http://users-service:5077/users/performer", data);

            return await response.Content.ReadAsStringAsync();
        }

        // Получить список идентификаторов любимых исполнителей
        public async Task<IEnumerable<int>> GetPerformersIds(int userId)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<List<int>>($"http://users-service:5077/users/performers/{userId}");

            return response!.AsEnumerable();
        }


        // Удалить исполнителя
        public async Task DeleteFavoritePerformer(int userId, int performerId)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                await client.DeleteFromJsonAsync<UsersPerformers>($"http://users-service:5077/users/deletePerformer?userId={userId}&performerId={performerId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Фотографии конкретного пользователя для конкретного исполнителя
        public async Task<IEnumerable<Photo>> GetPerformerPhotos(int userId, int performerId)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<IEnumerable<Photo>>($"http://users-service:5077/users/photos?userId={userId}&performerId={performerId}");

            return response!.AsEnumerable();
        }

        // Добавить фотографию
        public async Task<string> AddPhoto(Photo photo)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsJsonAsync("http://users-service:5077/users/newPhoto", photo);

            return await response.Content.ReadAsStringAsync();
        }

        // Удалить фотографию
        public async Task RemovePhoto(int photoId)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                await client.DeleteFromJsonAsync<Photo>($"http://users-service:5077/users/deletePhoto/{photoId}");          
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Альбомы конкретного исполнителя для конкретного пользователя
        public async Task<IEnumerable<int>> Albums(int userId,int performerId)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<IEnumerable<int>>($"http://users-service:5077/users/albums?userId={userId}&performerId={performerId}");

            return response!.AsEnumerable();

        }

        // Добавление альбома пользователю
        public async Task<string> AddAlbum(UsersAlbums album)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsJsonAsync("http://users-service:5077/users/newAlbum", album);

            return await response.Content.ReadAsStringAsync();
        }

        // Удаление альбома у пользователя
        public async Task RemoveAlbum(int albumId, int userId)
        {
            var client = _httpClientFactory.CreateClient();
            try
            {
                var response = await client.DeleteFromJsonAsync<UsersAlbums>($"http://users-service:5077/users/deleteFavoriteAlbum?albumId={albumId}&userId={userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

        }

        public async Task<UserPayload /*(string, string)*/> Login(CredentialsDTO credentials)
        {     
            User user = await GetUser(credentials);
           
            if (user != null)
            {
                string accessToken = _tokenService.GenerateToken(user.Login!, user.Role.ToString());
                RefreshToken refreshToken = _tokenService.GenerateRefreshToken(user.Id, user.Login, user.Role.ToString());

                _refreshTokenService.Add(refreshToken);
                return new UserPayload() { Id = user.Id, Email = user.Email, AccessToken = accessToken }; /*(accessToken, refreshToken.TokenValue!);*/
            }

            return new UserPayload(); /*("-1", "-1");*/
        }
        
        public (string, string) RefreshToken(string token)
        {
            var refreshToken = _refreshTokenService.GetTokenByValue(token);                     

            if (refreshToken?.ExpiryDate >  DateTime.Now)
            {                
                string newAccessToken = _tokenService.GenerateToken(refreshToken.Username, refreshToken.Role.ToString());
                return (newAccessToken, refreshToken.TokenValue!);
            }
            else
            {
                string newAccessToken = _tokenService.GenerateToken(refreshToken.Username, refreshToken.Role.ToString());
                RefreshToken newRefreshToken = _tokenService.GenerateRefreshToken(refreshToken.UserId, refreshToken.Username, refreshToken.Role);

                refreshToken.TokenValue = newRefreshToken.TokenValue;
                refreshToken.ExpiryDate = newRefreshToken.ExpiryDate;

                _refreshTokenService.Update(refreshToken);
                return (newAccessToken, newRefreshToken.TokenValue);
            }
        }
    }
};
