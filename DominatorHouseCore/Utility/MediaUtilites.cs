namespace DominatorHouseCore.Utility
{
    public class MediaUtilites
    {
        /// <summary>
        /// To get the thumbnail for the given video
        /// </summary>
        /// <param name="videoPath">pass the video path(local)</param>
        /// <param name="filePath">pass where you have to save the gathered thumbnail from video</param>       
        public void GetThumbnail(string videoPath, string filePath)
        {
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            ffMpeg.GetVideoThumbnail(videoPath, filePath, 2);
        }
    }
}