// ***********************************************************************
// Assembly         : CastleHillGaming.GameShare.VideoRecorder
// Author           : acscheiner
// Created          : 04-13-2016
//
// Last Modified By : acscheiner
// Last Modified On : 04-22-2016
// ***********************************************************************
// <copyright file="IVideoRecordingService.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.GameShare.VideoRecorder
{
    #region

    

    #endregion

    /// <summary>
    /// Interface IVideoRecordingService
    /// </summary>
    public interface IVideoRecordingService
    {
        /// <summary>
        /// Records the video.
        /// </summary>
        /// <param name="ticketId">The ticket identifier.</param>
        /// <param name="casino">The casino name.</param>
        /// <param name="gameTitle">The game title.</param>
        /// <param name="gamePlayedAt">The game played-at time (milliseconds since epoch)</param>
        /// <param name="gameRecallData">The game recall data.</param>
        /// <returns>Task.</returns>
        void RecordVideo(string ticketId, string casino, string gameTitle, long gamePlayedAt, string gameRecallData);

        /// <summary>
        /// Starts the recording.
        /// </summary>
        void StartRecording();

        /// <summary>
        /// Stops the recording.
        /// </summary>
        void StopRecording();
    }
}