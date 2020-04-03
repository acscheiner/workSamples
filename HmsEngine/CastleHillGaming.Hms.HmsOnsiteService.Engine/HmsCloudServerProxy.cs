// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.HmsOnsiteService.Engine
// Author           : acscheiner
// Created          : 11-02-2016
//
// Last Modified By : acscheiner
// Last Modified On : 11-02-2016
// ***********************************************************************
// <copyright file="HmsCloudServerProxy.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC. Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.HmsOnsiteService.Engine
{
    #region

    using System.ServiceModel;
    using log4net;
    using Contracts;
    using System.Reflection;

    #endregion

    /// <summary>
    ///     Class HmsCloudServerProxy.
    /// </summary>
    /// <seealso cref="System.ServiceModel.ClientBase{CastleHillGaming.Hms.Contracts.IHmsCloudService}" />
    /// <seealso cref="CastleHillGaming.Hms.Contracts.IHmsCloudService" />
    public class HmsCloudServerProxy : ClientBase<IHmsCloudService>, IHmsCloudService
    {
        #region Private static member data

        /// <summary>
        ///     The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Interface implementations

        /// <summary>
        ///     Reports the casino data.
        /// </summary>
        /// <param name="casinoDataReport">The casino data report.</param>
        public void ReportCasinoData(CasinoDataReport casinoDataReport)
        {
            Logger.Debug($"On-Site HMS service sending Casino Data Report: [{casinoDataReport}] to HMS Cloud Service");
            Channel.ReportCasinoData(casinoDataReport);
        }

        /// <summary>
        ///     Reports the data backup.
        /// </summary>
        /// <param name="casinoDataBackup">The casino data backup.</param>
        public void ReportDataBackup(CasinoDataBackup casinoDataBackup)
        {
            Logger.Debug($"On-Site HMS service sending Casino Data Backup: [{casinoDataBackup}] to HMS Cloud Service");
            Channel.ReportDataBackup(casinoDataBackup);
        }

        /// <summary>
        ///     Reports the casino diagnostics.
        /// </summary>
        /// <param name="casinoDiagnosticData">The casino diagnostic data.</param>
        public void ReportCasinoDiagnostics(CasinoDiagnosticData casinoDiagnosticData)
        {
            Logger.Debug(
                $"On-Site HMS service sending Casino Diagnositcs Data: [{casinoDiagnosticData}] to HMS Cloud Service");
            Channel.ReportCasinoDiagnostics(casinoDiagnosticData);
        }

        #endregion
    }
}