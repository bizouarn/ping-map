using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace Ping.Core.Tiles
{
    public class TilesGenerator
    {
        public static async Task GenerateTiles(string inputFilePath, string outputFilePath)
        {
            var bits = await File.ReadAllBytesAsync(inputFilePath);

            if (bits.Length <= 0)
                return;

            await GenerateTiles8(bits, outputFilePath);
        }

        private static async Task GenerateTiles8(byte[] bits, string outputFilePath)
        {
            var output = GetOutputPath(outputFilePath, "8");
            var height = bits.Length / bits.Length;
            var width = height;

            using var image = new MagickImage(MagickColor.FromRgb(0, 0, 0), width, height);
            image.Format = MagickFormat.Png;
            var drawable = new Drawables().FillColor(Constantes.Green);

            var index = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                    if (bits[index] == 1)
                        drawable.Point(x, y);
                index++;
            }

            image.Draw(drawable);
            await image.WriteAsync(output);
        }

        private static string GetOutputPath(string outputFilePath, string subDir)
        {
            var outputPath = Path.Combine(Path.GetDirectoryName(outputFilePath), subDir);
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            return Path.Combine(outputPath, Path.GetFileName(outputFilePath));
        }
    }
}