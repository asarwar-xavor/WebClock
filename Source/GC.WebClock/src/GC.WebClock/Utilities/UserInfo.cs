using GC.WebClock.Models;
using GC.WebClock.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading.Tasks;

namespace GC.WebClock.Utilities
{
    public class UserInfo
    {
        HttpContext _httpContext;
        GC_WebClockContext _context;
        ClaimsPrincipal _user;
        DBService dbService;
        string userInfoXml;

        public UserInfo(GC_WebClockContext context, HttpContext httpContext, ClaimsPrincipal user)
        {
            _context = context;
            _httpContext = httpContext;
            _user = user;
            dbService = new DBService(_context);
        }

        public int GetEmployeeId()
        {
            var employeeId = 0;
            try
            {
                var accountName = @"" + (_user.Identity.Name != null ? _user.Identity.Name : WindowsIdentity.GetCurrent().Name);

                var binding = new BasicHttpBinding();
                binding.Name = "UserInfoWebServiceSoap";
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                var addressUri = dbService.GetConfigurationProperties("UserInfoServiceUrl");

                if (addressUri.Contains("https"))
                    binding.Security.Mode = BasicHttpSecurityMode.Transport;

                var endpoint = new EndpointAddress(addressUri);
                var client = new UserInfoServiceReference.UserInfoWebServiceSoapClient(binding, endpoint);
                Util.ApplyCredentials("", "", client.ClientCredentials);
                var empId = client.getMyEmpIDByAccountNameAsync(accountName).ConfigureAwait(true).GetAwaiter().GetResult();

                int.TryParse(empId, out employeeId);

                if (employeeId < 0)
                    employeeId = 0;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
            }
            return employeeId;
        }

        public UserInfoModel GetUserInfo()
        {
            try
            {
                var accountName = @"" + (_user.Identity.Name != null ? _user.Identity.Name : WindowsIdentity.GetCurrent().Name);
                var employeeId = GetEmployeeId();

                var binding = new BasicHttpBinding();
                binding.Name = "UserInfoWebServiceSoap";
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                var addressUri = dbService.GetConfigurationProperties("UserInfoServiceUrl");

                if (addressUri.Contains("https"))
                    binding.Security.Mode = BasicHttpSecurityMode.Transport;

                var endpoint = new EndpointAddress(addressUri);
                var client = new UserInfoServiceReference.UserInfoWebServiceSoapClient(binding, endpoint);
                Util.ApplyCredentials("", "", client.ClientCredentials);

                var element = client.getUserInfoByEmpIDAsync(employeeId).ConfigureAwait(true).GetAwaiter().GetResult();
                userInfoXml = element.ToString();
                var info = Util.ConvertXElementToXmlDocument(element);
                var userInfo = Util.ConvertXmlNodeToObject<UserInfoModel>(info);

                employeeId = 0;
                var locationId = 0;
                var jobCode = 0;
                var departmentCode = 0;

                int.TryParse(userInfo.empID, out employeeId);
                int.TryParse(userInfo.locationID, out locationId);
                int.TryParse(userInfo.jobCode, out jobCode);
                int.TryParse(userInfo.dept, out departmentCode);

                if (employeeId < 0)
                    employeeId = 0;

                userInfo.empID = employeeId.ToString();
                userInfo.locationID = locationId.ToString();
                userInfo.jobCode = jobCode.ToString();
                userInfo.dept = departmentCode.ToString();

                return userInfo;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return new UserInfoModel();
            }
        }

        public string[] GetWebClockMembership()
        {
            try
            {
                var userName = _user.Identity.Name != null
                            ? Util.GetUsernameWithoutDomain(_user.Identity.Name)
                            : Util.GetUsernameWithoutDomain(System.Security.Principal.WindowsIdentity.GetCurrent().Name);

                var userDomain = _user.Identity.Name != null
                        ? Util.GetDomainNameFromUsername(_user.Identity.Name)
                        : Util.GetDomainNameFromUsername(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                var accountName = @"" + (_user.Identity.Name != null ? _user.Identity.Name : WindowsIdentity.GetCurrent().Name);
                var binding = new BasicHttpBinding();
                binding.Name = "UserInfoWebServiceSoap";
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                var addressUri = dbService.GetConfigurationProperties("UserInfoServiceUrl");

                if (addressUri.Contains("https"))
                    binding.Security.Mode = BasicHttpSecurityMode.Transport;

                var endpoint = new EndpointAddress(addressUri);
                var client = new UserInfoServiceReference.UserInfoWebServiceSoapClient(binding, endpoint);

                Util.ApplyCredentials("", "", client.ClientCredentials);
                var webClockGroups = client.getMembershipBySAMAccountNameAsync(accountName).ConfigureAwait(true).GetAwaiter().GetResult();
                return webClockGroups;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return null;
            }
        }

        public bool isAdmin()
        {            
            var ADGroups = GetWebClockMembership();
            string webClockGroups = dbService.GetConfigurationProperties("WebClockAdminGroups");
            var groupArray = webClockGroups.Split(',');
            foreach(string adGroup in ADGroups)
            {
                foreach(string webClockGroup in groupArray)
                {
                    if (webClockGroup.Trim().ToUpper() == adGroup.Trim().ToUpper())
                        return true;
                }
            }
            return false;
        }
    }
}
