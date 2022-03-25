using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.CustomEntries;

namespace DevilDaggersInfo.Web.BlazorWasm.Server.Extensions;

public static class AddUploadRequestExtensions
{
	// TODO: Move to base library and re-use in DDCL.
	public static string CreateValidation(this AddUploadRequest uploadRequest)
	{
		return string.Join(
			";",
			uploadRequest.PlayerId,
			uploadRequest.GetTime(),
			uploadRequest.GemsCollected,
			uploadRequest.GemsDespawned,
			uploadRequest.GemsEaten,
			uploadRequest.GemsTotal,
			uploadRequest.EnemiesKilled,
			uploadRequest.DeathType,
			uploadRequest.DaggersHit,
			uploadRequest.DaggersFired,
			uploadRequest.EnemiesAlive,
			uploadRequest.GetHomingStored(),
			uploadRequest.GetHomingEaten(),
			uploadRequest.IsReplay ? 1 : 0,
			uploadRequest.SurvivalHashMd5.ByteArrayToHexString(),
			string.Join(",", uploadRequest.GetLevelUpTime2(), uploadRequest.GetLevelUpTime3(), uploadRequest.GetLevelUpTime4()));
	}
}
