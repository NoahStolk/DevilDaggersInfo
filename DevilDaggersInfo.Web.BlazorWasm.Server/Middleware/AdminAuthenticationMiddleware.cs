using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace DevilDaggersInfo.Web.BlazorWasm.Server.Middleware;

public class AdminAuthenticationMiddleware
{
	private const string _adminApiRouteStart = "api/admin/";
	private const string _bearer = "Bearer ";
	private const string _role = "role: ";

	private static readonly Dictionary<string, string> _roleOverrides = new()
	{
		["custom-leaderboards"] = Roles.CustomLeaderboards,
		["donations"] = Roles.Donations,
		["mods"] = Roles.Mods,
		["mod-screenshots"] = Roles.Mods,
		["players"] = Roles.Players,
		["spawnsets"] = Roles.Spawnsets,
	};

	private readonly RequestDelegate _next;

	public AdminAuthenticationMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public Task InvokeAsync(HttpContext context)
	{
		PathString path = context.Request.Path;
		string pathString = path.ToString();
		if (pathString.Contains(_adminApiRouteStart))
			return HandleAdminApiCall(context, pathString);

		return _next(context);
	}

	private Task HandleAdminApiCall(HttpContext context, string pathString)
	{
		StringValues? auth = context.Request.Headers["Authorization"];
		string? authString = auth?.ToString();
		if (authString?.StartsWith(_bearer) != true)
			return Status(context, 401);

		string token = authString[_bearer.Length..];
		ClaimsPrincipal user = token.CreateClaimsPrincipalFromJwtTokenString();
		ClaimsIdentity? identity = user.Identities.FirstOrDefault();
		Claim? roleClaim = identity?.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
		string? roleClaimValueString = roleClaim?.Value?.ToString();
		if (roleClaimValueString?.StartsWith(_role) != true)
			return Status(context, 403);

		string[] userRoles = roleClaimValueString[_role.Length..].Split(',') ?? Array.Empty<string>();

		string endpointRoute = pathString[(pathString.IndexOf(_adminApiRouteStart) + _adminApiRouteStart.Length)..];
		if (endpointRoute.Contains('/'))
			endpointRoute = endpointRoute[..endpointRoute.IndexOf('/')];

		string requiredRole = _roleOverrides.ContainsKey(endpointRoute) ? _roleOverrides[endpointRoute] : "Admin";
		if (!userRoles.Contains(requiredRole))
			return Status(context, 403);

		return _next(context);
	}

	private static async Task Status(HttpContext context, int statusCode)
	{
		context.Response.StatusCode = statusCode;
		await Task.CompletedTask;
	}
}
