// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 04-26-2017
//
// Last Modified By : acscheiner
// Last Modified On : 04-26-2017
// ***********************************************************************
// <copyright file="EgmWindowsEventDao.cs" company="Castle Hill Gaming, LLC">
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
    /// Class EgmWindowsEventDao.
    /// </summary>
    public class EgmWindowsEventDao
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Public CRUD methods

        /// <summary>
        /// Gets all EgmWindowsEvent Entities.
        /// </summary>
        /// <returns>ICollection&lt;EgmWindowsEvent&gt;.</returns>
        public ICollection<EgmWindowsEvent> GetAll()
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmWindowsEvents.AsQueryable()
                        .OrderBy(winEvt => winEvt.CasinoCode)
                        .ThenBy(winEvt => winEvt.EgmSerialNumber)
                        .ThenBy(winEvt => winEvt.EgmAssetNumber)
                        .ThenBy(winEvt => winEvt.ReportedAt)
                        .ThenBy(winEvt => winEvt.OccurredAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets the EgmWindowsEvent entity with the specified ID/PK.
        /// </summary>
        /// <param name="windowsEventId">The windows event identifier.</param>
        /// <returns>EgmWindowsEvent.</returns>
        public EgmWindowsEvent GetById(long windowsEventId)
        {
            using (var context = new HmsDbContext())
            {
                return context.EgmWindowsEvents.Find(windowsEventId);
            }
        }

        /// <summary>
        /// Gets all unreported EgmWindowsEvent entities.
        /// </summary>
        /// <returns>ICollection&lt;EgmWindowsEvent&gt;.</returns>
        public ICollection<EgmWindowsEvent> GetUnreported()
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmWindowsEvents.AsQueryable()
                        .Where(winEvt => winEvt.ReportGuid == Guid.Empty)
                        .OrderBy(winEvt => winEvt.CasinoCode)
                        .ThenBy(winEvt => winEvt.EgmSerialNumber)
                        .ThenBy(winEvt => winEvt.EgmAssetNumber)
                        .ThenBy(winEvt => winEvt.ReportedAt)
                        .ThenBy(winEvt => winEvt.OccurredAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets all unsent EgmWindowsEvent entities.
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmWindowsEvents which have been reported.</param>
        /// <param name="maxRecords">The maximum records to return from a query.</param>
        /// <returns>ICollection&lt;EgmWindowsEvent&gt;.</returns>
        public ICollection<EgmWindowsEvent> GetUnsent(bool reported = false,
            int maxRecords = DaoUtilities.NoRecordLimit)
        {
            using (var context = new HmsDbContext())
            {
                var query =
                    context.EgmWindowsEvents.AsQueryable().Where(winEvt => winEvt.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(winEvt => winEvt.ReportGuid != Guid.Empty);
                }

                query = query.OrderBy(winEvt => winEvt.CasinoCode)
                    .ThenBy(winEvt => winEvt.EgmSerialNumber)
                    .ThenBy(winEvt => winEvt.EgmAssetNumber)
                    .ThenBy(winEvt => winEvt.ReportedAt)
                    .ThenBy(winEvt => winEvt.OccurredAt);

                if (DaoUtilities.NoRecordLimit != maxRecords && 0 < maxRecords)
                {
                    query = query.Take(maxRecords);
                }

                return query.ToList();
            }
        }

        /// <summary>
        /// Gets the Number of unsent EgmWindowsEvents.
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmWindowsEvents which have been reported.</param>
        /// <returns>The number of unsent EgmWindowsEvents</returns>
        public int NumUnsent(bool reported = false)
        {
            using (var context = new HmsDbContext())
            {
                var query =
                    context.EgmWindowsEvents.AsQueryable().Where(winEvt => winEvt.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(winEvt => winEvt.ReportGuid != Guid.Empty);
                }

                return query.Count();
            }
        }

        /// <summary>
        /// Are there any unsent EgmWindowsEvent Entities?
        /// </summary>
        /// <param name="reported">if set to <c>true</c> includes only EgmWindowsEvents which have been reported.</param>
        /// <returns><c>true</c> if there are any unsent EgmWindowsEvents, <c>false</c> otherwise.</returns>
        public bool AreUnsent(bool reported = false)
        {
            using (var context = new HmsDbContext())
            {
                var query =
                    context.EgmWindowsEvents.AsQueryable().Where(winEvt => winEvt.SentAt == DaoUtilities.UnsentData);
                if (reported)
                {
                    query = query.Where(winEvt => winEvt.ReportGuid != Guid.Empty);
                }

                return query.Any();
            }
        }

        /// <summary>
        /// Gets all EgmWindowsEvent entities with the specified ReportGuid.
        /// </summary>
        /// <param name="reportGuid">The report unique identifier.</param>
        /// <returns>ICollection&lt;EgmWindowsEvent&gt;.</returns>
        public ICollection<EgmWindowsEvent> GetByReportGuid(Guid reportGuid)
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmWindowsEvents.AsQueryable()
                        .Where(winEvt => winEvt.ReportGuid == reportGuid)
                        .OrderBy(winEvt => winEvt.CasinoCode)
                        .ThenBy(winEvt => winEvt.EgmSerialNumber)
                        .ThenBy(winEvt => winEvt.EgmAssetNumber)
                        .ThenBy(winEvt => winEvt.ReportedAt)
                        .ThenBy(winEvt => winEvt.OccurredAt)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets EgmWindowsEvent Entities older than specified date-time.
        /// </summary>
        /// <param name="oldDateTime">The time to use for the older-than comparison.</param>
        /// <returns>ICollection&lt;EgmWindowsEvent&gt;.</returns>
        public ICollection<EgmWindowsEvent> GetOlderThan(DateTime oldDateTime)
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.EgmWindowsEvents.AsQueryable()
                        .Where(winEvt =>
                            winEvt.ReportedAt != DaoUtilities.UnsentData && winEvt.ReportedAt < oldDateTime)
                        .ToList();
            }
        }

        /// <summary>
        /// Cleans EgmWindowsEvent Entities older than specified date-time.
        /// </summary>
        /// <param name="oldDateTime">The old date time.</param>
        public void CleanOlderThan(DateTime oldDateTime)
        {
            using (var context = new HmsDbContext())
            {
                context.EgmWindowsEvents.RemoveRange(
                    context.EgmWindowsEvents.Where(
                        winEvt => winEvt.ReportedAt != DaoUtilities.UnsentData && winEvt.ReportedAt < oldDateTime));
                DaoUtilities.SaveToDbWithRetry(context, DaoUtilities.SaveType.DeleteExistingEntity,
                    ResolveEntityUpdateConflict);
            }
        }

        /// <summary>
        /// Saves the specified egm windows event entity.
        /// </summary>
        /// <param name="egmWindowsEvent">The egm windows event entity.</param>
        public void Save(EgmWindowsEvent egmWindowsEvent)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                if (!context.EgmWindowsEvents.Any(winEvt => winEvt.Id == egmWindowsEvent.Id))
                {
                    // no matching PK for this EgmWindowsEvent in database,
                    // thus we create new entity and add it to db
                    DaoUtilities.SaveCreatedEntity(context, context.EgmWindowsEvents, egmWindowsEvent,
                        SetNewEntityState);
                }
                else
                {
                    // matching PK found, thus we update state of existing EgmWindowsEvent entity
                    DaoUtilities.SaveUpdatedEntity(context, context.EgmWindowsEvents, egmWindowsEvent,
                        UpdateExistingEntityState);
                }
            }
        }

        public void Save(ICollection<EgmWindowsEvent> egmWindowsEvents)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                foreach (var egmWindowsEvent in egmWindowsEvents)
                {
                    if (!context.EgmWindowsEvents.Any(wevt => wevt.Id == egmWindowsEvent.Id))
                    {
                        // no matching PK for this EgmWindowsEvent in database,
                        // thus we create new entity and add it to db
                        DaoUtilities.SaveCreatedEntity(context, context.EgmWindowsEvents, egmWindowsEvent,
                            SetNewEntityState, false);
                    }
                    else
                    {
                        // matching PK found, thus we update state of existing EgmWindowsEvent entity
                        DaoUtilities.SaveUpdatedEntity(context, context.EgmWindowsEvents, egmWindowsEvent,
                            UpdateExistingEntityState, false);
                    }
                }

                DaoUtilities.SaveToDbWithRetry(context, DaoUtilities.SaveType.UpdateExistingEntity,
                    ResolveEntityUpdateConflict);
            }
        }

        /// <summary>
        /// Deletes the specified egm windows event entity.
        /// </summary>
        /// <param name="egmWindowsEvent">The egm windows event entity.</param>
        public void Delete(EgmWindowsEvent egmWindowsEvent)
        {
            using (var context = new HmsDbContext())
            {
                if (!context.EgmWindowsEvents.Any(winEvt => winEvt.Id == egmWindowsEvent.Id))
                {
                    return;
                }

                // matching PK found, thus we proceed with Delete
                DaoUtilities.DeleteEntity(context, context.EgmWindowsEvents, egmWindowsEvent);
            }
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Generates the hash for the specified EgmWindowsEvent entity.
        /// </summary>
        /// <param name="egmWindowsEvent">The egm windows event entity.</param>
        /// <returns>System.String.</returns>
        public static string GenerateHash(EgmWindowsEvent egmWindowsEvent)
        {
            return GenerateHash(egmWindowsEvent.Code, egmWindowsEvent.Description, egmWindowsEvent.EgmSerialNumber,
                egmWindowsEvent.OccurredAt, egmWindowsEvent.CasinoCode, egmWindowsEvent.EventLogName);
        }

        /// <summary>
        /// Generates the hash.
        /// </summary>
        /// <param name="egmSerialNumber">The egm serial number.</param>
        /// <param name="casinoCode">The casino code.</param>
        /// <param name="egmWindowsEvent">The egm windows event.</param>
        /// <returns>System.String.</returns>
        public static string GenerateHash(string egmSerialNumber, string casinoCode, IWindowsEventData egmWindowsEvent)
        {
            return GenerateHash(egmWindowsEvent.Code, egmWindowsEvent.Description, egmSerialNumber,
                egmWindowsEvent.OccurredAt, casinoCode, egmWindowsEvent.EventLogName);
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
        /// <param name="eventLogName">Name of the event log.</param>
        /// <returns>System.String.</returns>
        private static string GenerateHash(int code, string description, string egmSerialNumber, DateTime occurredAt,
            string casinoCode, string eventLogName)
        {
            var md5 = MD5.Create();

            var stringToHash = code + description + egmSerialNumber + occurredAt + casinoCode + eventLogName +
                               Settings.Default.WindowsEventEntityHashKey;

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
        private static void SetNewEntityState(EgmWindowsEvent entity)
        {
            entity.Version = 0;
            entity.Hash = GenerateHash(entity);
        }

        /// <summary>
        /// Updates the state of the existing entity.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="egmWindowsEvent">The egm windows event entity.</param>
        private static void UpdateExistingEntityState(HmsDbContext context, DbEntityEntry entityEntry,
            EgmWindowsEvent egmWindowsEvent)
        {
            var winEvtEntity = (EgmWindowsEvent) entityEntry.Entity;
            if (!winEvtEntity.SentAt.Equals(egmWindowsEvent.SentAt))
            {
                winEvtEntity.SentAt = egmWindowsEvent.SentAt;
            }

            if (!winEvtEntity.ReportGuid.Equals(egmWindowsEvent.ReportGuid))
            {
                winEvtEntity.ReportGuid = egmWindowsEvent.ReportGuid;
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
            // This should never occur for EgmWindowsEvent entities
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