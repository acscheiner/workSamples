// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 10-10-2016
//
// Last Modified By : acscheiner
// Last Modified On : 10-25-2016
// ***********************************************************************
// <copyright file="EgmEventDao.cs" company="">
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
    /// Class EgmEventDao.
    /// </summary>
    public class EgmEventDao
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Public CRUD methods

        /// <summary>
        /// Gets all EgmEvent Entities.
        /// </summary>
        /// <returns>ICollection&lt;EgmEvent&gt;.</returns>
        public ICollection<EgmEvent> GetAll()
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmEvents.AsQueryable()
                        .OrderBy(evt => evt.CasinoCode)
                        .ThenBy(evt => evt.EgmSerialNumber)
                        .ThenBy(evt => evt.EgmAssetNumber)
                        .ThenBy(evt => evt.ReportedAt)
                        .ThenBy(evt => evt.OccurredAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets the EgmEvent entity with the specified ID/PK.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <returns>EgmEvent.</returns>
        public EgmEvent GetById(long eventId)
        {
            using (var context = new HmsDbContext())
            {
                return context.EgmEvents.Find(eventId);
            }
        }

        /// <summary>
        /// Gets all unreported EgmEvent entities.
        /// </summary>
        /// <returns>ICollection&lt;EgmEvent&gt;.</returns>
        public ICollection<EgmEvent> GetUnreported()
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmEvents.AsQueryable()
                        .Where(evt => evt.ReportGuid == Guid.Empty)
                        .OrderBy(evt => evt.CasinoCode)
                        .ThenBy(evt => evt.EgmSerialNumber)
                        .ThenBy(evt => evt.EgmAssetNumber)
                        .ThenBy(evt => evt.ReportedAt)
                        .ThenBy(evt => evt.OccurredAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets all unsent EgmEvent entities.
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmEvents which have been reported.</param>
        /// <param name="maxRecords">The maximum records to return from a query.</param>
        /// <returns>ICollection&lt;EgmEvent&gt;.</returns>
        public ICollection<EgmEvent> GetUnsent(bool reported = false, int maxRecords = DaoUtilities.NoRecordLimit)
        {
            using (var context = new HmsDbContext())
            {
                var query = context.EgmEvents.AsQueryable().Where(evt => evt.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(evt => evt.ReportGuid != Guid.Empty);
                }

                query = query.OrderBy(evt => evt.CasinoCode)
                    .ThenBy(evt => evt.EgmSerialNumber)
                    .ThenBy(evt => evt.EgmAssetNumber)
                    .ThenBy(evt => evt.ReportedAt)
                    .ThenBy(evt => evt.OccurredAt);

                if (DaoUtilities.NoRecordLimit != maxRecords && 0 < maxRecords)
                {
                    query = query.Take(maxRecords);
                }

                return query.ToList();
            }
        }

        /// <summary>
        /// Gets the Number of unsent EgmEvents.
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmEvents which have been reported.</param>
        /// <returns>The number of unsent EgmEvents</returns>
        public int NumUnsent(bool reported = false)
        {
            using (var context = new HmsDbContext())
            {
                var query = context.EgmEvents.AsQueryable().Where(evt => evt.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(evt => evt.ReportGuid != Guid.Empty);
                }

                return query.Count();
            }
        }

        /// <summary>
        /// Are there any unsent EgmEvents?
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmEvents which have been reported.</param>
        /// <returns><c>true</c> if there are any unsent EgmEvents, <c>false</c> otherwise.</returns>
        public bool AreUnsent(bool reported = false)
        {
            using (var context = new HmsDbContext())
            {
                var query = context.EgmEvents.AsQueryable().Where(evt => evt.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(evt => evt.ReportGuid != Guid.Empty);
                }

                return query.Any();
            }
        }

        /// <summary>
        /// Gets all EgmEvent entities with the specified ReportGuid.
        /// </summary>
        /// <param name="reportGuid">The report unique identifier.</param>
        /// <returns>ICollection&lt;EgmEvent&gt;.</returns>
        public ICollection<EgmEvent> GetByReportGuid(Guid reportGuid)
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmEvents.AsQueryable()
                        .Where(evt => evt.ReportGuid == reportGuid)
                        .OrderBy(evt => evt.CasinoCode)
                        .ThenBy(evt => evt.EgmSerialNumber)
                        .ThenBy(evt => evt.EgmAssetNumber)
                        .ThenBy(evt => evt.ReportedAt)
                        .ThenBy(evt => evt.OccurredAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets EgmEvent Entities older than specified date-time.
        /// </summary>
        /// <param name="oldDateTime">The time to use for the older-than comparison.</param>
        /// <returns>ICollection&lt;EgmEvent&gt;.</returns>
        public ICollection<EgmEvent> GetOlderThan(DateTime oldDateTime)
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmEvents.AsQueryable()
                        .Where(evt => evt.ReportedAt != DaoUtilities.UnsentData && evt.ReportedAt < oldDateTime)
                        .ToList();
            }
        }

        /// <summary>
        /// Cleans EgmEvent Entities older than specified date-time.
        /// </summary>
        /// <param name="oldDateTime">The old date time.</param>
        public void CleanOlderThan(DateTime oldDateTime)
        {
            using (var context = new HmsDbContext())
            {
                context.EgmEvents.RemoveRange(
                    context.EgmEvents.Where(
                        evt => evt.ReportedAt != DaoUtilities.UnsentData && evt.ReportedAt < oldDateTime));
                DaoUtilities.SaveToDbWithRetry(context, DaoUtilities.SaveType.DeleteExistingEntity,
                    ResolveEntityUpdateConflict);
            }
        }

        /// <summary>
        /// Saves the specified egm event.
        /// </summary>
        /// <param name="egmEvent">The egm event.</param>
        public void Save(EgmEvent egmEvent)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                if (!context.EgmEvents.Any(evt => evt.Id == egmEvent.Id))
                {
                    // no matching PK for this EgmEvent in database,
                    // thus we create new entity and add it to db
                    DaoUtilities.SaveCreatedEntity(context, context.EgmEvents, egmEvent, SetNewEntityState);
                }
                else
                {
                    // matching PK found, thus we update state of existing EgmEvent entity
                    DaoUtilities.SaveUpdatedEntity(context, context.EgmEvents, egmEvent, UpdateExistingEntityState);
                }
            }
        }

        /// <summary>
        /// Saves the specified egm events.
        /// </summary>
        /// <param name="egmEvents">The egm events.</param>
        public void Save(ICollection<EgmEvent> egmEvents)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                foreach (var egmEvent in egmEvents)
                {
                    if (!context.EgmEvents.Any(evt => evt.Id == egmEvent.Id))
                    {
                        // no matching PK for this EgmEvent in database,
                        // thus we create new entity and add it to db
                        DaoUtilities.SaveCreatedEntity(context, context.EgmEvents, egmEvent,
                            SetNewEntityState, false);
                    }
                    else
                    {
                        // matching PK found, thus we update state of existing EgmEvent entity
                        DaoUtilities.SaveUpdatedEntity(context, context.EgmEvents, egmEvent,
                            UpdateExistingEntityState, false);
                    }
                }

                DaoUtilities.SaveToDbWithRetry(context, DaoUtilities.SaveType.UpdateExistingEntity,
                    ResolveEntityUpdateConflict);
            }
        }

        /// <summary>
        /// Deletes the specified egm event entity.
        /// </summary>
        /// <param name="egmEvent">The egm event.</param>
        public void Delete(EgmEvent egmEvent)
        {
            using (var context = new HmsDbContext())
            {
                if (!context.EgmEvents.Any(evt => evt.Id == egmEvent.Id))
                {
                    return;
                }

                // matching PK found, thus we proceed with Delete
                DaoUtilities.DeleteEntity(context, context.EgmEvents, egmEvent);
            }
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Generates the hash for the specified EgmEvent entity.
        /// </summary>
        /// <param name="egmEvent">The egm event.</param>
        /// <returns>System.String.</returns>
        public static string GenerateHash(EgmEvent egmEvent)
        {
            return GenerateHash(egmEvent.Code, egmEvent.Description, egmEvent.EgmSerialNumber, egmEvent.OccurredAt,
                egmEvent.CasinoCode);
        }

        /// <summary>
        /// Generates the hash.
        /// </summary>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="casinoCode">The casino code.</param>
        /// <param name="egmEvent">The egm event.</param>
        /// <returns>System.String.</returns>
        public static string GenerateHash(string egmSerialNumber, string casinoCode, IEventData egmEvent)
        {
            return GenerateHash(egmEvent.Code, egmEvent.Description, egmSerialNumber, egmEvent.OccurredAt, casinoCode);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Generates the hash.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="description">The description.</param>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="occurredAt">The occurred at.</param>
        /// <param name="casinoCode">The casino code.</param>
        /// <returns>System.String.</returns>
        private static string GenerateHash(int code, string description, string egmSerialNumber, DateTime occurredAt, string casinoCode)
        {
            var md5 = MD5.Create();

            var stringToHash = code + description + egmSerialNumber + occurredAt + casinoCode + Settings.Default.EventEntityHashKey;

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
        /// <param name="entity">The egm event.</param>
        private static void SetNewEntityState(EgmEvent entity)
        {
            entity.Version = 0;
            entity.Hash = GenerateHash(entity);
        }

        /// <summary>
        /// Updates the state of the existing entity.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityEntry">The EgmEvent entity entry.</param>
        /// <param name="egmEvent">The egm event.</param>
        private static void UpdateExistingEntityState(HmsDbContext context, DbEntityEntry entityEntry,
            EgmEvent egmEvent)
        {
            var evtEntity = (EgmEvent) entityEntry.Entity;
            if (!evtEntity.SentAt.Equals(egmEvent.SentAt))
            {
                evtEntity.SentAt = egmEvent.SentAt;
            }

            if (!evtEntity.ReportGuid.Equals(egmEvent.ReportGuid))
            {
                evtEntity.ReportGuid = egmEvent.ReportGuid;
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
            // This should never occur for EgmEvent entities
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