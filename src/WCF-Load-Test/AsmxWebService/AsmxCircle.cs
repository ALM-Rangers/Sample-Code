//---------------------------------------------------------------------
// <copyright file="AsmxCircle.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The AsmxCircle type.</summary>
//---------------------------------------------------------------------

namespace AsmxWebService
{
    public class AsmxCircle : AsmxShape
    {
        private int radius;

        public int Radius
        {
            get { return this.radius; }
            set { this.radius = value; }
        }

        public override string Ignored
        {
            get
            {
                return base.Ignored;
            }

            set
            {
                base.Ignored = value;
            }
        }
    }
}
