using SAEON.Logs;
using SAEON.Observations.Data;
using System;
using System.Runtime.CompilerServices;

/// <summary>
/// Summary description for Auditing
/// </summary>
public static class Auditing
{
    public static void Log(Type type, MethodCallParameters parameters = null, [CallerMemberName] string methodName = "")
    {
        using (SAEONLogs.MethodCall(typeof(Auditing), new MethodCallParameters { { "methodName", methodName }, { "parameters", parameters } }))
        {
            try
            {
                AuditLog auditLog = new AuditLog
                {
                    AddedAt = null,
                    UpdatedAt = null,
                    Description = MethodCalls.MethodSignature(type, methodName, parameters),
                    UserId = AuthHelper.GetLoggedInUserId
                };
                auditLog.Save();
            }
            catch (Exception ex)
            {
                try
                {
                    SAEONLogs.Exception(ex);
                }
                catch (Exception)
                {
                    SAEONLogs.Exception(ex, "Log({methodName})", methodName);
                }
            }
        }
    }

}