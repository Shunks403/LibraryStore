namespace LibraryStore.Models;

public class BookOrder
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime ReturnDate { get; set; } 
}