using Microsoft.AspNetCore.DataProtection.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GC.WebClock.Models
{
    public class DataProtectionKeyRepository : IXmlRepository
    {
        private readonly GC_WebClockContext _db;

        public DataProtectionKeyRepository(GC_WebClockContext db)
        {
            _db = db;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return new ReadOnlyCollection<XElement>(_db.DataProtectionKeys.Select(k => XElement.Parse(k.XmlData)).ToList());
        }

        public void StoreElement(XElement element, string AuthKey)
        {
            var entity = _db.DataProtectionKeys.SingleOrDefault(k => k.AuthKey == AuthKey);
            if (null != entity)
            {
                entity.XmlData = element.ToString();
                _db.DataProtectionKeys.Update(entity);
            }
            else
            {
                _db.DataProtectionKeys.Add(new DataProtectionKey
                {
                    AuthKey = AuthKey,
                    XmlData = element.ToString()
                });
            }
            _db.SaveChanges();
        }
    }
}
