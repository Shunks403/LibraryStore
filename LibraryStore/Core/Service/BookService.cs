using LibraryStore.Core.Interface.IServices;
using LibraryStore.Interface.IRepositories;
using LibraryStore.Models;

namespace LibraryStore.Core.Service;

public class BookService : IBookService
{
    private readonly IRepository _repository;

    public BookService(IRepository repository)
    {
        _repository = repository;
    }

    public Task<Book> AddBook(Book book)
    {
        return _repository.Add(book);
    }

    public Task<Book> UpdateBook(Book book)
    {
        return _repository.Update(book);
    }

    public IEnumerable<Book> GetAllBooks(int page, int size)
    {
        if (page <= 0)
            page = 1;
        return _repository.GetAll<Book>().Skip((page - 1) * size).Take(size).ToList();
        
    }

    public Task DeleteBook(int id)
    {
        return _repository.Delete<Book>(id);
    }

    public Task<Book> FindBookById(int id)
    {
        return _repository.GetById<Book>(id);
    }
}