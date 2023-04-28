using System;
using System.IO;
using Newtonsoft.Json.Linq;
using iTextSharp.text.pdf;


namespace LuxAt163FDPDFGenerationFunction
{
    public class PDFGeneration
    {
        public MemoryStream FillForm(dynamic data, string Read163RFTemplate)
        {
            try
            {
                //string result = "No Data No Results";
                string jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                                
                if (jsondata != null)
                {                   
                    using (MemoryStream outputPdfStream = new MemoryStream())
                    {
                        using (PdfReader pdfReader = new PdfReader(Read163RFTemplate))
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
                                pdfStamper.FormFlattening = false;
                                pdfStamper.Close();

                            }
                        }
                        
                        var file = outputPdfStream.ToArray();
                        var output = new MemoryStream();
                        output.Write(file, 0, file.Length);
                        output.Position = 0;
                       

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

