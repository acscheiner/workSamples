namespace CastleHillGaming.Hms.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompositeEgmMeterData",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AuditDate = c.DateTime(nullable: false, precision: 6),
                        CasinoName = c.String(nullable: false),
                        SerialNumber = c.String(nullable: false),
                        AssetNumber = c.String(nullable: false),
                        GameTheme = c.String(nullable: false),
                        Denomination = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Zone = c.String(nullable: false),
                        Bank = c.String(nullable: false),
                        Stand = c.Int(nullable: false),
                        DateInPlace = c.DateTime(nullable: false, precision: 6),
                        DaysOnFloor = c.Int(nullable: false),
                        Par = c.String(nullable: false),
                        CoinIn = c.Long(nullable: false),
                        CoinOut = c.Long(nullable: false),
                        BillDrop = c.Long(nullable: false),
                        TicketDrop = c.Long(nullable: false),
                        TicketOut = c.Long(nullable: false),
                        Handpay = c.Long(nullable: false),
                        Jackpot = c.Long(nullable: false),
                        MeteredAttendantPaidProgressive = c.Long(nullable: false),
                        MeteredMachinePaidProgressive = c.Long(nullable: false),
                        GamesPlayed = c.Long(nullable: false),
                        GamesWon = c.Long(nullable: false),
                        GamesLost = c.Long(nullable: false),
                        AverageBet = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CoinInPerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WinPerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SlotRevenue = c.Long(nullable: false),
                        FreePlay = c.Long(nullable: false),
                        TotalNetWin = c.Long(nullable: false),
                        LessorPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LessorFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EgmEvent",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        OccurredAt = c.DateTime(nullable: false, precision: 6),
                        SentAt = c.DateTime(nullable: false, precision: 6),
                        CasinoCode = c.String(nullable: false),
                        ReportGuid = c.Guid(nullable: false),
                        EgmSerialNumber = c.String(nullable: false),
                        ReportedAt = c.DateTime(nullable: false, precision: 6),
                        EgmAssetNumber = c.String(nullable: false),
                        Hash = c.String(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EgmMeterReading",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GameTitle = c.String(nullable: false),
                        GameDenomination = c.Long(nullable: false),
                        Type = c.Int(nullable: false),
                        Value = c.Long(nullable: false),
                        Units = c.String(nullable: false),
                        ReadAt = c.DateTime(nullable: false, precision: 6),
                        SentAt = c.DateTime(nullable: false, precision: 6),
                        CasinoCode = c.String(nullable: false),
                        ReportGuid = c.Guid(nullable: false),
                        EgmSerialNumber = c.String(nullable: false),
                        ReportedAt = c.DateTime(nullable: false, precision: 6),
                        EgmAssetNumber = c.String(nullable: false),
                        Hash = c.String(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EgmMetric",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SentAt = c.DateTime(nullable: false, precision: 6),
                        CasinoCode = c.String(nullable: false),
                        ReportGuid = c.Guid(nullable: false),
                        ReportedAt = c.DateTime(nullable: false, precision: 6),
                        EgmSerialNumber = c.String(nullable: false),
                        EgmAssetNumber = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        Value = c.Single(nullable: false),
                        ReadAt = c.DateTime(nullable: false, precision: 6),
                        Hash = c.String(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EgmVersion",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CasinoCode = c.String(nullable: false),
                        EgmAssetNumber = c.String(nullable: false),
                        EgmSerialNumber = c.String(nullable: false),
                        ObjectName = c.String(nullable: false),
                        VersionInfo = c.String(nullable: false),
                        ReportedAt = c.DateTime(nullable: false, precision: 6),
                        ReportGuid = c.Guid(nullable: false),
                        SentAt = c.DateTime(nullable: false, precision: 6),
                        Hash = c.String(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EgmWindowsEvent",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CasinoCode = c.String(nullable: false),
                        ReportGuid = c.Guid(nullable: false),
                        ReportedAt = c.DateTime(nullable: false, precision: 6),
                        EgmSerialNumber = c.String(nullable: false),
                        EgmAssetNumber = c.String(nullable: false),
                        Code = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        OccurredAt = c.DateTime(nullable: false, precision: 6),
                        EventLogName = c.String(nullable: false),
                        SentAt = c.DateTime(nullable: false, precision: 6),
                        Hash = c.String(),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EgmWindowsEvent");
            DropTable("dbo.EgmVersion");
            DropTable("dbo.EgmMetric");
            DropTable("dbo.EgmMeterReading");
            DropTable("dbo.EgmEvent");
            DropTable("dbo.CompositeEgmMeterData");
        }
    }
}
