using System;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;
using Ping.Core.Utils;

namespace Ping.Core.Tiles;

public class TilesGenerator
{
    public static async Task GenerateTiles(string inputFilePath, string outputFilePath)
    {
        var bits = await File.ReadAllBytesAsync(inputFilePath);

        if (bits.Length <= 0)
            return;

        var containsZero = bits.ContainsOneBit(0);
        var containsOne = bits.ContainsOneBit(1);
        var size = (short) Math.Round(Math.Sqrt(bits.Length));
        const int desiredSize = 256;

        MagickImage image;
        if (containsOne && containsZero)
            image = GenerateTiles8(bits, size);
        else if (containsOne)
            image = GenerateColorTiles(Constantes.Green, size);
        else
            image = GenerateColorTiles(Constantes.Black, size);
        var output = GetOutputPath(outputFilePath, "8");
        if (image.Height != desiredSize)
            image.Scale(desiredSize, desiredSize);
        await image.WriteAsync(output);
        image.Dispose();
    }

    private static MagickImage GenerateTiles8(ReadOnlySpan<byte> bits, short size)
    {
        var image = new MagickImage(Constantes.Black, size, size);
        image.Format = MagickFormat.Png;
        var drawable = new Drawables().FillColor(Constantes.Green);

        var index = 0;
        for (var y = 0; y < size; y++)
        for (var x = 0; x < size; x++)
        {
            if (bits[index] == 1)
                drawable.Point(x, y);
            index++;
        }

        image.Draw(drawable);
        return image;
    }

    private static MagickImage GenerateColorTiles(MagickColor color, short size)
    {
        var image = new MagickImage(color, size, size);
        image.Format = MagickFormat.Png;
        return image;
    }

    private static string GetOutputPath(string outputFilePath, string subDir)
    {
        var outputPath = Path.Combine(Path.GetDirectoryName(outputFilePath), subDir);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        return Path.Combine(outputPath, Path.GetFileName(outputFilePath));
    }
}