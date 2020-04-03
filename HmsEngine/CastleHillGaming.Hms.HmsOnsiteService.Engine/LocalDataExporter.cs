// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.HmsOnsiteService.Engine
// Author           : acscheiner
// Created          : 03-08-2019
//
// Last Modified By : acscheiner
// Last Modified On : 03-08-2019
// ***********************************************************************
// <copyright file="LocalDataExporter.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC. Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.HmsOnsiteService.Engine
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using CastleHill.Utilities;
    using CastleHillGaming.Hms.Contracts;
    using CastleHillGaming.Hms.Interfaces;
    using log4net;
    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// Class LocalDataExporter.
    /// Implements the <see cref="IDataExportStrategy" />
    /// </summary>
    /// <seealso cref="IDataExportStrategy" />
    internal class LocalDataExporter : IDataExportStrategy
    {
        #region Private Static Data Members

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends the casino data report.
        /// </summary>
        /// <param name="reportable">The casino data report.</param>
        /// <inheritdoc />
        public void SendCasinoDataReport(IReportable reportable)
        {
            var casinoDataReport = reportable as CasinoDataReport;
            if (null == casinoDataReport) return;

            ExportCasinoDataReportAsJson(casinoDataReport);
        }

        /// <summary>
        /// Sends the data backup.
        /// </summary>
        /// <param name="backupData">The backup data.</param>
        /// <inheritdoc />
        public void SendDataBackup(IDictionary<string, IList<byte[]>> backupData)
        {
            // for now (and probably forever) no-op;
            //
            // It's not particularly important to gather a history of
            // ChgGameServer database backups and store those in cases
            // where there is no HMS CloudServer connectivity.
        }

        /// <summary>
        /// Sends the casino diagnostic data.
        /// </summary>
        /// <param name="diagnosticData">The diagnostic data.</param>
        /// <inheritdoc />
        public void SendCasinoDiagnosticData(IDictionary<string, IList<byte[]>> diagnosticData)
        {
            // for now (and probably forever) no-op;
            //
            // It's not particularly important to separately capture and
            // push the Diagnostics files here. The expectation is that 
            // field techs will capture them as needed from their original
            // location on the OnSite server host.
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Exports ICasinoDataReport data as CSV.
        /// </summary>
        /// <param name="casinoDataReport">The casino data report.</param>
        private void ExportCasinoDataReportAsJson(ICasinoDataReport casinoDataReport)
        {
            string casinoDataReportFile = null;

            try
            {
                var exportDirectory = Directory.CreateDirectory(Path.Combine(ExportLocation,
                    DateTime.UtcNow.ToString("yyyy-MM-dd")));

                casinoDataReportFile = Path.Combine(exportDirectory.FullName,
                    $"casinoDataReport-{Regex.Replace(casinoDataReport.CasinoCode, @"\s+", "")}-{casinoDataReport.ReportedAt:yyyy-MM-dd-HHmmss}-{casinoDataReport.ReportGuid.ToString()}.txt");
                var casinoDataReportJson = JsonConvert.SerializeObject(casinoDataReport, Formatting.None,
                    new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto});

                if (!string.IsNullOrWhiteSpace(casinoDataReportJson))
                {
                    File.WriteAllText(casinoDataReportFile, StringEncryption.EncryptString(casinoDataReportJson));
                }

                DataAggregator.SuccessfulCasinoDataReport(casinoDataReport.ReportGuid);
            }
            catch (Exception ex)
            {
                Logger.Warn(
                    $"An unexpected error occurred in LocalDataExporter.ExportCasinoDataReportAsJson method: [{ex.Message}]");
                var innerEx = ex.InnerException;
                while (null != innerEx)
                {
                    Logger.Warn($"[{innerEx.Message}]");
                    innerEx = innerEx.InnerException;
                }

                DataAggregator.UnsuccessfulCasinoDataReport(casinoDataReport.ReportGuid);
                Logger.Warn($"Stack Trace: [{Environment.StackTrace}]");

                // clean up any files created during this failed export
                try
                {
                    if (!string.IsNullOrWhiteSpace(casinoDataReportFile))
                    {
                        File.Delete(casinoDataReportFile);
                    }
                }
                catch (Exception exception)
                {
                    // just log the issue, nothing else needed
                    Logger.Warn(
                        $"LocalDataExporter.ExportCasinoDataReportAsJson: problem deleting [{casinoDataReportFile}] file in catch block [{exception.Message}]");
                }
            }
        }

        #endregion
    }
}