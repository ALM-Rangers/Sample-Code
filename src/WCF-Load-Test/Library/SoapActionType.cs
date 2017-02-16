//---------------------------------------------------------------------
// <copyright file="SoapActionType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The SoapActionType type.</summary>
//---------------------------------------------------------------------

using System.Xml.Serialization;

/// <summary>
/// Extends the <see cref="SoapActionType"/> type with a selection flag and overriding Equals
/// </summary>
public partial class SoapActionType
{
    /// <summary>
    /// A value indicating whether the soap action has been selected. Used in the wizard to preserve the selection status
    /// </summary>
    private bool selected;

    /// <summary>
    /// Gets or sets a value indicating whether the soap action has been selected. Used in the wizard to preserve the selection status
    /// </summary>
    /// <value>Selection status.</value>
    [XmlIgnore]
    public bool Selected
    {
        get { return this.selected; }
        set { this.selected = value; }
    }

    /// <summary>
    /// Overrides standard method
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if equal, false otherwise.</returns>
    public override bool Equals(object obj)
    {
        SoapActionType comparand = obj as SoapActionType;
        if (comparand == null)
        {
            return false;
        }

        return this.action == comparand.action;
    }

    /// <summary>
    /// Computes hash code for this object.
    /// </summary>
    /// <returns>The hash code for this object.</returns>
    public override int GetHashCode()
    {
        return this.action.GetHashCode();
    }
}
