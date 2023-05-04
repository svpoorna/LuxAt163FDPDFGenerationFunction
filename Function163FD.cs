using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LuxAt163FDPDFGenerationFunction;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Collections.Generic;
using Azure.Core;

namespace Function163FD
{
    public static class Function163FD
    {
        [FunctionName("Function163FD")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,ExecutionContext context)
        {
            
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
                HttpResponseMessage responseMessage = new HttpResponseMessage();

                //If request body with out content it will terminate the process
                if (req.Body == null)
                {
                    responseMessage.StatusCode = HttpStatusCode.BadRequest;                    
                    return responseMessage;
                }
                string name = req.Query["name"];

                //Testing with local Data
                string path = @"C:\Users\ADMIN\Downloads\sample190.json";
                string requestBody = await new StreamReader(path).ReadToEndAsync();

                //Remove comments wen you do with online data
                //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;
                

                //Read Formtype value from the given json and read respective template from environment variable
                GetJSON fileContent = JsonConvert.DeserializeObject<GetJSON>(requestBody);
                string taxFormName = fileContent.TaxFormLanguage.ToString();

                //Dynamic call to find lauguage template French/German
                string Template163Path = GetEnvironmentVariable(taxFormName);
                PDFGeneration GenerateTaxForm = new PDFGeneration();


                ResponseMessageModel fileStream = GenerateTaxForm.FillForm(data, Template163Path);

                //string responseMessage =  results;
                responseMessage.StatusCode= HttpStatusCode.OK;
                responseMessage.Content = new StreamContent(fileStream.memoryStream);
                responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                return responseMessage;
                

                // return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage();
                responseMessage.StatusCode = HttpStatusCode.BadRequest;
                responseMessage.Content= new StringContent(ex.Message.ToString()+"No PDF Generated");
                return responseMessage;
                //throw ex;
            }
            
        }
        //GetEnvironmentVariables for local and azure
        public static string GetEnvironmentVariable(string name)
        {
            return
                System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

        public class GetJSON
        {
            public string TaxFormLanguage { get; set; }
            
        }

        
    }
}
