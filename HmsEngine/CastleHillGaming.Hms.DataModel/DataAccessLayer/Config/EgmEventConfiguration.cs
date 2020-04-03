// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 10-12-2016
//
// Last Modified By : acscheiner
// Last Modified On : 10-25-2016
// ***********************************************************************
// <copyright file="EgmEventConfiguration.cs" company="">
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
    ///     Class EgmEventConfiguration.
    /// </summary>
    /// <seealso
    ///     cref="EgmEvent" />
    internal class EgmEventConfiguration : EntityTypeConfiguration<EgmEvent>
    {
        /// <summary>
        ///     The table name
        /// </summary>
        public const string TableName = "EgmEvent";

        /// <summary>
        ///     Initializes a new instance of the <see cref="EgmEventConfiguration" /> class.
        /// </summary>
        public EgmEventConfiguration()
        {
            ToTable(TableName);

            Property(evt => evt.Code).IsRequired();
            Property(evt => evt.Description).IsRequired();
            Property(evt => evt.EgmSerialNumber).IsRequired();
            Property(evt => evt.EgmAssetNumber).IsRequired();
            Property(evt => evt.CasinoCode).IsRequired();
            Property(evt => evt.ReportGuid).IsRequired();
            Property(evt => evt.SentAt).IsRequired();
            Property(evt => evt.OccurredAt).IsRequired();
            Property(evt => evt.ReportedAt).IsRequired();

            Property(evt => evt.Hash).IsOptional();

            Property(evt => evt.Version).IsRequired().IsConcurrencyToken();

            HasIndex(evt => evt.ReportedAt);
        }
    }
}