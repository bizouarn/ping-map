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

                Console.WriteLine(fileName);
            }

            TileMatrixBase matrix = new BigTilesMatrix("D:\\GIT\\pingMap\\www\\tiles\\src\\", 256);
            await matrix.ReduceImageMatrix(16,$"D:\\GIT\\pingMap\\www\\tiles\\0\\");
            await matrix.ReduceImageMatrix(8,$"D:\\GIT\\pingMap\\www\\tiles\\1\\");
            await matrix.ReduceImageMatrix(4,$"D:\\GIT\\pingMap\\www\\tiles\\2\\");
            await matrix.ReduceImageMatrix(2,$"D:\\GIT\\pingMap\\www\\tiles\\3\\");
            await matrix.ReduceImageMatrix(1,$"D:\\GIT\\pingMap\\www\\tiles\\4\\");   
        }
    }
}