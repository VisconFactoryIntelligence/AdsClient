/*  Copyright (c) 2011 Inando
 
    Permission is hereby granted, free of charge, to any person obtaining a 
    copy of this software and associated documentation files (the "Software"), 
    to deal in the Software without restriction, including without limitation 
    the rights to use, copy, modify, merge, publish, distribute, sublicense, 
    and/or sell copies of the Software, and to permit persons to whom the 
    Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included 
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
    DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
    OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */

using System;
using System.Reflection;

namespace Ads.Client
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