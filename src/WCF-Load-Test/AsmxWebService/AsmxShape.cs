//---------------------------------------------------------------------
// <copyright file="AsmxShape.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The AsmxShape type.</summary>
//---------------------------------------------------------------------

namespace AsmxWebService
{
    using System.Xml.Serialization;

    [XmlInclude(typeof(AsmxCircle))]
    [XmlInclude(typeof(AsmxRectangle))]
    public class AsmxShape
    {
        private int colour;

        public int Colour
        {
            get { return this.colour; }
            set { this.colour = value; }
        }

        [XmlIgnore]
        public virtual string Ignored { get; set; }
    }
}