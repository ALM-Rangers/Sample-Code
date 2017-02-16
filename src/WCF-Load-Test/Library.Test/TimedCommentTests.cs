//---------------------------------------------------------------------
// <copyright file="TimedCommentTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The TimedCommentTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Microsoft.WcfUnit.Library;

    [TestClass]
    public class TimedCommentTests
    {
        [TestMethod]
        public void TimedCommentHasATimestampProperty()
        {
            TimedComment tc = new TimedComment(new DateTime(2008, 9, 15, 12, 0, 0, DateTimeKind.Utc), "Comment");
            Assert.AreEqual<DateTime>(new DateTime(2008, 9, 15, 12, 0, 0, DateTimeKind.Utc), tc.Timestamp);
        }

        [TestMethod]
        public void TimedCommentHasACommentProperty()
        {
            TimedComment tc = new TimedComment(new DateTime(2008, 9, 15, 12, 0, 0, DateTimeKind.Utc), "Comment");
            Assert.AreEqual<string>("Comment", tc.Comment);
        }
    }
}
