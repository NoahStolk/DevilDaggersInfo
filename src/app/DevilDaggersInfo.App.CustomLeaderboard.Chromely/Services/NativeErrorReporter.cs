using DevilDaggersInfo.Core.NativeInterface;

namespace DevilDaggersInfo.App.CustomLeaderboard.Chromely.Services;

public class NativeErrorReporter : INativeErrorReporter
{
	public void ReportError(Exception exception)
	{
		// TODO: Log exception.
	}

	public void ReportError(string message)
	{
	}
}
