using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Books;

public class Book
{
    [Key]
    public int BookId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    public List<string> Authors { get; set; }

    [StringLength(100)]
    public string Publisher { get; set; }

    [Required]
    [RegularExpression(@"^\d{4}(-\d{2}-\d{2})?$", ErrorMessage = "PublishedDate must be a valid year or date.")]
    public string PublishedDate { get; set; }

    [StringLength(2000)]
    public string Description { get; set; }

    [Required]
    public List<IndustryIdentifier> IndustryIdentifiers { get; set; }

    [Range(1, int.MaxValue)]
    public int PageCount { get; set; }

    public List<string> Categories { get; set; }

    [Required]
    [StringLength(10)]
    public string Language { get; set; }

    public ImageLinks ImageLinks { get; set; }

    [Url]
    public string PreviewLink { get; set; }

    [Url]
    public string InfoLink { get; set; }
}

public class IndustryIdentifier
{
    [Required]
    [StringLength(20)]
    public string Type { get; set; }

    [Required]
    [StringLength(13, MinimumLength = 10)]
    public string Identifier { get; set; }
}

public class ImageLinks
{
    [Url]
    public string SmallThumbnail { get; set; }

    [Url]
    public string Thumbnail { get; set; }
}


//Info fetched from Google Books API
/*
{
  "kind": "books#volumes",
  "totalItems": 1,
  "items": [
    {
      "kind": "books#volume",
      "id": "ezeekgAACAAJ",
      "etag": "lyygGy8wxo0",
      "selfLink": "https://www.googleapis.com/books/v1/volumes/ezeekgAACAAJ",
      "volumeInfo": {
        "title": "The Way of Kings",
        "authors": [
          "Brandon Sanderson"
        ],
        "publishedDate": "2011",
        "description": "Released in two volumes due to size of book. According to mythology mankind used to live in The Tranquiline Halls. Heaven. But then the Voidbringers assaulted and captured heaven, casting out God and men. Men took root on Roshar, the world of storms. And the Voidbringers followed . . . They came against man ten thousand times. To help them cope, the Almighty gave men powerful suits of armor and mystical weapons, known as Shardblades. Led by ten angelic Heralds and ten orders of knights known as Radiants, mankind finally won. Or so the legends say. Today, the only remnants of those supposed battles are the Shardblades, the possession of which makes a man nearly invincible on the battlefield. The entire world is at war with itself - and has been for centuries since the Radiants turned against mankind. Kings strive to win more Shardblades, each secretly wishing to be the one who will finally unite all of mankind under a single throne. On a world scoured down to the rock by terrifying hurricanes that blow through every few days is a young spearman ,forced into the army of a Shardbearer, led to war against an enemy he doesn't understand and doesn't really want to fight. What happened deep in mankind's past? Why did the Radiants turn against mankind, and what happened to the magic they used to wield?",
        "industryIdentifiers": [
          {
            "type": "ISBN_10",
            "identifier": "0575102489"
          },
          {
            "type": "ISBN_13",
            "identifier": "9780575102484"
          }
        ],
        "readingModes": {
          "text": false,
          "image": false
        },
        "pageCount": 530,
        "printType": "BOOK",
        "categories": [
          "Fiction"
        ],
        "maturityRating": "NOT_MATURE",
        "allowAnonLogging": false,
        "contentVersion": "preview-1.0.0",
        "imageLinks": {
          "smallThumbnail": "http://books.google.com/books/content?id=ezeekgAACAAJ&printsec=frontcover&img=1&zoom=5&source=gbs_api",
          "thumbnail": "http://books.google.com/books/content?id=ezeekgAACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api"
        },
        "language": "en",
        "previewLink": "http://books.google.co.za/books?id=ezeekgAACAAJ&dq=isbn:9780575102484&hl=&cd=1&source=gbs_api",
        "infoLink": "http://books.google.co.za/books?id=ezeekgAACAAJ&dq=isbn:9780575102484&hl=&source=gbs_api",
        "canonicalVolumeLink": "https://books.google.com/books/about/The_Way_of_Kings.html?hl=&id=ezeekgAACAAJ"
      },
      "saleInfo": {
        "country": "ZA",
        "saleability": "NOT_FOR_SALE",
        "isEbook": false
      },
      "accessInfo": {
        "country": "ZA",
        "viewability": "NO_PAGES",
        "embeddable": false,
        "publicDomain": false,
        "textToSpeechPermission": "ALLOWED",
        "epub": {
          "isAvailable": false
        },
        "pdf": {
          "isAvailable": false
        },
        "webReaderLink": "http://play.google.com/books/reader?id=ezeekgAACAAJ&hl=&source=gbs_api",
        "accessViewStatus": "NONE",
        "quoteSharingAllowed": false
      },
      "searchInfo": {
        "textSnippet": "The brand new epic fantasy series from international bestseller Brandon Sanderson."
      }
    }
  ]
}
*/


//Info being stored
/*
 "title": "The Way of Kings",
        "authors": [
          "Brandon Sanderson"
        ],
        "publishedDate": "2011",
        "description": "Released in two volumes due to size of book. According to mythology mankind used to live in The Tranquiline Halls. Heaven. But then the Voidbringers assaulted and captured heaven, casting out God and men. Men took root on Roshar, the world of storms. And the Voidbringers followed . . . They came against man ten thousand times. To help them cope, the Almighty gave men powerful suits of armor and mystical weapons, known as Shardblades. Led by ten angelic Heralds and ten orders of knights known as Radiants, mankind finally won. Or so the legends say. Today, the only remnants of those supposed battles are the Shardblades, the possession of which makes a man nearly invincible on the battlefield. The entire world is at war with itself - and has been for centuries since the Radiants turned against mankind. Kings strive to win more Shardblades, each secretly wishing to be the one who will finally unite all of mankind under a single throne. On a world scoured down to the rock by terrifying hurricanes that blow through every few days is a young spearman ,forced into the army of a Shardbearer, led to war against an enemy he doesn't understand and doesn't really want to fight. What happened deep in mankind's past? Why did the Radiants turn against mankind, and what happened to the magic they used to wield?",
        "industryIdentifiers": [
          {
            "type": "ISBN_10",
            "identifier": "0575102489"
          },
          {
            "type": "ISBN_13",
            "identifier": "9780575102484"
          }
"pageCount": 530,
"categories": [
          "Fiction"
        ],
 "imageLinks": {
          "smallThumbnail": "http://books.google.com/books/content?id=ezeekgAACAAJ&printsec=frontcover&img=1&zoom=5&source=gbs_api",
          "thumbnail": "http://books.google.com/books/content?id=ezeekgAACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api"
        },
"language": "en",
        "previewLink": "http://books.google.co.za/books?id=ezeekgAACAAJ&dq=isbn:9780575102484&hl=&cd=1&source=gbs_api",
        "infoLink": "http://books.google.co.za/books?id=ezeekgAACAAJ&dq=isbn:9780575102484&hl=&source=gbs_api",
        "canonicalVolumeLink": "https://books.google.com/books/about/The_Way_of_Kings.html?hl=&id=ezeekgAACAAJ"
 */
