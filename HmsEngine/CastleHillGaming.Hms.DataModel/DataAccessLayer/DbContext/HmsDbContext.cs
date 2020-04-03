// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 11-01-2016
//
// Last Modified By : acscheiner
// Last Modified On : 11-01-2016
// ***********************************************************************
// <copyright file="HmsDbContext.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel.DataAccessLayer.DbContext
{
    #region

    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using CastleHillGaming.Hms.DataModel.DataAccessLayer.Config;
    using CastleHillGaming.Hms.DataModel.Migrations;

    #endregion

    /// <summary>
    /// Class HmsDbContext.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    public class HmsDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the egm meter readings.
        /// </summary>
        /// <value>The egm meter readings.</value>
        public DbSet<EgmMeterReading> EgmMeterReadings { get; set; }

        /// <summary>
        /// Gets or sets the egm events.
        /// </summary>
        /// <value>The egm events.</value>
        public DbSet<EgmEvent> EgmEvents { get; set; }

        /// <summary>
        /// Gets or sets the composite egm meter datas.
        /// </summary>
        /// <value>The composite egm meter datas.</value>
        public DbSet<CompositeEgmMeterData> CompositeEgmMeterDatas { get; set; }

        /// <summary>
        /// Gets or sets the egm metrics.
        /// </summary>
        /// <value>The egm metrics.</value>
        public DbSet<EgmMetric> EgmMetrics { get; set; }

        /// <summary>
        /// Gets or sets the egm windows events.
        /// </summary>
        /// <value>The egm windows events.</value>
        public DbSet<EgmWindowsEvent> EgmWindowsEvents { get; set; }

        /// <summary>
        /// Gets or sets the egm versions.
        /// </summary>
        /// <value>The egm versions.</value>
        public DbSet<EgmVersion> EgmVersions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HmsDbContext" /> class.
        /// </summary>
        public HmsDbContext() : base("HmsDbContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<HmsDbContext, Configuration>());
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuidler, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.</remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Type-specific configuration
            modelBuilder.Configurations.Add(new EgmEventConfiguration());
            modelBuilder.Configurations.Add(new EgmMeterReadingConfiguration());
            modelBuilder.Configurations.Add(new CompositeEgmMeterDataConfiguration());
            modelBuilder.Configurations.Add(new EgmMetricConfiguration());
            modelBuilder.Configurations.Add(new EgmWindowsEventConfiguration());
            modelBuilder.Configurations.Add(new EgmVersionConfiguration());

            // General data model configuration
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}