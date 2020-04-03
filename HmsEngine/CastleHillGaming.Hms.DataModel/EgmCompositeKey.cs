// ***********************************************************************
// Assembly         : CastleHillGaming.Hms.DataModel
// Author           : acscheiner
// Created          : 02-23-2017
//
// Last Modified By : acscheiner
// Last Modified On : 02-23-2017
// ***********************************************************************
// <copyright file="EgmCompositeKey.cs" company="Castle Hill Gaming, LLC">
//     Castle Hill Gaming, LLC Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CastleHillGaming.Hms.DataModel
{
    #region

    using System;

    #endregion

    /// <summary>
    /// Class EgmCompositeKey.
    /// </summary>
    /// <seealso cref="System.IEquatable{CastleHillGaming.Hms.DataModel.EgmCompositeKey}" />
    public class EgmCompositeKey : IEquatable<EgmCompositeKey>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the audit date.
        /// </summary>
        /// <value>The audit date.</value>
        public DateTime AuditDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the casino.
        /// </summary>
        /// <value>The name of the casino.</value>
        public string CasinoName { get; set; }

        /// <summary>
        /// Gets or sets the egm serial number.
        /// </summary>
        /// <value>The egm serial number.</value>
        public string EgmSerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the egm asset number.
        /// </summary>
        /// <value>The egm asset number.</value>
        public string EgmAssetNumber { get; set; }

        /// <summary>
        /// Gets or sets the game theme.
        /// </summary>
        /// <value>The game theme.</value>
        public string GameTheme { get; set; }

        /// <summary>
        /// Gets or sets the denomination.
        /// </summary>
        /// <value>The denomination.</value>
        public long Denomination { get; set; }

        #endregion

        #region IEquatable Implementation

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(EgmCompositeKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AuditDate.Equals(other.AuditDate) && string.Equals(CasinoName, other.CasinoName) &&
                   string.Equals(EgmSerialNumber, other.EgmSerialNumber) &&
                   string.Equals(EgmAssetNumber, other.EgmAssetNumber) && string.Equals(GameTheme, other.GameTheme) &&
                   Denomination == other.Denomination;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(EgmCompositeKey) && Equals((EgmCompositeKey) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = AuditDate.GetHashCode();
                hashCode = (hashCode * 397) ^ CasinoName.GetHashCode();
                hashCode = (hashCode * 397) ^ EgmSerialNumber.GetHashCode();
                hashCode = (hashCode * 397) ^ EgmAssetNumber.GetHashCode();
                hashCode = (hashCode * 397) ^ GameTheme.GetHashCode();
                hashCode = (hashCode * 397) ^ Denomination.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        public override string ToString()
        {
            return
                $"{nameof(AuditDate)}: {AuditDate}, {nameof(CasinoName)}: {CasinoName}, {nameof(EgmSerialNumber)}: {EgmSerialNumber}, {nameof(EgmAssetNumber)}: {EgmAssetNumber}, {nameof(GameTheme)}: {GameTheme}, {nameof(Denomination)}: {Denomination}";
        }
    }
}