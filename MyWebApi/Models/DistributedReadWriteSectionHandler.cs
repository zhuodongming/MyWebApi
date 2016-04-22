using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;

namespace MyWebApi.Models
{
    public class DistributedReadWriteSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            Dictionary<string, DistributedReadWriteSection> names = new Dictionary<string, DistributedReadWriteSection>();

            string _key = String.Empty;
            string _ip = String.Empty;
            int _port = 1433;
            string _dbName = String.Empty;
            string _userId = String.Empty;
            string _password = String.Empty;


            foreach (XmlNode node in section.ChildNodes)
            {
                if (node.Attributes["key"] != null)
                {
                    _key = node.Attributes["key"].Value;

                    if (node.Attributes["Ip"] != null)
                    {
                        _ip = node.Attributes["Ip"].Value;
                    }
                    if (node.Attributes["Port"] != null)
                    {
                        _port = Convert.ToInt32(node.Attributes["Port"].Value);
                    }
                    if (node.Attributes["DbName"] != null)
                    {
                        _dbName = node.Attributes["DbName"].Value;
                    }
                    if (node.Attributes["UserId"] != null)
                    {
                        _userId = node.Attributes["UserId"].Value;
                    }
                    if (node.Attributes["Password"] != null)
                    {
                        _password = node.Attributes["Password"].Value;
                    }

                    names.Add(_key, new DistributedReadWriteSection { Ip = _ip, Port = _port, DbName = _dbName, UserId = _userId, Password = _password });
                }

            }

            return names;
        }
    }
}