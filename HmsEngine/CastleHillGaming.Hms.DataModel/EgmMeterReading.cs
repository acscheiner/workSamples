// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 10-11-2016
//
// Last Modified By : acscheiner
// Last Modified On : 10-14-2016
// ***********************************************************************
// <copyright file="EgmMeterReading.cs" company="Castle Hill Gaming, LLC">
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
    using CastleHillGaming.Hms.Contracts;
    using CastleHillGaming.Hms.DataModel.DataAccessLayer.Dao;
    using CastleHillGaming.Hms.Interfaces;
    using log4net;

    #endregion

    /// <summary>
    /// Class EgmMeterReading.
    /// </summary>
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IEgmData" />
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IMeterData" />
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.ICasinoMember" />
    public class EgmMeterReading : HashedEntity, IEgmData, IMeterData
    {
        #region Private Static data

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the game title.
        /// </summary>
        /// <value>The game title.</value>
        public string GameTitle { get; set; }

        /// <summary>
        /// Gets or sets the game denomination.
        /// </summary>
        /// <value>The game denomination.</value>
        public long GameDenomination { get; set; }

        /// <summary>
        /// Gets or sets the meter type.
        /// </summary>
        /// <value>The meter type.</value>
        public MeterType Type { get; set; }

        /// <summary>
        /// Gets or sets the meter value.
        /// </summary>
        /// <value>The meter value.</value>
        public long Value { get; set; }

        /// <summary>
        /// Gets or sets the meter units.
        /// </summary>
        /// <value>The meter units.</value>
        public string Units { get; set; }

        /// <summary>
        /// Gets or sets the DateTime the meter was read.
        /// </summary>
        /// <value>The DateTime the meter was read.</value>
        public DateTime ReadAt { get; set; }

        /// <summary>
        /// Gets or sets the DateTime the meter reading was sent.
        /// </summary>
        /// <value>The DateTime the meter reading was sent.</value>
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
        /// Initializes a new instance of the <see cref="EgmMeterReading" /> class.
        /// </summary>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="egmAssetNumber">The egm asset number.</param>
        /// <param name="reportedAt">The datetime at which this EgmMeterReading was reported.</param>
        /// <param name="type">The meter type.</param>
        /// <param name="value">The meter value.</param>
        /// <param name="units">The meter units.</param>
        /// <param name="readAt">The DateTime the meter was read.</param>
        /// <param name="casinoCode">
        /// casino code for the casino to which this ICasinoMember belongs; if null or empty, use
        /// (default) casinoCode application property for this EgmMeterReading
        /// </param>
        /// <param name="gameTitle">The game title.</param>
        /// <param name="gameDenomination">The game denomination.</param>
        public EgmMeterReading(string egmSerialNumber, string egmAssetNumber, DateTime reportedAt, MeterType type,
            long value, string units, DateTime readAt, string casinoCode, string gameTitle, long gameDenomination)
        {
            if (string.IsNullOrWhiteSpace(egmSerialNumber))
            {
                Logger.Warn(
                    $"EgmMeterReading ctor received a null-valued or whitespace or empty value for {nameof(egmSerialNumber)}");
            }

            if (string.IsNullOrWhiteSpace(egmAssetNumber))
            {
                Logger.Warn(
                    $"EgmMeterReading ctor received a null-valued or whitespace or empty value for {nameof(egmAssetNumber)}");
            }

            if (string.IsNullOrWhiteSpace(casinoCode))
            {
                Logger.Warn(
                    $"EgmMeterReading ctor received a null-valued or whitespace or empty value for {nameof(casinoCode)}");
            }

            if (string.IsNullOrWhiteSpace(units))
            {
                Logger.Warn(
                    $"EgmMeterReading ctor received a null-valued or whitespace or empty value for {nameof(units)}");
            }

            if (string.IsNullOrWhiteSpace(gameTitle))
            {
                GameTitle = MeterData.NoGameTitle;
                GameDenomination = MeterData.NoGameDenomination;
            }
            else
            {
                GameTitle = gameTitle;
                GameDenomination = gameDenomination;
            }

            CasinoCode = !string.IsNullOrWhiteSpace(casinoCode) ? casinoCode : string.Empty;
            EgmSerialNumber = !string.IsNullOrWhiteSpace(egmSerialNumber) ? egmSerialNumber : string.Empty;
            EgmAssetNumber = !string.IsNullOrWhiteSpace(egmAssetNumber) ? egmAssetNumber : string.Empty;
            ReportedAt = reportedAt;
            Type = type;
            Value = value;
            Units = !string.IsNullOrWhiteSpace(units) ? units : string.Empty;
            ReadAt = readAt;

            ReportGuid = Guid.Empty;
            SentAt = DaoUtilities.UnsentData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EgmMeterReading" /> class.
        /// Used by Entity Framework when reconstituting a database record
        /// into an in-memory EgmMeterReading instance.
        /// </summary>
        protected EgmMeterReading()
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
                $"{nameof(Id)}: {Id}, {nameof(Version)}: {Version}, {nameof(Hash)}: {Hash}, {nameof(GameTitle)}: {GameTitle}, {nameof(GameDenomination)}: {GameDenomination}, {nameof(Type)}: {Type}, {nameof(Value)}: {Value}, {nameof(Units)}: {Units}, {nameof(ReadAt)}: {ReadAt}, {nameof(SentAt)}: {SentAt}, {nameof(CasinoCode)}: {CasinoCode}, {nameof(ReportGuid)}: {ReportGuid}, {nameof(EgmSerialNumber)}: {EgmSerialNumber}, {nameof(ReportedAt)}: {ReportedAt}, {nameof(EgmAssetNumber)}: {EgmAssetNumber}";
        }
    }
}