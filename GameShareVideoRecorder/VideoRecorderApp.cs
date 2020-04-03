// ***********************************************************************
// Assembly         : CastleHillGaming.GameShare.VideoRecorder
// Author           : acscheiner
// Created          : 04-13-2016
//
// Last Modified By : acscheiner
// Last Modified On : 04-25-2016
// ***********************************************************************
// <copyright file="VideoRecorderApp.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.GameShare.VideoRecorder
{
    #region

    using System;
    using Apache.NMS;
    using Common.Logging;
    using Spring.Context.Support;
    using Spring.Messaging.Nms.Core;
    using Spring.Messaging.Nms.Listener;
    using Spring.Messaging.Nms.Listener.Adapter;
    using Spring.Util;

    #endregion

    /// <summary>
    ///     Class VideoRecorderApp.
    /// </summary>
    internal class VideoRecorderApp
    {
        /// <summary>
        ///     Starts this instance.
        /// </summary>
        public void Start()
        {
            var ctx = ContextRegistry.GetContext();
            var msgListenerContainer = ctx.GetObject<SimpleMessageListenerContainer>("MessageListenerContainer");
            msgListenerContainer.SessionAcknowledgeMode = AcknowledgementMode.Transactional;
            msgListenerContainer.ErrorHandler = new MyErrorHandler();
            msgListenerContainer.ExceptionListener = new MyExceptionListener();
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public void Stop()
        {
        }
    }

    /// <summary>
    ///     Class MyErrorHandler.
    /// </summary>
    /// <seealso cref="Spring.Util.IErrorHandler" />
    internal class MyErrorHandler : IErrorHandler
    {
        /// <summary>
        ///     The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger<MyErrorHandler>();

        /// <summary>
        ///     Handles the error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void HandleError(Exception exception)
        {
            var logMsg = "VideoRecorderApp.MyErrorHandler.HandleError";
            var isRecordVideoException = exception is RecordVideoException;

            if (!string.IsNullOrWhiteSpace(exception.Message))
            {
                logMsg += Environment.NewLine + string.Format("{0}", exception.Message);
            }

            if (!string.IsNullOrWhiteSpace(exception.Source))
            {
                logMsg += Environment.NewLine + string.Format("{0}", exception.Source);
            }

            var innerException = exception.InnerException;
            while (null != innerException)
            {
                if (!isRecordVideoException)
                {
                    isRecordVideoException = innerException is RecordVideoException;
                }
                
                if (!string.IsNullOrWhiteSpace(innerException.Message))
                {
                    logMsg += Environment.NewLine + string.Format("{0}", innerException.Message);
                }
                innerException = innerException.InnerException;
            }

            Logger.WarnFormat("{0}", logMsg);

            if (exception is ListenerExecutionFailedException && isRecordVideoException) throw exception;
        }
    }

    /// <summary>
    ///     Class MyExceptionListener.
    /// </summary>
    /// <seealso cref="Spring.Messaging.Nms.Core.IExceptionListener" />
    internal class MyExceptionListener : IExceptionListener
    {
        /// <summary>
        ///     The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger<MyExceptionListener>();

        /// <summary>
        ///     Called when there is an exception in message processing.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void OnException(Exception exception)
        {
            var logMsg = "VideoRecorderApp.MyExceptionListener.OnException";
            var isRecordVideoException = exception is RecordVideoException;

            if (!string.IsNullOrWhiteSpace(exception.Message))
            {
                logMsg += Environment.NewLine + string.Format("{0}", exception.Message);
            }

            if (!string.IsNullOrWhiteSpace(exception.Source))
            {
                logMsg += Environment.NewLine + string.Format("{0}", exception.Source);
            }

            var innerException = exception.InnerException;
            while (null != innerException)
            {
                if (!isRecordVideoException)
                {
                    isRecordVideoException = innerException is RecordVideoException;
                }

                if (!string.IsNullOrWhiteSpace(innerException.Message))
                {
                    logMsg += Environment.NewLine + string.Format("{0}", innerException.Message);
                }
                innerException = innerException.InnerException;
            }

            Logger.WarnFormat("{0}", logMsg);

            if (exception is ListenerExecutionFailedException && isRecordVideoException) throw exception;
        }
    }
}