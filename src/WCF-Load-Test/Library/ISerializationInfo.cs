//---------------------------------------------------------------------
// <copyright file="ISerializationInfo.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ISerializationInfo type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Provides information about what is serialized for a particular web service technology (e.g. ASMX and WCF).
    /// </summary>
    public interface ISerializationInfo
    {
        /// <summary>
        /// Gets the error message to be displayed when a type is not serializable. The return value contains a format marker where the type name
        /// must be inserted.
        /// </summary>
        string NotSerializableError
        {
            get;
        }

        /// <summary>
        /// Tests the supplied method to see if it is a supported type of service.
        /// </summary>
        /// <param name="method">The method to test.</param>
        /// <returns>True if the method is supported, false otherwise.</returns>
        bool Supports(MethodInfo method);

        /// <summary>
        /// Checks to see if this is a simple type for which a 1-line declaration and initial value can be generated
        /// </summary>
        /// <param name="objectType">The type to be checked.</param>
        /// <returns>True if the type is a simple type, false otherwise</returns>
        bool IsSimpleType(Type objectType);

        /// <summary>
        /// Checks to see if this is a type that can be serialized by the chosen web service technology.
        /// </summary>
        /// <param name="objectType">The type to be checked.</param>
        /// <returns>True if the type is serializable, false otherwise</returns>
        bool IsSerializable(Type objectType);

        /// <summary>
        /// Gets the list of direct members of type <paramref name="objectType"/> which are serializable.
        /// </summary>
        /// <param name="objectType">The type to get the members of.</param>
        /// <returns>The list of serializable members.</returns>
        IEnumerable<MemberInfo> SerializableMembers(Type objectType);
    }
}
