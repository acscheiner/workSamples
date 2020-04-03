// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 10-10-2016
//
// Last Modified By : acscheiner
// Last Modified On : 10-25-2016
// ***********************************************************************
// <copyright file="EgmMeterReadingDao.cs" company="">
//     Copyright ©  2016
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
    /// Class EgmMeterReadingDao.
    /// </summary>
    public class EgmMeterReadingDao
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Public CRUD methods

        /// <summary>
        /// Gets all EgmMeterReading Entities.
        /// </summary>
        /// <returns>ICollection&lt;EgmMeterReading&gt;.</returns>
        public ICollection<EgmMeterReading> GetAll()
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmMeterReadings.AsQueryable()
                        .OrderBy(mr => mr.CasinoCode)
                        .ThenBy(mr => mr.EgmSerialNumber)
                        .ThenBy(mr => mr.EgmAssetNumber)
                        .ThenBy(mr => mr.GameTitle)
                        .ThenBy(mr => mr.GameDenomination)
                        .ThenBy(mr => mr.ReportedAt).ThenBy(mr => mr.ReadAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets the EgmMeterReading entity with the specified ID/PK.
        /// </summary>
        /// <param name="meterReadingId">The meter reading identifier.</param>
        /// <returns>EgmMeterReading.</returns>
        public EgmMeterReading GetById(long meterReadingId)
        {
            using (var context = new HmsDbContext())
            {
                return context.EgmMeterReadings.Find(meterReadingId);
            }
        }

        /// <summary>
        /// Gets all unreported EgmMeterReading entities.
        /// </summary>
        /// <returns>ICollection&lt;EgmMeterReading&gt;.</returns>
        public ICollection<EgmMeterReading> GetUnreported()
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmMeterReadings.AsQueryable()
                        .Where(mr => mr.ReportGuid == Guid.Empty)
                        .OrderBy(mr => mr.CasinoCode)
                        .ThenBy(mr => mr.EgmSerialNumber)
                        .ThenBy(mr => mr.EgmAssetNumber)
                        .ThenBy(mr => mr.GameTitle)
                        .ThenBy(mr => mr.GameDenomination)
                        .ThenBy(mr => mr.ReportedAt).ThenBy(mr => mr.ReadAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets all unsent EgmMeterReading entities.
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmMeterReadings which have been reported.</param>
        /// <param name="maxRecords">The maximum records to include in the query result.</param>
        /// <returns>ICollection&lt;EgmMeterReading&gt;.</returns>
        public ICollection<EgmMeterReading> GetUnsent(bool reported = false,
            int maxRecords = DaoUtilities.NoRecordLimit)
        {
            using (var context = new HmsDbContext())
            {
                var query = context.EgmMeterReadings.AsQueryable().Where(mr => mr.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(mr => mr.ReportGuid != Guid.Empty);
                }

                query = query.OrderBy(mr => mr.CasinoCode)
                    .ThenBy(mr => mr.EgmSerialNumber)
                    .ThenBy(mr => mr.EgmAssetNumber)
                    .ThenBy(mr => mr.GameTitle)
                    .ThenBy(mr => mr.GameDenomination)
                    .ThenBy(mr => mr.ReportedAt).ThenBy(mr => mr.ReadAt);

                if (DaoUtilities.NoRecordLimit != maxRecords && 0 < maxRecords)
                {
                    query = query.Take(maxRecords);
                }

                return query.ToList();
            }
        }

        /// <summary>
        /// Gets the Number of unsent EgmMeterReadings.
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmMeterReadings which have been reported.</param>
        /// <returns>The number of unsent EgmMeterReadings</returns>
        public int NumUnsent(bool reported = false)
        {
            using (var context = new HmsDbContext())
            {
                var query = context.EgmMeterReadings.AsQueryable().Where(evt => evt.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(evt => evt.ReportGuid != Guid.Empty);
                }

                return query.Count();
            }
        }

        /// <summary>
        /// Are there any unsent EgmMeterReadings?
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmMeterReadings which have been reported.</param>
        /// <returns><c>true</c> if there are any unsent EgmMeterReadings, <c>false</c> otherwise.</returns>
        public bool AreUnsent(bool reported = false)
        {
            using (var context = new HmsDbContext())
            {
                var query = context.EgmMeterReadings.AsQueryable().Where(evt => evt.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(evt => evt.ReportGuid != Guid.Empty);
                }

                return query.Any();
            }
        }

        /// <summary>
        /// Gets all EgmMeterReading entities with the specified ReportGuid.
        /// </summary>
        /// <param name="reportGuid">The report unique identifier.</param>
        /// <returns>ICollection&lt;EgmMeterReading&gt;.</returns>
        public ICollection<EgmMeterReading> GetByReportGuid(Guid reportGuid)
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmMeterReadings.AsQueryable()
                        .Where(mr => mr.ReportGuid == reportGuid)
                        .OrderBy(mr => mr.CasinoCode)
                        .ThenBy(mr => mr.EgmSerialNumber)
                        .ThenBy(mr => mr.EgmAssetNumber)
                        .ThenBy(mr => mr.GameTitle)
                        .ThenBy(mr => mr.GameDenomination)
                        .ThenBy(mr => mr.ReportedAt).ThenBy(mr => mr.ReadAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets EgmMeterReading Entities older than specified date-time.
        /// </summary>
        /// <param name="oldDateTime">The time to use for the older-than comparison.</param>
        /// <returns>ICollection&lt;EgmMeterReading&gt;.</returns>
        public ICollection<EgmMeterReading> GetOlderThan(DateTime oldDateTime)
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmMeterReadings.AsQueryable()
                        .Where(mr => mr.ReportedAt != DaoUtilities.UnsentData && mr.ReportedAt <= oldDateTime)
                        .ToList();
            }
        }

        /// <summary>
        /// Cleans EgmMeterReading Entities older than specified date-time.
        /// </summary>
        /// <param name="oldDateTime">The old date time.</param>
        public void CleanOlderThan(DateTime oldDateTime)
        {
            using (var context = new HmsDbContext())
            {
                context.EgmMeterReadings.RemoveRange(
                    context.EgmMeterReadings.Where(
                        mr => mr.ReportedAt != DaoUtilities.UnsentData && mr.ReportedAt < oldDateTime));
                DaoUtilities.SaveToDbWithRetry(context, DaoUtilities.SaveType.DeleteExistingEntity,
                    ResolveEntityUpdateConflict);
            }
        }

        /// <summary>
        /// Saves the specified egm meter reading.
        /// </summary>
        /// <param name="egmMeterReading">The egm meter reading.</param>
        public void Save(EgmMeterReading egmMeterReading)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                if (!context.EgmMeterReadings.Any(mr => mr.Id == egmMeterReading.Id))
                {
                    // no matching PK for this EgmMeterReading in database,
                    // thus we create new entity and add it to db
                    DaoUtilities.SaveCreatedEntity(context, context.EgmMeterReadings, egmMeterReading,
                        SetNewEntityState);
                }
                else
                {
                    // matching PK found, thus we update state of existing EgmMeterReading entity
                    DaoUtilities.SaveUpdatedEntity(context, context.EgmMeterReadings, egmMeterReading,
                        UpdateExistingEntityState);
                }
            }
        }

        /// <summary>
        /// Saves the specified egm meter readings.
        /// </summary>
        /// <param name="egmMeterReadings">The egm meter readings.</param>
        public void Save(ICollection<EgmMeterReading> egmMeterReadings)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                foreach (var egmMeterReading in egmMeterReadings)
                {
                    if (!context.EgmMeterReadings.Any(mr => mr.Id == egmMeterReading.Id))
                    {
                        // no matching PK for this EgmMeterReading in database,
                        // thus we create new entity and add it to db
                        DaoUtilities.SaveCreatedEntity(context, context.EgmMeterReadings, egmMeterReading,
                            SetNewEntityState, false);
                    }
                    else
                    {
                        // matching PK found, thus we update state of existing EgmMeterReading entity
                        DaoUtilities.SaveUpdatedEntity(context, context.EgmMeterReadings, egmMeterReading,
                            UpdateExistingEntityState, false);
                    }
                }

                DaoUtilities.SaveToDbWithRetry(context, DaoUtilities.SaveType.UpdateExistingEntity,
                    ResolveEntityUpdateConflict);
            }
        }

        /// <summary>
        /// Deletes the specified egm meter reading entity.
        /// </summary>
        /// <param name="egmMeterReading">The egm meter reading.</param>
        public void Delete(EgmMeterReading egmMeterReading)
        {
            using (var context = new HmsDbContext())
            {
                if (!context.EgmMeterReadings.Any(mr => mr.Id == egmMeterReading.Id))
                {
                    return;
                }

                // matching PK found, thus we proceed with Delete
                DaoUtilities.DeleteEntity(context, context.EgmMeterReadings, egmMeterReading);
            }
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Generates the hash for an EgmMeterReading.
        /// </summary>
        /// <param name="egmMeterReading">The egm meter reading.</param>
        /// <returns>System.String.</returns>
        public static string GenerateHash(EgmMeterReading egmMeterReading)
        {
            return GenerateHash(egmMeterReading.Type.ToString(), egmMeterReading.Value, egmMeterReading.EgmSerialNumber,
                egmMeterReading.Units, egmMeterReading.ReadAt, egmMeterReading.CasinoCode, egmMeterReading.GameTitle,
                egmMeterReading.GameDenomination);
        }

        /// <summary>
        /// Generates the hash.
        /// </summary>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="casinoCode">The casino code.</param>
        /// <param name="egmMeterReading">The egm meter reading.</param>
        /// <returns>System.String.</returns>
        public static string GenerateHash(string egmSerialNumber, string casinoCode, IMeterData egmMeterReading)
        {
            return GenerateHash(egmMeterReading.Type.ToString(), egmMeterReading.Value, egmSerialNumber,
                egmMeterReading.Units, egmMeterReading.ReadAt, casinoCode, egmMeterReading.GameTitle,
                egmMeterReading.GameDenomination);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Generates the hash.
        /// </summary>
        /// <param name="meterType">Type of the meter.</param>
        /// <param name="value">The value.</param>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="units">The units.</param>
        /// <param name="readAt">The read at.</param>
        /// <param name="casinoCode">The casino code.</param>
        /// <param name="gameTitle">The game title.</param>
        /// <param name="gameDenomination">The game denomination.</param>
        /// <returns>System.String.</returns>
        private static string GenerateHash(string meterType, long value, string egmSerialNumber, string units,
            DateTime readAt, string casinoCode, string gameTitle, long gameDenomination)
        {
            var md5 = MD5.Create();

            var stringToHash = meterType + value + egmSerialNumber + units + readAt + casinoCode + gameTitle +
                               gameDenomination + Settings.Default.MeterEntityHashKey;

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
        /// Sets the state of the new entity.
        /// </summary>
        /// <param name="entity">The egm meter reading.</param>
        private static void SetNewEntityState(EgmMeterReading entity)
        {
            entity.Version = 0;
            entity.Hash = GenerateHash(entity);
        }

        /// <summary>
        /// Updates the state of the existing entity.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityEntry">The meter reading entity entry.</param>
        /// <param name="egmMeterReading">The egm meter reading.</param>
        private static void UpdateExistingEntityState(HmsDbContext context, DbEntityEntry entityEntry,
            EgmMeterReading egmMeterReading)
        {
            var meterReadingEntity = (EgmMeterReading) entityEntry.Entity;

            if (!meterReadingEntity.SentAt.Equals(egmMeterReading.SentAt))
            {
                meterReadingEntity.SentAt = egmMeterReading.SentAt;
            }

            if (!meterReadingEntity.ReportGuid.Equals(egmMeterReading.ReportGuid))
            {
                meterReadingEntity.ReportGuid = egmMeterReading.ReportGuid;
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
            // This should never occur for EgmMeterReading entities
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