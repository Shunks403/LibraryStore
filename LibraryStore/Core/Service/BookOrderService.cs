using LibraryStore.Core.Interface.IServices;
using LibraryStore.Interface.IRepositories;
using LibraryStore.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Core.Service;

public class BookOrderService : IBookOrderService
{
    private readonly IRepository _repository;

    public BookOrderService(IRepository repository)
    {
        _repository = repository;
    }

    public Task<BookOrder> AddBookOrder(BookOrder order)
    {
        return _repository.Add(order);
        
    }

    public Task<BookOrder> UpdateBookOrder(BookOrder order)
    {
        return _repository.Update(order);
    }

    public IEnumerable<BookOrder> GetAllBookOrders(int page, int size)
    {
        if (page <= 0)
            page = 1;
        return _repository.GetAll<BookOrder>().Include(b => b.User).Include(b => b.Book).Skip((page - 1) * size).Take(size).ToList();
        
    }

    public Task DeleteBookOrder(int id)
    {
        return _repository.Delete<BookOrder>(id);
    }

    public Task<BookOrder> FindBookOrderById(int id)
    {
        return _repository.GetById<BookOrder>(id);
    }
}