//---------------------------------------------------------------------
// <copyright file="WcfUnitConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WcfUnitConfiguration type.</summary>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.WcfUnit.Library;

/// <summary>
/// Extends the <see cref="WcfUnitConfiguration"/> type with methods to add and remove assemblies
/// </summary>
public partial class WcfUnitConfiguration
{
    /// <summary>
    /// Adds the file name to the list of selected assemblies.
    /// </summary>
    /// <param name="fileName">The file name of the assembly to be added.</param>
    public void AddAssembly(string fileName)
    {
        List<AssemblyType> temp;
        if (this.assembly == null)
        {
            temp = new List<AssemblyType>();
        }
        else
        {
            temp = new List<AssemblyType>(this.assembly);
        }

        temp.Add(new AssemblyType(fileName));

        this.assembly = temp.ToArray();
    }

    /// <summary>
    /// Remove the chosen entry from the list of selected assemblies.
    /// </summary>
    /// <param name="index">The index of the entry in the list of selected assemblies to be removed.</param>
    public void RemoveAssembly(int index)
    {
        if (this.assembly == null)
        {
            throw new InvalidOperationException(Messages.ConfigurationX_CannotRemoveAssemblyFromNullList);
        }
        else if (this.assembly.Length <= index)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Messages.ConfigurationX_CannotRemoveAssemblyIndex, index));
        }

        List<AssemblyType> temp = new List<AssemblyType>(this.assembly);
        temp.RemoveAt(index);
        this.assembly = temp.ToArray();
    }

    /// <summary>
    /// Makes a deep copy of this instance.
    /// </summary>
    /// <returns>A deep copy of this instance.</returns>
    public WcfUnitConfiguration Clone()
    {
        WcfUnitConfiguration ans = new WcfUnitConfiguration();
        ans.clientTrace = this.clientTrace;
        ans.serviceTrace = this.serviceTrace;
        ans.operationTimerMode = this.operationTimerMode;
        ans.testMethodMode = this.testMethodMode;
        if (this.assembly != null)
        {
            for (int i = 0; i < this.assembly.Length; i++)
            {
                ans.AddAssembly(this.assembly[i].fileName);
            }
        }

        if (this.soapActions != null)
        {
            ans.soapActions = new WcfUnitConfigurationSoapActions();
            ans.soapActions.soapActionMode = this.soapActions.soapActionMode;
            if (this.soapActions.soapAction != null)
            {
                ans.soapActions.soapAction = new SoapActionType[this.soapActions.soapAction.Length];
                for (int i = 0; i < this.soapActions.soapAction.Length; i++)
                {
                    ans.soapActions.soapAction[i] = new SoapActionType();
                    ans.soapActions.soapAction[i].action = this.soapActions.soapAction[i].action;
                    ans.soapActions.soapAction[i].Selected = this.soapActions.soapAction[i].Selected;
                }
            }
        }

        if (this.parser != null)
        {
            ans.parser = new typeType();
            ans.parser.assembly = this.parser.assembly;
            ans.parser.type = this.parser.type;
        }

        return ans;
    }
}