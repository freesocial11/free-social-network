using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using mobSocial.Core;
using mobSocial.Core.Data;
using mobSocial.Core.Exception;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.MediaEntities;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Enum;
using mobSocial.Data.Helpers;
using mobSocial.Services.Helpers;

namespace mobSocial.Services.MediaServices
{
    public class MediaService : MobSocialEntityService<Media>, IMediaService
    {
        private readonly IDataRepository<EntityMedia> _entityMediaRepository;
        private readonly MediaSettings _mediaSettings;
        private readonly GeneralSettings _generalSettings;
        private readonly IMobSocialImageProcessor _imageProcessor;
        private readonly IMobSocialVideoProcessor _videoProcessor;

        public MediaService(IDataRepository<Media> dataRepository, 
            IDataRepository<EntityMedia> entityMediaRepository, 
            MediaSettings mediaSettings, 
            GeneralSettings generalSettings, 
            IMobSocialImageProcessor imageProcessor, 
            IMobSocialVideoProcessor videoProcessor) : base(dataRepository)
        {
            _entityMediaRepository = entityMediaRepository;
            _mediaSettings = mediaSettings;
            _generalSettings = generalSettings;
            _imageProcessor = imageProcessor;
            _videoProcessor = videoProcessor;
        }

        public void AttachMediaToEntity<T>(int entityId, int mediaId) where T : BaseEntity
        {
            if (mediaId == 0)
            {
                throw new mobSocialException("Can't attach entity with media with Id '0'");
            }
           //insert entity picture only if it doesn't exist
            var insertRequired  =
                !_entityMediaRepository.Get(x => x.EntityId == entityId && x.MediaId == mediaId).Any();

            if (!insertRequired) return;


            var entityPicture = new EntityMedia()
            {
                EntityId = entityId,
                MediaId = mediaId,
                EntityName = typeof(T).Name
            };

            _entityMediaRepository.Insert(entityPicture);
        }

        public void AttachMediaToEntity<T>(T entity, Media media) where T : BaseEntity
        {
            if (media.Id == 0)
            {
                Repository.Insert(media);
            }
            AttachMediaToEntity<T>(entity.Id, media.Id);
        }

        public void DetachMediaFromEntity<T>(int entityId, int mediaId) where T : BaseEntity
        {
            _entityMediaRepository.Delete(x => x.EntityId == entityId && x.MediaId == mediaId);
        }

        public void DetachMediaFromEntity<T>(T entity, Media media) where T : BaseEntity
        {
            DetachMediaFromEntity<T>(entity.Id, media.Id);
        }

        public void ClearMediaAttachments(Media media)
        {
            _entityMediaRepository.Delete(x => x.MediaId == media.Id);
        }

        public void ClearEntityMedia<T>(int entityId) where T : BaseEntity
        {
            _entityMediaRepository.Delete(x => x.EntityId == entityId);
        }

        public void ClearEntityMedia<T>(T entity) where T : BaseEntity
        {
            ClearEntityMedia<T>(entity.Id);
        }

        public string GetPictureUrl(int pictureId, int width = 0, int height = 0, bool returnDefaultIfNotFound = false)
        {
            var picture = Repository.Get(pictureId);
            return GetPictureUrl(picture, width, height, returnDefaultIfNotFound);
        }

        public string GetPictureUrl(Media picture, int width = 0, int height = 0, bool returnDefaultIfNotFound = false)
        {
            //check if picture is not null
            if (picture == null || picture.Id == 0)
                return string.Empty;

            if (_mediaSettings.PictureSaveLocation == MediaSaveLocation.Database)
            {
                //todo: implement database storage
            }
            var expectedFile = FileHelpers.GetPictureFileNameWithSize(picture.LocalPath, width, height);
            var expectedFileSystemPath = ServerHelper.GetLocalPathFromRelativePath(expectedFile);
            
            if (!File.Exists(expectedFileSystemPath))
            {
                //we need to create the file with required dimensions
                var fileSystemPathForOriginalImage = ServerHelper.GetLocalPathFromRelativePath(picture.LocalPath);
                
                //image format
                var imageFormat = PictureUtility.GetImageFormatFromContentType(picture.MimeType);
                //save resized image
                _imageProcessor.WriteResizedImage(fileSystemPathForOriginalImage, expectedFileSystemPath, width, height, imageFormat);
            }
            //return the image url
            var imageServeUrl = expectedFile.Replace("~", _generalSettings.ImageServerDomain);
            return imageServeUrl;
        }

        public string GetPictureUrl(int pictureId, string sizeName, bool returnDefaultIfNotFound = false)
        {
            var picture = Repository.Get(pictureId);
            return GetPictureUrl(picture, sizeName, returnDefaultIfNotFound);
        }

