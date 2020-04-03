// ***********************************************************************
// Assembly         : CastleHillGaming.GameShare.VideoRecorder
// Author           : acscheiner
// Created          : 04-13-2016
//
// Last Modified By : acscheiner
// Last Modified On : 04-13-2016
// ***********************************************************************
// <copyright file="MessageProducer.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.GameShare.VideoRecorder
{
    #region

    using System.Collections;
    using Apache.NMS;
    using Apache.NMS.ActiveMQ.Commands;
    using CommonUtils;
    using Spring.Messaging.Nms.Core;

    #endregion

    /// <summary>
    ///     Class MessageProducer.
    /// </summary>
    /// <seealso cref="Spring.Messaging.Nms.Core.NmsGatewaySupport" />
    /// <seealso cref="CastleHillGaming.GameShare.VideoRecorder.IMessageProducer" />
    public class MessageProducer : NmsGatewaySupport, IMessageProducer
    {
        /// <summary>
        ///     Gets the job information destination.
        /// </summary>
        /// <value>The job information destination.</value>
        public IDestination JobInfoDestination { get; private set; }

        /// <summary>
        ///     Gets the video upload destination.
        /// </summary>
        /// <value>The video upload destination.</value>
        public IDestination VideoUploadDestination { get; private set; }

        /// <summary>
        ///     Sends message to update the job status.
        /// </summary>
        /// <param name="ticketId">The ticket identifier.</param>
        /// <param name="jobStatus">The job status.</param>
        public void UpdateJobStatus(string ticketId, TicketStatus jobStatus)
        {
            NmsTemplate.SendWithDelegate(JobInfoDestination,
                delegate(ISession session)
                {
                    var msg = session.CreateMessage();
                    msg.Properties.SetString(MessageKeys.TicketMessageKey, ticketId);
                    msg.Properties.SetInt(MessageKeys.JobStatusMessageKey, (int) jobStatus);
                    return msg;
                });
        }

        /// <summary>
        ///     Sends message to update the job status.
        /// </summary>
        /// <param name="ticketId">The ticket identifier.</param>
        /// <param name="jobStatus">The job status.</param>
        /// <param name="videoBytes">game share video as a byte array</param>
        public void UpdateJobStatus(string ticketId, TicketStatus jobStatus, byte[] videoBytes)
        {
            NmsTemplate.SendWithDelegate(JobInfoDestination,
                delegate(ISession session)
                {
                    var msg = session.CreateBytesMessage();
                    msg.Properties.SetString(MessageKeys.TicketMessageKey, ticketId);
                    msg.Properties.SetInt(MessageKeys.JobStatusMessageKey, (int)jobStatus);
                    msg.WriteBytes(videoBytes);
                    return msg;
                });
        }

        /// <summary>
        ///     Sends message to notify the job failed.
        /// </summary>
        /// <param name="ticketId">The ticket identifier.</param>
        public void NotifyJobFailed(string ticketId)
        {
            UpdateJobStatus(ticketId, TicketStatus.Failed);
        }

        /// <summary>
        ///     Sends message to record the GameShare replay video.
        /// </summary>
        /// <param name="ticketId">The ticket identifier.</param>
        /// <param name="casino">The casino name.</param>
        /// <param name="gameTitle">The game title.</param>
        /// <param name="gamePlayedAt">The game played-at time (milliseconds since epoch)</param>
        /// <param name="videoBytes">game share video as a byte array</param>
        public void UploadVideo(string ticketId, string casino, string gameTitle, long gamePlayedAt, byte[] videoBytes)
        {
            NmsTemplate.SendWithDelegate(VideoUploadDestination,
                delegate(ISession session)
                {
                    var msg = session.CreateBytesMessage();
                    msg.Properties.SetString(MessageKeys.TicketMessageKey, ticketId);
                    msg.Properties.SetString(MessageKeys.CasinoNameMessageKey, casino);
                    msg.Properties.SetString(MessageKeys.GameTitleMessageKey, gameTitle);
                    msg.Properties.SetLong(MessageKeys.GamePlayTimeMessageKey, gamePlayedAt);
                    msg.WriteBytes(videoBytes);
                    return msg;
                });
        }
    }
}