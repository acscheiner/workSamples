// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 10-12-2016
//
// Last Modified By : acscheiner
// Last Modified On : 10-14-2016
// ***********************************************************************
// <copyright file="EgmEvent.cs" company="Castle Hill Gaming, LLC">
//     Copyright ©  2016
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
    /// Class EgmEvent.
    /// </summary>
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IEgmData" />
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IEventData" />
    public class EgmEvent : HashedEntity, IEgmData, IEventData
    {
        #region Private Static data

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event code.
        /// </summary>
        /// <value>The event code.</value>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the event description.
        /// </summary>
        /// <value>The event description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the DateTime that the event occurred.
        /// </summary>
        /// <value>The DateTime that the event occurred.</value>
        public DateTime OccurredAt { get; set; }

        /// <summary>
        /// Gets or sets the DateTime that the event was sent.
        /// </summary>
        /// <value>The DateTime that the event was sent.</value>
        [Browsable(false)]
        public DateTime SentAt { get; set; }

        /// <summary>
        /// Gets or sets the casino code for the casino to which this ICasinoMember belongs.
        /// </summary>
        /// <value>The casino code for the casino to which this ICasinoMember belongs.</value>
        [Browsable(false)]
        public string CasinoCode { get; set; }

        /// <summary>
        /// Gets or sets the casino report GUID in which this ICasinoMember was sent to the
        /// Cloud-Hosted HMS service.
        /// </summary>
        /// <value>
        /// The casino report GUID in which this ICasinoMember was sent to the
        /// Cloud-Hosted HMS service.
        /// </value>
        [Browsable(false)]
        public Guid ReportGuid { get; set; }

        /// <summary>
        /// Gets or sets the egm serial number.
        /// </summary>
        /// <value>The egm serial number.</value>
        public string EgmSerialNumber { get; set; }

        /// <summary>
        /// Gets or sets DateTime this IEgmData was reported.
        /// </summary>
        /// <value>The DateTime this IEgmData was reported.</value>
        [Browsable(false)]
        public DateTime ReportedAt { get; set; }

        /// <summary>
        /// Gets or sets the egm asset number.
        /// </summary>
        /// <value>The egm asset number.</value>
        [Browsable(false)]
        public string EgmAssetNumber { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EgmEvent" /> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="description">The description.</param>
        /// <param name="occurredAt">The DateTime this event occurred.</param>
        /// <param name="casinoCode">
        /// casino code for the casino to which this ICasinoMember belongs; if null or empty, use
        /// (default) casinoCode application property for this EgmMeterReading
        /// </param>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="egmAssetNumber">The egm asset number.</param>
        /// <param name="reportedAt">The DateTime this EgmEvent was reported.</param>
        public EgmEvent(int code, string description, DateTime occurredAt, string casinoCode, string egmSerialNumber,
            string egmAssetNumber, DateTime reportedAt)
        {
            if (string.IsNullOrWhiteSpace(egmSerialNumber))
            {
                Logger.Warn(
                    $"EgmEvent ctor received a null-valued or whitespace or empty value for {nameof(egmSerialNumber)}");
            }

            if (string.IsNullOrWhiteSpace(egmAssetNumber))
            {
                Logger.Warn(
                    $"EgmEvent ctor received a null-valued or whitespace or empty value for {nameof(egmAssetNumber)}");
            }

            if (string.IsNullOrWhiteSpace(casinoCode))
            {
                Logger.Warn(
                    $"EgmEvent ctor received a null-valued or whitespace or empty value for {nameof(casinoCode)}");
            }

            CasinoCode = !string.IsNullOrWhiteSpace(casinoCode) ? casinoCode : string.Empty;
            EgmSerialNumber = !string.IsNullOrWhiteSpace(egmSerialNumber) ? egmSerialNumber : string.Empty;
            EgmAssetNumber = !string.IsNullOrWhiteSpace(egmAssetNumber) ? egmAssetNumber : string.Empty;
            Code = code;
            Description = !string.IsNullOrWhiteSpace(description) ? description : string.Empty;
            OccurredAt = occurredAt;
            ReportedAt = reportedAt;

            ReportGuid = Guid.Empty;
            SentAt = DaoUtilities.UnsentData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EgmEvent" /> class.
        /// Used by Entity Framework when reconstituting a database record
        /// into an in-memory EgmEvent instance.
        /// </summary>
        protected EgmEvent()
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
                $"{nameof(Id)}: {Id}, {nameof(Version)}: {Version}, {nameof(Hash)}: {Hash}, {nameof(Code)}: {Code}, {nameof(Description)}: {Description}, {nameof(OccurredAt)}: {OccurredAt}, {nameof(SentAt)}: {SentAt}, {nameof(CasinoCode)}: {CasinoCode}, {nameof(ReportGuid)}: {ReportGuid}, {nameof(EgmSerialNumber)}: {EgmSerialNumber}, {nameof(ReportedAt)}: {ReportedAt}, {nameof(EgmAssetNumber)}: {EgmAssetNumber}";
        }
    }
}