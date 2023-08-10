using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace pingCore.Tiles
{
    public class TilesGenerator
    {
        public static async Task GenerateTiles(string inputFilePath, string outputFilePath)
        {
            var lines = await File.ReadAllLinesAsync(inputFilePath);

            if (lines.Length <= 0)
                return;

            await GenerateTiles8(lines, outputFilePath);
        }

        private static async Task GenerateTiles8(IReadOnlyList<string> lines, string outputFilePath)
        {
            var output = GetOutputPath(outputFilePath, "8");
            var height = lines.Count;
            var width = lines[0].Length;

            using (var image = new MagickImage(MagickColor.FromRgb(0, 0, 0), width, height))
            {
                image.Format = MagickFormat.Png;
                var drawable = new Drawables().FillColor(Constantes.Green);

                for (var y = 0; y < height; y++)
                {
                    var line = lines[y].Trim();
                    for (var x = 0; x < width; x++)
                        if (line[x] == '1')
                            drawable.Point(x, y);
                }

                image.Draw(drawable);
                await image.WriteAsync(output); // Enregistrer l'image au format PNG
            }
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