using System.Collections.Generic;
using System.Threading.Tasks;
using connpanion.API.Models;

namespace connpanion.API.Data
{
    public interface IConnpanionRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();
         Task<IEnumerable<User>> GetUsers();
         Task<User> GetUser(int id);
         Task<Photograph> GetPhoto(int id);
         Task<Photograph> GetMainPhotoForUser(int userID);
    }
}