//---------------------------------------------------------------------
// <copyright file="IMessageBoxView.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IMessageBoxView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    // TODO: Unify with the error dialogue window.

    /// <summary>
    /// The button combinations on a message box.
    /// </summary>
    /// <remarks>
    /// This type is defined to avoid a dependency on Windows Forms in the library.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "They are not flag values.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Justification = "The values refer to sets of buttons, so plural is correct here.")]
    public enum MessageBoxViewButtons
    {
        /// <summary>
        /// This value is not used.
        /// </summary>
        NotUsed,

        /// <summary>
        /// Displays a Yes and a No button.
        /// </summary>
        YesNo = 4,

        /// <summary>
        /// Displays a Yes, a No and a Cancel button.
        /// </summary>
        YesNoCancel = 3
    }

    /// <summary>
    /// The icons on a message box.
    /// </summary>
    /// <remarks>
    /// This type is defined to avoid a dependency on Windows Forms in the library.
    /// </remarks>
    public enum MessageBoxViewIcon
    {
        /// <summary>
        /// This value is not used.
        /// </summary>
        NotUsed,

        /// <summary>
        /// Represents a warning icon.
        /// </summary>
        Warning = 48
    }

    /// <summary>
    /// Possible results from a message box.
    /// </summary>
    /// <remarks>
    /// This type is defined to avoid a dependency on Windows Forms in the library.
    /// </remarks>
    public enum MessageBoxResult
    {
        /// <summary>
        /// No result yet.
        /// </summary>
        None = 0,

        /// <summary>
        /// The Yes button was pressed.
        /// </summary>
        Yes = 6,

        /// <summary>
        /// The No button was pressed.
        /// </summary>
        No = 7,

        /// <summary>
        /// The Cancel button was pressed.
        /// </summary>
        Cancel = 2
    }

    /// <summary>
    /// Defines the operations on a message box.
    /// </summary>
    public interface IMessageBoxView
    {
        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="message">The message to put in the message box.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="buttons">Which buttons to display.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>A value indicating which button was pressed.</returns>
        MessageBoxResult Show(string message, string title, MessageBoxViewButtons buttons, MessageBoxViewIcon icon);
    }
}
