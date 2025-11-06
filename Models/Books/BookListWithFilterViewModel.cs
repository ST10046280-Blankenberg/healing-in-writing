using HealingInWriting.Models.Filters;
using HealingInWriting.Models.Stories;

namespace HealingInWriting.Models.Books
{
    public class BookListWithFilterViewModel
    {
        public BookListViewModel BookList { get; set; }
        public BooksFilterViewModel Filter { get; set; }
    }
}
