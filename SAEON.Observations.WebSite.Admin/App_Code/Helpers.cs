using Ext.Net;

public static class TextFieldExtensions
{
    public static bool HasValue(this TextField field)
    {
        field.Text = field.Text.Trim();
        return !field.IsEmpty;
    }
}

public static class TextAreaExtensions
{
    public static bool HasValue(this TextArea field)
    {
        field.Text = field.Text.Trim();
        return !field.IsEmpty;
    }
}

public static class DateFieldExtensions
{
    public static bool HasValue(this DateField field)
    {
        return !field.IsEmpty && (field.SelectedDate.Year >= 1900);
    }
}

public static class HiddenExtensions
{
    public static bool HasValue(this Hidden field)
    {
        field.Text = field.Text.Trim();
        return !field.IsEmpty;
    }
}