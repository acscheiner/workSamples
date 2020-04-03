// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 03-29-2017
//
// Last Modified By : acscheiner
// Last Modified On : 03-29-2017
// ***********************************************************************
// <copyright file="EgmMetricDao.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel.DataAccessLayer.Dao
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using CastleHillGaming.Hms.DataModel.DataAccessLayer.DbContext;
    using CastleHillGaming.Hms.DataModel.Properties;
    using CastleHillGaming.Hms.Interfaces;
    using log4net;

    #endregion

    /// <summary>
    /// Class EgmMetricDao.
    /// </summary>
    public class EgmMetricDao
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Public CRUD methods

        /// <summary>
        /// Gets all EgmMetric Entities.
        /// </summary>
        /// <returns>ICollection&lt;EgmMetric&gt;.</returns>
        public ICollection<EgmMetric> GetAll()
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmMetrics.AsQueryable()
                        .OrderBy(mr => mr.CasinoCode)
                        .ThenBy(mr => mr.EgmSerialNumber)
                        .ThenBy(mr => mr.EgmAssetNumber)
                        .ThenBy(mr => mr.ReportedAt).ThenBy(mr => mr.ReadAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets the EgmMetric entity with the specified ID/PK.
        /// </summary>
        /// <param name="metricId">The metric identifier.</param>
        /// <returns>EgmMetric.</returns>
        public EgmMetric GetById(long metricId)
        {
            using (var context = new HmsDbContext())
            {
                return context.EgmMetrics.Find(metricId);
            }
        }

        /// <summary>
        /// Gets all unreported EgmMetric entities.
        /// </summary>
        /// <returns>ICollection&lt;EgmMetric&gt;.</returns>
        public ICollection<EgmMetric> GetUnreported()
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmMetrics.AsQueryable()
                        .Where(mr => mr.ReportGuid == Guid.Empty)
                        .OrderBy(mr => mr.CasinoCode)
                        .ThenBy(mr => mr.EgmSerialNumber)
                        .ThenBy(mr => mr.EgmAssetNumber)
                        .ThenBy(mr => mr.ReportedAt).ThenBy(mr => mr.ReadAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets all unsent EgmMetric entities.
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmMetric entities which have been reported.</param>
        /// <param name="maxRecords">The maximum records to include in the query result.</param>
        /// <returns>ICollection&lt;EgmMetric&gt;.</returns>
        public ICollection<EgmMetric> GetUnsent(bool reported = false, int maxRecords = DaoUtilities.NoRecordLimit)
        {
            using (var context = new HmsDbContext())
            {
                var query = context.EgmMetrics.AsQueryable().Where(mr => mr.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(mr => mr.ReportGuid != Guid.Empty);
                }

                query = query.OrderBy(mr => mr.CasinoCode)
                    .ThenBy(mr => mr.EgmSerialNumber)
                    .ThenBy(mr => mr.EgmAssetNumber)
                    .ThenBy(mr => mr.ReportedAt).ThenBy(mr => mr.ReadAt);

                if (DaoUtilities.NoRecordLimit != maxRecords && 0 < maxRecords)
                {
                    query = query.Take(maxRecords);
                }

                return query.ToList();
            }
        }

        /// <summary>
        /// Gets the Number of unsent EgmMetric entities.
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmMetric entities which have been reported.</param>
        /// <returns>The number of unsent EgmMetric entities</returns>
        public int NumUnsent(bool reported = false)
        {
            using (var context = new HmsDbContext())
            {
                var query = context.EgmMetrics.AsQueryable().Where(evt => evt.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(evt => evt.ReportGuid != Guid.Empty);
                }

                return query.Count();
            }
        }

        /// <summary>
        /// Are there any unsent EgmMetric entities?
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmMetric entities which have been reported.</param>
        /// <returns><c>true</c> if there are any unsent EgmMetric entities, <c>false</c> otherwise.</returns>
        public bool AreUnsent(bool reported = false)
        {
            using (var context = new HmsDbContext())
            {
                var query = context.EgmMetrics.AsQueryable().Where(evt => evt.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(evt => evt.ReportGuid != Guid.Empty);
                }

                return query.Any();
            }
        }

        /// <summary>
        /// Gets all EgmMetric entities with the specified ReportGuid.
        /// </summary>
        /// <param name="reportGuid">The report unique identifier.</param>
        /// <returns>ICollection&lt;EgmMetric&gt;.</returns>
        public ICollection<EgmMetric> GetByReportGuid(Guid reportGuid)
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmMetrics.AsQueryable()
                        .Where(mr => mr.ReportGuid == reportGuid)
                        .OrderBy(mr => mr.CasinoCode)
                        .ThenBy(mr => mr.EgmSerialNumber)
                        .ThenBy(mr => mr.EgmAssetNumber)
                        .ThenBy(mr => mr.ReportedAt).ThenBy(mr => mr.ReadAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets EgmMetric Entities older than specified date-time.
        /// </summary>
        /// <param name="oldDateTime">The time to use for the older-than comparison.</param>
        /// <returns>ICollection&lt;EgmMetric&gt;.</returns>
        public ICollection<EgmMetric> GetOlderThan(DateTime oldDateTime)
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmMetrics.AsQueryable()
                        .Where(mr => mr.ReportedAt != DaoUtilities.UnsentData && mr.ReportedAt <= oldDateTime)
                        .ToList();
            }
        }

        /// <summary>
        /// Cleans EgmMetric Entities older than specified date-time.
        /// </summary>
        /// <param name="oldDateTime">The old date time.</param>
        public void CleanOlderThan(DateTime oldDateTime)
        {
            using (var context = new HmsDbContext())
            {
                context.EgmMetrics.RemoveRange(
                    context.EgmMetrics.Where(
                        mr => mr.ReportedAt != DaoUtilities.UnsentData && mr.ReportedAt < oldDateTime));
                DaoUtilities.SaveToDbWithRetry(context, DaoUtilities.SaveType.DeleteExistingEntity,
                    ResolveEntityUpdateConflict);
            }
        }

        /// <summary>
        /// Saves the specified EgmMetric.
        /// </summary>
        /// <param name="egmMetric">The EgmMetric to save.</param>
        public void Save(EgmMetric egmMetric)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                if (!context.EgmMetrics.Any(metric => metric.Id == egmMetric.Id))
                {
                    // no matching PK for this EgmMetric in database,
                    // thus we create new entity and add it to db
                    DaoUtilities.SaveCreatedEntity(context, context.EgmMetrics, egmMetric, SetNewEntityState);
                }
                else
                {
                    // matching PK found, thus we update state of existing EgmMetric entity
                    DaoUtilities.SaveUpdatedEntity(context, context.EgmMetrics, egmMetric, UpdateExistingEntityState);
                }
            }
        }

        /// <summary>
        /// Saves the specified egm metrics.
        /// </summary>
        /// <param name="egmMetrics">The egm metrics.</param>
        public void Save(ICollection<EgmMetric> egmMetrics)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                foreach (var egmMetric in egmMetrics)
                {
                    if (!context.EgmMetrics.Any(metric => metric.Id == egmMetric.Id))
                    {
                        // no matching PK for this EgmMetric in database,
                        // thus we create new entity and add it to db
                        DaoUtilities.SaveCreatedEntity(context, context.EgmMetrics, egmMetric,
                            SetNewEntityState, false);
                    }
                    else
                    {
                        // matching PK found, thus we update state of existing EgmMetric entity
                        DaoUtilities.SaveUpdatedEntity(context, context.EgmMetrics, egmMetric,
                            UpdateExistingEntityState, false);
                    }
                }

                DaoUtilities.SaveToDbWithRetry(context, DaoUtilities.SaveType.UpdateExistingEntity,
                    ResolveEntityUpdateConflict);
            }
        }

        /// <summary>
        /// Deletes the specified EgmMetric entity.
        /// </summary>
        /// <param name="egmMetric">The EgmMetric entity to delete.</param>
        public void Delete(EgmMetric egmMetric)
        {
            using (var context = new HmsDbContext())
            {
                if (!context.EgmMetrics.Any(mr => mr.Id == egmMetric.Id))
                {
                    return;
                }

                // matching PK found, thus we proceed with Delete
                DaoUtilities.DeleteEntity(context, context.EgmMetrics, egmMetric);
            }
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Generates the hash for an EgmMetric entity.
        /// </summary>
        /// <param name="egmMetric">The EgmMetric to hash.</param>
        /// <returns>System.String.</returns>
        public static string GenerateHash(EgmMetric egmMetric)
        {
            return GenerateHash(egmMetric.Type.ToString(), egmMetric.Value, egmMetric.EgmSerialNumber, egmMetric.ReadAt,
                egmMetric.CasinoCode);
        }

        /// <summary>
        /// Generates the hash.
        /// </summary>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="casinoCode">The casino code.</param>
        /// <param name="egmMetric">The egm metric.</param>
        /// <returns>System.String.</returns>
        public static string GenerateHash(string egmSerialNumber, string casinoCode, IMetricData egmMetric)
        {
            return GenerateHash(egmMetric.Type.ToString(), egmMetric.Value, egmSerialNumber, egmMetric.ReadAt,
                casinoCode);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Generates the hash.
        /// </summary>
        /// <param name="metricType">Type of the metric.</param>
        /// <param name="value">The value.</param>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="readAt">The read at.</param>
        /// <param name="casinoCode">The casino code.</param>
        /// <returns>System.String.</returns>
        private static string GenerateHash(string metricType, float value, string egmSerialNumber, DateTime readAt,
            string casinoCode)
        {
            var md5 = MD5.Create();

            var stringToHash = metricType + value + egmSerialNumber + readAt + casinoCode +
                               Settings.Default.MetricEntityHashKey;

            var inputBytes = Encoding.ASCII.GetBytes(stringToHash);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hash)
            {
                sb.Append(hashByte.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Sets the new state of the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private static void SetNewEntityState(EgmMetric entity)
        {
            entity.Version = 0;
            entity.Hash = GenerateHash(entity);
        }

        /// <summary>
        /// Updates the state of the existing entity.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="egmMetric">The egm metric.</param>
        private static void UpdateExistingEntityState(HmsDbContext context, DbEntityEntry entityEntry,
            EgmMetric egmMetric)
        {
            var metricEntity = (EgmMetric) entityEntry.Entity;
            if (!metricEntity.SentAt.Equals(egmMetric.SentAt))
            {
                metricEntity.SentAt = egmMetric.SentAt;
            }

            if (!metricEntity.ReportGuid.Equals(egmMetric.ReportGuid))
            {
                metricEntity.ReportGuid = egmMetric.ReportGuid;
            }

            DaoUtilities.UpdateVersion(context, entityEntry);
        }

        /// <summary>
        /// Resolves the entity update conflict.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityEntry">The entity entry.</param>
        private static void ResolveEntityUpdateConflict(HmsDbContext context, DbEntityEntry entityEntry)
        {
            ///////////////////////////////////////////////////////////////////////////
            // This should never occur for EgmMetric entities
            // because we simply add records to the db table when we receive
            // them from the HMS EGM Client message (HMS Onsite Service) or the HMS Onsite
            // Service (HMS Cloud Service). We do update the records on the
            // HMS Onsite Service when we package them up and send them to
            // the HMS Cloud Service (ReportGuid and SentAt fields are updated)
            // but in a single-threaded way and only after having created the
            // initial record.
            // 
            // We will use the "first in wins" resolution approach, but this ought
            // never be invoked.
            //////////////////////////////////////////////////////////////////////////
            entityEntry.Reload();
            DaoUtilities.UpdateVersion(context, entityEntry);
        }

        #endregion
    }
}