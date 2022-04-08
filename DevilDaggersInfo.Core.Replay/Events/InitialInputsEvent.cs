namespace DevilDaggersInfo.Core.Replay.Events;

public readonly record struct InitialInputsEvent(bool Left, bool Right, bool Forward, bool Backward, byte Jump, bool Shoot, bool ShootHoming, short MouseX, short MouseY, float LookSpeed) : IEvent
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
		bw.Write(LookSpeed);
		bw.Write((byte)0x0a);
	}
}
