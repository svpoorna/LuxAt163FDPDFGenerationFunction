using System;
using System.IO;
using Newtonsoft.Json.Linq;
using iTextSharp.text.pdf;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using System.Reflection;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Org.BouncyCastle.Ocsp;
using Azure.Storage.Blobs.Models;

namespace LuxAt163FDPDFGenerationFunction
{
    public class PDFGeneration
    {
        public MemoryStream FillForm(dynamic data, string pdfTemplate, string pdfFileName, string containerName)
        {
            try
            {
                //string result = "No Data No Results";
                string jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                string taxpayerName = string.Empty;
                var binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var parentOfBinPath = new DirectoryInfo(binPath).Parent.FullName;            
                string bloburl = string.Empty;
                string blobfilename = string.Empty;
                if (jsondata != null)
                {
                    //byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfTemplate);
                    using (MemoryStream outputPdfStream = new MemoryStream())
                    {
                        using (PdfReader pdfReader = new PdfReader(pdfTemplate))
                        {
                            using (PdfStamper pdfStamper = new PdfStamper(pdfReader, outputPdfStream))
                            {
                                AcroFields pdfFormFields = pdfStamper.AcroFields;
                                JObject json = JObject.Parse(jsondata);
                                foreach (var field in json)
                                {
                                    //JTokenType fieldtype = field.Value.Type;
                                    string fieldName = field.Key;
                                    string fieldValue = field.Value.ToString();

                                    //  if (field.Key.Equals("JsonKey"))
                                    if (pdfFormFields.Fields.ContainsKey(fieldName))
                                    {
                                        pdfFormFields.SetField(fieldName, fieldValue);
                                    }

                                }
                                //if the value is set to true, the PDF will be locked against further edits. 
                                pdfStamper.FormFlattening = true;
                                pdfStamper.Close();

                            }
                        }
                        //results = AzureFileUploadAsync(memoryStream, containerName).Result;
                        var file = outputPdfStream.ToArray();
                        var output = new MemoryStream();
                        output.Write(file, 0, file.Length);
                        output.Position = 0;
                        //string myFileName = "Output21.pdf";
                        //New line
                        //byte[] pdfBytes = System.IO.File.ReadAllBytes(fileName);

                        //Dynamic Construction from local settings.json
                        //string Connection = Environment.GetEnvironmentVariable("AzureStorageConnection");
                        //// Get a reference to a blob.
                        //BlobServiceClient blobServiceClient = new BlobServiceClient(Connection);
                        //// Get a reference to a container.
                        //BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                        //BlobClient blobClient = containerClient.GetBlobClient(myFileName);
                        // Upload the PDF file to the blob.

                        //blobClient.UploadAsync(output, true);
                        //blobClient.UploadAsync(output, new BlobHttpHeaders { ContentType = "application/pdf" });
                        //bloburl = blobClient.Uri.AbsoluteUri.ToString();
                        //blobfilename = blobClient.Name.ToString();




                        //result = bloburl + "," + blobfilename;

                        return output;

                    }
                }
                else
                {
                    return new MemoryStream();
                }

            }
            catch (Exception ex)
            {
                //Catch the real exception and pass it back
                return new MemoryStream();


            }

        }





    }
}

