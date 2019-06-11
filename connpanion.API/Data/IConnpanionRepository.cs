using System.Collections.Generic;
using System.Threading.Tasks;
using connpanion.API.Helpers;
using connpanion.API.Models;

namespace connpanion.API.Data
{
    public interface IConnpanionRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();
         Task<PagedList<User>> GetUsers(UserParams userParams);
         Task<User> GetUser(int id);
         Task<Photograph> GetPhoto(int id);
         Task<Photograph> GetMainPhotoForUser(int userID);
         Task<Like> GetLike(int from, int to);
    }
}