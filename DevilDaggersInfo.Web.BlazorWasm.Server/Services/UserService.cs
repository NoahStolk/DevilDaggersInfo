using DevilDaggersInfo.Web.BlazorWasm.Shared.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DevilDaggersInfo.Web.BlazorWasm.Server.Services;

public class UserService : IUserService
{
	private readonly ApplicationDbContext _dbContext;
	private readonly IConfiguration _configuration;
	private readonly ILogger<UserService> _logger;

	public UserService(ApplicationDbContext context, IConfiguration configuration, ILogger<UserService> logger)
	{
		_dbContext = context;
		_configuration = configuration;
		_logger = logger;
	}

	public UserEntity? Authenticate(string name, string password)
	{
		if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
			return null;

		UserEntity? user = _dbContext.Users
			.Include(u => u.UserRoles)!
			.ThenInclude(ur => ur.Role)
			.SingleOrDefault(u => u.Name == name);
		if (user == null)
			return null;

		if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
			return null;

		return user;
	}

	public UserEntity Create(string name, string password)
	{
		if (_dbContext.Users.Any(u => u.Name == name))
			throw new($"Name '{name}' is already taken.");

		CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

		UserEntity user = new()
		{
			Name = name,
			PasswordHash = passwordHash,
			PasswordSalt = passwordSalt,
			DateRegistered = DateTime.UtcNow,
		};

		_dbContext.Users.Add(user);
		_dbContext.SaveChanges();

		return user;
	}

	public void UpdateName(int id, string name)
	{
		UserEntity? user = _dbContext.Users.Find(id);
		if (user == null)
			throw new("User not found.");

		if (_dbContext.Users.Any(u => u.Name == name))
			throw new($"Name '{user.Name}' is already taken.");

		user.Name = name;

		_dbContext.SaveChanges();
	}

	public void UpdatePassword(int id, string password)
	{
		UserEntity? user = _dbContext.Users.Find(id);
		if (user == null)
			throw new("User not found.");

		CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

		user.PasswordHash = passwordHash;
		user.PasswordSalt = passwordSalt;

		_dbContext.SaveChanges();
	}

	public void Delete(int id)
	{
		UserEntity? user = _dbContext.Users.Find(id);
		if (user != null)
		{
			_dbContext.Users.Remove(user);
			_dbContext.SaveChanges();
		}
	}

	private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
	{
		if (string.IsNullOrWhiteSpace(password))
			throw new ArgumentException("Value cannot be empty or whitespace-only string.", nameof(password));

		if (password.Length < AuthenticationConstants.MinimumPasswordLength)
			throw new ArgumentException($"Password must be at least {AuthenticationConstants.MinimumPasswordLength} characters in length.", nameof(password));

		using HMACSHA512 hmac = new();
		passwordSalt = hmac.Key;
		passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
	}

	private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
	{
		if (string.IsNullOrWhiteSpace(password))
			throw new ArgumentException("Value cannot be empty or whitespace-only string.", nameof(password));
		if (storedHash.Length != 64)
			throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));
		if (storedSalt.Length != 128)
			throw new ArgumentException("Invalid length of password salt (128 bytes expected).", nameof(storedSalt));

		using (HMACSHA512 hmac = new(storedSalt))
		{
			byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
			for (int i = 0; i < computedHash.Length; i++)
			{
				if (computedHash[i] != storedHash[i])
					return false;
			}
		}

		return true;
	}

	public string GenerateJwt(UserEntity userEntity)
	{
		string keyString = _configuration["JwtKey"];
		byte[] keyBytes = Encoding.ASCII.GetBytes(keyString);

		SecurityTokenDescriptor tokenDescriptor = new()
		{
			Subject = ClaimsIdentityUtils.CreateClaimsIdentity(userEntity.Name, (userEntity.UserRoles?.Select(ur => ur.Role?.Name).Where(s => s != null).ToList() ?? new())!),
			Expires = DateTime.UtcNow.AddDays(7),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature),
		};

		JwtSecurityTokenHandler tokenHandler = new();
		SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);

		return tokenHandler.WriteToken(token);
	}

	public UserEntity? GetUserByJwt(string jwt)
	{
		try
		{
			string keyString = _configuration["JwtKey"];
			byte[] keyBytes = Encoding.ASCII.GetBytes(keyString);

			TokenValidationParameters tokenValidationParameters = new()
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
				ValidateIssuer = false,
				ValidateAudience = false,
			};

			JwtSecurityTokenHandler tokenHandler = new();
			ClaimsPrincipal principal = tokenHandler.ValidateToken(jwt, tokenValidationParameters, out SecurityToken securityToken);
			if (securityToken is not JwtSecurityToken jwtSecurityToken)
				return null;

			if (DateTime.UtcNow >= jwtSecurityToken.ValidTo)
				return null;

			if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
				return null;

			string? name = principal.GetName();
			return _dbContext.Users
				.Include(u => u.UserRoles)!
					.ThenInclude(ur => ur.Role)
				.FirstOrDefault(u => u.Name == name);
		}
		catch (Exception ex)
		{
			ex.Data.Add("JWT", jwt);
			if (ex is not SecurityTokenExpiredException)
				_logger.LogWarning(ex, "Token was invalid.");

			return null;
		}
	}
}
