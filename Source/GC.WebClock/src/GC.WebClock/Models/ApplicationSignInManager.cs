using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Novell.Directory.Ldap;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using GC.WebClock.Utilities;

namespace GC.WebClock.Models
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        IConfiguration _config;
        public ApplicationSignInManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<ApplicationUser>> logger, IConfiguration config) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
            _config = config;
        }
        public override async Task<Microsoft.AspNetCore.Identity.SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            using (var cn = new LdapConnection())
            {
                var hostName = "EMPTY";
                var domain = Util.GetDomainNameFromUsername(userName);
                if (string.IsNullOrEmpty(domain)) { return Microsoft.AspNetCore.Identity.SignInResult.Failed; }
                var hostNames = _config["HostName"].Split(';');

                if (hostNames != null && hostNames.Length > 0)
                {
                    foreach (var name in hostNames)
                    {
                        if (name.Trim().ToLower().Contains(domain.Trim().ToLower()))
                        {
                            hostName = name;
                            break;
                        }
                    }
                }
                else
                    return Microsoft.AspNetCore.Identity.SignInResult.Failed;
               
                try
                {
                    cn.Connect(hostName, Int32.Parse(_config["Port"]));
                    cn.Bind(userName, password);
                    var user = new ApplicationUser { UserName = userName, Email = userName, PasswordHash = "", SecurityStamp = Guid.NewGuid().ToString() };
                    await base.SignInAsync(user, isPersistent);
                    return Microsoft.AspNetCore.Identity.SignInResult.Success;
                }
                catch (Exception e)
                {
                    return Microsoft.AspNetCore.Identity.SignInResult.Failed;
                }
                finally
                {
                    cn.Disconnect();
                }
            }
        }
    }
}
