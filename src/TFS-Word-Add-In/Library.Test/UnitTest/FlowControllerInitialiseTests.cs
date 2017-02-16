//---------------------------------------------------------------------
// <copyright file="FlowControllerInitialiseTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The FlowControllerInitialiseTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Moq;

    /// <summary>
    /// Tests the flow controller initialisation.
    /// </summary>
    [TestClass]
    public class FlowControllerInitialiseTests : FlowControllerTestsBase
    {
        /// <summary>
        /// The path to the template in the roaming profile.
        /// </summary>
        private string roamingTemplatePath = Path.Combine(RoamingTemplateDir, Constants.SystemTemplateName);

        /// <summary>
        /// The path to the source template in the installation directory.
        /// </summary>
        private string sourceTemplatePath = Path.Combine(Utilities.ReadExecutingDirectory(), Constants.SystemTemplateName);

        /// <summary>
        /// Date time for the installed template.
        /// </summary>
        private DateTime installDirTemplateFileTimeStamp = new DateTime(2011, 11, 21, 12, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Date time for a roaming template older than the installed template.
        /// </summary>
        private DateTime olderRoamingTemplateFileTimeStamp = new DateTime(2011, 11, 21, 11, 59, 59, DateTimeKind.Utc);

        /// <summary>
        /// Date time for a roaming template newer than the installed template.
        /// </summary>
        private DateTime newerRoamingTemplateFileTimeStamp = new DateTime(2011, 11, 21, 12, 0, 1, DateTimeKind.Utc);

        /// <summary>
        /// Initialise tells the ribbon presenter to load the ribbon state.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseTellsRibbonPresenterToLoadState()
        {
            // Assert
            this.MockRibbonPresenter.Verify(ribbon => ribbon.LoadState(), Times.Once());
        }

        /// <summary>
        /// Initialise tells the manager to initialise.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseTellsManagerToInitialise()
        {
            // Assert
            this.MockTeamProjectDocumentManager.Verify(mgr => mgr.Initialise(), Times.Once());
        }

        /// <summary>
        /// Initialise tells the word application to initialise.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseTellsWordApplicationToInitialise()
        {
            // Assert
            this.MockApplication.Verify(app => app.Initialise(), Times.Once());
        }

        /// <summary>
        /// Initialise tells the ribbon presenter to initialise.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseTellsRibbonPresenterToInitialise()
        {
            // Assert
            this.MockRibbonPresenter.Verify(ribbon => ribbon.Initialise(It.IsAny<Func<Uri>>()), Times.Once());
        }

        /// <summary>
        /// Initialise does not try to copy system template to roaming profile if installed system template is missing.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseDoesNotTryToCopySystemTemplateToRoamingProfileIfInstalledSystemTemplateIsMissing()
        {
            // Arrange
            this.MockFile.Setup(f => f.Exists(this.roamingTemplatePath)).Returns(false);
            this.MockFile.Setup(f => f.Exists(this.sourceTemplatePath)).Returns(false);

            // Act
            this.Sut.Initialise();

            // Assert
            this.AssertTemplateNotCopied();
        }

        /// <summary>
        /// Initialise copies system template to roaming profile if it is not already there and makes the copy writable.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseCopiesSystemTemplateToRoamingProfileIfItIsNotAlreadyThereAndMakesItWritable()
        {
            // Arrange
            this.SetupNoRoamingTemplate();

            // Act
            this.Sut.Initialise();

            // Assert
            this.AssertTemplateIsCopied();
            this.AssertTemplateIsCopiedWritable();
        }

        /// <summary>
        /// Initialise does not copy system template to roaming profile if it is already there and was last written at the same time.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseDoesNotCopySystemTemplateToRoamingProfileIfItIsAlreadyThereAndUnchanged()
        {
            // Arrange
            this.SetupWithRoamingTemplate(this.installDirTemplateFileTimeStamp);

            // Act
            this.Sut.Initialise();

            // Assert
            this.AssertTemplateNotCopied();
        }

        /// <summary>
        /// Initialise does not copy system template to roaming profile if it is already there and was more recently.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseDoesNotCopySystemTemplateToRoamingProfileIfItIsAlreadyThereAndRoamingOneIsMoreRecent()
        {
            // Arrange
            this.SetupWithRoamingTemplate(this.newerRoamingTemplateFileTimeStamp);

            // Act
            this.Sut.Initialise();

            // Assert
            this.AssertTemplateNotCopied();
        }

        /// <summary>
        /// Initialise prompts user for a system template upgrade decision if the system template is newer than the template in the roaming profile and the settings don't prohibit a prompt for this file.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialisePromptsForSystemTemplateUpgradeDecisionIfSystemTemplateIsNewerThanTemplateInRoamingProfile()
        {
            // Arrange
            this.SetupWithRoamingTemplate(this.olderRoamingTemplateFileTimeStamp);
            this.SetupSettingForTemplateUpgrade(null);

            // Act
            this.Sut.Initialise();

            // Assert
            this.AssertUserIsPromptedToUpgradeTemplate();
        }

        /// <summary>
        /// Initialise does not overwrite an older roaming profile template if the user replies No to the upgrade prompt.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseDoesNotOverwriteOlderRoamingProfileTemplateIfUserRefusesUpgrade()
        {
            // Arrange
            this.SetupWithRoamingTemplate(this.olderRoamingTemplateFileTimeStamp);
            this.SetupYesNoAnswer(false);

            // Act
            this.Sut.Initialise();

            // Assert
            this.AssertTemplateNotCopied();
        }

        /// <summary>
        /// Initialise disables future prompts if user refuses upgrade.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseDisablesFuturePromptsIfUserRefusesUpgrade()
        {
            // Arrange
            this.SetupWithRoamingTemplate(this.olderRoamingTemplateFileTimeStamp);
            this.SetupSettingForTemplateUpgrade(null);
            this.SetupYesNoAnswer(false);

            // Act
            this.Sut.Initialise();

            // Assert
            this.MockSettings.VerifySet(s => s.IgnoreSystemTemplateUpgradeFor = this.installDirTemplateFileTimeStamp, "Setting not updated.");
        }

        /// <summary>
        /// Initialise overwrites an older roaming profile template if the user replies Yes to the upgrade prompt. The new file is writable.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseOverwritesOlderRoamingProfileTemplateIfUserAcceptsUpgradeNewFileIsWritable()
        {
            // Arrange
            this.SetupWithRoamingTemplate(this.olderRoamingTemplateFileTimeStamp);
            this.SetupYesNoAnswer(true);

            // Act
            this.Sut.Initialise();

            // Assert
            this.AssertTemplateIsCopied();
            this.AssertTemplateIsCopiedWritable();
        }

        /// <summary>
        /// Initialise objects in correct order.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseInitialisesObjectsInCorrectOrder()
        {
            // Arrange
            int sequence = 0;
            this.MockFile.Setup(f => f.Exists(this.sourceTemplatePath)).Callback(() => Assert.AreEqual<int>(0, sequence++, "Must process template copy first")).Returns(true);
            this.MockApplication.Setup(app => app.Initialise()).Callback(() => Assert.AreEqual<int>(1, sequence++, "Must initialise word application after processing template copy"));
            this.MockTeamProjectDocumentManager.Setup(mgr => mgr.Initialise()).Callback(() => Assert.AreEqual<int>(2, sequence++, "Must initialise manager after initialising application"));
            this.MockRibbonPresenter.Setup(p => p.LoadState()).Callback(() => Assert.AreEqual<int>(3, sequence++, "Must load state after initialising manager"));

            // Act
            this.Sut.Initialise();
        }

        /// <summary>
        /// Initialise does not prompt to upgrade, or overwrite an older roaming profile template if the settings indicate this file is not be copied.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseUpgradeAndOverwriteOfRoamingProfileTemplateNotDoneIfSettingDisablesItForThisFile()
        {
            // Arrange
            this.SetupWithRoamingTemplate(this.olderRoamingTemplateFileTimeStamp);
            this.SetupSettingForTemplateUpgrade(this.installDirTemplateFileTimeStamp);

            // Act
            this.Sut.Initialise();

            // Assert
            this.AssertTemplateNotCopied();
            this.AssertUserIsNotPromptedToUpgradeTemplate();
        }

        /// <summary>
        /// Initialise prompts to upgrade if the settings indicate a different file is not be copied.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseUpgradeAndOverwriteOfRoamingProfileTemplateIsDoneIfSettingsDisableItForDifferentFile()
        {
            // Arrange
            this.SetupWithRoamingTemplate(this.olderRoamingTemplateFileTimeStamp);
            this.SetupSettingForTemplateUpgrade(this.olderRoamingTemplateFileTimeStamp);
            this.SetupYesNoAnswer(true);

            // Act
            this.Sut.Initialise();

            // Assert
            this.AssertTemplateIsCopied();
            this.AssertUserIsPromptedToUpgradeTemplate();
        }

        /// <summary>
        /// Initialise updates upgrade prompt setting if the user refuses upgrade after ignoring a setting for a different file not to be copied.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseUpdatesUpgradePromptSettingAfterUpgradeRefusalAfterIgnoringSettingForADifferentFile()
        {
            // Arrange
            this.SetupWithRoamingTemplate(this.olderRoamingTemplateFileTimeStamp);
            this.SetupSettingForTemplateUpgrade(this.olderRoamingTemplateFileTimeStamp);
            this.SetupYesNoAnswer(false);

            // Act
            this.Sut.Initialise();

            // Assert
            this.MockSettings.VerifySet(s => s.IgnoreSystemTemplateUpgradeFor = this.installDirTemplateFileTimeStamp, "Setting should be updated.");
        }

        /// <summary>
        /// Sets up the mocks so that there is no roaming template.
        /// </summary>
        private void SetupNoRoamingTemplate()
        {
            this.MockFile.Setup(f => f.Exists(this.roamingTemplatePath)).Returns(false);
            this.MockFile.Setup(f => f.Exists(this.sourceTemplatePath)).Returns(true);
        }

        /// <summary>
        /// Sets up the mocks so there is a roaming template with particular timestamp for the roaming template file.
        /// </summary>
        /// <param name="roamingDirTemplateFileTimeStamp">The timestamp of the roaming file.</param>
        private void SetupWithRoamingTemplate(DateTime roamingDirTemplateFileTimeStamp)
        {
            this.MockFile.Setup(f => f.Exists(this.roamingTemplatePath)).Returns(true);
            this.MockFile.Setup(f => f.Exists(this.sourceTemplatePath)).Returns(true);
            this.MockFile.Setup(f => f.GetLastWriteTimeUtc(this.sourceTemplatePath)).Returns(this.installDirTemplateFileTimeStamp);
            this.MockFile.Setup(f => f.GetLastWriteTimeUtc(this.roamingTemplatePath)).Returns(roamingDirTemplateFileTimeStamp);
        }

        /// <summary>
        /// Sets up the mock ribbon presenter to return a specific answer to the yes/no questions.
        /// </summary>
        /// <param name="ans">The user's answer.</param>
        private void SetupYesNoAnswer(bool ans)
        {
            this.MockRibbonPresenter.Setup(ribbon => ribbon.AskYesNoQuestion(It.IsAny<string>())).Returns(ans);
        }

        /// <summary>
        /// Sets up the mock settings with a value for the template upgrade setting.
        /// </summary>
        /// <param name="timestamp">The date time value to set.</param>
        private void SetupSettingForTemplateUpgrade(Nullable<DateTime> timestamp)
        {
            this.MockSettings.Setup(s => s.IgnoreSystemTemplateUpgradeFor).Returns(timestamp);
        }

        /// <summary>
        /// Asserts that the installed template file is not copied to the roaming template.
        /// </summary>
        private void AssertTemplateNotCopied()
        {
            this.MockFile.Verify(f => f.Copy(this.sourceTemplatePath, this.roamingTemplatePath, true), Times.Never(), "The installed template should not be copied.");
        }

        /// <summary>
        /// Asserts that the installed template file is copied to the roaming template.
        /// </summary>
        private void AssertTemplateIsCopied()
        {
            this.MockFile.Verify(f => f.Copy(this.sourceTemplatePath, this.roamingTemplatePath, true), Times.Once(), "The installed template must be copied.");
        }

        /// <summary>
        /// Asserts that the installed template is copied to the roaming template.
        /// </summary>
        private void AssertTemplateIsCopiedWritable()
        {
            this.MockFile.Verify(f => f.SetWritable(this.roamingTemplatePath), Times.Once(), "The roaming template must be writable.");
        }
        
        /// <summary>
        /// Asserts that the user is prompted to upgrade the template.
        /// </summary>
        private void AssertUserIsPromptedToUpgradeTemplate()
        {
            this.MockRibbonPresenter.Verify(rp => rp.AskYesNoQuestion(It.IsAny<string>()), Times.Once(), "The user was not prompted about upgrading the template.");
        }

        /// <summary>
        /// Asserts that the user is not prompted to upgrade the template.
        /// </summary>
        private void AssertUserIsNotPromptedToUpgradeTemplate()
        {
            this.MockRibbonPresenter.Verify(rp => rp.AskYesNoQuestion(It.IsAny<string>()), Times.Never(), "The user was prompted about upgrading the template.");
        }
    }
}
