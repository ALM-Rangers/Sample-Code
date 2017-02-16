//---------------------------------------------------------------------
// <copyright file="Deserializer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The Deserializer type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Deserializes the objects in a WCF message body,
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deserializer", Justification = "Happy with this spelling")]
    public class Deserializer
    {
        /// <summary>
        /// The string representing a stream body.
        /// </summary>
        private const string StreamBody = "... stream ...";

        /// <summary>
        /// Deserializes the objects in a WCF message body sent to a particular operation of a WCF service.
        /// </summary>
        /// <param name="messageBuffer">A message buffer from which an instance of the traced WCF message can be built</param>
        /// <param name="contractMethod">The <see cref="MethodBase"/> object of the contract method used to call the trace's operation.</param>
        /// <remarks>
        /// <para>
        /// If the contract parameter is a <see cref="System.IO.Stream">Stream</see> then the returned parameter type
        /// will be <see cref="System.IO.Stream">Stream</see>, but the value will be a
        /// <see cref="System.IO.MemoryStream">MemoryStream</see>.
        /// </para>
        /// </remarks>
        /// <returns>Array <see cref="CallParameterInfo"/> objects describing each parameter.</returns>
        public CallParameterInfo[] DeserializeInputParameters(MessageBuffer messageBuffer, MethodBase contractMethod)
        {
            if (contractMethod == null)
            {
                throw new ArgumentNullException("contractMethod");
            }

            Type proxyType = contractMethod.DeclaringType;
            string methodName = contractMethod.Name;
            if (proxyType == null)
            {
                throw new ArgumentException(Messages.DeserializerContractMethodNoDeclaringType);
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Deserializing buffer for method {0} in type {1}", methodName, proxyType.ToString()));
            CallParameterInfo[] ans = null;

            ParameterInfo[] parameters = contractMethod.GetParameters();
            string proxyNamespace = this.GetServiceNamespace(proxyType);

            // TODO: Pluggable: Make choice of deserializer more pluggable, use a factory.
            if (ProxyManager.IsXmlSerializerMethod(contractMethod))
            {
                if (ProxyManager.IsMessageContractMethod(contractMethod))
                {
                    ans = DeserializeXmlSerializerWithBareInputParameter(messageBuffer, proxyNamespace, parameters);
                }
                else
                {
                    ans = DeserializeXmlSerializerInputParameters(messageBuffer, proxyNamespace, parameters);
                }
            }
            else if (ProxyManager.IsMessageContractMethod(contractMethod))
            {
                ans = new CallParameterInfo[] { DeserializeMessageContractInputParameters(messageBuffer, proxyNamespace, parameters[0]) };
            }
            else
            {
                IList<Type> serviceKnownTypes = GetServiceKnownTypes(contractMethod.DeclaringType);
                ans = DeserializeDataContractInputParameters(messageBuffer, proxyNamespace, parameters, serviceKnownTypes);
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Completed deserializing buffer for method {0} in type {1}", methodName, proxyType.ToString()));
            return ans;
        }

        /// <summary>
        /// Gets the service known types.
        /// </summary>
        /// <param name="contractType">The contract types.</param>
        /// <returns>Service known types.</returns>
        private static IList<Type> GetServiceKnownTypes(Type contractType)
        {
            List<Type> ans = new List<Type>();

            object[] attrs = contractType.GetCustomAttributes(typeof(ServiceKnownTypeAttribute), false);
            foreach (object attr in attrs)
            {
                ServiceKnownTypeAttribute skta = (ServiceKnownTypeAttribute)attr;
                ans.Add(skta.Type);
            }

            return ans;
        }

        /// <summary>
        /// Deserializes data contract input parameters.
        /// </summary>
        /// <param name="messageBuffer">The buffer containing the message to be deserialized.</param>
        /// <param name="proxyNamespace">The namespace of the proxy.</param>
        /// <param name="parameters">The parameters to be deserialized.</param>
        /// <param name="serviceKnownTypes">The service known types, if any.</param>
        /// <returns>Deserialized input parameters.</returns>
        private static CallParameterInfo[] DeserializeDataContractInputParameters(MessageBuffer messageBuffer, string proxyNamespace, ParameterInfo[] parameters, IList<Type> serviceKnownTypes)
        {
            List<CallParameterInfo> ans = new List<CallParameterInfo>();

            Message message = messageBuffer.CreateMessage();
            XmlDictionaryReader xdr = message.GetReaderAtBodyContents();

            if (xdr.Value == StreamBody)
            {
                // In a client trace we take this path only if the parameter was a Stream, if it is a
                // MemoryStream then we get a "proper" structure but without the actual data and do not take this
                // branch because xdr.Value will not match the condition.
                //
                // In a service side trace it seems that we take this path for all streams. So in this case
                // the result will be that the contract parameter will always be Stream regardless of actual
                // parameter type.
                foreach (ParameterInfo parameter in parameters)
                {
                    if (typeof(Stream).IsAssignableFrom(parameter.ParameterType))
                    {
                        ans.Add(new CallParameterInfo(parameter.Name, parameter.ParameterType, FieldDirection.In, null));
                    }
                    else
                    {
                        ans.Add(new CallParameterInfo(parameter.Name, parameter.ParameterType, FieldDirection.In, null));
                    }
                }
            }
            else
            {
                // Move to inner node which is next
                while (xdr.Read() && xdr.NodeType != XmlNodeType.Element)
                {
                }

                foreach (ParameterInfo parameter in parameters)
                {
                    string parameterName = GetWireParameterName(parameter);
                    Type parameterType = GetParameterType(parameter);
                    FieldDirection parameterDirection = GetParameterDirection(parameter);

                    object o = null;
                    if (!parameter.IsOut && !typeof(Stream).IsAssignableFrom(parameter.ParameterType))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(parameterType, parameterName, proxyNamespace, serviceKnownTypes);
                        o = serializer.ReadObject(xdr, true);
                    }

                    ans.Add(new CallParameterInfo(parameter.Name, parameterType, parameterDirection, o));
                }
            }

            return ans.ToArray();
        }

        /// <summary>
        /// Deserializes message contract input parameters.
        /// </summary>
        /// <param name="messageBuffer">The buffer containing the message to be deserialized.</param>
        /// <param name="proxyNamespace">The namespace of the proxy.</param>
        /// <param name="parameter">The parameter to be deserialized.</param>
        /// <returns>Deserialized input parameter.</returns>
        private static CallParameterInfo DeserializeMessageContractInputParameters(MessageBuffer messageBuffer, string proxyNamespace, ParameterInfo parameter)
        {
            Message message = messageBuffer.CreateMessage();
            TypedMessageConverter converter = TypedMessageConverter.Create(parameter.ParameterType, message.Headers.Action, proxyNamespace);
            object o;
            try
            {
                o = converter.FromMessage(message);
            }
            catch (System.Xml.XmlException)
            {
                o = Activator.CreateInstance(parameter.ParameterType);
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                o = Activator.CreateInstance(parameter.ParameterType);
            }
            catch (CommunicationException)
            {
                o = Activator.CreateInstance(parameter.ParameterType);
            }

            return new CallParameterInfo(parameter.Name, parameter.ParameterType, FieldDirection.In, o);
        }

        /// <summary>
        /// Deserializes bare input parameters using the <see cref="XmlSerializer"/>.
        /// </summary>
        /// <param name="messageBuffer">The buffer containing the message to be deserialized.</param>
        /// <param name="proxyNamespace">The namespace of the proxy.</param>
        /// <param name="parameters">The parameters to be deserialized.</param>
        /// <returns>Deserialized input parameters.</returns>
        private static CallParameterInfo[] DeserializeXmlSerializerWithBareInputParameter(MessageBuffer messageBuffer, string proxyNamespace, ParameterInfo[] parameters)
        {
            Debug.Assert(parameters.Length == 1, "Expect only one parameter for bare XmlSerializer parameter");
            List<CallParameterInfo> ans = new List<CallParameterInfo>();

            Message message = messageBuffer.CreateMessage();
            foreach (ParameterInfo parameter in parameters)
            {
                TypedMessageConverter converter = TypedMessageConverter.Create(parameter.ParameterType, message.Headers.Action, proxyNamespace, new XmlSerializerFormatAttribute());
                object o = converter.FromMessage(message);
                ans.Add(new CallParameterInfo(parameter.Name, parameter.ParameterType, FieldDirection.In, o));
            }

            return ans.ToArray();
        }

        /// <summary>
        /// Deserializes the input parameters using the <see cref="XmlSerializer"/>.
        /// </summary>
        /// <param name="messageBuffer">The buffer containing the message to be deserialized.</param>
        /// <param name="proxyNamespace">The namespace of the proxy.</param>
        /// <param name="parameters">The parameters to be deserialized.</param>
        /// <returns>Deserialized input parameters.</returns>
        private static CallParameterInfo[] DeserializeXmlSerializerInputParameters(MessageBuffer messageBuffer, string proxyNamespace, ParameterInfo[] parameters)
        {
            List<CallParameterInfo> ans = new List<CallParameterInfo>();

            Message message = messageBuffer.CreateMessage();
            XmlDictionaryReader xdr = message.GetReaderAtBodyContents();

            // Move to inner node which is next
            while (xdr.Read() && xdr.NodeType != XmlNodeType.Element)
            {
            }

            foreach (ParameterInfo parameter in parameters)
            {
                XmlRootAttribute xmlRoot = new XmlRootAttribute();
                xmlRoot.ElementName = parameter.Name;
                xmlRoot.Namespace = proxyNamespace;
                xmlRoot.IsNullable = false;

                // TODO: XmlSerializer: support for ref and out parameters.
                XmlSerializer serializer = new XmlSerializer(parameter.ParameterType, xmlRoot);
                object o = serializer.Deserialize(xdr);
                ans.Add(new CallParameterInfo(parameter.Name, parameter.ParameterType, FieldDirection.In, o));
            }

            return ans.ToArray();
        }

        /// <summary>
        /// Gets the type of a parameter.
        /// </summary>
        /// <param name="parameter">The parameter to get the type of.</param>
        /// <returns>The parameter's type.</returns>
        private static Type GetParameterType(ParameterInfo parameter)
        {
            Type ans;
            if (parameter.ParameterType.IsByRef)
            {
                ans = parameter.ParameterType.GetElementType();
            }
            else
            {
                ans = parameter.ParameterType;
            }

            return ans;
        }

        /// <summary>
        /// Gets the direction of a parameter.
        /// </summary>
        /// <param name="parameter">The parameter to get the direction of.</param>
        /// <returns>The parameter's direction.</returns>
        private static FieldDirection GetParameterDirection(ParameterInfo parameter)
        {
            FieldDirection ans;

            if (parameter.IsOut)
            {
                ans = FieldDirection.Out;
            }
            else if (parameter.ParameterType.IsByRef)
            {
                ans = FieldDirection.Ref;
            }
            else
            {
                ans = FieldDirection.In;
            }

            return ans;
        }

        /// <summary>
        /// Gets the wire parameter name, which may be overridden by a <see cref="MessageParameterAttribute"/>.
        /// </summary>
        /// <param name="parameter">The parameter to get the wire version of the parameter name of.</param>
        /// <returns>The wire name of the parameter.</returns>
        private static string GetWireParameterName(ParameterInfo parameter)
        {
            string name = parameter.Name;
            object[] attrs = parameter.GetCustomAttributes(true);
            foreach (object attr in attrs)
            {
                MessageParameterAttribute mpa = attr as MessageParameterAttribute;
                if (mpa != null)
                {
                    name = mpa.Name;
                    break;
                }
            }

            return name;
        }

        /// <summary>
        /// Gets the namespace for the service.
        /// </summary>
        /// <param name="proxyType">The proxy type for the service.</param>
        /// <returns>The service namespace.</returns>
        private string GetServiceNamespace(Type proxyType)
        {
            string ans = this.FindTypeNamespace(proxyType);
            if (ans == null)
            {
                ans = "http://tempuri.org/";
            }

            return ans;
        }

        /// <summary>
        /// Finds the namespace for a proxy type.
        /// </summary>
        /// <param name="proxyType">The proxy type to get the namespace for.</param>
        /// <returns>The namespace of the proxy type.</returns>
        private string FindTypeNamespace(Type proxyType)
        {
            string ans = null;

            Attribute[] attributes = Attribute.GetCustomAttributes(proxyType, true); ////proxyType.Assembly.GetCustomAttributes(proxyType, false);

            bool found = false;
            foreach (Attribute a in attributes)
            {
                ServiceContractAttribute attr = a as ServiceContractAttribute;
                if (attr != null)
                {
                    ans = attr.Namespace;
                    found = true;
                    break;
                }
            }

            // Check inheritance hierarchy for other places
            if (!found)
            {
                Type[] interfaces = proxyType.GetInterfaces();
                foreach (Type t in interfaces)
                {
                    ans = this.FindTypeNamespace(t);
                    if (ans != null)
                    {
                        break;
                    }
                }
            }

            return ans;
        }
    }
}
