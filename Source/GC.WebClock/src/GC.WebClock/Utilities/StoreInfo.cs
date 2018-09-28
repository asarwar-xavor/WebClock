using GC.WebClock.Models;
using GC.WebClock.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.ServiceModel;
using System.Threading.Tasks;

namespace GC.WebClock.Utilities
{
    public class StoreInfo
    {
        HttpContext _httpContext;
        GC_WebClockContext _context;
        ClaimsPrincipal _user;
        DBService dbService;
        string userInfoXml;

        public StoreInfo(GC_WebClockContext context, HttpContext httpContext, ClaimsPrincipal user)
        {
            _context = context;
            _httpContext = httpContext;
            _user = user;
            dbService = new DBService(_context);
        }

        public dynamic GetAllStatesFromStoreInfo(string isPhysical)
        {
            try
            {
                ADLocationsModel locations = new ADLocationsModel();
                var binding = new BasicHttpBinding();
                binding.Name = "StoreInfoWebServiceSoap";
                binding.MaxReceivedMessageSize = 6553600;
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                var addressUri = dbService.GetConfigurationProperties("StoreInfoServiceUrl");
                if (addressUri.Contains("https"))
                    binding.Security.Mode = BasicHttpSecurityMode.Transport;

                var endpoint = new EndpointAddress(addressUri);
                var client = new StoreServiceReference.RetrieveNameSoapClient(binding, endpoint);

                Util.ApplyCredentials("", "", client.ClientCredentials);
                var element = client.getAllLocationsAsync(isPhysical.ToString()).ConfigureAwait(true).GetAwaiter().GetResult();

                if (element != null)
                {
                    var xmlDocument = Util.ConvertXElementToXmlDocument(element.Nodes[1]);
                    var info = xmlDocument.FirstChild.FirstChild;
                    locations = Util.ConvertXmlNodeToObject<ADLocationsModel>(info);
                }

                return locations;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return new ADLocationsModel();
            }
        }
    }
}
