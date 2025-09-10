using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
