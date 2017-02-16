//---------------------------------------------------------------------
// <copyright file="FlowControllerLayoutDesignerTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The FlowControllerLayoutDesignerTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Presenter;
    using Moq;

    /// <summary>
    /// Tests the layout designer aspects of the flow controller.
    /// </summary>
    [TestClass]
    public class FlowControllerLayoutDesignerTests : FlowControllerTestsBase
    {
        /// <summary>
        /// ShowLayoutDesigner event causes the layout designer presenter to be instructed to show the layout designer.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowLayoutDesignerEventTellsLayoutDesignerPresenterToShowTheLayoutDesigner()
        {
            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.ShowLayoutDesigner += null, EventArgs.Empty);

            // Assert
            this.MockLayoutDesignerPresenter.Verify(p => p.Show(), Times.Once());
        }

        /// <summary>
        /// HideLayoutDesigner event causes the layout designer presenter to be instructed to hide the layout designer.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void HideLayoutDesignerEventTellsLayoutDesignerPresenterToHideTheLayoutDesigner()
        {
            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.HideLayoutDesigner += null, EventArgs.Empty);

            // Assert
            this.MockLayoutDesignerPresenter.Verify(p => p.Hide(), Times.Once());
        }

        /// <summary>
        /// Updates ribbon state after showing the layout designer
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdatesRibbonStateAfterShowingTheLayoutDesigner()
        {
            // Assert
            this.MockRibbonPresenter.Verify(ribbonPresenter => ribbonPresenter.UpdateState(), Times.Never(), "Pre-requisite is that update state not called yet.");

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.ShowLayoutDesigner += null, EventArgs.Empty);

            // Assert
            this.MockRibbonPresenter.Verify(ribbonPresenter => ribbonPresenter.UpdateState(), Times.Once(), "Ribbon state should have been updated.");
        }

        /// <summary>
        /// Invokes the connection dialog when the layout designer presenter raises the <see cref="ILayoutDesignerPresenter.Connect"/> event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InvokesConnectionDialogWhenLayoutDesignerRaisesConnectEvent()
        {
            // Act
            this.MockLayoutDesignerPresenter.Raise(designerPresenter => designerPresenter.Connect += null, EventArgs.Empty);

            // Assert
            this.MockTeamProjectPickerPresenter.Verify(picker => picker.ChooseTeamProject(), Times.Once());
        }

        /// <summary>
        /// No change to the designer document if the connection dialog for the layout designer is cancelled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotSaveConnectionIfConnectionDialogForLayoutDesignerIsCancelled()
        {
            // Arrange
            this.SetupDummyNoPickedProject();

            // Act
            this.MockLayoutDesignerPresenter.Raise(designerPresenter => designerPresenter.Connect += null, EventArgs.Empty);

            // Assert
            this.MockTeamProjectPickerPresenter.Verify(picker => picker.SaveTeamProject(), Times.Never(), "Should not save connection");
        }

        /// <summary>
        /// Saves team project to the designer document if the connection dialog for the layout designer is completed successfully.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SavesConnectionIfConnectionDialogForLayoutDesignerIsCompletedSuccessfully()
        {
            // Arrange
            this.SetupDummyPickedProject();

            // Act
            this.MockLayoutDesignerPresenter.Raise(designerPresenter => designerPresenter.Connect += null, EventArgs.Empty);

            // Assert
            this.MockTeamProjectPickerPresenter.Verify(picker => picker.SaveTeamProject(), Times.Once(), "Connection was not saved");
        }

        /// <summary>
        /// ShowLayoutDesigner event uses same layout designer presenter if the first one was not closed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowLayoutDesignerUsesSameLayoutDesignerPresenterIfTheFirstOneIsNotClosed()
        {
            // Arrange
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.ShowLayoutDesigner += null, EventArgs.Empty);
            Mock<ILayoutDesignerPresenter> mockSecondPresenter = TestHelper.CreateAndRegisterMock<ILayoutDesignerPresenter>(this.Container);

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.ShowLayoutDesigner += null, EventArgs.Empty);

            // Assert
            mockSecondPresenter.Verify(p => p.Show(), Times.Never(), "New presenter should not be instantiated");
        }

        /// <summary>
        /// ShowLayoutDesigner event causes new layout designer presenter to be instantiated if the first one was closed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowLayoutDesignerCausesNewLayoutDesignerPresenterToBeInstantiatedIfTheFirstOneWasClosed()
        {
            // Arrange
            Mock<ILayoutDesignerPresenter> mockSecondPresenter = this.ClosePresenterAndPrepareNextOne(this.MockLayoutDesignerPresenter);

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.ShowLayoutDesigner += null, EventArgs.Empty);

            // Assert
            mockSecondPresenter.Verify(p => p.Show(), Times.Once(), "New presenter not instantiated");
        }

        /// <summary>
        /// ShowLayoutDesigner event causes new layout designer presenter to be instantiated if the second one was closed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowLayoutDesignerCausesNewLayoutDesignerPresenterToBeInstantiatedIfTheSecondOneWasClosed()
        {
            // Arrange
            Mock<ILayoutDesignerPresenter> mockSecondPresenter = this.ClosePresenterAndPrepareNextOne(this.MockLayoutDesignerPresenter);
            Mock<ILayoutDesignerPresenter> mockThirdPresenter = this.ClosePresenterAndPrepareNextOne(mockSecondPresenter);

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.ShowLayoutDesigner += null, EventArgs.Empty);

            // Assert
            mockThirdPresenter.Verify(p => p.Show(), Times.Once(), "New presenter not instantiated");
        }

        /// <summary>
        /// Invokes the connection dialog when a second layout designer presenter raises the <see cref="ILayoutDesignerPresenter.Connect"/> event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InvokesConnectionDialogWhenSecondLayoutDesignerRaisesConnectEvent()
        {
            // Arrange
            Mock<ILayoutDesignerPresenter> mockSecondPresenter = this.ClosePresenterAndPrepareNextOne(this.MockLayoutDesignerPresenter);
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.ShowLayoutDesigner += null, EventArgs.Empty);

            // Act
            mockSecondPresenter.Raise(designerPresenter => designerPresenter.Connect += null, EventArgs.Empty);

            // Assert
            this.MockTeamProjectPickerPresenter.Verify(picker => picker.ChooseTeamProject(), Times.Once());
        }

        /// <summary>
        /// Arranges for the first presenter to be closed and a second one to be created.
        /// </summary>
        /// <param name="mockCurrentPresenter">The mock presenter which will close.</param>
        /// <returns>Mock layout designer presenter</returns>
        private Mock<ILayoutDesignerPresenter> ClosePresenterAndPrepareNextOne(Mock<ILayoutDesignerPresenter> mockCurrentPresenter)
        {
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.ShowLayoutDesigner += null, EventArgs.Empty);
            mockCurrentPresenter.Raise(designerPresenter => designerPresenter.Close += null, EventArgs.Empty);
            Mock<ILayoutDesignerPresenter> mockNextPresenter = TestHelper.CreateAndRegisterMock<ILayoutDesignerPresenter>(this.Container);
            return mockNextPresenter;
        }
    }
}
