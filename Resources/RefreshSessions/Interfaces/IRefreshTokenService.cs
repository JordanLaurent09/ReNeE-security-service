using security_service.Database.Entities;

namespace security_service.Resources.RefreshSessions.Interfaces
{
    public interface IRefreshTokenService
    {
        IEnumerable<RefreshToken> GetAll();

        RefreshToken GetTokenByValue(string token);

        void Add(RefreshToken refreshToken);

        void Delete(int id);

        void Update(RefreshToken refreshToken);
    }
}
