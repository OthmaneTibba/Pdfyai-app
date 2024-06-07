namespace pdfyai.Utils
{
    public class AppUtils
    {
        public List<string> SplitTextIntoChunks(string inputText, int chunkSize, int overlapSize)
        {
            List<string> chunks = new List<string>();

            for (int i = 0; i < inputText.Length; i += chunkSize - overlapSize)
            {
                int length = Math.Min(chunkSize, inputText.Length - i);

                chunks.Add(inputText.Substring(i, length));
            }

            return chunks;
        }



        public string RemoveNoAscii(string text)
        {
            return text.Replace(@"[^\u0000-\u007F]+", string.Empty);
        }
    }




}