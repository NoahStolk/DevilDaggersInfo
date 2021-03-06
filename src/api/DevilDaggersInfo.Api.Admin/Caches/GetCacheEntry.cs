namespace DevilDaggersInfo.Api.Admin.Caches;

public record GetCacheEntry
{
	public GetCacheEntry(string name, int count)
	{
		Name = name;
		Count = count;
	}

	public string Name { get; }

	public int Count { get; }
}
