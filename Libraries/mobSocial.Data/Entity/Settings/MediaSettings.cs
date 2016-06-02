using mobSocial.Core.Config;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Settings
{

    public class MediaSettings : ISettingGroup
    {
        /// <summary>
        /// Maximum file upload size in bytes for images
        /// </summary>
        public long MaximumFileUploadSizeForImages { get; set; }

        /// <summary>
        /// Maximum file upload size in bytes for videos
        /// </summary>
        public long MaximumFileUploadSizeForVideos { get; set; }

        /// <summary>
        /// Maximum file upload size in bytes for documents
        /// </summary>
        public long MaximumFileUploadSizeForDocuments { get; set; }

        /// <summary>
        /// The path where pictures will be saved on file system
        /// </summary>
        public string PictureSavePath { get; set; }

        /// <summary>
        /// Whether the picture should be saved in database or file system
        /// </summary>
        public MediaSaveLocation PictureSaveLocation { get; set; }

        /// <summary>
        /// The path where videos will be saved on file system
        /// </summary>
        public string VideoSavePath { get; set; }

        /// <summary>
        /// The path where other media will be saved on file system
        /// </summary>
        public string OtherMediaSavePath { get; set; }

        /// <summary>
        /// Whether mediashould be saved in database or file system
        /// </summary>
        public MediaSaveLocation OtherMediaSaveLocation { get; set; }

        /// <summary>
        /// The thumbnail size of a picture in pixels. Value should be written as WidthxHeight e.g. 800x600
        /// </summary>
        public string ThumbnailPictureSize { get; set; }

        /// <summary>
        /// The small profile size of a picture in pixels. Value should be written as WidthxHeight e.g. 100x100
        /// </summary>
        public string SmallProfilePictureSize { get; set; }

        /// <summary>
        /// The medium profile size of a picture in pixels. Value should be written as WidthxHeight e.g. 800x600
        /// </summary>
        public string MediumProfilePictureSize { get; set; }

        /// <summary>
        /// The small cover size of a picture in pixels. Value should be written as WidthxHeight e.g. 800x600
        /// </summary>
        public string SmallCoverPictureSize { get; set; }

        /// <summary>
        /// The medium cover size of a picture in pixels. Value should be written as WidthxHeight e.g. 800x600
        /// </summary>
        public string MediumCoverPictureSize { get; set; }

    }


}