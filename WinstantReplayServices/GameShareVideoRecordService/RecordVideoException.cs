namespace CastleHillGaming.GameShare.VideoRecorder
{
    #region

    using System;

    #endregion

    class RecordVideoException : ApplicationException
    {
        public RecordVideoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RecordVideoException(string message) : base(message)
        {
        }
    }
}