using DevilDaggersInfo.Core.Mod;

string inputPath = args[0];
string outputDirectory = args[1];

string fileName = Path.GetFileName(inputPath);
byte[] fileContents = File.ReadAllBytes(inputPath);
ModBinary modBinary = new(fileName, fileContents, true);
modBinary.ExtractAssets(outputDirectory);