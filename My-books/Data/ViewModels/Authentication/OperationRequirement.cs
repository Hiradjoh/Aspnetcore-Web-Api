using Microsoft.AspNetCore.Authorization;

namespace My_books.Data.ViewModels.Authentication
{
    public class OperationRequirement : IAuthorizationRequirement
    {
        public string Operation { get; }
        public OperationRequirement(string operation)
        {
            Operation = operation;
        }
    }
}
