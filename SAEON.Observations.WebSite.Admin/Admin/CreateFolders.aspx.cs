using DocumentFormat.OpenXml.Packaging;
using Ext.Net;
using SAEON.Logs;
using SAEON.OpenXML;
using System;
using System.IO;

public partial class Admin_CreateFolders : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //TemplateFile.Text = @"G:\My Drive\Elwandle\Node Drive\Data Store\Observations\Observations Database Setup Template.xlsx";
            ObservationsFolder.Text = @"D:\Observations";
        }
    }

    protected void CreateClick(object sender, DirectEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            if (!TemplateFile.HasFile)
            {
                ExtNet.Msg.Hide();
                MessageBoxes.Error("Error", "No Template Spreadsheet selected");
                return;
            }
            if (!Directory.Exists(ObservationsFolder.Text))
            {
                ExtNet.Msg.Hide();
                MessageBoxes.Error("Error", $"Unable to find {ObservationsFolder.Text}");
                return;
            }
            try
            {
                Logging.Information("FileName: {FileName}",TemplateFile.PostedFile.FileName);
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(TemplateFile.PostedFile.InputStream, false))
                {

                    var instruments = ExcelHelper.GetRangeValues(doc, "Instruments!T3:T102");
                    foreach (string instrument in instruments)
                    {
                        if (!string.IsNullOrWhiteSpace(instrument))
                        {
                            var splits = instrument.Split(new string[] { ", " }, StringSplitOptions.None);
                            Logging.Information("Programme: {Programmee} Project: {Project} Site: {Site} Station: {Station} Instrument: {Instrument}", splits[0], splits[1], splits[2], splits[3], splits[4]);
                            Directory.CreateDirectory(Path.Combine(ObservationsFolder.Text, splits[0], splits[1], splits[2], splits[3], splits[4], "Operational metadata"));
                            Directory.CreateDirectory(Path.Combine(ObservationsFolder.Text, splits[0], splits[1], splits[2], splits[3], splits[4], "Files", "Version 00"));
                            Directory.CreateDirectory(Path.Combine(ObservationsFolder.Text, splits[0], splits[1], splits[2], splits[3], splits[4], "Files", "Version 01"));
                            Directory.CreateDirectory(Path.Combine(ObservationsFolder.Text, splits[0], splits[1], splits[2], splits[3], splits[4], "Files", "Version 02"));
                            Directory.CreateDirectory(Path.Combine(ObservationsFolder.Text, splits[0], splits[1], splits[2], splits[3], splits[4], "Metadata records"));
                        }
                    }
                }
                ExtNet.Msg.Hide();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                ExtNet.Msg.Hide();
                MessageBoxes.Error("Error", ex.Message);
            }
        }
    }

    protected void ValidateTemplateFile(object sender, RemoteValidationEventArgs e)
    {
        if (File.Exists(TemplateFile.Text))
        {
            e.Success = true;
        }
        else
        {
            e.Success = false;
            e.ErrorMessage = "Template spreadsheet does not exist";
        }
    }
    protected void ValidateObservationFolder(object sender, RemoteValidationEventArgs e)
    {
        if (Directory.Exists(ObservationsFolder.Text))
        {
            e.Success = true;
        }
        else
        {
            e.Success = false;
            e.ErrorMessage = "Observations Folder does not exist";
        }
    }
}