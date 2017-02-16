//---------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The Utilities type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.Win32;
    using Microsoft.Word4Tfs.Library;

    /// <summary>
    /// General utilities
    /// </summary>
    public static class Utilities
    {
        /////// <summary>
        /////// The path to the registry key where the Visual Studio location can be determined.
        /////// </summary>
        ////private const string RegistryKeyForVisualStudioLocation = @"SOFTWARE\Microsoft\VisualStudio\SxS\VS7";

        /////// <summary>
        /////// The value in the registry key where the Visual Studi location can be determined.
        /////// </summary>
        ////private const string RegistryValueForVisualStudioLocation = "10.0";

        /////// <summary>
        /////// Caches the path to where Visual Studio is installed.
        /////// </summary>
        ////private static string visualStudioPath = null;

        /// <summary>
        /// Finds a button within a control.
        /// </summary>
        /// <remarks>
        /// The button is required to be tagged with the string of the <paramref name="button"/>enumeration.
        /// </remarks>
        /// <param name="control">The control container to be searched.</param>
        /// <param name="button">The button to be returned.</param>
        /// <returns>The button, or <c>null</c> if not found.</returns>
        public static Control FindButton(Control control, Enum button)
        {
            string tag = GetButtonTag(button);
            return FindControl(control, tag);
        }

        /// <summary>
        /// Gets the tag associated with a button.
        /// </summary>
        /// <param name="button">The button whose tag is needed.</param>
        /// <returns>The tag for the button.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "There is no sensible validation that can be done.")]
        public static string GetButtonTag(Enum button)
        {
            string tag = button.ToString();
            return tag;
        }

        /////// <summary>
        /////// Reads the path to where Visual Studio is installed.
        /////// </summary>
        /////// <returns>The path to where Visual Studio is installed.</returns>
        /////// <remarks>
        /////// The location is cached so that there is no need to read the registry each time.
        /////// </remarks>
        ////public static string ReadVisualStudioPath()
        ////{
        ////    if (string.IsNullOrEmpty(visualStudioPath))
        ////    {
        ////        using (RegistryKey baseKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
        ////        {
        ////            using (RegistryKey key = baseKey32.OpenSubKey(RegistryKeyForVisualStudioLocation))
        ////            {
        ////                visualStudioPath = key.GetValue(RegistryValueForVisualStudioLocation) as string;
        ////            }
        ////        }
        ////    }

        ////    return visualStudioPath;
        ////}

        /// <summary>
        /// Shows an error dialog.
        /// </summary>
        /// <param name="messageType">The type of message to display.</param>
        /// <param name="title">The title to be given to the message box.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="details">Optional details to display.</param>
         public static void DisplayMessage(UIMessageType messageType, string title, string message, string details)
        {
            using (MessageDialogue errorDialogue = new MessageDialogue())
            {
                errorDialogue.DisplayMessage(messageType, title, message, details);
                errorDialogue.ShowDialog();
            }
            ////RightToLeftAwareMessageBox.Show(null, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Finds a control with the given tag within a control.
        /// </summary>
        /// <param name="container">The control container to search.</param>
        /// <param name="tag">The tag of the control to be found.</param>
        /// <returns>The control with the given tag, or <c>null</c> if not found.</returns>
        private static Control FindControl(Control container, string tag)
        {
            Control ans = null;
            foreach (Control c in container.Controls)
            {
                if (string.Compare((string)c.Tag, tag, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ans = c;
                }
                else
                {
                    ans = FindControl(c, tag);
                }

                if (ans != null)
                {
                    break;
                }
            }

            return ans;
        }
    }
}
