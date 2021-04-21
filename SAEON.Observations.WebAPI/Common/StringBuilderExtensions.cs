using System.Collections.Generic;
using System.Text;

namespace SAEON.Observations.WebAPI
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendDD(this StringBuilder builder, string text)
        {
            return builder.AppendLine($"<dd>{text}</dd>");
        }

        public static StringBuilder AppendDDStart(this StringBuilder builder)
        {
            return builder.AppendLine("<dd>");
        }

        public static StringBuilder AppendDDStart(this StringBuilder builder, string text)
        {
            return builder.AppendLine($"<dd>{text}");
        }

        public static StringBuilder AppendDDEnd(this StringBuilder builder)
        {
            return builder.AppendLine("</dd>");
        }

        public static StringBuilder AppendDLStart(this StringBuilder builder)
        {
            return builder.AppendLine("<dl class='dl-horizontal'>");
        }
        public static StringBuilder AppendDLEnd(this StringBuilder builder)
        {
            return builder.AppendLine("</dl>");
        }

        public static StringBuilder AppendDT(this StringBuilder builder, string text)
        {
            return builder.AppendLine($"<dt>{text}</dt>");
        }

        public static StringBuilder AppendDTDD(this StringBuilder builder, string term, string description)
        {
            builder.AppendDT(term);
            return builder.AppendDD(description);
        }

        public static StringBuilder AppendHtmlH2(this StringBuilder builder, string text)
        {
            return builder.AppendLine($"<h2>{text}</h2>");
        }

        public static StringBuilder AppendHtmlH3(this StringBuilder builder, string text)
        {
            return builder.AppendLine($"<h3>{text}</h3>");
        }

        public static StringBuilder AppendHtmlP(this StringBuilder builder, string text)
        {
            return builder.AppendLine($"<p>{text}</p>");
        }

        public static StringBuilder AppendHtmlUL(this StringBuilder builder, List<string> items)
        {
            builder.AppendLine("<ul>");
            foreach (var item in items)
                builder.AppendLine($"<li>{item}</li>");
            return builder.AppendLine("</ul>");
        }

        public static StringBuilder AppendHtmlUL(this StringBuilder builder, IEnumerable<string> items)
        {
            builder.AppendLine("<ul>");
            foreach (var item in items)
                builder.AppendLine($"<li>{item}</li>");
            return builder.AppendLine("</ul>");
        }

        public static StringBuilder AppendTrailing(this StringBuilder builder, string text)
        {
            return builder.ToString().EndsWith(text) ? builder : builder.Append(text);
        }

        public static bool EndsWith(this StringBuilder builder, string text)
        {
            return builder.ToString().EndsWith(text);
        }
    }
}
