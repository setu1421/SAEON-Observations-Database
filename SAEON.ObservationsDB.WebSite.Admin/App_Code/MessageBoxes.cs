using Ext.Net;
using System;

/// <summary>
/// Summary description for MessageBoxes
/// </summary>
public static class MessageBoxes
{
    public static void Error(string message, params object[] values)
    {
        X.Msg.Show(new MessageBoxConfig
        {
            Title = "Error",
            Message = string.Format(message, values),
            Buttons = MessageBox.Button.OK,
            Icon = MessageBox.Icon.ERROR
        });
    }

    public static void Error(Exception ex, string message, params object[] values)
    {
        if ((ex == null) || string.IsNullOrEmpty(ex.Message))
            Error(message, values);
        else
            Error(string.Format(message, values) + Environment.NewLine + "Exception: " + ex.Message);
    }

    public static void Info(string title, string message, params object[] values)
    {
        X.Msg.Show(new MessageBoxConfig
        {
            Title = title,
            Message = string.Format(message, values),
            Buttons = MessageBox.Button.OK,
            Icon = MessageBox.Icon.INFO
        });
    }


}