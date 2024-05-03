using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderboardEntityState : INetworkSerializable, IEquatable<LeaderboardEntityState>
{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;
    public int Exp;

    public bool Equals(LeaderboardEntityState other)
    {
        return ClientId == other.ClientId &&
            PlayerName.Equals(other.PlayerName) &&
            Exp == other.Exp;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Exp);
    }
}