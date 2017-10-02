//---------------------------------------------------------------------
// <copyright file="LowLevelSettings.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LowLevelSettings type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using Microsoft.Win32;

    /// <summary>
    /// Provides raw low level access to the settings.
    /// </summary>
    public class LowLevelSettings : ILowLevelSettings, IDisposable
    {
        /// <summary>
        /// The registry key used to get and set settings.
        /// </summary>
        private RegistryKey settingsKey;

        /// <summary>
        /// Track whether <see cref="Dispose"/> has been called.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="LowLevelSettings"/> class.
        /// </summary>
        public LowLevelSettings()
        {
            this.settingsKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Word4Tfs");
        }

       /// <summary>
        /// Finalizes an instance of the <see cref="LowLevelSettings"/> class.
        /// </summary>
        ~LowLevelSettings()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets or sets a named setting.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <returns>The value of the setting. <c>Null</c> if the setting does not exist.</returns>
        public object this[string name]
        {
            get
            {
                return this.settingsKey.GetValue(name);
            }

            set
            {
                this.settingsKey.SetValue(name, value);
            }
        }

        /// <summary>
        /// Disposes of the object and any resources it holds.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs actual dispose.
        /// </summary>
        /// <param name="disposing"><c>true</c> if <see cref="Dispose"/> has been called explicitly, <c>false</c> if disposal is being done by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.settingsKey.Dispose();
                }

                // Note disposing has been done.
                this.disposed = true;
            }
        }
    }
}
