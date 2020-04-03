// ***********************************************************************
// Assembly         : CastleHillGaming.GameShare.VideoRecorder
// Author           : acscheiner
// Created          : 04-13-2016
//
// Last Modified By : acscheiner
// Last Modified On : 04-25-2016
// ***********************************************************************
// <copyright file="VideoRecordingService.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.GameShare.VideoRecorder
{
    #region

    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoIt;
    using CastleHillGaming.GameShare.CommonUtils;
    using Common.Logging;
    using Spring.Transaction.Interceptor;

    #endregion

    /// <summary>
    /// Class VideoRecordingService.
    /// </summary>
    /// <seealso cref="CastleHillGaming.GameShare.VideoRecorder.IVideoRecordingService" />
    /// <seealso cref="System.IDisposable" />
    public class VideoRecordingService : IVideoRecordingService, IDisposable
    {
        #region Private Static Member Data

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger<VideoRecordingService>();

        #endregion

        #region Private Instance Member Data

        /// <summary>
        /// The socket server
        /// </summary>
        private readonly SocketServer _socketServer;

        /// <summary>
        /// IP address for configuring socket server
        /// </summary>
        private const string ServerIpAddress = "127.0.0.1";

        /// <summary>
        /// Port for configuring socket server
        /// </summary>
        private const int ServerPort = 24191;

        /// <summary>
        /// The Air Game process
        /// </summary>
        private Process _airGameProcess;

        /// <summary>
        /// The blocking collection used for inter-thread communication when
        /// finalizing the recorded video file
        /// </summary>
        private BlockingCollection<byte[]> _videoRecordingByteReader;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the game directory path.
        /// </summary>
        /// <value>The game directory path.</value>
        public string GameDirectoryPath { get; private set; }

        /// <summary>
        /// Gets the video recording directory path.
        /// </summary>
        /// <value>The video directory path.</value>
        public string VideoRecordDirectoryPath { get; private set; }

        /// <summary>
        /// Gets the video uploading directory path.
        /// </summary>
        /// <value>The video directory path.</value>
        public string VideoUploadDirectoryPath { get; private set; }

        /// <summary>
        /// Gets the Message producer.
        /// </summary>
        /// <value>The Message producer.</value>
        public IMessageProducer MsgProducer { get; private set; }

        /// <summary>
        /// Gets the video recorder.
        /// </summary>
        /// <value>The recorder.</value>
        public IRecorder Recorder { get; private set; }

        /// <summary>
        /// Gets the Air Game window title.
        /// </summary>
        /// <value>The Air Game window title.</value>
        public string AirGameWindowTitle { get; private set; }

        /// <summary>
        /// Gets the game recall data file.
        /// </summary>
        /// <value>The game recall data file.</value>
        public string GameRecallDataFile { get; private set; }

        /// <summary>
        /// Gets the Air Game Process wait timeout (in milliseconds) to define a maximum wait time
        /// for Air Game replay run to complete.
        /// </summary>
        /// <value>The Air Game Process wait timeout (in milliseconds).</value>
        public int AirProcWaitTimeoutMillis { get; private set; }

        /// <summary>
        /// Gets the maximum file operation retries.
        /// </summary>
        /// <value>The maximum file operation retries.</value>
        public int FinalizeWaitTimeoutMillis { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoRecordingService" /> class.
        /// </summary>
        public VideoRecordingService(int airProcWaitTimeoutMillis)
        {
            AirProcWaitTimeoutMillis = airProcWaitTimeoutMillis;
            _socketServer = new SocketServer(ServerIpAddress, ServerPort, this);
            _socketServer.Run();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _socketServer.Stop();
        }

        /// <summary>
        /// Starts the recording (delegates to the IRecorder).
        /// </summary>
        public void StartRecording()
        {
            Logger.Debug("StartRecording");
            Recorder.StartRecording();
        }

        /// <summary>
        /// Stops the recording (delegates to the IRecorder).
        /// </summary>
        public async void StopRecording()
        {
            Logger.Debug("StopRecording");
            Recorder.StopRecording();
            await Task.Delay(800);
            Recorder.PowerOff();
        }

        /// <summary>
        /// Records the video.
        /// </summary>
        /// <param name="ticketId">The ticket identifier.</param>
        /// <param name="casino"></param>
        /// <param name="gameTitle">The game title.</param>
        /// <param name="gamePlayedAt">The game played-at time (milliseconds since epoch)</param>
        /// <param name="gameRecallData">The game recall data.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.IO.FileNotFoundException">Recorded Game Share Video MP4 file not found</exception>
        [Transaction]
        public void RecordVideo(string ticketId, string casino, string gameTitle, long gamePlayedAt,
            string gameRecallData)
        {
            try
            {
                MsgProducer.UpdateJobStatus(ticketId, TicketStatus.Recording);

                Logger.DebugFormat(
                    "Entered VideoRecordingService.RecordVideo - ticketId={0} casino={3} gameTitle={1} gameRecallData={2}",
                    ticketId, gameTitle, gameRecallData, casino);

                // clean up any mp4 files left in video recording directory
                foreach (
                    var mp4File in
                    Directory.GetFiles(VideoRecordDirectoryPath).Where(mp4File => mp4File.EndsWith(".mp4")))
                {
                    File.Delete(mp4File);
                }

                // write game share recallData to file for reading by Air Game exe at run-time
                var normalizedGameTitle = Regex.Replace(gameTitle, @"\s+", string.Empty);
                var gameShareRecallData =
                    new FileInfo(GameDirectoryPath + @"\" + normalizedGameTitle + @"\" + GameRecallDataFile);
                if (gameShareRecallData.Exists)
                {
                    gameShareRecallData.Delete();
                }

                using (var gsrdTextWriter = gameShareRecallData.CreateText())
                {
                    gsrdTextWriter.Write(gameRecallData);
                }

                // start up the video recording app
                Recorder.PowerOn();
                Thread.Sleep(800);

                // start up the Air Game Exe as a separate process
                RunAirGameProc(normalizedGameTitle);

                // get the mp4 recording as a byte[] and send message to VideoUploader
                // service with this byte[] as payload
                var videoBytes = FinalizeVideo();
                MsgProducer.UpdateJobStatus(ticketId, TicketStatus.Recorded, videoBytes);
                MsgProducer.UploadVideo(ticketId, casino, gameTitle, gamePlayedAt, videoBytes);
            }
            catch (RecordVideoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.WarnFormat("Exception caught: {0}" + Environment.NewLine + "{1}", ex.Message, ex.StackTrace);
                throw new RecordVideoException("Failed to Record Video", ex);
            }
            finally
            {
                Logger.Trace("In finally");
                try
                {
                    // clean up any mp4 files left in video recording directory
                    foreach (
                        var mp4File in
                        Directory.GetFiles(VideoRecordDirectoryPath).Where(mp4File => mp4File.EndsWith(".mp4")))
                    {
                        File.Delete(mp4File);
                    }

                    // clean up Game Share recallData file from this recording session
                    var gameShareRecallData =
                        new FileInfo(GameDirectoryPath + @"\" + gameTitle + @"\" + GameRecallDataFile);
                    if (gameShareRecallData.Exists)
                    {
                        gameShareRecallData.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Logger.WarnFormat("Exception caught: {0}" + Environment.NewLine + "{1}", ex.Message, ex.StackTrace);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Runs the air game proc.
        /// </summary>
        /// <param name="gameTitle">The game title.</param>
        /// <exception cref="System.TimeoutException"></exception>
        private void RunAirGameProc(string gameTitle)
        {
            // (should not happen under normal circumstances
            // but, in case something abnormal occurs) Kill
            // Air Game App window if it is active before we
            // launch new Air Game exe instance
            AutoItX.AutoItSetOption("WinTitleMatchMode", 3);
            if (0 != AutoItX.WinExists(AirGameWindowTitle))
            {
                AutoItX.WinKill(AirGameWindowTitle);
            }

            // Start Air Game exe as separate process
            _airGameProcess = new Process
            {
                StartInfo = {FileName = GameDirectoryPath + @"\" + gameTitle + @"\" + gameTitle + @".exe"}
            };
            _airGameProcess.Start();

            // if Air Game has not exited after max wait time, we assume
            // something went wrong
            if (!_airGameProcess.WaitForExit(AirProcWaitTimeoutMillis))
            {
                Recorder.PowerOff();
                _airGameProcess.Kill();

                var errMsg =
                    $"Exceeded wait time ({AirProcWaitTimeoutMillis/1000/60} minutes) for Air Game execution to complete";
                throw new TimeoutException(errMsg);
            }
        }

        /// <summary>
        /// Finalizes the video recording file.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="RecordVideoException">Failed to Record Video</exception>
        private byte[] FinalizeVideo()
        {
            var getVideoBytesThread = new Thread(GetVideoBytes);

            var didGetVideoBytes = false;
            byte[] videoBytes = null;
            using (_videoRecordingByteReader = new BlockingCollection<byte[]>(1))
            {
                getVideoBytesThread.Start();
                didGetVideoBytes = _videoRecordingByteReader.TryTake(out videoBytes,
                    TimeSpan.FromMilliseconds(FinalizeWaitTimeoutMillis));
            }

            if (getVideoBytesThread.IsAlive)
            {
                getVideoBytesThread.Abort();
            }

            if (!didGetVideoBytes || null == videoBytes)
            {
                throw new RecordVideoException("Failed to Record Video");
            }

            return videoBytes;
        }

        /// <summary>
        /// Gets the video bytes from the video recording. This is designed to be run
        /// as a separate thread (as called from FinalizeVideo).
        /// </summary>
        private void GetVideoBytes()
        {
            var mp4Files =
                Directory.GetFiles(VideoRecordDirectoryPath).Where(mp4File => mp4File.EndsWith(".mp4")).ToList();
            if (1 != mp4Files.Count)
            {
                // something wrong - should be a single .mp4 file file in the video recording directory
                // after OBS has exited successfully
                Logger.WarnFormat(
                    "Video Recording Directory in bad state - {0} .mp4 files present; expecting 1 (and only 1) mp4 file in directory {1}",
                    mp4Files.Count, VideoRecordDirectoryPath);
                _videoRecordingByteReader.Add(null);
                return;
            }

            var recording = mp4Files[0];
            var videoBytes = ReadVideoFileBytes(recording);
            while (null == videoBytes)
            {
                Thread.Sleep(80);
                videoBytes = ReadVideoFileBytes(recording);
            }

            _videoRecordingByteReader.Add(videoBytes);
        }

        /// <summary>
        /// Reads the video file bytes from the video recording file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>System.Byte[].</returns>
        private static byte[] ReadVideoFileBytes(string filename)
        {
            byte[] videoBytes;
            try
            {
                videoBytes = File.ReadAllBytes(filename);
            }
            catch (Exception)
            {
                return null;
            }

            return videoBytes;
        }

        #endregion
    }
}