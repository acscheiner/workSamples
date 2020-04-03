// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 10-12-2016
//
// Last Modified By : acscheiner
// Last Modified On : 10-25-2016
// ***********************************************************************
// <copyright file="EgmMeterReadingConfiguration.cs" company="">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel.DataAccessLayer.Config
{
    #region

    using System.Data.Entity.ModelConfiguration;

    #endregion

    /// <summary>
    ///     Class EgmMeterReadingConfiguration.
    /// </summary>
    /// <seealso
    ///     cref="EgmMeterReading" />
    internal class EgmMeterReadingConfiguration : EntityTypeConfiguration<EgmMeterReading>
    {
        /// <summary>
        ///     The table name
        /// </summary>
        public const string TableName = "EgmMeterReading";

        /// <summary>
        ///     Initializes a new instance of the <see cref="EgmMeterReadingConfiguration" /> class.
        /// </summary>
        public EgmMeterReadingConfiguration()
        {
            ToTable(TableName);

            Property(mr => mr.Type).IsRequired();
            Property(mr => mr.Units).IsRequired();
            Property(mr => mr.EgmSerialNumber).IsRequired();
            Property(mr => mr.EgmAssetNumber).IsRequired();
            Property(mr => mr.CasinoCode).IsRequired();
            Property(mr => mr.GameTitle).IsRequired();
            Property(mr => mr.GameDenomination).IsRequired();
            Property(mr => mr.Value).IsRequired();
            Property(mr => mr.ReadAt).IsRequired();
            Property(mr => mr.SentAt).IsRequired();
            Property(mr => mr.ReportedAt).IsRequired();
            Property(mr => mr.ReportGuid).IsRequired();

            Property(mr => mr.Hash).IsOptional();

            Property(mr => mr.Version).IsRequired().IsConcurrencyToken();

            HasIndex(mr => mr.ReportedAt);
        }
    }
}