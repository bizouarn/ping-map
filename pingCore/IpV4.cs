namespace Ping.Core;

internal readonly struct IpV4
{
    public readonly byte[] Address;

    public IpV4(byte octet1, byte octet2, byte octet3, byte octet4)
    {
        Address = [octet1, octet2, octet3, octet4];
    }

    public override string ToString()
    {
        return $"{Address[0]}.{Address[1]}.{Address[2]}.{Address[3]}";
    }

    public bool IsValid()
    {
        return !(Address.Length != 4 || Address[0] < 1 || Address[0] >= 240);
    }
}