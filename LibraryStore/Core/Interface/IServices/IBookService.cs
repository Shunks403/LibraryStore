using LibraryStore.Models;

namespace LibraryStore.Core.Interface.IServices;

public interface IBookService
{ 
        Task<Book> AddBook(Book book); 
        
        Task<Book> UpdateBook(Book book);
        
        IEnumerable<Book> GetAllBooks(int page, int size);
        
        Task DeleteBook(int id);
        
        Task<Book> FindBookById(int id);
}