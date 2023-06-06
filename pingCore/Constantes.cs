using ImageMagick;

namespace pingCore
{
    public class Constantes
    {
        public static string BaseDir = "D:\\GIT\\pingMap\\";
        public static string InputDirectory = BaseDir + "res"; // Répertoire d'entrée contenant les fichiers
        public static string OutputDirectory = BaseDir + "www\\tiles"; // Répertoire de sortie pour les images
        public static MagickColor Green = MagickColors.Green;
    }
}