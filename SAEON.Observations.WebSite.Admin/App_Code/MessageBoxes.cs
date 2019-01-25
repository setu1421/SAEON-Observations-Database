using Ext.Net;
using System;

/// <summary>
/// Summary description for MessageBoxes
/// </summary>
public static class MessageBoxes
{
    public static void Confirm(string title, string handler, string message, params object[] values)
    {
        X.Msg.Confirm(title, string.Format(message, values), new MessageBoxButtonsConfig
        {
            Yes = new MessageBoxButtonConfig { Handler = handler, Text = "Yes" },
            No = new MessageBoxButtonConfig { Text = "No" }
        }).Show(); 
    }

    public static void Error(string title, string message, params object[] values)
    {
        X.Msg.Show(new MessageBoxConfig
        {
            Title = title,
            Message = string.Format(message, values),
            Buttons = MessageBox.Button.OK,
            Icon = MessageBox.Icon.ERROR
        });
    }

    public static void Error(Exception ex, string title, string message, params object[] values)
    {
        if ((ex == null) || string.IsNullOrEmpty(ex.Message))
            Error(title, message, values);
        else
            Error(title, string.Format(message, values) + Environment.NewLine + "Exception: " + ex.Message);
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

    public static void Warning(string title, string message, params object[] values)
    {
        X.Msg.Show(new MessageBoxConfig
        {
            Title = title,
            Message = string.Format(message, values),
            Buttons = MessageBox.Button.OK,
            Icon = MessageBox.Icon.WARNING
        });
    }


}