// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 03-29-2017
//
// Last Modified By : acscheiner
// Last Modified On : 03-29-2017
// ***********************************************************************
// <copyright file="EgmMetricConfiguration.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel.DataAccessLayer.Config
{
    #region

    using System.Data.Entity.ModelConfiguration;

    #endregion

    /// <summary>
    ///     Class EgmMetricConfiguration.
    /// </summary>
    /// <seealso cref="System.Data.Entity.ModelConfiguration.EntityTypeConfiguration{CastleHillGaming.Hms.DataModel.EgmMetric}" />
    internal class EgmMetricConfiguration : EntityTypeConfiguration<EgmMetric>
    {
        /// <summary>
        ///     The table name
        /// </summary>
        public const string TableName = "EgmMetric";

        /// <summary>
        ///     Initializes a new instance of the <see cref="EgmMetricConfiguration" /> class.
        /// </summary>
        public EgmMetricConfiguration()
        {
            ToTable(TableName);

            Ignore(metric => metric.Type);
            Property(metric => metric.DynamicMetricType).HasColumnName("Type").IsRequired();
            Property(metric => metric.EgmSerialNumber).IsRequired();
            Property(metric => metric.EgmAssetNumber).IsRequired();
            Property(metric => metric.CasinoCode).IsRequired();
            Property(metric => metric.Value).IsRequired();
            Property(metric => metric.ReadAt).IsRequired();
            Property(metric => metric.SentAt).IsRequired();
            Property(metric => metric.ReportedAt).IsRequired();
            Property(metric => metric.ReportGuid).IsRequired();

            Property(metric => metric.Hash).IsOptional();

            Property(metric => metric.Version).IsRequired().IsConcurrencyToken();

            HasIndex(metric => metric.ReportedAt);
        }
    }
}