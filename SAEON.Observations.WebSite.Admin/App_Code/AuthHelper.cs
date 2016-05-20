using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Remoting.Contexts;
using SAEON.Observations.Data;
using System.Web.Security;

/// <summary>
/// Summary description for AuthHelper
/// </summary>
public class AuthHelper
{
    public AuthHelper()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public static bool IsLoggedIn
    {
        get
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static Guid GetLoggedInUserId
    {
        get
        {
            Guid id = Guid.Empty;

            if (!IsLoggedIn)
                //id =  new Guid(HttpContext.Current.User.Identity.Name.ToString());
                id = new Guid("60cbb958-9657-42f4-ad9b-b1943b4f3386");
            else
            {
                AspnetUser user = new AspnetUser("UserName", HttpContext.Current.User.Identity.Name);

                id = user.UserId;
            }

            return id;
        }
    }

}

public enum UpdateFrequency
{
    AdHoc,
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Annually,
}

public class listHelper
{

    public static Dictionary<int, string> GetUpdateFrequencyList()
    {
        Dictionary<int, string> list = new Dictionary<int, string>();

        Array values = Enum.GetValues(typeof(UpdateFrequency)); 

        foreach(UpdateFrequency val in values) 
        { 
            int ival = (int)val;

            switch (ival)
	        {
                case 0:
                    list.Add(ival, "Ad-Hoc");
                    break;
		        default:
                    list.Add(ival, Enum.GetName(typeof(UpdateFrequency), ival)); 
                    break;
	        }
        } 

         return list;
    }

}