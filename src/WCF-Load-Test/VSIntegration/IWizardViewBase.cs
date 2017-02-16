//---------------------------------------------------------------------
// <copyright file="IWizardViewBase.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The IWizardViewBase type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    /// <summary>
    /// Interface common to wizards.
    /// </summary>
    public interface IWizardViewBase : IWizardViewPage
    {
        /// <summary>
        /// Gets the current page that the wizard is on.
        /// </summary>
        WizardPage CurrentPage
        {
            get;
        }

        /// <summary>
        /// Moves the wizard to nominated page.
        /// </summary>
        /// <param name="page">The page that the wizard must move to.</param>
        void MoveToPage(WizardPage page);

        /// <summary>
        /// Sets the control to be enabled or not.
        /// </summary>
        /// <param name="control">The control for which the status is to be set.</param>
        /// <param name="enabled">The enabled/disabled status to set.</param>
        void SetControlState(WizardControl control, bool enabled);

        /// <summary>
        /// Gets the enabled/disabled state of a control.
        /// </summary>
        /// <param name="control">The control to get the state of.</param>
        /// <returns>True if the control is enabled, false otherwise.</returns>
        bool GetControlState(WizardControl control);

        /// <summary>
        /// Sets the wizard page to be enabled or not.
        /// </summary>
        /// <param name="page">The page for which the status is to be set.</param>
        /// <param name="enabled">The enabled/disabled status to set.</param>
        void SetPageState(WizardPage page, bool enabled);

        /// <summary>
        /// Gets the enabled/disabled state of a page
        /// </summary>
        /// <param name="page">The page to get the state of.</param>
        /// <returns>True if the page is enabled, false otherwise.</returns>
        bool GetPageState(WizardPage page);

        /// <summary>
        /// Sets the wizard button to be enabled or not
        /// </summary>
        /// <param name="button">The button for which the status is to be set.</param>
        /// <param name="enabled">The enabled/disabled status to set.</param>
        void SetButtonState(WizardButton button, bool enabled);

        /// <summary>
        /// Gets the enabled/disabled state of a button
        /// </summary>
        /// <param name="button">The button to get the state of.</param>
        /// <returns>True if the button is enabled, false otherwise.</returns>
        bool GetButtonState(WizardButton button);

        /// <summary>
        /// Sets the wizard radio button to be checked or not
        /// </summary>
        /// <param name="button">The button for which the checked value is to be set.</param>
        /// <param name="check">The checked/unchecked status to set.</param>
        void SetRadioState(WizardButton button, bool check);

        /// <summary>
        /// Gets the checked/unchecked state of a radio button
        /// </summary>
        /// <param name="button">The radio button to get the checked/unchecked of.</param>
        /// <returns>True if the radio button is checked, false otherwise.</returns>
        bool GetRadioState(WizardButton button);

        /// <summary>
        /// Displays an error message.
        /// </summary>
        /// <param name="title">The title to be given to the message box.</param>
        /// <param name="errorMessage">The error message to display.</param>
        void DisplayError(string title, string errorMessage);
    }
}
