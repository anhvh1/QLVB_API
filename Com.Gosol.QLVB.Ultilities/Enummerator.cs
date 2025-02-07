using System;
using System.Web;

namespace Com.Gosol.QLVB.Ultilities
{
    #region QLVB
    public enum EnumLoaiDanhMuc : Int32
    {
        DM_Truong = 1,
        DM_DanToc = 2,
        DM_MonHoc = 3,
        DM_XepLoai = 4,
        DM_DienXetTotNghiep = 5,
        DM_NguoiDuyetKetQua = 6,
        DM_HoiDong = 7,

        DM_BieuMau_HEAD = 8,
        DM_BieuMau_BODY = 9,
        DM_BieuMau_FOOT = 10,
        DM_HanhKiem = 11,

        Nhom_DiemLop12 = 1,
        Nhom_DiemThi = 2,
        Nhom_DiemPhucKhao = 3,
        Nhom_DiemBaoLuu = 4,
        Nhom_CuoiMau_NhapTay = 5,

    }

    public enum EnumTieuChiThongKe : Int32
    {
        ToanBoTheoNam = 0,
        DienUT = 1,
        LoaiGioi = 2,
        LoaiKha = 3,
        LoaiTB = 4,
        KhongTN = 5,
    }
    public enum EnumMaxLength : Int32
    {
        Text = 200,
        CCCD = 15,
        NgaySinh = 15,
    }
    #endregion
    #region AnhVH
    public enum EnumLogType
    {
        Error = 0, // lỗi
        //Action = 1, // thực hiện các chức năng
        DangNhap = 100,

        Insert = 101,
        Update = 102,
        Delete = 103,

        GetByID = 201,// lấy dữ liệu theo ID
        GetByName = 202, // lấy dữ liệu theo tên, key
        GetList = 203, // lấy danh sách dữ liệu      

        BackupDatabase = 901,
        RestoreDatabase = 902,

        Other = 500,

    }
    public enum EnumQuanHe : Int32
    {
        Vo = 1,
        Chong = 2,
        ConChuaThanhNien = 3,
    }

    public enum EnumLog
    {
        Insert = 1,
        Update = 2,
        Delete = 3,
    }
    public enum EnumTrangThaiDuyet
    {
        KeKhai_BanTam = 10,
        KeKhai = 100,                     // cán bộ tạo kê khai tài sản
        //HuyPheDuyetCap1 = 101,          // Chủ tịch (xã, huyện) gửi lại bản kê khai kèm ghi chú, file đính kèm
        KekhaiLai = 101,
        GuiBanKeKhai = 200,             // cán bộ gửi bản kê khai lên chủ tịch (Xã hoặc huyện)
        //HuyDuyetCap2 = 201,             // Cán bộ phòng nội vụ/ thanh tra gửi lại bản kê khai kèm ghi chú, file đính kèm
        DuyetCap1 = 300,                  // Chủ tịch (Xã, Huyện) duyệt bản kê khai (duyệt cấp 1) (Lãng đạo đơn vị duyệt bản kê khai)
        GuiThanhTraTinh = 310,                   //Gửi bản kê khai lên thanh tra Tỉnh
        ThanhTraTinhDuyet = 400,            //Thanh tra tỉnh duyệt
        HoSoPhapLy = 400,                 // duyệt cấp 2



        // trạng thái < 200 thì người kê khai mới được sửa, xóa
        // trạng thái == 200 thì chủ tịch mới được duyệt lần 1
        // 200 <=  trạng thái < 300 thì người duyệt lần 1 mới được thao tác
        // Trạng thái == 300 thì cán bộ phòng nội vụ/ thanh tra mới được duyệt lần 2
        // trạng thái == 400 không được tác động tới nữa
    }

    public enum EnumTrangThaiFilter
    {
        TongSoCanBo = 0,
        DaKeKhai = 1,
        ChuaKeKhai = 2,
        ChuaGui = 3,
        DaGui = 4,
        DaTiepNhan = 5,
        ChuaTiepNhan = 6,
    }

    public enum EnumPheDuyetBanKeKhai
    {
        PheDuyet = 1,
        XuLyLai = 2,
    }

    public enum EnumCapCoQuan : Int32
    {
        CapTrungUong = 0,
        CapTinh = 1,
        CapSo = 2,
        CapHuyen = 3,
        CapPhong = 4,
        CapXa = 5,
    }

    public enum EnumCapQuanLyCanBo : Int32
    {
        CapTinh = 1,
        CapHuyen = 2,
        ToanTinh = 3
    }
    public enum EnumLoaiDotKeKhai : Int32
    {
        HangNam = 1, // kê khai hàng năm
        BoSung = 2, // Kê khai bổ sung
        BoNhiem = 3, // Kê khai phục vụ công tác cán bộ
        LanDau = 4, // Kê khai lần đầu
    }

    public enum EnumTrangThaiCanBo
    {
        DangLamViec = 1,
        NghiHuu = 2,
    }
    public enum EnumVaiTroCanBo
    {
        Admin = 1,
        LanhDao = 2,
        ChuyenVien = 3,
    }

    /// <summary>
    /// biến động tài sản của cán bộ
    /// </summary>
    public enum EnumBienDongTaiSan : Int32
    {
        KhongBienDong = 0,
        Giam = 1,
        Tang = 2,
    }

    public enum EnumLoaiFileDinhKem : Int32
    {
        FileBangDiem = 1,
        FileDuyet = 2,
        AnhHoSo = 3,
        HuongDanSuDung = 4,  
        FileCapNhatPhuLuc = 5,  
    }

