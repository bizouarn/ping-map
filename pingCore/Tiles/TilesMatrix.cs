using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace pingCore.Tiles
{
    public class TilesMatrix : TileMatrixBase, IDisposable
    {
        private MagickImage[][] _matrix;
        public TilesMatrix(string dir, int size = 255) : base(size)
        {
            _matrix = new MagickImage[size][];
            for (var i = 0; i < size ; i++)
            {
                _matrix[i] = new MagickImage[size];
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    string imagePath = Path.Combine(dir, $"{i}.{j}.png");
                    if (File.Exists(imagePath))
                    {
                        _matrix[i][j] = new MagickImage(imagePath);
                    }
                }

                Console.WriteLine(i + "/" + size);
            }
        }

        public void Dispose()
        {
            for(var i = 0; i < _matrix.Length; i++)
            {
                for(var j = 0; j < _matrix[i].Length; j++)
                {
                   _matrix[i][j].Dispose();
                }
            }
        }

        protected override MagickImage GetTile(int x, int y){
            return _matrix[x][y];
        }
    }
}