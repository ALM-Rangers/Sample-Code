//---------------------------------------------------------------------
// <copyright file="AsmxRectangle.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The AsmxRectangle type.</summary>
//---------------------------------------------------------------------

namespace AsmxWebService
{
    public class AsmxRectangle : AsmxShape
    {
        private int verticalSideLength;
        private int horizontalSideLength;

        public int VerticalSideLength
        {
            get { return this.verticalSideLength; }
            set { this.verticalSideLength = value; }
        }

        public int HorizontalSideLength
        {
            get { return this.horizontalSideLength; }
            set { this.horizontalSideLength = value; }
        }
    }
}
