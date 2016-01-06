using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;

namespace Utilities
{
    public class WebSiteSettings
    {
        public static T GetSettingValue<T>(string key)
        {
            string value = ConfigurationManager.AppSettings[key];

            if (value == null)
            {
                throw new Exception(String.Format("Could not find setting '{0}',", key));
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}
