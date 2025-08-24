using System.Xml.Serialization;

namespace security_service.Database.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();

        T GetTokenByValue(string token);

        void Add(T entity);

        Task Update(T entity);

        void Delete(int id);

    }
}
