// Copyright (c) 2019 Castle Hill Gaming, LLC. All rights reserved.
namespace CastleHillGaming.Hms.DataModel.DataAccessLayer.Dao
{
    using System;
    using System.Collections;
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

    /// <summary>
    /// Class EgmVersionDao.
    /// </summary>
    public class EgmVersionDao
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Generates the hash for the specified EgmVersion entity.
        /// </summary>
        /// <param name="egmVersion">The egm version.</param>
        /// <returns>System.String.</returns>
        public static string GenerateHash(EgmVersion egmVersion)
        {
            return GenerateHash(egmVersion.ObjectName, egmVersion.VersionInfo, egmVersion.EgmSerialNumber, egmVersion.CasinoCode);
        }

        /// <summary>
        /// Generates the hash.
        /// </summary>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="casinoCode">The casino code.</param>
        /// <param name="egmVersion">The egm version.</param>
        /// <returns>System.String.</returns>
        public static string GenerateHash(string egmSerialNumber, string casinoCode, IVersionData egmVersion)
        {
            return GenerateHash(egmVersion.ObjectName, egmVersion.VersionInfo, egmSerialNumber, casinoCode);
        }

        /// <summary>
        /// Gets all EgmVersion Entities.
        /// </summary>
        /// <returns>ICollection&lt;EgmVersion&gt;.</returns>
        public ICollection<EgmVersion> GetAll()
        {
            using (var context = new HmsDbContext())
            {
                return context.EgmVersions.AsQueryable()
                    .OrderBy(v => v.CasinoCode)
                    .ThenBy(v => v.EgmSerialNumber)
                    .ThenBy(v => v.EgmAssetNumber)
                    .ThenBy(v => v.ReportedAt)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets the EgmVersion entity with the specified ID/PK.
        /// </summary>
        /// <param name="versionId">The version identifier.</param>
        /// <returns>EgmMeterReading.</returns>
        public EgmVersion GetById(long versionId)
        {
            using (var context = new HmsDbContext())
            {
                return context.EgmVersions.Find(versionId);
            }
        }

        /// <summary>
        /// Gets all unreported EgmVersion entities.
        /// </summary>
        /// <returns>ICollection&lt;EgmVersion&gt;.</returns>
        public ICollection<EgmVersion> GetUnReported()
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmVersions.AsQueryable()
                        .Where(v => v.ReportGuid == Guid.Empty)
                        .OrderBy(v => v.CasinoCode)
                        .ThenBy(v => v.EgmSerialNumber)
                        .ThenBy(v => v.EgmAssetNumber)
                        .ThenBy(v => v.ReportedAt)
                        .ToList();
            }
        }
        /// <summary>
        /// Gets all unsent EgmVersions entities.
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmVersion entities which have been reported.</param>
        /// <param name="maxRecords">The maximum records to include in the query result.</param>
        /// <returns>ICollection&lt;EgmVersion&gt;.</returns>
        public ICollection<EgmVersion> GetUnsent(bool reported = false, int maxRecords = DaoUtilities.NoRecordLimit)
        {
            using (var context = new HmsDbContext())
            {
                var query = context.EgmVersions.AsQueryable().Where(v => v.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(v => v.ReportGuid != Guid.Empty);
                }

                query = query.OrderBy(v => v.CasinoCode)
                    .ThenBy(v => v.EgmSerialNumber)
                    .ThenBy(v => v.EgmAssetNumber)
                    .ThenBy(v => v.ReportedAt);

                if (DaoUtilities.NoRecordLimit != maxRecords && 0 < maxRecords)
                {
                    query = query.Take(maxRecords);
                }

                return query.ToList();
            }
        }

        /// <summary>
        /// Gets the Number of unsent EgmVersion entities.
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmVersion entities which have been reported.</param>
        /// <returns>The number of unsent EgmVersion entities</returns>
        public int NumUnsent(bool reported = false)
        {
            using (var context = new HmsDbContext())
            {
                var query = context.EgmVersions.AsQueryable().Where(v => v.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(v => v.ReportGuid != Guid.Empty);
                }

                return query.Count();
            }
        }

