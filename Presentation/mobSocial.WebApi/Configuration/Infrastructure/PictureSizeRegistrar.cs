using System.Collections.Generic;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Infrastructure.Media;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Helpers;

namespace mobSocial.WebApi.Configuration.Infrastructure
{
    public class PictureSizeRegistrar : IPictureSizeRegistrar
    {
        public void RegisterPictureSize(IList<PictureSize> pictureSizes)
        {
            var mediaSettings = mobSocialEngine.ActiveEngine.Resolve<MediaSettings>();

            //register default sizes
            pictureSizes.UpdateOrInsertSize(PictureUtility.ParsePictureSize(mediaSettings.ThumbnailPictureSize, PictureSizeNames.ThumbnailImage));

            pictureSizes.UpdateOrInsertSize(PictureUtility.ParsePictureSize(mediaSettings.SmallProfilePictureSize, PictureSizeNames.SmallProfileImage));

            pictureSizes.UpdateOrInsertSize(PictureUtility.ParsePictureSize(mediaSettings.MediumProfilePictureSize, PictureSizeNames.MediumProfileImage));

            pictureSizes.UpdateOrInsertSize(PictureUtility.ParsePictureSize(mediaSettings.SmallCoverPictureSize, PictureSizeNames.SmallCover));

            pictureSizes.UpdateOrInsertSize(PictureUtility.ParsePictureSize(mediaSettings.MediumCoverPictureSize, PictureSizeNames.MediumCover));

            pictureSizes.UpdateOrInsertSize(PictureSizeNames.OriginalProfileImage, 0, 0);

            pictureSizes.UpdateOrInsertSize(PictureSizeNames.OriginalCover, 0, 0);

            pictureSizes.UpdateOrInsertSize(PictureSizeNames.OriginalImage, 0, 0);

        }
    }
}