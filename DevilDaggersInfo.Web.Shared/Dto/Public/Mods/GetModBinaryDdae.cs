using DevilDaggersInfo.Core.Mod.Enums;

namespace DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.Mods;

public record GetModBinaryDdae
{
	public string Name { get; init; } = null!;

	public long Size { get; init; }

	public ModBinaryType ModBinaryType { get; init; }
}
