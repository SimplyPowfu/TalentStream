using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;
using TalentStream.Core.Repositories;

namespace TalentStream.WebApi.Filter
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeUserAttribute : TypeFilterAttribute
    {
        public AuthorizeUserAttribute() : base(typeof(AuthorizeUserFilter))
		{
		}
    }

	public sealed class AuthorizeUserFilter : IAsyncActionFilter
	{
		private readonly IUserRepository _userRepository;

		public AuthorizeUserFilter(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var userIdClaim = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userIdClaim))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Utente non identificato nel token." });
                return;
            }

            int userId = int.Parse(userIdClaim);
			var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                context.Result = new NotFoundObjectResult(new { message = "Utente non trovato su SQL Server." });
                return;
            }

			context.HttpContext.Items["ValidatedUser"] = user;

			await next();
		}
	}
}