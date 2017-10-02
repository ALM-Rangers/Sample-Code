//---------------------------------------------------------------------
// <copyright file="Settings.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The Settings type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Globalization;

    /// <summary>
    /// The settings that govern the overall operation of the Add-in.
    /// </summary>
    public class Settings : ISettings
    {
        /// <summary>
        /// The low level settings object that backs up the settings.
        /// </summary>
        private ILowLevelSettings lowLevelSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="lowLevelSettings">The low level settings object that backs up the settings.</param>
        public Settings(ILowLevelSettings lowLevelSettings)
        {
            this.lowLevelSettings = lowLevelSettings;
        }

        /// <summary>
        /// Gets or sets a value indicating whether bookmarks should be shown or not.
        /// </summary>
        /// <remarks>A <c>null</c> value indicates the setting is not present.</remarks>
        public Nullable<bool> ShowBookmarks
        {
            get
            {
                Nullable<bool> ans = null;
                string showBookmarksValue = this.lowLevelSettings[Constants.ShowBookmarksSettingName] as string;
                bool parsedValue;
                bool valid = bool.TryParse(showBookmarksValue, out parsedValue);
                if (valid)
                {
                    ans = parsedValue;
                }

                return ans;
            }

            set
            {
                this.lowLevelSettings[Constants.ShowBookmarksSettingName] = value;
            }
        }

        /// <summary>
        /// Gets or sets the timestamp of system template for which we are not to prompt for an upgrade.
        /// </summary>
        /// <remarks>A <c>null</c> value indicates the setting is not present.</remarks>
        public Nullable<DateTime> IgnoreSystemTemplateUpgradeFor
        {
            get
            {
                Nullable<DateTime> ans = null;
                string stringValue = this.lowLevelSettings[Constants.TemplateUpgradeSettingName] as string;
                DateTime parsedValue;
                bool valid = DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out parsedValue);
                if (valid)
                {
                    ans = parsedValue;
                }

                return ans;
            }

            set
            {
                this.lowLevelSettings[Constants.TemplateUpgradeSettingName] = value.Value.ToString("O", CultureInfo.InvariantCulture);
            }
        }
    }
}
