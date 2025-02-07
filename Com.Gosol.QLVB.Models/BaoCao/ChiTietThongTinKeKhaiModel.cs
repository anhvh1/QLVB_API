using Com.Gosol.QLVB.Models.HeThong;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class ChiTietThongTinKeKhaiModel
    {
        public HeThongCanBoModel BanThan { get; set; }
        public KeKhaiThanNhanModel VoChong { get; set; }
        public List<KeKhaiThanNhanModel> ConChuaThanhNien { get; set; }
        public List<ThongTinTaiSanModelPartial> ThongTinTaiSan { get; set; }
        public List<ThongTinTaiSanModelPartial> BienDongTaiSan { get; set; }
        public List<TaiSanLogModel> DanhSachThongTinTaiSanLog { get; set; }
        public List<FileLogModel> DanhSachFileDinhKemLog { get; set; }
        public List<KeKhaiModel> DanhSachBanKeKhai { get; set; }
        public int LoaiDotKeKhai { get; set; }
        public int NamKeKhai { get; set; }
        public int KeKhaiID { get; set; }
        public string Barcode { get; set; }
        public string Url { get; set; }
        public string UrlPdf { get; set; }
        public ChiTietThongTinKeKhaiModel()
        {

        }

    }

}
