namespace CastleHillGaming.Hms.DataModel.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using CastleHillGaming.Hms.DataModel.DataAccessLayer.DbContext;
    using Devart.Data.PostgreSql.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<CastleHillGaming.Hms.DataModel.DataAccessLayer.DbContext.HmsDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = typeof(HmsDbContext).ToString();
            SetSqlGenerator(PgSqlConnectionInfo.InvariantName, new PgSqlEntityMigrationSqlGenerator());
        }

        protected override void Seed(CastleHillGaming.Hms.DataModel.DataAccessLayer.DbContext.HmsDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
