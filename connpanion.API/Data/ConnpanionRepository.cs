using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using connpanion.API.Models;
using Microsoft.EntityFrameworkCore;

namespace connpanion.API.Data
{
    public class ConnpanionRepository : IConnpanionRepository
    {
        private readonly DataContext _dataContext;

        public ConnpanionRepository(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public void Add<T>(T entity) where T : class
        {
            _dataContext.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _dataContext.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            return await _dataContext.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.ID == id);
        }

        public async Task<Photograph> GetPhoto(int id)
        {
            return await _dataContext.Photographs.FirstOrDefaultAsync(p => p.ID == id);
        }

        public async Task<Photograph> GetMainPhotoForUser(int userID)
        {
            return await _dataContext.Photographs.Where(u => u.UserID == userID).FirstOrDefaultAsync(p => p.IsMainPhotograph);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _dataContext.Users.Include(u => u.Photos).ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}