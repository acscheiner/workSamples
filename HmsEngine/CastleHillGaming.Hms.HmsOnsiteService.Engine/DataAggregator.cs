// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.HmsOnsiteService.Engine
// Author           : acscheiner
// Created          : 10-04-2016
//
// Last Modified By : acscheiner
// Last Modified On : 10-04-2016
// ***********************************************************************
// <copyright file="DataAggregator.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.HmsOnsiteService.Engine
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using CastleHill.SharedUtils;
    using CastleHillGaming.Hms.Contracts;
    using CastleHillGaming.Hms.DataModel;
    using CastleHillGaming.Hms.DataModel.DataAccessLayer.Dao;
    using CastleHillGaming.Hms.HmsOnsiteService.Engine.Properties;
    using CastleHillGaming.Hms.Interfaces;
    using log4net;

    #endregion

    /// <summary>
    /// Class DataAggregator.
    /// </summary>
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IDataAggregator" />
    /// <seealso cref="System.IDisposable" />
    public class DataAggregator : IDataAggregator
    {
        #region Private consts

        /// <summary>
        /// The breadcrumb
        /// </summary>
        private const string Breadcrumb = @"\breadcrumb";

        /// <summary>
        /// The default maximum data records per report
        /// </summary>
        private const int DefaultMaxDataRecordsPerReport = 3400;

        #endregion

        #region Private Static data

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Game Titles present in this list will be ignored - i.e., not imported into the HMS
        /// OnSite Server data store.
        /// </summary>
        private static readonly ICollection<string> GameTitlesToIgnore = new List<string> {"Unused"};

        #endregion

        #region Private Instance data

        /// <summary>
        /// The produce consume
        /// </summary>
        private ProduceConsume<IEgmData> _produceConsume;

        /// <summary>
        /// The _locker for thread safety
        /// </summary>
        private readonly object _locker = new object();

        /// <summary>
        /// Boolean flag to track if the DataAggregator has been started
        /// </summary>
        private bool _isStarted;

        /// <summary>
        /// Boolean flag to track if the DataAggregator has been disposed
        /// </summary>
        private bool _isDisposed;

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
        /// Gets the data backup location.
        /// </summary>
        /// <value>The data backup location.</value>
        public string DataBackupLocation { get; private set; }

        /// <summary>
        /// The calendar
        /// </summary>
        private Calendar BackupCalendar { get; } = CultureInfo.InvariantCulture.Calendar;


        /// <summary>
        /// Gets the CancellationTokenSource.
        /// </summary>
        /// <value>The CancellationTokenSource.</value>
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

        #endregion

        #region Public instance methods

        /// <summary>
        /// Reports the egm meters.
        /// </summary>
        /// <param name="egmMeterData">The egm meter data.</param>
        public void ReportEgmMeters(IEgmMeterData egmMeterData)
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return;
            }

            lock (_locker)
            {
                _produceConsume.Produce(egmMeterData);
            }
        }

        /// <summary>
        /// Reports the egm events.
        /// </summary>
        /// <param name="egmEventData">The egm event data.</param>
        public void ReportEgmEvents(IEgmEventData egmEventData)
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return;
            }

            lock (_locker)
            {
                _produceConsume.Produce(egmEventData);
            }
        }

        /// <summary>
        /// Reports the egm metrics.
        /// </summary>
        /// <param name="egmMetricData">The egm metric data.</param>
        public void ReportEgmMetrics(IEgmMetricData egmMetricData)
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return;
            }

            lock (_locker)
            {
                _produceConsume.Produce(egmMetricData);
            }
        }

        /// <summary>
        /// Reports the windows events.
        /// </summary>
        /// <param name="egmWindowsEventData">The egm windows event data.</param>
        public void ReportWindowsEvents(IEgmWindowsEventData egmWindowsEventData)
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return;
            }

            lock (_locker)
            {
                _produceConsume.Produce(egmWindowsEventData);
            }
        }

        public void ReportEgmVersions(IEgmVersionData egmVersionData)
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return;
            }

            lock (_locker)
            {
                _produceConsume.Produce(egmVersionData);
            }
        }

        /// <summary>
        /// Gets the casino data report for submission to the Cloud-host HMS service.
        /// </summary>
        /// <returns>ICasinoDataReport.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public ICasinoDataReport GetCasinoDataReport()
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return null;
            }

            // Determine how much data needs to be gathered/sent to Cloud
            var numMeterReadings = EgmMeterReadingDao.NumUnsent();
            var numEvents = EgmEventDao.NumUnsent();
            var numMetrics = EgmMetricDao.NumUnsent();
            var numWinEvents = EgmWindowsEventDao.NumUnsent();
            var numVersions = EgmVersionDao.NumUnsent();
            var numTotal = numMeterReadings + numEvents + numMetrics + numWinEvents + numVersions;

            if (0 >= numTotal)
            {
                // no new data to report, so return null
                return null;
            }

            // Determine bundle sizes for each type of EGM data
            var meterReadingBundleSize = numMeterReadings;
            var eventBundleSize = numEvents;
            var metricBundleSize = numMetrics;
            var winEventBundleSize = numWinEvents;
            var versionBundleSize = numVersions;

            if (DefaultMaxDataRecordsPerReport < numTotal)
            {
                // Too much data to send in a single WCF-MSMQ message
                // Thus, we need to limit data bundled/sent
                if (DefaultMaxDataRecordsPerReport < meterReadingBundleSize)
                {
                    meterReadingBundleSize = DefaultMaxDataRecordsPerReport;
                    eventBundleSize = 0;
                    metricBundleSize = 0;
                    winEventBundleSize = 0;
                    versionBundleSize = 0;
                }
                else
                {
                    var remainder = DefaultMaxDataRecordsPerReport - meterReadingBundleSize;
                    if (remainder < eventBundleSize)
                    {
                        eventBundleSize = remainder;
                        metricBundleSize = 0;
                        winEventBundleSize = 0;
                        versionBundleSize = 0;
                    }
                    else
                    {
                        remainder -= eventBundleSize;
                        if (remainder < metricBundleSize)
                        {
                            metricBundleSize = remainder;
                            winEventBundleSize = 0;
                            versionBundleSize = 0;
                        }
                        else
                        {
                            remainder -= metricBundleSize;
                            if (remainder < winEventBundleSize)
                            {
                                winEventBundleSize = remainder;
                                versionBundleSize = 0;
                            }
                            else
                            {
                                remainder -= winEventBundleSize;
                                if (remainder < versionBundleSize)
                                {
                                    versionBundleSize = remainder;
                                }
                            }
                        }
                    }
                }
            }

            var reportGuid = Guid.NewGuid();

            var casinoDataReport = new CasinoDataReport
            {
                CasinoCode = Settings.Default.CasinoCode,
                ReportGuid = reportGuid,
                ReportedAt = DateTime.UtcNow,
                EgmEventData = new List<IEgmEventData>(),
                EgmMeterData = new List<IEgmMeterData>(),
                EgmMetricData = new List<IEgmMetricData>(),
                EgmWindowsEventData = new List<IEgmWindowsEventData>(),
                EgmVersionData = new List<IEgmVersionData>()
            };

            // bundle up the EgmMeterReadings into the CasinoDataReport
            var unsentMeterReadingIds = new List<long>();
            var corruptMeterReadingIds = new List<long>();
            BundleMeterReadingsData(reportGuid, unsentMeterReadingIds, corruptMeterReadingIds, casinoDataReport,
                meterReadingBundleSize);

            // bundle up the EgmEvents into the CasinoDataReport
            var unsentEventIds = new List<long>();
            var corruptEventIds = new List<long>();
            BundleEventData(reportGuid, unsentEventIds, corruptEventIds, casinoDataReport, eventBundleSize);

            // bundle up the EgmMetric data into the CasinoDataReport
            var unsentMetricIds = new List<long>();
            var corruptMetricIds = new List<long>();
            BundleMetricData(reportGuid, unsentMetricIds, corruptMetricIds, casinoDataReport, metricBundleSize);

            // bundle up the EgmEvents into the CasinoDataReport
            var unsentWindowsEventIds = new List<long>();
            var corruptWindowsEventIds = new List<long>();
            BundleWindowsEventData(reportGuid, unsentWindowsEventIds, corruptWindowsEventIds, casinoDataReport,
                winEventBundleSize);

            // bundle up the Egm Version into the CasinoDataReport
            var unsentVersionIds = new List<long>();
            var corruptVersionIds = new List<long>();
            BundleVersionData(reportGuid, unsentVersionIds, corruptVersionIds, casinoDataReport, versionBundleSize);

            // remove any corrupt records which were found from the data store
            PurgeCorruptEntities(corruptMeterReadingIds, corruptEventIds, corruptMetricIds, corruptWindowsEventIds,
                corruptVersionIds);

            // save the updated meter reading, event, and metric entities back to the data store
            SetUnsentReportGuid(reportGuid, unsentMeterReadingIds, unsentEventIds, unsentMetricIds,
                unsentWindowsEventIds, unsentVersionIds);

            return casinoDataReport;
        }

        /// <summary>
        /// Handles tasks resulting from a successful send of casino data report
        /// to the HMS Cloud Service.
        /// </summary>
        /// <param name="reportGuid">The report unique identifier.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void SuccessfulCasinoDataReport(Guid reportGuid)
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return;
            }

            if (reportGuid.Equals(Guid.Empty))
            {
                Logger.Warn("Guid.Empty is not a valid report GUID for DataAggregator.SuccessfulCasinoDataReport");
                return;
            }

            var reportMeterIds =
                EgmMeterReadingDao.GetByReportGuid(reportGuid).Select(meterReading => meterReading.Id).ToList();
            var reportEventIds = EgmEventDao.GetByReportGuid(reportGuid).Select(evt => evt.Id).ToList();
            var reportMetricIds = EgmMetricDao.GetByReportGuid(reportGuid).Select(metric => metric.Id).ToList();
            var reportWindowsEventIds =
                EgmWindowsEventDao.GetByReportGuid(reportGuid).Select(winEvt => winEvt.Id).ToList();
            var reportVersionIds = EgmVersionDao.GetByReportGuid(reportGuid).Select(version => version.Id).ToList();

            var sentAt = DateTime.UtcNow;

            var egmMeterReadings = new List<EgmMeterReading>();
            foreach (var meterId in reportMeterIds)
            {
                var meterEntity = EgmMeterReadingDao.GetById(meterId);
                meterEntity.SentAt = sentAt;
                egmMeterReadings.Add(meterEntity);
            }

            var egmEvents = new List<EgmEvent>();
            foreach (var eventId in reportEventIds)
            {
                var eventEntity = EgmEventDao.GetById(eventId);
                eventEntity.SentAt = sentAt;
                egmEvents.Add(eventEntity);
            }

            var egmMetrics = new List<EgmMetric>();
            foreach (var metricId in reportMetricIds)
            {
                var metricEntity = EgmMetricDao.GetById(metricId);
                metricEntity.SentAt = sentAt;
                egmMetrics.Add(metricEntity);
            }

            var egmWindowsEvents = new List<EgmWindowsEvent>();
            foreach (var windowsEventId in reportWindowsEventIds)
            {
                var windowsEventEntity = EgmWindowsEventDao.GetById(windowsEventId);
                windowsEventEntity.SentAt = sentAt;
                egmWindowsEvents.Add(windowsEventEntity);
            }

            var egmVersions = new List<EgmVersion>();
            foreach (var versionId in reportVersionIds)
            {
                var versionEntity = EgmVersionDao.GetById(versionId);
                versionEntity.SentAt = sentAt;
                egmVersions.Add(versionEntity);
            }

            if (0 < egmMeterReadings.Count)
            {
                EgmMeterReadingDao.Save(egmMeterReadings);
            }

            if (0 < egmEvents.Count)
            {
                EgmEventDao.Save(egmEvents);
            }

            if (0 < egmMetrics.Count)
            {
                EgmMetricDao.Save(egmMetrics);
            }

            if (0 < egmWindowsEvents.Count)
            {
                EgmWindowsEventDao.Save(egmWindowsEvents);
            }

            if (egmVersions.Count > 0)
            {
                EgmVersionDao.Save(egmVersions);
            }
        }

        public void UnsuccessfulCasinoDataReport(Guid reportGuid)
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return;
            }

            if (reportGuid.Equals(Guid.Empty))
            {
                Logger.Warn("Guid.Empty is not a valid report GUID for DataAggregator.UnsuccessfulCasinoDataReport");
                return;
            }

            var reportMeterIds =
                EgmMeterReadingDao.GetByReportGuid(reportGuid).Select(meterReading => meterReading.Id).ToList();
            var reportEventIds = EgmEventDao.GetByReportGuid(reportGuid).Select(evt => evt.Id).ToList();
            var reportMetricIds = EgmMetricDao.GetByReportGuid(reportGuid).Select(metric => metric.Id).ToList();
            var reportWindowsEventIds =
                EgmWindowsEventDao.GetByReportGuid(reportGuid).Select(winEvt => winEvt.Id).ToList();
            var reportVersionIds = EgmVersionDao.GetByReportGuid(reportGuid).Select(version => version.Id).ToList();

            var egmMeterReadings = new List<EgmMeterReading>();
            foreach (var meterId in reportMeterIds)
            {
                var meterEntity = EgmMeterReadingDao.GetById(meterId);
                meterEntity.SentAt = DaoUtilities.UnsentData;
                egmMeterReadings.Add(meterEntity);
            }

            var egmEvents = new List<EgmEvent>();
            foreach (var eventId in reportEventIds)
            {
                var eventEntity = EgmEventDao.GetById(eventId);
                eventEntity.SentAt = DaoUtilities.UnsentData;
                egmEvents.Add(eventEntity);
            }

            var egmMetrics = new List<EgmMetric>();
            foreach (var metricId in reportMetricIds)
            {
                var metricEntity = EgmMetricDao.GetById(metricId);
                metricEntity.SentAt = DaoUtilities.UnsentData;
                egmMetrics.Add(metricEntity);
            }

            var egmWindowsEvents = new List<EgmWindowsEvent>();
            foreach (var windowsEventId in reportWindowsEventIds)
            {
                var windowsEventEntity = EgmWindowsEventDao.GetById(windowsEventId);
                windowsEventEntity.SentAt = DaoUtilities.UnsentData;
                egmWindowsEvents.Add(windowsEventEntity);
            }

            var egmVersions = new List<EgmVersion>();
            foreach (var versionId in reportVersionIds)
            {
                var versionEntity = EgmVersionDao.GetById(versionId);
                versionEntity.SentAt = DaoUtilities.UnsentData;
                egmVersions.Add(versionEntity);
            }

            if (0 < egmMeterReadings.Count)
            {
                EgmMeterReadingDao.Save(egmMeterReadings);
            }

            if (0 < egmEvents.Count)
            {
                EgmEventDao.Save(egmEvents);
            }

            if (0 < egmMetrics.Count)
            {
                EgmMetricDao.Save(egmMetrics);
            }

            if (0 < egmWindowsEvents.Count)
            {
                EgmWindowsEventDao.Save(egmWindowsEvents);
            }

            if (egmVersions.Count > 0)
            {
                EgmVersionDao.Save(egmVersions);
            }
        }

        /// <summary>
        /// Gets the data backup.
        /// </summary>
        /// <returns>ICasinoDataBackup.</returns>
        public IDictionary<string, IList<byte[]>> GetDataBackup()
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return null;
            }

            var backupData = new Dictionary<string, IList<byte[]>>();
            var prevWeekBackupPath = ConstructPrevWeekDataBackupPath();

            // Previous weeks backup directory is missing -OR- we already exported previous weeks backup data,
            // there is no backup to be done.
            if (!Directory.Exists(prevWeekBackupPath))
            {
                return backupData;
            }

            // construct a DailyBackup from each backup directory archive
            var stringSeparators = new[] {"-"};
            foreach (var backupDirectory in Directory.GetDirectories(prevWeekBackupPath))
            {
                // parse the backup directory name into the date components
                var backupDateString = new DirectoryInfo(backupDirectory).Name;
                var backupDateArray = backupDateString.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (3 != backupDateArray.Length) continue;

                int yyyy;
                int mm;
                int dd;
                var didParseYear = int.TryParse(backupDateArray[0], out yyyy);
                var didParseMonth = int.TryParse(backupDateArray[1], out mm);
                var didParseDay = int.TryParse(backupDateArray[2], out dd);
                if (!didParseYear || !didParseMonth || !didParseDay) continue;

                // compress/zip the backup directory and capture its contents as a byte array
                var zipFilePath = prevWeekBackupPath + @"\" + backupDateString + @".zip";
                var zipzapFilePath = zipFilePath + @"zap";

                // if zipzap file exits, this data has already been exported
                if (File.Exists(zipzapFilePath)) continue;

                // If zip file already exists (e.g., if pushing this data backup to HMS Cloud
                // Server was previously attempted, but, ultimately, unsuccessful, such zip files
                // could remain if there was also a failure in the UnsuccessfulCasinoDataBackup method),
                // delete it before proceeding.
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }

                try
                {
                    ZipFile.CreateFromDirectory(backupDirectory, zipFilePath, CompressionLevel.Optimal, true,
                        Encoding.UTF8);
                    var chunks = Chunker.ChunkByteArray(File.ReadAllBytes(zipFilePath), Chunker.MsmqPayloadLimit);

                    var zipFileName = Path.GetFileNameWithoutExtension(zipFilePath);

                    Logger.Debug($"DataAggregator.GetDataBackup: filename={zipFilePath} numChunks={chunks.Count}");
                    for (var iChunk = 0; iChunk < chunks.Count; ++iChunk)
                    {
                        Logger.Debug($"Chunk[{iChunk}] Size={chunks[iChunk].Length}");
                    }

                    if (!string.IsNullOrWhiteSpace(zipFileName) && 0 < chunks.Count)
                    {
                        backupData.Add(zipFileName, chunks);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn($"DataAggregator.GetDataBackup failed with exception: {ex.Message}");
                    Logger.Warn($"Stack Trace:\n {ex.StackTrace}");
                    backupData.Clear();
                }
            }

            return backupData;
        }

        /// <summary>
        /// Handles tasks resulting from a successful send of casino data backup
        /// to the HMS Cloud Service.
        /// </summary>
        public void SuccessfulCasinoDataBackup(string backupFilename, Guid reportGuid)
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
            }

            var prevWeekBackupPath = ConstructPrevWeekDataBackupPath();
            if (!Directory.Exists(prevWeekBackupPath))
            {
                return;
            }

            var processedFiles = Directory.GetFiles(prevWeekBackupPath, backupFilename);
            if (1 != processedFiles.Length)
            {
                Logger.Warn(
                    $"DataAggregator.SuccessfulCasinoDataBackup - {processedFiles.Length} matching files found for filename {backupFilename} in {prevWeekBackupPath}. We were expecting only 1.");
                return;
            }

            var processedFile = processedFiles[0];
            if (string.IsNullOrWhiteSpace(processedFile)) return;

            try
            {
                // delete the zip files created for this backup export
                File.Delete(processedFile);
                // create a zipzap file to indicate that this data has successfully
                // been exported to cloud
                File.Create(Path.Combine(prevWeekBackupPath, backupFilename + @"zap"));
            }
            catch (Exception ex)
            {
                Logger.Warn($"DataAggregator.SuccessfulCasinoDataBackup failed with exception: {ex.Message}");
                Logger.Warn($"Stack Trace:\n {ex.StackTrace}");

                try
                {
                    // try to clean up, if possible
                    File.Delete(processedFile);
                    File.Delete(Path.Combine(prevWeekBackupPath, backupFilename + @"zap"));
                }
                catch (Exception ex2)
                {
                    Logger.Warn($"DataAggregator.SuccessfulCasinoDataBackup failed failure cleanup with exception: {ex2.Message}");
                }
            }
        }

        /// <summary>
        /// Notifies the DataAggregator of an unsuccessful casino data backup submission
        /// to the Cloud-hosted HMS service.
        /// ///
        /// </summary>
        public void UnsuccessfulCasinoDataBackup(string backupFilename, Guid reportGuid)
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
            }

            var prevWeekBackupPath = ConstructPrevWeekDataBackupPath();
            if (!Directory.Exists(prevWeekBackupPath))
            {
                return;
            }


            var processedFiles = Directory.GetFiles(prevWeekBackupPath, backupFilename);
            if (1 != processedFiles.Length)
            {
                Logger.Warn(
                    $"DataAggregator.UnsuccessfulCasinoDataBackup - {processedFiles.Length} matching files found for filename {backupFilename} in {prevWeekBackupPath}. We were expecting only 1.");
                return;
            }

            var processedFile = processedFiles[0];
            if (string.IsNullOrWhiteSpace(processedFile)) return;

            try
            {
                // delete the zip files created for this backup export
                File.Delete(processedFile);
            }
            catch (Exception ex)
            {
                Logger.Warn($"DataAggregator.UnsuccessfulCasinoDataBackup failed with exception: {ex.Message}");
                Logger.Warn($"Stack Trace:\n {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Gets the casino diagnostic data.
        /// </summary>
        /// <returns>ICasinoDiagnosticData.</returns>
        public IDictionary<string, IList<byte[]>> GetCasinoDiagnosticData()
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return null;
            }

            var diagnosticDir = Settings.Default.DiagnosticPackagePath;
            var diagnosticData = new Dictionary<string, IList<byte[]>>();
            if (!Directory.Exists(diagnosticDir)) return diagnosticData;

            try
            {
                foreach (var diagnosticFile in Directory.GetFiles(diagnosticDir))
                {
                    var filename = Path.GetFileName(diagnosticFile);
                    var chunks = Chunker.ChunkByteArray(File.ReadAllBytes(diagnosticFile), Chunker.MsmqPayloadLimit);

                    Logger.Debug(
                        $"DataAggregator.GetCasinoDiagnosticData: filename={filename} numChunks={chunks.Count}");
                    for (var iChunk = 0; iChunk < chunks.Count; ++iChunk)
                    {
                        Logger.Debug($"Chunk[{iChunk}] Size={chunks[iChunk].Length}");
                    }

                    if (!string.IsNullOrWhiteSpace(filename) && 0 < chunks.Count)
                    {
                        diagnosticData.Add(filename, chunks);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"DataAggregator.GetCasinoDiagnosticData failed with exception: {ex.Message}");
                Logger.Warn($"Stack Trace:\n {ex.StackTrace}");
                diagnosticData.Clear();
            }

            return diagnosticData;
        }

        /// <summary>
        /// Handles tasks resulting from a successful send of casino diagnostics data file
        /// to the HMS Cloud Service.
        /// </summary>
        /// <param name="diagnosticsFilename">The diagnostics filename.</param>
        /// <param name="reportGuid">The report unique identifier.</param>
        public void SuccessfulCasinoDiagnosticReport(string diagnosticsFilename, Guid reportGuid)
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return;
            }

            var diagnosticDir = Settings.Default.DiagnosticPackagePath;
            var processedFiles = Directory.GetFiles(diagnosticDir, diagnosticsFilename);
            if (1 != processedFiles.Length)
            {
                Logger.Warn(
                    $"DataAggregator.SuccessfulCasinoDiagnosticReport - {processedFiles.Length} matching files found for filename {diagnosticsFilename} in {diagnosticDir}. We were expecting only 1.");
                return;
            }

            var processedFile = processedFiles[0];
            var processedFilename = Path.GetFileName(processedFile);

            var processedDir = diagnosticDir + @"\Processed";
            Directory.CreateDirectory(processedDir);

            File.Move(processedFile, processedDir + @"\" + processedFilename);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (IsDisposed)
            {
                Logger.Warn("Cannot Start DataAggregator - it has been Disposed.");
                return;
            }

            if (IsStarted)
            {
                Logger.Info("Cannot Start DataAggregator - it has already been started.");
                return;
            }

            Logger.Debug("Starting DataAggregator");

            lock (_locker)
            {
                IsStarted = true;
                _produceConsume = new ProduceConsume<IEgmData>(16);
                _produceConsume.DataConsumed += ProcessEgmData;
                _produceConsume.Start();
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

            Logger.Debug("Disposing DataAggregator");

            if (isDispose)
            {
                lock (_locker)
                {
                    _produceConsume.Dispose();
                }
            }

            IsDisposed = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// EventHandler triggered when ProduceConsume instance has IEgmData available
        /// for processing.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="evtArgs">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ProcessEgmData(object sender, EventArgs evtArgs)
        {
            if (IsDisposed)
            {
                Logger.Error("DataAggregator cannot accept work request - it has been Disposed.");
                return;
            }

            try
            {
                var egmData = (evtArgs as DataConsumedEventArgs<IEgmData>)?.Consumed;
                if (null == egmData)
                {
                    Logger.Warn(
                        "ProcessEgmData received Event data object of unexpected type - cannot process");
                    return;
                }

                var egmSerialNumber = egmData.EgmSerialNumber;
                var egmAssetNumber = egmData.EgmAssetNumber;
                var casinoCode = egmData.CasinoCode;
                if (string.IsNullOrEmpty(casinoCode))
                {
                    casinoCode = Settings.Default.CasinoCode;
                }

                var reportedAt = egmData.ReportedAt;

                // Event payload can hold different sub-types of IEgmData.
                // Here we determine specific sub-type of IEgmData payload and
                // process accordingly.
                if (egmData is IEgmMeterData)
                {
                    var egmMeterReadings = (from meterData in (egmData as IEgmMeterData).MeterDataList
                        where !GameTitlesToIgnore.Contains(meterData.GameTitle)
                        select new EgmMeterReading(egmSerialNumber, egmAssetNumber, reportedAt, meterData.Type,
                            meterData.Value, meterData.Units, meterData.ReadAt, casinoCode, meterData.GameTitle,
                            meterData.GameDenomination)).ToList();
                    EgmMeterReadingDao.Save(egmMeterReadings);
                }
                else if (egmData is IEgmEventData)
                {
                    var egmEvents =
                        (egmData as IEgmEventData).EventDataList.Select(
                            evt =>
                                new EgmEvent(evt.Code, evt.Description, evt.OccurredAt, casinoCode, egmSerialNumber,
                                    egmAssetNumber, reportedAt)).ToList();
                    EgmEventDao.Save(egmEvents);
                }
                else if (egmData is IEgmMetricData)
                {
                    var egmMetrics =
                        (egmData as IEgmMetricData).MetricList.Select(
                            metric =>
                                // if DynamicMetricType property is set, we use that (string) value;
                                // otherwise we use Type property (MetricType enum) value in string form
                                null != metric.DynamicMetricType
                                    ? new EgmMetric(casinoCode, egmSerialNumber, egmAssetNumber, reportedAt,
                                        metric.DynamicMetricType, metric.Value, metric.ReadAt)
                                    : new EgmMetric(casinoCode, egmSerialNumber, egmAssetNumber, reportedAt,
                                        metric.Type.ToString(),
                                        metric.Value, metric.ReadAt)
                        ).ToList();
                    EgmMetricDao.Save(egmMetrics);
                }
                else if (egmData is IEgmWindowsEventData)
                {
                    var egmWindowsEvents =
                        (egmData as IEgmWindowsEventData).WindowsEventList.Select(
                            winevt =>
                                new EgmWindowsEvent(winevt.Code, winevt.Description, winevt.EventLogName,
                                    winevt.OccurredAt,
                                    casinoCode, egmSerialNumber, egmAssetNumber, reportedAt)).ToList();
                    EgmWindowsEventDao.Save(egmWindowsEvents);
                }
                else if (egmData is IEgmVersionData)
                {
                    var egmVersions =
                        (egmData as IEgmVersionData).VersionDataList.Select(
                            version =>
                                new EgmVersion(casinoCode, egmSerialNumber, egmAssetNumber, version.ObjectName,
                                    version.VersionInfo, reportedAt)).ToList();
                    EgmVersionDao.Save(egmVersions);
                }
                else
                {
                    Logger.Warn(
                        "ProcessEgmData received Event data object of unexpected type - cannot process");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not process egm data", ex);
            }
        }

        /// <summary>
        /// Bundles the meter readings data in preparation for sending to HMS Cloud service.
        /// </summary>
        /// <param name="reportGuid">The report GUID.</param>
        /// <param name="unsentMeterReadingIds">The unsent meter reading ids will be populated inside this method.</param>
        /// <param name="corruptMeterReadingIds">The corrupt meter reading ids will be populated inside this method.</param>
        /// <param name="casinoDataReport">The casino data report will be added to inside this method.</param>
        /// <param name="bundleSize">Size of the bundle.</param>
        private void BundleMeterReadingsData(Guid reportGuid, ICollection<long> unsentMeterReadingIds,
            ICollection<long> corruptMeterReadingIds, ICasinoDataReport casinoDataReport, int bundleSize)
        {
            if (0 >= bundleSize) return;

            var currentCasino = string.Empty;
            var currentEgmSerialNumber = string.Empty;
            var currentEgmAssetNumber = string.Empty;
            var currentReportedAt = DaoUtilities.UnsentData;

            IEgmMeterData currentEgmMeterData = null;

            foreach (var meterReading in EgmMeterReadingDao.GetUnsent(false, bundleSize))
            {
                // check hash on this data record
                if (meterReading.Hash != EgmMeterReadingDao.GenerateHash(meterReading))
                {
                    Logger.Warn(
                        $"Corrupt EgmMeterReading database record detected - will delete record with Id={meterReading.Id}");
                    corruptMeterReadingIds.Add(meterReading.Id);
                    continue;
                }

                unsentMeterReadingIds.Add(meterReading.Id);

                var nextCasino = meterReading.CasinoCode;
                var nextEgmSerialNumber = meterReading.EgmSerialNumber;
                var nextEgmAssetNumber = meterReading.EgmAssetNumber;
                var nextReportedAt = meterReading.ReportedAt;

                if (!currentCasino.Equals(nextCasino) || !currentEgmSerialNumber.Equals(nextEgmSerialNumber) ||
                    !currentEgmAssetNumber.Equals(nextEgmAssetNumber) || !currentReportedAt.Equals(nextReportedAt))
                {
                    currentCasino = nextCasino;
                    currentEgmSerialNumber = nextEgmSerialNumber;
                    currentEgmAssetNumber = nextEgmAssetNumber;
                    currentReportedAt = nextReportedAt;

                    if (null != currentEgmMeterData)
                    {
                        casinoDataReport.EgmMeterData.Add(currentEgmMeterData);
                    }

                    currentEgmMeterData = new EgmMeterData
                    {
                        CasinoCode = nextCasino,
                        EgmSerialNumber = nextEgmSerialNumber,
                        EgmAssetNumber = nextEgmAssetNumber,
                        ReportedAt = nextReportedAt,
                        ReportGuid = reportGuid,
                        MeterDataList = new List<IMeterData>()
                    };
                }

                var meterData = new MeterData(meterReading);
                currentEgmMeterData?.MeterDataList.Add(meterData);
            }

            casinoDataReport.EgmMeterData.Add(currentEgmMeterData);
        }

        /// <summary>
        /// Bundles the event data in preparation for sending to HMS Cloud service.
        /// </summary>
        /// <param name="reportGuid">The report GUID.</param>
        /// <param name="unsentEventIds">The unsent event ids will be be populated inside this method.</param>
        /// <param name="corruptEventIds">The corrupt event ids will be populated inside this method.</param>
        /// <param name="casinoDataReport">The casino data report will be added to inside this method.</param>
        /// <param name="bundleSize">Size of the bundle.</param>
        private void BundleEventData(Guid reportGuid, ICollection<long> unsentEventIds,
            ICollection<long> corruptEventIds, ICasinoDataReport casinoDataReport, int bundleSize)
        {
            if (0 >= bundleSize) return;

            var currentCasino = string.Empty;
            var currentEgmSerialNumber = string.Empty;
            var currentEgmAssetNumber = string.Empty;
            var currentReportedAt = DaoUtilities.UnsentData;

            IEgmEventData currentEgmEventData = null;

            foreach (var evt in EgmEventDao.GetUnsent(false, bundleSize))
            {
                // check hash on this data record
                if (evt.Hash != EgmEventDao.GenerateHash(evt))
                {
                    Logger.Warn($"Corrupt EgmEvent database record detected - will delete record with Id={evt.Id}");
                    corruptEventIds.Add(evt.Id);
                    continue;
                }

                unsentEventIds.Add(evt.Id);

                var nextCasino = evt.CasinoCode;
                var nextEgmSerialNumber = evt.EgmSerialNumber;
                var nextEgmAssetNumber = evt.EgmAssetNumber;
                var nextReportedAt = evt.ReportedAt;

                if (!currentCasino.Equals(nextCasino) || !currentEgmSerialNumber.Equals(nextEgmSerialNumber) ||
                    !currentEgmAssetNumber.Equals(nextEgmAssetNumber) || !currentReportedAt.Equals(nextReportedAt))
                {
                    currentCasino = nextCasino;
                    currentEgmSerialNumber = nextEgmSerialNumber;
                    currentEgmAssetNumber = nextEgmAssetNumber;
                    currentReportedAt = nextReportedAt;

                    if (null != currentEgmEventData)
                    {
                        casinoDataReport.EgmEventData.Add(currentEgmEventData);
                    }

                    currentEgmEventData = new EgmEventData
                    {
                        CasinoCode = nextCasino,
                        EgmSerialNumber = nextEgmSerialNumber,
                        EgmAssetNumber = nextEgmAssetNumber,
                        ReportedAt = nextReportedAt,
                        ReportGuid = reportGuid,
                        EventDataList = new List<IEventData>()
                    };
                }

                var eventData = new EventData(evt);
                currentEgmEventData?.EventDataList.Add(eventData);
            }

            casinoDataReport.EgmEventData.Add(currentEgmEventData);
        }

        /// <summary>
        /// Bundles the metric data in preparation for sending to HMS Cloud service.
        /// </summary>
        /// <param name="reportGuid">The report GUID.</param>
        /// <param name="unsentMetricIds">The unsent metric ids will be populated inside this method.</param>
        /// <param name="corruptMetricIds">The corrupt metric ids will be populated inside this method.</param>
        /// <param name="casinoDataReport">The casino data report will be added to inside this method.</param>
        /// <param name="bundleSize">Size of the bundle.</param>
        private void BundleMetricData(Guid reportGuid, ICollection<long> unsentMetricIds,
            ICollection<long> corruptMetricIds, ICasinoDataReport casinoDataReport, int bundleSize)
        {
            if (0 >= bundleSize) return;

            var currentCasino = string.Empty;
            var currentEgmSerialNumber = string.Empty;
            var currentEgmAssetNumber = string.Empty;
            var currentReportedAt = DaoUtilities.UnsentData;

            IEgmMetricData currentEgmMetricData = null;

            foreach (var metric in EgmMetricDao.GetUnsent(false, bundleSize))
            {
                // check hash on this data record
                if (metric.Hash != EgmMetricDao.GenerateHash(metric))
                {
                    Logger.Warn($"Corrupt EgmMetric database record detected - will delete record with Id={metric.Id}");
                    corruptMetricIds.Add(metric.Id);
                    continue;
                }

                unsentMetricIds.Add(metric.Id);

                var nextCasino = metric.CasinoCode;
                var nextEgmSerialNumber = metric.EgmSerialNumber;
                var nextEgmAssetNumber = metric.EgmAssetNumber;
                var nextReportedAt = metric.ReportedAt;

                if (!currentCasino.Equals(nextCasino) || !currentEgmSerialNumber.Equals(nextEgmSerialNumber) ||
                    !currentEgmAssetNumber.Equals(nextEgmAssetNumber) || !currentReportedAt.Equals(nextReportedAt))
                {
                    currentCasino = nextCasino;
                    currentEgmSerialNumber = nextEgmSerialNumber;
                    currentEgmAssetNumber = nextEgmAssetNumber;
                    currentReportedAt = nextReportedAt;

                    if (null != currentEgmMetricData)
                    {
                        casinoDataReport.EgmMetricData.Add(currentEgmMetricData);
                    }

                    currentEgmMetricData = new EgmMetricData
                    {
                        CasinoCode = nextCasino,
                        EgmSerialNumber = nextEgmSerialNumber,
                        EgmAssetNumber = nextEgmAssetNumber,
                        ReportedAt = nextReportedAt,
                        ReportGuid = reportGuid,
                        MetricList = new List<IMetricData>()
                    };
                }

                var metricData = new MetricData(metric);
                currentEgmMetricData?.MetricList.Add(metricData);
            }

            casinoDataReport.EgmMetricData.Add(currentEgmMetricData);
        }

        /// <summary>
        /// Bundles the windows event data in preparation for sending to HMS Cloud service.
        /// </summary>
        /// <param name="reportGuid">The report GUID.</param>
        /// <param name="unsentWindowsEventIds">The unsent windows event ids will be populated inside this method.</param>
        /// <param name="corruptWindowsEventIds">The corrupt windows event ids will be populated inside this method.</param>
        /// <param name="casinoDataReport">The casino data report will be added to inside this method.</param>
        /// <param name="bundleSize">Size of the bundle.</param>
        private void BundleWindowsEventData(Guid reportGuid, ICollection<long> unsentWindowsEventIds,
            ICollection<long> corruptWindowsEventIds, ICasinoDataReport casinoDataReport, int bundleSize)
        {
            if (0 >= bundleSize) return;

            var currentCasino = string.Empty;
            var currentEgmSerialNumber = string.Empty;
            var currentEgmAssetNumber = string.Empty;
            var currentReportedAt = DaoUtilities.UnsentData;

            IEgmWindowsEventData currentEgmWindowsEventData = null;

            foreach (var winEvt in EgmWindowsEventDao.GetUnsent(false, bundleSize))
            {
                // check hash on this data record
                if (winEvt.Hash != EgmWindowsEventDao.GenerateHash(winEvt))
                {
                    Logger.Warn(
                        $"Corrupt EgmWindowsEvent database record detected - will delete record with Id={winEvt.Id}");
                    corruptWindowsEventIds.Add(winEvt.Id);
                    continue;
                }

                unsentWindowsEventIds.Add(winEvt.Id);

                var nextCasino = winEvt.CasinoCode;
                var nextEgmSerialNumber = winEvt.EgmSerialNumber;
                var nextEgmAssetNumber = winEvt.EgmAssetNumber;
                var nextReportedAt = winEvt.ReportedAt;

                if (!currentCasino.Equals(nextCasino) || !currentEgmSerialNumber.Equals(nextEgmSerialNumber) ||
                    !currentEgmAssetNumber.Equals(nextEgmAssetNumber) || !currentReportedAt.Equals(nextReportedAt))
                {
                    currentCasino = nextCasino;
                    currentEgmSerialNumber = nextEgmSerialNumber;
                    currentEgmAssetNumber = nextEgmAssetNumber;
                    currentReportedAt = nextReportedAt;

                    if (null != currentEgmWindowsEventData)
                    {
                        casinoDataReport.EgmWindowsEventData.Add(currentEgmWindowsEventData);
                    }

                    currentEgmWindowsEventData = new EgmWindowsEventData
                    {
                        CasinoCode = nextCasino,
                        EgmSerialNumber = nextEgmSerialNumber,
                        EgmAssetNumber = nextEgmAssetNumber,
                        ReportedAt = nextReportedAt,
                        ReportGuid = reportGuid,
                        WindowsEventList = new List<IWindowsEventData>()
                    };
                }

                var eventData = new WindowsEventData(winEvt);
                currentEgmWindowsEventData?.WindowsEventList.Add(eventData);
            }

            casinoDataReport.EgmWindowsEventData.Add(currentEgmWindowsEventData);
        }

        /// <summary>
        /// Bundles the version  data in preparation for sending to HMS Cloud service.
        /// </summary>
        /// <param name="reportGuid">The report GUID.</param>
        /// <param name="unsentVersionIds">The unsent version ids will be populated inside this method.</param>
        /// <param name="corruptVersionIds">The corrupt version ids will be populated inside this method.</param>
        /// <param name="casinoDataReport">The casino data report will be added to inside this method.</param>
        /// <param name="bundleSize">Size of the bundle.</param>
        private void BundleVersionData(Guid reportGuid, ICollection<long> unsentVersionIds,
            ICollection<long> corruptVersionIds, ICasinoDataReport casinoDataReport, int bundleSize)
        {
            if (0 >= bundleSize) return;

            var currentCasino = string.Empty;
            var currentEgmSerialNumber = string.Empty;
            var currentEgmAssetNumber = string.Empty;
            var currentReportedAt = DaoUtilities.UnsentData;

            IEgmVersionData currentEgmVersionData = null;

            foreach (var version in EgmVersionDao.GetUnsent(false, bundleSize))
            {
                // check hash on this data record
                if (version.Hash != EgmVersionDao.GenerateHash(version))
                {
                    Logger.Warn(
                        $"Corrupt EgmVersion database record detected - will delete record with Id={version.Id}");
                    corruptVersionIds.Add(version.Id);
                    continue;
                }

                unsentVersionIds.Add(version.Id);

                var nextCasino = version.CasinoCode;
                var nextEgmSerialNumber = version.EgmSerialNumber;
                var nextEgmAssetNumber = version.EgmAssetNumber;
                var nextReportedAt = version.ReportedAt;

                if (!currentCasino.Equals(nextCasino) || !currentEgmSerialNumber.Equals(nextEgmSerialNumber) ||
                    !currentEgmAssetNumber.Equals(nextEgmAssetNumber) || !currentReportedAt.Equals(nextReportedAt))
                {
                    if (null != currentEgmVersionData)
                    {
                        casinoDataReport.EgmVersionData.Add(currentEgmVersionData);
                    }

                    currentEgmVersionData = new EgmVersionData
                    {
                        CasinoCode = nextCasino,
                        EgmSerialNumber = nextEgmSerialNumber,
                        EgmAssetNumber = nextEgmAssetNumber,
                        ReportedAt = nextReportedAt,
                        ReportGuid = reportGuid,
                        VersionDataList = new List<IVersionData>()
                    };

                    currentCasino = nextCasino;
                    currentEgmSerialNumber = nextEgmSerialNumber;
                    currentEgmAssetNumber = nextEgmAssetNumber;
                    currentReportedAt = nextReportedAt;
                }

                var versionData = new VersionData(version);
                currentEgmVersionData?.VersionDataList.Add(versionData);
            }

            casinoDataReport.EgmVersionData.Add(currentEgmVersionData);
        }

        /// <summary>
        /// Purges the corrupt entities.
        /// </summary>
        /// <param name="corruptMeterReadingIds">The corrupt meter reading ids.</param>
        /// <param name="corruptEventIds">The corrupt event ids.</param>
        /// <param name="corruptMetricIds">The corrupt metric ids.</param>
        /// <param name="corruptWindowsEventIds">The corrupt windows event ids.</param>
        /// <param name="corruptVersionIds">The corrupt version ids.</param>
        private void PurgeCorruptEntities(IEnumerable<long> corruptMeterReadingIds, IEnumerable<long> corruptEventIds,
            IEnumerable<long> corruptMetricIds, IEnumerable<long> corruptWindowsEventIds,
            IEnumerable<long> corruptVersionIds)
        {
            foreach (var meterId in corruptMeterReadingIds)
            {
                var meterReadingEntity = EgmMeterReadingDao.GetById(meterId);
                EgmMeterReadingDao.Delete(meterReadingEntity);
            }

            foreach (var eventId in corruptEventIds)
            {
                var eventEntity = EgmEventDao.GetById(eventId);
                EgmEventDao.Delete(eventEntity);
            }

            foreach (var metricId in corruptMetricIds)
            {
                var metricEntity = EgmMetricDao.GetById(metricId);
                EgmMetricDao.Delete(metricEntity);
            }

            foreach (var windowsEventId in corruptWindowsEventIds)
            {
                var winEvtEntity = EgmWindowsEventDao.GetById(windowsEventId);
                EgmWindowsEventDao.Delete(winEvtEntity);
            }

            foreach (var versionId in corruptVersionIds)
            {
                var versionEntity = EgmVersionDao.GetById(versionId);
                EgmVersionDao.Delete(versionEntity);
            }
        }

        /// <summary>
        /// Sets the ReportGuid property for unsent entities about to be sent to HMS Cloud service.
        /// </summary>
        /// <param name="reportGuid">The report GUID.</param>
        /// <param name="unsentMeterReadingIds">The unsent meter reading ids.</param>
        /// <param name="unsentEventIds">The unsent event ids.</param>
        /// <param name="unsentMetricIds">The unsent metric ids.</param>
        /// <param name="unsentWindowsEventIds">The unsent windows event ids.</param>
        /// <param name="unsentVersionIds">The unsent version ids.</param>
        private void SetUnsentReportGuid(Guid reportGuid, IEnumerable<long> unsentMeterReadingIds,
            IEnumerable<long> unsentEventIds, IEnumerable<long> unsentMetricIds,
            IEnumerable<long> unsentWindowsEventIds,
            IEnumerable<long> unsentVersionIds)
        {
            var egmMeterReadings = new List<EgmMeterReading>();
            foreach (var meterId in unsentMeterReadingIds)
            {
                var meterReadingEntity = EgmMeterReadingDao.GetById(meterId);
                meterReadingEntity.ReportGuid = reportGuid;
                meterReadingEntity.SentAt = DaoUtilities.DataSendInProgress;
                egmMeterReadings.Add(meterReadingEntity);
            }

            var egmEvents = new List<EgmEvent>();
            foreach (var eventId in unsentEventIds)
            {
                var eventEntity = EgmEventDao.GetById(eventId);
                eventEntity.ReportGuid = reportGuid;
                eventEntity.SentAt = DaoUtilities.DataSendInProgress;
                egmEvents.Add(eventEntity);
            }

            var egmMetrics = new List<EgmMetric>();
            foreach (var metricId in unsentMetricIds)
            {
                var metricEntity = EgmMetricDao.GetById(metricId);
                metricEntity.ReportGuid = reportGuid;
                metricEntity.SentAt = DaoUtilities.DataSendInProgress;
                egmMetrics.Add(metricEntity);
            }

            var egmWindowsEvents = new List<EgmWindowsEvent>();
            foreach (var windowsEventId in unsentWindowsEventIds)
            {
                var winEvtEntity = EgmWindowsEventDao.GetById(windowsEventId);
                winEvtEntity.ReportGuid = reportGuid;
                winEvtEntity.SentAt = DaoUtilities.DataSendInProgress;
                egmWindowsEvents.Add(winEvtEntity);
            }

            var egmVersions = new List<EgmVersion>();
            foreach (var versionId in unsentVersionIds)
            {
                var versionEntity = EgmVersionDao.GetById(versionId);
                versionEntity.ReportGuid = reportGuid;
                versionEntity.SentAt = DaoUtilities.DataSendInProgress;
                egmVersions.Add(versionEntity);
            }

            if (0 < egmMeterReadings.Count)
            {
                EgmMeterReadingDao.Save(egmMeterReadings);
            }

            if (0 < egmEvents.Count)
            {
                EgmEventDao.Save(egmEvents);
            }

            if (0 < egmMetrics.Count)
            {
                EgmMetricDao.Save(egmMetrics);
            }

            if (0 < egmWindowsEvents.Count)
            {
                EgmWindowsEventDao.Save(egmWindowsEvents);
            }

            if (egmVersions.Count > 0)
            {
                EgmVersionDao.Save(egmVersions);
            }
        }

        /// <summary>
        /// Constructs the previous week data backup path.
        /// </summary>
        /// <returns>System.String.</returns>
        private string ConstructPrevWeekDataBackupPath()
        {
            // Construct the full path to last week's data backup files
            var now = DateTime.Now;
            var year = BackupCalendar.GetYear(now);
            var week = BackupCalendar.GetWeekOfYear(now, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            var prevWeek = week - 1;
            if (0 == prevWeek)
            {
                prevWeek = 52;
                --year;
            }

            return DataBackupLocation + year + @"\" + prevWeek;
        }

        #endregion
    }
}