        public string GetPictureUrl(Media picture, string sizeName, bool returnDefaultIfNotFound = false)
        {
            //find the width and height from size name
            var size = mobSocialEngine.ActiveEngine.PictureSizes.FirstOrDefault(x => x.Name == sizeName);
            if (size == null)
                return GetPictureUrl(picture, returnDefaultIfNotFound: returnDefaultIfNotFound);

            return GetPictureUrl(picture, size.Width, size.Height, returnDefaultIfNotFound);

        }

        public string GetVideoUrl(int mediaId)
        {
            throw new NotImplementedException();
        }

        public string GetVideoUrl(Media media)
        {
            throw new NotImplementedException();
        }

        public void WritePictureBytes(Media picture, MediaSaveLocation pictureSaveLocation)
        {
            //we need to save the file on file system
            if (picture.Binary == null || !picture.Binary.Any())
            {
                throw new mobSocialException("Can't write empty bytes for picture");
            }

            if (pictureSaveLocation == MediaSaveLocation.FileSystem)
            {
                //get the directory path from the relative path
                var directoryPath = ServerHelper.GetLocalPathFromRelativePath(_mediaSettings.PictureSavePath);
                var fileExtension = PathUtility.GetFileExtensionFromContentType(picture.MimeType);

                if (string.IsNullOrEmpty(picture.SystemName))
                    picture.SystemName = $"{Guid.NewGuid().ToString("n")}";

                var proposedFileName = $"{picture.SystemName}{fileExtension}";
                var filePath = PathUtility.GetFileSavePath(directoryPath, proposedFileName);

                var imageFormat = PictureUtility.GetImageFormatFromContentType(picture.MimeType);
                _imageProcessor.WriteBytesToImage(picture.Binary, filePath, imageFormat);

                //clear bytes
                picture.Binary = null;
                picture.LocalPath = filePath;

                picture.ThumbnailPath = ServerHelper.GetRelativePathFromLocalPath(filePath);
            }           
        }

        public void WriteVideoBytes(Media video)
        {
            //we need to save the file on file system
            if (video.Binary == null || !video.Binary.Any())
            {
                throw new mobSocialException("Can't write empty bytes for picture");
            }

            //get the directory path from the relative path
            var directoryPath = ServerHelper.GetLocalPathFromRelativePath(_mediaSettings.VideoSavePath);
            var fileExtension = PathUtility.GetFileExtensionFromContentType(video.MimeType);

            if (string.IsNullOrEmpty(video.SystemName))
                video.SystemName = $"{Guid.NewGuid().ToString("n")}";

            var proposedFileName = $"{video.SystemName}{fileExtension}";
            var filePath = PathUtility.GetFileSavePath(directoryPath, proposedFileName);
            File.WriteAllBytes(filePath, video.Binary);

            //clear bytes
            video.Binary = null;
            video.LocalPath = ServerHelper.GetRelativePathFromLocalPath(filePath);

            //create thumbnail for video
            var thumbnailRelativeFilePath = Path.Combine(_mediaSettings.PictureSavePath, video.SystemName + ".thumb.jpg");
            var thumbnailLocalFilePath = ServerHelper.GetLocalPathFromRelativePath(thumbnailRelativeFilePath);
            //TODO: Generate thumbnails of different sizes to save bandwidth

            _videoProcessor.WriteVideoThumbnailPicture(filePath, thumbnailLocalFilePath);
            //store relative path in thumbnail path
            video.ThumbnailPath = thumbnailRelativeFilePath;
        }

        public void WriteOtherMediaBytes(Media media, MediaSaveLocation saveLocation)
        {
            //we need to save the file on file system
            if (media.Binary == null || !media.Binary.Any())
            {
                throw new mobSocialException("Can't write empty bytes for picture");
            }

            if (saveLocation == MediaSaveLocation.FileSystem)
            {
                //get the directory path from the relative path
                var directoryPath = ServerHelper.GetLocalPathFromRelativePath(_mediaSettings.OtherMediaSavePath);

                if (string.IsNullOrEmpty(media.SystemName))
                    media.SystemName = Guid.NewGuid().ToString("n");

                var filePath = PathUtility.GetFileSavePath(directoryPath, media.SystemName);
                File.WriteAllBytes(filePath, media.Binary);

                //clear bytes
                media.Binary = null;
                media.LocalPath = filePath;
            }
        }

        public IList<Media> GetEntityMedia<TEntityType>(int entityId, MediaType mediaType) where TEntityType : BaseEntity
        {
            //first get the picture ids for this entity
            var mediaIds = _entityMediaRepository.Get(x => x.EntityId == entityId).Select(x => x.MediaId);
            return Repository.Get(x => mediaIds.Contains(x.Id) && x.MediaType == mediaType).ToList();
        }
    }
}