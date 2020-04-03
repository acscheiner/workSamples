// ***********************************************************************
// Assembly         : CastleHillGaming.GameShare.VideoRecorder
// Author           : acscheiner
// Created          : 04-18-2016
//
// Last Modified By : acscheiner
// Last Modified On : 04-22-2016
// ***********************************************************************
// <copyright file="ObsRecorder.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.GameShare.VideoRecorder
{
    #region

    using AutoIt;
    using Common.Logging;

    #endregion

    /// <summary>
    /// Class ObsRecorder.
    /// </summary>
    /// <seealso cref="CastleHillGaming.GameShare.VideoRecorder.IRecorder" />
    public class ObsRecorder : IRecorder
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger<ObsRecorder>();

        /// <summary>
        /// Flag indicating whether the Video Recorder is recording
        /// </summary>
        private volatile bool _isRecording;

        /// <summary>
        /// Gets the game share root directory.
        /// </summary>
        /// <value>The game share root directory.</value>
        public string GameShareRootDirectory { get; private set; }

        /// <summary>
        /// Gets the obs window title.
        /// </summary>
        /// <value>The obs window title.</value>
        public string ObsWindowTitle { get; private set; }

        /// <summary>
        /// Gets the obs start/stop button identifier.
        /// </summary>
        /// <value>The obs start/stop button identifier.</value>
        public string ObsStartStopButtonId { get; private set; }

        /// <summary>
        /// Gets the obs start stop button x position.
        /// </summary>
        /// <value>The obs start stop button x position.</value>
        public int ObsStartStopButtonXPos { get; private set; }

        /// <summary>
        /// Gets the obs start stop button y position.
        /// </summary>
        /// <value>The obs start stop button y position.</value>
        public int ObsStartStopButtonYPos { get; private set; }

        /// <summary>
        /// Gets the obs exit button identifier.
        /// </summary>
        /// <value>The obs exit button identifier.</value>
        public string ObsExitButtonId { get; private set; }

        /// <summary>
        /// Gets the obs exit button x position.
        /// </summary>
        /// <value>The obs exit button x position.</value>
        public int ObsExitButtonXPos { get; private set; }

        /// <summary>
        /// Gets the obs exit button y position.
        /// </summary>
        /// <value>The obs exit button y position.</value>
        public int ObsExitButtonYPos { get; private set; }


        /// <summary>
        /// The sync object (used for thread locking)
        /// </summary>
        private readonly object _syncObj = new object();

        /// <summary>
        /// Powers On the Video Recorder.
        /// </summary>
        public void PowerOn()
        {
            lock (_syncObj)
            {
                _isRecording = false;

                AutoItX.AutoItSetOption("WinTitleMatchMode", 2);
                if (0 == AutoItX.WinExists(ObsWindowTitle))
                {
                    AutoItX.Run(GameShareRootDirectory + @"\obsRun.bat", GameShareRootDirectory);
                }
                AutoItX.WinActivate(ObsWindowTitle);
            }
        }

        /// <summary>
        /// Powers Off the Video Recorder.
        /// </summary>
        public void PowerOff()
        {
            Logger.Debug("Exit");

            lock (_syncObj)
            {
                if (_isRecording)
                {
                    StopRecording();
                }

                AutoItX.AutoItSetOption("WinTitleMatchMode", 2);
                if (0 == AutoItX.WinExists(ObsWindowTitle)) return;

                AutoItX.WinActivate(ObsWindowTitle);
                AutoItX.ControlClick(ObsWindowTitle, "", ObsExitButtonId, "LEFT", 1, ObsExitButtonXPos,
                    ObsExitButtonYPos);
            }
        }

        /// <summary>
        /// Starts the recording.
        /// </summary>
        public void StartRecording()
        {
            Logger.Debug("Start Recording");

            lock (_syncObj)
            {
                if (_isRecording) return;

                _isRecording = true;

                AutoItX.AutoItSetOption("WinTitleMatchMode", 2);
                AutoItX.WinActivate(ObsWindowTitle);
                AutoItX.ControlClick(ObsWindowTitle, "", ObsStartStopButtonId, "LEFT", 1, ObsStartStopButtonXPos,
                    ObsStartStopButtonYPos);
            }
        }

        /// <summary>
        /// Stops the recording.
        /// </summary>
        public void StopRecording()
        {
            Logger.Debug("Stop Recording");

            lock (_syncObj)
            {
                if (!_isRecording) return;

                _isRecording = false;

                AutoItX.AutoItSetOption("WinTitleMatchMode", 2);
                AutoItX.WinActivate(ObsWindowTitle);
                AutoItX.ControlClick(ObsWindowTitle, "", ObsStartStopButtonId, "LEFT", 1, ObsStartStopButtonXPos,
                    ObsStartStopButtonYPos);
            }
        }

        /// <summary>
        /// Determines whether this instance is recording.
        /// </summary>
        /// <returns><c>true</c> if this instance is recording; otherwise, <c>false</c>.</returns>
        public bool IsRecording()
        {
            lock (_syncObj)
            {
                return _isRecording;
            }
        }
    }
}