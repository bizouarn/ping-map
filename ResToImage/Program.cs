using System;
using System.IO;
using System.Threading.Tasks;
using pingCore;
using pingCore.Tiles;

namespace ResToImage
{
    internal class Program
    {
        private static async Task Main()
        {
            // Obtenir la liste des fichiers dans le répertoire d'entrée
            var files = Directory.GetFiles(Constantes.InputDirectory);

            if(!Directory.Exists(Constantes.OutputDirectory))
                Directory.CreateDirectory(Constantes.OutputDirectory);

            foreach (var filePath in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var outputFilePath = Path.Combine(Constantes.OutputDirectory, fileName + ".png");
                if (File.Exists(outputFilePath))
                    continue;

                await TilesGenerator.GenerateTiles(filePath, outputFilePath);

                Console.WriteLine($"L'image {fileName}.png a été créée et enregistrée dans le répertoire 'image'.");
            }
        }
    }
}