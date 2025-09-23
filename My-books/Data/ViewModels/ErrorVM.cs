using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace My_books.Data.ViewModels
{
    public class ErrorVM
    {
        public int StatusCode { get; set; }
        public String Message { get; set; }
        public String Path { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
