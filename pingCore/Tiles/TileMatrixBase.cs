using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace pingCore.Tiles
{
    public abstract class TileMatrixBase
    {
        protected TileMatrixBase(int size)
        {
            Size = size;
        }

        public int Size;
        public async Task ReduceImageMatrix(int size, string outPath)
        {
            var x = 0;
            var y = 0;
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);
            for (int i = 0; i < Size / size; i ++ )
            {
                for (int j = 0; j < Size / size; j ++)
                {
                    await CombineImages(i * size,j* size,i* size + size,j* size +size, outPath + "\\" + x + "." + y + ".png" );

                    y++;
                }

                y = 0;
                x++;
            }
        }

        public virtual async Task CombineImages(int lx, int ly, int rx, int ry, string outPath)
        {
            Console.WriteLine($"start combinedImage ... ({lx},{ly},{rx},{ry})");
            using (MagickImage combinedImage = new MagickImage(new MagickColor("#ffffff"), (rx-lx)*255, (ry-ly)*255))
            {
                combinedImage.ColorType = ColorType.TrueColor;
                combinedImage.Format = MagickFormat.Png;
                combinedImage.HasAlpha = true;

                for (int i = lx; i < rx && i < Size; i++)
                {
                    for (int j = ly; j < ry && j < Size; j++)
                    {
                        var tile = GetTile(i, j);
                        if(tile == null)
                            continue;
                        using (MagickImage image = tile)
                        {
                            combinedImage.Composite(image,  (j-ly) * 255, (i-lx) * 255, CompositeOperator.SrcOver);
                        }
                    }
                }

                combinedImage.Resize(255,255);
                await combinedImage.WriteAsync(outPath);
            }
            Console.WriteLine("combinedImage ok");
        }
        
        protected abstract MagickImage GetTile(int x, int y);
    }
}
