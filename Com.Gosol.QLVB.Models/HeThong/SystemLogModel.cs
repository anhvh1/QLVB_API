using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.Models.HeThong
{
    public class SystemLogModel
    {
        public int SystemLogid { get; set; }
        public int CanBoID { get; set; }
        public string LogInfo { get; set; }
        public DateTime LogTime { get; set; }
        public int LogType { get; set; }
        public string TenCanBo { get; set; }
    }
    public class SystemLogPartialModel : SystemLogModel
    {
        public string TenCoQuan { get; set; }
    }
}
