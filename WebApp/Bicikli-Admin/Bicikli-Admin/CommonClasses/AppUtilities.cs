using System;
using System.Web.Security;

public class AppUtilities
{
    /// <summary>
    /// Resolves currently logged in user's role to a friendly name
    /// </summary>
    /// <returns></returns>
    public static string ResolveUserRole()
    {
        return ResolveUserRole(Membership.GetUser().UserName);
    }

    /// <summary>
    /// Resolves the specified user's role to a friendly name
    /// </summary>
    /// <param name="username">username of user</param>
    /// <returns></returns>
    public static string ResolveUserRole(string username)
    {
        if (Roles.IsUserInRole(username, "SiteAdmin"))
        {
            return FriendlyRoleName("SiteAdmin");
        }
        else
        {
            return FriendlyRoleName();
        }
    }

    /// <summary>
    /// Returns true if the current user is site administrator, false otherwise
    /// </summary>
    /// <returns></returns>
    public static bool IsSiteAdmin()
    {
        return Roles.IsUserInRole("SiteAdmin");
    }

    /// <summary>
    /// Resolves a raw role name to a friendly name
    /// </summary>
    /// <param name="raw_name">Raw name of role (ie: SiteAdmin)</param>
    /// <returns></returns>
    public static string FriendlyRoleName(String raw_name = null)
    {
        if (raw_name == "SiteAdmin")
        {
            return "Site Adminisztrátor";
        }
        else
        {
            return "Kölcsönző Adminisztrátor";
        }
    }
}