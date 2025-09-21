namespace My_books.Data.Models
{
    public class Book_Author
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        //like BookId = 1 , AuthorId = 3 or  BookId = 1 , AuthorId = 4


    }
}
