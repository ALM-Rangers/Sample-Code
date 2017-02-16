//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The main program.</summary>
//---------------------------------------------------------------------

namespace Client
{
    using System;
    using System.ServiceModel;
    using GeneratedContracts;

    /// <summary>
    /// The main program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main program for the test.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            ChannelFactory<GeneratedContracts.ICollections> ccf = new ChannelFactory<GeneratedContracts.ICollections>("Collections");
            ChannelFactory<GeneratedContracts.IArithmetic> cf = new ChannelFactory<GeneratedContracts.IArithmetic>("Basic");
            GeneratedContracts.IArithmetic ia = cf.CreateChannel();
            GeneratedContracts.ICollections ic = ccf.CreateChannel();

            AddRequest r = new AddRequest();
            r.A = 10;
            r.B = 5;
            Console.WriteLine(ia.Add(r).Answer);
            Console.WriteLine(ia.Add2(r));
            Console.WriteLine(ia.Add3(20, 25));
        }
    }
}
