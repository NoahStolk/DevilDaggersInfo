using DevilDaggersInfo.Core.Replay.Events.Interfaces;

namespace DevilDaggersInfo.Core.Replay;

public class ReplayBinary<TReplayBinaryHeader>
	where TReplayBinaryHeader : IReplayBinaryHeader<TReplayBinaryHeader>
{
	public ReplayBinary(byte[] contents)
	{
		using MemoryStream ms = new(contents);
		using BinaryReader br = new(ms);

		Header = TReplayBinaryHeader.CreateFromBinaryReader(br);

		int compressedDataLength;
		if (TReplayBinaryHeader.UsesLengthPrefixedEvents)
			compressedDataLength = br.ReadInt32();
		else
			compressedDataLength = (int)(contents.Length - br.BaseStream.Position);

		EventsData = ReplayEventsParser.Parse(br.ReadBytes(compressedDataLength));
	}

	public ReplayBinary(TReplayBinaryHeader header, byte[] compressedEvents)
	{
		Header = header;
		EventsData = ReplayEventsParser.Parse(compressedEvents);
	}

	public ReplayBinary(TReplayBinaryHeader header, ReplayEventsData eventsData)
	{
		Header = header;
		EventsData = eventsData;
	}

	public TReplayBinaryHeader Header { get; }
	public ReplayEventsData EventsData { get; }

	public static ReplayBinary<TReplayBinaryHeader> CreateDefault()
	{
		return new(
			header: TReplayBinaryHeader.CreateDefault(),
			compressedEvents: ReplayEventsCompiler.CompileEvents(new List<IEvent> { default(EndEvent) })); // TODO: Check if this is valid by saving this default empty replay and playing it in DD. Might need an initial inputs event.
	}

	public byte[] Compile()
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);

		bw.Write(Header.ToBytes());

		byte[] compressedEvents = ReplayEventsCompiler.CompileEvents(EventsData.Events.ToList());
		bw.Write(compressedEvents.Length);
		bw.Write(compressedEvents);

		return ms.ToArray();
	}
}
