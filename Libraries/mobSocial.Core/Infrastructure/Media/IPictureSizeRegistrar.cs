using System.Collections.Generic;

namespace mobSocial.Core.Infrastructure.Media
{
    public interface IPictureSizeRegistrar
    {
        void RegisterPictureSize(IList<PictureSize> pictureSizes);
    }
}