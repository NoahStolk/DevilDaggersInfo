using DevilDaggersInfo.App.Core.ApiClient;

namespace DevilDaggersInfo.Razor.CustomLeaderboard.HttpClients;

public partial class DdclApiHttpClient : ApiHttpClient
{
	public DdclApiHttpClient(HttpClient client)
		: base(client)
	{
	}
}
