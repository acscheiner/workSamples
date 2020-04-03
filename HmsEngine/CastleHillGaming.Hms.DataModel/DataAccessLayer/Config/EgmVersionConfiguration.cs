// Copyright (c) 2019 Castle Hill Gaming, LLC. All rights reserved.
namespace CastleHillGaming.Hms.DataModel.DataAccessLayer.Config
{
    #region

    using System.Data.Entity.ModelConfiguration;

    #endregion

    /// <summary>
    /// Class EgmVersionConfiguration.
    /// </summary>
    /// <seealso cref="System.Data.Entity.ModelConfiguration.EntityTypeConfiguration{CastleHillGaming.Hms.DataModel.EgmVersion}" />
    internal class EgmVersionConfiguration : EntityTypeConfiguration<EgmVersion>
    {
        /// <summary>
        /// The table name
        /// </summary>
        public const string TableName = "EgmVersion";

        /// <summary>
        /// Initializes a new instance of the <see cref="EgmVersionConfiguration" /> class.
        /// </summary>
        public EgmVersionConfiguration()
        {
            ToTable(TableName);

            Property(v => v.ObjectName).IsRequired();
            Property(v => v.VersionInfo).IsRequired();
            Property(v => v.EgmSerialNumber).IsRequired();
            Property(v => v.EgmAssetNumber).IsRequired();
            Property(v => v.CasinoCode).IsRequired();
            Property(v => v.ReportGuid).IsRequired();
            Property(v => v.SentAt).IsRequired();
            Property(v => v.ReportedAt).IsRequired();

            Property(v => v.Hash).IsOptional();

            Property(v => v.Version).IsRequired().IsConcurrencyToken();

            HasIndex(v => v.ReportedAt);
        }
    }
}