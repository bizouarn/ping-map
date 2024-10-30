using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace Ping.Core.Tiles;

public abstract class TileMatrixBase
{
    public readonly short Size;

    protected TileMatrixBase(short size)
    {
        Size = size;
    }

    public async Task ReduceImageMatrix(short size, string outPath)
    {
        Directory.CreateDirectory(outPath);

        short x = 0;
        short y = 0;

        var taskList = new Task[Size];
        Array.Fill(taskList, Task.CompletedTask);

        for (short i = 0; i < Size / size; i++)
        {
            
            for (short j = 0; j < Size / size; j++)
            {
                taskList[j] = CombineImages(
                    (short) (i * size), 
                    (short) (j * size), 
                    (short) (i * size + size),
                    (short) (j * size + size),
                    Path.Combine(outPath, $"{x}.{y}.png")
                );

                y++;
            }

            await Task.WhenAll(taskList);

            y = 0;
            x++;
        }
    }

    public async Task CombineImages(short lx, short ly, short rx, short ry, string outPath)
    {
        Console.WriteLine($"Combined image: ({lx}, {ly}, {rx}, {ry})");

        if (rx - lx == 1 && ry - ly == 1)
        {
            // Copier le fichier plutôt que de le combiner
            using var tile = GetTile(lx, ly);
            if (tile != null)
                await tile.WriteAsync(outPath);
            else
                Console.WriteLine("No tile found at the specified location.");
        }
        else
        {
            using var combinedImage = new MagickImage(Constantes.Black, (rx - lx) * 255, (ry - ly) * 255);
            combinedImage.ColorType = ColorType.TrueColorAlpha;
            combinedImage.Format = MagickFormat.Png;

            const int tileSize = 255;

            for (var i = lx; i < rx && i < Size; i++)
            for (var j = ly; j < ry && j < Size; j++)
            {
                using var tile = GetTile(i, j);
                if (tile == null)
                    continue;
                combinedImage.Composite(tile, (j - ly) * tileSize, (i - lx) * tileSize,
                    CompositeOperator.SrcOver);
            }

            if (rx - lx == 2 && ry - ly == 2)
                combinedImage.AdaptiveResize(tileSize, tileSize);
            else
                combinedImage.Resize(tileSize, tileSize);
            await combinedImage.WriteAsync(outPath);
        }
    }

    protected abstract MagickImage GetTile(short x, short y);
}
