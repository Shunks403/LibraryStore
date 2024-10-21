using LibraryStore.Models;

namespace LibraryStore.Core.Interface.IServices;

public interface IUserService
{
    Task<User> AddUser(User user); 
        
    Task<User> UpdateUser(User user);
        
    IEnumerable<User> GetAllUsers(int page, int size);
        
    Task DeleteUser(int id);
        
    Task<User> FindUserById(int id);

    Task<User> FindUserByEmail(string email);
}