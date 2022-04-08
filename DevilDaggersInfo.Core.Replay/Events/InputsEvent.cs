namespace DevilDaggersInfo.Core.Replay.Events;

public readonly record struct InputsEvent(byte Left, byte Right, byte Forward, byte Backward, byte Jump, byte Shoot, byte ShootHoming, short MouseX, short MouseY) : IEvent
{
	public void Write(BinaryWriter bw)
	{
		bw.Write((byte)0x09);
		bw.Write(Left);
		bw.Write(Right);
		bw.Write(Forward);
		bw.Write(Backward);
		bw.Write(Jump);
		bw.Write(Shoot);
		bw.Write(ShootHoming);
		bw.Write(MouseX);
		bw.Write(MouseY);
		bw.Write((byte)0x0a);
	}
}
