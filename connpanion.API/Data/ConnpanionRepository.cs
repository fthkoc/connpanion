using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using connpanion.API.Helpers;
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

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _dataContext.Users.Include(u => u.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => u.ID != userParams.UserID);
            users = users.Where(u => u.Gender == userParams.Gender);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserID, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.ID));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserID, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.ID));
            }

            var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge);
            var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);
            users = users.Where(u => u.DateOfBirth <= maxDateOfBirth && u.DateOfBirth > minDateOfBirth);

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }
            
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int userID, bool likers)
        {
            var user = await _dataContext.Users
                .Include(x => x.Likers)
                .Include(x => x.Likees)
                .FirstOrDefaultAsync(u => u.ID == userID);

            if (likers)
            {
                return user.Likers.Where(u => u.LikeeID == userID).Select(i => i.LikerID);
            }
            else
                return user.Likees.Where(u => u.LikerID == userID).Select(i => i.LikeeID);
        }

        public async Task<bool> SaveAll()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<Like> GetLike(int from, int to)
        {
            return await _dataContext.Likes.FirstOrDefaultAsync(u => u.LikerID == from && u.Likee.ID == to);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _dataContext.Messages.FirstOrDefaultAsync(m => m.ID == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _dataContext.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Receiver).ThenInclude(p => p.Photos)
                .AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(u => u.ReceiverID == messageParams.UserID && u.ReceiverDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(u => u.SenderID == messageParams.UserID && u.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(u => u.ReceiverID == messageParams.UserID && u.ReceiverDeleted == false 
                        && u.isRead == false);
                    break;
            }

            messages = messages.OrderByDescending(d => d.MessageSent);
            
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int senderID, int receiverID)
        {
            var messages = await _dataContext.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Receiver).ThenInclude(p => p.Photos)
                .Where(m => m.ReceiverID == senderID && m.ReceiverDeleted == false && m.SenderID == receiverID
                    || m.ReceiverID == receiverID && m.SenderID == senderID && m.SenderDeleted == false)
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync();

            return messages;
        }
    }
}