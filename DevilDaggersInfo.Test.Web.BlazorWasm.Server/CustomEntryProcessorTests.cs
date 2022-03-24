using DevilDaggersInfo.Core.Encryption;
using DevilDaggersInfo.Core.Extensions;
using DevilDaggersInfo.Web.BlazorWasm.Server.Caches.SpawnsetHashes;
using DevilDaggersInfo.Web.BlazorWasm.Server.Enums;
using DevilDaggersInfo.Web.BlazorWasm.Server.Exceptions;
using DevilDaggersInfo.Web.BlazorWasm.Shared.Dto.Public.CustomEntries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace DevilDaggersInfo.Test.Web.BlazorWasm.Server;

[TestClass]
public class CustomEntryProcessorTests
{
	private readonly Mock<ApplicationDbContext> _dbContext;
	private readonly CustomEntryProcessor _customEntryProcessor;
	private readonly AesBase32Wrapper _encryptionWrapper;
	private readonly byte[] _fakeReplay;
	private readonly byte[] _v3Hash;

	public CustomEntryProcessorTests()
	{
		MockEntities mockEntities = new();

		DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
		_dbContext = new Mock<ApplicationDbContext>(optionsBuilder.Options)
			.SetUpDbSet(db => db.Tools, mockEntities.MockDbSetTools)
			.SetUpDbSet(db => db.Players, mockEntities.MockDbSetPlayers)
			.SetUpDbSet(db => db.Spawnsets, mockEntities.MockDbSetSpawnsets)
			.SetUpDbSet(db => db.CustomLeaderboards, mockEntities.MockDbSetCustomLeaderboards)
			.SetUpDbSet(db => db.CustomEntries, mockEntities.MockDbSetCustomEntries)
			.SetUpDbSet(db => db.CustomEntryData, mockEntities.MockDbSetCustomEntryData);

		string spawnsetsPath = Path.Combine(TestUtils.ResourcePath, "Spawnsets");
		string replaysPath = Path.Combine(TestUtils.ResourcePath, "Replays");

		Mock<IFileSystemService> fileSystemService = new();
		fileSystemService.Setup(m => m.GetPath(DataSubDirectory.Spawnsets)).Returns(spawnsetsPath);
		fileSystemService.Setup(m => m.GetPath(DataSubDirectory.CustomEntryReplays)).Returns(replaysPath);

		Directory.CreateDirectory(replaysPath);

		Mock<ILogger<SpawnsetHashCache>> spawnsetHashCacheLogger = new();
		Mock<SpawnsetHashCache> spawnsetHashCache = new(fileSystemService.Object, spawnsetHashCacheLogger.Object);
		Mock<ILogger<CustomEntryProcessor>> customEntryProcessorLogger = new();
		Mock<IWebHostEnvironment> environment = new();

		const string secret = "secretsecretsecr";
		Dictionary<string, object> appSettings = new()
		{
			["CustomLeaderboardSecrets"] = new Dictionary<string, string>
			{
				["InitializationVector"] = secret,
				["Password"] = secret,
				["Salt"] = secret,
			},
		};
		ConfigurationBuilder builder = new();
		builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(appSettings))));
		IConfigurationRoot configuration = builder.Build();

		_encryptionWrapper = new(secret, secret, secret);
		_customEntryProcessor = new(_dbContext.Object, customEntryProcessorLogger.Object, spawnsetHashCache.Object, fileSystemService.Object, environment.Object, configuration, new LogContainerService());

		byte[] spawnsetFileContents = File.ReadAllBytes(Path.Combine(spawnsetsPath, "V3"));
		if (SpawnsetBinary.TryParse(spawnsetFileContents, out SpawnsetBinary? spawnsetBinary))
			_v3Hash = MD5.HashData(spawnsetBinary.ToBytes());
		else
			Assert.Fail("Spawnset could not be parsed.");

		_fakeReplay = BuildFakeReplay(spawnsetFileContents);
	}

	private static byte[] BuildFakeReplay(byte[] spawnsetFileContents)
	{
		const string name = "user";

		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Seek(50, SeekOrigin.Begin);
		bw.Write(name.Length);
		for (int i = 0; i < name.Length; i++)
			bw.Write((byte)name[i]);

		bw.Seek(26, SeekOrigin.Current);
		bw.Write(spawnsetFileContents.Length);
		bw.Write(spawnsetFileContents);

		return ms.ToArray();
	}

	private AddUploadRequest CreateUploadRequest(double time, int playerId, int status, string clientVersion)
	{
		AddUploadRequest uploadRequest = new()
		{
			TimeInSeconds = time,
			PlayerId = playerId,
			ClientVersion = clientVersion,
			SurvivalHashMd5 = _v3Hash,
			PlayerName = $"TestPlayer{playerId}",
			Client = "DevilDaggersCustomLeaderboards",
			Status = status,
			ReplayData = _fakeReplay,
			GameData = new(),
		};
		uploadRequest = uploadRequest with { Validation = HttpUtility.HtmlEncode(_encryptionWrapper.EncryptAndEncode(uploadRequest.CreateValidation())) };
		return uploadRequest;
	}

	[TestMethod]
	public async Task ProcessUploadRequest_ExistingPlayer_ExistingEntry_NoHighscore()
	{
		AddUploadRequest uploadRequest = CreateUploadRequest(10, 1, 3, TestConstants.DdclVersion);
		GetUploadSuccess uploadSuccess = await _customEntryProcessor.ProcessUploadRequestAsync(uploadRequest);

		_dbContext.Verify(db => db.SaveChangesAsync(default), Times.AtLeastOnce);
		Assert.AreEqual(1, uploadSuccess.TotalPlayers);
		Assert.IsTrue(uploadSuccess.Message.StartsWith("No new highscore"));
	}

	[TestMethod]
	public async Task ProcessUploadRequest_ExistingPlayer_ExistingEntry_NewHighscore()
	{
		AddUploadRequest uploadRequest = CreateUploadRequest(20, 1, 4, TestConstants.DdclVersion);
		GetUploadSuccess uploadSuccess = await _customEntryProcessor.ProcessUploadRequestAsync(uploadRequest);

		_dbContext.Verify(db => db.SaveChangesAsync(default), Times.AtLeastOnce);
		Assert.AreEqual(1, uploadSuccess.TotalPlayers);
		Assert.IsTrue(uploadSuccess.Message.StartsWith("NEW HIGHSCORE"));
	}

	[TestMethod]
	public async Task ProcessUploadRequest_ExistingPlayer_NewEntry()
	{
		AddUploadRequest uploadRequest = CreateUploadRequest(20, 2, 5, TestConstants.DdclVersion);
		GetUploadSuccess uploadSuccess = await _customEntryProcessor.ProcessUploadRequestAsync(uploadRequest);

		_dbContext.Verify(db => db.CustomEntries.AddAsync(It.Is<CustomEntryEntity>(ce => ce.PlayerId == 2 && ce.Time == 200000), default), Times.Once);
		_dbContext.Verify(db => db.SaveChangesAsync(default), Times.AtLeastOnce);
		Assert.IsTrue(uploadSuccess.Message.StartsWith("Welcome"));
	}

	[TestMethod]
	public async Task ProcessUploadRequest_NewPlayer()
	{
		AddUploadRequest uploadRequest = CreateUploadRequest(30, 3, 3, TestConstants.DdclVersion);
		GetUploadSuccess uploadSuccess = await _customEntryProcessor.ProcessUploadRequestAsync(uploadRequest);

		_dbContext.Verify(db => db.SaveChangesAsync(default), Times.AtLeastOnce);
		_dbContext.Verify(db => db.Players.AddAsync(It.Is<PlayerEntity>(p => p.Id == 3 && p.PlayerName == "TestPlayer3"), default), Times.Once);
		_dbContext.Verify(db => db.CustomEntries.AddAsync(It.Is<CustomEntryEntity>(ce => ce.PlayerId == 3 && ce.Time == 300000), default), Times.Once);
		Assert.IsTrue(uploadSuccess.Message.StartsWith("Welcome"));
	}

	[DataTestMethod]
	[DataRow(0, false)]
	[DataRow(1, false)]
	[DataRow(2, false)]
	[DataRow(3, true)]
	[DataRow(4, true)]
	[DataRow(5, true)]
	[DataRow(6, false)]
	[DataRow(7, false)]
	[DataRow(8, false)]
	public async Task ProcessUploadRequest_InvalidStatus(int status, bool accepted)
	{
		AddUploadRequest uploadRequest = CreateUploadRequest(30, 3, status, TestConstants.DdclVersion);
		if (accepted)
			await _customEntryProcessor.ProcessUploadRequestAsync(uploadRequest);
		else
			await Assert.ThrowsExceptionAsync<CustomEntryValidationException>(async () => await _customEntryProcessor.ProcessUploadRequestAsync(uploadRequest));
	}

	[TestMethod]
	public async Task ProcessUploadRequest_Outdated()
	{
		AddUploadRequest uploadRequest = CreateUploadRequest(10, 1, 4, "0.0.0.0");
		CustomEntryValidationException ex = await Assert.ThrowsExceptionAsync<CustomEntryValidationException>(async () => await _customEntryProcessor.ProcessUploadRequestAsync(uploadRequest));

		_dbContext.Verify(db => db.SaveChangesAsync(default), Times.Never);

		Assert.IsTrue(ex.Message.Contains("unsupported and outdated"));
	}

	[TestMethod]
	public async Task ProcessUploadRequest_InvalidValidation()
	{
		AddUploadRequest uploadRequest = CreateUploadRequest(10, 1, 4, "0.0.0.0") with { Validation = "Malformed validation" };
		CustomEntryValidationException ex = await Assert.ThrowsExceptionAsync<CustomEntryValidationException>(async () => await _customEntryProcessor.ProcessUploadRequestAsync(uploadRequest));

		_dbContext.Verify(db => db.SaveChangesAsync(default), Times.Never);

		Assert.IsTrue(ex.Message.StartsWith("Could not decrypt"));
	}
}