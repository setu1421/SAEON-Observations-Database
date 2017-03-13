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
    public class ParameterList : Dictionary<string, object> { }

    public static class Logging
    {
        public static bool UseFullName { get; set; } = true;

        public static void Exception(Exception ex, string message = "", params object[] values)
        {
            Log.Error(ex, string.IsNullOrEmpty(message) ? "An exception occured" : message, values);
        }

        //public static void LogException(this object obj, Exception ex, string message = "", params object[] values)
        //{
        //    LogException(ex, message, values);
        //}

        public static void Error(string message = "", params object[] values)
        {
            Log.Error(string.IsNullOrEmpty(message) ? "An error occured" : message, values);
        }

        //public static void LogError(this object obj, string message = "", params object[] values)
        //{
        //    LogError(message, values);
        //}

        public static void Information(string message, params object[] values)
        {
            Log.Information(message, values);
        }

        //public static void LogInformation(this object obj, string message, params object[] values)
        //{
        //    LogInformation(message, values);
        //}

        private static string GetTypeName(Type type, bool onlyName = false)
        {
            return UseFullName && !onlyName ? type.FullName : type.Name;
        }
        
        private static string GetParamNames(ParameterList parameters)
        {
            string result = string.Empty;
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
                        result += ", ";
                    }
                    result += param.Key;
                }
            return result;
        }

        public static IDisposable MethodCall(Type type, ParameterList parameters = null, [CallerMemberName] string methodName = "")
        {
            var methodCall = $"{GetTypeName(type)}.{methodName}({GetParamNames(parameters)})";
            var result = LogContext.PushProperty("Method", methodCall);
            Log.Verbose(methodCall);
            return result;
        }

        //public static IDisposable LogMethodCall(this object obj, ParameterList parameters = null, [CallerMemberName] string methodName = "")
        //{
        //    return LogMethodCall(obj.GetType(), parameters, methodName);
        //}

        public static IDisposable MethodCall<TEntity>(Type type, ParameterList parameters = null, [CallerMemberName] string methodName = "")
        {
            var methodCall = $"{GetTypeName(type)}.{methodName}<{GetTypeName(typeof(TEntity),true)}>({GetParamNames(parameters)})";
            var result = LogContext.PushProperty("Method", methodCall);
            Log.Verbose(methodCall);
            return result;
        }

        //public static IDisposable LogMethodCall<TEntity>(this object obj, ParameterList parameters = null, [CallerMemberName] string methodName = "")
        //{
        //    return LogMethodCall<TEntity>(obj.GetType(), parameters, methodName);
        //}

        public static IDisposable MethodCall<TEntity, TRelatedEntity>(Type type, ParameterList parameters = null, [CallerMemberName] string methodName = "")
        {
            var methodCall = $"{GetTypeName(type)}.{methodName}<{GetTypeName(typeof(TEntity),true)},{GetTypeName(typeof(TRelatedEntity),true)}>({GetParamNames(parameters)})";
            var result = LogContext.PushProperty("Method", methodCall);
            Log.Verbose(methodCall);
            return result;
        }

        //public static IDisposable LogMethodCall<TEntity, TRelatedEntity>(this object obj, ParameterList parameters = null, [CallerMemberName] string methodName = "")
        //{
        //    return LogMethodCall<TEntity, TRelatedEntity>(obj.GetType(), parameters, methodName);
        //}

        public static void Verbose(string message, params object[] values)
        {
            Log.Verbose(message, values);
        }

        //public static void LogVerbose(this object obj, string message, params object[] values)
        //{
        //    LogVerbose(message, values);
        //}
    }
}
