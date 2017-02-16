//---------------------------------------------------------------------
// <copyright file="CallParameterInfo.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The CallParameterInfo type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.CodeDom;

    /// <summary>
    /// Basic information about each parameter to a call to a service.
    /// </summary>
    public class CallParameterInfo
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        private readonly Type type;

        /// <summary>
        /// The direction of the parameter.
        /// </summary>
        private readonly FieldDirection direction;

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// Initialises a new instance of the <see cref="CallParameterInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="direction">The direction of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public CallParameterInfo(string name, Type type, FieldDirection direction, object value)
        {
            this.name = name;
            this.type = type;
            this.direction = direction;
            this.value = value;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        /// <value>The parameter name.</value>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <value>The parameter type.</value>
        public Type ParameterType
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets a value indicating the direction of the parameter.
        /// </summary>
        /// <value>A <see cref="FieldDirection"/> value.</value>
        public FieldDirection Direction
        {
            get { return this.direction; }
        }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        /// <value>The value of the parameter.</value>
        public object Value
        {
            get { return this.value; }
        }
    }
}
