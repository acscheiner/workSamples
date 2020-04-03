// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 04-21-2017
//
// Last Modified By : acscheiner
// Last Modified On : 04-21-2017
// ***********************************************************************
// <copyright file="DaoUtilities.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel.DataAccessLayer.Dao
{
    #region

    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;
    using CastleHillGaming.Hms.DataModel.DataAccessLayer.DbContext;
    using log4net;

    #endregion

    /// <summary>
    /// Class DaoUtilities.
    /// </summary>
    public static class DaoUtilities
    {
        #region Private Static data

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The maximum save retries
        /// </summary>
        private const int MaxSaveRetries = 12;

        #endregion

        #region Public Static Data

        /// <summary>
        /// The default maximum records
        /// </summary>
        public const int NoRecordLimit = 0;

        /// <summary>
        /// The DateTime value signifying HMS data record has not yet been sent to HMS Cloud service
        /// </summary>
        public static DateTime UnsentData = DateTime.MinValue;

        /// <summary>
        /// The DateTime value signifying HMS data record is in the process of being sent HMS Cloud service
        /// </summary>
        public static DateTime DataSendInProgress = DateTime.MinValue + TimeSpan.FromMilliseconds(1.0);

        #endregion

        #region Public Enum

        public enum SaveType
        {
            AddNewEntity,
            UpdateExistingEntity,
            DeleteExistingEntity
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Saves to database with retry.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="saveType">Type of the save.</param>
        /// <param name="resolveConflict">The function to call when conflict resolution is needed.</param>
        public static void SaveToDbWithRetry(HmsDbContext context, SaveType saveType,
            Action<HmsDbContext, DbEntityEntry> resolveConflict)
        {
            var numSaveAttempts = 0;

            while (MaxSaveRetries > numSaveAttempts)
            {
                try
                {
                    SaveToDb(context);
                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (MaxSaveRetries <= ++numSaveAttempts)
                    {
                        Logger.Warn(
                            $"DaoUtilities.SaveToDbWithRetry - DbUpdateConcurrencyException retry count exceeded max limit of {MaxSaveRetries}. Aborting.");
                    }
                    else
                    {
                        Logger.Info(
                            $"DaoUtilities.SaveToDbWithRetry - DbUpdateConcurrencyException caught [{ex.Message}]; retrying context save");
                        switch (saveType)
                        {
                            case SaveType.AddNewEntity:
                                // This should never happen - No-op
                                break;

                            case SaveType.DeleteExistingEntity:
                                foreach (var failedEntityEntry in ex.Entries)
                                {
                                    failedEntityEntry.Reload();
                                    failedEntityEntry.State = EntityState.Deleted;
                                }

                                break;

                            case SaveType.UpdateExistingEntity:
                                foreach (var failedEntityEntry in ex.Entries)
                                {
                                    resolveConflict(context, failedEntityEntry);
                                }

                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(saveType), saveType, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves Pending Changes in the Open Context to the database.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void SaveToDb(HmsDbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException dbcex)
            {
                // This type of exception is indicative of an optimistic locking conflict - e.g., another
                // thread has updated the same db record after we had pulled the entity out of the db, but before
                // we saved. In such situations, we re-throw here so the calling code can retry the save if possible.
                Logger.Warn(
                    $"DaoUtilities.SaveToDb : DbUpdateConcurrencyException caught - [{dbcex.Message}]");
                throw;
            }
            catch (Exception ex)
            {
                Logger.Warn($"DaoUtilities.SaveToDb : Exception caught - [{ex.Message}]");
                var innerEx = ex.InnerException;
                while (null != innerEx)
                {
                    Logger.Warn($"DaoUtilities.SaveToDb : Inner Exception: [{innerEx.Message}]");
                    innerEx = innerEx.InnerException;
                }
            }
        }

        /// <summary>
        /// Gets the existing entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbSet">The database set.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>T.</returns>
        public static T GetExistingEntity<T>(DbSet<T> dbSet, T entity) where T : EntityBase
        {
            try
            {
                return dbSet.Single(ent => ent.Id == entity.Id);
            }
            catch (InvalidOperationException)
            {
                Logger.Warn(
                    $"DaoUtilities.GetExistingEntity - Could not find a unique [{typeof(T)}] entity with PK=[{entity.Id}]");
                return null;
            }
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="dbSet">The database set.</param>
        /// <param name="entity">The entity.</param>
        public static void DeleteEntity<T>(HmsDbContext context, DbSet<T> dbSet, T entity) where T : EntityBase
        {
            var foundEntity = GetExistingEntity(dbSet, entity);
            if (null == foundEntity) return;

            DbEntityEntry entityEntry = context.Entry(foundEntity);
            entityEntry.State = EntityState.Deleted;

            var numSaveAttempts = 0;

            while (MaxSaveRetries > numSaveAttempts)
            {
                try
                {
                    SaveToDb(context);
                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (MaxSaveRetries <= ++numSaveAttempts)
                    {
                        Logger.Warn(
                            $"DaoUtilities.DeleteEntity - DbUpdateConcurrencyException retry count exceeded max limit of {MaxSaveRetries}. Aborting.");
                    }
                    else
                    {
                        entityEntry = ex.Entries.Single();
                        entityEntry.Reload();
                        entityEntry.State = EntityState.Deleted;
                    }
                }
            }
        }

        /// <summary>
        /// Saves the created entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="dbSet">The database set.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="createEntityAction">The create entity action.</param>
        /// <param name="doSaveChanges">if true, save changes in HmsDbContext; otherwise do not</param>
        public static void SaveCreatedEntity<T>(HmsDbContext context, DbSet<T> dbSet, T entity,
            Action<T> createEntityAction, bool doSaveChanges = true)
            where T : EntityBase
        {
            createEntityAction(entity);
            dbSet.Add(entity);
            if (doSaveChanges)
            {
                SaveToDb(context);
            }
        }

        /// <summary>
        /// Saves the updated entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="dbSet">The database set.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="updateEntityAction">The update entity action.</param>
        /// <param name="doSaveChanges">if true, save changes in HmsDbContext; otherwise do not</param>
        public static void SaveUpdatedEntity<T>(HmsDbContext context, DbSet<T> dbSet, T entity,
            Action<HmsDbContext, DbEntityEntry, T> updateEntityAction, bool doSaveChanges = true)
            where T : EntityBase
        {
            var foundEntity = GetExistingEntity(dbSet, entity);
            if (null == foundEntity) return;

            DbEntityEntry entityEntry = context.Entry(foundEntity);

            var numSaveAttempts = 0;

            while (MaxSaveRetries > numSaveAttempts)
            {
                updateEntityAction(context, entityEntry, entity);
                if (!doSaveChanges) break;

                try
                {
                    SaveToDb(context);
                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (MaxSaveRetries <= ++numSaveAttempts)
                    {
                        Logger.Warn(
                            $"DaoUtilities.SaveUpdatedEntity - DbUpdateConcurrencyException retry count exceeded max limit of {MaxSaveRetries}. Aborting.");
                    }
                    else
                    {
                        entityEntry = ex.Entries.Single();
                        entityEntry.Reload();
                    }
                }
            }
        }

        /// <summary>
        /// Updates the version of the specified entity.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityEntry">The entity entry</param>
        public static void UpdateVersion(HmsDbContext context, DbEntityEntry entityEntry)
        {
            if ((entityEntry.State & EntityState.Modified) != 0)
            {
                ((EntityBase) entityEntry.Entity).Version++;
            }
        }

        #endregion
    }
}