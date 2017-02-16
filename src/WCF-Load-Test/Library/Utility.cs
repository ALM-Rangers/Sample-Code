//---------------------------------------------------------------------
// <copyright file="Utility.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The Utility type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides general utility functions.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// The regular expression pattern for a C# identifier.
        /// </summary>
        public const string IdentifierPatternString = LeadingCharacterPatternString + SubsequentCharacterPatternString + @"*";

        /// <summary>
        /// The regular expression pattern for the first character of a C# identifier.
        /// </summary>
        private const string LeadingCharacterPatternString = @"(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|_)";

        /// <summary>
        /// The regular expression pattern for the subsequent characters of a C# identifier.
        /// </summary>
        private const string SubsequentCharacterPatternString = @"(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc})";

        /// <summary>
        /// The regular expression pattern for a C# identifier.
        /// </summary>
        private static Regex cSharpIdentifierPattern = new Regex(@"^" + IdentifierPatternString + @"$");

        /// <summary>
        /// The regular expression for the first character of a C# identifier.
        /// </summary>
        private static Regex leadingCharacterPattern = new Regex(LeadingCharacterPatternString);

        /// <summary>
        /// The regular expression pattern for the subsequent characters of a C# identifier.
        /// </summary>
        private static Regex subsequentCharacterPattern = new Regex(SubsequentCharacterPatternString);

        /// <summary>
        /// Checks that a string represents a valid C# identifier
        /// </summary>
        /// <param name="identifier">The string to check.</param>
        /// <returns>True if the string is a valid C# identifier, false otherwise.</returns>
        public static bool IsValidIdentifier(string identifier)
        {
            bool ans = cSharpIdentifierPattern.IsMatch(identifier);
            return ans;
        }

        /// <summary>
        /// Turns a string into a valid C# identifier, filtering out any invalid characters.
        /// </summary>
        /// <param name="identifier">The string to convert.</param>
        /// <returns>A valid C# identifier</returns>
        public static string MakeSafeIdentifier(string identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            StringBuilder ans = new StringBuilder(identifier.Length);

            bool foundValidLeadingCharacter = false;
            for (int i = 0; i < identifier.Length; i++)
            {
                Match m;
                if (!foundValidLeadingCharacter)
                {
                    m = leadingCharacterPattern.Match(identifier, i, 1);
                    if (m.Success)
                    {
                        foundValidLeadingCharacter = true;
                    }
                }
                else
                {
                    m = subsequentCharacterPattern.Match(identifier, i, 1);
                }

                if (m.Success)
                {
                    ans.Append(m.Value);
                }
            }

            return ans.ToString();
        }

        /// <summary>
        /// Converts an identifier to camel case.
        /// </summary>
        /// <param name="identifier">The identifier to convert.</param>
        /// <returns>A camel case version of the identifier.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Camel case required lowercase")]
        public static string ToCamelCase(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException("identifier");
            }

            string ans;

            // Find last contiguous uppercase letter
            int lastUpperIndex = -1;
            for (int i = 0; i < identifier.Length; i++)
            {
                if (char.IsUpper(identifier[i]))
                {
                    lastUpperIndex = i;
                }
                else
                {
                    break;
                }
            }

            if (lastUpperIndex == -1)
            {
                ans = identifier;
            }
            else if (lastUpperIndex == 0)
            {
                ans = identifier.Substring(0, 1).ToLowerInvariant() + identifier.Substring(1);
            }
            else
            {
                ans = identifier.Substring(0, lastUpperIndex).ToLowerInvariant() + identifier.Substring(lastUpperIndex);
            }

            return ans;
        }

        /// <summary>
        /// Gets the version of the assembly.
        /// </summary>
        /// <returns>The version of the assembly.</returns>
        public static string ReadVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Creates an instance of a configured type.
        /// </summary>
        /// <typeparam name="TConfigured">The type that the configured type is expected to implement.</typeparam>
        /// <param name="configuredTypeName">The configured type name and assembly.</param>
        /// <returns>An instance of the configured type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "There is no sensible alternative without requiring a cast at the call site.")]
        public static TConfigured CreateConfiguredType<TConfigured>(typeType configuredTypeName) where TConfigured : class
        {
            if (configuredTypeName == null)
            {
                throw new ArgumentNullException("configuredTypeName");
            }

            TConfigured ans = null;
            string assemblyQualifiedName = configuredTypeName.type + "," + configuredTypeName.assembly;
            try
            {
                Type configuredType = Type.GetType(assemblyQualifiedName, true, false);
                ans = Activator.CreateInstance(configuredType) as TConfigured;
                if (ans == null)
                {
                    throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.ConfiguredTypeMismatch, assemblyQualifiedName, typeof(TConfigured).FullName));
                }
            }
            catch (IOException fle)
            {
                throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.ConfiguredTypeLoadError, assemblyQualifiedName, fle.Message));
            }
            catch (TypeLoadException tle)
            {
                throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.ConfiguredTypeLoadError, assemblyQualifiedName, tle.Message));
            }
            catch (MemberAccessException mae)
            {
                throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.ConfiguredTypeLoadError, assemblyQualifiedName, mae.Message));
            }

            return ans;
        }
    }
}
