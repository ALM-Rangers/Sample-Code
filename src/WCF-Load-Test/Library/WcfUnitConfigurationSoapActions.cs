//---------------------------------------------------------------------
// <copyright file="WcfUnitConfigurationSoapActions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WcfUnitConfigurationSoapActions type.</summary>
//---------------------------------------------------------------------

/// <summary>
/// Extends <see cref="WcfUnitConfigurationSoapActions"/> type with counting methods.
/// </summary>
public partial class WcfUnitConfigurationSoapActions
{
    /// <summary>
    /// Gets the number of selected soap actions.
    /// </summary>
    /// <value>The number of selected soap actions.</value>
    public int SelectedCount
    {
        get
        {
            int count = 0;
            if (this.soapAction != null)
            {
                foreach (SoapActionType sat in this.soapAction)
                {
                    if (sat.Selected)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }

    /// <summary>
    /// Gets the number of unselected soap actions.
    /// </summary>
    /// <value>The number of unselected soap actions.</value>
    public int UnselectedCount
    {
        get
        {
            int count = 0;
            if (this.soapAction != null)
            {
                foreach (SoapActionType sat in this.soapAction)
                {
                    if (!sat.Selected)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}