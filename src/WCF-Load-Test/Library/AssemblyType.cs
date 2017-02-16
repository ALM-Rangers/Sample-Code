//---------------------------------------------------------------------
// <copyright file="AssemblyType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The AssemblyType type.</summary>
//---------------------------------------------------------------------

/// <summary>
/// Extends the <see cref="AssemblyType"/> type.
/// </summary>
public partial class AssemblyType
{
    /// <summary>
    /// Initialises a new instance of the <see cref="AssemblyType"/> class.
    /// </summary>
    public AssemblyType()
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="AssemblyType"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    public AssemblyType(string fileName)
    {
        this.fileNameField = fileName;
    }
}
