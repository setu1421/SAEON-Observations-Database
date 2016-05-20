using SAEON.Observations.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

/// <summary>
/// Summary description for Auditing
/// </summary>
public static class Auditing
{
    public static string MethodCall(string name, Dictionary<string,object> parameters)
    {
        string result = name+"(";
        bool isFirst = true;
        foreach (var kvPair in parameters)
        {
            if (!isFirst) result += ", ";
            isFirst = false;
            result += kvPair.Key + "=";
            if (kvPair.Value is string)  
                result += string.Format("'{0}'", kvPair.Value);
            //else if (kvPair.Value is Guid)
            //    result += string.Format("{0}", kvPair.Value);
            else
                result += kvPair.Value.ToString();
        }
        result += ")";
        return result;
    }

    //public static void Log(string description, params object[] values)
    //{
    //    try
    //    {
    //        AuditLog auditLog = new AuditLog();
    //        auditLog.Description = string.Format(description,values);
    //        auditLog.UserId = AuthHelper.GetLoggedInUserId;
    //        auditLog.Save();
    //    }
    //    catch (Exception ex)
    //    {
    //        Serilog.Log.Error(ex, "Log({0})", string.Format(description, values));
    //    }
    //}

    public static void Log(string methodName, Dictionary<string, object> methodParameters)
    {
        try
        {
            AuditLog auditLog = new AuditLog();
            auditLog.AddedAt = DateTime.Now;
            auditLog.Description = Auditing.MethodCall(methodName, methodParameters);
            auditLog.UserId = AuthHelper.GetLoggedInUserId;
            auditLog.Save();
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Log({0})", methodName);
        }
    }

}