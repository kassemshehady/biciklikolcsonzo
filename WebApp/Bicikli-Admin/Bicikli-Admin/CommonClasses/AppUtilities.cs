using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

public class AppUtilities
{
    /*
     * Resolves currently logged in user's role to a friendly name
     * 
     * Throws exception if no user is logged in
     */
    public static string ResolveUserRole()
    {
        return ResolveUserRole(Membership.GetUser().UserName);
    }

    public static string ResolveUserRole(string username)
    {
        if (Roles.IsUserInRole(username, "SiteAdmin"))
        {
            return "Site Adminisztrátor";
        }
        else
        {
            return "Kölcsönző Adminisztrátor";
        }
    }

    /*
     * Returns true if the current user is site administrator, false otherwise
     * 
     * Throws exception if no user is logged in
     */
    public static bool IsSiteAdmin()
    {
        return Roles.IsUserInRole("SiteAdmin");
    }
}