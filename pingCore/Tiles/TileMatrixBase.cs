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
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);

            var x = 0;
            var y = 0;

            for (var i = 0; i < Size / size; i++)
            {
                for (var j = 0; j < Size / size; j++)
                {
                    await CombineImages(i * size, j * size, i * size + size, j * size + size,
                        Path.Combine(outPath, $"{y}.{x}.png"));

                    y++;
                }

                y = 0;
                x++;
            }
        }

        public virtual async Task CombineImages(int lx, int ly, int rx, int ry, string outPath)
        {
            Console.WriteLine($"start combinedImage ... ({lx},{ly},{rx},{ry})");

            using (var combinedImage = new MagickImage(new MagickColor("#ffffff"), (rx - lx) * 255, (ry - ly) * 255))
            {
                combinedImage.ColorType = ColorType.TrueColor;
                combinedImage.Format = MagickFormat.Png;
                combinedImage.HasAlpha = true;

                for (var i = lx; i < rx && i < Size; i++)
                for (var j = ly; j < ry && j < Size; j++)
                {
                    var tile = GetTile(i, j);
                    if (tile == null)
                        continue;

                    using (var image = tile)
                    {
                        combinedImage.Composite(image, (j - ly) * 255, (i - lx) * 255, CompositeOperator.SrcOver);
                    }
                }

                combinedImage.Resize(255, 255);
                await combinedImage.WriteAsync(outPath);
            }

            Console.WriteLine("combinedImage ok");
        }

        protected abstract MagickImage GetTile(int x, int y);
    }
}