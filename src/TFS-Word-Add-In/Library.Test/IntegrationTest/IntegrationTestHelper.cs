//---------------------------------------------------------------------
// <copyright file="IntegrationTestHelper.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IntegrationTestHelper type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Helper for integration-specific tests.
    /// </summary>
    public class IntegrationTestHelper
    {
        /// <summary>
        /// The Uri of the team project collection to use in integration tests.
        /// </summary>
        private static Uri tpcUri = new Uri("https://vstf-eu-dub-01.partners.extranet.microsoft.com:8443/tfs/ext04_consol_tpc");

        /// <summary>
        /// Gets the team project collection to use in the integration tests.
        /// </summary>
        public static TfsTeamProjectCollection Tpc
        {
            get
            {
                return new TfsTeamProjectCollection(tpcUri);
            }
        }
    }
}
