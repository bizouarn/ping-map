using System;
using System.Collections.Generic;
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
            var outputDirectory = Constantes.OutputDirectory;
            Directory.CreateDirectory(outputDirectory);

            var listOutput = new HashSet<string>();
            foreach (var file in Directory.GetFiles(Path.Combine(outputDirectory,"8")))
            {
                listOutput.Add(Path.GetFileNameWithoutExtension(file));
            }

            Parallel.ForEach(files, async filePath =>
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var outputFilePath = Path.Combine(outputDirectory, fileName + ".png");
                /*if (listOutput.Contains(fileName))
                    return;*/

                await TilesGenerator.GenerateTiles(filePath, outputFilePath);
                Console.WriteLine(fileName);
            });
            
            var sourceResolution = 256;

            for (var i = 7; i >= 0; i--)
            {
                var sourceDirectory = $"{outputDirectory}\\{i + 1}\\";
                var targetDirectory = $"{outputDirectory}\\{i}\\";

                TileMatrixBase matrix;
                if (sourceResolution > 8)
                    matrix = new BigTilesMatrix(sourceDirectory, sourceResolution);
                else
                    matrix = new TilesMatrix(sourceDirectory, sourceResolution);
                await matrix.ReduceImageMatrix(2, targetDirectory);

                sourceResolution /= 2;
            }
        }
    }
}