using System;
using System.Reflection;

namespace Viscon.Communication.Ads.Helpers
{
    /// <summary>
    /// An attribute class for members of serializable classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AdsAttribute : Attribute
    {
        /// <summary>
        /// The byte size.
        /// </summary>
        public uint ByteSize;

        /// <summary>
        /// The order.
        /// </summary>
        public uint Order;

        /// <summary>
        /// The array size.
        /// </summary>
        public uint ArraySize;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AdsAttribute()
        {
            ByteSize = 0;
            Order = 99;
            ArraySize = 1;
        }

		private MemberInfo member;
        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        internal MemberInfo Member
        {
            get { return member; }
            set { member = value; }
        }
    }
}