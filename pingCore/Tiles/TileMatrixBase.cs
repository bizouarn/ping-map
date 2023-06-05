using System;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace pingCore.Tiles
{
    public abstract class TileMatrixBase
    {
        public int Size;

        protected TileMatrixBase(int size)
        {
            Size = size;
        }

        public async Task ReduceImageMatrix(int size, string outPath)
        {
            Directory.CreateDirectory(outPath);

            var x = 0;
            var y = 0;

            for (var i = 0; i < Size / size; i++)
            {
                for (var j = 0; j < Size / size; j++)
                {
                    await CombineImages(i * size, j * size, i * size + size, j * size + size,
                        Path.Combine(outPath, $"{x}.{y}.png"));

                    y++;
                }

                y = 0;
                x++;
            }
        }

        public virtual async Task CombineImages(int lx, int ly, int rx, int ry, string outPath)
        {
            Console.WriteLine($"Combined image: ({lx}, {ly}, {rx}, {ry})");

            if (rx - lx == 1 && ry - ly == 1)
                // Copier le fichier plutôt que de le combiner
                using (var tile = GetTile(lx, ly))
                {
                    if (tile != null)
                    {
                        await tile.WriteAsync(outPath);
                        Console.WriteLine("Image copied.");
                    }
                    else
                    {
                        Console.WriteLine("No tile found at the specified location.");
                    }
                }
            else
                using (var combinedImage = new MagickImage(MagickColors.Transparent, (rx - lx) * 255, (ry - ly) * 255))
                {
                    combinedImage.ColorType = ColorType.TrueColorAlpha;
                    combinedImage.Format = MagickFormat.Png;

                    for (var i = lx; i < rx && i < Size; i++)
                    for (var j = ly; j < ry && j < Size; j++)
                        using (var tile = GetTile(i, j))
                        {
                            if (tile == null)
                                continue;

                            combinedImage.Composite(tile, (j - ly) * 255, (i - lx) * 255, CompositeOperator.SrcOver);
                        }

                    combinedImage.Resize(255, 255);
                    await combinedImage.WriteAsync(outPath);
                    Console.WriteLine("Images combined.");
                }
        }


        protected abstract MagickImage GetTile(int x, int y);
    }
}