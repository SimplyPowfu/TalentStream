using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;
using TalentStream.Core.Repositories;

namespace TalentStream.WebApi.Filter
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeCandidateAttribute : TypeFilterAttribute
    {
        public AuthorizeCandidateAttribute() : base(typeof(AuthorizeCandidateFilter))
		{
		}
    }

	public sealed class AuthorizeCandidateFilter : IAsyncActionFilter
	{
		private readonly ICandidateRepository _candidateRepository;
		private readonly IUserRepository _userRepository;

		public AuthorizeCandidateFilter(ICandidateRepository candidateRepository, IUserRepository userRepository)
		{
			_candidateRepository = candidateRepository;
			_userRepository = userRepository;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!context.ActionArguments.TryGetValue("id", out var idObj) || idObj is not int ProfileId)
			{
				context.Result = new BadRequestObjectResult(new { message = "ID Profile non valido o mancante." });
				return;
			}

			var profile = await _candidateRepository.GetByUserIdAsync(ProfileId);
			if (profile == null)
			{
				context.Result = new NotFoundObjectResult(new { message = "Profilo non trovato." });
				return;
			}

			var userIdClaim = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userIdClaim))
			{
				context.Result = new UnauthorizedObjectResult(new { message = "Utente non identificato." });
				return;
			}

			int userId = int.Parse(userIdClaim);
			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null)
			{
				context.Result = new NotFoundObjectResult(new { message = "Utente non trovato." });
				return;
			}

			if (profile.UserId != user.Id)
			{
				context.Result = new ForbidResult();
				return;
			}

			context.HttpContext.Items["ValidatedJob"] = profile;

			await next();
		}
	}
}