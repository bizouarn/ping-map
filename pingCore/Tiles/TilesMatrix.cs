using System;
using System.IO;
using ImageMagick;

namespace Ping.Core.Tiles;

public class TilesMatrix : TileMatrixBase, IDisposable
{
    private readonly MagickImage[][] _matrix;

    public TilesMatrix(string dir, int size = 255) : base(size)
    {
        _matrix = new MagickImage[size][];

        for (var i = 0; i < size; i++) _matrix[i] = new MagickImage[size];

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                var imagePath = Path.Combine(dir, $"{i}.{j}.png");
                if (File.Exists(imagePath)) _matrix[i][j] = new MagickImage(imagePath);
            }

            Console.WriteLine($"{i}/{size}");
        }
    }

    public void Dispose()
    {
        foreach (var row in _matrix)
        foreach (var image in row)
            image?.Dispose();
    }

    protected override MagickImage GetTile(int x, int y)
    {
        return _matrix[x][y];
    }
}