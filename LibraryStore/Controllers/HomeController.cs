using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using LibraryStore.Core.Interface.IServices;
using Microsoft.AspNetCore.Mvc;
using LibraryStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace LibraryStore.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBookService _bookService;
    private readonly IBookOrderService _bookOrderService;
    private readonly IUserService _userService;
    private readonly IDistributedCache _cache;

    public HomeController(ILogger<HomeController> logger, IBookService context, IDistributedCache cache , IBookOrderService bookOrderService, IUserService userService)
    {
        _logger = logger;
        _bookService = context;
        _cache = cache;
        _bookOrderService = bookOrderService;
        _userService = userService;
    }

    [HttpGet]
    [Route("")]
    public async Task<ViewResult> Index()
    {
       
        ViewBag.IsAdmin = User.IsInRole("Admin");
            
        List<Book> bookList;
        string cacheKey = "bookList";
        string serializedBookList;
        var booksFromCache = await _cache.GetAsync(cacheKey);



        if (booksFromCache != null)
        {
            serializedBookList = Encoding.UTF8.GetString(booksFromCache);
            bookList = JsonConvert.DeserializeObject<List<Book>>(serializedBookList);
        }
        else
        {
            bookList = _bookService.GetAllBooks(0, 10).ToList();

            serializedBookList = JsonConvert.SerializeObject(bookList);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(serializedBookList), cacheOptions);
        }

        return View(bookList);
        
        
    }


[HttpGet]
    [Route("filter")]
    public async Task<IActionResult> Index(string SearchQuery, int? MinPrice, int? MaxPrice)
    {
        
        var books = from b in _bookService.GetAllBooks(0,10)
            select b;

        
        if (!string.IsNullOrEmpty(SearchQuery))
        {
            books = books.Where(b => b.Name.Contains(SearchQuery) || b.Author.Contains(SearchQuery));
        }
        
        if (MinPrice.HasValue)
        {
            books = books.Where(b => b.Price >= MinPrice.Value);
        }

        if (MaxPrice.HasValue)
        {
            books = books.Where(b => b.Price <= MaxPrice.Value);
        }
        
        var filteredBooks =  books.ToList();

        
        ViewBag.SearchQuery = SearchQuery;
        ViewBag.MinPrice = MinPrice;
        ViewBag.MaxPrice = MaxPrice;

        if (User.IsInRole("Admin"))
        {
            ViewBag.IsAdmin = true;
        }
        else
        {
            ViewBag.IsAdmin = false;
        }
        
        return View(filteredBooks);
    }
   
    public IActionResult Creat()
    {
        return View();
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Creat(Book book)
    {
        if (ModelState.IsValid)
        {
            _bookService.AddBook(book);  
            await _cache.RemoveAsync("bookList");
            return RedirectToAction(nameof(Index)); 
        }

        return View(book);  
    }

   
    public async Task<IActionResult> Edit(int id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _bookService.FindBookById(id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book book)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _bookService.UpdateBook(book);  
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        return View(book);  
    }


    private bool BookExists(int id)
    {
        return _bookService.GetAllBooks(0,10).Any(e => e.Id == id);
    }
    
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book =  _bookService.GetAllBooks(0,10).FirstOrDefault(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _bookService.FindBookById(id);
        _bookService.DeleteBook(book.Id);  
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Details(int id)
    {
        
        var book = await _bookService.FindBookById(id);
        if (book == null)
        {
            return NotFound();
        }

        
        return View(book);
    }
    
    [HttpPost]
    [Authorize] 
    public async Task<IActionResult> OrderBook(int bookId)
    {
        var email =  User.FindFirstValue(ClaimTypes.Email);
        
        var user = await _userService.FindUserByEmail(email);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var book = await _bookService.FindBookById(bookId);

        if (book == null)
        {
            return NotFound();
        }

        var order = new BookOrder
        {
            BookId = book.Id,
            UserId = user.Id,
            OrderDate = DateTime.Now,
            ReturnDate = DateTime.Now.AddDays(14) 
        };
        if (order == null)
        {
            return NotFound();
        }
        await _bookOrderService.AddBookOrder(order);
        

        TempData["Message"] = "Книга успішно замовлена!";

        return RedirectToAction("Index");
    }
    
    
    public async Task<IActionResult> Orders()
    {
        
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (userEmail == null)
        {
            return RedirectToAction("Login", "Account");
        }


        var orders = _bookOrderService.GetAllBookOrders(1, 10)
            .Where(b => b.User.Email == userEmail)
            .ToList();

        return View(orders);
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}