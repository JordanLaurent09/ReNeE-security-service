using security_service.Database.Entities;
using security_service.Database.Repositories.Interfaces;
using security_service.Resources.RefreshSessions.Interfaces;

namespace security_service.Resources.RefreshSessions
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private IRepository<RefreshToken> _refreshSessionsRepository;

        public RefreshTokenService(IRepository<RefreshToken> refreshSessionsRepository)
        {
            _refreshSessionsRepository = refreshSessionsRepository;
        }

        public void Add(RefreshToken token)
        {
            _refreshSessionsRepository.Add(token);
        }

        public void Delete(int id)
        {
            _refreshSessionsRepository.Delete(id);
        }

        public IEnumerable<RefreshToken> GetAll()
        {
            return _refreshSessionsRepository.GetAll();
        }

        public RefreshToken GetTokenByValue(string token)
        {
            return _refreshSessionsRepository.GetTokenByValue(token);
        }

        public void Update(RefreshToken token)
        {
            _refreshSessionsRepository.Update(token);
        }
    }
}
