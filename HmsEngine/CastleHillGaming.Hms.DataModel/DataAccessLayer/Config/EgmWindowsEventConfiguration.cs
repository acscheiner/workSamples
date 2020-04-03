// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 04-26-2017
//
// Last Modified By : acscheiner
// Last Modified On : 04-26-2017
// ***********************************************************************
// <copyright file="EgmWindowsEventConfiguration.cs" company="Castle Hill Gaming, LLC">
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
    ///     Class EgmWindowsEventConfiguration.
    /// </summary>
    /// <seealso
    ///     cref="System.Data.Entity.ModelConfiguration.EntityTypeConfiguration{CastleHillGaming.Hms.DataModel.EgmWindowsEvent}" />
    internal class EgmWindowsEventConfiguration : EntityTypeConfiguration<EgmWindowsEvent>
    {
        /// <summary>
        ///     The table name
        /// </summary>
        public const string TableName = "EgmWindowsEvent";

        /// <summary>
        ///     Initializes a new instance of the <see cref="EgmWindowsEventConfiguration" /> class.
        /// </summary>
        public EgmWindowsEventConfiguration()
        {
            ToTable(TableName);

            Property(winEvt => winEvt.Code).IsRequired();
            Property(winEvt => winEvt.Description).IsRequired();
            Property(winEvt => winEvt.EgmSerialNumber).IsRequired();
            Property(winEvt => winEvt.EgmAssetNumber).IsRequired();
            Property(winEvt => winEvt.CasinoCode).IsRequired();
            Property(winEvt => winEvt.ReportGuid).IsRequired();
            Property(winEvt => winEvt.SentAt).IsRequired();
            Property(winEvt => winEvt.OccurredAt).IsRequired();
            Property(winEvt => winEvt.ReportedAt).IsRequired();
            Property(winEvt => winEvt.EventLogName).IsRequired();

            Property(winEvt => winEvt.Hash).IsOptional();

            Property(winEvt => winEvt.Version).IsRequired().IsConcurrencyToken();

            HasIndex(winEvt => winEvt.ReportedAt);
        }
    }
}