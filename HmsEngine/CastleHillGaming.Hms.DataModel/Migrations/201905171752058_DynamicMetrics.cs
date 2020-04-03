namespace CastleHillGaming.Hms.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DynamicMetrics : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EgmMetric", "Type", col => col.String(nullable: false));
            Sql(@"
                UPDATE public.""EgmMetric""
                SET ""Type"" =
                CASE ""Type""
                    WHEN '0' THEN 'TotalPlayTimeOnDevice'
                    WHEN '1' THEN 'NumberOfSessions'
                    WHEN '2' THEN 'TotalReelSpinTime'
                    WHEN '3' THEN 'BonusGameHitCount'
                    WHEN '4' THEN 'TotalReelSpinCount'
                    ELSE 'UnknownMetricType'
                END
            ");
        }

        public override void Down()
        {
            throw new Exception("Cannot revert from DynamicMetrics DbMigration - migration to DynamicMetrics is irreversible");
        }
    }
}
