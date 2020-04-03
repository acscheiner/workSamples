// ***********************************************************************
// Assembly         : CastleHillGaming.GameShare.VideoRecorder
// Author           : acscheiner
// Created          : 04-15-2016
//
// Last Modified By : acscheiner
// Last Modified On : 04-25-2016
// ***********************************************************************
// <copyright file="SocketServer.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.GameShare.VideoRecorder
{
    #region

    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Common.Logging;

    #endregion

    /// <summary>
    /// Class SocketServer.
    /// </summary>
    public class SocketServer
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger<SocketServer>();

        /// <summary>
        /// The IP address for this socket server
        /// </summary>
        private readonly IPAddress _ipAddress;

        /// <summary>
        /// The port for this socket server
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// The buffer for receiving socket data
        /// </summary>
        private readonly byte[] _buffer;

        /// <summary>
        /// The buffer size for receiving socket data
        /// </summary>
        private const int BufferSize = 8;

        /// <summary>
        /// Flag indicating whether this socket server is running
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// The video recording service
        /// </summary>
        private readonly IVideoRecordingService _videoRecordingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServer" /> class.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="port">The port.</param>
        /// <param name="videoRecordingService">The video recording service.</param>
        /// <exception cref="System.Exception">No IPv4 address for server</exception>
        public SocketServer(string ipAddress, int port, IVideoRecordingService videoRecordingService)
        {
            _videoRecordingService = videoRecordingService;
            _port = port;
            _ipAddress = null;

            _buffer = new byte[BufferSize];

            _ipAddress = IPAddress.Parse(ipAddress);

            if (null == _ipAddress)
            {
                throw new Exception("No IPv4 address for server");
            }
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task Run()
        {
            var listener = new TcpListener(_ipAddress, _port);
            _isRunning = true;

            listener.Start();
            Logger.DebugFormat("Async TCP Listener is now running on ipAddress {0} at port {1}", _ipAddress.ToString(),
                _port);

            while (_isRunning)
            {
                try
                {
                    Logger.Trace("Waiting for a request ...");
                    using (var tcpClient = await listener.AcceptTcpClientAsync())
                    {
                        await Process(tcpClient);
                    }
                }
                catch (Exception ex)
                {
                    Logger.DebugFormat("{0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Processes the specified TCP client.
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        /// <returns>Task.</returns>
        private async Task Process(TcpClient tcpClient)
        {
            Logger.DebugFormat("Received connection request from {0}", tcpClient.Client.RemoteEndPoint.ToString());

            try
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    while (_isRunning)
                    {
                        var numBytesRead = await networkStream.ReadAsync(_buffer, 0, 8);
                        if (0 < numBytesRead)
                        {
                            var received = Encoding.UTF8.GetString(_buffer).Trim();
                            Logger.DebugFormat("Received service request: {0}; numbytes={1}", received, numBytesRead);
                            switch (received.Substring(0, 2))
                            {
                                case "ar":
                                    Logger.Debug("Received Start Recording message");
                                    _videoRecordingService.StartRecording();
                                    break;
                                case "or":
                                    Logger.Debug("Received Stop Recording message");
                                    _videoRecordingService.StopRecording();
                                    break;
                                default:
                                    Logger.WarnFormat("SocketServer.Process - received unknown message: {0}", received);
                                    break;
                            }
                        }
                        else
                        {
                            // client closed connection
                            Logger.DebugFormat("Closing TCP connection to {0}",
                                tcpClient.Client.RemoteEndPoint.ToString());
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WarnFormat("{0}", ex.Message);
            }
        }
    }
}