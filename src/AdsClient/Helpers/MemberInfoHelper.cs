namespace Ads.Client.Helpers
{
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Extension classes for MemberInfo
    /// </summary>
    public static class MemberInfoHelper
    {
        private static object memberDictLock = new object();
        private static Dictionary<Type, List<MemberInfo>> memberDict = new Dictionary<Type, List<MemberInfo>>();

        private static object adsAttributeDictLock = new object();
        private static Dictionary<MemberInfo, AdsAttribute> adsAttributeDict = new Dictionary<MemberInfo, AdsAttribute>();

        private static object adsSerializableDictLock = new object();
        private static Dictionary<Type, bool> adsSerializableDict = new Dictionary<Type, bool>();

        /// <summary>
        /// Gets a flag, whether the type is ADS serializable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True, if the type is serializable.</returns>
        public static bool IsAdsSerializable(this Type type)
        {
            lock (adsSerializableDictLock)
            {
                if (!adsSerializableDict.ContainsKey(type))
                    adsSerializableDict.Add(type, type.GetTypeInfo().GetCustomAttributes().OfType<AdsSerializableAttribute>().Count() > 0);
                return adsSerializableDict[type];
            }
        }

        /// <summary>
        /// Gets the <see cref="AdsAttribute"/> that is associated to this member.
        /// </summary>
        /// <param name="info">The <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</param>
        /// <returns>The attached <see cref="AdsAttribute"/>, if available. Otherwise, null.</returns>
        public static AdsAttribute GetAdsAttribute(this MemberInfo info)
        {
            lock (adsAttributeDictLock)
            {
                if (!adsAttributeDict.ContainsKey(info))
                    adsAttributeDict.Add(info, info.GetAdsAttributeInternal());
                return adsAttributeDict[info];
            }
        }

        /// <summary>
        /// Gets the <see cref="AdsAttribute"/> that is associated to this member.
        /// </summary>
        /// <param name="info">The <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</param>
        /// <returns>The attached <see cref="AdsAttribute"/>, if available. Otherwise, null.</returns>
        private static AdsAttribute GetAdsAttributeInternal(this MemberInfo info)
        {
            var attributes = info.GetCustomAttributes(typeof(AdsAttribute), false);

            if ((attributes != null) && (attributes.Count() > 0))
                return (AdsAttribute)attributes.FirstOrDefault();
            else
                return null;
        }

        /// <summary>
        /// Get all public fields and properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>All public fields and properties.</returns>
        public static IEnumerable<MemberInfo> GetPublicFieldsAndProperties(this Type type)
        {
            lock (memberDictLock)
            {
                if (!memberDict.ContainsKey(type))
                    memberDict.Add(type, type.GetPublicFieldsAndPropertiesInternal().ToList());
                return memberDict[type];
            }
        }

        /// <summary>
        /// Get all public fields and properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>All public fields and properties.</returns>
        private static IEnumerable<MemberInfo> GetPublicFieldsAndPropertiesInternal(this Type type)
        {
            // First, return all fields.
            foreach (var f in type.GetRuntimeFields())
                if (f.IsPublic)
                    yield return f;

            // Then, return all properties.
            foreach (var p in type.GetRuntimeProperties())
                yield return p;
        }

        /// <summary>
        /// Gets the member type.
        /// </summary>
        /// <param name="info">The <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</param>
        /// <returns>The underlying member type.</returns>
        public static Type GetMemberType(this MemberInfo info)
        {
            if (info is FieldInfo)
                return ((FieldInfo)info).FieldType;
            else
                return ((PropertyInfo)info).PropertyType;
        }

        /// <summary>
        /// Sets the value of a member.
        /// </summary>
        /// <param name="info">The <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</param>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetValue(this MemberInfo info, object obj, object value)
        {
            if (info is FieldInfo)
                ((FieldInfo)info).SetValue(obj, value);
            else
            {
                var pi = (PropertyInfo)info;
                if (pi.CanWrite)
                    pi.SetValue(obj, value);
            }
        }

        /// <summary>
        /// Gets the value of a member.
        /// </summary>
        /// <param name="info">The <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.</param>
        /// <param name="obj">The object.</param>
        /// <returns>The value.</returns>
        public static object GetValue(this MemberInfo info, object obj)
        {
            if (info is FieldInfo)
                return ((FieldInfo)info).GetValue(obj);
            else
                return ((PropertyInfo)info).GetValue(obj);
        }
    }
}
