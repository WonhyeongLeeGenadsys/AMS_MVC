using System.Web.Mvc;

namespace Web.Common
{
    public static class HiColorHelper
    {
        public static string GetHiColor(int hi)
        {
            switch (hi)
            {
                case 1: return "#5B9F56";
                case 2: return "#B4DEB1";
                case 3: return "#EBCD1D";
                case 4: return "#F18425";
                case 5: return "#F05153";
                default: return "#888888";
            }
        }

        public static MvcHtmlString HiCell(this HtmlHelper html, int hiValue)
        {
            var color = GetHiColor(hiValue);
            var td = $"<td style='background-color:{color};color:white;text-align:center;'>{hiValue}</td>";
            return new MvcHtmlString(td);
        }
    }
}
