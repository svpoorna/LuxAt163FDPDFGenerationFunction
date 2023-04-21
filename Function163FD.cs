using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LuxAt163FDPDFGenerationFunction;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace Function163FD
{
    public static class Function163FD
    {
        [FunctionName("Function163FD")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string rootpath = context.FunctionAppDirectory ;

            //var Readpath = Environment.GetEnvironmentVariable("HOME") + @"\site\wwwroot\Read163RF2021.pdf";
            //var Writepath = Environment.GetEnvironmentVariable("HOME") + @"\site\wwwroot\Write163RF2021.pdf";
            var pdffile = context.FunctionAppDirectory;

            //Load the PDF document.
            //string FilePath = Path.Combine(context.FunctionAppDirectory, "Write163RF2021.pdf");

            string name = req.Query["name"];

            //Testing with local Data
            //string path = @"C:\Users\ADMIN\Downloads\sample.json";
            //string requestBody = await new StreamReader(path).ReadToEndAsync();

            //Remove comments wen you do with online data
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string Templatepath = GetEnvironmentVariable("PDFTemplatePath");
            string FilePath = GetEnvironmentVariable("PDFOutPutFileName");
            string ContainerName = GetEnvironmentVariable("ContainerName");

            PDFGeneration PDF100 = new PDFGeneration();

            
           var fileStream = PDF100.FillForm(data, Templatepath, FilePath, ContainerName);
           //var results = PDF100.FillForms(data, Templatepath, TemplateoutfileName, ContainerName);

            //string responseMessage =  results;
            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            responseMessage.Content = new StreamContent(fileStream);
            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            return responseMessage;

           // return new OkObjectResult(responseMessage);
        }
        //GetEnvironmentVariables for local and azure
        public static string GetEnvironmentVariable(string name)
        {
            return
                System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
