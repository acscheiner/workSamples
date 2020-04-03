// ***********************************************************************
// Assembly         : CastleHillGaming.GameShare.VideoRecorder
// Author           : acscheiner
// Created          : 04-18-2016
//
// Last Modified By : acscheiner
// Last Modified On : 04-18-2016
// ***********************************************************************
// <copyright file="IRecorder.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.GameShare.VideoRecorder
{
    /// <summary>
    ///     Interface IRecorder
    /// </summary>
    public interface IRecorder
    {
        /// <summary>
        ///     Powers On the Video Recorder.
        /// </summary>
        void PowerOn();

        /// <summary>
        ///     Powers Off the Video Recorder.
        /// </summary>
        void PowerOff();

        /// <summary>
        ///     Starts the recording.
        /// </summary>
        void StartRecording();

        /// <summary>
        ///     Stops the recording.
        /// </summary>
        void StopRecording();

        /// <summary>
        ///     Determines whether this instance is recording.
        /// </summary>
        /// <returns><c>true</c> if this instance is recording; otherwise, <c>false</c>.</returns>
        bool IsRecording();
    }
}