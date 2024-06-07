using System.Text;
using Microsoft.IdentityModel.Tokens;
using UglyToad.PdfPig;

namespace pdfyai_api.Utils
{
    public class PdfAuthils
    {
        public bool CanReadDocuments(IFormFile documents)
        {

            try
            {

                PdfDocument pdf = PdfDocument.Open(documents.OpenReadStream());
                StringBuilder stringBuilder = new();
                for (int i = 0; i < pdf.NumberOfPages; i++)
                {
                    var page = pdf.GetPage(i + 1);
                    stringBuilder.Append(page.Text);
                }

                if (stringBuilder.ToString() == "" || stringBuilder.ToString().IsNullOrEmpty())
                {
                    return false;
                }

                return true;

            }
            catch (Exception)
            {
                return false;
            }





        }
    }
}