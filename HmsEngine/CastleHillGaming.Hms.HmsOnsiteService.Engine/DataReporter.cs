// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.HmsOnsiteService.Engine
// Author           : acscheiner
// Created          : 10-26-2016
//
// Last Modified By : acscheiner
// Last Modified On : 10-26-2016
// ***********************************************************************
// <copyright file="DataReporter.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.HmsOnsiteService.Engine
{
    #region

    using System;
    using System.Reflection;
    using System.Threading;
    using CastleHill.SharedUtils;
    using CastleHillGaming.Hms.HmsOnsiteService.Engine.Properties;
    using CastleHillGaming.Hms.Interfaces;
    using log4net;

    #endregion

    /// <summary>
    /// Class DataReporter.
    /// </summary>
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IStartable" />
    /// <seealso cref="System.IDisposable" />
    public class DataReporter : IStartable, IDisposable
    {
        #region Private Static data

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The re-entrant guard maximum minutes
        /// </summary>
        public static int ReentrantGuardMaxMinutes { get; } = 8;

        #endregion

        #region Private Instance Data

        /// <summary>
        /// Boolean flag to track if the DataReporter has been started
        /// </summary>
        private bool _isStarted;

        /// <summary>
        /// The _locker for thread safety
        /// </summary>
        private readonly object _locker = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data aggregator.
        /// </summary>
        /// <value>The data aggregator.</value>
        public IDataAggregator DataAggregator { get; private set; }

        /// <summary>
        /// Gets the data exporter.
        /// </summary>
        /// <value>The data exporter.</value>
        internal IDataExportStrategy DataExporter { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is started.
        /// </summary>
        /// <value><c>true</c> if this instance is started; otherwise, <c>false</c>.</value>
        public bool IsStarted
        {
            get
            {
                lock (_locker)
                {
                    return _isStarted;
                }
            }
            private set
            {
                lock (_locker)
                {
                    _isStarted = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the timer which triggers when a casino data report needs to be sent to HMS Cloud Service.
        /// </summary>
        /// <value>The timer which triggers when a casino data report needs to be sent to HMS Cloud Service.</value>
        public CHGTimer CasinoDataReportTimer { get; private set; }

        /// <summary>
        /// Gets the timer which triggers when a copy of the data backup needs to be sent to HMS Cloud Service.
        /// </summary>
        /// <value>The timer which triggers when a copy of the data backup needs to be sent to HMS Cloud Service.</value>
        public CHGTimer DataBackupTimer { get; private set; }

        /// <summary>
        /// Gets the timer which triggers when a copy of the Diagnostics files need to be sent to HMS Cloud Service.
        /// </summary>
        /// <value>The timer which triggers when a copy of the Diagnostics files need to be sent to HMS Cloud Service.</value>
        public CHGTimer DiagnosticsTimer { get; private set; }

        #endregion

        #region Interface Implementations

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Start()
        {
            if (IsDisposed)
            {
                Logger.Warn("Cannot Start DataReporter - it has been Disposed.");
                return;
            }

            if (IsStarted)
            {
                Logger.Info("Cannot Start DataReporter - it has already been started.");
                return;
            }

            Logger.Debug($"Starting DataReporter - DataExporter is a {DataExporter.GetType()}");

            lock (_locker)
            {
                IsStarted = true;

                // Set up timers for triggering push of data to HMS Cloud service.
                CasinoDataReportTimer = new CHGTimer();
                CasinoDataReportTimer.TimerEvent += GetEgmDataForReport;
                CasinoDataReportTimer.Start(TimeSpan.FromMinutes(Settings.Default.CloudReportStartupDelay),
                    TimeSpan.FromMinutes(Settings.Default.CloudReportInterval));

                // Note that if DoReportToCloud configuration parameter is set to False,
                // we set the DataBackup timers such that they never trigger
                DataBackupTimer = new CHGTimer();
                DataBackupTimer.TimerEvent += GetWeeklyDataBackupFiles;
                DataBackupTimer.Start(Settings.Default.DoReportToCloud
                        ? TimeSpan.FromMinutes(Settings.Default.DataBackupStartupDelay)
                        : CHGTimer.DisableTimer,
                    Settings.Default.DoReportToCloud
                        ? TimeSpan.FromMinutes(Settings.Default.DataBackupInterval)
                        : CHGTimer.DisableTimer);

                // Note that if DoReportToCloud configuration parameter is set to False,
                // we set the DiagnosticFile timers such that they never trigger
                DiagnosticsTimer = new CHGTimer();
                DiagnosticsTimer.TimerEvent += GetDiagnosticsFiles;
                DiagnosticsTimer.Start(Settings.Default.DoReportToCloud
                        ? TimeSpan.FromMinutes(Settings.Default.DiagnosticsStartupDelay)
                        : CHGTimer.DisableTimer,
                    Settings.Default.DoReportToCloud
                        ? TimeSpan.FromMinutes(Settings.Default.DiagnosticsInterval)
                        : CHGTimer.DisableTimer);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDispose">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool isDispose)
        {
            if (IsDisposed)
            {
                return;
            }

            Logger.Debug("Disposing DataReporter");

            if (isDispose)
            {
                lock (_locker)
                {
                    CasinoDataReportTimer.Dispose();
                    DataBackupTimer.Dispose();
                    DiagnosticsTimer.Dispose();
                }
            }

            IsDisposed = true;
        }

        #endregion

        #region Private instance methods

        /// <summary>
        /// Gets the egm data for report.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <param name="o">The o.</param>
        private void GetEgmDataForReport(Timer timer, object o)
        {
            var casinoDataReport = DataAggregator.GetCasinoDataReport();
            if (null == casinoDataReport) return;

            // Guard against Timer event re-entrancy by breaking out of
            // data processing do...while loop (below) when we are approaching
            // next Timer event (even if there is more data to process).
            var startAt = DateTime.UtcNow;

            var reentrantGuardMinutes = Settings.Default.CloudReportInterval * 0.10d;
            if (ReentrantGuardMaxMinutes < reentrantGuardMinutes)
            {
                reentrantGuardMinutes = ReentrantGuardMaxMinutes;
            }

            var reentrantGuardTimeSpan =
                TimeSpan.FromMinutes(Settings.Default.CloudReportInterval)
                    .Subtract(TimeSpan.FromMinutes(reentrantGuardMinutes));

            do
            {
                Logger.Debug($"Casino Data Report {casinoDataReport}");
                try
                {
                    DataExporter.SendCasinoDataReport(casinoDataReport);
                }
                catch (Exception ex)
                {
                    Logger.Warn(
                        $"An unexpected error occurred while calling DataReporter.SendCasinoDataReport. Check WCF HMS configuration: [{ex.Message}]");
                    var innerEx = ex.InnerException;
                    while (null != innerEx)
                    {
                        Logger.Warn($"[{innerEx.Message}]");
                        innerEx = innerEx.InnerException;
                    }

                    DataAggregator.UnsuccessfulCasinoDataReport(casinoDataReport.ReportGuid);
                    Logger.Warn($"Stack Trace: [{Environment.StackTrace}]");
                    break;
                }

                casinoDataReport = DataAggregator.GetCasinoDataReport();
            } while (null != casinoDataReport && DateTime.UtcNow.Subtract(startAt) < reentrantGuardTimeSpan);
        }

        /// <summary>
        /// Gets the weekly data backup files.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <param name="o">The o.</param>
        private void GetWeeklyDataBackupFiles(Timer timer, object o)
        {
            // Get last week's pg_dump files and send them (via WCF/MSMQ message payload)
            // up to HMS cloud server
            var backupData = DataAggregator.GetDataBackup();
            if (null == backupData || 0 >= backupData.Count) return;

            try
            {
                DataExporter.SendDataBackup(backupData);
            }
            catch (Exception ex)
            {
                Logger.Warn(
                    $"An unexpected error occurred while calling DataReporter.SendDataBackup. Check WCF HMS configuration: [{ex.Message}]");
                var innerEx = ex.InnerException;
                while (null != innerEx)
                {
                    Logger.Warn($"[{innerEx.Message}]");
                    innerEx = innerEx.InnerException;
                }

                Logger.Warn($"Stack Trace: [{Environment.StackTrace}]");
            }
        }

        /// <summary>
        /// Gets the diagnostics files for reporting to the HMS Cloud service.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <param name="obj">The object.</param>
        private void GetDiagnosticsFiles(Timer timer, object obj)
        {
            var diagnosticData = DataAggregator.GetCasinoDiagnosticData();
            if (diagnosticData == null || 0 >= diagnosticData.Count) return;

            try
            {
                DataExporter.SendCasinoDiagnosticData(diagnosticData);
            }
            catch (Exception ex)
            {
                Logger.Warn(
                    $"An unexpected error occurred while calling DataReporter.SendCasinoDiagnosticData. Check WCF HMS configuration: [{ex.Message}]");
                var innerEx = ex.InnerException;
                while (null != innerEx)
                {
                    Logger.Warn($"[{innerEx.Message}]");
                    innerEx = innerEx.InnerException;
                }

                Logger.Warn($"Stack Trace: [{Environment.StackTrace}]");
            }
        }

        #endregion
    }
}