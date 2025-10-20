using Microsoft.AspNetCore.Mvc;
using HealingInWriting.Domain.Books;

namespace HealingInWriting.Controllers
{
    // TODO: Inject a books service to manage catalogue operations.
    public class BooksController : Controller
    {
        // TODO: Add thin endpoints that dispatch catalogue requests to the service.
        
        /// <summary>
        /// Temporary Index action with mock data for frontend development.
        /// TODO: Replace with service call once IBookService is implemented.
        /// </summary>
        public IActionResult Index()
        {
            // Mock data for frontend development
            var mockBooks = new List<Book>
            {
                new Book
                {
                    BookId = 1,
                    Title = "The Way of Kings",
                    Authors = new List<string> { "Brandon Sanderson" },
                    Publisher = "Tor Books",
                    PublishedDate = "2010",
                    Description = "From #1 New York Times bestselling author Brandon Sanderson, The Way of Kings, Book One of the Stormlight Archive begins an incredible new saga of epic proportion. Roshar is a world of stone and storms. Uncanny tempests of incredible power sweep across the rocky terrain...",
                    PageCount = 1007,
                    Categories = new List<string> { "Fantasy", "Epic Fantasy", "Fiction" },
                    Language = "en",
                    ImageLinks = new ImageLinks
                    {
                        SmallThumbnail = "http://books.google.com/books/content?id=X_dJAAAACAAJ&printsec=frontcover&img=1&zoom=5&source=gbs_api",
                        Thumbnail = "http://books.google.com/books/content?id=X_dJAAAACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api"
                    },
                    IndustryIdentifiers = new List<IndustryIdentifier>
                    {
                        new IndustryIdentifier { Type = "ISBN_13", Identifier = "9780765326355" }
                    }
                },
                new Book
                {
                    BookId = 2,
                    Title = "Educated: A Memoir",
                    Authors = new List<string> { "Tara Westover" },
                    Publisher = "Random House",
                    PublishedDate = "2018",
                    Description = "An unforgettable memoir about a young girl who, kept out of school, leaves her survivalist family and goes on to earn a PhD from Cambridge University. Born to survivalists in the mountains of Idaho, Tara Westover was seventeen the first time she set foot in a classroom...",
                    PageCount = 334,
                    Categories = new List<string> { "Biography & Autobiography", "Personal Memoirs", "Education" },
                    Language = "en",
                    ImageLinks = new ImageLinks
                    {
                        SmallThumbnail = "http://books.google.com/books/content?id=2ObWDgAAQBAJ&printsec=frontcover&img=1&zoom=5&edge=curl&source=gbs_api",
                        Thumbnail = "http://books.google.com/books/content?id=2ObWDgAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"
                    },
                    IndustryIdentifiers = new List<IndustryIdentifier>
                    {
                        new IndustryIdentifier { Type = "ISBN_13", Identifier = "9780399590504" }
                    }
                },
                new Book
                {
                    BookId = 3,
                    Title = "Atomic Habits",
                    Authors = new List<string> { "James Clear" },
                    Publisher = "Avery",
                    PublishedDate = "2018",
                    Description = "The #1 New York Times bestseller. Over 4 million copies sold! Tiny Changes, Remarkable Results. No matter your goals, Atomic Habits offers a proven framework for improving--every day. James Clear, one of the world's leading experts on habit formation, reveals practical strategies...",
                    PageCount = 320,
                    Categories = new List<string> { "Self-Help", "Personal Growth", "Success" },
                    Language = "en",
                    ImageLinks = new ImageLinks
                    {
                        SmallThumbnail = "http://books.google.com/books/content?id=XfFvDwAAQBAJ&printsec=frontcover&img=1&zoom=5&edge=curl&source=gbs_api",
                        Thumbnail = "http://books.google.com/books/content?id=XfFvDwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"
                    },
                    IndustryIdentifiers = new List<IndustryIdentifier>
                    {
                        new IndustryIdentifier { Type = "ISBN_13", Identifier = "9780735211292" }
                    }
                }
            };
            
            return View(mockBooks);
        }
    }
}
