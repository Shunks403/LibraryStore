using LibraryStore.Core.Interface.IServices;
using LibraryStore.Interface.IRepositories;
using LibraryStore.Models;
using LibraryStore.Storage;

namespace LibraryStore.Core.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public Task<User> AddUser(User user)
    {
        return _repository.Add(user);
    }

    public Task<User> UpdateUser(User user)
    {
        return _repository.Update(user);
    }

    public IEnumerable<User> GetAllUsers(int page, int size)
    {
        if (page <= 0)
            page = 1;
        return _repository.GetAll<User>().Skip((page - 1) * size).Take(size).ToList();
        
    }

    public Task DeleteUser(int id)
    {
        return _repository.Delete<User>(id);
    }

    public Task<User> FindUserById(int id)
    {
        return _repository.GetById<User>(id);
    }
    
    public Task<User> FindUserByEmail(string email)
    {
        return _repository.GetByEmail(email);
    }
    
    
}