using SAEON.Logs;
using SAEON.Observations.Data;
using System;
using System.Runtime.CompilerServices;

/// <summary>
/// Summary description for Auditing
/// </summary>
public static class Auditing
{
    public static void Log(Type type, ParameterList parameters = null, [CallerMemberName] string methodName = "")
    {
        using (Logging.MethodCall(typeof(Auditing), new ParameterList { { "methodName", methodName }, { "parameters", parameters } }))
        {
            try
            {
                AuditLog auditLog = new AuditLog
                {
                    AddedAt = null,
                    UpdatedAt = null,
                    Description = Logging.MethodSignature(type, methodName, parameters),
                    UserId = AuthHelper.GetLoggedInUserId
                };
                auditLog.Save();
            }
            catch (Exception ex)
            {
                try
                {
                    Logging.Exception(ex);
                }
                catch (Exception)
                {
                    Logging.Exception(ex, "Log({methodName})", methodName);
                }
            }
        }
    }

}