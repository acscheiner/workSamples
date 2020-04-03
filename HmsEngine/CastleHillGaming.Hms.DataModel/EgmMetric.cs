// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 03-29-2017
//
// Last Modified By : acscheiner
// Last Modified On : 03-29-2017
// ***********************************************************************
// <copyright file="EgmMetric.cs" company="Castle Hill Gaming, LLC">
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
    /// Class EgmMetric.
    /// </summary>
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IEgmData" />
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IMetricData" />
    public class EgmMetric : HashedEntity, IEgmData, IMetricData
    {
        #region Private Static data

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the DateTime the metric was sent.
        /// </summary>
        /// <value>The DateTime the metric was sent.</value>
        [Browsable(false)]
        public DateTime SentAt { get; set; }

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
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public MetricType Type { get; set; }

        /// <summary>
        /// Gets or sets the type of the dynamic metric.
        /// </summary>
        /// <value>The type of the dynamic metric.</value>
        public string DynamicMetricType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public float Value { get; set; }

        /// <summary>
        /// Gets or sets the read at.
        /// </summary>
        /// <value>The read at.</value>
        public DateTime ReadAt { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EgmMetric" /> class.
        /// </summary>
        /// <param name="casinoCode">The casino code.</param>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="egmAssetNumber">The egm asset number.</param>
        /// <param name="reportedAt">The reported at.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <param name="readAt">The read at.</param>
        public EgmMetric(string casinoCode, string egmSerialNumber, string egmAssetNumber, DateTime reportedAt,
            string type, float value, DateTime readAt)
        {
            if (string.IsNullOrWhiteSpace(egmSerialNumber))
            {
                Logger.WarnFormat("EgmMetric ctor received a null-valued or whitespace or empty value for {0}",
                    nameof(egmSerialNumber));
            }

            if (string.IsNullOrWhiteSpace(egmAssetNumber))
            {
                Logger.WarnFormat("EgmMetric ctor received a null-valued or whitespace or empty value for {0}",
                    nameof(egmAssetNumber));
            }

            if (string.IsNullOrWhiteSpace(casinoCode))
            {
                Logger.WarnFormat("EgmMetric ctor received a null-valued or whitespace or empty value for {0}",
                    nameof(casinoCode));
            }

            CasinoCode = !string.IsNullOrWhiteSpace(casinoCode) ? casinoCode : string.Empty;
            EgmSerialNumber = !string.IsNullOrWhiteSpace(egmSerialNumber) ? egmSerialNumber : string.Empty;
            EgmAssetNumber = !string.IsNullOrWhiteSpace(egmAssetNumber) ? egmAssetNumber : string.Empty;
            ReportedAt = reportedAt;
            DynamicMetricType = type;
            Value = value;
            ReadAt = readAt;

            ReportGuid = Guid.Empty;
            SentAt = DaoUtilities.UnsentData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EgmMetric" /> class.
        /// Used by Entity Framework when reconstituting a database record
        /// into an in-memory EgmMetric instance.
        /// </summary>
        protected EgmMetric()
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
                $"{nameof(Id)}: {Id}, {nameof(Version)}: {Version}, {nameof(Hash)}: {Hash}, {nameof(SentAt)}: {SentAt}, {nameof(CasinoCode)}: {CasinoCode}, {nameof(ReportGuid)}: {ReportGuid}, {nameof(ReportedAt)}: {ReportedAt}, {nameof(EgmSerialNumber)}: {EgmSerialNumber}, {nameof(EgmAssetNumber)}: {EgmAssetNumber}, {nameof(Type)}: {Type}, {nameof(Value)}: {Value}, {nameof(ReadAt)}: {ReadAt}";
        }
    }
}