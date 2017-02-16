//---------------------------------------------------------------------
// <copyright file="TimingsFileTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The TimingsFileTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.Globalization;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;

    [TestClass]
    public class TimingsFileTests
    {
        [TestMethod]
        public void TimingsFileHowToFormatTimestamp()
        {
            DateTime testTime = new DateTime(2008, 9, 15, 13, 01, 02, DateTimeKind.Utc);
            Assert.AreEqual<string>("2008-09-15T13:01:02.000000Z", testTime.ToUniversalTime().ToString(@"yyyy-MM-dd\THH:mm:ss.ffffffK", CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void TimingsFileReadATimedCommentReturnsNullWhenFileIsEmpty()
        {
            Stream file = TestHelper.BuildFile(string.Empty);
            TimingsFile tf = new TimingsFile(file);

            Assert.IsNull(tf.NextTimedComment());
        }

        [TestMethod]
        public void TimingsFileImplementsIDisposable()
        {
            Stream file = TestHelper.BuildFile(string.Empty);
            using (TimingsFile tf = new TimingsFile(file))
            {
            }

            TestHelper.TestForObjectDisposedException(
                delegate
                {
                    file.ReadByte();
                });
        }

        [TestMethod]
        public void TimingsFileTimestampReadCorrectlyFromRecord()
        {
            Stream file = TestHelper.BuildFile("2008-09-15T12:01:02.003000Z,Comment\n");
            TimingsFile tf = new TimingsFile(file);

            DateTime expectedTime = new DateTime(2008, 9, 15, 12, 1, 2, 3, DateTimeKind.Utc);
            Assert.AreEqual<DateTime>(expectedTime, tf.NextTimedComment().Timestamp);
        }

        [TestMethod]
        public void TimingsFileCommentReadCorrectlyFromRecord()
        {
            Stream file = TestHelper.BuildFile("2008-09-15T12:01:02.003000ZComment\n");
            TimingsFile tf = new TimingsFile(file);

            Assert.AreEqual<string>("Comment", tf.NextTimedComment().Comment);
        }

        [TestMethod]
        public void TimingsFileTimestampReadCorrectlyFromSecondRecord()
        {
            Stream file = TestHelper.BuildFile("2008-09-15T12:01:02.003000ZComment\n2008-10-15T12:01:02.003000ZComment2\n");
            TimingsFile tf = new TimingsFile(file);
            tf.NextTimedComment();

            DateTime expectedTime = new DateTime(2008, 10, 15, 12, 1, 2, 3, DateTimeKind.Utc);
            Assert.AreEqual<DateTime>(expectedTime, tf.NextTimedComment().Timestamp);
        }

        [TestMethod]
        public void TimingsFileCommentReadCorrectlyFromSecondRecord()
        {
            Stream file = TestHelper.BuildFile("2008-09-15T12:01:02.003000ZComment\n2008-10-15T12:01:02.003000ZComment2\n");
            TimingsFile tf = new TimingsFile(file);
            tf.NextTimedComment();

            Assert.AreEqual<string>("Comment2", tf.NextTimedComment().Comment);
        }

        [TestMethod]
        public void TimingsFileNullReturnedAtEndOfMultiRecordFile()
        {
            Stream file = TestHelper.BuildFile("2008-09-15T12:01:02.003000ZComment\n2008-10-15T12:01:02.003000ZComment2\n");
            TimingsFile tf = new TimingsFile(file);
            tf.NextTimedComment();
            tf.NextTimedComment();

            Assert.IsNull(tf.NextTimedComment());
        }
    }
}
