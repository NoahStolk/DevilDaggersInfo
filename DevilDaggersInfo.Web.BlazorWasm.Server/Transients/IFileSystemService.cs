namespace DevilDaggersInfo.Web.BlazorWasm.Server.Transients;

public interface IFileSystemService
{
	string Root { get; }

	string[] TryGetFiles(DataSubDirectory subDirectory);

	string GetLeaderboardHistoryPathFromDate(DateTime dateTime);

	string GetPath(DataSubDirectory subDirectory);

	string FormatPath(string path);
}
