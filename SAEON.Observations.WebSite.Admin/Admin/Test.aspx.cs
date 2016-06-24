using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_Test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CultureInfo culture = CultureInfo.CurrentCulture;
        var numberFormatInfo = NumberFormatInfo.GetInstance(culture);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(string.Format("<br />Currency Decimal Digits: {0}", numberFormatInfo.CurrencyDecimalDigits));
        sb.AppendLine(string.Format("<br />Currency Decimal Separator: {0}", numberFormatInfo.CurrencyDecimalSeparator));
        sb.AppendLine(string.Format("<br />Currency Group Separator: {0}", numberFormatInfo.CurrencyGroupSeparator));
        sb.AppendLine(string.Format("<br />Currency Group Sizes: {0}", String.Join(",", numberFormatInfo.CurrencyGroupSizes)));
        sb.AppendLine(string.Format("<br />Currency Negative Pattern: {0}", numberFormatInfo.CurrencyNegativePattern));
        sb.AppendLine(string.Format("<br />Currency Positive Pattern: {0}", numberFormatInfo.CurrencyPositivePattern));
        sb.AppendLine(string.Format("<br />Currency Symbol: {0}", numberFormatInfo.CurrencySymbol));
        sb.AppendLine(string.Format("<br />Digit Substitution: {0}", numberFormatInfo.DigitSubstitution));
        sb.AppendLine(string.Format("<br />NaN Symbol: {0}", numberFormatInfo.NaNSymbol));
        sb.AppendLine(string.Format("<br />Native Digits: {0}", String.Join(",", numberFormatInfo.NativeDigits)));
        sb.AppendLine(string.Format("<br />Negative Infinity Symbol: {0}", numberFormatInfo.NegativeInfinitySymbol));
        sb.AppendLine(string.Format("<br />Negative Sign: {0}", numberFormatInfo.NegativeSign));
        sb.AppendLine(string.Format("<br />Number Decimal Digits: {0}", numberFormatInfo.NumberDecimalDigits));
        sb.AppendLine(string.Format("<br />Number Decimal Separator: {0}", numberFormatInfo.NumberDecimalSeparator));
        sb.AppendLine(string.Format("<br />Number Group Separator: {0}", numberFormatInfo.NumberGroupSeparator));
        sb.AppendLine(string.Format("<br />Number Group Sizes: {0}", String.Join(",", numberFormatInfo.NumberGroupSizes)));
        sb.AppendLine(string.Format("<br />Number Negative Pattern: {0}", numberFormatInfo.NumberNegativePattern));
        sb.AppendLine(string.Format("<br />Percent Decimal Digits: {0}", String.Join(",", numberFormatInfo.PercentDecimalDigits)));
        sb.AppendLine(string.Format("<br />Percent Decimal Separator: {0}", numberFormatInfo.PercentDecimalSeparator));
        sb.AppendLine(string.Format("<br />Percent Group Separator: {0}", numberFormatInfo.PercentGroupSeparator));
        sb.AppendLine(string.Format("<br />Percent Group Sizes: {0}", String.Join(",", numberFormatInfo.PercentGroupSizes)));
        sb.AppendLine(string.Format("<br />Percent Negative Pattern: {0}", numberFormatInfo.PercentNegativePattern));
        sb.AppendLine(string.Format("<br />Percent Positive Pattern: {0}", numberFormatInfo.PercentPositivePattern));
        sb.AppendLine(string.Format("<br />Percent Symbol: {0}", numberFormatInfo.PercentSymbol));
        sb.AppendLine(string.Format("<br />Per Mille Symbol: {0}", numberFormatInfo.PerMilleSymbol));
        sb.AppendLine(string.Format("<br />Positive InfinitySymbol: {0}", numberFormatInfo.PositiveInfinitySymbol));
        sb.AppendLine(string.Format("<br />Positive Sign: {0}", numberFormatInfo.PositiveSign));
        lblTest.Text = sb.ToString();
    }
}