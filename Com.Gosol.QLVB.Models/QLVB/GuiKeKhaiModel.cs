using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class GuiKeKhaiModel
    {
        public List<int> DanhSachBanKekhaiID { get; set; }
        public string? SoCongVan { get; set; }
        public int? TiepNhanAll { get; set; }
        public int? FileID { get; set; }
        public int? Nam { get { return DateTime.Now.Year; } }
    }
}
