using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Models.QLVB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.DanhMuc
{
    public class DuLieuDiemThiModel
    {
        public ThongTinToChucThi ThongTinToChucThi { get; set; }
        public List<ThongTinThiSinh> ThongTinThiSinh { get; set; }
        public List<ThongTinThiSinh> ListThiSinhDelete { get; set; }
        public MauPhieuModel ChiTietMauPhieu { get; set; }
        public List<ThiSinhLog> DanhSachChinhSua { get; set; }
        public int? CoQuanID { get; set; }
        public int? DuLieuCuaNam { get; set; }
        public List<FileDinhKemModel> DSFileDinhKem { get; set; }
        public List<FileDinhKemModel> DSXoaFileDinhKem { get; set; }
        public List<ErrorThongTinThiSinh> ListErrorThiSinh { get; set; }
    }

    public class ThongTinThiSinh
    {  
        public int ThiSinhID { get; set; }
        public int? KyThiID { get; set; }
        public int? MauPhieuID { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string NgaySinhStr { get; set; }
        public string NoiSinh { get; set; }
        public Boolean? GioiTinh { get; set; }
        public int? DanToc { get; set; }
        public string TenDanToc { get; set; }
        public string CMND { get; set; }
        public string SoBaoDanh { get; set; }
        public string STT { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Lop { get; set; }
        public int? TruongTHPT { get; set; }
        public string TenTruongTHPT { get; set; }
        public string LoaiDuThi { get; set; }
        public string DonViDKDT { get; set; }
        public string LaoDong { get; set; }
        public string VanHoa { get; set; }
        public string RLTT { get; set; }
        public int? XepLoaiHanhKiem { get; set; }
        public string XepLoaiHanhKiemStr { get; set; }
        public string Do { get; set; }
        public string DoThem { get; set; }
        public string Hong { get; set; }
        public string GhiChu { get; set; }
        public DateTime? KhoaThiNgay { get; set; }
        public int? XepLoaiHocLuc { get; set; }
        public string XepLoaiHocLucStr { get; set; }
        public decimal? DiemTBLop12 { get; set; }
        public decimal? DiemXL { get; set; }
        public decimal? DiemUT { get; set; }
        public decimal? DiemKK { get; set; }
        public int? DienXTN { get; set; }
        public int? HoiDongThi { get; set; }
        public string TenHoiDongThi { get; set; }
        public decimal? DiemXetTotNghiep { get; set; }
        public string KetQuaTotNghiep { get; set; }
        public string SoHieuBang { get; set; }
        public string VaoSoCapBangSo { get; set; }
        public DateTime? NgayCapBang { get; set; }
        public int? NamThi { get; set; }
        public int? TrangThai { get; set; }
        public int? TrangThaiCapBang { get; set; }
        public decimal? TongSoDiemThi { get; set; }
        public string Hang { get; set; }
        public decimal? DiemTBCacBaiThi { get; set; }
        public string? DienUuTien { get; set; }
        public decimal? DiemTBC { get; set; }
        public string QueQuan { get; set; }
        public string ChungNhanNghe { get; set; }
        public string DTConLietSi { get; set; }
        public string GiaiTDKT { get; set; }
        public string DanhSachLoi { get; set; }
        public string HoiDong { get; set; }
        public string MonKN { get; set; }
        public string TBCNMonKN { get; set; }
        public string DiemThiCu { get; set; }
        public string DiemThiMoi { get; set; }
        public string TongBQ { get; set; }
        public string BQA { get; set; }
        public string BQT { get; set; }
        public string DC { get; set; }
        public string Ban { get; set; }

        public string BODY_DAODUC { get; set; }
        public string BODY_RLEV { get; set; }
        public string BODY_DIENKK { get; set; }
        public string BODY_PHONGTHI { get; set; }
        public string BODY_DIEMTNC { get; set; }
        public string BODY_XLTNC { get; set; }
        public string BODY_TDTCU { get; set; }
        public string BODY_KETLUAN { get; set; }
        public string BODY_DIEMTHICUCHA { get; set; }
        public string BODY_GIAIHSG { get; set; }
        public string BODY_GIAIHSGK { get; set; }
        public string BODY_CHUNGCHINN { get; set; }
        public string BODY_CHUNGCHITH { get; set; }
        public string BODY_TONGDIEMMOI { get; set; }
        public string BODY_BQAMOI { get; set; }
        public string BODY_BQTMOI { get; set; }
        public string BODY_SOCAPGIAYCN { get; set; }
        public string BODY_XLHT { get; set; }

        public string BODY_TRANG { get; set; }
        public string BODY_QUYEN { get; set; }
        public string BODY_QUOCGIA { get; set; }

        public string SoQuyen { get; set; }
        public int? SoTrang { get; set; }
        public int? NamTotNghiep { get; set; }
        public int? Maped { get; set; }
        public int? ThiSinhMaped { get; set; }
        public int? ThongTinThiSinhID { get; set; }
        public int? TrangThaiMap { get; set; }
        public int? ThiSinhTrung1 { get; set; }
        public int? Index { get; set; }
        public int? NgaySinh_Int { get; set; }
        public ThongTinThiSinh ThiSinhTrung { get; set; }
        public List<ThongTinDiemThi> ListThongTinDiemThi { get; set; }
        public List<FileDinhKemModel> DSFileDinhKem { get; set; }
        public List<ThongTinChinhSua> ThongTinChinhSua { get; set; }
    }

    public class ThongTinThiSinhLog
    {
        public int ThiSinhLogID { get; set; }
        public int ThiSinhID { get; set; }
        public int? KyThiCuID { get; set; }
        public string HoTenCu { get; set; }
        public DateTime? NgaySinhCu { get; set; }
        public string NoiSinhCu { get; set; }
        public Boolean? GioiTinhCu { get; set; }
        public int? DanTocCu { get; set; }
        public string? DanTocStrCu { get; set; }
        public string CMNDCu { get; set; }
        public string SoBaoDanhCu { get; set; }
        public string STTCu { get; set; }
        public string SoDienThoaiCu { get; set; }
        public string DiaChiCu { get; set; }
        public string LopCu { get; set; }
        public int? TruongTHPTCu { get; set; }
        public string TenTruongTHPTCu { get; set; }
        public string LoaiDuThiCu { get; set; }
        public string DonViDKDTCu { get; set; }
        public string LaoDongCu { get; set; }
        public string VanHoaCu { get; set; }
        public string RLTTCu { get; set; }
        public int? XepLoaiHanhKiemCu { get; set; }
        public string XepLoaiHanhKiemStrCu { get; set; }
        public string DoCu { get; set; }
        public string DoThemCu { get; set; }
        public string HongCu { get; set; }
        public string GhiChuCu { get; set; }
        public int? XepLoaiHocLucCu { get; set; }
        public string XepLoaiHocLucStrCu { get; set; }
        public decimal? DiemTBLop12Cu { get; set; }
        public decimal? DiemXLCu { get; set; }
        public decimal? DiemUTCu { get; set; }
        public decimal? DiemKKCu { get; set; }
        public int? DienXTNCu { get; set; }
        public int? HoiDongThiCu { get; set; }
        public string? HoiDongThiStrCu { get; set; }
        public decimal? DiemXetTotNghiepCu { get; set; }
        public string KetQuaTotNghiepCu { get; set; }
        public string SoHieuBangCu { get; set; }
        public string VaoSoCapBangSoCu { get; set; }
        public DateTime? NgayCapBangCu { get; set; }
        public int? NamThiCu { get; set; }
        public int? TrangThaiCu { get; set; }
        public int? TrangThaiCapBangCu { get; set; }
        public decimal? TongSoDiemThiCu { get; set; }
        public string HangCu { get; set; }
        public decimal? DiemTBCacBaiThiCu { get; set; }
        public string? DienUuTienCu { get; set; }
        public decimal? DiemTBCCu { get; set; }
        public int? KyThiMoiID { get; set; }
        public string HoTenMoi { get; set; }
        public DateTime? NgaySinhMoi { get; set; }
        public string NoiSinhMoi { get; set; }
        public Boolean? GioiTinhMoi { get; set; }
        public int? DanTocMoi { get; set; }
        public string? DanTocStrMoi { get; set; }
        public string CMNDMoi { get; set; }
        public string SoBaoDanhMoi { get; set; }
        public string STTMoi { get; set; }
        public string SoDienThoaiMoi { get; set; }
        public string DiaChiMoi { get; set; }
        public string LopMoi { get; set; }
        public int? TruongTHPTMoi { get; set; }
        public string TenTruongTHPTMoi { get; set; }
        public string LoaiDuThiMoi { get; set; }
        public string DonViDKDTMoi { get; set; }
        public string LaoDongMoi { get; set; }
        public string VanHoaMoi { get; set; }
        public string RLTTMoi { get; set; }
        public int? XepLoaiHanhKiemMoi { get; set; }
        public string XepLoaiHanhKiemStrMoi { get; set; }
        public string DoMoi { get; set; }
        public string DoThemMoi { get; set; }
        public string HongMoi { get; set; }
        public string GhiChuMoi { get; set; }
        public int? XepLoaiHocLucMoi { get; set; }
        public string XepLoaiHocLucStrMoi { get; set; }
        public decimal? DiemTBLop12Moi { get; set; }
        public decimal? DiemXLMoi { get; set; }
        public decimal? DiemUTMoi { get; set; }
        public decimal? DiemKKMoi { get; set; }
        public int? DienXTNMoi { get; set; }
        public int? HoiDongThiMoi { get; set; }
        public string? HoiDongThiStrMoi { get; set; }
        public decimal? DiemXetTotNghiepMoi { get; set; }
        public string KetQuaTotNghiepMoi { get; set; }
        public string SoHieuBangMoi { get; set; }
        public string VaoSoCapBangSoMoi { get; set; }
        public DateTime? NgayCapBangMoi { get; set; }
        public int? NamThiMoi { get; set; }
        public int? TrangThaiMoi { get; set; }
        public int? TrangThaiCapBangMoi { get; set; }
        public decimal? TongSoDiemThiMoi { get; set; }
        public string HangMoi { get; set; }
        public decimal? DiemTBCacBaiThiMoi { get; set; }
        public string? DienUuTienMoi { get; set; }
        public decimal? DiemTBCMoi { get; set; }

        public string QueQuanCu { get; set; }
        public string QueQuanMoi { get; set; }
        public string ChungNhanNgheCu { get; set; }
        public string ChungNhanNgheMoi { get; set; }
        public string DTConLietSiCu { get; set; }
        public string DTConLietSiMoi { get; set; }
        public string GiaiTDKTCu { get; set; }
        public string GiaiTDKTMoi { get; set; }

        public string HoiDongCu { get; set; }
        public string HoiDongMoi { get; set; }
        public string MonKNCu { get; set; }
        public string MonKNMoi { get; set; }
        public string TBCNMonKNCu { get; set; }
        public string TBCNMonKNMoi { get; set; }
        public string DiemThiCuCu { get; set; }
        public string DiemThiCuMoi { get; set; }
        public string DiemThiMoiCu { get; set; }
        public string DiemThiMoiMoi { get; set; }
        public string TongBQCu { get; set; }
        public string TongBQMoi { get; set; }

        public string BQACu { get; set; }
        public string BQAMoi { get; set; }
        public string BQTCu { get; set; }
        public string BQTMoi { get; set; }
        public string DCCu { get; set; }
        public string DCMoi { get; set; }
        public string BanCu { get; set; }
        public string BanMoi { get; set; }
        public string NgaySinhStrCu { get; set; }
        public string NgaySinhStrMoi { get; set; }
        public string BODY_QUOCGIACu { get; set; }
        public string BODY_QUOCGIAMoi { get; set; }

        public List<ThongTinDiemThi> ListThongTinDiemThi { get; set; }
        public List<ThongTinDiemThiLog> ListThongTinDiemThiMoi { get; set; }

        public int? ThaoTac { get; set; }
        public int? CanBoID { get; set; }
        public string? TenCanBo { get; set; }
        public DateTime? NgayChinhSua { get; set; }
    }

    public class ThongTinToChucThi
    {
        public int KyThiID { get; set; }
        public string TenKyThi { get; set; }
        public int? HoiDongThiID { get; set; }
        public int? HoiDongChamThiID { get; set; }
        public string TenHoiDongChamThi { get; set; }
        public int? HoiDongGiamThiID { get; set; }
        public string TenHoiDongGiamThi { get; set; }
        public int? HoiDongGiamKhaoID { get; set; }
        public string TenHoiDongGiamKhao { get; set; }
        public int? HoiDongCoiThiID { get; set; }
        public string TenHoiDongCoiThi { get; set; }
        public string TenHoiDongThi { get; set; }
        public int? KhoaThiID { get; set; }
        public string TenKhoaThi { get; set; }
        public DateTime? KhoaThiNgay { get; set; }
        public string SBDDau { get; set; }
        public string SBDCuoi { get; set; }
        public string NguoiDocDiem { get; set; }
        public string NguoiNhapVaInDiem { get; set; }
        public string NguoiDocSoatBanGhi { get; set; }
        public DateTime? NgayDuyetBangDiem { get; set; }
        public DateTime? NgayDuyetCham { get; set; }
        public DateTime? NgaySoDuyet { get; set; }
        public string CanBoXetDuyet { get; set; }
        public string CanBoSoKT { get; set; }
        public string ChuTichHoiDongChamThi { get; set; }
        public string GiamDocSo { get; set; }
        public int? SoThiSinhDuThi { get; set; }
        public int? DuocCongNhanTotNghiep { get; set; }
        public int? KhongDuocCongNhanTotNghiep { get; set; }
        public int? TNLoaiGioi { get; set; }
        public int? TNLoaiKha { get; set; }
        public int? TNLoaiTB { get; set; }
        public int? DienTotNghiep2 { get; set; }
        public int? DienTotNghiep3 { get; set; }
        public int? TotNghiepDienA { get; set; }
        public int? TotNghiepDienB { get; set; }
        public int? TotNghiepDienC { get; set; }
        public int? DienTotNghiep4_5 { get; set; }
        public int? DienTotNghiep4_75 { get; set; }
        public int? TrangThai { get; set; }
        public int? TrangThaiKhoa { get; set; }
        public string GhiChu { get; set; }
        public string PhongThi { get; set; }
        public string Ban { get; set; }
        public int? MauPhieuID { get; set; }
        public string ThuKy { get; set; }
        public string ChanhChuKhao { get; set; }
        public string PhoChuKhao { get; set; }
        public string SoQuyen { get; set; }
        public int? SoTrang { get; set; }
        public int? TongSoThiSinh { get; set; }
        public string Tinh { get; set; }
        public string ToTruongHoiPhach { get; set; }
        public string ChuTichHoiDongCoiThi { get; set; }
        public string ChuTichHoiDong { get; set; }
        public string HieuTruong { get; set; }
        public int? Nam { get; set; }
        public string DiaDanh { get; set; }
        //public string QueQuan { get; set; }
        public string GhiChuCuoiTrang { get; set; }
        public string SBDDau_CuoiTrang { get; set; }
        public string SBDCuoi_CuoiTrang { get; set; }
        public int? TSDoThang { get; set; }
        public int? TSDoThem { get; set; }
        public int? TSThiHong { get; set; }
        public string PGiamDoc { get; set; }
        public string NguoiKiemTra { get; set; }


        public string FOOT_RPDD { get; set; }
        public string FOOT_KTD { get; set; }
        public string FOOT_DTBDIS { get; set; }
        public string FOOT_THUKY { get; set; }
        public string FOOT_GIAMSAT { get; set; }
        public string FOOT_So_DCNTN { get; set; }
        public string FOOT_So_Dien45 { get; set; }
        public string FOOT_So_Dien475 { get; set; }
        public string FOOT_So_Loai_Gioi { get; set; }
        public string FOOT_So_Loai_Kha { get; set; }
        public string FOOT_So_Loai_TB { get; set; }
        public string FOOT_CONGTHEM1DIEM { get; set; }
        public string FOOT_CONGTHEM15DIEM { get; set; }
        public string FOOT_CONGTHEM2DIEM { get; set; }
        public string FOOT_CONGTHEMTREN2DIEM { get; set; }
        public string FOOT_VANGMATKHITHI { get; set; }
        public string FOOT_VIPHAMQUYCHETHI { get; set; }
        public string FOOT_HSDIENUUTIEN { get; set; }
        public string FOOT_HSCOCHUNGNHANNGHE { get; set; }
        public string FOOT_NGUOIKIEMTRAHS { get; set; }
        public string FOOT_SKĐCNTN { get; set; }
        public string FOOT_STSDT { get; set; }
        public string FOOT_SSTSDT { get; set; }
        public string HEAD_HDCT { get; set; }
        public string FOOT_TND_D { get; set; }
        public string FOOT_TND_E { get; set; }
        public string FOOT_LTHUONG { get; set; }
        public string FOOT_SLTHUONG { get; set; }
        public string FOOT_HSCONLIETSI { get; set; }
        public string FOOT_HSCACDIENKHAC { get; set; }
        public string FOOT_NGUOILAPBANG { get; set; }
        public string FOOT_NXNLAPBANG { get; set; }
        public string FOOT_NXNHOIDONGCOITHI { get; set; }
        public string FOOT_NXNCHAMTHIXTN { get; set; }
        public string FOOT_VTVGDTX { get; set; }
        public string HEAD_TRUONG { get; set; }
        public string FOOT_CTHDPHUCKHAO { get; set; }
        public string HEAD_HDCL { get; set; }
        public string TenNguoiNhap { get; set; }
        public int? NguoiNhapID { get; set; }
        public int? Type { get; set; }

    }

    public class ThongTinDiemThi
    {
        public int DiemThiID { get; set; }
        public int ThiSinhID { get; set; }
        public int? MonThiID { get; set; }
        public decimal? Diem { get; set; }
        public string DiemBaiToHop { get; set; }
        public int? NhomID { get; set; }
        public string TenMonThi { get; set; }
        public string TenNhom { get; set; }
    }

    public class ThongTinDiemThiLog
    {
        public int DiemThiID { get; set; }
        public int ThiSinhID { get; set; }
        public int? MonThiID { get; set; }
        public string TenMonThi { get; set; }
        public decimal? DiemCu { get; set; }
        public string DiemBaiToHopCu { get; set; }
        public decimal? DiemMoi { get; set; }
        public string DiemBaiToHopMoi { get; set; }
        public int? NhomID { get; set; }
        public DateTime? NgayChinhSua { get; set; }
        public int? ThaoTac { get; set; }
    }

    public class ChiTietDuLieuDiemThiModel : ThongTinToChucThi
    {
        public int ThiSinhID { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string NoiSinh { get; set; }
        public Boolean? GioiTinh { get; set; }
        public int? DanToc { get; set; }
        public string CMND { get; set; }
        public string SoBaoDanh { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Lop { get; set; }
        public int? TruongTHPT { get; set; }
        public string TenTruongTHPT { get; set; }
        public string LoaiDuThi { get; set; }
        public string DonViDKDT { get; set; }
        public string LaoDong { get; set; }
        public string VanHoa { get; set; }
        public string RLTT { get; set; }
        public int? XepLoaiHanhKiem { get; set; }
        public string XepLoaiHanhKiemStr { get; set; }
        public string Do { get; set; }
        public string DoThem { get; set; }
        public string Hong { get; set; }
        public int? XepLoaiHocLuc { get; set; }
        public string XepLoaiHocLucStr { get; set; }
        public decimal? DiemTBLop12 { get; set; }
        public decimal? DiemXL { get; set; }
        public decimal? DiemUT { get; set; }
        public decimal? DiemKK { get; set; }
        public int? DienXTN { get; set; }
        public int? HoiDongThi { get; set; }
        public decimal? DiemXetTotNghiep { get; set; }
        public string KetQuaTotNghiep { get; set; }
        public string SoHieuBang { get; set; }
        public string VaoSoCapBangSo { get; set; }
        public DateTime? NgayCapBang { get; set; }
        public int? NamThi { get; set; }
        public int? TrangThaiThiSinh { get; set; }
        public int? TrangThaiKyThi { get; set; }
        public int? TrangThaiCapBang { get; set; }
        public decimal? TongSoDiemThi { get; set; }
        public string GhiChuKyThi { get; set; }
        public string GhiChuThiSinh { get; set; }
        public string Hang { get; set; }
        public decimal? DiemTBCacBaiThi { get; set; }
        public string? DienUuTien { get; set; }
        public decimal? DiemTBC { get; set; }
        public List<ThongTinDiemThi> ListThongTinDiemThi { get; set; }
        public int DiemThiID { get; set; }
        public int? MonThiID { get; set; }
        public decimal? Diem { get; set; }
        public string DiemBaiToHop { get; set; }
        public int? NhomID { get; set; }
    }

    public class ThiSinhLogModel
    {
        public DateTime? NgayChinhSua { get; set; }
        public string? NguoiChinhSua { get; set; }
        public int? ThaoTac { get; set; }
        public string NoiDungChinhSua { get; set; }
        public string HoTenThiSinh { get; set; }
        public int? ThiSinhID { get; set; }

        public List<ThiSinhChinhSuaModel> DanhSachThiSinhChinhSua { get; set; }
        //public List<ThongTinChinhSuaModel> DanhSachChinhSua { get; set; }

    }

    public class ThiSinhLog
    {
        public DateTime? NgayChinhSua { get; set; }
        public string? NguoiChinhSua { get; set; }
        public int? ThaoTac { get; set; }
        public string NoiDungChinhSua { get; set; }
        public string HoTenThiSinh { get; set; }
        public int? ThiSinhID { get; set; }
        public string SBD { get; set; }
    }

    public class ThiSinhChinhSuaModel
    {
        public int? ThiSinhID { get; set; }
        public string? TenThiSinh { get; set; }
        public List<CacTruongSua> DanhSachChinhSua { get; set; }
    }

    public class CacTruongSua
    {
        public string Cu { get; set; }
        public string Moi { get; set; }
        public string ThongTinChinhSua { get; set; }
    }

    public class NamThiTree
    {
        public string Name { get; set; }
        public int? NamThi { get; set; }
        public string SoQuyen { get; set; }
        public int? SoTrang { get; set; }
        public int? TongSoThiSinh { get; set; }
        public int? Type { get; set; }
        public List<NamThiTree> children { get; set; }
    }

    public class ErrorThongTinThiSinh
    {
        public string MaCot { get; set; }
        public int Index { get; set; }
        public string DanhSachLoi { get; set; }
        public bool? isError { get; set; }
        public ErrorThongTinThiSinh(string MaCot, int Index, string DanhSachLoi, bool? isError)
        {
            this.MaCot = MaCot;
            this.Index = Index; 
            this.DanhSachLoi = DanhSachLoi;
            this.isError = isError;
        }
    }
}
