namespace Ping.Core;

internal class IpV4
{
    public readonly byte[] address;

    public IpV4(byte octet1, byte octet2, byte octet3, byte octet4)
    {
        address = [octet1, octet2, octet3, octet4];
    }

    public override string ToString()
    {
        return $"{address[0]}.{address[1]}.{address[2]}.{address[3]}";
    }

    public bool IsValid()
    {
        return !(address.Length != 4 || address[0] < 1 || address[0] >= 240);
    }
}