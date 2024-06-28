using System.Linq;

namespace Ping.Core;

internal class IpV4
{
    public readonly int[] address;

    public IpV4(int octet1, int octet2, int octet3, int octet4)
    {
        address = [octet1, octet2, octet3, octet4];
    }

    public override string ToString()
    {
        return $"{address[0]}.{address[1]}.{address[2]}.{address[3]}";
    }

    public bool IsValid()
    {
        // Il doit y avoir exactement 4 octets
        // Le premier octet doit être entre 1 et 253
        if (address.Length != 4 || address[0] < 1 || address[0] >= 240) return false;

        foreach (var octet in address.Skip(1)) // On vérifie les octets suivants (à partir du deuxième)
            if (octet is < 0 or > 255)
                return false; // Les octets suivants doivent être entre 0 et 255

        return true;
    }
}