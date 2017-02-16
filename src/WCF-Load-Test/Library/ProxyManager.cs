//---------------------------------------------------------------------
// <copyright file="ProxyManager.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ProxyManager type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper class to get information about the WCF proxy.
    /// </summary>
    public class ProxyManager
    {
        /// <summary>
        /// The list of proxy assemblies the manager is currently aware of.
        /// </summary>
        private Assembly[] proxyAssemblies;

        /// <summary>
        /// Initialises a new instance of the <see cref="ProxyManager"/> class.
        /// </summary>
        /// <param name="assemblyFileNames">The files containing the proxy assemblies.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom", Justification = "The nature of the program requires this call to be used")]
        public ProxyManager(params string[] assemblyFileNames)
        {
            if (assemblyFileNames == null)
            {
                throw new ArgumentNullException("assemblyFileNames");
            }

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.AssemblyResolveHandler);
            this.proxyAssemblies = new Assembly[assemblyFileNames.Length];
            for (int i = 0; i < assemblyFileNames.Length; i++)
            {
                if (!File.Exists(assemblyFileNames[i]))
                {
                    throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.ProxyManager_FileNotFound, assemblyFileNames[i]));
                }

                try
                {
                    this.proxyAssemblies[i] = Assembly.LoadFrom(assemblyFileNames[i]);
                }
                catch (BadImageFormatException bife)
                {
                    throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.ProxyManager_InvalidAssembly, assemblyFileNames[i]), bife);
                }
            }
        }

        /// <summary>
        /// Raised when a type is resolved from a proxy assembly to inform the subscriber of the
        /// assembly that contained the type.
        /// </summary>
        public event EventHandler<ProxyAssemblyEventArgs> TypeResolved;

        /// <summary>
        /// Determines whether a method uses the message contract or not.
        /// </summary>
        /// <param name="contractMethod">The method to check.</param>
        /// <returns>True if <paramref name="contractMethod"/> uses the message contract.</returns>
        public static bool IsMessageContractMethod(MethodBase contractMethod)
        {
            if (contractMethod == null)
            {
                throw new ArgumentNullException("contractMethod");
            }

            bool ans = false;

            ParameterInfo[] parameters = contractMethod.GetParameters();

            ans = parameters.Length == 1 && Attribute.GetCustomAttribute(parameters[0].ParameterType, typeof(MessageContractAttribute)) != null;

            return ans;
        }

        // TODO: Unified method of checking format, include interactions with message contracts.

        /// <summary>
        /// Determines whether a method uses the XmlSerializer format or not.
        /// </summary>
        /// <param name="contractMethod">The method to check.</param>
        /// <returns>True if <paramref name="contractMethod"/> uses the XmlSerializer format.</returns>
        public static bool IsXmlSerializerMethod(MethodBase contractMethod)
        {
            if (contractMethod == null)
            {
                throw new ArgumentNullException("contractMethod");
            }

            bool ans = false;

            if (MethodTypeHasXmlSerializerFormatAttribute(contractMethod))
            {
                if (!MethodHasDataContractFormatAttribute(contractMethod))
                {
                    ans = true;
                }
            }
            else if (MethodHasXmlSerializerFormatAttribute(contractMethod))
            {
                ans = true;
            }

            return ans;
        }

        /// <summary>
        /// Gets the type object for a named type from the proxy assembly.
        /// </summary>
        /// <param name="typeName">The name of the type to retrieve.</param>
        /// <returns>The corresponding type object.</returns>
        public Type GetProxyType(string typeName)
        {
            Type ans = null;
            foreach (Assembly a in this.proxyAssemblies)
            {
                Type[] exportedTypes = a.GetExportedTypes();
                foreach (Type t in exportedTypes)
                {
                    if (t.FullName == typeName)
                    {
                        ans = t;
                        break;
                    }
                }

                if (ans != null)
                {
                    this.OnTypeResolved(a);
                    break;
                }
            }

            return ans;
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> for the proxy method implementation of the contract method.
        /// </summary>
        /// <remarks>
        /// Looks for a class that implements the contract and is also derived from ICommunicationObject.
        /// </remarks>
        /// <param name="contractMethod">The <see cref="MethodBase"/> for the contract method.</param>
        /// <returns>The <see cref="MethodInfo"/> object for the proxy method, or null if not found.</returns>
        public MethodInfo GetProxyMethod(MethodBase contractMethod)
        {
            if (contractMethod == null)
            {
                throw new ArgumentNullException("contractMethod");
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Looking for proxy method for method {0} on {1}", contractMethod.Name, contractMethod.DeclaringType.ToString()));

            MethodInfo ans = null;

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Contract method is defined on an interface, now looking for the class"));
            Type[] exportedTypes = null;
            foreach (Assembly a in this.proxyAssemblies)
            {
                ////Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Checking assembly {0}", a.FullName));
                try
                {
                    exportedTypes = a.GetExportedTypes();
                    foreach (Type t in exportedTypes)
                    {
                        ////Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Checking if {0} implements the proxy", t.FullName));
                        if (TypeIsAProxyClass(contractMethod.DeclaringType, t))
                        {
                            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "This class implements the proxy"));

                            // Need to call overload of the GetMethod method with a type array because the contract may
                            // have overloaded method names which vary only by the SOAP action (using Name= or Action= on
                            // the OperationContract attribute.
                            ParameterInfo[] paramInfo = contractMethod.GetParameters();
                            Type[] parameterTypes = new Type[paramInfo.Length];
                            for (int i = 0; i < paramInfo.Length; i++)
                            {
                                parameterTypes[i] = paramInfo[i].ParameterType;
                            }

                            ans = t.GetMethod(contractMethod.Name, parameterTypes);
                            if (ans == null)
                            {
                                // Lets see if it an explicit interface implementation
                                InterfaceMapping im = t.GetInterfaceMap(contractMethod.DeclaringType);

                                // Get list of methods which match by name and then select by type
                                List<MethodBase> matchingMethods = new List<MethodBase>();
                                Regex pattern = new Regex(string.Concat(@"^(", Utility.IdentifierPatternString, @"\.)*", contractMethod.DeclaringType.Name, @"\.", contractMethod.Name, "$"));
                                foreach (MethodBase targetMethod in im.TargetMethods)
                                {
                                    if (pattern.IsMatch(targetMethod.Name))
                                    {
                                        matchingMethods.Add(targetMethod);
                                    }
                                }

                                MethodBase mb = Type.DefaultBinder.SelectMethod(BindingFlags.Instance, matchingMethods.ToArray(), parameterTypes, null);
                                ans = mb as MethodInfo;
                                if (ans != null)
                                {
                                    Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Interface method is implemented explicitly by {0}", ans.Name));
                                }
                            }

                            break;
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    // This means we could not load the exported types, so we just ignore this
                    Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Could not get exported types for {0} as the assembly could not be found/loaded", a.FullName));
                }

                if (ans != null)
                {
                    this.OnTypeResolved(a);
                    break;
                }
            }

            if (ans == null)
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Failed to find proxy method for method {0}", contractMethod.Name));
            }
            else
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Proxy method for method {0} found in {1}", contractMethod.Name, ans.ReflectedType.ToString()));
            }

            return ans;
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> for the method on the contract interface or class that has the
        /// given SOAP action in the proxy.
        /// </summary>
        /// <param name="soapAction">The SOAP action to look for.</param>
        /// <returns>The <see cref="MethodInfo"/> object for the method, or null if not found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "In this case the null and empty string cases have to be treated differently")]
        public MethodInfo GetContractMethod(string soapAction)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Looking for contract method for SOAP action {0}", soapAction));
            MethodInfo ans = null;
            Type[] exportedTypes = null;

            foreach (Assembly a in this.proxyAssemblies)
            {
                ////Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Checking assembly {0}", a.FullName));
                try
                {
                    exportedTypes = a.GetExportedTypes();
                    foreach (Type t in exportedTypes)
                    {
                        ServiceContractAttribute serviceContractAttribute = (ServiceContractAttribute)Attribute.GetCustomAttribute(t, typeof(ServiceContractAttribute));
                        if (serviceContractAttribute != null)
                        {
                            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Found service contract on {0}", t.FullName));
                            string defaultSoapActionPrefix;

                            // Use ServiceContract Name attribute if present.   
                            string formatArgName = null;
                            if (!string.IsNullOrEmpty(serviceContractAttribute.Name))
                            {
                                formatArgName = serviceContractAttribute.Name;
                            }
                            else
                            {
                                formatArgName = t.Name;
                            }

                            if (serviceContractAttribute.Namespace == null)
                            {
                                defaultSoapActionPrefix = string.Format(CultureInfo.InvariantCulture, "http://tempuri.org/{0}/", formatArgName);
                            }
                            else if (serviceContractAttribute.Namespace == string.Empty)
                            {
                                defaultSoapActionPrefix = string.Format(CultureInfo.InvariantCulture, "urn:{0}/", formatArgName);
                            }
                            else
                            {
                                if (serviceContractAttribute.Namespace.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                                {
                                    defaultSoapActionPrefix = string.Format(CultureInfo.InvariantCulture, "{0}{1}/", serviceContractAttribute.Namespace, formatArgName);
                                }
                                else
                                {
                                    defaultSoapActionPrefix = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/", serviceContractAttribute.Namespace, formatArgName);
                                }
                            }

                            foreach (MethodInfo mi in t.GetMethods())
                            {
                                ////Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Checking method {0}", mi.Name));
                                MethodInfo baseMethod = mi.GetBaseDefinition();
                                OperationContractAttribute attr = (OperationContractAttribute)Attribute.GetCustomAttribute(baseMethod, typeof(OperationContractAttribute));
                                if (attr != null)
                                {
                                    string operationAction;
                                    if (attr.Action != null)
                                    {
                                        operationAction = attr.Action;
                                    }
                                    else
                                    {
                                        if (attr.Name != null)
                                        {
                                            operationAction = defaultSoapActionPrefix + attr.Name;
                                        }
                                        else
                                        {
                                            operationAction = defaultSoapActionPrefix + mi.Name;
                                        }
                                    }

                                    if (operationAction == soapAction)
                                    {
                                        Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Found service contract, type is {0}", t.FullName));
                                        ans = mi;
                                        break;
                                    }
                                }
                            }
                        }

                        if (ans != null)
                        {
                            break;
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    // This means we could not load the exported types, so we just ignore this
                    Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Could not get exported types for {0} as the assembly could not be found/loaded", a.FullName));
                }

                if (ans != null)
                {
                    this.OnTypeResolved(a);
                    break;
                }
            }

            if (ans == null)
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Failed to find contract method for SOAP action {0}", soapAction));
            }
            else
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Contract method for SOAP action {0} is {1} in {2}", soapAction, ans.Name, ans.DeclaringType.ToString()));
            }

            return ans;
        }

        /// <summary>
        /// Determines if the candidate type is a proxy rather than an interface.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <param name="candidateType">The type being checked.</param>
        /// <returns>True if <paramref name="candidateType"/> is a proxy and not an interface.</returns>
        private static bool TypeIsAProxyClass(Type contractType, Type candidateType)
        {
            return
                !candidateType.IsInterface
                &&
                (candidateType.GetInterface(contractType.FullName) != null
                 ||
                 candidateType.IsSubclassOf(contractType)
                 ||
                 candidateType == contractType)
                &&
                candidateType.GetInterface(typeof(ICommunicationObject).FullName) != null;
        }

        /// <summary>
        /// Determines if the type where a method is defined is serialized using XML.
        /// </summary>
        /// <param name="contractMethod">The method being checked.</param>
        /// <returns>True if the type where <paramref name="contractMethod"/> defined is serialized using XML.</returns>
        private static bool MethodTypeHasXmlSerializerFormatAttribute(MethodBase contractMethod)
        {
            return Attribute.GetCustomAttribute(contractMethod.DeclaringType, typeof(XmlSerializerFormatAttribute)) != null;
        }

        /// <summary>
        /// Determines if a method is serialized using XML.
        /// </summary>
        /// <param name="contractMethod">The method being checked.</param>
        /// <returns>True if <paramref name="contractMethod"/> is serialized using XML.</returns>
        private static bool MethodHasXmlSerializerFormatAttribute(MethodBase contractMethod)
        {
            return Attribute.GetCustomAttribute(contractMethod, typeof(XmlSerializerFormatAttribute)) != null;
        }

        /// <summary>
        /// Determines if a method is serialized using Data Contracts.
        /// </summary>
        /// <param name="contractMethod">The method being checked.</param>
        /// <returns>True if <paramref name="contractMethod"/> is serialized using Data Contracts.</returns>
        private static bool MethodHasDataContractFormatAttribute(MethodBase contractMethod)
        {
            return Attribute.GetCustomAttribute(contractMethod, typeof(DataContractFormatAttribute)) != null;
        }

        /// <summary>
        /// Handles assembly resolve events.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event arguments.</param>
        /// <returns>The resolved assembly.</returns>
        private Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Assembly resolve handler called for {0}", args.Name));
            Assembly ans = null;

            // Check if already loaded using LoadFrom and return it, not sure why this is needed
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (args.Name == a.FullName)
                {
                    Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Assembly resolve handler found it in the already loaded assemblies"));
                    ans = a;
                    break;
                }
            }

            if (ans == null)
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Assembly resolve handler failed for {0}", args.Name));
            }
            else
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Assembly resolve handler for {0} found {1}", args.Name, ans.FullName));
            }

            return ans;
        }

        /// <summary>
        /// Processes resolution of an assembly by raising an event.
        /// </summary>
        /// <param name="a">The assembly that has been resolved.</param>
        private void OnTypeResolved(Assembly a)
        {
            if (this.TypeResolved != null)
            {
                ProxyAssemblyEventArgs args = new ProxyAssemblyEventArgs();
                args.Assembly = a;
                this.TypeResolved(this, args);
            }
        }
    }
}
