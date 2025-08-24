using Microsoft.EntityFrameworkCore;
using security_service.Database.Context;
using security_service.Database.Entities;
using security_service.Database.Repositories.Interfaces;

namespace security_service.Database.Repositories
{
    public class RefreshTokenRepository : IRepository<RefreshToken>
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
            _context.SaveChanges();
        }

        public IEnumerable<RefreshToken> GetAll()
        {
            return _context.refreshSessions;
        }

        public RefreshToken GetTokenByValue(string token)
        {
            return _context.refreshSessions.FirstOrDefault(r => r.TokenValue == token) ?? throw new Exception("Specified session not found");
        }

        public void Add(RefreshToken entity)
        {
            _context.refreshSessions.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            RefreshToken session = _context.refreshSessions.FirstOrDefault(s => s.Id == id) ?? throw new Exception("Specified session not found");
            _context.refreshSessions.Remove(session);
            _context.SaveChanges();
        }

        public async Task Update(RefreshToken entity)
        {
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }
}
