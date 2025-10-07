namespace My_books.Data.Models
{
    public class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //navigation properties
        public List<Book> Books { get; set; }


    }
}
