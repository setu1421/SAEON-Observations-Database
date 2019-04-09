using DocumentFormat.OpenXml.Packaging;
using Ext.Net;
using SAEON.Logs;
using SAEON.OpenXML;
using System;
using System.IO;

public partial class Admin_Folders : System.Web.UI.Page
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
            //if (!File.Exists(TemplateFile.Text))
            //{
            //    ExtNet.Msg.Hide();
            //    MessageBoxes.Error("Error", $"Unable to find {TemplateFile.Text}");
            //    return;
            //}
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
                    /*
                    // Programmes
                    var programmeNames = ExcelHelper.GetRangeValues(doc, "Programmes!B3:B102");
                    var programmesList = ExcelHelper.GetRangeValues(doc, "Programmes!G3:H102");
                    var projectNames = ExcelHelper.GetRangeValues(doc, "Projects!E3:E102");
                    var projectsList = ExcelHelper.GetRangeValues(doc, "Projects!J3:K102");
                    var projectProgrammeCodes = ExcelHelper.GetRangeValues(doc, "Projects!A3:A102");
                    var stationNames = ExcelHelper.GetRangeValues(doc, "Stations!H3:H102");
                    var stationsList = ExcelHelper.GetRangeValues(doc, "Stations!P3:Q102");
                    var stationProjects = ExcelHelper.GetRangeValues(doc, "Stations!A3:B102");
                    var stationSites = ExcelHelper.GetRangeValues(doc, "Stations!D3:E102");
                    for (int rProgramme = 0; rProgramme < programmeNames.GetUpperBound(0)+1; rProgramme++)
                    {
                        var programmeName = (string)programmeNames[rProgramme, 0];
                        if (!string.IsNullOrWhiteSpace(programmeName))
                        {
                            Logging.Information("Programme: {Programme}", programmeName);
                            Directory.CreateDirectory(Path.Combine(ObservationsFolder.Text, programmeName));
                            var programmeCode = (string)programmesList[rProgramme, 0];
                            // Projects
                            for (int rProject = 0; rProject < projectNames.GetUpperBound(0)+1; rProject++)
                            {
                                var projectName = (string)projectNames[rProject,0];
                                if (!string.IsNullOrWhiteSpace(projectName))
                                {
                                    var projectCode = (string)projectsList[rProject, 0];
                                    var projectProgrammeCode = (string)projectProgrammeCodes[rProject, 0];
                                    if (projectProgrammeCode.EndsWith(programmeCode))
                                    {
                                        Logging.Information("Programme: {Programme} Project: {Project}", programmeName, projectName);
                                        Directory.CreateDirectory(Path.Combine(ObservationsFolder.Text, programmeName, projectName));
                                        // Sites
                                        // Stations
                                        for (int rStation = 0; rStation<stationNames.GetUpperBound(0)+1;rStation++)
                                        {
                                            var stationName = (string)stationNames[rStation, 0];
                                            if (!string.IsNullOrWhiteSpace(stationName))
                                            {
                                                var stationCode = (string)stationsList[rStation, 0];
                                                var stationProjectCode = (string)stationProjects[rStation, 0];
                                                var stationSiteCode = (string)stationSites[rStation, 0];
                                                if (stationProjectCode == projectCode)
                                                {
                                                    var stationSiteName = (string)stationSites[rStation, 1];
                                                    Logging.Information("Programme: {Programme} Project: {Project} Site: {Site} Station: {Station}", programmeName, projectName, stationSiteName, stationName);
                                                    Directory.CreateDirectory(Path.Combine(ObservationsFolder.Text, programmeName, projectName, stationSiteName, stationName));
                                                    // Instruments
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    */
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