        /// <summary>
        /// Gets all EgmVersion entities with the specified ReportGuid.
        /// </summary>
        /// <param name="reportGuid">The report unique identifier.</param>
        /// <returns>ICollection&lt;EgmVersion&gt;.</returns>
        public ICollection<EgmVersion> GetByReportGuid(Guid reportGuid)
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmVersions.AsQueryable()
                        .Where(v => v.ReportGuid == reportGuid)
                        .OrderBy(v => v.CasinoCode)
                        .ThenBy(v => v.EgmSerialNumber)
                        .ThenBy(v => v.EgmAssetNumber)
                        .ThenBy(v => v.ReportedAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Cleans EgmVersion Entities older than specified date-time.
        /// </summary>
        /// <param name="oldDateTime">The old date time.</param>
        public void CleanOlderThan(DateTime oldDateTime)
        {
            using (var context = new HmsDbContext())
            {
                context.EgmVersions.RemoveRange(
                    context.EgmVersions.Where(
                        v => v.ReportedAt != DaoUtilities.UnsentData && v.ReportedAt < oldDateTime));
                DaoUtilities.SaveToDbWithRetry(context, DaoUtilities.SaveType.DeleteExistingEntity,
                    ResolveEntityUpdateConflict);
            }
        }

        /// <summary>
        /// Saves the specified EgmVersion.
        /// </summary>
        /// <param name="egmVersion">The EgmVersion to save.</param>
        public void Save(EgmVersion egmVersion)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                if (!context.EgmVersions.Any(version => version.Id == egmVersion.Id))
                {
                    // no matching PK for this EgmVersion in database,
                    // thus we create new entity and add it to db
                    DaoUtilities.SaveCreatedEntity(context, context.EgmVersions, egmVersion, SetNewEntityState);
                }
                else
                {
                    // matching PK found, thus we update state of existing EgmVersion entity
                    DaoUtilities.SaveUpdatedEntity(context, context.EgmVersions, egmVersion, UpdateExistingEntityState);
                }
            }
        }

        /// <summary>
        /// Saves the specified egm versions.
        /// </summary>
        /// <param name="egmVersions">The egm versions.</param>
        public void Save(ICollection<EgmVersion> egmVersions)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                foreach (var egmVersion in egmVersions)
                {
                    if (!context.EgmVersions.Any(version => version.Id == egmVersion.Id))
                    {
                        // no matching PK for this EgmVersion in database,
                        // thus we create new entity and add it to db
                        DaoUtilities.SaveCreatedEntity(context, context.EgmVersions, egmVersion,
                            SetNewEntityState, false);
                    }
                    else
                    {
                        // matching PK found, thus we update state of existing EgmVersion entity
                        DaoUtilities.SaveUpdatedEntity(context, context.EgmVersions, egmVersion,
                            UpdateExistingEntityState, false);
                    }
                }

                DaoUtilities.SaveToDbWithRetry(context, DaoUtilities.SaveType.UpdateExistingEntity,
                    ResolveEntityUpdateConflict);
            }
        }

        /// <summary>
        /// Deletes the specified EgmVersion entity.
        /// </summary>
        /// <param name="egmVersion">The EgmVersion entity to delete.</param>
        public void Delete(EgmVersion egmVersion)
        {
            using (var context = new HmsDbContext())
            {
                if (!context.EgmVersions.Any(v => v.Id == egmVersion.Id))
                {
                    return;
                }

                // matching PK found, thus we proceed with Delete
                DaoUtilities.DeleteEntity(context, context.EgmVersions, egmVersion);
            }
        }


        /// <summary>
        /// Sets the new state of the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private static void SetNewEntityState(EgmVersion entity)
        {
            entity.Version = 0;
            entity.Hash = GenerateHash(entity);
        }

        /// <summary>
        /// Updates the state of the existing entity.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="egmVersion">The egm version data.</param>
        private static void UpdateExistingEntityState(HmsDbContext context, DbEntityEntry entityEntry,
            EgmVersion egmVersion)
        {
            var entity = (EgmVersion)entityEntry.Entity;
            if (!entity.SentAt.Equals(egmVersion.SentAt))
            {
                entity.SentAt = egmVersion.SentAt;
            }

            if (!entity.ReportGuid.Equals(egmVersion.ReportGuid))
            {
                entity.ReportGuid = egmVersion.ReportGuid;
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
            // This should never occur for EgmVersion entities
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

        /// <summary>
        /// Generates the hash for an EgmVersion entity.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="versionInfo">The version information.</param>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="casinoCode">The casino code.</param>
        /// <returns>System.String.</returns>
        private static string GenerateHash(string objectName, string versionInfo, string egmSerialNumber, string casinoCode)
        {
            var md5 = MD5.Create();

            var stringToHash =
                $"{objectName}{versionInfo}e{egmSerialNumber}{casinoCode}{Settings.Default.VersionEntityHashKey}";

            var inputBytes = Encoding.ASCII.GetBytes(stringToHash);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hash)
            {
                sb.Append(hashByte.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
