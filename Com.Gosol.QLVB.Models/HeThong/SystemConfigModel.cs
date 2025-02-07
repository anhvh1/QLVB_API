using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.Models.HeThong
{
    public class SystemConfigModel
    {
        public int SystemConfigID { get; set; }
        public String ConfigKey { get; set; }
        public String ConfigValue { get; set; }
        public String Description { get; set; }
        public SystemConfigModel()
        {

        }
        public SystemConfigModel(int SystemConfigID, string ConfigKey, string ConfigValue, string Description)
        {
            this.SystemConfigID = SystemConfigID;
            this.ConfigKey = ConfigKey;
            this.ConfigValue = ConfigValue;
            this.Description = Description;
        }
    }
}
