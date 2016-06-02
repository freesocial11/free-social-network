using mobSocial.Core.Infrastructure.Media;

namespace mobSocial.Services.MediaServices
{
    public interface IMobSocialVideoProcessor
    {
        void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath);

        void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath, PictureSize size);

        void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath, float? frameTime);

        void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath, PictureSize size, float? frameTime);
    }
}