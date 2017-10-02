//---------------------------------------------------------------------
// <copyright file="RightToLeftAwareMessageBox.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The RightToLeftAwareMessageBox type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System.Globalization;
    using System.Windows.Forms;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// Right-to-left aware message box.
    /// </summary>
    public class RightToLeftAwareMessageBox : IMessageBoxView
    {
        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="message">The message to put in the message box.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="buttons">Which buttons to display.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>A value indicating which button was pressed.</returns>
        public MessageBoxResult Show(string message, string title, MessageBoxViewButtons buttons, MessageBoxViewIcon icon)
        {
            return (MessageBoxResult)Show(null, message, title, (MessageBoxButtons)buttons, (MessageBoxIcon)icon);
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="owner">The control that wants to show the message box.</param>
        /// <param name="text">The text in the message box.</param>
        /// <param name="caption">The title of the message box window.</param>
        /// <param name="buttons">The buttons to be shown.</param>
        /// <param name="icon">The icon to show.</param>
        /// <returns>The dialog result after the user has finished with the message box.</returns>
        private static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBoxOptions options = 0;
            if (IsRightToLeft(owner))
            {
                options |= MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign;
            }

            return MessageBox.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, options);
        }

        /// <summary>
        /// Determines if the message box needs to be RightToLeft.
        /// </summary>
        /// <param name="owner">The control that wants to show the message box.</param>
        /// <returns><c>true</c> if the message box needs to be Right To Left, <c>false</c> otherwise.</returns>
        private static bool IsRightToLeft(IWin32Window owner)
        {
            Control control = owner as Control;

            if (control != null)
            {
                return control.RightToLeft == RightToLeft.Yes;
            }

            // If no parent control is available, ask the CurrentUICulture
            // if we are running under right-to-left.
            return CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
        }
    }
}
