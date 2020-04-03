// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 02-22-2017
//
// Last Modified By : acscheiner
// Last Modified On : 02-22-2017
// ***********************************************************************
// <copyright file="CompositeEgmMeterDataConfiguration.cs" company="Castle Hill Gaming, LLC">
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
    ///     Class CompositeEgmMeterDataConfiguration.
    /// </summary>
    /// <seealso
    ///     cref="System.Data.Entity.ModelConfiguration.EntityTypeConfiguration{CastleHillGaming.Hms.DataModel.CompositeEgmMeterData}" />
    internal class CompositeEgmMeterDataConfiguration : EntityTypeConfiguration<CompositeEgmMeterData>
    {
        /// <summary>
        ///     The table name
        /// </summary>
        public const string TableName = "CompositeEgmMeterData";

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompositeEgmMeterDataConfiguration" /> class.
        /// </summary>
        public CompositeEgmMeterDataConfiguration()
        {
            ToTable(TableName);

            Property(cemd => cemd.AuditDate).IsRequired();
            Property(cemd => cemd.CasinoName).IsRequired();
            Property(cemd => cemd.SerialNumber).IsRequired();
            Property(cemd => cemd.AssetNumber).IsRequired();
            Property(cemd => cemd.GameTheme).IsRequired();
            Property(cemd => cemd.Denomination).IsRequired();
            Property(cemd => cemd.IsActive).IsRequired();
            Property(cemd => cemd.Zone).IsRequired();
            Property(cemd => cemd.Bank).IsRequired();
            Property(cemd => cemd.Stand).IsRequired();
            Property(cemd => cemd.DateInPlace).IsRequired();
            Property(cemd => cemd.DaysOnFloor).IsRequired();
            Property(cemd => cemd.Par).IsRequired();
            Property(cemd => cemd.CoinIn).IsRequired();
            Property(cemd => cemd.CoinOut).IsRequired();
            Property(cemd => cemd.BillDrop).IsRequired();
            Property(cemd => cemd.TicketDrop).IsRequired();
            Property(cemd => cemd.TicketOut).IsRequired();
            Property(cemd => cemd.Handpay).IsRequired();
            Property(cemd => cemd.Jackpot).IsRequired();
            Property(cemd => cemd.MeteredAttendantPaidProgressive).IsRequired();
            Property(cemd => cemd.MeteredMachinePaidProgressive).IsRequired();
            Property(cemd => cemd.GamesPlayed).IsRequired();
            Property(cemd => cemd.GamesWon).IsRequired();
            Property(cemd => cemd.GamesLost).IsRequired();
            Property(cemd => cemd.AverageBet).IsRequired();
            Property(cemd => cemd.CoinInPerUnit).IsRequired();
            Property(cemd => cemd.WinPerUnit).IsRequired();
            Property(cemd => cemd.SlotRevenue).IsRequired();
            Property(cemd => cemd.FreePlay).IsRequired();
            Property(cemd => cemd.TotalNetWin).IsRequired();
            Property(cemd => cemd.LessorPercent).IsRequired();
            Property(cemd => cemd.LessorFee).IsRequired();

            Property(cemd => cemd.Version).IsRequired().IsConcurrencyToken();

            HasIndex(cemd => new
            {
                cemd.AuditDate,
                cemd.CasinoName,
                cemd.SerialNumber,
                cemd.AssetNumber,
                cemd.GameTheme,
                cemd.Denomination
            });
            HasIndex(cemd => cemd.AuditDate);
        }
    }
}