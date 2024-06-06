using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ping.Core;
using Ping.Core.Tiles;

namespace ResToImage;

internal class Program
{
    private static async Task Main()
    {
        var files = Directory.GetFiles(Constantes.InputDirectory);
        var outputDirectory = Constantes.OutputDirectory;
        Directory.CreateDirectory(outputDirectory);

        var listOutput = new HashSet<string>();
        foreach (var file in Directory.GetFiles(Path.Combine(outputDirectory, "8")))
            listOutput.Add(Path.GetFileNameWithoutExtension(file));

        await Parallel.ForEachAsync(files, async (filePath, _) => {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var outputFilePath = Path.Combine(outputDirectory, fileName + ".png");
            /*if (listOutput.Contains(fileName))
                return;*/

            await TilesGenerator.GenerateTiles(filePath, outputFilePath);
            Console.WriteLine(fileName);
        });

        return;

        var sourceResolution = 256;

        for (var i = 7; i >= 0; i--)
        {
            var sourceDirectory = Path.Combine(outputDirectory, (i + 1) + "");
            var targetDirectory = Path.Combine(outputDirectory, i + "");

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
