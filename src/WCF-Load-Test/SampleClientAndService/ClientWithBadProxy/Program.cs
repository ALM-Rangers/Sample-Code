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
    using Contracts;

    public class Program
    {
        public static void Main(string[] args)
        {
            ChannelFactory<Contracts.ICollections> ccf = new ChannelFactory<Contracts.ICollections>("Collections");
            ChannelFactory<Contracts.IArithmetic> cf = new ChannelFactory<Contracts.IArithmetic>("Basic");
            Contracts.IArithmetic ia = cf.CreateChannel();
            Contracts.ICollections ic = ccf.CreateChannel();

            AddRequest r = new AddRequest();
            r.A = 10;
            r.B = 5;
            Console.WriteLine(ia.Add(r).Answer);
            Console.WriteLine(ia.Add2(r));
            Console.WriteLine(ia.Add3(20, 25));
        }
    }
}
