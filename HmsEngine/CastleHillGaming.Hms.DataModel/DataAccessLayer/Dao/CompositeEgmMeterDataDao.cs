// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 02-22-2017
//
// Last Modified By : acscheiner
// Last Modified On : 02-22-2017
// ***********************************************************************
// <copyright file="CompositeEgmMeterDataDao.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel.DataAccessLayer.Dao
{
    #region

    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;
    using CastleHillGaming.Hms.DataModel.DataAccessLayer.DbContext;
    using CastleHillGaming.Hms.Interfaces;
    using log4net;

    #endregion

    /// <summary>
    /// Class CompositeEgmMeterDataDao.
    /// </summary>
    public class CompositeEgmMeterDataDao
    {
        #region Static and Constant Class Members

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public CRUD methods

        /// <summary>
        /// Gets all CompositeEgmMeterData Entities.
        /// </summary>
        /// <returns>ICollection&lt;CompositeEgmMeterData&gt;.</returns>
        public ICollection<CompositeEgmMeterData> GetAll()
        {
            using (var context = new HmsDbContext())
            {
                return
                    context.CompositeEgmMeterDatas.AsQueryable()
                        .OrderBy(cemd => cemd.CasinoName)
                        .ThenBy(cemd => cemd.SerialNumber)
                        .ThenBy(cemd => cemd.AssetNumber)
                        .ThenBy(cemd => cemd.GameTheme)
                        .ThenBy(cemd => cemd.Denomination)
                        .ThenBy(cemd => cemd.AuditDate)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets the CompositeEgmMeterData entity with the specified ID/PK.
        /// </summary>
        /// <param name="compositeMeterDataId">The composite EGM meter data identifier.</param>
        /// <returns>CompositeEgmMeterData.</returns>
        public CompositeEgmMeterData GetById(long compositeMeterDataId)
        {
            using (var context = new HmsDbContext())
            {
                return context.CompositeEgmMeterDatas.Find(compositeMeterDataId);
            }
        }

        /// <summary>
        /// Gets the CompositeEgmMeterData with the specified composite key.
        /// </summary>
        /// <param name="compositeKey">The composite key.</param>
        /// <returns>CompositeEgmMeterData.</returns>
        public CompositeEgmMeterData GetByCompositeKey(EgmCompositeKey compositeKey)
        {
            using (var context = new HmsDbContext())
            {
                CompositeEgmMeterData retval = null;

                var candidates = context.CompositeEgmMeterDatas.AsQueryable()
                    .Where(cemd => cemd.AuditDate == compositeKey.AuditDate)
                    .Where(cemd => cemd.CasinoName == compositeKey.CasinoName)
                    .Where(cemd => cemd.SerialNumber == compositeKey.EgmSerialNumber)
                    .Where(cemd => cemd.AssetNumber == compositeKey.EgmAssetNumber)
                    .Where(cemd => cemd.GameTheme == compositeKey.GameTheme)
                    .Where(cemd => cemd.Denomination == compositeKey.Denomination)
                    .OrderBy(cemd => cemd.Id)
                    .ToList();

                // we expect only 1 match to this query
                // if we find more than 1, something is amiss
                if (1 == candidates.Count)
                {
                    retval = candidates[0];
                }
                else if (1 < candidates.Count)
                {
                    Logger.Warn(
                        $"CompositeEgmMeterDataDao.GetByCompositeKey found {candidates.Count} matches with a composite key of {compositeKey}. Should only ever be 1 record which matches a composite key.");
                    retval = candidates[0];
                    //for (var iCandidate = 1; iCandidate < candidates.Count; ++iCandidate)
                    //{
                    //    DaoUtilities.DeleteEntity(context, context.CompositeEgmMeterDatas, candidates[iCandidate]);
                    //}
                }

                return retval;
            }
        }

        /// <summary>
        /// Saves the specified composite EGM meter data.
        /// </summary>
        /// <param name="compositeEgmMeterData">The composite EGM meter data.</param>
        public void Save(CompositeEgmMeterData compositeEgmMeterData)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                if (!context.CompositeEgmMeterDatas.Any(cemd => cemd.Id == compositeEgmMeterData.Id))
                {
                    // no matching PK for this CompositeEgmMeterData in database,
                    // thus we create new entity and add it to db
                    DaoUtilities.SaveCreatedEntity(context, context.CompositeEgmMeterDatas, compositeEgmMeterData,
                        SetNewEntityState);
                }
                else
                {
                    // matching PK found, thus we update state of existing CompositeEgmMeterData entity
                    DaoUtilities.SaveUpdatedEntity(context, context.CompositeEgmMeterDatas, compositeEgmMeterData,
                        UpdateExistingEntityState);
                }
            }
        }

        /// <summary>
        /// Saves the specified composite egm meter datas.
        /// </summary>
        /// <param name="compositeEgmMeterDatas">The composite egm meter datas.</param>
        public void Save(ICollection<CompositeEgmMeterData> compositeEgmMeterDatas)
        {
            using (var context = new HmsDbContext())
            {
                //context.Database.Log = Console.Write;

                foreach (var compositeEgmMeterData in compositeEgmMeterDatas)
                {
                    if (!context.CompositeEgmMeterDatas.Any(cemd => cemd.Id == compositeEgmMeterData.Id))
                    {
                        // no matching PK for this CompositeEgmMeterData in database,
                        // thus we create new entity and add it to db
                        DaoUtilities.SaveCreatedEntity(context, context.CompositeEgmMeterDatas, compositeEgmMeterData,
                            SetNewEntityState, false);
                    }
                    else
                    {
                        // matching PK found, thus we update state of existing CompositeEgmMeterData entity
                        DaoUtilities.SaveUpdatedEntity(context, context.CompositeEgmMeterDatas, compositeEgmMeterData,
                            UpdateExistingEntityState, false);
                    }
                }

                DaoUtilities.SaveToDbWithRetry(context, DaoUtilities.SaveType.UpdateExistingEntity,
                    ResolveEntityUpdateConflict);
            }
        }

        /// <summary>
        /// Deletes the specified composite egm meter data.
        /// </summary>
        /// <param name="compositeEgmMeterData">The composite egm meter data.</param>
        public void Delete(CompositeEgmMeterData compositeEgmMeterData)
        {
            using (var context = new HmsDbContext())
            {
                if (!context.CompositeEgmMeterDatas.Any(cemd => cemd.Id == compositeEgmMeterData.Id))
                {
                    return;
                }

                // matching PK found, thus we proceed with Delete
                DaoUtilities.DeleteEntity(context, context.CompositeEgmMeterDatas, compositeEgmMeterData);
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Sets the new state of the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private static void SetNewEntityState(CompositeEgmMeterData entity)
        {
            entity.Version = 0;
        }

        /// <summary>
        /// Updates the state of the existing entity.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="compositeEgmMeterData">The composite egm meter data.</param>
        private static void UpdateExistingEntityState(HmsDbContext context, DbEntityEntry entityEntry,
            ICompositeEgmMeterData compositeEgmMeterData)
        {
            entityEntry.CurrentValues.SetValues(compositeEgmMeterData);
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
            // There is potential for DbConcurrencyUpdate optimistic locking conflicts
            // with CompositeEgmMeterData entities.
            //
            // Specifically, when multiple threads are consuming incoming meter reading
            // data on the HMS Cloud Service, there is the potential for more than one
            // such thread to have meter readings with the same EgmCompositeKey.
            // 
            // Thus, we need to sort out such potential conflicts here.
            //////////////////////////////////////////////////////////////////////////
            var currentValues = entityEntry.CurrentValues;
            var originalValues = entityEntry.OriginalValues;

            entityEntry.Reload();
            var dbValues = entityEntry.CurrentValues;

            var currentUpdates = new List<string>();
            var dbUpdates = new List<string>();

            var mutableMeters = new List<string>
            {
                nameof(CompositeEgmMeterData.CoinIn),
                nameof(CompositeEgmMeterData.CoinOut),
                nameof(CompositeEgmMeterData.BillDrop),
                nameof(CompositeEgmMeterData.GamesPlayed),
                nameof(CompositeEgmMeterData.GamesLost),
                nameof(CompositeEgmMeterData.GamesWon),
                nameof(CompositeEgmMeterData.Handpay),
                nameof(CompositeEgmMeterData.Jackpot),
                nameof(CompositeEgmMeterData.MeteredAttendantPaidProgressive),
                nameof(CompositeEgmMeterData.MeteredMachinePaidProgressive),
                nameof(CompositeEgmMeterData.TicketDrop),
                nameof(CompositeEgmMeterData.TicketOut)
            };

            // Look for updates to meters (relative to original values) in both
            // current changes and conflicting db updates
            foreach (var meter in mutableMeters)
            {
                if (currentValues.GetValue<long>(meter) != originalValues.GetValue<long>(meter))
                {
                    currentUpdates.Add(meter);
                }

                if (dbValues.GetValue<long>(meter) != originalValues.GetValue<long>(meter))
                {
                    dbUpdates.Add(meter);
                }
            }

            // Check if there are meters which were updated in both our current updates
            // and the conflicting db updates.
            foreach (var commonMeter in currentUpdates.Intersect(dbUpdates))
            {
                // since the mutable meters can only increase in value,
                // we take the larger value as winner
                var currentMeterValue = currentValues.GetValue<long>(commonMeter);
                var dbMeterValue = dbValues.GetValue<long>(commonMeter);
                entityEntry.CurrentValues[commonMeter] = currentMeterValue > dbMeterValue
                    ? currentMeterValue
                    : dbMeterValue;

                currentUpdates.Remove(commonMeter);
                dbUpdates.Remove(commonMeter);
            }

            // Take any current updates and apply to entity
            foreach (var meter in currentUpdates)
            {
                entityEntry.CurrentValues[meter] = currentValues.GetValue<long>(meter);
            }

            // Finally, we update the dependent meters (SlotRevenue and AverageBet)
            var coinIn = entityEntry.CurrentValues.GetValue<long>(nameof(CompositeEgmMeterData.CoinIn));
            var coinOut = entityEntry.CurrentValues.GetValue<long>(nameof(CompositeEgmMeterData.CoinOut));
            var gamesPlayed = entityEntry.CurrentValues.GetValue<long>(nameof(CompositeEgmMeterData.GamesPlayed));
            var jackpot = entityEntry.CurrentValues.GetValue<long>(nameof(CompositeEgmMeterData.Jackpot));

            if (CompositeEgmMeterData.MeterNotRecordedL != coinIn &&
                CompositeEgmMeterData.MeterNotRecordedL != gamesPlayed && 0 < gamesPlayed)
            {
                entityEntry.CurrentValues[nameof(CompositeEgmMeterData.AverageBet)] = (decimal) coinIn / gamesPlayed;
            }

            if (CompositeEgmMeterData.MeterNotRecordedL != coinIn &&
                CompositeEgmMeterData.MeterNotRecordedL != coinOut &&
                CompositeEgmMeterData.MeterNotRecordedL != jackpot)
            {
                entityEntry.CurrentValues[nameof(CompositeEgmMeterData.SlotRevenue)] = coinIn - coinOut - jackpot;
            }

            DaoUtilities.UpdateVersion(context, entityEntry);
        }

        #endregion
    }
}