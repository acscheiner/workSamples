// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 04-24-2017
//
// Last Modified By : acscheiner
// Last Modified On : 04-24-2017
// ***********************************************************************
// <copyright file="EgmWindowsEvent.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel
{
    #region

    using System;
    using System.ComponentModel;
    using System.Reflection;
    using CastleHillGaming.Hms.DataModel.DataAccessLayer.Dao;
    using CastleHillGaming.Hms.Interfaces;
    using log4net;

    #endregion

    /// <summary>
    /// Class EgmWindowsEvent.
    /// </summary>
    /// <seealso cref="CastleHillGaming.Hms.DataModel.HashedEntity" />
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IEgmData" />
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IWindowsEventData" />
    public class EgmWindowsEvent : HashedEntity, IEgmData, IWindowsEventData
    {
        #region Private Static data

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the casino code.
        /// </summary>
        /// <value>The casino code.</value>
        [Browsable(false)]
        public string CasinoCode { get; set; }

        /// <summary>
        /// Gets or sets the report unique identifier.
        /// </summary>
        /// <value>The report unique identifier.</value>
        [Browsable(false)]
        public Guid ReportGuid { get; set; }

        /// <summary>
        /// Gets or sets the reported at.
        /// </summary>
        /// <value>The reported at.</value>
        [Browsable(false)]
        public DateTime ReportedAt { get; set; }

        /// <summary>
        /// Gets or sets the egm serial number.
        /// </summary>
        /// <value>The egm serial number.</value>
        public string EgmSerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the egm asset number.
        /// </summary>
        /// <value>The egm asset number.</value>
        [Browsable(false)]
        public string EgmAssetNumber { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the occurred at.
        /// </summary>
        /// <value>The occurred at.</value>
        public DateTime OccurredAt { get; set; }

        /// <summary>
        /// Gets or sets the name of the event log.
        /// </summary>
        /// <value>The name of the event log.</value>
        [Browsable(false)]
        public string EventLogName { get; set; }

        /// <summary>
        /// Gets or sets the DateTime that the windows event was sent.
        /// </summary>
        /// <value>The DateTime that the windows event was sent.</value>
        [Browsable(false)]
        public DateTime SentAt { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EgmWindowsEvent" /> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="description">The description.</param>
        /// <param name="eventLogName">Name of the event log.</param>
        /// <param name="occurredAt">The occurred at.</param>
        /// <param name="casinoCode">The casino code.</param>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="egmAssetNumber">The egm asset number.</param>
        /// <param name="reportedAt">The reported at.</param>
        public EgmWindowsEvent(int code, string description, string eventLogName, DateTime occurredAt,
            string casinoCode,
            string egmSerialNumber, string egmAssetNumber, DateTime reportedAt)
        {
            if (string.IsNullOrWhiteSpace(egmSerialNumber))
            {
                Logger.Warn(
                    $"EgmWindowsEvent ctor received a null-valued or whitespace or empty value for {nameof(egmSerialNumber)}");
            }

            if (string.IsNullOrWhiteSpace(egmAssetNumber))
            {
                Logger.Warn(
                    $"EgmWindowsEvent ctor received a null-valued or whitespace or empty value for {nameof(egmAssetNumber)}");
            }

            if (string.IsNullOrWhiteSpace(casinoCode))
            {
                Logger.Warn(
                    $"EgmWindowsEvent ctor received a null-valued or whitespace or empty value for {nameof(casinoCode)}");
            }

            CasinoCode = !string.IsNullOrWhiteSpace(casinoCode) ? casinoCode : string.Empty;
            EgmSerialNumber = !string.IsNullOrWhiteSpace(egmSerialNumber) ? egmSerialNumber : string.Empty;
            EgmAssetNumber = !string.IsNullOrWhiteSpace(egmAssetNumber) ? egmAssetNumber : string.Empty;
            Code = code;
            Description = !string.IsNullOrWhiteSpace(description) ? description : string.Empty;
            OccurredAt = occurredAt;
            ReportedAt = reportedAt;
            EventLogName = !string.IsNullOrWhiteSpace(eventLogName) ? eventLogName : string.Empty;

            ReportGuid = Guid.Empty;
            SentAt = DaoUtilities.UnsentData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EgmWindowsEvent" /> class.
        /// Used by Entity Framework when reconstituting a database record
        /// into an in-memory EgmWindowsEvent instance.
        /// </summary>
        protected EgmWindowsEvent()
        {
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return
                $"{nameof(CasinoCode)}: {CasinoCode}, {nameof(ReportGuid)}: {ReportGuid}, {nameof(ReportedAt)}: {ReportedAt}, {nameof(EgmSerialNumber)}: {EgmSerialNumber}, {nameof(EgmAssetNumber)}: {EgmAssetNumber}, {nameof(Code)}: {Code}, {nameof(Description)}: {Description}, {nameof(OccurredAt)}: {OccurredAt}, {nameof(EventLogName)}: {EventLogName}, {nameof(SentAt)}: {SentAt}";
        }
    }
}