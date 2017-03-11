using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Core
{
    public static class Logging
    {
        public static void ErrorInCall(Exception ex, string message = "", params object[] values)
        {
            Log.Error(ex, message, values);
        }

        public static void ErrorInCall(this object obj, Exception ex, string message = "", params object[] values)
        {
            ErrorInCall(ex, message, values);
        }

        public static IDisposable MethodCall(Type type, object[] parameters = null, [CallerMemberName] string methodName = "")
        {
            var methodCall = $"{type.FullName}.{methodName}(";
            bool first = true;
            if (parameters != null)
                foreach (var param in parameters)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        methodCall += ", ";
                    }
                    methodCall += nameof(param);
                }
            methodCall += ")";
            var result = LogContext.PushProperty("Method", methodCall);
            Log.Verbose(methodCall);
            return result;
        }

        public static IDisposable MethodCall(this object obj, object[] parameters = null, [CallerMemberName] string methodName = "")
        {
            return MethodCall(obj.GetType(), parameters, methodName);
        }

        public static IDisposable MethodCall<TRelatedEntity>(Type type, object[] parameters = null, [CallerMemberName] string methodName = "")
        {
            var methodCall = $"{type.FullName}.{methodName}<{nameof(TRelatedEntity)}>(";
            bool first = true;
            if (parameters != null)
                foreach (var param in parameters)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        methodCall += ", ";
                    }
                    methodCall += nameof(param);
                }
            methodCall += ")";
            var result = LogContext.PushProperty("Method", methodCall);
            Log.Verbose(methodCall);
            return result;
        }

        public static IDisposable MethodCall<TRelatedEntity>(this object obj, object[] parameters = null, [CallerMemberName] string methodName = "")
        {
            return MethodCall<TRelatedEntity>(obj.GetType(), parameters, methodName);
        }

    }
}
