// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 02-21-2017
//
// Last Modified By : acscheiner
// Last Modified On : 02-21-2017
// ***********************************************************************
// <copyright file="CompositeEgmMeterData.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel
{
    #region

    using System;
    using System.Reflection;
    using CastleHillGaming.Hms.Contracts;
    using CastleHillGaming.Hms.Interfaces;
    using log4net;

    #endregion

    /// <summary>
    /// Class CompositeEgmMeterData.
    /// </summary>
    public class CompositeEgmMeterData : EntityBase, ICompositeEgmMeterData
    {
        #region Private Static data

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Constants

        public const long MeterNotRecordedL = -1L;
        public const int MeterNotRecordedS = -1;
        public const decimal MeterNotRecordedD = -1;

        public const int StandUnknown = -1;
        public const int DaysOnFloorUnknown = -1;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the audit date.
        /// </summary>
        /// <value>The audit date.</value>
        public DateTime AuditDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the casino.
        /// </summary>
        /// <value>The name of the casino.</value>
        public string CasinoName { get; set; }

        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        /// <value>The serial number.</value>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the asset number.
        /// </summary>
        /// <value>The asset number.</value>
        public string AssetNumber { get; set; }

        /// <summary>
        /// Gets or sets the game theme.
        /// </summary>
        /// <value>The game theme.</value>
        public string GameTheme { get; set; }

        /// <summary>
        /// Gets or sets the denomination.
        /// </summary>
        /// <value>The denomination.</value>
        public long Denomination { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the zone.
        /// </summary>
        /// <value>The zone.</value>
        public string Zone { get; set; }

        /// <summary>
        /// Gets or sets the bank.
        /// </summary>
        /// <value>The bank.</value>
        public string Bank { get; set; }

        /// <summary>
        /// Gets or sets the stand.
        /// </summary>
        /// <value>The stand.</value>
        public int Stand { get; set; }

        /// <summary>
        /// Gets or sets the date in place.
        /// </summary>
        /// <value>The date in place.</value>
        public DateTime DateInPlace { get; set; }

        /// <summary>
        /// Gets or sets the days on floor.
        /// </summary>
        /// <value>The days on floor.</value>
        public int DaysOnFloor { get; set; }

        /// <summary>
        /// Gets or sets the par.
        /// </summary>
        /// <value>The par.</value>
        public string Par { get; set; }

        /// <summary>
        /// Gets or sets the coin in.
        /// </summary>
        /// <value>The coin in.</value>
        public long CoinIn { get; set; }

        /// <summary>
        /// Gets or sets the coin out.
        /// </summary>
        /// <value>The coin out.</value>
        public long CoinOut { get; set; }

        /// <summary>
        /// Gets or sets the bill drop.
        /// </summary>
        /// <value>The bill drop.</value>
        public long BillDrop { get; set; }

        /// <summary>
        /// Gets or sets the ticket drop.
        /// </summary>
        /// <value>The ticket drop.</value>
        public long TicketDrop { get; set; }

        /// <summary>
        /// Gets or sets the ticket out.
        /// </summary>
        /// <value>The ticket out.</value>
        public long TicketOut { get; set; }

        /// <summary>
        /// Gets or sets the handpay.
        /// </summary>
        /// <value>The handpay.</value>
        public long Handpay { get; set; }

        /// <summary>
        /// Gets or sets the jackpot.
        /// </summary>
        /// <value>The jackpot.</value>
        public long Jackpot { get; set; }

        /// <summary>
        /// Gets or sets the metered attendant paid progressive.
        /// </summary>
        /// <value>The metered attendant paid progressive.</value>
        public long MeteredAttendantPaidProgressive { get; set; }

        /// <summary>
        /// Gets or sets the metered machine paid progressive.
        /// </summary>
        /// <value>The metered machine paid progressive.</value>
        public long MeteredMachinePaidProgressive { get; set; }

        /// <summary>
        /// Gets or sets the games played.
        /// </summary>
        /// <value>The games played.</value>
        public long GamesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the games won.
        /// </summary>
        /// <value>The games won.</value>
        public long GamesWon { get; set; }

        /// <summary>
        /// Gets or sets the games lost.
        /// </summary>
        /// <value>The games lost.</value>
        public long GamesLost { get; set; }

        /// <summary>
        /// Gets or sets the average bet.
        /// </summary>
        /// <value>The average bet.</value>
        public decimal AverageBet { get; set; }

        /// <summary>
        /// Gets or sets the coin in per unit.
        /// </summary>
        /// <value>The coin in per unit.</value>
        public decimal CoinInPerUnit { get; set; }

        /// <summary>
        /// Gets or sets the win per unit.
        /// </summary>
        /// <value>The win per unit.</value>
        public decimal WinPerUnit { get; set; }

        /// <summary>
        /// Gets or sets the slot revenue.
        /// </summary>
        /// <value>The slot revenue.</value>
        public long SlotRevenue { get; set; }

        /// <summary>
        /// Gets or sets the free play.
        /// </summary>
        /// <value>The free play.</value>
        public long FreePlay { get; set; }

        /// <summary>
        /// Gets or sets the total net win.
        /// </summary>
        /// <value>The total net win.</value>
        public long TotalNetWin { get; set; }

        /// <summary>
        /// Gets or sets the lessor percent.
        /// </summary>
        /// <value>The lessor percent.</value>
        public decimal LessorPercent { get; set; }

        /// <summary>
        /// Gets or sets the lessor fee.
        /// </summary>
        /// <value>The lessor fee.</value>
        public decimal LessorFee { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeEgmMeterData" /> class.
        /// </summary>
        /// <param name="auditDate">The audit date.</param>
        /// <param name="casinoName">Name of the casino.</param>
        /// <param name="serialNumber">The serial number.</param>
        /// <param name="assetNumber">The asset number.</param>
        /// <param name="gameTheme">The game theme.</param>
        /// <param name="denomination">The denomination.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="zone">The zone.</param>
        /// <param name="bank">The bank.</param>
        /// <param name="stand">The stand.</param>
        /// <param name="dateInPlace">The date in place.</param>
        /// <param name="daysOnFloor">The days on floor.</param>
        /// <param name="par">The par.</param>
        /// <param name="coinIn">The coin in.</param>
        /// <param name="coinOut">The coin out.</param>
        /// <param name="billDrop">The bill drop.</param>
        /// <param name="ticketDrop">The ticket drop.</param>
        /// <param name="ticketOut">The ticket out.</param>
        /// <param name="handpay">The handpay.</param>
        /// <param name="jackpot">The jackpot.</param>
        /// <param name="meteredAttendantPaidProgressive">The metered attendant paid progressive.</param>
        /// <param name="meteredMachinePaidProgressive">The metered machine paid progressive.</param>
        /// <param name="gamesPlayed">The games played.</param>
        /// <param name="gamesWon">The games won.</param>
        /// <param name="gamesLost">The games lost.</param>
        /// <param name="averageBet">The average bet.</param>
        /// <param name="coinInPerUnit">The coin in per unit.</param>
        /// <param name="winPerUnit">The win per unit.</param>
        /// <param name="slotRevenue">The slot revenue.</param>
        /// <param name="freePlay">The free play.</param>
        /// <param name="totalNetWin">The total net win.</param>
        /// <param name="lessorPercent">The lessor percent.</param>
        /// <param name="lessorFee">The lessor fee.</param>
        public CompositeEgmMeterData(
            DateTime auditDate
            , string casinoName
            , string serialNumber
            , string assetNumber
            , string gameTheme = MeterData.NoGameTitle
            , long denomination = MeterData.NoGameDenomination
            , string zone = null
            , string bank = null
            , int stand = StandUnknown
            , bool isActive = true
            , DateTime? dateInPlace = null
            , int daysOnFloor = DaysOnFloorUnknown
            , string par = null
            , long coinIn = MeterNotRecordedL
            , long coinOut = MeterNotRecordedL
            , long billDrop = MeterNotRecordedL
            , long ticketDrop = MeterNotRecordedL
            , long ticketOut = MeterNotRecordedL
            , long handpay = MeterNotRecordedL
            , long jackpot = MeterNotRecordedL
            , long meteredAttendantPaidProgressive = MeterNotRecordedL
            , long meteredMachinePaidProgressive = MeterNotRecordedL
            , long gamesPlayed = MeterNotRecordedL
            , long gamesWon = MeterNotRecordedL
            , long gamesLost = MeterNotRecordedL
            , decimal averageBet = MeterNotRecordedD
            , decimal coinInPerUnit = MeterNotRecordedD
            , decimal winPerUnit = MeterNotRecordedD
            , long slotRevenue = MeterNotRecordedL
            , long freePlay = MeterNotRecordedL
            , long totalNetWin = MeterNotRecordedL
            , decimal lessorPercent = MeterNotRecordedD
            , decimal lessorFee = MeterNotRecordedD
        )
        {
            if (string.IsNullOrWhiteSpace(casinoName))
            {
                Logger.WarnFormat(
                    "CompositeEgmMeterData ctor received a null-valued or whitespace or empty value for {0}",
                    nameof(casinoName));
            }

            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                Logger.WarnFormat(
                    "CompositeEgmMeterData ctor received a null-valued or whitespace or empty value for {0}",
                    nameof(serialNumber));
            }

            if (string.IsNullOrWhiteSpace(assetNumber))
            {
                Logger.WarnFormat(
                    "CompositeEgmMeterData ctor received a null-valued or whitespace or empty value for {0}",
                    nameof(assetNumber));
            }

            AuditDate = auditDate;
            CasinoName = !string.IsNullOrWhiteSpace(casinoName) ? casinoName : string.Empty;
            SerialNumber = !string.IsNullOrWhiteSpace(serialNumber) ? serialNumber : string.Empty;
            AssetNumber = !string.IsNullOrWhiteSpace(assetNumber) ? assetNumber : string.Empty;
            GameTheme = !string.IsNullOrWhiteSpace(gameTheme) ? gameTheme : MeterData.NoGameTitle;
            Denomination = denomination;
            IsActive = isActive;
            Zone = !string.IsNullOrWhiteSpace(zone) ? zone : string.Empty;
            Bank = !string.IsNullOrWhiteSpace(bank) ? bank : string.Empty;
            Stand = stand;

            if (null != dateInPlace)
            {
                DateInPlace = (DateTime) dateInPlace;
            }
            else
            {
                DateInPlace = DateTime.MinValue;
            }

            DaysOnFloor = daysOnFloor;
            Par = !string.IsNullOrWhiteSpace(par) ? par : string.Empty;
            CoinIn = coinIn;
            CoinOut = coinOut;
            BillDrop = billDrop;
            TicketDrop = ticketDrop;
            TicketOut = ticketOut;
            Handpay = handpay;
            Jackpot = jackpot;
            MeteredAttendantPaidProgressive = meteredAttendantPaidProgressive;
            MeteredMachinePaidProgressive = meteredMachinePaidProgressive;
            GamesPlayed = gamesPlayed;
            GamesWon = gamesWon;
            GamesLost = gamesLost;
            AverageBet = averageBet;
            CoinInPerUnit = coinInPerUnit;
            WinPerUnit = winPerUnit;
            SlotRevenue = slotRevenue;
            FreePlay = freePlay;
            TotalNetWin = totalNetWin;
            LessorPercent = lessorPercent;
            LessorFee = lessorFee;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeEgmMeterData" /> class.
        /// Used by Entity Framework when reconstituting a database record
        /// into an in-memory CompositeEgmMeterData instance.
        /// </summary>
        protected CompositeEgmMeterData()
        {
        }

        #endregion
    }
}