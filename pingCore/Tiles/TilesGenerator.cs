using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace pingCore.Tiles
{
    public class TilesGenerator
    {
        public static async Task<bool> GenerateTiles(string inputFilePath, string outputFilePath)
        {
            var lines = await File.ReadAllLinesAsync(inputFilePath);
            var height = lines.Length;
            if (lines.Length <= 0)
                return false;
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
                await image.WriteAsync(outputFilePath); // Enregistrer l'image au format PNG
            }

            return true;
        }
    }
}