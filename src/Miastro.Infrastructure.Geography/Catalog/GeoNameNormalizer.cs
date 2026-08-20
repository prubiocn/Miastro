using System.Globalization;
using System.Text;

namespace Miastro.Infrastructure.Geography.Catalog;

public static class GeoNameNormalizer
{
    public static string Normalize(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var decomposed = value
            .Trim()
            .Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder(decomposed.Length);
        var previousWasSpace = false;

        foreach (var ch in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) ==
                UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            var c = char.ToLowerInvariant(ch);

            if (char.IsWhiteSpace(c))
            {
                if (!previousWasSpace && builder.Length > 0)
                {
                    builder.Append(' ');
                    previousWasSpace = true;
                }

                continue;
            }

            builder.Append(c);
            previousWasSpace = false;
        }

        return builder
            .ToString()
            .Trim()
            .Normalize(NormalizationForm.FormC);
    }
}
