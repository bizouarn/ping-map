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
            var files = Directory.GetFiles(Constantes.InputDirectory);
            Directory.CreateDirectory(Constantes.OutputDirectory);

            Parallel.ForEach(files, async filePath =>
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var outputFilePath = Path.Combine(Constantes.OutputDirectory, fileName + ".png");

                await TilesGenerator.GenerateTiles(filePath, outputFilePath);

                Console.WriteLine(fileName);
            });

            string outputDirectory = Constantes.OutputDirectory;
            int sourceResolution = 256;

            for (int i = 7; i >= 0; i--)
            {
                string sourceDirectory = $"{outputDirectory}\\{i + 1}\\";
                string targetDirectory = $"{outputDirectory}\\{i}\\";

                TileMatrixBase matrix = new BigTilesMatrix(sourceDirectory, sourceResolution);
                await matrix.ReduceImageMatrix(2, targetDirectory);

                sourceResolution /= 2;
            }

        }
    }
}