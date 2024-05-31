using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace Ping.Core.Tiles;

public class BigTilesMatrix : TileMatrixBase, IDisposable
{
    private readonly List<MagickImage> _cache = [];
    private readonly string[][] _matrix;
    private readonly string _tempDir = Path.Combine("%TEMP%", "IM");

    public BigTilesMatrix(string dir, int size = 255) : base(size)
    {
        Directory.CreateDirectory(_tempDir);
        MagickNET.SetTempDirectory(_tempDir);
        _matrix = new string[size][];

        for (var i = 0; i < size; i++)
            _matrix[i] = new string[size];

        Parallel.For(0, size, i =>
        {
            for (var j = 0; j < size; j++)
            {
                var imagePath = Path.Combine(dir, $"{i}.{j}.png");
                if (File.Exists(imagePath))
                    _matrix[i][j] = imagePath;
            }

            Console.WriteLine($"{i}/{size}");
        });
    }

    public void Dispose()
    {
        ClearCache();
    }

    protected override MagickImage GetTile(int x, int y)
    {
        var fileName = _matrix[x][y];
        if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            return null;

        var ret = new MagickImage(fileName);
        _cache.Add(ret);
        return ret;
    }

    private void ClearCache()
    {
        Directory.Delete(_tempDir, true);
        Directory.CreateDirectory(_tempDir);

        foreach (var image in _cache)
            image.Dispose();

        _cache.Clear();
    }

    public override async Task CombineImages(int lx, int ly, int rx, int ry, string outPath)
    {
        await base.CombineImages(lx, ly, rx, ry, outPath);
        ClearCache();
    }
}