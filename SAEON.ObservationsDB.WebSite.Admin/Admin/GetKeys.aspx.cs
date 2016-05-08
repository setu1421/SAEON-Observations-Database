using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_GetKeys : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        MachineKeySection section = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");
        Decryption.Text = section.Decryption;
        DecryptionKey.Text = section.DecryptionKey;
        Validation.Text = section.Validation.ToString();
        ValidationAlgorithm.Text = section.ValidationAlgorithm;
        ValidationKey.Text = section.ValidationKey;
        var machineKey = typeof(MachineKeySection).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Single(a => a.Name == "GetApplicationConfig").Invoke(null, new object[0]);

        var type = Assembly.Load("System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").GetTypes().Single(a => a.Name == "MachineKeyMasterKeyProvider");

        var instance = type.Assembly.CreateInstance(
            type.FullName, false,
            BindingFlags.Instance | BindingFlags.NonPublic,
            null, new object[] { machineKey, null, null, null, null }, null, null);

        var validationKey = type.GetMethod("GetValidationKey").Invoke(instance, new object[0]);
        var key = (byte[])validationKey.GetType().GetMethod("GetKeyMaterial").Invoke(validationKey, new object[0]);

        DecryptionKeyValue.Text = BitConverter.ToString(GetKey(instance, "GetEncryptionKey")).Replace("-", "");
        ValidationKeyValue.Text = BitConverter.ToString(GetKey(instance, "GetValidationKey")).Replace("-", "");
    }

    public static byte[] GetKey(object provider, string name)
    {
        var validationKey = provider.GetType().GetMethod(name).Invoke(provider, new object[0]);
        return (byte[])validationKey.GetType().GetMethod("GetKeyMaterial").Invoke(validationKey, new object[0]);
    }
}