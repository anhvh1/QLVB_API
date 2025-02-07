using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Com.Gosol.QLVB.Models.BaoCao
{
    public class ThongKeBanKeKhaiPartialModel
    {
        public int CoQuanID { get; set; }
        public string TenCoQuan { get; set; }
        public int CanBoID { get; set; }
        public string TenCanBo { get; set; }
        public string MaCanBo { get; set; }
        public DateTime NgaySinh { get; set; }
        public string HoKhau { get; set; }
        public string DiaChi { get; set; }
        public int TrangThai { get; set; }
        public int DotKeKhaiID { get; set; }
        public int NamKeKhai { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string MaCoQuan { get; set; }
        public List<int> DanhSachChucVuID { get; set; }
        public List<string> DanhSachTenChucVu { get; set; }

        public ThongKeBanKeKhaiPartialModel()
        {

        }

    }
    public class BaoCaoBienDongTaiSanModel : ThongKeBanKeKhaiPartialModel
    {
        public double? GiaTri { get; set; }
        public int? KeKhaiID { get; set; }
        public BaoCaoBienDongTaiSanModel()
        {

        }
        public BaoCaoBienDongTaiSanModel(double? GiaTri)
        {
            this.GiaTri = GiaTri;
        }
    }
    public class BaoCaoBienDongTaiSanModelPartial : ThongKeBanKeKhaiPartialModel
    {
        public double? MucBienDong { get; set; }
        public int TuNam { get; set; }
        public int DenNam { get; set; }
        public BaoCaoBienDongTaiSanModelPartial()
        {

        }
        public BaoCaoBienDongTaiSanModelPartial(double MucBienDong, int TuNam, int DenNam)
        {
            this.MucBienDong = MucBienDong;
            this.TuNam = TuNam;
            this.DenNam = DenNam;
        }
    }
    public class ThongKeBanKeKhaiModel
    {
        public int CoQuanID { get; set; }
        public string TenCoQuan { get; set; }

        public List<ThongKeBanKeKhaiPartialModel> DanhSachBanKeKhai { get; set; }
    }
    public class ThongKeChiTietKeKhaiTaiSan
    {
        public int TongSoDotKeKhai { get; set; }
        public int? CoQuanID { get; set; }
        public int? CoQuanChaID { get; set; }
        public string TenCoQuanCha { get; set; }
        //public int? CoQuanChaID { get; set; }
        public int? CapID { get; set; }
        public string TenCoQuan { get; set; }
        public int KeKhaiHangNamDaKeKhai { get; set; }
        public int KeKhaiBoSungDaKeKhai { get; set; }
        public int KeKhaiBoNhiemDaKeKhai { get; set; }
        public int KeKhaiLanDauDaKeKhai { get; set; }
        public int KeKhaiHangNamChuaKeKhai { get; set; }
        public int KeKhaiBoSungChuaKeKhai { get; set; }
        public int KeKhaiBoNhiemChuaKeKhai { get; set; }
        public int KeKhaiLanDauChuaKeKhai { get; set; }
        public int TongSoDaKeKhai { get; set; }
        public int TongSoChuaKeKhai { get; set; }
        public int SLChuaKeKhai { get; set; }
        public int SLDaKeKhai { get; set; }
        public int LoaiDotKeKhai { get; set; }
        public List<ThongKeChiTietKeKhaiTaiSan> Children { get; set; }

        public ThongKeChiTietKeKhaiTaiSan()
        {

        }
        public ThongKeChiTietKeKhaiTaiSan(int TongSoDotKeKhai, int CoQuanID, string TenCoQuan, int KeKhaiHangNamDaKeKhai, int KeKhaiBoSungDaKeKhai, int KeKhaiBoNhiemDaKeKhai, int KeKhaiLanDauDaKeKhai, int? CoQuanChaID)
        {
            this.CoQuanID = CoQuanID;
            this.TenCoQuan = TenCoQuan;
            this.KeKhaiHangNamDaKeKhai = KeKhaiHangNamDaKeKhai;
            this.KeKhaiBoSungDaKeKhai = KeKhaiBoSungDaKeKhai;
            this.KeKhaiBoNhiemDaKeKhai = KeKhaiBoNhiemDaKeKhai;
            this.KeKhaiLanDauDaKeKhai = KeKhaiLanDauDaKeKhai;
            this.TongSoDotKeKhai = TongSoDotKeKhai;
            this.CoQuanChaID = CoQuanChaID;
        }
    }
    public class ThongKeChiTietKeKhaiTaiSanPar : ThongKeChiTietKeKhaiTaiSan
    {

        public List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTiet { get; set; }
        public List<int> ListCapID { get; set; }
        public List<ThongKeChiTietKeKhaiTaiSan> ListThongKeCoQuanCha { get; set; }
        public string TenCapCoQuan { get; set; }
        public int TongKeKhaiLanDauDaKeKhai { get; set; }
        public int TongKeKhaiBoSungDaKeKhai { get; set; }
        public int TongKeKhaiBoNhiemDaKeKhai { get; set; }
        public int TongKeKhaiHangNamDaKeKhai { get; set; }
        public int TongKeKhaiLanDauChuaKeKhai { get; set; }
        public int TongKeKhaiBoSungChuaKeKhai { get; set; }
        public int TongKeKhaiBoNhiemChuaKeKhai { get; set; }
        public int TongKeKhaiHangNamChuaKeKhai { get; set; }
       
        public ThongKeChiTietKeKhaiTaiSanPar()
        {

        }

        public ThongKeChiTietKeKhaiTaiSanPar(int CapID, string TenCapCoQuan, List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChitiet, int TongSoChuaKeKhai, int TongSoDaKeKhai)
        {
            this.CapID = CapID;
            this.TenCapCoQuan = TenCapCoQuan;
            this.ListThongKeChiTiet = ListThongKeChitiet;
            this.TongSoDaKeKhai = TongSoDaKeKhai;
            this.TongSoChuaKeKhai = TongSoChuaKeKhai;
        }
        public ThongKeChiTietKeKhaiTaiSanPar(List<ThongKeChiTietKeKhaiTaiSan> ListThongKeChiTiet, int TongKeKhaiLanDauDaKeKhai,int TongKeKhaiBoSungDaKeKhai ,int TongKeKhaiBoNhiemDaKeKhai ,int TongKeKhaiHangNamDaKeKhai, int TongKeKhaiLanDauChuaKeKhai  ,int TongKeKhaiBoSungChuaKeKhai, int TongKeKhaiBoNhiemChuaKeKhai ,int TongKeKhaiHangNamChuaKeKhai, string TenCapCoQuan
            ,List<ThongKeChiTietKeKhaiTaiSan> ListThongKeCoQuanCha) 
        {
            this.ListThongKeChiTiet = ListThongKeChiTiet;
            this.TongKeKhaiLanDauDaKeKhai = TongKeKhaiLanDauDaKeKhai;
            this.TongKeKhaiBoSungDaKeKhai = TongKeKhaiBoSungDaKeKhai;
            this.TongKeKhaiBoNhiemDaKeKhai = TongKeKhaiBoNhiemDaKeKhai;
            this.TongKeKhaiHangNamDaKeKhai = TongKeKhaiHangNamDaKeKhai;
            this.TongKeKhaiLanDauChuaKeKhai = TongKeKhaiLanDauChuaKeKhai;
            this.TongKeKhaiBoSungChuaKeKhai = TongKeKhaiBoSungChuaKeKhai;
            this.TongKeKhaiBoNhiemChuaKeKhai = TongKeKhaiBoNhiemChuaKeKhai;
            this.TongKeKhaiHangNamChuaKeKhai = TongKeKhaiHangNamChuaKeKhai;
            this.TenCapCoQuan = TenCapCoQuan;
            this.ListThongKeCoQuanCha = ListThongKeCoQuanCha;

        }
    }
    public class ThongKeTaiSanModel
    {
        public List<ThongKeChiTietKeKhaiTaiSanPar> ThongKeTaiSan_Chart { get; set; }
        public ThongKeChiTietKeKhaiTaiSanPar ThongKeTaiSan_Table { get; set; }
    }

    public class ThongTinTongQuanModel
    {
        public int TongCanBoDaKeKhai { get; set; }
        public int TongCanBoDaGuiKeKhai { get; set; }
        public int TongCanBoChuaGuiKeKhai { get; set; }
    }
}
