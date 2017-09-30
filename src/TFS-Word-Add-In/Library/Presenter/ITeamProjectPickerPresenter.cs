//---------------------------------------------------------------------
// <copyright file="ITeamProjectPickerPresenter.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITeamProjectPickerPresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using Microsoft.Word4Tfs.Library.Model;

    /// <summary>
    /// The operations of a team project picker presenter.
    /// </summary>
    public interface ITeamProjectPickerPresenter
    {
        /// <summary>
        /// Gets the team and project information.
        /// </summary>
        /// <returns>The information for the selected team and project, <c>null</c> if none selected.</returns>
        ITeamProject ChooseTeamProject();

        /// <summary>
        /// Chooses a team project collection.
        /// </summary>
        /// <remarks>Used when rebinding a document.</remarks>
        /// <returns>The <see cref="Uri"/> of the team project collection.</returns>
        Uri ChooseTeamProjectCollection();

        /// <summary>
        /// Saves the team and project information in the model.
        /// </summary>
        void SaveTeamProject();
    }
}
