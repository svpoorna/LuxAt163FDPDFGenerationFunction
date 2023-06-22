using System;
using System.IO;
using Newtonsoft.Json.Linq;
using iTextSharp.text.pdf;
using static iTextSharp.text.pdf.codec.TiffWriter;


namespace LuxAt163FDPDFGenerationFunction
{
    public class PDFGeneration
    {
        public ResponseMessageModel FillForm(dynamic data, string Read163RFTemplate)
        {
            ResponseMessageModel responseMessageModel = new ResponseMessageModel();
            try
            {
               string jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(data);


                using (MemoryStream outputPdfStream = new MemoryStream())
                {
                    using (PdfReader pdfReader = new PdfReader(Read163RFTemplate))
                    {
                        using (PdfStamper pdfStamper = new PdfStamper(pdfReader, outputPdfStream))
                        {
                            var fieldtype=string.Empty;
                            
                            AcroFields pdfFormFields = pdfStamper.AcroFields;                           

                            // Convert hidden calculated fileds to readable
                            makeFieldsVisibile(pdfFormFields);                            
                            JObject json = JObject.Parse(jsondata);
                            foreach (var field in json)
                              if (pdfFormFields.Fields.ContainsKey(field.Key))
                                   pdfFormFields.SetField(field.Key, field.Value?.ToString());                                    
                             
                            //if the FormFlattening value is set to true, the PDF will be locked against further edits. 
                            pdfStamper.FormFlattening = false;                      
                            pdfStamper.Close(); 

                        }
                    }

                    var file = outputPdfStream.ToArray();
                    var output = new MemoryStream();
                    output.Write(file, 0, file.Length);
                    output.Position = 0;
                    responseMessageModel.memoryStream = output;
                    responseMessageModel.isRequestSuccess = true;
                    responseMessageModel.responseMessage = "PDF File Generated";

                    return responseMessageModel;

                }

            }
            catch (Exception ex)
            {
                //Catch the real exception and pass it back                
                responseMessageModel.isRequestSuccess = false;
                responseMessageModel.responseMessage = ex.Message.ToString();
                return responseMessageModel;
            }
        }

        private void makeFieldsVisibile(AcroFields pdfFormFields)
        {

            foreach (var fieldEntry in pdfFormFields.Fields)
            {
                string fieldName = fieldEntry.Key;
                AcroFields.Item item = fieldEntry.Value;

                if (fieldName.StartsWith("T"))
                {
                    pdfFormFields.SetFieldProperty(fieldName, "flags", PdfAnnotation.FLAGS_PRINT, null);

                    pdfFormFields.SetFieldProperty(fieldName, "visibility", BaseField.VISIBLE, null);
                }

            }
            //trow new NotImplementedException();
        }
    }
}