    public enum EnumLoaiThongBao : Int32
    {
        DotKeKhai = 1,
        GuiHoSoKeKhai = 2,
        DuyetHoSoKeKhai = 3, //thanh tra tinh tiep nhan ban ke khai tu lanh dao don vi
        LuuHoSoKeKhai = 4,
        BoSungHoSoKeKhai = 5,
        DuyetCongKhai = 6,
        //TTTTiepNhan = 7,
    }

    public enum EnumLoaiKeKhai : Int32
    {
        KeKhaiHangNamKeKhai = 1,
        KeKhaiBoSungDaKeKhai = 2,
        KeKhaiBoNhiemDaKeKhai = 3,
        KeKhaiLanDauDaKeKhai = 4,
        KeKhaiHangNamChuaKeKhai = 5,
        KeKhaiBoSungChuaKeKhai = 6,
        KeKhaiBoNhiemChuaKeKhai = 7,
        KeKhaiLanDauChuaKeKhai = 8

    }
    #endregion
    #region old
    public enum TrangThaiPhanGiaiQuyet
    {
        DangXuLy = 1,
        DangDuyet = 2,
        DaDuyet = 3
    }

    public enum DetailBookType
    {
        CashBook = 1,
        BankBook = 2,
        ADBook = 3,
        BCBook = 4,
        IBook = 5,
        PMCBook = 6,
        OPBook = 7,
        BDBook = 8,
        PDBook = 9,
        AsBook = 10,
        AsDBook = 11
    }

    public enum GeneralBookType
    {
        MainBook = 1,
        DDBook = 2,
        DDRBook = 3
    }

    public enum DocumentType
    {
        SoftCopy = 1, HardCopy = 2
    };

    public enum ValidType
    {
        Valid = 1,
        NotValid = 0
    }

    public enum RecruitmentType
    {
        State = 1, //Biên chế
        Recruit = 2, //Tiếp nhận
        Contract = 3, //Hợp đồng
        Job = 4, //Khoán
        Discontinue = 5 // Thôi, chuyển
    }

    public enum ActionType
    {
        EDanhMucS = 1,
        PMS = 2,
        HRMS = 3,
        CDanhMucS = 4,
        MS = 5
    }

    public enum FelicitationType
    {
        Reward = 1,
        Punish = 0
    }
    public enum ChuoiNhaThuocEnum
    {
        Gosolutions = 1
    }

    public enum VaiTroEnum
    {
        PhuTrach = 1,
        PhoiHop = 2,
        TheoDoi = 3
    }

    public enum EnumChucVu
    {
        LanhDao = 1,
        //TruongPhong = 2,
        NhanVien = 3,
    }

    public enum EnumLoaiHinhThuc
    {
        NhaThuocTuNhan = 1,
        ChuoiNhaThuoc = 2,
        NhaThuocBenhVien = 3,
    }

    public enum EnumNhomMacDinh
    {
        QuanTri = 1,
        NhanVien = 2,
        Staff = 3,
    }

    public enum EnumLoaiKiemKe
    {
        KiemKeQuayThuoc = 1,
        KiemKeNhaThuoc = 2,
    }

    public enum EnumTrangThaiCapNhat
    {
        ChuaCapNhat = 1,
        DaCapNhat = 2,
    }

    public enum EnumLoaiHuy
    {
        QuayThuoc = 2,
        NhaThuoc = 1,
    }

    public enum EnumLoaiKho
    {
        QuayThuoc = 1,
        NhaThuoc = 2,
        PhaChe = 5,
    }

    public enum EnumNoiLuuTru
    {
        Quay1 = 8,
        Quay2 = 12,
        NhaThuoc = 7,
    }

    public enum EnumKho
    {
        Quay1 = 8,
        Quay2 = 12,
        NhaThuoc = 7,
    }

    public enum EnumLoaiHoaDon
    {
        NhapThuocKhachHangTraLai = 2,
        BanLeThuocChoBenhNhan = 1,
    }





    public enum EnumTrangThaiTra
    {
        DaTra = 2,
        ChuaTra = 1,

    }

    public enum EnumTrangThaiLinh
    {
        ChoLinh = 1,
        DaLinh = 2,
        DaXuatChoQuay = 3, // phieu du tru da xuat cho quay
    }
    public enum EnumTrangThaiduTru
    {
        ChuaNhanYeuCau = 1,
        DaNhanYeuCau = 2,
    }
    public enum EnumTrangThaiKhoiTao
    {
        /*Trang thai khoi tao*/
        ChuaKhoiTao = 1,
        DaKhoiTao = 2,
    }
    public enum EnumTrangThaiXuatThuocPhaChe
    {
        ChuaXuat = 1,
        DaXuat = 2,
    }

    public enum EnumLoaiduTru
    {
        DuTruThuong = 1,
        DuTruXuatNhuong = 2,
        DuTruLinhThuocTuKho = 3,
    }
    public enum EnumLoaiduTru_QuayThuoc
    {
        DuTruDinhKy = 1,
        DuTruDotXuat = 2,
    }

    public enum EnumLoaiThuoc
    {
        ThuocNoi = 1,
        ThuocNgoai = 2,
    }

    public enum enumHuyThuocPhaChe
    {
        ChuaHuy = 1,
        DaHuy = 2,
    }
    public enum enumTrangThaiNhapDuocPhamPhaChe
    {
        ChuaNhan = 1,
        DaNhan = 2,
        DaHuy = 3,
        KhongNhan = 4,
    }
    // ant
    public enum EnumKieuNhapXuat
    {
        NhaThuoc = 1,
        QuayThuoc = 2,
    }

    public enum EnumHinhThucDangKyGoi
    {
        ThayDoiGoidichVu = 1,
        GiaHanSuDung = 2,
    }
    #endregion
};