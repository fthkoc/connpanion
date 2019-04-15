using System.Threading.Tasks;
using connpanion.API.Models;

namespace connpanion.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string password);
         Task<User> Login(string userName, string password);
         Task<bool> isUserExists(string userName);
    }
}