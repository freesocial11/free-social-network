using mobSocial.Core.Infrastructure.Media;
using NReco.VideoConverter;

namespace mobSocial.Services.MediaServices
{
    public class MobSocialVideoProcessor : IMobSocialVideoProcessor
    {
        public void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath)
        {
            WriteVideoThumbnailPicture(videoFilePath, imageFilePath, null, null);
        }

        public void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath, PictureSize size)
        {
            WriteVideoThumbnailPicture(videoFilePath, imageFilePath, size, null);
        }

        public void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath, float? frameTime)
        {
            WriteVideoThumbnailPicture(videoFilePath, imageFilePath, null, frameTime);
        }

        public void WriteVideoThumbnailPicture(string videoFilePath, string imageFilePath, PictureSize size, float? frameTime)
        {
            var ffmpeg = new FFMpegConverter();
            ffmpeg.GetVideoThumbnail(videoFilePath, imageFilePath, frameTime);
        }
    }
}