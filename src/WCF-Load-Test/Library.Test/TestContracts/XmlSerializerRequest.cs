//---------------------------------------------------------------------
// <copyright file="XmlSerializerRequest.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The XmlSerializerRequest type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test.TestContracts
{
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public class XmlSerializerRequest
    {
        private string stringField;

        private int integerField;

        [System.Xml.Serialization.XmlAttributeAttribute]
        public string S
        {
            get
            {
                return this.stringField;
            }

            set
            {
                this.stringField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute]
        public int I
        {
            get
            {
                return this.integerField;
            }

            set
            {
                this.integerField = value;
            }
        }
    }
}