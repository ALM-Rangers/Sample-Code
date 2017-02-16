//---------------------------------------------------------------------
// <copyright file="AsmxSerializationInfoClass.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The AsmxSerializationInfoClass type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.ServiceModel;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Provides the serialization info for ASMX types.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Asmx", Justification = "Happy with this spelling")]
    public class AsmxSerializationInfoClass : ISerializationInfo
    {
        /// <summary>
        /// Gets the error message to be displayed when a type is not serializable. The return value contains a format marker where the type name
        /// must be inserted.
        /// </summary>
        /// <value>The error message to be displayed when a type is not serializable.</value>
        public string NotSerializableError
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Tests the supplied method to see if it is a supported type of service.
        /// </summary>
        /// <param name="method">The method to test.</param>
        /// <returns>True if the method is supported, false otherwise.</returns>
        public bool Supports(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            bool methodIsDataContract = method.GetCustomAttributes(typeof(DataContractFormatAttribute), false).Length > 0;
            bool methodIsXmlSerializer = method.GetCustomAttributes(typeof(XmlSerializerFormatAttribute), false).Length > 0;
            bool classIsXmlSerializer = method.DeclaringType.GetCustomAttributes(typeof(XmlSerializerFormatAttribute), false).Length > 0;

            bool methodIsNeither = !methodIsDataContract && !methodIsXmlSerializer;

            return methodIsXmlSerializer
                   ||
                   (classIsXmlSerializer && methodIsNeither);
        }

        /// <summary>
        /// Checks to see if this is a simple type for which a 1-line declaration and initial value can be generated
        /// </summary>
        /// <param name="objectType">The type to be checked.</param>
        /// <returns>True if the type is a simple type, false otherwise</returns>
        public bool IsSimpleType(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            bool ans = false;

            if (objectType.IsPrimitive
                ||
                objectType.IsEnum
                ||
                objectType == typeof(string)
                ||
                objectType == typeof(decimal)
                ||
                objectType == typeof(DateTime)
                ||
                objectType == typeof(Guid)
                ||
                objectType == typeof(XmlQualifiedName)
                ||
                IsNullableType(objectType))
            {
                ans = true;
            }

            return ans;
        }

        /// <summary>
        /// Checks to see if this is a type that can be serialized by the chosen web service technology.
        /// </summary>
        /// <remarks>
        /// With ASMX all types are included, there is no equivalent of the WCF opt-in DataContract attribute.
        /// </remarks>
        /// <param name="objectType">The type to be checked.</param>
        /// <returns>True if the type is serializable, false otherwise</returns>
        public bool IsSerializable(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// Gets the list of direct members of type <paramref name="objectType"/> which are serializable.
        /// </summary>
        /// <param name="objectType">The type to get the members of.</param>
        /// <returns>The list of serializable members.</returns>
        public IEnumerable<MemberInfo> SerializableMembers(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            List<MemberInfo> ans = new List<MemberInfo>();
            MemberInfo[] members = objectType.GetMembers(BindingFlags.Instance | BindingFlags.Public);
            foreach (MemberInfo member in members)
            {
                if (IsSerializable(member))
                {
                    ans.Add(member);
                }
            }

            return ans;
        }

        /// <summary>
        /// Checks to see if the Type is a nullable generic.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if nullable, false otherwise</returns>
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.Name == "Nullable`1" && type.Namespace == "System";
        }

        /// <summary>
        /// Tests a member to see if it is serializable.
        /// </summary>
        /// <param name="member">The member to test.</param>
        /// <returns>True if it is serializable.</returns>
        private static bool IsSerializable(MemberInfo member)
        {
            bool ans = false;
            bool ignore = IsIgnored(member);

            if (!ignore)
            {
                if (member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo pi = (PropertyInfo)member;
                    ans = pi.CanRead && pi.CanWrite;
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    ans = true;
                }
            }

            return ans;
        }

        /// <summary>
        /// Tests to see if the member should be ignored.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <returns>True if the member is to be ignored.</returns>
        private static bool IsIgnored(MemberInfo member)
        {
            const string Specified = "Specified";

            bool ignore = member.GetCustomAttributes(typeof(XmlIgnoreAttribute), false).Length > 0;

            // Check to see if is the "Specified" field for another field
            if (ignore)
            {
                if (member.Name.EndsWith(Specified, StringComparison.Ordinal))
                {
                    string fieldName = member.Name.Substring(0, member.Name.Length - Specified.Length);
                    bool fieldExists = member.ReflectedType.GetMember(fieldName, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public).Length > 0;
                    ignore = !fieldExists;
                }
            }

            return ignore;
        }
    }
}
