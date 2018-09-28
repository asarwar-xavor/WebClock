using GC.WebClock.Models;
using GC.WebClock.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace GC.WebClock.Services
{
    public class DBService
    {
        GC_WebClockContext dbContext;

        public DBService(GC_WebClockContext context)
        {
            dbContext = context;
        }

        public dynamic GetConfigurationProperties(string key)
        {
            try
            {
                var result = dbContext.ConfigurationProperties.First(p => p.IsActive && p.PropertyName.ToUpper().Trim() == key.ToUpper().Trim());
                if (result != null)
                {
                    return result.PropertyValue.Trim();
                }
                return null;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return null;
            }

        }      
        
        public dynamic GetLocationsWithClocks(ADLocationsModel storeLocations)
        {
            try
            {                
                var result = dbContext.ClocksList.Where(loc => loc.IsActive).OrderBy(loc => loc.Location.Length).ThenBy(loc => loc.Location).Select(x => new ClocksListDTO {
                    Value = x.Location,                   
                    Location = x.Location
                }).ToArray();
                //creating dicrionary to improve performance of location filling
                var locDic = storeLocations.Locations.ToDictionary(p => p.Location.TrimStart('0'), q => q.Label);
                //Filling location name
                foreach (var locs in result)
                {
                    if (locDic.ContainsKey(locs.Location.TrimStart('0')))
                        locs.Label = locDic[locs.Location.TrimStart('0')];
                    else
                        locs.Label = locs.Location;
                }

                if (result != null)
                {
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return null;
            }

        }
        public bool ValidateClock(string locationId)
        {
            try
            {
                int loc = 0;
                bool isLocation = int.TryParse(locationId, out loc);
                List<ClocksList> result ;
                if(isLocation)
                     result = dbContext.ClocksList.Where(cl => cl.IsActive && cl.Location.Trim().ToUpper().TrimStart('0') == locationId.Trim().ToUpper().TrimStart('0')).ToList();
                else
                     result = dbContext.ClocksList.Where(cl => cl.IsActive && cl.ClockName.Trim().ToUpper() == locationId.Trim().ToUpper()).ToList();
                if (result != null && result.Count() > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return false;
            }
        }
        public string GetClockNameFromLocationId(string LocationId)
        {
            try
            {
                var result = dbContext.ClocksList.Where(cl => cl.IsActive && cl.Location.Trim().ToUpper().TrimStart('0') == LocationId.Trim().ToUpper().TrimStart('0')).ToList();
                if (result != null && result.Count() > 0)
                {
                    return result[0].ClockName;
                }
                return "";
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return "";
            }
        }
        public string GetEncodedSecurityCode(string LocationId)
        {
            try
            {
                var result = dbContext.ClocksList.Where(cl => cl.IsActive && cl.Location.Trim().ToUpper().TrimStart('0') == LocationId.Trim().ToUpper().TrimStart('0')).ToList();
                if (result != null && result.Count() > 0)
                {
                    return result[0].EncodedSecurityCode;
                }
                return "";
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return "";
            }
        }
       
        public dynamic GetLocationsFromClocksList(ADLocationsModel storeLocations)
        {
            try
            {                             
                
                var result = dbContext.ClocksList.Where(cl => cl.IsActive).OrderBy(loc => loc.Location.Length).ThenBy(loc => loc.Location).ToList();
                
                //creating dicrionary to improve performance of location filling
                var locDic = storeLocations.Locations.ToDictionary(p => p.Location.TrimStart('0'), q => q.Label);

                //Filling location name
                foreach (var locs in result)
                {
                    locs.SecurityCode = Util.DecodeSecurityCode(locs.EncodedSecurityCode);
                    if (locDic.ContainsKey(locs.Location.TrimStart('0')))
                        locs.LocationName = locDic[locs.Location.TrimStart('0')];
                    else
                        locs.LocationName = locs.Location;
                }
                if (result != null && result.Count() > 0)
                {
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return null;
            }
        }
        public dynamic DeleteClock(int clockId)
        {
            try
            {
                var clockObj = dbContext.ClocksList.First(cl => cl.ClockId == clockId);
                var result = dbContext.ClocksList.Remove(clockObj);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return false;
            }
        }
        public dynamic UpdateClock(int clockId, string location, string clockName,string encodedSecurityCode)
        {
            try
            {
                var clockObj = dbContext.ClocksList.First(cl => cl.ClockId == clockId);
                clockObj.Location = String.IsNullOrEmpty(location) ? clockObj.Location : location.Trim();
                clockObj.ClockName = String.IsNullOrEmpty(clockName) ? clockObj.ClockName : clockName.Trim();
                clockObj.EncodedSecurityCode = String.IsNullOrEmpty(encodedSecurityCode) ? clockObj.EncodedSecurityCode : encodedSecurityCode.Trim();
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return false;
            }
        }
        public dynamic AddClock(string location, string clockName,string locationName, string encodedSecurityCode,string clockType)
        {
            try
            {
                List<ClocksList> checkLocation = dbContext.ClocksList.Where(cl => cl.Location.Trim().TrimStart('0') == location.Trim().TrimStart('0')).ToList();
                if (checkLocation.Count() > 0)
                    return false;
                            
                ClocksList addObj = new ClocksList();
                addObj.Location = location.Trim();
                addObj.ClockName = clockName.Trim();
                addObj.EncodedSecurityCode = encodedSecurityCode.Trim();               
                addObj.ClockType = clockType.Trim();
                dbContext.ClocksList.Add(addObj);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                CustomLogging.ErrorLog(ex);
                return false;
            }
        }       
    }
}
