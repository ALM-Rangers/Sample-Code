//---------------------------------------------------------------------
// <copyright file="ConfigurationReader.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ConfigurationReader type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Reads configuration data.
    /// </summary>
    public static class ConfigurationReader
    {
        /// <summary>
        /// Reads the configuration from a file.
        /// </summary>
        /// <param name="fileName">The configuration to be read.</param>
        /// <returns>The configuration.</returns>
        public static WcfUnitConfiguration Read(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.Configuration_FileNotFound, fileName));
            }

            WcfUnitConfiguration ans = null;
            XmlSerializer xs = new XmlSerializer(typeof(WcfUnitConfiguration));
            using (XmlReader reader = XmlReader.Create(fileName, GetReaderSettings()))
            {
                try
                {
                    ans = (WcfUnitConfiguration)xs.Deserialize(reader);
                }
                catch (InvalidOperationException ioe)
                {
                    throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.Configuration_InvalidFile, fileName), ioe);
                }
            }

            return ans;
        }

        /// <summary>
        /// Sets up the settings for the <see cref="XmlReader"/>.
        /// </summary>
        /// <returns>The <see cref="XmlReader"/> settings.</returns>
        private static XmlReaderSettings GetReaderSettings()
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            XmlReader schemaReader = XmlReader.Create(Assembly.GetAssembly(typeof(ConfigurationReader)).GetManifestResourceStream("Microsoft.WcfUnit.Library.ConfigurationSchema.xsd"));
            readerSettings = new XmlReaderSettings();
            ////readerSettings.CheckCharacters = true;
            readerSettings.CloseInput = true;
            readerSettings.Schemas.Add(null, schemaReader);
            readerSettings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.IgnoreComments = true;
            readerSettings.IgnoreWhitespace = true;
            readerSettings.IgnoreProcessingInstructions = true;
            return readerSettings;
        }
    }
}
