// ***********************************************************************
// Assembly         : CastleHillGaming.GameShare.VideoRecorder
// Author           : acscheiner
// Created          : 04-13-2016
//
// Last Modified By : acscheiner
// Last Modified On : 04-13-2016
// ***********************************************************************
// <copyright file="IMessageProducer.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.GameShare.VideoRecorder
{
    #region

    using CommonUtils;

    #endregion

    /// <summary>
    ///     Interface IMessageProducer
    /// </summary>
    public interface IMessageProducer
    {
        /// <summary>
        ///     Sends message to update the job status.
        /// </summary>
        /// <param name="ticketId">The ticket identifier.</param>
        /// <param name="jobStatus">The job status.</param>
        void UpdateJobStatus(string ticketId, TicketStatus jobStatus);

        /// <summary>
        ///     Sends message to update the job status.
        /// </summary>
        /// <param name="ticketId">The ticket identifier.</param>
        /// <param name="jobStatus">The job status.</param>
        /// <param name="videoBytes">game share video as a byte array</param>
        void UpdateJobStatus(string ticketId, TicketStatus jobStatus, byte[] videoBytes);

        /// <summary>
        ///     Sends message to notify the job failed.
        /// </summary>
        /// <param name="ticketId">The ticket identifier.</param>
        void NotifyJobFailed(string ticketId);

        /// <summary>
        ///     Sends message to record the GameShare replay video.
        /// </summary>
        /// <param name="ticketId">The ticket identifier.</param>
        /// <param name="casino">The casino name.</param>
        /// <param name="gameTitle">The game title.</param>
        /// <param name="gamePlayedAt">The game played-at time (milliseconds since epoch)</param>
        /// <param name="videoBytes">game share video as a byte array</param>
        void UploadVideo(string ticketId, string casino, string gameTitle, long gamePlayedAt, byte[] videoBytes);
    }
}