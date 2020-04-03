// ***********************************************************************
// Assembly         : CastleHillGaming.GameShare.VideoRecorder
// Author           : acscheiner
// Created          : 04-13-2016
//
// Last Modified By : acscheiner
// Last Modified On : 04-25-2016
// ***********************************************************************
// <copyright file="Program.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.GameShare.VideoRecorder
{
    #region

    using Topshelf;

    #endregion

    /// <summary>
    ///     Class Program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.Service<VideoRecorderApp>(serviceConfigurator =>
                {
                    serviceConfigurator.ConstructUsing(name => new VideoRecorderApp());
                    serviceConfigurator.WhenStarted(videoRecorderApp => videoRecorderApp.Start());
                    serviceConfigurator.WhenStopped(videoRecorderApp => videoRecorderApp.Stop());
                });
                hostConfigurator.RunAsLocalSystem();

                hostConfigurator.SetDescription("Game Share Video Recording Service");
                hostConfigurator.SetDisplayName("GameShareVideoRecorder");
                hostConfigurator.SetServiceName("GameShareVideoRecorder");
                hostConfigurator.StartAutomaticallyDelayed();
            });
        }
    }
}