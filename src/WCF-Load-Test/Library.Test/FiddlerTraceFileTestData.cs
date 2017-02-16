//---------------------------------------------------------------------
// <copyright file="FiddlerTraceFileTestData.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The FiddlerTraceFileTestData type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Contains fragments of trace file data for various situations.
    /// </summary>
    public static class FiddlerTraceFileTestData
    {
        public const string ProcessSimpleAsmxRequestWrapped = @"POST /TestAsmxService.asmx HTTP/1.1
User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 2.0.50727.3053)
VsDebuggerCausalityData: uIDPozEBIG1LNclBmuOoMp2HwJAAAAAA+NETMazWt06HHQECKs9lCqBpBH+fVBNAsn7Zauks2FIACQAA
Content-Type: text/xml; charset=utf-8
SOAPAction: ""http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestWrapped""
Host: 127.0.0.1:8082
Content-Length: 383
Expect: 100-continue
Connection: Keep-Alive

<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><ProcessSimpleAsmxRequestWrapped xmlns=""http://contoso.com/asmxservice/test""><r A=""99""><B>hello</B></r></ProcessSimpleAsmxRequestWrapped></soap:Body></soap:Envelope>
HTTP/1.1 200 OK
Server: ASP.NET Development Server/9.0.0.0
Date: Tue, 14 Apr 2009 09:28:10 GMT
X-AspNet-Version: 2.0.50727
Cache-Control: private, max-age=0
Content-Type: text/xml; charset=utf-8
Content-Length: 459
Connection: Close

<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><ProcessSimpleAsmxRequestWrappedResponse xmlns=""http://contoso.com/asmxservice/test""><ProcessSimpleAsmxRequestWrappedResult>hello99</ProcessSimpleAsmxRequestWrappedResult></ProcessSimpleAsmxRequestWrappedResponse></soap:Body></soap:Envelope>

------------------------------------------------------------------
";

        public const string ProcessSimpleAsmxRequestBare = @"
POST /TestAsmxService.asmx HTTP/1.1
User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 2.0.50727.3053)
VsDebuggerCausalityData: uIDPozIBIG1LNclBmuOoMp2HwJAAAAAA+NETMazWt06HHQECKs9lCqBpBH+fVBNAsn7Zauks2FIACQAA
Content-Type: text/xml; charset=utf-8
SOAPAction: ""http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestBare""
Host: 127.0.0.1:8082
Content-Length: 316
Expect: 100-continue

<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><r A=""99"" xmlns=""http://contoso.com/asmxservice/test""><B>hello</B></r></soap:Body></soap:Envelope>
HTTP/1.1 200 OK
Server: ASP.NET Development Server/9.0.0.0
Date: Tue, 14 Apr 2009 09:28:10 GMT
X-AspNet-Version: 2.0.50727
Cache-Control: private, max-age=0
Content-Type: text/xml; charset=utf-8
Content-Length: 370
Connection: Close

<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><ProcessSimpleAsmxRequestBareResult xmlns=""http://contoso.com/asmxservice/test"">hello99</ProcessSimpleAsmxRequestBareResult></soap:Body></soap:Envelope>

------------------------------------------------------------------
";

        public const string ProcessMultipleParametersWrapped = @"
POST /TestAsmxService.asmx HTTP/1.1
User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 2.0.50727.3053)
VsDebuggerCausalityData: uIDPozMBIG1LNclBmuOoMp2HwJAAAAAA+NETMazWt06HHQECKs9lCqBpBH+fVBNAsn7Zauks2FIACQAA
Content-Type: text/xml; charset=utf-8
SOAPAction: ""http://contoso.com/asmxservice/test/ProcessMultipleParametersWrapped""
Host: 127.0.0.1:8082
Content-Length: 380
Expect: 100-continue

<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><ProcessMultipleParametersWrapped xmlns=""http://contoso.com/asmxservice/test""><a>99</a><b>hello</b></ProcessMultipleParametersWrapped></soap:Body></soap:Envelope>
HTTP/1.1 200 OK
Server: ASP.NET Development Server/9.0.0.0
Date: Tue, 14 Apr 2009 09:28:10 GMT
X-AspNet-Version: 2.0.50727
Cache-Control: private, max-age=0
Content-Type: text/xml; charset=utf-8
Content-Length: 463
Connection: Close

<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><ProcessMultipleParametersWrappedResponse xmlns=""http://contoso.com/asmxservice/test""><ProcessMultipleParametersWrappedResult>hello99</ProcessMultipleParametersWrappedResult></ProcessMultipleParametersWrappedResponse></soap:Body></soap:Envelope>

------------------------------------------------------------------
";

        public const string ProcessMultipleParametersWrappedWithSoapActionInResponse = @"
POST /TestAsmxService.asmx HTTP/1.1
User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 2.0.50727.3053)
VsDebuggerCausalityData: uIDPozMBIG1LNclBmuOoMp2HwJAAAAAA+NETMazWt06HHQECKs9lCqBpBH+fVBNAsn7Zauks2FIACQAA
Content-Type: text/xml; charset=utf-8
SOAPAction: ""http://contoso.com/asmxservice/test/ProcessMultipleParametersWrapped""
Host: 127.0.0.1:8082
Content-Length: 380
Expect: 100-continue

<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><ProcessMultipleParametersWrapped xmlns=""http://contoso.com/asmxservice/test""><a>99</a><b>hello</b></ProcessMultipleParametersWrapped></soap:Body></soap:Envelope>
HTTP/1.1 200 OK
Server: ASP.NET Development Server/9.0.0.0
Date: Tue, 14 Apr 2009 09:28:10 GMT
X-AspNet-Version: 2.0.50727
Cache-Control: private, max-age=0
Content-Type: text/xml; charset=utf-8
Content-Length: 463
Connection: Close

<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><ProcessMultipleParametersWrappedResponse xmlns=""http://contoso.com/asmxservice/test"">
SOAPAction: ""test""
<ProcessMultipleParametersWrappedResult>hello99</ProcessMultipleParametersWrappedResult></ProcessMultipleParametersWrappedResponse></soap:Body></soap:Envelope>

------------------------------------------------------------------
";
    }
}
