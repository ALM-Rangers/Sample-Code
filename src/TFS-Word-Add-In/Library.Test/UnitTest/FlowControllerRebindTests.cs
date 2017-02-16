//---------------------------------------------------------------------
// <copyright file="FlowControllerRebindTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The FlowControllerRebindTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Tests the document rebind functionality.
    /// </summary>
    [TestClass]
    public class FlowControllerRebindTests : FlowControllerTestsBase
    {
        /// <summary>
        /// Tests that a rebind callback from the ribbon presenter invokes the project picker for the team project collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RebindCallbackFromTheRibbonPresenterInvokesTheProjectPickerForTheTeamProjectCollection()
        {
            // Act
            this.CapturedRebindCallback();

            // Assert
            this.MockTeamProjectPickerPresenter.Verify(picker => picker.ChooseTeamProjectCollection(), Times.Once());
        }

        /// <summary>
        /// Tests that a rebind callback from the ribbon presenter returns the collection Uri chosen in the project picker.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RebindCallbackFromTheRibbonPresenterReturnsCollectionUriChosenInProjectPicker()
        {
            // Arrange
            this.SetupDummyPickedProject();

            // Act
            Uri ans = this.CapturedRebindCallback();

            // Assert
            Assert.AreEqual<Uri>(new Uri(TestHelper.ValidProjectCollectionUri), ans, "Collection Uri from the team project picker not returned.");
        }

        /// <summary>
        /// Tests that a rebind callback from the ribbon presenter returns null if the project picker is cancelled
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RebindCallbackFromTheRibbonPresenterReturnsNullIfProjectPickerCancelled()
        {
            // Arrange
            this.SetupDummyNoPickedProject();

            // Act
            Uri ans = this.CapturedRebindCallback();

            // Assert
            Assert.IsNull(ans, "Callback must return null if the project picker is cancelled.");
        }
    }
}
