// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 10-12-2016
//
// Last Modified By : acscheiner
// Last Modified On : 10-12-2016
// ***********************************************************************
// <copyright file="DbContextExtensions.cs" company="">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel.DataAccessLayer.DbContext
{
    #region

    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.ModelConfiguration.Configuration;

    #endregion

    /// <summary>
    /// Class DbContextExtensions.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Converts a DbContext to its associated ObjectContext.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <returns>ObjectContext.</returns>
        public static ObjectContext ToObjectContext(this DbContext dbContext)
        {
            return (dbContext as IObjectContextAdapter).ObjectContext;
        }

        /// <summary>
        /// Helper used for specifying a unique db property.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>PrimitivePropertyConfiguration.</returns>
        public static PrimitivePropertyConfiguration IsUnique(this PrimitivePropertyConfiguration configuration)
        {
            return configuration.HasColumnAnnotation("Index",
                new IndexAnnotation(new IndexAttribute {IsUnique = true}));
        }
    }
}