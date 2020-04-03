// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.HmsCloudService.Engine
// Author           : acscheiner
// Created          : 11-17-2016
//
// Last Modified By : acscheiner
// Last Modified On : 11-17-2016
// ***********************************************************************
// <copyright file="DataCleaner.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using CastleHill.SharedUtils;
    using CastleHillGaming.Hms.DataModel.DataAccessLayer.Dao;
    using log4net;

    #endregion

    /// <summary>
    /// Class DataCleaner.
    /// </summary>
    public class DataCleaner
    {
        #region Private Static data

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Private Instance Data

        /// <summary>
        /// Boolean flag to track if the DataCleaner has been started
        /// </summary>
        private bool _isStarted;

        /// <summary>
        /// Boolean flag to track if the DataCleaner has been disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// The _locker for thread safety
        /// </summary>
        private readonly object _locker = new object();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the egm meter reading DAO.
        /// </summary>
        /// <value>The egm meter reading DAO.</value>
        public EgmMeterReadingDao EgmMeterReadingDao { get; private set; }

        /// <summary>
        /// Gets the egm event DAO.
        /// </summary>
        /// <value>The egm event DAO.</value>
        public EgmEventDao EgmEventDao { get; private set; }

        /// <summary>
        /// Gets the egm metric DAO.
        /// </summary>
        /// <value>The egm metric DAO.</value>
        public EgmMetricDao EgmMetricDao { get; private set; }

        /// <summary>
        /// Gets the egm windows event DAO.
        /// </summary>
        /// <value>The egm windows event DAO.</value>
        public EgmWindowsEventDao EgmWindowsEventDao { get; private set; }

        /// <summary>
        /// Gets the egm version DAO.
        /// </summary>
        /// <value>The egm version DAO.</value>
        public EgmVersionDao EgmVersionDao { get; private set; }

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
        public bool IsDisposed
        {
            get
            {
                lock (_locker)
                {
                    return _isDisposed;
                }
            }
            private set
            {
                lock (_locker)
                {
                    _isDisposed = value;
                }
            }
        }

        /// <summary>
        /// Gets the timer for cleaning old EGM report data.
        /// </summary>
        /// <value>The timer for cleaning old EGM report data.</value>
        public CHGTimer EgmDataCleanTimer { get; private set; }

        /// <summary>
        /// Gets the timer for cleaning old Data Backups.
        /// </summary>
        /// <value>The timer for cleaning old Data Backups.</value>
        public CHGTimer DataBackupCleanTimer { get; private set; }

        /// <summary>
        /// Gets the diagnostic data clean timer.
        /// </summary>
        /// <value>The diagnostic data clean timer.</value>
        public CHGTimer DiagnosticDataCleanTimer { get; private set; }

        /// <summary>
        /// Gets the local export data clean timer.
        /// </summary>
        /// <value>The local export data clean timer.</value>
        public CHGTimer LocalExportDataCleanTimer { get; private set; }

        /// <summary>
        /// Gets the EGM data clean interval hours.
        /// </summary>
        /// <value>The EGM data clean interval hours.</value>
        public int EgmDataCleanIntervalHours { get; private set; }

        /// <summary>
        /// Gets the Data Backup clean interval hours.
        /// </summary>
        /// <value>The Data Backup clean interval hours.</value>
        public int DataBackupCleanIntervalHours { get; private set; }

        /// <summary>
        /// Gets the diagnostic data clean interval hours.
        /// </summary>
        /// <value>The diagnostic data clean interval hours.</value>
        public int DiagnosticDataCleanIntervalHours { get; private set; }

        /// <summary>
        /// Gets the local export data clean interval hours.
        /// </summary>
        /// <value>The local export data clean interval hours.</value>
        public int LocalExportDataCleanIntervalHours { get; private set; }

        /// <summary>
        /// Gets the event data expiry hours.
        /// </summary>
        /// <value>The event data expiry hours.</value>
        public int EventDataExpiryHours { get; private set; }

        /// <summary>
        /// Gets the meter data expiry hours.
        /// </summary>
        /// <value>The meter data expiry hours.</value>
        public int MeterDataExpiryHours { get; private set; }

        /// <summary>
        /// Gets the metric data expiry hours.
        /// </summary>
        /// <value>The metric data expiry hours.</value>
        public int MetricDataExpiryHours { get; private set; }

        /// <summary>
        /// Gets the windows event data expiry hours.
        /// </summary>
        /// <value>The windows event data expiry hours.</value>
        public int WindowsEventDataExpiryHours { get; private set; }

        /// <summary>
        /// Gets the version data expiry hours.
        /// </summary>
        /// <value>The version data expiry hours.</value>
        public int VersionDataExpiryHours { get; private set; }

        /// <summary>
        /// Gets the backup data expiry weeks.
        /// </summary>
        /// <value>The backup data expiry weeks.</value>
        public int BackupDataExpiryWeeks { get; private set; }

        /// <summary>
        /// Gets the diagnostic data expiry days.
        /// </summary>
        /// <value>The diagnostic data expiry days.</value>
        public int DiagnosticDataExpiryDays { get; private set; }

        /// <summary>
        /// Gets the local export data expiry weeks.
        /// </summary>
        /// <value>The local export data expiry weeks.</value>
        public int LocalExportDataExpiryWeeks { get; private set; }

        /// <summary>
        /// Gets the data backup location.
        /// </summary>
        /// <value>The data backup location.</value>
        public string DataBackupLocation { get; private set; }

        /// <summary>
        /// Gets the diagnostics location.
        /// </summary>
        /// <value>The diagnostics location.</value>
        public string DiagnosticsLocation { get; private set; }

        /// <summary>
        /// Gets the local export data location.
        /// </summary>
        /// <value>The local export data location.</value>
        public string LocalExportDataLocation { get; private set; }

        /// <summary>
        /// The calendar
        /// </summary>
        private Calendar CleanupCalendar { get; } = CultureInfo.InvariantCulture.Calendar;

        #endregion

        #region Interface implementations

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (IsDisposed)
            {
                Logger.Warn("Cannot Start DataCleaner - it has been Disposed.");
                return;
            }

            if (IsStarted)
            {
                Logger.Info("Cannot Start DataCleaner - it has already been started.");
                return;
            }

            Logger.Debug("Starting DataCleaner");

            lock (_locker)
            {
                IsStarted = true;

                // this is here to initialize database (and apply any pending migrations) at startup
                EgmEventDao.GetById(long.MinValue);

                // set up the daily timers to fire at 10:00 AM and 10:15 AM (note that it will be UTC 
                // since servers are all set to UTC as time zone), respectively
                var now = DateTime.Now;
                var tenOClockAm = DateTime.Today.AddHours(10.0);
                var tenFifteenAm = tenOClockAm.AddMinutes(15.0);
                var tenTwentyAm = tenFifteenAm.AddMinutes(5.0);
                var tenTwentyFiveAm = tenTwentyAm.AddMinutes(5.0);

                // if already passed trigger times for today, we wait until tomorrow for first trigger
                if (now > tenOClockAm)
                {
                    tenOClockAm = tenOClockAm.AddDays(1.0);
                }

                if (now > tenFifteenAm)
                {
                    tenFifteenAm = tenFifteenAm.AddDays(1.0);
                }

                if (now > tenTwentyAm)
                {
                    tenTwentyAm = tenTwentyAm.AddDays(1.0);
                }

                if (now > tenTwentyFiveAm)
                {
                    tenTwentyFiveAm = tenTwentyFiveAm.AddDays(1.0);
                }

                var secondsUntilTen = (tenOClockAm - now).TotalSeconds;
                var secondsUntilTenFifteen = (tenFifteenAm - now).TotalSeconds;
                var secondsUntilTenTwenty = (tenTwentyAm - now).TotalSeconds;
                var secondsUntilTenTwentyFive = (tenTwentyFiveAm - now).TotalSeconds;

                EgmDataCleanTimer = new CHGTimer();
                EgmDataCleanTimer.TimerEvent += CleanEgmDataFromDatastore;
                EgmDataCleanTimer.Start(TimeSpan.FromSeconds(secondsUntilTen),
                    TimeSpan.FromHours(EgmDataCleanIntervalHours));

                DataBackupCleanTimer = new CHGTimer();
                DataBackupCleanTimer.TimerEvent += CleanBackupData;
                DataBackupCleanTimer.Start(TimeSpan.FromSeconds(secondsUntilTenFifteen),
                    TimeSpan.FromHours(DataBackupCleanIntervalHours));

                DiagnosticDataCleanTimer = new CHGTimer();
                DiagnosticDataCleanTimer.TimerEvent += CleanDiagnosticData;
                DiagnosticDataCleanTimer.Start(TimeSpan.FromSeconds(secondsUntilTenTwenty),
                    TimeSpan.FromHours(DiagnosticDataCleanIntervalHours));

                LocalExportDataCleanTimer = new CHGTimer();
                LocalExportDataCleanTimer.TimerEvent += CleanLocalExportData;
                LocalExportDataCleanTimer.Start(TimeSpan.FromSeconds(secondsUntilTenTwentyFive),
                    TimeSpan.FromHours(LocalExportDataCleanIntervalHours));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
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

            Logger.Debug("Disposing DataCleaner");

            if (isDispose)
            {
                lock (_locker)
                {
                    EgmDataCleanTimer.Dispose();
                    DataBackupCleanTimer.Dispose();
                }
            }

            IsDisposed = true;
        }

        #endregion

        #region Private instance methods

        /// <summary>
        /// Cleans the egm data from datastore.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        private void CleanEgmDataFromDatastore(Timer arg1, object arg2)
        {
            var now = DateTime.UtcNow;
            EgmEventDao.CleanOlderThan(now.Subtract(TimeSpan.FromHours(EventDataExpiryHours)));
            EgmMeterReadingDao.CleanOlderThan(now.Subtract(TimeSpan.FromHours(MeterDataExpiryHours)));
            EgmMetricDao.CleanOlderThan(now.Subtract(TimeSpan.FromHours(MetricDataExpiryHours)));
            EgmWindowsEventDao.CleanOlderThan(now.Subtract(TimeSpan.FromHours(WindowsEventDataExpiryHours)));
            EgmVersionDao.CleanOlderThan(now.Subtract(TimeSpan.FromHours(VersionDataExpiryHours)));
        }

        /// <summary>
        /// Cleans the backup data.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void CleanBackupData(Timer arg1, object arg2)
        {
            if (!Directory.Exists(DataBackupLocation)) return;

            var now = DateTime.Now;
            var yearNow = CleanupCalendar.GetYear(now);
            var weekNow = CleanupCalendar.GetWeekOfYear(now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            var cleanYear = yearNow - BackupDataExpiryWeeks / 52;
            var cleanWeek = weekNow - BackupDataExpiryWeeks % 52;
            if (0 >= cleanWeek)
            {
                cleanWeek = 52 + cleanWeek;
                --cleanYear;
            }

            var directoriesToDelete = new List<string>();
            var backupDirectories = Directory.EnumerateDirectories(DataBackupLocation).ToList();
            foreach (var backupDirectory in backupDirectories)
            {
                int year;
                if (!int.TryParse(new DirectoryInfo(backupDirectory).Name, out year)) continue;

                var backupSubDirectories = Directory.EnumerateDirectories(backupDirectory);
                foreach (var backupSubDirectory in backupSubDirectories)
                {
                    int week;
                    if (!int.TryParse(new DirectoryInfo(backupSubDirectory).Name, out week)) continue;

                    if (year < cleanYear || year == cleanYear && week <= cleanWeek)
                    {
                        directoriesToDelete.Add(backupSubDirectory);
                    }
                }
            }

            foreach (var directoryToDelete in directoriesToDelete)
            {
                var parent = new DirectoryInfo(directoryToDelete).Parent;
                Directory.Delete(directoryToDelete, true);
                try
                {
                    parent?.Delete();
                }
                catch (Exception)
                {
                    // no-op - couldn't delete parent directory; will occur if parent directory is not empty
                    // in which case we don't want to delete it
                }
            }
        }

        /// <summary>
        /// Cleans the diagnostic data.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        private void CleanDiagnosticData(Timer arg1, object arg2)
        {
            if (!Directory.Exists(DiagnosticsLocation)) return;

            foreach (var processedFile in Directory.GetFiles(DiagnosticsLocation))
            {
                if (DateTime.UtcNow.Subtract(File.GetCreationTimeUtc(processedFile)) >
                    TimeSpan.FromDays(DiagnosticDataExpiryDays))
                {
                    File.Delete(processedFile);
                }
            }
        }

        /// <summary>
        /// Cleans the local export data.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        private void CleanLocalExportData(Timer arg1, object arg2)
        {
            if (!Directory.Exists(LocalExportDataLocation)) return;

            var cleanUpDateTime = CleanupCalendar.AddWeeks(DateTime.Now, -1 * LocalExportDataExpiryWeeks);

            var exportDirectories = Directory.EnumerateDirectories(LocalExportDataLocation);
            var directoriesToDelete = new List<string>();
            foreach (var exportDirectory in exportDirectories)
            {
                char[] splitter = {'-'};
                var exportDataElements = new DirectoryInfo(exportDirectory).Name.Split(splitter);
                if (3 != exportDataElements.Length) continue;

                var yyyy = exportDataElements[0];
                var mm = exportDataElements[1];
                var dd = exportDataElements[2];
                int year;
                int month;
                int day;

                if (!int.TryParse(yyyy, out year) || !int.TryParse(mm, out month) ||
                    !int.TryParse(dd, out day)) continue;

                var dt = new DateTime(year, month, day);
                if (0 >= dt.CompareTo(cleanUpDateTime))
                {
                    directoriesToDelete.Add(exportDirectory);
                }
            }

            foreach (var directory in directoriesToDelete)
            {
                Directory.Delete(directory, true);
            }
        }

        #endregion
    }
}