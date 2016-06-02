using System.Drawing.Imaging;

namespace mobSocial.Services.MediaServices
{
    public interface IMobSocialImageProcessor
    {
        void WriteBytesToImage(byte[] imageBytes, string filePath, ImageFormat imageFormat);

        byte[] ResizeImage(byte[] imageBytes, int width, int height);

        void WriteResizedImage(byte[] imageBytes, int width, int height, string filePath, ImageFormat imageFormat);

        void WriteResizedImage(string sourceFile, string destinationFile, int width, int height, ImageFormat imageFormat);
    }
}