namespace DominatorHouseCore.Utility
{
    public class MediaUtilites
    {
        /// <summary>
        /// To get the thumbnail for the given video
        /// </summary>
        /// <param name="filePath">pass the media path(local)</param>              
        public string GetThumbnail(string filePath)
        {
            var extension = System.IO.Path.GetExtension(filePath)?.Replace(".", "");
            if (ConstantVariable.SupportedVideoFormat.Contains(extension))
            {
                var newFilePath = $"{filePath}{ConstantVariable.VideoToImageConvertFileName}";
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                ffMpeg.GetVideoThumbnail(filePath, newFilePath, 2);
                return newFilePath;
            }
            return filePath;
        }
    }
}