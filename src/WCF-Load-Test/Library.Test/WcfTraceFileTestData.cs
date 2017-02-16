//---------------------------------------------------------------------
// <copyright file="WcfTraceFileTestData.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WcfTraceFileTestData type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Contains fragments of trace file data for various situations.
    /// </summary>
    /// <remarks>
    /// Introduced when lots of test code already based on files, so only partially used.
    /// </remarks>
    public static class WcfTraceFileTestData
    {
        /// <summary>
        /// Sample test data for a simple client-side message.
        /// </summary>
        public const string Add = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:21.9940000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:21.9660000+00:00"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://tempuri.org/IArithmetic/Add</Action>
            </s:Header>
            <s:Body>
              <Add xmlns=""http://tempuri.org/"">
                <request xmlns:d4p1=""http://schemas.datacontract.org/2004/07/Contracts"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                  <d4p1:A>10</d4p1:A>
                  <d4p1:B>5</d4p1:B>
                </request>
              </Add>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:21.9990000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:21.9980000+00:00"" Source=""TransportSend"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <Addressing>
            <Action>http://tempuri.org/IArithmetic/Add</Action>
            <To>http://localhost:8081/arithmetic/basic</To>
          </Addressing>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Body>
              <Add xmlns=""http://tempuri.org/"">
                <request xmlns:a=""http://schemas.datacontract.org/2004/07/Contracts"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                  <a:A>10</a:A>
                  <a:B>5</a:B>
                </request>
              </Add>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:22.0600000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:22.0600000+00:00"" Source=""TransportReceive"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpResponse>
            <StatusCode>OK</StatusCode>
            <StatusDescription>OK</StatusDescription>
            <WebHeaders>
              <Content-Length>307</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Date>Wed, 21 Feb 2007 08:39:21 GMT</Date>
              <Server>Microsoft-HTTPAPI/2.0</Server>
            </WebHeaders>
          </HttpResponse>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header></s:Header>
            <s:Body>
              <AddResponse xmlns=""http://tempuri.org/"">
                <AddResult xmlns:a=""http://schemas.datacontract.org/2004/07/Contracts"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                  <a:Answer>15</a:Answer>
                </AddResult>
              </AddResponse>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:22.0610000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:22.0610000+00:00"" Source=""ServiceLevelReceiveReply"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpResponse>
            <StatusCode>OK</StatusCode>
            <StatusDescription>OK</StatusDescription>
            <WebHeaders>
              <Content-Length>307</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Date>Wed, 21 Feb 2007 08:39:21 GMT</Date>
              <Server>Microsoft-HTTPAPI/2.0</Server>
            </WebHeaders>
          </HttpResponse>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header></s:Header>
            <s:Body>
              <AddResponse xmlns=""http://tempuri.org/"">
                <AddResult xmlns:a=""http://schemas.datacontract.org/2004/07/Contracts"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                  <a:Answer>15</a:Answer>
                </AddResult>
              </AddResponse>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a simple service-side WCF message.
        /// </summary>
        public const string AddServiceSide = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-07-28T16:35:38.6420000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""ConsoleHost"" ProcessID=""4764"" ThreadID=""4"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-07-28T09:35:38.5930000-07:00"" Source=""ServiceLevelReceiveRequest"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpRequest>
            <Method>POST</Method>
            <QueryString></QueryString>
            <WebHeaders>
              <SOAPAction>""http://tempuri.org/IArithmetic/Add""</SOAPAction>
              <Connection>Keep-Alive</Connection>
              <Content-Length>149</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Expect>100-continue</Expect>
              <Host>localhost:8081</Host>
            </WebHeaders>
          </HttpRequest>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <To s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://localhost:8081/arithmetic/basic</To>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://tempuri.org/IArithmetic/Add</Action>
            </s:Header>
            <s:Body>
              <Add xmlns=""http://tempuri.org/"">
                <a>1</a>
                <b>1</b>
              </Add>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-07-28T16:35:38.6740000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""ConsoleHost"" ProcessID=""4764"" ThreadID=""4"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-07-28T09:35:38.6720000-07:00"" Source=""ServiceLevelSendReply"" Type=""System.ServiceModel.Channels.BodyWriterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://tempuri.org/IArithmetic/AddResponse</Action>
            </s:Header>
            <s:Body>
              <AddResponse xmlns=""http://tempuri.org/"">
                <AddResult>2</AddResult>
              </AddResponse>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a simple WCF message.
        /// </summary>
        public const string Add2 = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:22.0670000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:22.0670000+00:00"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://tempuri.org/IArithmetic/Add2</Action>
            </s:Header>
            <s:Body>
              <Add2 xmlns=""http://tempuri.org/"">
                <request xmlns:d4p1=""http://schemas.datacontract.org/2004/07/Contracts"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                  <d4p1:A>10</d4p1:A>
                  <d4p1:B>5</d4p1:B>
                </request>
              </Add2>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:22.0680000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:22.0680000+00:00"" Source=""TransportSend"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <Addressing>
            <Action>http://tempuri.org/IArithmetic/Add2</Action>
            <To>http://localhost:8081/arithmetic/basic</To>
          </Addressing>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Body>
              <Add2 xmlns=""http://tempuri.org/"">
                <request xmlns:a=""http://schemas.datacontract.org/2004/07/Contracts"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                  <a:A>10</a:A>
                  <a:B>5</a:B>
                </request>
              </Add2>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:22.0700000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:22.0690000+00:00"" Source=""TransportReceive"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpResponse>
            <StatusCode>OK</StatusCode>
            <StatusDescription>OK</StatusDescription>
            <WebHeaders>
              <Content-Length>178</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Date>Wed, 21 Feb 2007 08:39:21 GMT</Date>
              <Server>Microsoft-HTTPAPI/2.0</Server>
            </WebHeaders>
          </HttpResponse>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header></s:Header>
            <s:Body>
              <Add2Response xmlns=""http://tempuri.org/"">
                <Add2Result>15</Add2Result>
              </Add2Response>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:22.0700000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:22.0700000+00:00"" Source=""ServiceLevelReceiveReply"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpResponse>
            <StatusCode>OK</StatusCode>
            <StatusDescription>OK</StatusDescription>
            <WebHeaders>
              <Content-Length>178</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Date>Wed, 21 Feb 2007 08:39:21 GMT</Date>
              <Server>Microsoft-HTTPAPI/2.0</Server>
            </WebHeaders>
          </HttpResponse>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header></s:Header>
            <s:Body>
              <Add2Response xmlns=""http://tempuri.org/"">
                <Add2Result>15</Add2Result>
              </Add2Response>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a simple WCF message.
        /// </summary>
        public const string Add3 = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:22.0710000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:22.0710000+00:00"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Channels.BodyWriterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://tempuri.org/IArithmetic/Add3</Action>
            </s:Header>
            <s:Body>
              <Add3 xmlns=""http://tempuri.org/"">
                <a>20</a>
                <b>25</b>
              </Add3>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:22.0710000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:22.0710000+00:00"" Source=""TransportSend"" Type=""System.ServiceModel.Channels.BodyWriterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <Addressing>
            <Action>http://tempuri.org/IArithmetic/Add3</Action>
            <To>http://localhost:8081/arithmetic/basic</To>
          </Addressing>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Body>
              <Add3 xmlns=""http://tempuri.org/"">
                <a>20</a>
                <b>25</b>
              </Add3>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:22.0730000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:22.0730000+00:00"" Source=""TransportReceive"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpResponse>
            <StatusCode>OK</StatusCode>
            <StatusDescription>OK</StatusDescription>
            <WebHeaders>
              <Content-Length>178</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Date>Wed, 21 Feb 2007 08:39:21 GMT</Date>
              <Server>Microsoft-HTTPAPI/2.0</Server>
            </WebHeaders>
          </HttpResponse>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header></s:Header>
            <s:Body>
              <Add3Response xmlns=""http://tempuri.org/"">
                <Add3Result>45</Add3Result>
              </Add3Response>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:22.0730000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:22.0730000+00:00"" Source=""ServiceLevelReceiveReply"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpResponse>
            <StatusCode>OK</StatusCode>
            <StatusDescription>OK</StatusDescription>
            <WebHeaders>
              <Content-Length>178</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Date>Wed, 21 Feb 2007 08:39:21 GMT</Date>
              <Server>Microsoft-HTTPAPI/2.0</Server>
            </WebHeaders>
          </HttpResponse>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header></s:Header>
            <s:Body>
              <Add3Response xmlns=""http://tempuri.org/"">
                <Add3Result>45</Add3Result>
              </Add3Response>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>";

        /// <summary>
        /// Sample test data for a message log that does not have service level logging enabled. 
        /// </summary>
        public const string AddWithoutServiceLevelLogging = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T09:13:10.4910000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""7844"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T09:13:10.4890000+00:00"" Source=""TransportSend"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <Addressing>
            <Action>http://tempuri.org/IArithmetic/Add</Action>
            <To>http://localhost:8081/arithmetic/basic</To>
          </Addressing>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Body>
              <Add xmlns=""http://tempuri.org/"">
                <request xmlns:a=""http://schemas.datacontract.org/2004/07/Contracts"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                  <a:A>10</a:A>
                  <a:B>5</a:B>
                </request>
              </Add>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T09:13:10.5670000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""7844"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T09:13:10.5670000+00:00"" Source=""TransportReceive"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpResponse>
            <StatusCode>OK</StatusCode>
            <StatusDescription>OK</StatusDescription>
            <WebHeaders>
              <Content-Length>307</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Date>Wed, 21 Feb 2007 09:13:10 GMT</Date>
              <Server>Microsoft-HTTPAPI/2.0</Server>
            </WebHeaders>
          </HttpResponse>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header></s:Header>
            <s:Body>
              <AddResponse xmlns=""http://tempuri.org/"">
                <AddResult xmlns:a=""http://schemas.datacontract.org/2004/07/Contracts"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                  <a:Answer>15</a:Answer>
                </AddResult>
              </AddResponse>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a message that has no body.
        /// </summary>
        public const string AddWithNoMessageBody = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:33:49.3060000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""6940"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:33:49.2550000+00:00"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://tempuri.org/IArithmetic/Add</Action>
            </s:Header>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:33:49.3820000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""6940"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:33:49.3820000+00:00"" Source=""TransportSend"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <Addressing>
            <Action>http://tempuri.org/IArithmetic/Add</Action>
            <To>http://localhost:8081/arithmetic/basic</To>
          </Addressing>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""></s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:33:49.5300000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""6940"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:33:49.5300000+00:00"" Source=""TransportReceive"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpResponse>
            <StatusCode>OK</StatusCode>
            <StatusDescription>OK</StatusDescription>
            <WebHeaders>
              <Content-Length>307</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Date>Wed, 21 Feb 2007 08:33:49 GMT</Date>
              <Server>Microsoft-HTTPAPI/2.0</Server>
            </WebHeaders>
          </HttpResponse>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header></s:Header>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:33:49.5310000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""6940"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:33:49.5310000+00:00"" Source=""ServiceLevelReceiveReply"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpResponse>
            <StatusCode>OK</StatusCode>
            <StatusDescription>OK</StatusDescription>
            <WebHeaders>
              <Content-Length>307</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Date>Wed, 21 Feb 2007 08:33:49 GMT</Date>
              <Server>Microsoft-HTTPAPI/2.0</Server>
            </WebHeaders>
          </HttpResponse>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header></s:Header>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a MEX message.
        /// </summary>
        public const string Mex = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-03-29T21:13:41.7200000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Microsoft.Sample.WCF.Client"" ProcessID=""112384"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-03-29T22:13:41.7180000+01:00"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Channels.BodyWriterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:a=""http://www.w3.org/2005/08/addressing"" xmlns:s=""http://www.w3.org/2003/05/soap-envelope"">
            <s:Header>
              <a:Action s:mustUnderstand=""1"">http://schemas.xmlsoap.org/ws/2004/09/transfer/Get</a:Action>
              <a:MessageID>urn:uuid:e53a3f62-ccac-4ab0-9f44-1469db50fc36</a:MessageID>
              <a:ReplyTo>
                <a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>
              </a:ReplyTo>
            </s:Header>
            <s:Body></s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-03-29T21:13:42.2490000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Microsoft.Sample.WCF.Client"" ProcessID=""112384"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-03-29T22:13:42.2440000+01:00"" Source=""ServiceLevelReceiveReply"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpResponse>
            <StatusCode>OK</StatusCode>
            <StatusDescription>OK</StatusDescription>
            <WebHeaders>
              <Content-Length>68037</Content-Length>
              <Content-Type>application/soap+xml; charset=utf-8</Content-Type>
              <Date>Thu, 29 Mar 2007 21:13:42 GMT</Date>
              <Server>Microsoft-HTTPAPI/2.0</Server>
            </WebHeaders>
          </HttpResponse>
          <s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope"" xmlns:a=""http://www.w3.org/2005/08/addressing"">
            <s:Header>
              <a:Action s:mustUnderstand=""1"">http://schemas.xmlsoap.org/ws/2004/09/transfer/GetResponse</a:Action>
              <ActivityId CorrelationId=""e6412b76-65ec-4ba2-a85d-77395a50c96a"" xmlns=""http://schemas.microsoft.com/2004/09/ServiceModel/Diagnostics"">c00fbc87-f530-48d2-abe0-51d0e9d60321</ActivityId>
              <a:RelatesTo>urn:uuid:e53a3f62-ccac-4ab0-9f44-1469db50fc36</a:RelatesTo>
            </s:Header>
            <s:Body>
              <Metadata xmlns=""http://schemas.xmlsoap.org/ws/2004/09/mex"" xmlns:wsx=""http://schemas.xmlsoap.org/ws/2004/09/mex"">
                <wsx:MetadataSection Dialect=""http://schemas.xmlsoap.org/wsdl/"" Identifier=""http://microsoft.com/sample/wcf"" xmlns="""">
                  <wsdl:definitions targetNamespace=""http://microsoft.com/sample/wcf"" xmlns:wsdl=""http://schemas.xmlsoap.org/wsdl/"" xmlns:soap=""http://schemas.xmlsoap.org/wsdl/soap/"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:soapenc=""http://schemas.xmlsoap.org/soap/encoding/"" xmlns:tns=""http://microsoft.com/sample/wcf"" xmlns:wsa=""http://schemas.xmlsoap.org/ws/2004/08/addressing"" xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"" xmlns:wsap=""http://schemas.xmlsoap.org/ws/2004/08/addressing/policy"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:msc=""http://schemas.microsoft.com/ws/2005/12/wsdl/contract"" xmlns:wsaw=""http://www.w3.org/2006/05/addressing/wsdl"" xmlns:soap12=""http://schemas.xmlsoap.org/wsdl/soap12/"" xmlns:wsa10=""http://www.w3.org/2005/08/addressing"">
                    <wsdl:types>
                      <xsd:schema targetNamespace=""http://microsoft.com/sample/wcf/Imports"">
                        <xsd:import namespace=""http://microsoft.com/sample/wcf""></xsd:import>
                        <xsd:import namespace=""http://schemas.microsoft.com/2003/10/Serialization/""></xsd:import>
                      </xsd:schema>
                    </wsdl:types>
                    <wsdl:message name=""ISampleService_Logon_InputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:Logon""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Logon_OutputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:LogonResponse""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Logon_ArrayOfBusinessErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:ArrayOfBusinessError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Logon_TechnicalErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:TechnicalError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Request1_InputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:Request1""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Request1_OutputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:Request1Response""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Request1_TechnicalErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:TechnicalError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Request1_ArrayOfBusinessErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:ArrayOfBusinessError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_PlaceOrder_InputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:PlaceOrder""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_PlaceOrder_OutputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:PlaceOrderResponse""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_PlaceOrder_ArrayOfBusinessErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:ArrayOfBusinessError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_PlaceOrder_TechnicalErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:TechnicalError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Logoff_InputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:Logoff""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Logoff_OutputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:LogoffResponse""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Logoff_ArrayOfBusinessErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:ArrayOfBusinessError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Logoff_TechnicalErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:TechnicalError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Performance_InputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:Performance""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Performance_OutputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:PerformanceResponse""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Performance_TechnicalErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:TechnicalError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_Performance_ArrayOfBusinessErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:ArrayOfBusinessError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_LogException_InputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:LogException""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_LogException_OutputMessage"">
                      <wsdl:part name=""parameters"" element=""tns:LogExceptionResponse""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_LogException_TechnicalErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:TechnicalError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:message name=""ISampleService_LogException_ArrayOfBusinessErrorFault_FaultMessage"">
                      <wsdl:part name=""detail"" element=""tns:ArrayOfBusinessError""></wsdl:part>
                    </wsdl:message>
                    <wsdl:portType name=""ISampleService"">
                      <wsdl:operation name=""Logon"">
                        <wsdl:input wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/Logon"" message=""tns:ISampleService_Logon_InputMessage""></wsdl:input>
                        <wsdl:output wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/LogonResponse"" message=""tns:ISampleService_Logon_OutputMessage""></wsdl:output>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/LogonArrayOfBusinessErrorFault"" name=""ArrayOfBusinessErrorFault"" message=""tns:ISampleService_Logon_ArrayOfBusinessErrorFault_FaultMessage""></wsdl:fault>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/LogonTechnicalErrorFault"" name=""TechnicalErrorFault"" message=""tns:ISampleService_Logon_TechnicalErrorFault_FaultMessage""></wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Request1"">
                        <wsdl:input wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/Request1"" message=""tns:ISampleService_Request1_InputMessage""></wsdl:input>
                        <wsdl:output wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/Request1Response"" message=""tns:ISampleService_Request1_OutputMessage""></wsdl:output>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/Request1TechnicalErrorFault"" name=""TechnicalErrorFault"" message=""tns:ISampleService_Request1_TechnicalErrorFault_FaultMessage""></wsdl:fault>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/Request1ArrayOfBusinessErrorFault"" name=""ArrayOfBusinessErrorFault"" message=""tns:ISampleService_Request1_ArrayOfBusinessErrorFault_FaultMessage""></wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""PlaceOrder"">
                        <wsdl:input wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/PlaceOrder"" message=""tns:ISampleService_PlaceOrder_InputMessage""></wsdl:input>
                        <wsdl:output wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/PlaceOrderResponse"" message=""tns:ISampleService_PlaceOrder_OutputMessage""></wsdl:output>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/PlaceOrderArrayOfBusinessErrorFault"" name=""ArrayOfBusinessErrorFault"" message=""tns:ISampleService_PlaceOrder_ArrayOfBusinessErrorFault_FaultMessage""></wsdl:fault>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/PlaceOrderTechnicalErrorFault"" name=""TechnicalErrorFault"" message=""tns:ISampleService_PlaceOrder_TechnicalErrorFault_FaultMessage""></wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Logoff"">
                        <wsdl:input wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/Logoff"" message=""tns:ISampleService_Logoff_InputMessage""></wsdl:input>
                        <wsdl:output wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/LogoffResponse"" message=""tns:ISampleService_Logoff_OutputMessage""></wsdl:output>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/LogoffArrayOfBusinessErrorFault"" name=""ArrayOfBusinessErrorFault"" message=""tns:ISampleService_Logoff_ArrayOfBusinessErrorFault_FaultMessage""></wsdl:fault>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/LogoffTechnicalErrorFault"" name=""TechnicalErrorFault"" message=""tns:ISampleService_Logoff_TechnicalErrorFault_FaultMessage""></wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Performance"">
                        <wsdl:input wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/Performance"" message=""tns:ISampleService_Performance_InputMessage""></wsdl:input>
                        <wsdl:output wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/PerformanceResponse"" message=""tns:ISampleService_Performance_OutputMessage""></wsdl:output>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/PerformanceTechnicalErrorFault"" name=""TechnicalErrorFault"" message=""tns:ISampleService_Performance_TechnicalErrorFault_FaultMessage""></wsdl:fault>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/PerformanceArrayOfBusinessErrorFault"" name=""ArrayOfBusinessErrorFault"" message=""tns:ISampleService_Performance_ArrayOfBusinessErrorFault_FaultMessage""></wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""LogException"">
                        <wsdl:input wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/LogException"" message=""tns:ISampleService_LogException_InputMessage""></wsdl:input>
                        <wsdl:output wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/LogExceptionResponse"" message=""tns:ISampleService_LogException_OutputMessage""></wsdl:output>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/LogExceptionTechnicalErrorFault"" name=""TechnicalErrorFault"" message=""tns:ISampleService_LogException_TechnicalErrorFault_FaultMessage""></wsdl:fault>
                        <wsdl:fault wsaw:Action=""http://microsoft.com/sample/wcf/ISampleService/LogExceptionArrayOfBusinessErrorFault"" name=""ArrayOfBusinessErrorFault"" message=""tns:ISampleService_LogException_ArrayOfBusinessErrorFault_FaultMessage""></wsdl:fault>
                      </wsdl:operation>
                    </wsdl:portType>
                  </wsdl:definitions>
                </wsx:MetadataSection>
                <wsx:MetadataSection Dialect=""http://schemas.xmlsoap.org/wsdl/"" Identifier=""http://tempuri.org/"" xmlns="""">
                  <wsdl:definitions name=""SampleService"" targetNamespace=""http://tempuri.org/"" xmlns:wsdl=""http://schemas.xmlsoap.org/wsdl/"" xmlns:soap=""http://schemas.xmlsoap.org/wsdl/soap/"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:soapenc=""http://schemas.xmlsoap.org/soap/encoding/"" xmlns:tns=""http://tempuri.org/"" xmlns:wsa=""http://schemas.xmlsoap.org/ws/2004/08/addressing"" xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"" xmlns:i0=""http://microsoft.com/sample/wcf"" xmlns:wsap=""http://schemas.xmlsoap.org/ws/2004/08/addressing/policy"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:msc=""http://schemas.microsoft.com/ws/2005/12/wsdl/contract"" xmlns:wsaw=""http://www.w3.org/2006/05/addressing/wsdl"" xmlns:soap12=""http://schemas.xmlsoap.org/wsdl/soap12/"" xmlns:wsa10=""http://www.w3.org/2005/08/addressing"">
                    <wsp:Policy wsu:Id=""BasicWithSSL_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:TransportBinding xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <wsp:Policy>
                              <sp:TransportToken>
                                <wsp:Policy>
                                  <sp:HttpsToken RequireClientCertificate=""false""></sp:HttpsToken>
                                </wsp:Policy>
                              </sp:TransportToken>
                              <sp:AlgorithmSuite>
                                <wsp:Policy>
                                  <sp:Basic256></sp:Basic256>
                                </wsp:Policy>
                              </sp:AlgorithmSuite>
                              <sp:Layout>
                                <wsp:Policy>
                                  <sp:Lax></sp:Lax>
                                </wsp:Policy>
                              </sp:Layout>
                              <sp:IncludeTimestamp></sp:IncludeTimestamp>
                            </wsp:Policy>
                          </sp:TransportBinding>
                          <sp:SignedSupportingTokens xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <wsp:Policy>
                              <sp:UsernameToken sp:IncludeToken=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy/IncludeToken/AlwaysToRecipient"">
                                <wsp:Policy>
                                  <sp:WssUsernameToken10></sp:WssUsernameToken10>
                                </wsp:Policy>
                              </sp:UsernameToken>
                            </wsp:Policy>
                          </sp:SignedSupportingTokens>
                          <sp:Wss10 xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <wsp:Policy>
                              <sp:MustSupportRefKeyIdentifier></sp:MustSupportRefKeyIdentifier>
                              <sp:MustSupportRefIssuerSerial></sp:MustSupportRefIssuerSerial>
                            </wsp:Policy>
                          </sp:Wss10>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:AsymmetricBinding xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <wsp:Policy>
                              <sp:InitiatorToken>
                                <wsp:Policy>
                                  <sp:X509Token sp:IncludeToken=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy/IncludeToken/AlwaysToRecipient"">
                                    <wsp:Policy>
                                      <sp:WssX509V3Token10></sp:WssX509V3Token10>
                                    </wsp:Policy>
                                  </sp:X509Token>
                                </wsp:Policy>
                              </sp:InitiatorToken>
                              <sp:RecipientToken>
                                <wsp:Policy>
                                  <sp:X509Token sp:IncludeToken=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy/IncludeToken/Never"">
                                    <wsp:Policy>
                                      <sp:WssX509V3Token10></sp:WssX509V3Token10>
                                    </wsp:Policy>
                                  </sp:X509Token>
                                </wsp:Policy>
                              </sp:RecipientToken>
                              <sp:AlgorithmSuite>
                                <wsp:Policy>
                                  <sp:Basic256></sp:Basic256>
                                </wsp:Policy>
                              </sp:AlgorithmSuite>
                              <sp:Layout>
                                <wsp:Policy>
                                  <sp:Lax></sp:Lax>
                                </wsp:Policy>
                              </sp:Layout>
                              <sp:IncludeTimestamp></sp:IncludeTimestamp>
                              <sp:EncryptSignature></sp:EncryptSignature>
                              <sp:OnlySignEntireHeadersAndBody></sp:OnlySignEntireHeadersAndBody>
                            </wsp:Policy>
                          </sp:AsymmetricBinding>
                          <sp:Wss10 xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <wsp:Policy>
                              <sp:MustSupportRefKeyIdentifier></sp:MustSupportRefKeyIdentifier>
                              <sp:MustSupportRefIssuerSerial></sp:MustSupportRefIssuerSerial>
                            </wsp:Policy>
                          </sp:Wss10>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Logon_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Logon_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Logon_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Logon_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Request1_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Request1_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Request1_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Request1_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_PlaceOrder_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_PlaceOrder_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_PlaceOrder_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_PlaceOrder_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Logoff_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Logoff_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Logoff_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Logoff_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Performance_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Performance_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Performance_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_Performance_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_LogException_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_LogException_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_LogException_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""BasicClientCertificate_LogException_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SymmetricBinding xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <wsp:Policy>
                              <sp:ProtectionToken>
                                <wsp:Policy>
                                  <sp:SecureConversationToken sp:IncludeToken=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy/IncludeToken/AlwaysToRecipient"">
                                    <wsp:Policy>
                                      <sp:RequireDerivedKeys></sp:RequireDerivedKeys>
                                      <sp:BootstrapPolicy>
                                        <wsp:Policy>
                                          <sp:SignedParts>
                                            <sp:Body></sp:Body>
                                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                                          </sp:SignedParts>
                                          <sp:EncryptedParts>
                                            <sp:Body></sp:Body>
                                          </sp:EncryptedParts>
                                          <sp:SymmetricBinding>
                                            <wsp:Policy>
                                              <sp:ProtectionToken>
                                                <wsp:Policy>
                                                  <mssp:SslContextToken sp:IncludeToken=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy/IncludeToken/AlwaysToRecipient"" xmlns:mssp=""http://schemas.microsoft.com/ws/2005/07/securitypolicy"">
                                                    <wsp:Policy>
                                                      <sp:RequireDerivedKeys></sp:RequireDerivedKeys>
                                                    </wsp:Policy>
                                                  </mssp:SslContextToken>
                                                </wsp:Policy>
                                              </sp:ProtectionToken>
                                              <sp:AlgorithmSuite>
                                                <wsp:Policy>
                                                  <sp:Basic256></sp:Basic256>
                                                </wsp:Policy>
                                              </sp:AlgorithmSuite>
                                              <sp:Layout>
                                                <wsp:Policy>
                                                  <sp:Strict></sp:Strict>
                                                </wsp:Policy>
                                              </sp:Layout>
                                              <sp:IncludeTimestamp></sp:IncludeTimestamp>
                                              <sp:EncryptSignature></sp:EncryptSignature>
                                              <sp:OnlySignEntireHeadersAndBody></sp:OnlySignEntireHeadersAndBody>
                                            </wsp:Policy>
                                          </sp:SymmetricBinding>
                                          <sp:SignedSupportingTokens>
                                            <wsp:Policy>
                                              <sp:UsernameToken sp:IncludeToken=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy/IncludeToken/AlwaysToRecipient"">
                                                <wsp:Policy>
                                                  <sp:WssUsernameToken10></sp:WssUsernameToken10>
                                                </wsp:Policy>
                                              </sp:UsernameToken>
                                            </wsp:Policy>
                                          </sp:SignedSupportingTokens>
                                          <sp:Wss11>
                                            <wsp:Policy>
                                              <sp:MustSupportRefKeyIdentifier></sp:MustSupportRefKeyIdentifier>
                                              <sp:MustSupportRefIssuerSerial></sp:MustSupportRefIssuerSerial>
                                              <sp:MustSupportRefThumbprint></sp:MustSupportRefThumbprint>
                                              <sp:MustSupportRefEncryptedKey></sp:MustSupportRefEncryptedKey>
                                            </wsp:Policy>
                                          </sp:Wss11>
                                          <sp:Trust10>
                                            <wsp:Policy>
                                              <sp:MustSupportIssuedTokens></sp:MustSupportIssuedTokens>
                                              <sp:RequireClientEntropy></sp:RequireClientEntropy>
                                              <sp:RequireServerEntropy></sp:RequireServerEntropy>
                                            </wsp:Policy>
                                          </sp:Trust10>
                                        </wsp:Policy>
                                      </sp:BootstrapPolicy>
                                    </wsp:Policy>
                                  </sp:SecureConversationToken>
                                </wsp:Policy>
                              </sp:ProtectionToken>
                              <sp:AlgorithmSuite>
                                <wsp:Policy>
                                  <sp:Basic256></sp:Basic256>
                                </wsp:Policy>
                              </sp:AlgorithmSuite>
                              <sp:Layout>
                                <wsp:Policy>
                                  <sp:Strict></sp:Strict>
                                </wsp:Policy>
                              </sp:Layout>
                              <sp:IncludeTimestamp></sp:IncludeTimestamp>
                              <sp:EncryptSignature></sp:EncryptSignature>
                              <sp:OnlySignEntireHeadersAndBody></sp:OnlySignEntireHeadersAndBody>
                            </wsp:Policy>
                          </sp:SymmetricBinding>
                          <sp:Wss11 xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <wsp:Policy>
                              <sp:MustSupportRefKeyIdentifier></sp:MustSupportRefKeyIdentifier>
                              <sp:MustSupportRefIssuerSerial></sp:MustSupportRefIssuerSerial>
                              <sp:MustSupportRefThumbprint></sp:MustSupportRefThumbprint>
                              <sp:MustSupportRefEncryptedKey></sp:MustSupportRefEncryptedKey>
                            </wsp:Policy>
                          </sp:Wss11>
                          <sp:Trust10 xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <wsp:Policy>
                              <sp:MustSupportIssuedTokens></sp:MustSupportIssuedTokens>
                              <sp:RequireClientEntropy></sp:RequireClientEntropy>
                              <sp:RequireServerEntropy></sp:RequireServerEntropy>
                            </wsp:Policy>
                          </sp:Trust10>
                          <wsaw:UsingAddressing></wsaw:UsingAddressing>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Logon_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Logon_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Logon_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Logon_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Request1_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Request1_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Request1_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Request1_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_PlaceOrder_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_PlaceOrder_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_PlaceOrder_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_PlaceOrder_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Logoff_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Logoff_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Logoff_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Logoff_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Performance_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Performance_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Performance_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_Performance_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_LogException_Input_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_LogException_output_policy"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_LogException_TechnicalErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsp:Policy wsu:Id=""ws-security_LogException_ArrayOfBusinessErrorFault_Fault"">
                      <wsp:ExactlyOne>
                        <wsp:All>
                          <sp:SignedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                            <sp:Header Name=""To"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""From"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""FaultTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""ReplyTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""MessageID"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""RelatesTo"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                            <sp:Header Name=""Action"" Namespace=""http://www.w3.org/2005/08/addressing""></sp:Header>
                          </sp:SignedParts>
                          <sp:EncryptedParts xmlns:sp=""http://schemas.xmlsoap.org/ws/2005/07/securitypolicy"">
                            <sp:Body></sp:Body>
                          </sp:EncryptedParts>
                        </wsp:All>
                      </wsp:ExactlyOne>
                    </wsp:Policy>
                    <wsdl:import namespace=""http://microsoft.com/sample/wcf"" location=""""></wsdl:import>
                    <wsdl:types></wsdl:types>
                    <wsdl:binding name=""BasicWithSSL"" type=""i0:ISampleService"">
                      <wsp:PolicyReference URI=""#BasicWithSSL_policy""></wsp:PolicyReference>
                      <soap:binding transport=""http://schemas.xmlsoap.org/soap/http""></soap:binding>
                      <wsdl:operation name=""Logon"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Logon"" style=""document""></soap:operation>
                        <wsdl:input>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Request1"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Request1"" style=""document""></soap:operation>
                        <wsdl:input>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""PlaceOrder"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/PlaceOrder"" style=""document""></soap:operation>
                        <wsdl:input>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Logoff"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Logoff"" style=""document""></soap:operation>
                        <wsdl:input>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Performance"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Performance"" style=""document""></soap:operation>
                        <wsdl:input>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""LogException"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/LogException"" style=""document""></soap:operation>
                        <wsdl:input>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                    </wsdl:binding>
                    <wsdl:binding name=""BasicClientCertificate"" type=""i0:ISampleService"">
                      <wsp:PolicyReference URI=""#BasicClientCertificate_policy""></wsp:PolicyReference>
                      <soap:binding transport=""http://schemas.xmlsoap.org/soap/http""></soap:binding>
                      <wsdl:operation name=""Logon"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Logon"" style=""document""></soap:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Logon_Input_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Logon_output_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Logon_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Logon_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Request1"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Request1"" style=""document""></soap:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Request1_Input_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Request1_output_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Request1_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Request1_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""PlaceOrder"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/PlaceOrder"" style=""document""></soap:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_PlaceOrder_Input_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_PlaceOrder_output_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_PlaceOrder_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_PlaceOrder_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Logoff"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Logoff"" style=""document""></soap:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Logoff_Input_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Logoff_output_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Logoff_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Logoff_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Performance"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Performance"" style=""document""></soap:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Performance_Input_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Performance_output_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Performance_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_Performance_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""LogException"">
                        <soap:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/LogException"" style=""document""></soap:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_LogException_Input_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#BasicClientCertificate_LogException_output_policy""></wsp:PolicyReference>
                          <soap:body use=""literal""></soap:body>
                        </wsdl:output>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_LogException_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""TechnicalErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#BasicClientCertificate_LogException_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                    </wsdl:binding>
                    <wsdl:binding name=""ws-security"" type=""i0:ISampleService"">
                      <wsp:PolicyReference URI=""#ws-security_policy""></wsp:PolicyReference>
                      <soap12:binding transport=""http://schemas.xmlsoap.org/soap/http""></soap12:binding>
                      <wsdl:operation name=""Logon"">
                        <soap12:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Logon"" style=""document""></soap12:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#ws-security_Logon_Input_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#ws-security_Logon_output_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:output>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_Logon_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_Logon_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""TechnicalErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Request1"">
                        <soap12:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Request1"" style=""document""></soap12:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#ws-security_Request1_Input_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#ws-security_Request1_output_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:output>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_Request1_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""TechnicalErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_Request1_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""PlaceOrder"">
                        <soap12:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/PlaceOrder"" style=""document""></soap12:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#ws-security_PlaceOrder_Input_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#ws-security_PlaceOrder_output_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:output>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_PlaceOrder_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_PlaceOrder_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""TechnicalErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Logoff"">
                        <soap12:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Logoff"" style=""document""></soap12:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#ws-security_Logoff_Input_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#ws-security_Logoff_output_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:output>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_Logoff_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_Logoff_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""TechnicalErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""Performance"">
                        <soap12:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/Performance"" style=""document""></soap12:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#ws-security_Performance_Input_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#ws-security_Performance_output_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:output>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_Performance_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""TechnicalErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_Performance_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                      <wsdl:operation name=""LogException"">
                        <soap12:operation soapAction=""http://microsoft.com/sample/wcf/ISampleService/LogException"" style=""document""></soap12:operation>
                        <wsdl:input>
                          <wsp:PolicyReference URI=""#ws-security_LogException_Input_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:input>
                        <wsdl:output>
                          <wsp:PolicyReference URI=""#ws-security_LogException_output_policy""></wsp:PolicyReference>
                          <soap12:body use=""literal""></soap12:body>
                        </wsdl:output>
                        <wsdl:fault name=""TechnicalErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_LogException_TechnicalErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""TechnicalErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                        <wsdl:fault name=""ArrayOfBusinessErrorFault"">
                          <wsp:PolicyReference URI=""#ws-security_LogException_ArrayOfBusinessErrorFault_Fault""></wsp:PolicyReference>
                          <soap12:fault name=""ArrayOfBusinessErrorFault"" use=""literal""></soap12:fault>
                        </wsdl:fault>
                      </wsdl:operation>
                    </wsdl:binding>
                    <wsdl:service name=""SampleService"">
                      <wsdl:port name=""BasicWithSSL"" binding=""tns:BasicWithSSL"">
                        <soap:address location=""https://localhost:8081/SampleWCFService""></soap:address>
                      </wsdl:port>
                      <wsdl:port name=""BasicClientCertificate"" binding=""tns:BasicClientCertificate"">
                        <soap:address location=""http://localhost:8080/SampleWCFService/clientcert""></soap:address>
                      </wsdl:port>
                      <wsdl:port name=""ws-security"" binding=""tns:ws-security"">
                        <soap12:address location=""http://localhost:8080/SampleWCFService/ws""></soap12:address>
                        <wsa10:EndpointReference>
                          <wsa10:Address>http://localhost:8080/SampleWCFService/ws</wsa10:Address>
                          <Identity xmlns=""http://schemas.xmlsoap.org/ws/2006/02/addressingidentity"">
                            <KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">
                              <X509Data>
                                <X509Certificate>MIICEjCCAX+gAwIBAgIQXqQa9Esyso1C4b9+to7AtzAJBgUrDgMCHQUAMCYxJDAiBgNVBAMTG1Rlc3QgQW5kIERldiBSb290IEF1dGhvcml0eTAeFw0wNzAyMDkxNzExMDBaFw0zOTEyMzEyMzU5NTlaMBQxEjAQBgNVBAMTCWxvY2FsaG9zdDCBnzANBgkqhkiG9w0BAQEFAAOBjQAwgYkCgYEA5ureqM6mlbg5u6BcY+iS60DUZmeBkmnJoJfiwC4NHKGYSL3n2NJOpsplb6QrxDAm4I5KS+JN2pIj8JbK2xdE9eKIV3uOVWGOCeradER8K+aVMdH3lf8rGFRn4pPLp67BEKECvvs0PetGzJ4FcWEf/auMKpWfaQAvw7JFNPvCEicCAwEAAaNbMFkwVwYDVR0BBFAwToAQuAKeHjLMTtKsNMCR2mNetaEoMCYxJDAiBgNVBAMTG1Rlc3QgQW5kIERldiBSb290IEF1dGhvcml0eYIQ6g+6lrRhcrJEOiTEed0CkTAJBgUrDgMCHQUAA4GBAEygu7bwRsycJa8Sr6+7WTFZeSAa2hiwyefl4/YOHv2dcj14FDs5Tr6vzogBxQ9z4Ht2S90pUy4ARjY98IV/R4UGfLoZEaaBU6t2gKF42Bl950EXsHp8XoipKqsKdkeQgQ0cQGgV3vJRbxoNVFdoqopZL4qNgwjiR3HhoURz/2YD</X509Certificate>
                              </X509Data>
                            </KeyInfo>
                          </Identity>
                        </wsa10:EndpointReference>
                      </wsdl:port>
                    </wsdl:service>
                  </wsdl:definitions>
                </wsx:MetadataSection>
                <wsx:MetadataSection Dialect=""http://www.w3.org/2001/XMLSchema"" Identifier=""http://microsoft.com/sample/wcf"" xmlns="""">
                  <xs:schema elementFormDefault=""qualified"" targetNamespace=""http://microsoft.com/sample/wcf"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:ser=""http://schemas.microsoft.com/2003/10/Serialization/"">
                    <xs:import namespace=""http://schemas.microsoft.com/2003/10/Serialization/""></xs:import>
                    <xs:element name=""Logon"">
                      <xs:complexType>
                        <xs:sequence></xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:element name=""LogonResponse"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element minOccurs=""0"" name=""LogonResult"" type=""ser:guid""></xs:element>
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:complexType name=""ArrayOfBusinessError"">
                      <xs:sequence>
                        <xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""BusinessError"" nillable=""true"" type=""q1:BusinessError"" xmlns:q1=""http://microsoft.com/sample/wcf""></xs:element>
                      </xs:sequence>
                    </xs:complexType>
                    <xs:element name=""ArrayOfBusinessError"" nillable=""true"" type=""q2:ArrayOfBusinessError"" xmlns:q2=""http://microsoft.com/sample/wcf""></xs:element>
                    <xs:complexType name=""BusinessError"">
                      <xs:sequence>
                        <xs:element minOccurs=""0"" name=""Code"" nillable=""true"" type=""xs:string""></xs:element>
                        <xs:element minOccurs=""0"" name=""Description"" nillable=""true"" type=""xs:string""></xs:element>
                      </xs:sequence>
                    </xs:complexType>
                    <xs:element name=""BusinessError"" nillable=""true"" type=""q3:BusinessError"" xmlns:q3=""http://microsoft.com/sample/wcf""></xs:element>
                    <xs:complexType name=""TechnicalError"">
                      <xs:sequence>
                        <xs:element minOccurs=""0"" name=""ErrorId"" type=""ser:guid""></xs:element>
                      </xs:sequence>
                    </xs:complexType>
                    <xs:element name=""TechnicalError"" nillable=""true"" type=""q4:TechnicalError"" xmlns:q4=""http://microsoft.com/sample/wcf""></xs:element>
                    <xs:element name=""Request1"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element minOccurs=""0"" name=""request"" nillable=""true"" type=""q5:Request1RequestMessage"" xmlns:q5=""http://microsoft.com/sample/wcf""></xs:element>
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:complexType name=""Request1RequestMessage"">
                      <xs:complexContent mixed=""false"">
                        <xs:extension base=""q6:BaseRequest"" xmlns:q6=""http://microsoft.com/sample/wcf"">
                          <xs:sequence>
                            <xs:element minOccurs=""0"" name=""Request1Data"" nillable=""true"" type=""xs:string""></xs:element>
                          </xs:sequence>
                        </xs:extension>
                      </xs:complexContent>
                    </xs:complexType>
                    <xs:element name=""Request1RequestMessage"" nillable=""true"" type=""q7:Request1RequestMessage"" xmlns:q7=""http://microsoft.com/sample/wcf""></xs:element>
                    <xs:complexType name=""BaseRequest"">
                      <xs:sequence>
                        <xs:element name=""Language"" nillable=""true"" type=""xs:string""></xs:element>
                      </xs:sequence>
                    </xs:complexType>
                    <xs:element name=""BaseRequest"" nillable=""true"" type=""q8:BaseRequest"" xmlns:q8=""http://microsoft.com/sample/wcf""></xs:element>
                    <xs:element name=""Request1Response"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element minOccurs=""0"" name=""Request1Result"" nillable=""true"" type=""q9:Request1ResponseMessage"" xmlns:q9=""http://microsoft.com/sample/wcf""></xs:element>
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:complexType name=""Request1ResponseMessage"">
                      <xs:complexContent mixed=""false"">
                        <xs:extension base=""q10:BaseResponse"" xmlns:q10=""http://microsoft.com/sample/wcf"">
                          <xs:sequence>
                            <xs:element minOccurs=""0"" name=""Request1Data"" nillable=""true"" type=""xs:string""></xs:element>
                          </xs:sequence>
                        </xs:extension>
                      </xs:complexContent>
                    </xs:complexType>
                    <xs:element name=""Request1ResponseMessage"" nillable=""true"" type=""q11:Request1ResponseMessage"" xmlns:q11=""http://microsoft.com/sample/wcf""></xs:element>
                    <xs:complexType name=""BaseResponse"">
                      <xs:sequence></xs:sequence>
                    </xs:complexType>
                    <xs:element name=""BaseResponse"" nillable=""true"" type=""q12:BaseResponse"" xmlns:q12=""http://microsoft.com/sample/wcf""></xs:element>
                    <xs:element name=""PlaceOrder"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element minOccurs=""0"" name=""request"" nillable=""true"" type=""q13:ProductOrderEntity"" xmlns:q13=""http://microsoft.com/sample/wcf""></xs:element>
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:complexType name=""ProductOrderEntity"">
                      <xs:sequence>
                        <xs:element minOccurs=""0"" name=""Product"" nillable=""true"" type=""q14:ProductEntity"" xmlns:q14=""http://microsoft.com/sample/wcf""></xs:element>
                        <xs:element minOccurs=""0"" name=""Quantity"" type=""xs:int""></xs:element>
                      </xs:sequence>
                    </xs:complexType>
                    <xs:element name=""ProductOrderEntity"" nillable=""true"" type=""q15:ProductOrderEntity"" xmlns:q15=""http://microsoft.com/sample/wcf""></xs:element>
                    <xs:complexType name=""ProductEntity"">
                      <xs:sequence>
                        <xs:element minOccurs=""0"" name=""Code"" nillable=""true"" type=""xs:string""></xs:element>
                        <xs:element minOccurs=""0"" name=""Weight"" type=""xs:float""></xs:element>
                      </xs:sequence>
                    </xs:complexType>
                    <xs:element name=""ProductEntity"" nillable=""true"" type=""q16:ProductEntity"" xmlns:q16=""http://microsoft.com/sample/wcf""></xs:element>
                    <xs:element name=""PlaceOrderResponse"">
                      <xs:complexType>
                        <xs:sequence></xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:element name=""Logoff"">
                      <xs:complexType>
                        <xs:sequence></xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:element name=""LogoffResponse"">
                      <xs:complexType>
                        <xs:sequence></xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:element name=""Performance"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element minOccurs=""0"" name=""length"" type=""xs:int""></xs:element>
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:element name=""PerformanceResponse"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element minOccurs=""0"" name=""PerformanceResult"" nillable=""true"" type=""xs:string""></xs:element>
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:element name=""LogException"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element minOccurs=""0"" name=""exceptionDetails"" nillable=""true"" type=""xs:string""></xs:element>
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:element name=""LogExceptionResponse"">
                      <xs:complexType>
                        <xs:sequence></xs:sequence>
                      </xs:complexType>
                    </xs:element>
                  </xs:schema>
                </wsx:MetadataSection>
                <wsx:MetadataSection Dialect=""http://www.w3.org/2001/XMLSchema"" Identifier=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns="""">
                  <xs:schema attributeFormDefault=""qualified"" elementFormDefault=""qualified"" targetNamespace=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:tns=""http://schemas.microsoft.com/2003/10/Serialization/"">
                    <xs:element name=""anyType"" nillable=""true"" type=""xs:anyType""></xs:element>
                    <xs:element name=""anyURI"" nillable=""true"" type=""xs:anyURI""></xs:element>
                    <xs:element name=""base64Binary"" nillable=""true"" type=""xs:base64Binary""></xs:element>
                    <xs:element name=""boolean"" nillable=""true"" type=""xs:boolean""></xs:element>
                    <xs:element name=""byte"" nillable=""true"" type=""xs:byte""></xs:element>
                    <xs:element name=""dateTime"" nillable=""true"" type=""xs:dateTime""></xs:element>
                    <xs:element name=""decimal"" nillable=""true"" type=""xs:decimal""></xs:element>
                    <xs:element name=""double"" nillable=""true"" type=""xs:double""></xs:element>
                    <xs:element name=""float"" nillable=""true"" type=""xs:float""></xs:element>
                    <xs:element name=""int"" nillable=""true"" type=""xs:int""></xs:element>
                    <xs:element name=""long"" nillable=""true"" type=""xs:long""></xs:element>
                    <xs:element name=""QName"" nillable=""true"" type=""xs:QName""></xs:element>
                    <xs:element name=""short"" nillable=""true"" type=""xs:short""></xs:element>
                    <xs:element name=""string"" nillable=""true"" type=""xs:string""></xs:element>
                    <xs:element name=""unsignedByte"" nillable=""true"" type=""xs:unsignedByte""></xs:element>
                    <xs:element name=""unsignedInt"" nillable=""true"" type=""xs:unsignedInt""></xs:element>
                    <xs:element name=""unsignedLong"" nillable=""true"" type=""xs:unsignedLong""></xs:element>
                    <xs:element name=""unsignedShort"" nillable=""true"" type=""xs:unsignedShort""></xs:element>
                    <xs:element name=""char"" nillable=""true"" type=""tns:char""></xs:element>
                    <xs:simpleType name=""char"">
                      <xs:restriction base=""xs:int""></xs:restriction>
                    </xs:simpleType>
                    <xs:element name=""duration"" nillable=""true"" type=""tns:duration""></xs:element>
                    <xs:simpleType name=""duration"">
                      <xs:restriction base=""xs:duration"">
                        <xs:pattern value=""\-?P(\d*D)?(T(\d*H)?(\d*M)?(\d*(\.\d*)?S)?)?""></xs:pattern>
                        <xs:minInclusive value=""-P10675199DT2H48M5.4775808S""></xs:minInclusive>
                        <xs:maxInclusive value=""P10675199DT2H48M5.4775807S""></xs:maxInclusive>
                      </xs:restriction>
                    </xs:simpleType>
                    <xs:element name=""guid"" nillable=""true"" type=""tns:guid""></xs:element>
                    <xs:simpleType name=""guid"">
                      <xs:restriction base=""xs:string"">
                        <xs:pattern value=""[\da-fA-F]{8}-[\da-fA-F]{4}-[\da-fA-F]{4}-[\da-fA-F]{4}-[\da-fA-F]{12}""></xs:pattern>
                      </xs:restriction>
                    </xs:simpleType>
                    <xs:attribute name=""FactoryType"" type=""xs:QName""></xs:attribute>
                  </xs:schema>
                </wsx:MetadataSection>
              </Metadata>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a service side message.
        /// </summary>
        public const string SubtractServiceSide = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-07-28T16:35:40.5880000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""ConsoleHost"" ProcessID=""4764"" ThreadID=""4"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-07-28T09:35:40.5870000-07:00"" Source=""ServiceLevelReceiveRequest"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpRequest>
            <Method>POST</Method>
            <QueryString></QueryString>
            <WebHeaders>
              <SOAPAction>""http://tempuri.org/IArithmetic/Sub""</SOAPAction>
              <Content-Length>149</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Expect>100-continue</Expect>
              <Host>localhost:8081</Host>
            </WebHeaders>
          </HttpRequest>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <To s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://localhost:8081/arithmetic/basic</To>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://tempuri.org/IArithmetic/Sub</Action>
            </s:Header>
            <s:Body>
              <Sub xmlns=""http://tempuri.org/"">
                <a>2</a>
                <b>1</b>
              </Sub>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-07-28T16:35:40.5900000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""ConsoleHost"" ProcessID=""4764"" ThreadID=""4"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-07-28T09:35:40.5900000-07:00"" Source=""ServiceLevelSendReply"" Type=""System.ServiceModel.Channels.BodyWriterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://tempuri.org/IArithmetic/SubResponse</Action>
            </s:Header>
            <s:Body>
              <SubResponse xmlns=""http://tempuri.org/"">
                <SubResult>1</SubResult>
              </SubResponse>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>";

        /// <summary>
        /// Sample test data for a bare Asmx request using soap 1.1
        /// </summary>
        public const string ProcessSimpleAsmxRequestBareSoap11 = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
	<System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
		<EventID>0</EventID>
		<Type>3</Type>
		<SubType Name=""Information"">0</SubType>
		<Level>8</Level>
		<TimeCreated SystemTime=""2008-09-17T13:31:46.6028626Z"" />
		<Source Name=""System.ServiceModel.MessageLogging"" />
		<Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
		<Execution ProcessName=""Client.vshost"" ProcessID=""7140"" ThreadID=""9"" />
		<Channel/>
		<Computer>RJARRATT000</Computer>
	</System>
	<ApplicationData>
		<TraceData>
			<DataItem>
				<MessageLogTraceRecord Time=""2008-09-17T13:31:46.6020000Z"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
					<HttpRequest>
						<Method>POST</Method>
						<QueryString></QueryString>
						<WebHeaders>
							<VsDebuggerCausalityData>uIDPo53rAoOsN11Lhse+HESthmYAAAAAk+1pTuYQfEG1YGMbtnWjEtUDxGX1aoVDmVbBo9zJQfkACAAA</VsDebuggerCausalityData>
						</WebHeaders>
					</HttpRequest>
					<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
						<s:Header>
							<Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestBare</Action>
						</s:Header>
						<s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
							<r A=""99"" xmlns=""http://contoso.com/asmxservice/test"">
								<B>hello</B>
							</r>
						</s:Body>
					</s:Envelope>
				</MessageLogTraceRecord>
			</DataItem>
		</TraceData>
	</ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a bare Asmx request using soap 1.2
        /// </summary>
        public const string ProcessSimpleAsmxRequestBareSoap12 = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
	<System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
		<EventID>0</EventID>
		<Type>3</Type>
		<SubType Name=""Information"">0</SubType>
		<Level>8</Level>
		<TimeCreated SystemTime=""2008-09-17T13:31:46.6658689Z"" />
		<Source Name=""System.ServiceModel.MessageLogging"" />
		<Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
		<Execution ProcessName=""Client.vshost"" ProcessID=""7140"" ThreadID=""9"" />
		<Channel/>
		<Computer>RJARRATT000</Computer>
	</System>
	<ApplicationData>
		<TraceData>
			<DataItem>
				<MessageLogTraceRecord Time=""2008-09-17T14:31:46.6660000Z"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
					<HttpRequest>
						<Method>POST</Method>
						<QueryString></QueryString>
						<WebHeaders>
							<VsDebuggerCausalityData>uIDPo5/rAoOsN11Lhse+HESthmYAAAAAk+1pTuYQfEG1YGMbtnWjEtUDxGX1aoVDmVbBo9zJQfkACAAA</VsDebuggerCausalityData>
						</WebHeaders>
					</HttpRequest>
					<s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope"">
						<s:Header>
							<Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestBare</Action>
						</s:Header>
						<s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
							<r A=""99"" xmlns=""http://contoso.com/asmxservice/test"">
								<B>hello</B>
							</r>
						</s:Body>
					</s:Envelope>
				</MessageLogTraceRecord>
			</DataItem>
		</TraceData>
	</ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a wrapped Asmx request using soap 1.1
        /// </summary>
        public const string ProcessSimpleAsmxRequestWrappedSoap11 = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
	<System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
		<EventID>0</EventID>
		<Type>3</Type>
		<SubType Name=""Information"">0</SubType>
		<Level>8</Level>
		<TimeCreated SystemTime=""2008-09-17T13:31:45.5347558Z"" />
		<Source Name=""System.ServiceModel.MessageLogging"" />
		<Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
		<Execution ProcessName=""Client.vshost"" ProcessID=""7140"" ThreadID=""9"" />
		<Channel/>
		<Computer>RJARRATT000</Computer>
	</System>
	<ApplicationData>
		<TraceData>
			<DataItem>
				<MessageLogTraceRecord Time=""2008-09-17T14:31:45.3520000Z"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
					<HttpRequest>
						<Method>POST</Method>
						<QueryString></QueryString>
						<WebHeaders>
							<VsDebuggerCausalityData>uIDPo5zrAoOsN11Lhse+HESthmYAAAAAk+1pTuYQfEG1YGMbtnWjEtUDxGX1aoVDmVbBo9zJQfkACAAA</VsDebuggerCausalityData>
						</WebHeaders>
					</HttpRequest>
					<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
						<s:Header>
							<Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestWrapped</Action>
						</s:Header>
						<s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
							<ProcessSimpleAsmxRequestWrapped xmlns=""http://contoso.com/asmxservice/test"">
								<r A=""99"">
									<B>hello</B>
								</r>
							</ProcessSimpleAsmxRequestWrapped>
						</s:Body>
					</s:Envelope>
				</MessageLogTraceRecord>
			</DataItem>
		</TraceData>
	</ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a wrapped Asmx request using soap 1.2
        /// </summary>
        public const string ProcessSimpleAsmxRequestWrappedSoap12 = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
	<System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
		<EventID>0</EventID>
		<Type>3</Type>
		<SubType Name=""Information"">0</SubType>
		<Level>8</Level>
		<TimeCreated SystemTime=""2008-09-17T13:31:46.6508674Z"" />
		<Source Name=""System.ServiceModel.MessageLogging"" />
		<Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
		<Execution ProcessName=""Client.vshost"" ProcessID=""7140"" ThreadID=""9"" />
		<Channel/>
		<Computer>RJARRATT000</Computer>
	</System>
	<ApplicationData>
		<TraceData>
			<DataItem>
				<MessageLogTraceRecord Time=""2008-09-17T14:31:46.6230000Z"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
					<HttpRequest>
						<Method>POST</Method>
						<QueryString></QueryString>
						<WebHeaders>
							<VsDebuggerCausalityData>uIDPo57rAoOsN11Lhse+HESthmYAAAAAk+1pTuYQfEG1YGMbtnWjEtUDxGX1aoVDmVbBo9zJQfkACAAA</VsDebuggerCausalityData>
						</WebHeaders>
					</HttpRequest>
					<s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope"">
						<s:Header>
							<Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestWrapped</Action>
						</s:Header>
						<s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
							<ProcessSimpleAsmxRequestWrapped xmlns=""http://contoso.com/asmxservice/test"">
								<r A=""99"">
									<B>hello</B>
								</r>
							</ProcessSimpleAsmxRequestWrapped>
						</s:Body>
					</s:Envelope>
				</MessageLogTraceRecord>
			</DataItem>
		</TraceData>
	</ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a message where the addressing namespace is on the actual action element rather than higher in the hierarchy.
        /// </summary>
        public const string RequestWithAddressingNamespaceOnTheActionElement = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2009-04-24T18:10:59.6680000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""WebDev.WebServer"" ProcessID=""5036"" ThreadID=""8"" />
    <Channel/>
    <Computer>WILLYS-THINKPAD</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2009-04-24T18:10:59.66800000Z"" Source=""ServiceLevelReceiveRequest"" Type=""System.ServiceModel.Security.SecurityVerifiedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpRequest>
            <Method>POST</Method>
            <QueryString></QueryString>
            <WebHeaders>
              <Content-Length>6071</Content-Length>
              <Content-Type>application/soap+xml; charset=utf-8</Content-Type>
              <Expect>100-continue</Expect>
              <Host>localhost:56028</Host>
              <VsDebuggerCausalityData>uIDPo18CppDeF79Im7XTRzBmnMAAAAAA/lfJ2lV0tUeHcg709iLGncJRMBe7gNNBvMiLEmrN/UIACQAA</VsDebuggerCausalityData>
            </WebHeaders>
          </HttpRequest>
          <s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope"">
            <s:Header>
              <a:Action s:mustUnderstand=""1"" u:Id=""_2"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:a=""http://www.w3.org/2005/08/addressing"">http://tempuri.org/ICalculator/Ping</a:Action>
              <a:MessageID u:Id=""_3"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:a=""http://www.w3.org/2005/08/addressing"">urn:uuid:1d51a323-9df6-4dc3-8cd4-4d0905f713ab</a:MessageID>
              <a:ReplyTo u:Id=""_4"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:a=""http://www.w3.org/2005/08/addressing"">
                <a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>
              </a:ReplyTo>
              <a:To s:mustUnderstand=""1"" u:Id=""_5"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:a=""http://www.w3.org/2005/08/addressing"">http://localhost:56028/CalculatorService.svc</a:To>
              <o:Security s:mustUnderstand=""1"" xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                <u:Timestamp u:Id=""uuid-af80ea00-83c1-4588-918a-104ec8e48bcf-11"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                  <u:Created>2009-04-24T18:10:59.661Z</u:Created>
                  <u:Expires>2009-04-24T18:15:59.661Z</u:Expires>
                </u:Timestamp>
                <c:SecurityContextToken u:Id=""uuid-43a81682-a3c0-4b75-a81d-ed6bd7904d39-4"" xmlns:c=""http://schemas.xmlsoap.org/ws/2005/02/sc"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                  <c:Identifier>urn:uuid:71f8e92c-dcaa-4fb8-8e64-c177c4235327</c:Identifier>
                </c:SecurityContextToken>
                <c:DerivedKeyToken u:Id=""uuid-af80ea00-83c1-4588-918a-104ec8e48bcf-9"" xmlns:c=""http://schemas.xmlsoap.org/ws/2005/02/sc"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                  <o:SecurityTokenReference>
                    <o:Reference ValueType=""http://schemas.xmlsoap.org/ws/2005/02/sc/sct"" URI=""#uuid-43a81682-a3c0-4b75-a81d-ed6bd7904d39-4""></o:Reference>
                  </o:SecurityTokenReference>
                  <c:Offset>0</c:Offset>
                  <c:Length>24</c:Length>
                  <c:Nonce>
                    <!--Removed-->
                  </c:Nonce>
                </c:DerivedKeyToken>
                <c:DerivedKeyToken u:Id=""uuid-af80ea00-83c1-4588-918a-104ec8e48bcf-10"" xmlns:c=""http://schemas.xmlsoap.org/ws/2005/02/sc"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                  <o:SecurityTokenReference>
                    <o:Reference ValueType=""http://schemas.xmlsoap.org/ws/2005/02/sc/sct"" URI=""#uuid-43a81682-a3c0-4b75-a81d-ed6bd7904d39-4""></o:Reference>
                  </o:SecurityTokenReference>
                  <c:Nonce>
                    <!--Removed-->
                  </c:Nonce>
                </c:DerivedKeyToken>
                <e:ReferenceList xmlns:e=""http://www.w3.org/2001/04/xmlenc#"">
                  <e:DataReference URI=""#_1""></e:DataReference>
                  <e:DataReference URI=""#_6""></e:DataReference>
                </e:ReferenceList>
                <Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">
                  <SignedInfo>
                    <CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></CanonicalizationMethod>
                    <SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#hmac-sha1""></SignatureMethod>
                    <Reference URI=""#_0"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>aAfSM5N2MOa424mkhg4SpSe4JHc=</DigestValue>
                    </Reference>
                    <Reference URI=""#_2"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>eQO7mtLl4a3+W19bhat8tuBrDGM=</DigestValue>
                    </Reference>
                    <Reference URI=""#_3"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>vD8hA6IsJokXvzSNEM4CaSoKMDw=</DigestValue>
                    </Reference>
                    <Reference URI=""#_4"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>l6mMmQ2LE9VFtjaA6Qc4GKBXURw=</DigestValue>
                    </Reference>
                    <Reference URI=""#_5"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>ucCrd8WEMUjVreugN0LHkHBwy70=</DigestValue>
                    </Reference>
                    <Reference URI=""#uuid-af80ea00-83c1-4588-918a-104ec8e48bcf-11"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>UP2gsTKJE9fjjyXfrJJyq+Qxpqk=</DigestValue>
                    </Reference>
                  </SignedInfo>
                  <SignatureValue>6ZkFZLw2rB+ScRpK0n9i6b0WZNo=</SignatureValue>
                  <KeyInfo>
                    <o:SecurityTokenReference xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                      <o:Reference ValueType=""http://schemas.xmlsoap.org/ws/2005/02/sc/dk"" URI=""#uuid-af80ea00-83c1-4588-918a-104ec8e48bcf-9""></o:Reference>
                    </o:SecurityTokenReference>
                  </KeyInfo>
                </Signature>
              </o:Security>
            </s:Header>
            <s:Body u:Id=""_0"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
              <Ping xmlns=""http://tempuri.org/""></Ping>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a message where the addressing namespace is on the actual action element rather than higher in the hierarchy and the action header is not the first header.
        /// </summary>
        public const string RequestWithAddressingNamespaceOnTheActionElementAndNotFirstHeader = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2010-06-22T14:22:51.6132764Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""7180"" ThreadID=""4"" />
    <Channel/>
    <Computer>RJARRATT001</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2010-06-22T15:22:51.61300000Z"" Source=""ServiceLevelReceiveDatagram"" Type=""System.ServiceModel.Security.SecurityVerifiedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpRequest>
            <Method>POST</Method>
            <QueryString></QueryString>
            <WebHeaders>
              <Content-Length>6385</Content-Length>
              <Content-Type>application/soap+xml; charset=utf-8</Content-Type>
              <Expect>100-continue</Expect>
              <Host>rjarratt001.europe.corp.microsoft.com</Host>
            </WebHeaders>
          </HttpRequest>
          <s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope"">
            <s:Header>
              <r:AckRequested u:Id=""_2"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:r=""http://schemas.xmlsoap.org/ws/2005/02/rm"">
                <r:Identifier>urn:uuid:b1e4c3f2-c033-4f10-b082-5060d24bb764</r:Identifier>
              </r:AckRequested>
              <r:Sequence s:mustUnderstand=""1"" u:Id=""_3"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:r=""http://schemas.xmlsoap.org/ws/2005/02/rm"">
                <r:Identifier>urn:uuid:b1e4c3f2-c033-4f10-b082-5060d24bb764</r:Identifier>
                <r:MessageNumber>3</r:MessageNumber>
              </r:Sequence>
              <a:Action s:mustUnderstand=""1"" u:Id=""_4"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:a=""http://www.w3.org/2005/08/addressing"">http://contoso.com/service/test/ISharePrices/PriceOneWay</a:Action>
              <ActivityId CorrelationId=""46b2eb10-d457-47fa-a5ab-f21abf187108"" xmlns=""http://schemas.microsoft.com/2004/09/ServiceModel/Diagnostics"">c5b4c591-7481-482d-a89d-b36497dc972a</ActivityId>
              <a:To s:mustUnderstand=""1"" u:Id=""_5"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:a=""http://www.w3.org/2005/08/addressing"">http://rjarratt001.europe.corp.microsoft.com/Temporary_Listen_Addresses/bf48686b-a3fe-4e7d-80f5-9adad8176bc9/28ffbfac-d078-45ef-a7db-defffafea6c0</a:To>
              <o:Security s:mustUnderstand=""1"" xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                <u:Timestamp u:Id=""uuid-27bf398c-8b75-4f4c-8086-bdb359895ce9-15"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                  <u:Created>2010-06-22T14:22:51.610Z</u:Created>
                  <u:Expires>2010-06-22T14:27:51.610Z</u:Expires>
                </u:Timestamp>
                <c:DerivedKeyToken u:Id=""uuid-27bf398c-8b75-4f4c-8086-bdb359895ce9-7"" xmlns:c=""http://schemas.xmlsoap.org/ws/2005/02/sc"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                  <o:SecurityTokenReference>
                    <o:Reference URI=""urn:uuid:acbf7077-02e0-48e7-b11e-b2affc365945"" ValueType=""http://schemas.xmlsoap.org/ws/2005/02/sc/sct""></o:Reference>
                  </o:SecurityTokenReference>
                  <c:Offset>0</c:Offset>
                  <c:Length>24</c:Length>
                  <c:Nonce>
                    <!--Removed-->
                  </c:Nonce>
                </c:DerivedKeyToken>
                <c:DerivedKeyToken u:Id=""uuid-27bf398c-8b75-4f4c-8086-bdb359895ce9-8"" xmlns:c=""http://schemas.xmlsoap.org/ws/2005/02/sc"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                  <o:SecurityTokenReference>
                    <o:Reference URI=""urn:uuid:acbf7077-02e0-48e7-b11e-b2affc365945"" ValueType=""http://schemas.xmlsoap.org/ws/2005/02/sc/sct""></o:Reference>
                  </o:SecurityTokenReference>
                  <c:Nonce>
                    <!--Removed-->
                  </c:Nonce>
                </c:DerivedKeyToken>
                <e:ReferenceList xmlns:e=""http://www.w3.org/2001/04/xmlenc#"">
                  <e:DataReference URI=""#_1""></e:DataReference>
                  <e:DataReference URI=""#_6""></e:DataReference>
                </e:ReferenceList>
                <Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">
                  <SignedInfo>
                    <CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></CanonicalizationMethod>
                    <SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#hmac-sha1""></SignatureMethod>
                    <Reference URI=""#_0"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>IQlHYc+2I9gwx2YpWGVnjRyf3g4=</DigestValue>
                    </Reference>
                    <Reference URI=""#_2"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>RI5eyriRSlowOS1lNXwtpj3SKls=</DigestValue>
                    </Reference>
                    <Reference URI=""#_3"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>k0y1Qmt4ZLn6aPfefiqB7Xno//c=</DigestValue>
                    </Reference>
                    <Reference URI=""#_4"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>HZMPqTWHn097ugj+1fBg25IfEg4=</DigestValue>
                    </Reference>
                    <Reference URI=""#_5"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>QwdOnrEYSoH+ntPLlWn3cPmYjho=</DigestValue>
                    </Reference>
                    <Reference URI=""#uuid-27bf398c-8b75-4f4c-8086-bdb359895ce9-15"">
                      <Transforms>
                        <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>
                      </Transforms>
                      <DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>
                      <DigestValue>fzdKPNeYZIrSpdeln7J7ipRBfpU=</DigestValue>
                    </Reference>
                  </SignedInfo>
                  <SignatureValue>HAQ251n5f6CXX7/omUYifOp09Og=</SignatureValue>
                  <KeyInfo>
                    <o:SecurityTokenReference xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                      <o:Reference ValueType=""http://schemas.xmlsoap.org/ws/2005/02/sc/dk"" URI=""#uuid-27bf398c-8b75-4f4c-8086-bdb359895ce9-7""></o:Reference>
                    </o:SecurityTokenReference>
                  </KeyInfo>
                </Signature>
              </o:Security>
            </s:Header>
            <s:Body u:Id=""_0"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
              <PriceOneWay xmlns=""http://contoso.com/service/test"">
                <symbol>MSFT</symbol>
                <price>31</price>
              </PriceOneWay>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a one-way client side message.
        /// </summary>
        public const string OneWayClientSide = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2009-07-23T09:52:16.6160000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""7648"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT000</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2009-07-23T09:52:16.61600000Z"" Source=""ServiceLevelSendDatagram"" Type=""System.ServiceModel.Channels.BodyWriterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://contoso.com/service/test/IArithmetic/OneWayOperation</Action>
            </s:Header>
            <s:Body>
              <OneWayOperation xmlns=""http://contoso.com/service/test"">
                <a>5</a>
              </OneWayOperation>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>";

        /// <summary>
        /// Sample test data for a one way service side message.
        /// </summary>
        public const string OneWayServiceSide = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2009-07-23T09:54:38.1550000Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{61839b54-2cc6-49dd-a298-0b7882a4926a}"" />
    <Execution ProcessName=""Host"" ProcessID=""4976"" ThreadID=""3"" />
    <Channel/>
    <Computer>RJARRATT000</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2009-07-23T09:54:38.15500000Z"" Source=""ServiceLevelReceiveDatagram"" Type=""System.ServiceModel.Channels.BufferedMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpRequest>
            <Method>POST</Method>
            <QueryString></QueryString>
            <WebHeaders>
              <SOAPAction>""http://contoso.com/service/test/IArithmetic/OneWayOperation""</SOAPAction>
              <Content-Length>177</Content-Length>
              <Content-Type>text/xml; charset=utf-8</Content-Type>
              <Expect>100-continue</Expect>
              <Host>localhost:8081</Host>
            </WebHeaders>
          </HttpRequest>
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <To s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://localhost:8081/arithmetic/basic</To>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://contoso.com/service/test/IArithmetic/OneWayOperation</Action>
            </s:Header>
            <s:Body>
              <OneWayOperation xmlns=""http://contoso.com/service/test"">
                <a>5</a>
              </OneWayOperation>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue message.
        /// </summary>
        public const string RstIssue = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2010-06-22T14:22:50.1839906Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""7180"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT001</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2010-06-22T15:22:50.1829904+01:00"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Channels.BodyWriterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:a=""http://www.w3.org/2005/08/addressing"" xmlns:s=""http://www.w3.org/2003/05/soap-envelope"">
            <s:Header>
              <a:Action s:mustUnderstand=""1"">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</a:Action>
              <a:MessageID>urn:uuid:f7441b7b-2aa3-49f5-b2e3-61b30c95e44c</a:MessageID>
              <a:ReplyTo>
                <a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>
              </a:ReplyTo>
            </s:Header>
            <s:Body>... stream ...</s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a http://schemas.xmlsoap.org/ws/2005/02/trust/RST/SCT message.
        /// </summary>
        public const string RstSct = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2010-06-22T14:22:50.2299998Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""7180"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT001</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2010-06-22T15:22:50.2299998+01:00"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Channels.BodyWriterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:a=""http://www.w3.org/2005/08/addressing"" xmlns:s=""http://www.w3.org/2003/05/soap-envelope"">
            <s:Header>
              <a:Action s:mustUnderstand=""1"">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/SCT</a:Action>
              <a:MessageID>urn:uuid:0f6e5205-98e0-49e2-a465-569e30134de0</a:MessageID>
            </s:Header>
            <s:Body>
              <t:RequestSecurityToken xmlns:t=""http://schemas.xmlsoap.org/ws/2005/02/trust"">
                <t:TokenType>http://schemas.xmlsoap.org/ws/2005/02/sc/sct</t:TokenType>
                <t:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</t:RequestType>
                <t:Entropy>
                  <!--Removed-->
                </t:Entropy>
                <t:KeySize>256</t:KeySize>
              </t:RequestSecurityToken>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// Sample test data for a http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/Issue message.
        /// </summary>
        public const string RstrIssue = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2010-06-22T14:22:50.2019942Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""7180"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT001</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2010-06-22T15:22:50.2019942+01:00"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Channels.BodyWriterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:a=""http://www.w3.org/2005/08/addressing"" xmlns:s=""http://www.w3.org/2003/05/soap-envelope"">
            <s:Header>
              <a:Action s:mustUnderstand=""1"">http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/Issue</a:Action>
              <a:MessageID>urn:uuid:16ad4df5-2efb-4049-af20-da47af1bdd9d</a:MessageID>
              <a:ReplyTo>
                <a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>
              </a:ReplyTo>
            </s:Header>
            <s:Body>
              <t:RequestSecurityTokenResponse Context=""uuid-a62ec5e7-3eb8-4c40-a490-699a46fc6b6c-1"" xmlns:t=""http://schemas.xmlsoap.org/ws/2005/02/trust"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                <t:BinaryExchange ValueType=""http://schemas.xmlsoap.org/ws/2005/02/trust/spnego"" EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">oXcwdaADCgEBoloEWE5UTE1TU1AAAwAAAAAAAABYAAAAAAAAAFgAAAAAAAAAWAAAAAAAAABYAAAAAAAAAFgAAAAAAAAAWAAAADXCmOIGAbAdAAAAD1QdPA4qOpxzuKjbclhbcvWjEgQQAQAAAPUXp1AtIpqEAAAAAA==</t:BinaryExchange>
              </t:RequestSecurityTokenResponse>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>
";

        /// <summary>
        /// The kind of null message found in service side traces.
        /// </summary>
        public const string ServiceSideNullMessage = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2010-07-06T12:54:07.3217742Z"" />
    <Source Name=""System.ServiceModel.MessageLogging"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""w3wp"" ProcessID=""13036"" ThreadID=""3"" />
    <Channel/>
    <Computer>OPTI-LT-SW</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2010-07-06T14:54:07.3217742+02:00"" Source=""ServiceLevelReceiveRequest"" Type=""System.ServiceModel.Channels.NullMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <HttpRequest>
            <Method>GET</Method>
            <QueryString>xsd=xsd2</QueryString>
            <WebHeaders>
              <Connection>Keep-Alive</Connection>
              <Host>opti-lt-sw.sbp.co.za</Host>
            </WebHeaders>
          </HttpRequest>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>";

        /// <summary>
        /// Example of a trace record that appears in traces and not message logs.
        /// </summary>
        public const string TraceRecord = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Transfer"">0</SubType>
    <Level>255</Level>
    <TimeCreated SystemTime=""2010-07-15T06:28:18.5905388Z"" />
    <Source Name=""System.ServiceModel"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" RelatedActivityID=""{d3a57aa7-edbb-43f1-9d61-7d46f2b2e5cb}"" />
    <Execution ProcessName=""Client"" ProcessID=""10896"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT001</Computer>
  </System>
  <ApplicationData></ApplicationData>
</E2ETraceEvent>";

        /// <summary>
        /// Sample test data for a log record that does not have a source element in the system element.
        /// </summary>
        public const string LogRecordWithoutASource = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:21.9940000Z"" />
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:21.9660000+00:00"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://tempuri.org/IArithmetic/Add</Action>
            </s:Header>
            <s:Body>
              <Add xmlns=""http://tempuri.org/"">
                <request xmlns:d4p1=""http://schemas.datacontract.org/2004/07/Contracts"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                  <d4p1:A>10</d4p1:A>
                  <d4p1:B>5</d4p1:B>
                </request>
              </Add>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>";

        /// <summary>
        /// Sample test data for a log record that has a source element in the system element, but no name attribute
        /// </summary>
        public const string LogRecordWithSourceButNoSourceName = @"<E2ETraceEvent xmlns=""http://schemas.microsoft.com/2004/06/E2ETraceEvent"">
  <System xmlns=""http://schemas.microsoft.com/2004/06/windows/eventlog/system"">
    <EventID>0</EventID>
    <Type>3</Type>
    <SubType Name=""Information"">0</SubType>
    <Level>8</Level>
    <TimeCreated SystemTime=""2007-02-21T08:39:21.9940000Z"" />
    <Source/>
    <Correlation ActivityID=""{00000000-0000-0000-0000-000000000000}"" />
    <Execution ProcessName=""Client"" ProcessID=""3936"" ThreadID=""1"" />
    <Channel/>
    <Computer>RJARRATT003</Computer>
  </System>
  <ApplicationData>
    <TraceData>
      <DataItem>
        <MessageLogTraceRecord Time=""2007-02-21T08:39:21.9660000+00:00"" Source=""ServiceLevelSendRequest"" Type=""System.ServiceModel.Dispatcher.OperationFormatter+OperationFormatterMessage"" xmlns=""http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace"">
          <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
            <s:Header>
              <Action s:mustUnderstand=""1"" xmlns=""http://schemas.microsoft.com/ws/2005/05/addressing/none"">http://tempuri.org/IArithmetic/Add</Action>
            </s:Header>
            <s:Body>
              <Add xmlns=""http://tempuri.org/"">
                <request xmlns:d4p1=""http://schemas.datacontract.org/2004/07/Contracts"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                  <d4p1:A>10</d4p1:A>
                  <d4p1:B>5</d4p1:B>
                </request>
              </Add>
            </s:Body>
          </s:Envelope>
        </MessageLogTraceRecord>
      </DataItem>
    </TraceData>
  </ApplicationData>
</E2ETraceEvent>";

        /// <summary>
        /// Data that is not even an XML file.
        /// </summary>
        public const string RandomTextTraceRecord = "this is not even XML";

        /// <summary>
        /// Data that is XML but not from WCF E2E tracing.
        /// </summary>
        public const string RandomXmlData = "<somethingelse/>";
    }
}
