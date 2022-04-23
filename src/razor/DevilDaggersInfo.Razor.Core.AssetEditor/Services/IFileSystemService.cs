namespace DevilDaggersInfo.Razor.Core.AssetEditor.Services;

public interface IFileSystemService
{
	FileResult? Open();

	void Save(byte[] buffer);

	string? SelectDirectory();

	public class FileResult
	{
		public FileResult(string path, byte[] contents)
		{
			Path = path;
			Contents = contents;
		}

		public string Path { get; }

		public byte[] Contents { get; }
	}
}
