//---------------------------------------------------------------------
// <copyright file="SettingsTests.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The QueryUtilitiesTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System;
    using System.Globalization;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model;
    using Moq;

    /// <summary>
    /// Tests the <see cref="SettingsTests"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class SettingsTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock word document used to test the model.
        /// </summary>
        private Mock<ILowLevelSettings> mockLowLevelSettings;

        /// <summary>
        /// The model to be tested.
        /// </summary>
        private ISettings sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.mockLowLevelSettings = TestHelper.CreateAndRegisterMock<ILowLevelSettings>(this.container);

            this.sut = this.container.Resolve<Settings>();
        }

        /// <summary>
        /// Cleans up the test, disposing the Unity container.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            this.container.Dispose();
        }

        /// <summary>
        /// The <see cref="ISettings.ShowBookmarks"/> setting is null if the low level setting is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowBookmarksIsNullIfTheLowLevelSettingIsNull()
        {
            // Arrange
            this.mockLowLevelSettings.Setup(s => s[Constants.ShowBookmarksSettingName]).Returns(null);

            // Act
            Nullable<bool> ans = this.sut.ShowBookmarks;

            // Assert
            Assert.IsFalse(ans.HasValue, "value should be null if null in the low level settings");
        }

        /// <summary>
        /// The <see cref="ISettings.ShowBookmarks"/> setting is null if the low level setting is not a boolean.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowBookmarksIsNullIfTheSettingIsNotAValidBoolean()
        {
            // Arrange
            this.mockLowLevelSettings.Setup(s => s[Constants.ShowBookmarksSettingName]).Returns("abc");

            // Act
            Nullable<bool> ans = this.sut.ShowBookmarks;

            // Assert
            Assert.IsFalse(ans.HasValue, "value should be null if invalid in the low level settings");
        }

        /// <summary>
        /// The <see cref="ISettings.ShowBookmarks"/> setting is true if the low level setting is true.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowBookmarksIsTrueIfTheSettingIsTrue()
        {
            // Arrange
            this.mockLowLevelSettings.Setup(s => s[Constants.ShowBookmarksSettingName]).Returns("true");

            // Act
            Nullable<bool> ans = this.sut.ShowBookmarks;

            // Assert
            Assert.IsTrue(ans.Value, "value should be true if true in the low level settings");
        }

        /// <summary>
        /// The <see cref="ISettings.ShowBookmarks"/> setting is false if the low level setting is false.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowBookmarksIsFalseIfTheSettingIsFalse()
        {
            // Arrange
            this.mockLowLevelSettings.Setup(s => s[Constants.ShowBookmarksSettingName]).Returns("false");

            // Act
            Nullable<bool> ans = this.sut.ShowBookmarks;

            // Assert
            Assert.IsFalse(ans.Value, "value should be false if false in the low level settings");
        }

        /// <summary>
        /// The <see cref="ISettings.IgnoreSystemTemplateUpgradeFor"/> setting is null if the low level setting is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IgnoreSystemTemplateUpgradeForIsNullIfTheLowLevelSettingIsNull()
        {
            // Arrange
            this.mockLowLevelSettings.Setup(s => s[Constants.TemplateUpgradeSettingName]).Returns(null);

            // Act
            Nullable<DateTime> ans = this.sut.IgnoreSystemTemplateUpgradeFor;

            // Assert
            Assert.IsFalse(ans.HasValue, "value should be null if null in the low level settings");
        }

        /// <summary>
        /// The <see cref="ISettings.IgnoreSystemTemplateUpgradeFor"/> setting is null if the low level setting is not a <see cref="DateTime"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IgnoreSystemTemplateUpgradeForIsNullIfTheSettingIsNotAValidDateTime()
        {
            // Arrange
            this.mockLowLevelSettings.Setup(s => s[Constants.TemplateUpgradeSettingName]).Returns("abc");

            // Act
            Nullable<DateTime> ans = this.sut.IgnoreSystemTemplateUpgradeFor;

            // Assert
            Assert.IsFalse(ans.HasValue, "value should be null if invalid in the low level settings");
        }

        /// <summary>
        /// The <see cref="ISettings.IgnoreSystemTemplateUpgradeFor"/> setting is the DateTime of the low level setting DateTime value.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IgnoreSystemTemplateUpgradeForFollowsLowLevelSettingValue()
        {
            // Arrange
            DateTime testDateTime = DateTime.Now;
            this.mockLowLevelSettings.Setup(s => s[Constants.TemplateUpgradeSettingName]).Returns(testDateTime.ToString("O", CultureInfo.InvariantCulture));

            // Act
            Nullable<DateTime> ans = this.sut.IgnoreSystemTemplateUpgradeFor;

            // Assert
            Assert.AreEqual<DateTime>(testDateTime, ans.Value, "value should follow the low level setting");
        }

        /// <summary>
        /// The low level settings are updated for <see cref="ISettings.IgnoreSystemTemplateUpgradeFor"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IgnoreSystemTemplateUpgradeForSettingWritesDateTimeStringToLowLevel()
        {
            // Arrange
            DateTime testDateTime = DateTime.Now;

            // Act
            this.sut.IgnoreSystemTemplateUpgradeFor = testDateTime;

            // Assert
            this.mockLowLevelSettings.VerifySet(s => s[Constants.TemplateUpgradeSettingName] = testDateTime.ToString("O", CultureInfo.InvariantCulture), "Setting not updated.");
        }
    }
}