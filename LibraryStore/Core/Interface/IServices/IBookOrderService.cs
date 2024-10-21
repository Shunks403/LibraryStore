using LibraryStore.Models;

namespace LibraryStore.Core.Interface.IServices;

public interface IBookOrderService
{
    Task<BookOrder> AddBookOrder(BookOrder order); 
        
    Task<BookOrder> UpdateBookOrder(BookOrder order);
        
    IEnumerable<BookOrder> GetAllBookOrders(int page, int size);
        
    Task DeleteBookOrder(int id);
        
    Task<BookOrder> FindBookOrderById(int id);
}