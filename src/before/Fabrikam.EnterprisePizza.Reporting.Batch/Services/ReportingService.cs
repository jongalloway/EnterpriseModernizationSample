using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Fabrikam.EnterprisePizza.Reporting.Batch.Configuration;
using Fabrikam.EnterprisePizza.Reporting.Batch.Models;

namespace Fabrikam.EnterprisePizza.Reporting.Batch.Services
{
    public class ReportingService
    {
        private const string SoapEnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
        private const string ReportExecutionNamespace = "http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices/ReportExecution2005";
        private readonly ReadOnlyCollection<ReportingDataSourceConfiguration> dataSources;

        public ReportingService()
            : this(ReportingServiceConfiguration.LoadFromAppSettings(), ReportingDataSourceConfiguration.LoadFromAppSettings())
        {
        }

        public ReportingService(ReportingServiceConfiguration configuration, IEnumerable<ReportingDataSourceConfiguration> dataSources)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            if (dataSources == null)
            {
                throw new ArgumentNullException(nameof(dataSources));
            }

            var configuredSources = dataSources.ToList();
            if (configuredSources.Count == 0)
            {
                throw new ArgumentException("At least one reporting data source configuration is required.", nameof(dataSources));
            }

            this.dataSources = new ReadOnlyCollection<ReportingDataSourceConfiguration>(configuredSources);
        }

        public ReportingServiceConfiguration Configuration { get; }

        public ReadOnlyCollection<ReportingDataSourceConfiguration> DataSources => dataSources;

        public ReportingDataSourceConfiguration GetDataSource(string sharedDataSourcePath)
        {
            if (string.IsNullOrWhiteSpace(sharedDataSourcePath))
            {
                throw new ArgumentException("Shared data source path is required.", nameof(sharedDataSourcePath));
            }

            var normalizedPath = ReportingServiceConfiguration.NormalizeServerPath(sharedDataSourcePath);
            var match = dataSources.FirstOrDefault(dataSource => string.Equals(dataSource.SharedDataSourcePath, normalizedPath, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                throw new InvalidOperationException("No reporting data source is configured for '" + normalizedPath + "'.");
            }

            return match;
        }

        public ReportExecutionRequest CreateExecutionRequest(ReportDefinition reportDefinition, IReportParameterModel parameterModel)
        {
            if (reportDefinition == null)
            {
                throw new ArgumentNullException(nameof(reportDefinition));
            }

            if (parameterModel == null)
            {
                throw new ArgumentNullException(nameof(parameterModel));
            }

            GetDataSource(reportDefinition.SharedDataSourcePath);
            return new ReportExecutionRequest(
                Configuration.ExecutionEndpoint,
                reportDefinition.ReportPath,
                Configuration.DefaultRenderFormat,
                parameterModel.ToReportParameters(),
                Configuration.ExecutionTimeoutSeconds);
        }

        public string BuildLoadReportSoapEnvelope(ReportExecutionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return CreateSoapEnvelope(
                null,
                new XElement(XName.Get("LoadReport", ReportExecutionNamespace),
                    new XElement(XName.Get("Report", ReportExecutionNamespace), request.ReportPath),
                    new XElement(XName.Get("HistoryID", ReportExecutionNamespace), string.Empty)));
        }

        public string BuildSetExecutionParametersSoapEnvelope(string executionId, IEnumerable<ReportParameterValue> parameters)
        {
            if (string.IsNullOrWhiteSpace(executionId))
            {
                throw new ArgumentException("Execution identifier is required.", nameof(executionId));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return CreateSoapEnvelope(
                executionId,
                new XElement(XName.Get("SetExecutionParameters", ReportExecutionNamespace),
                    new XElement(XName.Get("Parameters", ReportExecutionNamespace),
                        parameters.Select(parameter =>
                            new XElement(XName.Get("ParameterValue", ReportExecutionNamespace),
                                new XElement(XName.Get("Name", ReportExecutionNamespace), parameter.Name),
                                new XElement(XName.Get("Value", ReportExecutionNamespace), parameter.Value)))),
                    new XElement(XName.Get("ParameterLanguage", ReportExecutionNamespace), "en-US")));
        }

        public string BuildRenderSoapEnvelope(ReportExecutionRequest request, string executionId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(executionId))
            {
                throw new ArgumentException("Execution identifier is required.", nameof(executionId));
            }

            return CreateSoapEnvelope(
                executionId,
                new XElement(XName.Get("Render", ReportExecutionNamespace),
                    new XElement(XName.Get("Format", ReportExecutionNamespace), request.RenderFormat),
                    new XElement(XName.Get("DeviceInfo", ReportExecutionNamespace), "<DeviceInfo><Toolbar>False</Toolbar></DeviceInfo>")));
        }

        public byte[] Render(ReportExecutionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var loadResponse = ExecuteSoapRequest(BuildLoadReportSoapEnvelope(request), "LoadReport");
            var executionId = ExtractExecutionId(loadResponse);

            if (request.Parameters.Count > 0)
            {
                ExecuteSoapRequest(BuildSetExecutionParametersSoapEnvelope(executionId, request.Parameters), "SetExecutionParameters");
            }

            var renderResponse = ExecuteSoapRequest(BuildRenderSoapEnvelope(request, executionId), "Render");
            return ExtractRenderedBytes(renderResponse);
        }

        private string ExecuteSoapRequest(string soapEnvelope, string operationName)
        {
            var request = (HttpWebRequest)WebRequest.Create(Configuration.ExecutionEndpoint);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";
            request.Accept = "text/xml";
            request.Timeout = Configuration.ExecutionTimeoutSeconds * 1000;
            request.Headers.Add("SOAPAction", ReportExecutionNamespace + "/" + operationName);

            if (Configuration.UseDefaultCredentials)
            {
                request.Credentials = CredentialCache.DefaultCredentials;
            }

            var payload = Encoding.UTF8.GetBytes(soapEnvelope);
            request.ContentLength = payload.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(payload, 0, payload.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream ?? Stream.Null))
            {
                return reader.ReadToEnd();
            }
        }

        private static string CreateSoapEnvelope(string executionId, XElement bodyContent)
        {
            XNamespace soap = SoapEnvelopeNamespace;
            XNamespace reportExecution = ReportExecutionNamespace;
            var envelope = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(soap + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "soap", soap),
                    new XAttribute(XNamespace.Xmlns + "rexec", reportExecution),
                    string.IsNullOrWhiteSpace(executionId)
                        ? null
                        : new XElement(soap + "Header",
                            new XElement(reportExecution + "ExecutionHeader",
                                new XElement(reportExecution + "ExecutionID", executionId))),
                    new XElement(soap + "Body", bodyContent)));

            return envelope.ToString();
        }

        private static string ExtractExecutionId(string soapResponse)
        {
            var document = XDocument.Parse(soapResponse);
            var executionId = document.Descendants(XName.Get("ExecutionID", ReportExecutionNamespace)).Select(element => element.Value).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(executionId))
            {
                throw new InvalidOperationException("The SSRS execution response did not include an execution identifier.");
            }

            return executionId;
        }

        private static byte[] ExtractRenderedBytes(string soapResponse)
        {
            var document = XDocument.Parse(soapResponse);
            var renderedPayload = document.Descendants(XName.Get("Result", ReportExecutionNamespace)).Select(element => element.Value).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(renderedPayload))
            {
                throw new InvalidOperationException("The SSRS render response did not include a report payload.");
            }

            return Convert.FromBase64String(renderedPayload);
        }
    }
}
