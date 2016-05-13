using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Utilities
/// </summary>
public static class Utilities
{
    public static Guid MakeGuid(object value)
    {
        if ((value == null) || ((value is string) && string.IsNullOrEmpty((string)value)))
            return Guid.Empty;
        else
            return new Guid(value.ToString());
    }
}