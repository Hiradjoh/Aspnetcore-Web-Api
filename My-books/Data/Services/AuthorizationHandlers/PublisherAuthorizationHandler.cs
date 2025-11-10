using Microsoft.AspNetCore.Authorization;
using My_books.Data.Models;
using My_books.Data.ViewModels.Authentication;
using System.Security.Claims;

namespace My_books.Data.Services.AuthorizationHandlers
{
    public class PublisherAuthorizationHandler : AuthorizationHandler<OperationRequirement, Publisher>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationRequirement requirement,
            Publisher resource)
        {
            // Write operation
            if (requirement.Operation == "Write" &&
                (context.User.IsInRole("Admin") || context.User.IsInRole("Publisher")))
            {
                context.Succeed(requirement);
            }

            // Delete operation
            if (requirement.Operation == "Delete")
            {
                if (context.User.IsInRole("Admin") ||
                    Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value) == resource.AddedByUserId)//kasi ke sakhdatesh role moshakhas nmishe 
                {
                    context.Succeed(requirement);
                }
            }

            
            return Task.CompletedTask;
        }
    }
}
