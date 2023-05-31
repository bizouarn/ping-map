using System;
using System.IO;
using ImageMagick;

namespace ResToImage
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var inputDirectory = "D:\\GIT\\pingMap\\res"; // Répertoire d'entrée contenant les fichiers
            var outputDirectory = "D:\\GIT\\pingMap\\www\\image"; // Répertoire de sortie pour les images

            // Obtenir la liste des fichiers dans le répertoire d'entrée
            var files = Directory.GetFiles(inputDirectory);

            foreach (var filePath in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var outputFilePath = Path.Combine(outputDirectory, fileName + ".png");
                if (File.Exists(outputFilePath))
                    continue;

                var lines = File.ReadAllLines(filePath);
                var height = lines.Length;
                var width = lines[0].Length;

                using (var image = new MagickImage(MagickColor.FromRgb(0, 0, 0), width, height))
                {
                    image.Format = MagickFormat.Png;

                    for (var y = 0; y < height; y++)
                    {
                        var line = lines[y].Trim();
                        for (var x = 0; x < width; x++)
                            if (line[x] == '1')
                            {
                                var green = new MagickColor("#00FF00");
                                var drawable = new Drawables().FillColor(green).Point(x, y);
                                image.Draw(drawable);
                            }
                    }

                    image.Write(outputFilePath); // Enregistrer l'image au format PNG
                }

                Console.WriteLine($"L'image {fileName}.png a été créée et enregistrée dans le répertoire 'image'.");
            }
        }
    }
}