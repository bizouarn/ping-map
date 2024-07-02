using System.Collections.Generic;
using System.Linq;
using ImageMagick;

namespace Ping.Core;

public class Constantes
{
    public static readonly string BaseDir = @"D:\git\ping-map\";
    public static readonly string InputDirectory = BaseDir + "res"; // Répertoire d'entrée contenant les fichiers
    public static readonly string OutputDirectory = BaseDir + "www/tiles"; // Répertoire de sortie pour les images
    public static readonly MagickColor Green = MagickColors.Green;
    public static readonly MagickColor Black = MagickColors.Black;
    public static readonly byte[] ByteRange = GetByteArray();

    private static byte[] GetByteArray()
    {
        var ret = new List<byte>();
        foreach (byte val in Enumerable.Range(byte.MinValue, byte.MaxValue)) ret.Add(val);
        return ret.ToArray();
    }
}