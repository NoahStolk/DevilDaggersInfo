using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Mod.Enums;
using DevilDaggersInfo.Razor.Core.AssetEditor.Services;
using Microsoft.AspNetCore.Components;

namespace DevilDaggersInfo.Razor.Core.AssetEditor.Pages;

public partial class EditBinary
{
	private string? _binaryName;
	private ModBinary? _binary;

	[Inject]
	public IErrorReporter ErrorReporter { get; set; } = null!;

	[Inject]
	public IFileSystemService FileSystemService { get; set; } = null!;

	public void NewBinary(ModBinaryType modBinaryType)
	{
		_binary = new(modBinaryType);
	}

	public void OpenBinary()
	{
		_binary = null;

		IFileSystemService.FileResult? fileResult = FileSystemService.Open();
		if (fileResult == null)
			return;

		try
		{
			_binaryName = Path.GetFileName(fileResult.Path);
			_binary = new(fileResult.Contents, ModBinaryReadComprehensiveness.All);
		}
		catch (Exception ex)
		{
			ErrorReporter.ReportError(ex);
		}
	}

	public void ExtractBinary()
	{
		if (_binary == null)
			return;

		string outputDirectory = ""; // TODO
		_binary.ExtractAssets(outputDirectory);
	}

	public void CompileBinary()
	{
		if (_binary == null)
			return;

		byte[] compiledBinary = _binary.Compile();
		FileSystemService.Save(compiledBinary);
	}
}