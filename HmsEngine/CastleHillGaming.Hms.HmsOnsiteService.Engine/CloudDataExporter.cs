// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.HmsOnsiteService.Engine
// Author           : acscheiner
// Created          : 03-08-2019
//
// Last Modified By : acscheiner
// Last Modified On : 03-08-2019
// ***********************************************************************
// <copyright file="CloudDataExporter.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC. Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.HmsOnsiteService.Engine
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.ServiceModel;
    using System.Transactions;
    using CastleHillGaming.Hms.Contracts;
    using CastleHillGaming.Hms.HmsOnsiteService.Engine.Properties;
    using CastleHillGaming.Hms.Interfaces;
    using ChannelAdam.ServiceModel;
    using log4net;

    #endregion

    /// <summary>
    /// Class CloudDataExporter.
    /// Implements the <see cref="IDataExportStrategy" />
    /// </summary>
    /// <seealso cref="IDataExportStrategy" />
    internal class CloudDataExporter : IDataExportStrategy
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets the data aggregator.
        /// </summary>
        /// <value>The data aggregator.</value>
        public IDataAggregator DataAggregator { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets the data export location.
        /// </summary>
        /// <value>The export data location.</value>
        public string ExportLocation { get; set; }

        /// <inheritdoc />
        /// <exception cref="T:System.Exception">
        /// ServiceConsumerFactory.Create returned null/invalid IHmsCloudService reference.
        /// Check WCF HMS configuration.
        /// </exception>
        public void SendCasinoDataReport(IReportable reportable)
        {
            var dataReport = reportable as CasinoDataReport;
            if (null == dataReport) return;

            using (
                var cloudServiceProxy = ServiceConsumerFactory.Create<IHmsCloudService>(() => new HmsCloudServerProxy())
            )
            {
                try
                {
                    if (cloudServiceProxy?.Operations == null)
                    {
                        throw new Exception(
                            "ServiceConsumerFactory.Create returned null/invalid IHmsCloudService reference. Check WCF HMS configuration.");
                    }

                    // The TransactionScope here will provide behavior such that - if/when the call to
                    // DataAggregator.SuccessfulCasinoDataReport fails for some reason (e.g., problem
                    // writing to the database) - the transactional WCF/MSMQ message delivery will be
                    // rolled back (i.e., the messages will be discarded from the client-side MQ)
                    var txnOptions = new TransactionOptions {IsolationLevel = IsolationLevel.RepeatableRead};
                    using (var txnScope = new TransactionScope(TransactionScopeOption.Required, txnOptions))
                    {
                        cloudServiceProxy.Operations.ReportCasinoData(dataReport);
                        DataAggregator.SuccessfulCasinoDataReport(reportable.ReportGuid);

                        txnScope.Complete();
                    }
                }
                catch (FaultException fe)
                {
                    Logger.Warn($"Service operation ReportCasinoData threw a fault: [{fe.Message}]");
                    DataAggregator.UnsuccessfulCasinoDataReport(reportable.ReportGuid);
                }
                catch (Exception ex)
                {
                    Logger.Warn(
                        $"An unexpected error occurred while calling the ReportCasinoData service operation: [{ex.Message}]");
                    var innerEx = ex.InnerException;
                    while (null != innerEx)
                    {
                        Logger.Warn($"[{innerEx.Message}]");
                        innerEx = innerEx.InnerException;
                    }

                    DataAggregator.UnsuccessfulCasinoDataReport(reportable.ReportGuid);
                    Logger.Warn($"Stack Trace: [{Environment.StackTrace}]");
                }
            }
        }

        /// <inheritdoc />
        /// <exception cref="T:System.Exception">
        /// ServiceConsumerFactory.Create returned null/invalid IHmsCloudService reference.
        /// Check WCF HMS configuration.
        /// </exception>
        public void SendDataBackup(IDictionary<string, IList<byte[]>> backupData)
        {
            if (null == backupData || 0 >= backupData.Count) return;

            var casinoCode = Settings.Default.CasinoCode;
            var dataBackupFilename = string.Empty;
            var reportGuid = Guid.Empty;

            using (
                var cloudServiceProxy = ServiceConsumerFactory.Create<IHmsCloudService>(() => new HmsCloudServerProxy())
            )
            {
                try
                {
                    if (cloudServiceProxy?.Operations == null)
                    {
                        throw new Exception(
                            "ServiceConsumerFactory.Create returned null/invalid IHmsCloudService reference. Check WCF HMS configuration.");
                    }

                    foreach (var backupFilename in backupData.Keys)
                    {
                        dataBackupFilename = backupFilename;

                        var backupDataFileChunks = backupData[backupFilename];
                        if (null == backupDataFileChunks || 0 >= backupDataFileChunks.Count) continue;

                        // create a new ReportGuid - one for each filename being sent (in either one or
                        // multiple messages/chunks - based on size of backupDataFileChunks IList)
                        reportGuid = Guid.NewGuid();
                        var reportedAt = DateTime.UtcNow;

                        // The TransactionScope here will provide behavior such that - if/when the call to
                        // DataAggregator.SuccessfulCasinoDataBackup fails for some reason - the transactional 
                        // WCF/MSMQ message delivery will be rolled back (i.e., the messages will be discarded
                        // from the client-side MQ)
                        //
                        // Additionally, we bundle all the chunks for each backup data file into a single
                        // transaction. Thus, we will roll back if not all chunks are successfully
                        // processed/sent.
                        var txnOptions = new TransactionOptions {IsolationLevel = IsolationLevel.RepeatableRead};
                        using (var txnScope = new TransactionScope(TransactionScopeOption.Required, txnOptions))
                        {
                            for (var iChunk = 0; iChunk < backupDataFileChunks.Count; ++iChunk)
                            {
                                var chunk = backupDataFileChunks[iChunk];
                                if (null == chunk || 0 >= chunk.LongLength) continue;

                                var casinoDataBackup = new CasinoDataBackup
                                {
                                    Filename = backupFilename,
                                    Chunk = chunk,
                                    ChunkIndex = iChunk,
                                    NumChunks = backupDataFileChunks.Count,
                                    CasinoCode = casinoCode,
                                    ReportGuid = reportGuid,
                                    ReportedAt = reportedAt
                                };

                                cloudServiceProxy.Operations.ReportDataBackup(casinoDataBackup);
                            }

                            DataAggregator.SuccessfulCasinoDataBackup(backupFilename + @".zip", reportGuid);
                            txnScope.Complete();
                        }
                    }
                }
                catch (FaultException fe)
                {
                    Logger.Warn($"Service operation ReportDataBackup threw a fault: [{fe.Message}]");
                    if (!string.IsNullOrWhiteSpace(dataBackupFilename))
                    {
                        DataAggregator.UnsuccessfulCasinoDataBackup(dataBackupFilename + @".zip", reportGuid);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(
                        $"An unexpected error occurred while calling the ReportDataBackup service operation: [{ex.Message}]");
                    var innerEx = ex.InnerException;
                    while (null != innerEx)
                    {
                        Logger.Warn($"[{innerEx.Message}]");
                        innerEx = innerEx.InnerException;
                    }

                    Logger.Warn($"Stack Trace: [{Environment.StackTrace}]");

                    if (!string.IsNullOrWhiteSpace(dataBackupFilename))
                    {
                        DataAggregator.UnsuccessfulCasinoDataBackup(dataBackupFilename + @".zip", reportGuid);
                    }
                }
            }
        }

        /// <inheritdoc />
        /// <exception cref="T:System.Exception">
        /// ServiceConsumerFactory.Create returned null/invalid IHmsCloudService reference.
        /// Check WCF HMS configuration.
        /// </exception>
        public void SendCasinoDiagnosticData(IDictionary<string, IList<byte[]>> diagnosticData)
        {
            if (null == diagnosticData || 0 >= diagnosticData.Count) return;

            var casinoCode = Settings.Default.CasinoCode;

            using (
                var cloudServiceProxy = ServiceConsumerFactory.Create<IHmsCloudService>(() => new HmsCloudServerProxy())
            )
            {
                try
                {
                    if (cloudServiceProxy?.Operations == null)
                    {
                        throw new Exception(
                            "ServiceConsumerFactory.Create returned null/invalid IHmsCloudService reference. Check WCF HMS configuration.");
                    }

                    foreach (var diagnosticFilename in diagnosticData.Keys)
                    {
                        var diagnosticFileChunks = diagnosticData[diagnosticFilename];
                        if (null == diagnosticFileChunks || 0 >= diagnosticFileChunks.Count) continue;

                        // create a new ReportGuid - one for each filename being sent (in either one or
                        // multiple messages/chunks - based on size of diagnosticFileChunks IList)
                        var reportGuid = Guid.NewGuid();
                        var reportedAt = DateTime.UtcNow;

                        // The TransactionScope here will provide behavior such that - if/when the call to
                        // DataAggregator.SuccessfulCasinoDiagnosticReport fails for some reason - the transactional
                        // WCF/MSMQ message delivery will be rolled back (i.e., the messages will be discarded
                        // from the client-side MQ).
                        //
                        // Additionally, we bundle all the chunks for each diagnostic file into a single
                        // transaction. Thus, we will roll back if not all chunks are successfully
                        // processed/sent.
                        var txnOptions = new TransactionOptions {IsolationLevel = IsolationLevel.RepeatableRead};
                        using (var txnScope = new TransactionScope(TransactionScopeOption.Required, txnOptions))
                        {
                            for (var iChunk = 0; iChunk < diagnosticFileChunks.Count; ++iChunk)
                            {
                                var chunk = diagnosticFileChunks[iChunk];
                                if (null == chunk || 0 >= chunk.LongLength) continue;

                                var casinoDiagnosticData = new CasinoDiagnosticData
                                {
                                    Filename = diagnosticFilename,
                                    Chunk = chunk,
                                    ChunkIndex = iChunk,
                                    NumChunks = diagnosticFileChunks.Count,
                                    CasinoCode = casinoCode,
                                    ReportGuid = reportGuid,
                                    ReportedAt = reportedAt
                                };

                                cloudServiceProxy.Operations.ReportCasinoDiagnostics(casinoDiagnosticData);
                            }

                            DataAggregator.SuccessfulCasinoDiagnosticReport(diagnosticFilename, reportGuid);
                            txnScope.Complete();
                        }
                    }
                }
                catch (FaultException fe)
                {
                    Logger.Warn($"Service operation ReportCasinoDiagnostics threw a fault: [{fe.Message}]");
                }
                catch (Exception ex)
                {
                    Logger.Warn(
                        $"An unexpected error occurred while calling the ReportCasinoDiagnostics service operation: [{ex.Message}]");
                    var innerEx = ex.InnerException;
                    while (null != innerEx)
                    {
                        Logger.Warn($"[{innerEx.Message}]");
                        innerEx = innerEx.InnerException;
                    }

                    Logger.Warn($"Stack Trace: [{Environment.StackTrace}]");
                }
            }
        }
    }
}