// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 04-21-2017
//
// Last Modified By : acscheiner
// Last Modified On : 04-21-2017
// ***********************************************************************
// <copyright file="EntityBase.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel
{
    #region

    using System.ComponentModel;

    #endregion

    /// <summary>
    /// Class EntityBase.
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// Gets or sets the identifier (PK for entity database record).
        /// </summary>
        /// <value>The identifier (PK for entity database record).</value>
        [Browsable(false)]
        public long Id { get; protected set; }

        /// <summary>
        /// Gets or sets the version of corresponding entity record in data store
        /// (used for detecting concurrency conflicts with Entity Framework).
        /// </summary>
        /// <value>The version.</value>
        [Browsable(false)]
        public int Version { get; set; }
    }
}