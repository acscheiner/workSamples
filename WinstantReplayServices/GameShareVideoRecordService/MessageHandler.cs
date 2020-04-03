// ***********************************************************************
// Assembly         : CastleHillGaming.GameShare.VideoRecorder
// Author           : acscheiner
// Created          : 04-13-2016
//
// Last Modified By : acscheiner
// Last Modified On : 04-25-2016
// ***********************************************************************
// <copyright file="MessageHandler.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.GameShare.VideoRecorder
{
    #region

    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using Apache.NMS;
    using CastleHillGaming.GameShare.CommonUtils;
    using Common.Logging;

    #endregion

    /// <summary>
    /// Class MessageHandler.
    /// </summary>
    /// <seealso cref="CastleHillGaming.GameShare.VideoRecorder.IMessageHandler" />
    public class MessageHandler : IMessageHandler
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger<MessageHandler>();

        /// <summary>
        /// Gets/Sets the Video Recording service.
        /// </summary>
        /// <value>The Video Recording service.</value>
        public IVideoRecordingService VideoRecordingService { get; private set; }

        /// <summary>
        /// Gets the message producer.
        /// </summary>
        /// <value>The message producer.</value>
        public IMessageProducer MsgProducer { get; private set; }

        /// <summary>
        /// Gets the game directory path.
        /// </summary>
        /// <value>The game directory path.</value>
        public string GameDirectoryPath { get; private set; }


        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">
        /// ticketUuid
        /// or
        /// gameTitle
        /// or
        /// recallData
        /// </exception>
        public void HandleMessage(ITextMessage message)
        {
            Logger.Debug("Entered HandleMessage");

            if (null == message) throw new ArgumentNullException("message");

            var ticketUuid = message.Properties.GetString(MessageKeys.TicketMessageKey);
            var gameTitle = message.Properties.GetString(MessageKeys.GameTitleMessageKey);
            var casino = message.Properties.GetString(MessageKeys.CasinoNameMessageKey);
            var gamePlayedAt = message.Properties.GetLong(MessageKeys.GamePlayTimeMessageKey);

            var recallData = message.Text;

            var ticketIdOk = true;
            var gameTitleOk = true;
            var recallDataOk = true;
            var casinoOk = true;
            var gamePlayedAtOk = true;

            var numBadParams = 0;
            if (string.IsNullOrWhiteSpace(ticketUuid))
            {
                ticketIdOk = false;
                ++numBadParams;
            }

            if (string.IsNullOrWhiteSpace(gameTitle))
            {
                gameTitleOk = false;
                ++numBadParams;
            }
            else
            {
                if (!Directory.Exists(GameDirectoryPath + @"\" + Regex.Replace(gameTitle, @"\s+", string.Empty)))
                {
                    gameTitleOk = false;
                    ++numBadParams;
                }
            }

            if (string.IsNullOrWhiteSpace(recallData))
            {
                recallDataOk = false;
                ++numBadParams;
            }

            if (string.IsNullOrWhiteSpace(casino))
            {
                casinoOk = false;
                ++numBadParams;
            }

            if (0 > gamePlayedAt)
            {
                gamePlayedAtOk = false;
                ++numBadParams;
            }

            if (0 < numBadParams)
            {
                MsgProducer.NotifyJobFailed(ticketUuid);

                var errMsg = "Invalid message payload: ";
                var badParams = string.Empty;

                if (!ticketIdOk)
                {
                    badParams += "ticketUuid";
                }

                if (!gameTitleOk)
                {
                    if (!string.IsNullOrWhiteSpace(badParams))
                    {
                        badParams += "/";
                    }
                    badParams += "gameTitle";
                }

                if (!recallDataOk)
                {
                    if (!string.IsNullOrWhiteSpace(badParams))
                    {
                        badParams += "/";
                    }
                    badParams += "recallData";
                }

                if (!casinoOk)
                {
                    if (!string.IsNullOrWhiteSpace(badParams))
                    {
                        badParams += "/";
                    }
                    badParams += "casino";
                }

                if (!gamePlayedAtOk)
                {
                    if (!string.IsNullOrWhiteSpace(badParams))
                    {
                        badParams += "/";
                    }
                    badParams += "gamePlayedAt";
                }

                var value = numBadParams > 1 ? "values" : "value";
                errMsg += $"{badParams} {value} must be non-null and non-empty";
                throw new ArgumentException(errMsg);
            }

            Logger.DebugFormat(
                "HandleMessage - ticketUuid={0} casino={3} gameTitle={1} gamePlayedTime={4} recallData={2}", ticketUuid,
                gameTitle, recallData, casino, gamePlayedAt);

            VideoRecordingService.RecordVideo(ticketUuid, casino, gameTitle, gamePlayedAt, recallData);
        }
    }
}