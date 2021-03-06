namespace DevilDaggersInfo.Common.Tests;

[TestClass]
public class NullTerminatorTests
{
	[TestMethod]
	public void TestNullTerminatedStrings()
	{
		byte[] buffer = new byte[] { 0x68, 0x61, 0x6E, 0x64, 0, 0x64, 0x64, 0 };

		using MemoryStream ms = new(buffer);
		using BinaryReader br = new(ms);
		string hand = br.ReadNullTerminatedString();
		Assert.AreEqual("hand", hand);
		string dd = br.ReadNullTerminatedString();
		Assert.AreEqual("dd", dd);
	}
}
