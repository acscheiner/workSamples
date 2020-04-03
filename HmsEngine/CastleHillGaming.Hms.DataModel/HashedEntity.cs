// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 04-21-2017
//
// Last Modified By : acscheiner
// Last Modified On : 04-21-2017
// ***********************************************************************
// <copyright file="HashedEntity.cs" company="Castle Hill Gaming, LLC">
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
    /// Class HashedEntity.
    /// </summary>
    /// <seealso cref="CastleHillGaming.Hms.DataModel.EntityBase" />
    public abstract class HashedEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>The hash.</value>
        [Browsable(false)]
        public string Hash { get; set; }
    }
}