//---------------------------------------------------------------------
// <copyright file="WcfSerializationInfoClass.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WcfSerializationInfoClass type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Provides the serialization info for WCF types.
    /// </summary>
    public class WcfSerializationInfoClass : ISerializationInfo
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
                return Messages.ObjectGenerator_NoDataOrMessageContract;
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
            bool classIsDataContract = method.DeclaringType.GetCustomAttributes(typeof(DataContractFormatAttribute), false).Length > 0;
            bool methodIsXmlSerializer = method.GetCustomAttributes(typeof(XmlSerializerFormatAttribute), false).Length > 0;
            bool classIsXmlSerializer = method.DeclaringType.GetCustomAttributes(typeof(XmlSerializerFormatAttribute), false).Length > 0;

            bool classIsNeither = !classIsDataContract && !classIsXmlSerializer;
            bool methodIsNeither = !methodIsDataContract && !methodIsXmlSerializer;

            return methodIsDataContract
                   ||
                   (classIsNeither && methodIsNeither)
                   ||
                   (classIsDataContract && methodIsNeither);
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
                objectType == typeof(TimeSpan)
                ||
                objectType == typeof(Guid)
                ||
                objectType == typeof(Uri)
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
        /// <param name="objectType">The type to be checked.</param>
        /// <returns>True if the type is serializable, false otherwise</returns>
        public bool IsSerializable(Type objectType)
        {
            bool ans = Attribute.GetCustomAttribute(objectType, typeof(DataContractAttribute)) != null
                       ||
                       Attribute.GetCustomAttribute(objectType, typeof(MessageContractAttribute)) != null;

            if (!ans)
            {
                ans = typeof(IXmlSerializable).IsAssignableFrom(objectType);
            }

            return ans;
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
                    if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
                    {
                        ans.Add(member);
                    }
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
        /// Checks to see if this is a member that can be serialized by the chosen web service technology.
        /// </summary>
        /// <param name="member">The member to be checked.</param>
        /// <returns>True if the member is serializable, false otherwise</returns>
        private static bool IsSerializable(MemberInfo member)
        {
            bool ans = Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute)) != null
                       ||
                       Attribute.GetCustomAttribute(member, typeof(MessageHeaderAttribute)) != null
                       ||
                       Attribute.GetCustomAttribute(member, typeof(MessageBodyMemberAttribute)) != null;

            return ans;
        }
    }
}
