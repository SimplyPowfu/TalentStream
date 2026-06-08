using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TalentStream.Core.Repositories;

namespace TalentStream.WebApi.Filters
{
	// Risolve S3993: Specifica esplicitamente l'uso dell'attributo (Classi e Metodi)
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AuthorizeJobOwnerAttribute : TypeFilterAttribute
	{
		public AuthorizeJobOwnerAttribute() : base(typeof(AuthorizeJobOwnerFilter))
		{
		}
	}

	public sealed class AuthorizeJobOwnerFilter : IAsyncActionFilter
	{
		private readonly IJobRepository _jobRepository;
		private readonly IUserRepository _userRepository;

		public AuthorizeJobOwnerFilter(IJobRepository jobRepository, IUserRepository userRepository)
		{
			_jobRepository = jobRepository;
			_userRepository = userRepository;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!context.ActionArguments.TryGetValue("id", out var idObj) || idObj is not int jobId)
			{
				context.Result = new BadRequestObjectResult(new { message = "ID annuncio non valido o mancante." });
				return;
			}

			var job = await _jobRepository.GetIdJobPostingAsync(jobId);
			if (job == null)
			{
				context.Result = new NotFoundObjectResult(new { message = "JobPost non trovata." });
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

			if (user.CompanyId == null || job.CompanyId != user.CompanyId)
			{
				context.Result = new ForbidResult();
				return;
			}

			context.HttpContext.Items["ValidatedJob"] = job;

			await next();
		}
	}
}