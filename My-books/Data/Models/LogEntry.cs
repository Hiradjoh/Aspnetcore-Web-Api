using System.ComponentModel.DataAnnotations.Schema;

namespace My_books.Data.Models
{
    [Table("Logs")]
    public class LogEntry
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? MessageTemplate { get; set; }
        public string? Level { get; set; }
        public DateTime TimeStamp { get; set; } // این معمولا NULL نمی‌شود
        public string? Exception { get; set; }
        public string? Properties { get; set; }
    }
}

