using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.BaoCao
{
    public class ThongKeGuiThanhTraTinhModel
    {
        public int TongSo { get; set; }
        public int DaKeKhai { get; set; }
        public int ChuaKeKhai { get; set; }
        public int DaGui { get; set; }
        public List<HeThongCanBoThongKeModel> DanhSachCanBoDaKeKhai { get; set; }
        public List<HeThongCanBoThongKeModel> DanhSachCanBoChuaKeKhai { get; set; }
    }

    public class HeThongCanBoThongKeModel 
    {
        public int CanBoID { get; set; }
        public string TenCanBo { get; set; }
        public string TenCoQuan { get; set; }
        public string FileUrl { get; set; }
        public int TrangThaiBanKeKhai { get; set; }
    }

    public class QuanLyBanKeKhaiModel
    {
        public int TongSoCanKeKhai{ get; set; }
        public int DaKeKhai { get; set; }
        public int ChuaKeKhai { get; set; }
        public int ChuaGui { get; set; }
        public int DaGui { get; set; }
        public int DaTiepNhan { get; set; }
        public int ChuaTiepNhan { get; set; }
        public List<KeKhaiModelPartial> DanhSach { get; set; }
        //public List<KeKhaiModelPartial> DanhSachChuaKeKhai { get; set; }
        //public List<KeKhaiModelPartial> DanhSachDaGui { get; set; }
        //public List<KeKhaiModelPartial> DanhSachChuaGui { get; set; }
    }

}
