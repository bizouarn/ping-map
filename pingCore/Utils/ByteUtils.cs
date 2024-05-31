using System;

namespace Ping.Core.Utils;

public static class ByteUtils
{
    public static bool ContainsOneBit(this ReadOnlySpan<byte> span, byte value)
    {
        return span.Contains(value);
    }

    public static bool ContainsOneBit(this byte[] byteArray, byte value)
    {
        return ContainsOneBit((ReadOnlySpan<byte>) byteArray, value);
    }
}