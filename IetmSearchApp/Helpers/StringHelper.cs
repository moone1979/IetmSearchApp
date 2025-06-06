using System.Globalization;
using System.Text;

namespace ItemSearchApp.Helpers
{
    public static class StringHelper
    {
        public static string Normalize(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            string normalized = input.Normalize(NormalizationForm.FormKC);
            normalized = normalized.ToLowerInvariant();

            // ひらがな→カタカナ変換
            var sb = new StringBuilder();
            foreach (char c in normalized)
            {
                if (c >= 0x3041 && c <= 0x3096)
                    sb.Append((char)(c + 0x60));
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
