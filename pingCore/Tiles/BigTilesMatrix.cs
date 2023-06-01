using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;


namespace pingCore.Tiles
{
    public class BigTilesMatrix : TileMatrixBase, IDisposable
    {
        private string[][] _matrix;
        List<WeakReference> _cache = new List<WeakReference>();
        private string _tempDir = "C:\\TEMP\\IM";

        public BigTilesMatrix(string dir, int size = 255) : base(size)
        {
            Directory.CreateDirectory(_tempDir);
            MagickNET.SetTempDirectory(_tempDir);
            _matrix = new string[size][];
            for (var i = 0; i < size ; i++)
            {
                _matrix[i] = new string[size];
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    string imagePath = Path.Combine(dir, $"{i}.{j}.png");
                    if (File.Exists(imagePath))
                    {
                        _matrix[i][j] = imagePath;
                    }
                }

                Console.WriteLine(i + "/" + size);
            }
        }

        protected override MagickImage GetTile(int x, int y)
        {
            var ret = new MagickImage(_matrix[x][y]);
            _cache.Add(new WeakReference(ret));
            return ret;
        }

        public void ClearCache()
        {
            Directory.Delete(_tempDir, true);
            Directory.CreateDirectory(_tempDir);
            for (var i = 0; i < _cache.Count; i++)
            {
                if (_cache[i].Target is IMagickImage image)
                {
                    image.Dispose();
                }
            }
            _cache.RemoveAll(reference => !reference.IsAlive);
        }

        
        public override async Task CombineImages(int lx, int ly, int rx, int ry, string outPath)
        {
            base.CombineImages(lx, ly, rx, ry, outPath);
            ClearCache();
        }


        public void Dispose()
        {
            ClearCache();
        }
    }
}
