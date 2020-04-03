// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.HmsOnsiteService.Engine
// Author           : acscheiner
// Created          : 09-22-2016
//
// Last Modified By : acscheiner
// Last Modified On : 09-22-2016
// ***********************************************************************
// <copyright file="HmsOnsiteService.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.HmsOnsiteService.Engine
{
    #region

    using System;
    using System.IO;
    using System.ServiceModel;
    using log4net;
    using Contracts;
    using Contracts.Streaming;
    using Interfaces;
    using Properties;
    using Spring.Context.Support;
    using System.Reflection;

    #endregion

    /// <summary>
    ///     Class HmsOnsiteService.
    /// </summary>
    /// <seealso cref="IHmsService" />
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class HmsOnsiteService : IHmsService, IHmsFileUploadService
    {
        #region Private static member data

        /// <summary>
        ///     The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        private const int BufferSize = 4096;

        /// <summary>
        ///     The data aggregator
        /// </summary>
        private IDataAggregator _dataAggregator;

        /// <summary>
        ///     Gets the data aggregator.
        /// </summary>
        /// <value>The data aggregator.</value>
        private IDataAggregator DataAggregator
        {
            get
            {
                // lazy initialization from the Spring Application Context
                if (null == _dataAggregator)
                {
                    _dataAggregator = ContextRegistry.GetContext().GetObject<IDataAggregator>("DataAggregator");

                    // And, if we were unable to get the DataAggregator Spring Bean, log an error
                    if (null == _dataAggregator)
                    {
                        Logger.Error("HmsOnsiteService.DataAggregator - cannot access DataAggregator Spring Bean");
                    }
                }

                return _dataAggregator;
            }
        }

        #region IHmsService Implementation

        /// <summary>
        ///     Reports EGM meters to the On-site Health Maintenance Service
        ///     (from an EGM on the casino floor).
        /// </summary>
        /// <param name="egmMeterData">The egm meter data.</param>
        public void ReportEgmMeters(EgmMeterData egmMeterData)
        {
            Logger.Debug($"HmsOnsiteService.ReportEgmMeters: [{egmMeterData}]");
            DataAggregator?.ReportEgmMeters(egmMeterData);
        }

        /// <summary>
        ///     Reports EGM events to the On-site Health Maintenance Service
        ///     (from an EGM on the casino floor).
        /// </summary>
        /// <param name="egmEventData">The egm event data.</param>
        public void ReportEgmEvents(EgmEventData egmEventData)
        {
            Logger.Debug($"HmsOnsiteService.ReportEgmEvents: [{egmEventData}]");
            DataAggregator?.ReportEgmEvents(egmEventData);
        }

        /// <summary>
        ///     Reports EGM metrics to the On-site Health Maintenance Service
        ///     (from an EGM on the casino floor).
        /// </summary>
        /// <param name="egmMetricData">The egm metric data.</param>
        public void ReportEgmMetrics(EgmMetricData egmMetricData)
        {
            Logger.Debug($"HmsOnsiteService.ReportEgmMetrics: [{egmMetricData}]");
            DataAggregator?.ReportEgmMetrics(egmMetricData);
        }

        /// <summary>
        ///     Reports the windows events.
        /// </summary>
        /// <param name="egmWindowsEventData">The egm windows event data.</param>
        public void ReportWindowsEvents(EgmWindowsEventData egmWindowsEventData)
        {
            Logger.Debug($"HmsOnsiteService.ReportWindowsEvents: [{egmWindowsEventData}]");
            DataAggregator?.ReportWindowsEvents(egmWindowsEventData);
        }

        /// <summary>
        /// Reports the egm versions.
        /// </summary>
        /// <param name="egmVersionData">The egm version data.</param>
        public void ReportEgmVersions(EgmVersionData egmVersionData)
        {
            Logger.Debug($"HmsOnsiteService.ReportEgmVersions: [{egmVersionData}]");
            DataAggregator?.ReportEgmVersions(egmVersionData);
        }

        #endregion

        #region IHmsFileUploadService Implementation

        /// <summary>
        ///     Uploads the passed in diagnostics data from the calling client.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>UploadResponseMessage.</returns>
        public UploadResponseMessage UploadDiagnosticsData(UploadRequestMessage request)
        {
            var rtnVal = new UploadResponseMessage
            {
                Success = false,
                Reason = "Unknown"
            };

            if (request == null)
            {
                Logger.Warn("No request message sent");
                rtnVal.Reason = "No request message";
                return rtnVal;
            }

            if (string.IsNullOrWhiteSpace(request.FileName) || request.DataStream == null)
            {
                Logger.Warn("Request message sent with invalid file name or data stream");
                rtnVal.Reason = "Invalid request message either file name or data is invalid";
                return rtnVal;
            }

            Logger.Info($"Received diagnostics upload request {request.FileName}");

            try
            {
                var diagnosticDir = Settings.Default.DiagnosticPackagePath;
                Directory.CreateDirectory(diagnosticDir);

                var pathToForFile = Path.Combine(diagnosticDir, request.FileName);
                byte[] buffer = new byte[BufferSize];

                using (var strm = new FileStream(pathToForFile, FileMode.Create, FileAccess.Write))
                {
                    var bytesRead = request.DataStream.Read(buffer, 0, BufferSize);
                    while (bytesRead > 0)
                    {
                        strm.Write(buffer, 0, bytesRead);
                        bytesRead = request.DataStream.Read(buffer, 0, BufferSize);
                    }
                    strm.Close();
                }

                rtnVal.Success = true;
                rtnVal.Reason = "Success";
            }
            catch (Exception exp)
            {
                Logger.Error($"Could not save off data stream for '{request.FileName}'", exp);
                rtnVal.Reason = exp.Message;
            }

            return rtnVal;
        }

        #endregion
    }
}