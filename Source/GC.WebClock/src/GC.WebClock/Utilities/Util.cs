
using GC.WebClock.Models;
using GC.WebClock.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GC.WebClock.Utilities
{
    public class Util
    {                      
        public static void ApplyCredentials(string userName, string password, ClientCredentials clientCredentials)
        {
            clientCredentials.UserName.UserName = userName;
            clientCredentials.UserName.Password = password;
            clientCredentials.Windows.ClientCredential.UserName = userName;
            clientCredentials.Windows.ClientCredential.Password = password;
            clientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
        }
        public static XmlDocument ConvertXElementToXmlDocument(XElement element)
        {
            XmlDocument info = new XmlDocument();
            using (XmlReader xmlReader = element.CreateReader())
            {
                info.Load(xmlReader);
            }
            return info;
        }

        public static T ConvertXmlNodeToObject<T>(XmlNode node) where T : class
        {
            MemoryStream stm = new MemoryStream();

            StreamWriter stw = new StreamWriter(stm);
            stw.Write(node.OuterXml);
            stw.Flush();

            stm.Position = 0;

            XmlSerializer ser = new XmlSerializer(typeof(T));
            T result = (ser.Deserialize(stm) as T);

            return result;
        }
        public static string ConvertToBase64(string plainText)
        {
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }catch(Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return "";
            }
          
        }
        public static string ConvertFromBase64(string encodedText)
        {
            try
            {
                byte[] data = System.Convert.FromBase64String(encodedText);                
                return System.Text.Encoding.UTF8.GetString(data);
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return "";
            }

        }
        public static string GetEncodedSecurityCode(string securityCode)
        {          
            try
            {
                string base64String = ConvertToBase64(securityCode);
                return base64String.Replace("=", "%3D");
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return "";
            }
        }
          
        public static string DecodeSecurityCode(string encodedCode)
        {
            try
            {

                string tempString = encodedCode.Replace("%3D", "=");
                string securityCode = ConvertFromBase64(tempString);
                return securityCode;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return "";
            }
        }       

        public static string GetSilverLightURL(string deviceName, string encodedSC, string baseURL, string urlParams)
        {
            try
            {
                var urlParamsArray = urlParams.Split(',');
                baseURL = baseURL + "?";
                baseURL = baseURL + urlParamsArray[0] + "=" + deviceName + "&" + urlParamsArray[1] + "=" + encodedSC;
                return baseURL;

            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return "";
            }
        }
        public static string GetHTMLClocktURL(string deviceName, string encodedSC, string baseURL, string urlParams, string nsValue)
        {
            try
            {
                var urlParamsArray = urlParams.Split(',');
                baseURL = baseURL + "?";
                baseURL = baseURL + urlParamsArray[0]+"=" + nsValue+"&" + urlParamsArray[1]+"=" + deviceName + "&" + urlParamsArray[2] + "=" + encodedSC;
                return baseURL;

            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return "";
            }
        }

        public static string GetStoreFromUserAgent(string userAgent)
        {
            try
            {
                var storeNumber = "";
                //userAgent = "Mozilla/5.0+(Windows+NT+5.1)+Chrome/44.0.2403.157+(BRAND=GC;+STORE=0) 301 0 0 374";
                //Get store number on basis of index
                var storeIndex = userAgent.IndexOf("STORE=", StringComparison.CurrentCultureIgnoreCase) + 6;
                while(Char.IsDigit(userAgent[storeIndex]))
                {
                    storeNumber = storeNumber + userAgent[storeIndex];
                    storeIndex++;
                }                
                return storeNumber;
            }catch(Exception ex)
            {
                return "";
            }
            
        }
        public static string GetDomainNameFromUsername(string username)
        {
            var domain = "";
            const string slash = @"\";
            var slashIndex = username.IndexOf(slash, StringComparison.Ordinal);

            if (slashIndex <= 0) return domain;
            domain = username.Substring(0, slashIndex + 1);
            domain = domain.Replace("\\", "");

            return domain;
        }
        public static string GetUsernameWithoutDomain(string username)
        {
            var domain = "";
            const string slash = @"\";
            var strUserName = username;
            var slashIndex = username.IndexOf(slash, StringComparison.Ordinal);

            if (slashIndex > 0)
            {
                domain = username.Substring(0, slashIndex + 1);
            }
            if (!string.IsNullOrEmpty(domain))
            {
                strUserName = strUserName.Replace(domain, "");
            }
            return strUserName;
        }

        public static string GetClockPrefix(string clockName)
        {
            try
            {
                clockName = clockName.ToUpper();
                if (clockName[0] == 'T')
                    return clockName.Substring(0, 2);
                else if (clockName[0] == 'M')
                    return clockName.Substring(0, 3);
                else
                    return clockName;                
            }catch(Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return "";
            }                       
        }
        public static readonly Random Random = new Random();
        public static string GenerateDynamicToken(int length)
        {
            if (length.Equals(16))
            {
                return RandomString(length, "0123456789");
            }
            else if (length.Equals(32))
            {
                return RandomString(length, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            }
            else
            {
                return RandomString(length, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            }
        }
        public static string RandomString(int length, string chars)
        {
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }       

    }
}
