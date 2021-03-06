using System.ComponentModel.DataAnnotations;

namespace DevilDaggersInfo.Api.Main.Authentication;

public record UpdateNameRequest
{
	public string CurrentName { get; set; } = null!;

	public string CurrentPassword { get; set; } = null!;

	[Required]
	[StringLength(32, MinimumLength = 2)]
	public string NewName { get; set; } = null!;
}
