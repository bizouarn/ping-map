using System;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace pingCore.Tiles
{
    public class TilesGenerator
    {
        public static async Task<bool> GenerateTiles(string inputFilePath, string outputFilePath, int[] zooms = null)
        {
            var lines = await File.ReadAllLinesAsync(inputFilePath);
            zooms ??= new[] {-1};

            if (lines.Length <= 0)
                return false;

            foreach (var zoom in zooms)
                switch (zoom)
                {
                    case 1:
                        await GenerateTiles1(lines, outputFilePath);
                        break;
                    case -1:
                        await GenerateTilesSrc(lines, outputFilePath);
                        break;
                }

            return true;
        }

        private static async Task<bool> GenerateTiles1(string[] lines, string outputFilePath)
        {
            var output = GetOutputPath(outputFilePath, "1");
            if (File.Exists(output))
                return false;

            var green = 0;

            foreach (var line in lines)
            foreach (var charc in line)
                if (charc == '1')
                    green++;

            green /= 255;
            if (green > 255)
                green = 255;

            var color = new MagickColor($"#00{green.ToString("X2")}00");

            using (var image = new MagickImage(MagickColor.FromRgb(0, 0, 0), 255, 255))
            {
                image.Format = MagickFormat.Png;
                var drawable = new Drawables().FillColor(color);
                drawable = drawable.Rectangle(0, 0, 255, 255);
                image.Draw(drawable);
                await image.WriteAsync(output); // Enregistrer l'image au format PNG
                Console.WriteLine($"L'image {output}.png a été créée et enregistrée dans le répertoire 'image'.");
            }

            return true;
        }

        private static async Task<bool> GenerateTilesSrc(string[] lines, string outputFilePath)
        {
            var output = GetOutputPath(outputFilePath, "src");
            if (File.Exists(output))
                return false;

            var height = lines.Length;
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
                            drawable = drawable.Point(x, y);
                }

                image.Draw(drawable);
                await image.WriteAsync(output); // Enregistrer l'image au format PNG
            }

            return true;
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