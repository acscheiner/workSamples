// ***********************************************************************
// Assembly         : CastleHillGaming.GameShare.VideoRecorder
// Author           : acscheiner
// Created          : 04-13-2016
//
// Last Modified By : acscheiner
// Last Modified On : 04-22-2016
// ***********************************************************************
// <copyright file="IMessageHandler.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.GameShare.VideoRecorder
{
    #region

    using Apache.NMS;

    #endregion

    /// <summary>
    /// Interface IMessageHandler
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void HandleMessage(ITextMessage message);
    }
}