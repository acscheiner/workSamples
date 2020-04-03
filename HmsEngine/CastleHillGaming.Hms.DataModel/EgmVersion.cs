// Copyright (c) 2019 Castle Hill Gaming, LLC. All rights reserved.
namespace CastleHillGaming.Hms.DataModel
{ 
    using System;
    using System.Reflection;
    using CastleHillGaming.Hms.DataModel.DataAccessLayer.Dao;
    using CastleHillGaming.Hms.Interfaces;
    using log4net;

    /// <summary>
    /// Class EgmVersion.
    /// Implements the <see cref="CastleHillGaming.Hms.DataModel.HashedEntity" />
    /// Implements the <see cref="CastleHillGaming.Hms.Interfaces.IEgmData" />
    /// Implements the <see cref="CastleHillGaming.Hms.Interfaces.IVersionData" />
    /// </summary>
    /// <seealso cref="CastleHillGaming.Hms.DataModel.HashedEntity" />
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IEgmData" />
    /// <seealso cref="CastleHillGaming.Hms.Interfaces.IVersionData" />
    public class EgmVersion : HashedEntity, IEgmData, IVersionData
    {
        #region Private Static data

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EgmVersion"/> class.
        /// </summary>
        /// <param name="casinoCode">The casino code.</param>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="egmAssetNumber">The egm asset number.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="versionInfo">The version information.</param>
        /// <param name="reportedAt">The reported at.</param>
        public EgmVersion(string casinoCode, string egmSerialNumber, string egmAssetNumber, string objectName, string versionInfo, DateTime reportedAt)
        {
            if (string.IsNullOrWhiteSpace(egmSerialNumber))
            {
                Logger.WarnFormat("EgmVersion ctor received a null-valued or whitespace or empty value for {0}",
                    nameof(egmSerialNumber));
            }

            if (string.IsNullOrWhiteSpace(egmAssetNumber))
            {
                Logger.WarnFormat("EgmVersion ctor received a null-valued or whitespace or empty value for {0}",
                    nameof(egmAssetNumber));
            }

            if (string.IsNullOrWhiteSpace(casinoCode))
            {
                Logger.WarnFormat("EgmVersion ctor received a null-valued or whitespace or empty value for {0}",
                    nameof(casinoCode));
            }

            CasinoCode = !string.IsNullOrWhiteSpace(casinoCode) ? casinoCode : string.Empty;
            EgmSerialNumber = !string.IsNullOrWhiteSpace(egmSerialNumber) ? egmSerialNumber : string.Empty;
            EgmAssetNumber = !string.IsNullOrWhiteSpace(egmAssetNumber) ? egmAssetNumber : string.Empty;
            ReportedAt = reportedAt;
            ObjectName = objectName;
            VersionInfo = versionInfo;

            ReportGuid = Guid.Empty;
            SentAt = DaoUtilities.UnsentData;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EgmVersion"/> class.
        /// </summary>
        protected EgmVersion()
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the casino code.
        /// </summary>
        /// <value>The casino code.</value>
        public string CasinoCode { get; set; }
        /// <summary>
        /// Gets or sets the egm asset number.
        /// </summary>
        /// <value>The egm asset number.</value>
        public string EgmAssetNumber { get; set; }
        /// <summary>
        /// Gets or sets the egm serial number.
        /// </summary>
        /// <value>The egm serial number.</value>
        public string EgmSerialNumber { get; set; }
        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>The name of the object.</value>
        public string ObjectName { get; set; }
        /// <summary>
        /// Gets or sets the version information.
        /// </summary>
        /// <value>The version information.</value>
        public string VersionInfo { get; set; }
        /// <summary>
        /// Gets or sets the reported at.
        /// </summary>
        /// <value>The reported at.</value>
        public DateTime ReportedAt { get; set; }
        /// <summary>
        /// Gets or sets the report unique identifier.
        /// </summary>
        /// <value>The report unique identifier.</value>
        public Guid ReportGuid { get; set; }
        /// <summary>
        /// Gets or sets the DateTime the metric was sent.
        /// </summary>
        /// <value>The DateTime the metric was sent.</value>
        public DateTime SentAt { get; set; }
        #endregion
    }
}
