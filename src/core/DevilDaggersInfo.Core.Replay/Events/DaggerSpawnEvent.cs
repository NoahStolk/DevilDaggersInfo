namespace DevilDaggersInfo.Core.Replay.Events;

public readonly record struct DaggerSpawnEvent(int EntityId, int A, Int16Vec3 Position, Int16Mat3x3 Orientation, bool IsShot, DaggerType DaggerType) : IEvent
{
	public void Write(BinaryWriter bw)
	{
		bw.Write((byte)0x00);
		bw.Write((byte)0x01);

		bw.Write(A);
		bw.Write(Position);
		bw.Write(Orientation);
		bw.Write(IsShot);
		bw.Write((byte)DaggerType);
	}
}
