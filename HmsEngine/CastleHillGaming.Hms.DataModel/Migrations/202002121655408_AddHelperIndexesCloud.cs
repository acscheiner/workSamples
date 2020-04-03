namespace CastleHillGaming.Hms.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHelperIndexesCloud : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.CompositeEgmMeterData", "AuditDate");
            CreateIndex("dbo.CompositeEgmMeterData", new[] { "CasinoName", "SerialNumber", "AssetNumber", "GameTheme", "Denomination" }, name: "IX_ADate_Casino_SerialNum_AssetNum_Game_Denom");
            CreateIndex("dbo.EgmEvent", "ReportedAt");
            CreateIndex("dbo.EgmMeterReading", "ReportedAt");
            CreateIndex("dbo.EgmMetric", "ReportedAt");
            CreateIndex("dbo.EgmVersion", "ReportedAt");
            CreateIndex("dbo.EgmWindowsEvent", "ReportedAt");
        }
        
        public override void Down()
        {
            DropIndex("dbo.EgmWindowsEvent", new[] { "ReportedAt" });
            DropIndex("dbo.EgmVersion", new[] { "ReportedAt" });
            DropIndex("dbo.EgmMetric", new[] { "ReportedAt" });
            DropIndex("dbo.EgmMeterReading", new[] { "ReportedAt" });
            DropIndex("dbo.EgmEvent", new[] { "ReportedAt" });
            DropIndex("dbo.CompositeEgmMeterData", "IX_ADate_Casino_SerialNum_AssetNum_Game_Denom");
            DropIndex("dbo.CompositeEgmMeterData", new[] { "AuditDate" });
        }
    }
}
