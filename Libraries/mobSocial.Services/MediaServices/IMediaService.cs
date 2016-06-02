using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.MediaEntities;
using mobSocial.Data.Enum;

namespace mobSocial.Services.MediaServices
{
    public interface IMediaService : IBaseEntityService<Media>
    {
       void AttachMediaToEntity<T>(int entityId, int mediaId) where T : BaseEntity;

        void AttachMediaToEntity<T>(T entity, Media media) where T : BaseEntity;

        void DetachMediaFromEntity<T>(int entityId, int mediaId) where T : BaseEntity;

        void DetachMediaFromEntity<T>(T entity, Media media) where T : BaseEntity;

        void ClearEntityMedia<T>(int entityId) where T : BaseEntity;

        void ClearEntityMedia<T>(T entity) where T : BaseEntity;

        void ClearMediaAttachments(Media media);

        string GetPictureUrl(int pictureId, int width = 0, int height = 0, bool returnDefaultIfNotFound = false);

        string GetPictureUrl(Media picture, int width = 0, int height = 0, bool returnDefaultIfNotFound = false);

        string GetPictureUrl(int pictureId, string sizeName, bool returnDefaultIfNotFound = false);

        string GetPictureUrl(Media picture, string sizeName, bool returnDefaultIfNotFound = false);

        string GetVideoUrl(int mediaId);

        string GetVideoUrl(Media media);

        void WritePictureBytes(Media picture, MediaSaveLocation saveLocation);

        void WriteVideoBytes(Media video);

        void WriteOtherMediaBytes(Media media, MediaSaveLocation saveLocation);

        IList<Media> GetEntityMedia<TEntityType>(int entityId, MediaType mediaType) where TEntityType : BaseEntity;
    }
}