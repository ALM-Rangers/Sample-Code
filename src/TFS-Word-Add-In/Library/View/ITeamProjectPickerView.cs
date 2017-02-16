//---------------------------------------------------------------------
// <copyright file="ITeamProjectPickerView.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITeamProjectPickerView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using Microsoft.Word4Tfs.Library.Data;

    /// <summary>
    /// Defines the functions of the Team Project picker.
    /// </summary>
    public interface ITeamProjectPickerView : IViewBase
    {
        /// <summary>
        /// Displays a dialog to get the user to select the team project collection and the team project.
        /// </summary>
        /// <returns>The data for the selected Team Project, or <c>null</c> if no project was selected.</returns>
        TeamProjectInformation ChooseTeamProject();

        /// <summary>
        /// Displays a dialog to get the user to select the team project collection.
        /// </summary>
        /// <returns>The <see cref="Uri"/> for the selected Team Project Collection, or <c>null</c> if no Team Project Collection was selected.</returns>
        Uri ChooseTeamProjectCollection();
    }
}
