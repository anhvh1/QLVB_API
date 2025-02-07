using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using Com.Gosol.QLVB.Security.Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Security;

namespace Com.Gosol.QLVB.DAL.KeKhai
{

    public interface IThongTinTaiSanDAL
    {
        public BaseResultModel Insert(List<ThongTinTaiSanModelPartial> ListThongTinTaiSanModel, int CanBoID, bool? BienDong, bool? LaBanTam);
        public BaseResultModel Update(List<ThongTinTaiSanModelPartial> ListThongTinTaiSanModel, int CanBoID, bool? LaBanTam);
        public ThongTinTaiSanModelPartial GetByID(int ThongTinTaiSanID);
        public List<KeKhaiModel> GetPagingBySearch(BasePagingParamsForFilter p, int CanBoID, ref int TotalRow);
        public List<ThongTinTaiSanModelPartial> ThongTinTaiSan_GetByKeKhaiID(int KeKhaiID);
        public BaseResultModel Delete(List<int> ListThongTinTaiSanID);
        public BaseResultModel Delete_BanKeKhai(List<int> ListKeKhaiID);
        public List<ThongTinTaiSanModel> GetAll();
        public List<ThongTinTaiSanModelPartial> GetLastThongTinTaiSan(int CanBoID);
        public ThongTinTaiSanLogModel ThongTinTaiSan_GetByThongTinTaiSanIDForInsertLogEncrypt(int ThongTinTaiSanID);

        // báo cáo
        public DataTable ThongKeBienDongTaiSan(int CanBoID);
        public BienDongTaiSanModel ThongKe_BienDongTaiSan(NewParams p);
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_By_CanBoID(int CanBoID, int? NamKeKhai);
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_By_CanBoID_BanTam(int CanBoID, int? NamKeKhai, int? TrangThai);
        public BaseResultModel GuiBanKeKhai(int BanKeKhaiID);
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_AndBienDong_By_KeKhaiID(int KeKhaiID);
        public CheckKeKhaiTaiSan CheckKeKhaiTaiSan(int CanBoID);
        public BaseResultModel UpdateTrangThaiBanKeKhai(KeKhaiModel KeKhaiModel);
        public BaseResultModel GetImages(string filename);
        public List<ThongTinTaiSanModel> ThongTinTaiSan_GetByKeKhaiIDForInsertLog(int KeKhaiID);
        public BaseResultModel UpdateBarcode(KeKhaiModel KeKhaiModel);
        public int CheckBarcode(int KeKhaiID, string Barcode);
    }
    public class ThongTinTaiSanDAL : IThongTinTaiSanDAL
    {
        //tên các store procedure
        private const string INSERT_THONG_TIN_TAI_SAN = @"v1_KeKhai_ThongTinTaiSan_Insert";
        private const string UPDATE_THONG_TIN_TAI_SAN = @"v1_KeKhai_ThongTinTaiSan_Update";
        private const string UPDATE_TRANG_THAI_BAN_KE_KHAI = @"v1_KeKhai_UpdateTrangThai";
        private const string DELETE_THONG_TIN_TAI_SAN = @"v1_KeKhai_ThongTinTaiSan_Delete";
        private const string DELETE_THONG_TIN_TAI_SAN_BY_KEKHAIID = @"v1_KeKhai_ThongTinTaiSan_DeleteAllByKeKhaiID";
        private const string KEKHAI_THONGTINKEKHAI_GET_BY_ID = @"v1_KeKhai_ThongTinTaiSan_GetByID";
        private const string KEKHAI_THONGTINKEKHAI_GET_BY_KEKHAIID = @"v1_KeKhai_ThongTinTaiSan_GetByKeKhaiID";
        private const string KEKHAI_THONGTINKEKHAI_GET_BY_KEKHAIID_FOR_QUANLY_BAN_KE_KHAI = @"v1_KeKhai_ThongTinTaiSan_GetByKeKhaiID_ForQuanLyBanKeKhai";
        private const string KEKHAI_THONGTINKEKHAI_GET_BY_CAPCONGTRINHID = @"v1_KeKhai_ThongTinTaiSan_GetByIDCapCongTrinh";
        private const string KEKHAI_THONGTINKEKHAI_GET_HSPL_BY_CANBOID = @"v1_KeKhai_ThongTinTaiSan_Get_HSPL_By_CanBoID";
        private const string KEKHAI_THONGTINKEKHAI_GET_HSPL_BY_CANBOID_NEW = @"v1_KeKhai_ThongTinTaiSan_Get_HSPL_By_CanBoID_New";
        private const string KEKHAI_THONGTINKEKHAI_GET_BY_CANBOID = @"v1_KeKhai_ThongTinTaiSan_GetAll_CanBoID";
        private const string KEKHAI_KEKHAI_GETPAGINGBYSEARCH = @"v1_KeKhai_KeKhai_GetPagingBySearch";
        private const string DELETE_KEKHAI = @"v1_KeKhai_Delete";
        private const string KEKHAI_GUI_BANKEKHAI = @"v1_KeKhai_GuiBanKeKhai";
        private const string KEKHAI_THONGTINKEKHAI_GETALL = @"v1_KeKhai_ThongTinTaiSan_GetAll";
        private const string CHECK_BARCODE = @"v1_KeKhai_CheckBarcode";


        //Ten các params
        private const string THONG_TIN_TAI_SAN_ID = "NV00401";
        private const string DOT_KE_KHAI_ID = "NV00302";
        private const string KE_KHAI_ID = "NV00301";
        private const string KE_KHAI_ID_THONGTINTAISAN = "NV00402";
        private const string NHOM_TAI_SAN_ID = "NV00403";
        private const string TEN_TAI_SAN = "NV00404";
        private const string DIEN_TICH = "NV00405";
        private const string GIA_TRI = "NV00406";
        private const string GIAY_CHUNG_NHAN_QUYEN_SU_DUNG = "NV00407";
        private const string LOAI_TAI_SAN_ID = "NV00408";
        private const string CAP_CONG_TRINH_ID = "NV00409";
        private const string GIAI_TRINH_NGUON_GOC = "NV00410";
        private const string THONG_TIN_KHAC = "NV00411";
        private const string CAN_BO_ID = "NV00412";
        private const string NGUOI_DUNG_TEN = "NV00413";
        private const string NGUOI_DUNG_TEN_LA_CAN_BO = "NV00414";
        private const string NAM_KE_KHAI = "NV00415";
        private const string SO_LUONG = "NV00416";
        private const string NHOM_TAI_SAN_CON_ID = "NV00417";
        private const string DIA_CHI = "NV00418";
        private const string TEN_BAN_KE_KHAI = "NV00306";
        private const string TRANG_THAI_BAN_KE_KHAI = "NV00305";
        private const string NAM = "NV00304";
        private const string LOAI_DOT_KE_KHAI = "NV00106";
        private const string DEN_NGAY = "NV00103";

        //Params log   
        private const string KE_KHAI_ID_THONGTINTAISAN_LOG = "NV004021";
        private const string NHOM_TAI_SAN_ID_LOG = "NV004031";
        private const string TEN_TAI_SAN_LOG = "NV004041";
        private const string DIEN_TICH_LOG = "NV004051";
        private const string GIA_TRI_LOG = "NV004061";
        private const string GIAY_CHUNG_NHAN_QUYEN_SU_DUNG_LOG = "NV004071";
        private const string LOAI_TAI_SAN_ID_LOG = "NV004081";
        private const string CAP_CONG_TRINH_ID_LOG = "NV004091";
        private const string GIAI_TRINH_NGUON_GOC_LOG = "NV004101";
        private const string THONG_TIN_KHAC_LOG = "NV004111";
        private const string CAN_BO_ID_LOG = "NV004121";
        private const string NGUOI_DUNG_TEN_LOG = "NV004131";
        private const string NGUOI_DUNG_TEN_LA_CAN_BO_LOG = "NV004141";
        private const string NAM_KE_KHAI_LOG = "NV004151";
        private const string SO_LUONG_LOG = "NV004161";
        private const string NHOM_TAI_SAN_CON_ID_LOG = "NV004171";
        private const string DIA_CHI_LOG = "NV004181";
        //private const string TEN_BAN_KE_KHAI = "NV00306";
        //private const string TRANG_THAI_BAN_KE_KHAI = "NV00305";
        //private const string NAM = "NV00304";
        //private const string LOAI_DOT_KE_KHAI = "NV00106";
        //private const string DEN_NGAY = "NV00103";

        #region thông tin tài sản
        public BaseResultModel InsertOne(ThongTinTaiSanModel ThongTinTaiSanModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (ThongTinTaiSanModel.LaBanTam == null || (ThongTinTaiSanModel.LaBanTam != null && ThongTinTaiSanModel.LaBanTam == false))
                {
                    switch (ThongTinTaiSanModel.NhomTaiSanID)
                    {
                        case 1:     // nhà ở
                            {
                                if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Địa chỉ nhà không được để trống";
                                    return Result;
                                }
                                if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Địa chỉ nhà quá 500 ký tự";
                                    return Result;
                                }
                                if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                {

                                }
                                else
                                {
                                    var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                    if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Loại tài sản không tồn tại";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Diện tích nhà không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crDienTich = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Diện tích không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị nhà không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        case 11:    // công trình xây dựng
                            {
                                if (ThongTinTaiSanModel.TenTaiSan.Trim().Length < 1)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Địa chỉ công trình không được để trống";
                                    return Result;
                                }
                                if (ThongTinTaiSanModel.TenTaiSan.Trim().Length > 500)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Địa chỉ công trình quá 500 ký tự";
                                    return Result;
                                }
                                if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Địa chỉ công trình không được để trống";
                                    return Result;
                                }
                                if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Địa chỉ công trình quá 500 ký tự";
                                    return Result;
                                }
                                if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                {

                                }
                                else
                                {
                                    var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                    if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Loại tài sản không tồn tại";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.CapCongTrinhID == null || ThongTinTaiSanModel.CapCongTrinhID <= 0)
                                {

                                }
                                else
                                {
                                    var capCongTrinh = new DanhMucCapCongTrinhDAL().GetByID(ThongTinTaiSanModel.CapCongTrinhID.Value);
                                    if (capCongTrinh == null || capCongTrinh.CapCongTrinhID < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Cấp công trình không tồn tại";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Diện tích công trình không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crDienTich = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Diện tích không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị công trình không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        case 12:    // đất ở
                            {
                                if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Địa chỉ không được trống";
                                    return Result;
                                }
                                if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Địa chỉ không được quá 500 ký tự";
                                    return Result;
                                }
                                if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Diện tích đất không được trống hoặc <=0";
                                    return Result;
                                }
                                else
                                {
                                    var crDienTich = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Diện tích không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị đất không được trống hoặc <=0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        case 13:    // các loại đất khác
                            {
                                if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Địa chỉ không được trống";
                                    return Result;
                                }
                                if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Địa chỉ không được quá 500 ký tự";
                                    return Result;
                                }
                                if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Diện tích đất không được trống hoặc <=0";
                                    return Result;
                                }
                                else
                                {
                                    var crDienTich = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Diện tích không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị đất không được trống hoặc <=0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        case 6:     // Tiền
                            {
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        case 3:     // Phương tiện di chuyển
                            {
                                if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Loại phương tiện không được trống";
                                    return Result;
                                }
                                else
                                {
                                    var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                    if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Loại tài sản không tồn tại";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        case 2:     // Vàng bạc trang sức
                            {
                                if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Loại tài sản không được trống";
                                    return Result;
                                }
                                else
                                {
                                    var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                    if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Loại tài sản không tồn tại";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        case 8:     // các loại tài sản khác
                            {
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                if (ThongTinTaiSanModel.TenTaiSan == null || ThongTinTaiSanModel.TenTaiSan == "")
                                {
                                    Result.Status = 0;
                                    Result.Message = "Tên tài sản không được bỏ trống";
                                    return Result;
                                }
                                break;
                            }
                        case 10:    // tài sản ở nước ngoài
                            {
                                switch (ThongTinTaiSanModel.NhomTaiSanConID)
                                {
                                    case 1:     // nhà ở
                                        {
                                            if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Địa chỉ nhà không được để trống";
                                                return Result;
                                            }
                                            if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Địa chỉ nhà quá 500 ký tự";
                                                return Result;
                                            }
                                            if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                            {

                                            }
                                            else
                                            {
                                                var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                                if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Loại tài sản không tồn tại";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Diện tích nhà không được trống hoặc nhỏ hơn 0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crDienTich = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Diện tích không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Giá trị nhà không được trống hoặc nhỏ hơn 0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crGiaTri = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không được trống";
                                                return Result;
                                            }
                                            else
                                            {
                                                if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                {
                                                    var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                    if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                        return Result;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    case 11:    // công trình xây dựng
                                        {
                                            if (ThongTinTaiSanModel.TenTaiSan.Trim().Length < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Địa chỉ công trình không được để trống";
                                                return Result;
                                            }
                                            if (ThongTinTaiSanModel.TenTaiSan.Trim().Length > 500)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Địa chỉ công trình quá 500 ký tự";
                                                return Result;
                                            }
                                            if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Địa chỉ công trình không được để trống";
                                                return Result;
                                            }
                                            if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Địa chỉ công trình quá 500 ký tự";
                                                return Result;
                                            }
                                            if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                            {

                                            }
                                            else
                                            {
                                                var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                                if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Loại tài sản không tồn tại";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.CapCongTrinhID == null || ThongTinTaiSanModel.CapCongTrinhID <= 0)
                                            {

                                            }
                                            else
                                            {
                                                var capCongTrinh = new DanhMucCapCongTrinhDAL().GetByID(ThongTinTaiSanModel.CapCongTrinhID.Value);
                                                if (capCongTrinh == null || capCongTrinh.CapCongTrinhID < 1)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Cấp công trình không tồn tại";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Diện tích công trình không được trống hoặc nhỏ hơn 0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crDienTich = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Diện tích không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Giá trị công trình không được trống hoặc nhỏ hơn 0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crGiaTri = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không được trống";
                                                return Result;
                                            }
                                            else
                                            {
                                                if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                {
                                                    var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                    if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                        return Result;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    case 12:    // đất ở
                                        {
                                            if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Địa chỉ không được trống";
                                                return Result;
                                            }
                                            if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Địa chỉ không được quá 500 ký tự";
                                                return Result;
                                            }
                                            if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Diện tích đất không được trống hoặc <=0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crDienTich = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Diện tích không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Giá trị đất không được trống hoặc <=0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crGiaTri = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không được trống";
                                                return Result;
                                            }
                                            else
                                            {
                                                if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                {
                                                    var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                    if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                        return Result;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    case 13:    // các loại đất khác
                                        {
                                            if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Địa chỉ không được trống";
                                                return Result;
                                            }
                                            if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Địa chỉ không được quá 500 ký tự";
                                                return Result;
                                            }
                                            if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Diện tích đất không được trống hoặc <=0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crDienTich = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Diện tích không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Giá trị đất không được trống hoặc <=0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crGiaTri = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không được trống";
                                                return Result;
                                            }
                                            else
                                            {
                                                if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                {
                                                    var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                    if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                        return Result;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    case 6:     // Tiền
                                        {
                                            if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crGiaTri = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không được trống";
                                                return Result;
                                            }
                                            else
                                            {
                                                if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                {
                                                    var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                    if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                        return Result;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    case 2:     // Vàng bạc trang sức
                                        {
                                            if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Loại tài sản không được trống";
                                                return Result;
                                            }
                                            else
                                            {
                                                var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                                if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Loại tài sản không tồn tại";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crGiaTri = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không được trống";
                                                return Result;
                                            }
                                            else
                                            {
                                                if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                {
                                                    var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                    if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                        return Result;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    case 8:     // các loại tài sản khác
                                        {
                                            if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crGiaTri = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không được trống";
                                                return Result;
                                            }
                                            else
                                            {
                                                if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                {
                                                    var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                    if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                        return Result;
                                                    }
                                                }
                                            }
                                            if (ThongTinTaiSanModel.TenTaiSan == null || ThongTinTaiSanModel.TenTaiSan == "")
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Tên tài sản không được bỏ trống";
                                                return Result;
                                            }
                                            break;
                                        }
                                    case 14:     // Tài sản gắn liến với đất
                                        {
                                            if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crGiaTri = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.SoLuong == null || ThongTinTaiSanModel.SoLuong <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Số lượng không được trống hoặc nhỏ hơn 0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crSoLuong = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.SoLuong.ToString(), out crSoLuong))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Số lượng không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không được trống";
                                                return Result;
                                            }
                                            else
                                            {
                                                if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                {
                                                    var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                    if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                        return Result;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    case 15:     // Cổ phiếu, trái phiếu
                                        {
                                            if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crGiaTri = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.SoLuong == null || ThongTinTaiSanModel.SoLuong <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Số lượng không được trống hoặc nhỏ hơn 0";
                                                return Result;
                                            }
                                            else
                                            {
                                                var crSoLuong = 0f; ;
                                                if (!float.TryParse(ThongTinTaiSanModel.SoLuong.ToString(), out crSoLuong))
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Số lượng không đúng định dạng";
                                                    return Result;
                                                }
                                            }
                                            if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không được trống";
                                                return Result;
                                            }
                                            else
                                            {
                                                if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                {
                                                    var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                    if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                        return Result;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    case 16:     // Tài khoản nước ngoài
                                        {
                                            if (ThongTinTaiSanModel.ThongTinKhac == null || ThongTinTaiSanModel.ThongTinKhac == "")
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Số tài khoản không được trống";
                                                return Result;
                                            }
                                            if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không được trống";
                                                return Result;
                                            }
                                            else
                                            {
                                                if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                {
                                                    var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                    if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                        return Result;
                                                    }
                                                }
                                            }
                                            if (ThongTinTaiSanModel.GiayChungNhanQuyenSuDung == null || ThongTinTaiSanModel.GiayChungNhanQuyenSuDung == "")
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Tên ngân hàng, chi nhánh không được trống";
                                                return Result;
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case 7:     // các khoản nợ
                            {
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        case 9:     // tổng thu nhập trong năm
                            {
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        case 14:     // Tài sản gắn liến với đất
                            {
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.SoLuong == null || ThongTinTaiSanModel.SoLuong <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Số lượng không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crSoLuong = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.SoLuong.ToString(), out crSoLuong))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Số lượng không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        case 15:     // Cổ phiếu, trái phiếu
                            {
                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crGiaTri = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.SoLuong == null || ThongTinTaiSanModel.SoLuong <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Số lượng không được trống hoặc nhỏ hơn 0";
                                    return Result;
                                }
                                else
                                {
                                    var crSoLuong = 0f; ;
                                    if (!float.TryParse(ThongTinTaiSanModel.SoLuong.ToString(), out crSoLuong))
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Số lượng không đúng định dạng";
                                        return Result;
                                    }
                                }
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        case 16:     // Tài khoản nước ngoài
                            {
                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Người đứng tên không được trống";
                                    return Result;
                                }
                                else
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                    {
                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                            return Result;
                                        }
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                Result.Status = 0;
                                Result.Message = "Vui lòng chọn nhóm tài sản trước khi thêm mới";
                                return Result;
                            }
                    }

                }
                SqlParameter[] parameters = new SqlParameter[]
                {
                        new SqlParameter(KE_KHAI_ID_THONGTINTAISAN,SqlDbType.Int),
                        new SqlParameter(NHOM_TAI_SAN_ID,SqlDbType.Int),
                        new SqlParameter(TEN_TAI_SAN,SqlDbType.NVarChar),
                        new SqlParameter(DIEN_TICH,SqlDbType.NVarChar),
                        new SqlParameter(GIA_TRI,SqlDbType.NVarChar),
                        new SqlParameter(GIAY_CHUNG_NHAN_QUYEN_SU_DUNG,SqlDbType.NVarChar),
                        new SqlParameter(LOAI_TAI_SAN_ID,SqlDbType.Int),
                        new SqlParameter(CAP_CONG_TRINH_ID,SqlDbType.Int),
                        new SqlParameter(GIAI_TRINH_NGUON_GOC,SqlDbType.NVarChar),
                        new SqlParameter(THONG_TIN_KHAC,SqlDbType.NVarChar),
                        new SqlParameter(CAN_BO_ID,SqlDbType.Int),
                        new SqlParameter(NGUOI_DUNG_TEN,SqlDbType.NVarChar),
                        new SqlParameter(NGUOI_DUNG_TEN_LA_CAN_BO,SqlDbType.Bit),
                        new SqlParameter(NAM_KE_KHAI,SqlDbType.Int),
                        new SqlParameter(SO_LUONG, SqlDbType.NVarChar),
                        new SqlParameter(NHOM_TAI_SAN_CON_ID, SqlDbType.Int),
                        new SqlParameter(DIA_CHI,SqlDbType.NVarChar),
                        new SqlParameter("ThongTinTaiSanIDLast",SqlDbType.Int),
                };
                parameters[0].Value = ThongTinTaiSanModel.KeKhaiID ?? Convert.DBNull;
                parameters[1].Value = ThongTinTaiSanModel.NhomTaiSanID ?? Convert.DBNull;
                parameters[2].Value = string.IsNullOrEmpty(ThongTinTaiSanModel.TenTaiSan) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.TenTaiSan);
                parameters[3].Value = Utils.ConvertToIntDouble(ThongTinTaiSanModel.DienTich, 0) == 0 ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.DienTich.ToString());
                parameters[4].Value = Utils.ConvertToIntDouble(ThongTinTaiSanModel.GiaTri, 0) == 0 ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.GiaTri.ToString());
                parameters[5].Value = string.IsNullOrEmpty(ThongTinTaiSanModel.GiayChungNhanQuyenSuDung) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.GiayChungNhanQuyenSuDung);
                parameters[6].Value = ThongTinTaiSanModel.LoaiTaiSanID ?? Convert.DBNull;
                parameters[7].Value = ThongTinTaiSanModel.CapCongTrinhID ?? Convert.DBNull;
                parameters[8].Value = string.IsNullOrEmpty(ThongTinTaiSanModel.GiaiTrinhNguonGoc) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.GiaiTrinhNguonGoc);
                parameters[9].Value = string.IsNullOrEmpty(ThongTinTaiSanModel.ThongTinKhac) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.ThongTinKhac);
                parameters[10].Value = ThongTinTaiSanModel.CanBoID ?? Convert.DBNull;
                parameters[11].Value = Utils.ConvertToInt32(ThongTinTaiSanModel.NguoiDungTen, 0) == 0 ? Convert.DBNull : ThongTinTaiSanModel.NguoiDungTen;
                parameters[12].Value = ThongTinTaiSanModel.NguoiDungTenLaCanBo ?? Convert.DBNull;
                parameters[13].Value = ThongTinTaiSanModel.NamKeKhai;
                parameters[14].Value = Utils.ConvertToInt32(ThongTinTaiSanModel.SoLuong, 0) == 0 ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.SoLuong.ToString());
                parameters[15].Value = ThongTinTaiSanModel.NhomTaiSanConID ?? Convert.DBNull;
                parameters[16].Value = string.IsNullOrEmpty(ThongTinTaiSanModel.DiaChi) ? Convert.DBNull : Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.DiaChi);
                parameters[17].Direction = ParameterDirection.Output;
                parameters[17].Size = 8;
                int thongTinTaiSanID = 0;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERT_THONG_TIN_TAI_SAN, parameters);
                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = "Kê khai tài sản thành công";
                            thongTinTaiSanID = Utils.ConvertToInt32(parameters[17].Value, 0);
                        }
                        catch (Exception ex)
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
                //Edited by ChungNN 20/1/2021
                KeKhaiModel kekhaiModel = new KeKhaiDAL().GetByID(ThongTinTaiSanModel.KeKhaiID.Value);
                if (kekhaiModel.TrangThai > 100)
                {
                    ThongTinTaiSanLogModel thongTinTaiSanLogModel =
                    new ThongTinTaiSanLogModel(
                        thongTinTaiSanID,
                        Utils.ConvertToInt32(parameters[0].Value, 0),
                        Utils.ConvertToInt32(parameters[1].Value, 0),
                        Utils.ConvertToString(parameters[2].Value, string.Empty),
                        Utils.ConvertToString(parameters[3].Value, string.Empty),
                        Utils.ConvertToString(parameters[4].Value, string.Empty),
                        Utils.ConvertToString(parameters[5].Value, string.Empty),
                        Utils.ConvertToInt32(parameters[6].Value, 0),
                        Utils.ConvertToInt32(parameters[7].Value, 0),
                        Utils.ConvertToString(parameters[8].Value, string.Empty),
                        Utils.ConvertToString(parameters[9].Value, string.Empty),
                        Utils.ConvertToInt32(parameters[10].Value, 0),
                        Utils.ConvertToString(parameters[11].Value, string.Empty),
                        Utils.ConvertToBoolean(parameters[12].Value, false),
                        Utils.ConvertToInt32(parameters[13].Value, 0),
                        Utils.ConvertToString(parameters[14].Value, string.Empty),
                        Utils.ConvertToInt32(parameters[15].Value, 0),
                        Utils.ConvertToString(parameters[16].Value, string.Empty)
                        );
                    int temp = new ThongTinTaiSanLogDAL().Insert(thongTinTaiSanLogModel, (int)EnumBanKeKhailog.Insert, null);
                }
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;

        }

        public BaseResultModel Insert(List<ThongTinTaiSanModelPartial> ListThongTinTaiSanModel, int CanBoID, bool? BienDong, bool? LaBanTam)
        {
            Boolean laBanTam = LaBanTam ?? false;

            var Result = new BaseResultModel();
            var KeKhaiID = 0;
            if (!laBanTam)
            {
                if (ListThongTinTaiSanModel[0].NamKeKhai == null || ListThongTinTaiSanModel[0].NamKeKhai < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Năm kê khai không được trống";
                    return Result;
                }
            }

            try
            {
                //var listDotKeKhai = new DotKeKhaiDAL().GetByNamKeKhai(ListThongTinTaiSanModel[0].NamKeKhai)
                //                    .Where(x => x.TuNgay <= DateTime.Now.Date
                //                    && x.DenNgay >= DateTime.Now.Date
                //                    && x.TrangThai == true
                //                    && x.CanBoID == CanBoID)
                //                    .ToList();
                var listDotKeKhai = new DotKeKhaiDAL().GetByCanBoID_NamKeKhai(DateTime.Now.Year, CanBoID);

                if (listDotKeKhai == null || listDotKeKhai.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Đợt kê khai không tồn tại hoặc đã hết thời hạn kê khai!";
                    return Result;
                }
                var crDotKeKhai = listDotKeKhai.FirstOrDefault();
                var crKeKhai = new KeKhaiDAL().GetByDotKeKhaiIDAndCanBoID(crDotKeKhai.DotKeKhaiID, CanBoID);

                if (crKeKhai == null || crKeKhai.KeKhaiID < 1)
                {
                    // tạo bản kê khai mới cho cán bộ
                    crKeKhai = new KeKhaiModel();
                    crKeKhai.DotKeKhaiID = crDotKeKhai.DotKeKhaiID;
                    crKeKhai.CanBoID = CanBoID;
                    crKeKhai.NamKeKhai = crDotKeKhai.NamKeKhai.Value;
                    crKeKhai.TrangThai = EnumTrangThaiDuyet.KeKhai.GetHashCode();
                    if (laBanTam)
                    {
                        crKeKhai.TrangThai = EnumTrangThaiDuyet.KeKhai_BanTam.GetHashCode();
                    }
                    var listBanKeKhaiCuaCanBoTrongNam = new KeKhaiDAL().GetByCanBoIDAndNamKeKhai(CanBoID, crDotKeKhai.NamKeKhai.Value);
                    crKeKhai.TenBanKeKhai = ("Bản kê khai" + (listBanKeKhaiCuaCanBoTrongNam.Count > 0 ? (" " + (listBanKeKhaiCuaCanBoTrongNam.Count + 1).ToString()) : "") + " năm " + crDotKeKhai.NamKeKhai.Value.ToString()).Trim();
                    if (BienDong != null && BienDong == false) { crKeKhai.BienDong = false; crKeKhai.TrangThai = 400; }
                    else crKeKhai.BienDong = true;
                    Result = new KeKhaiDAL().Insert(crKeKhai);
                    KeKhaiID = Utils.ConvertToInt32(Result.Data, 0);
                    crKeKhai.KeKhaiID = KeKhaiID;

                }
                else
                {
                    if (crKeKhai.TrangThai >= 200)
                    {
                        Result.Status = 0;
                        Result.Message = "Bản kê khai đã được gửi đi, không thể thêm thông tin tài sản!";
                        return Result;
                    }
                }

                for (int i = 0; i < ListThongTinTaiSanModel.Count; i++)
                {
                    var crThongTinTaiSan = ListThongTinTaiSanModel[i];
                    crThongTinTaiSan.KeKhaiID = crKeKhai.KeKhaiID;
                    crThongTinTaiSan.CanBoID = CanBoID;
                    crThongTinTaiSan.NamKeKhai = ListThongTinTaiSanModel[0].NamKeKhai;
                    crThongTinTaiSan.LaBanTam = LaBanTam;
                    Result = InsertOne(crThongTinTaiSan);
                    if (Result.Status < 1)
                    {
                        return Result;
                    }
                }
                Result.Data = KeKhaiID;
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }

        public BaseResultModel UpdateOne(ThongTinTaiSanModel ThongTinTaiSanModel)
        {
            var Result = new BaseResultModel();
            try
            {
                //Mã hóa ThongTinTaiSanModel
                string TenTaiSanUpdateEncrypt = Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.TenTaiSan);
                string DienTichUpdateEncrypt = Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.DienTich.ToString());
                string GiaTriUpdateEncrypt = Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.GiaTri.ToString());
                string GiayChungNhanQuyenSuDungUpdateEncrypt = Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.GiayChungNhanQuyenSuDung);
                string GiaiTrinhNguonGocUpdateEncrypt = Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.GiaiTrinhNguonGoc);
                string ThongTinKhacUpdateEncrypt = Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.ThongTinKhac);
                //string NguoiDungTenUpdateEncrypt = Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.NguoiDungTen.ToString());
                string SoLuongUpdateEncrypt = Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.SoLuong.ToString());
                string DiaChiUpdateEncrypt = Encrypt_Decrypt.EncryptStrings_Aes(ThongTinTaiSanModel.DiaChi);
                if (ThongTinTaiSanModel == null || ThongTinTaiSanModel.ThongTinTaiSanID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Thông tin tài sản không tồn tại";
                    return Result;
                }
                else
                {
                    var crThongTinTaiSan = GetByID(ThongTinTaiSanModel.ThongTinTaiSanID);
                    if (crThongTinTaiSan == null || crThongTinTaiSan.ThongTinTaiSanID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Thông tin tài sản không tồn tại";
                        return Result;
                    }
                    if (ThongTinTaiSanModel.LaBanTam == null || (ThongTinTaiSanModel.LaBanTam != null && ThongTinTaiSanModel.LaBanTam == false))
                    {
                        switch (ThongTinTaiSanModel.NhomTaiSanID.Value)
                        {
                            case 1:     // nhà ở
                                {
                                    if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Địa chỉ nhà không được để trống";
                                        return Result;
                                    }
                                    if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Địa chỉ nhà quá 500 ký tự";
                                        return Result;
                                    }
                                    if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                    {

                                    }
                                    else
                                    {
                                        var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                        if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Loại tài sản không tồn tại";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Diện tích nhà không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crDienTich = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Diện tích không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị nhà không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 11:    // công trình xây dựng
                                {
                                    if (ThongTinTaiSanModel.TenTaiSan.Trim().Length < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Địa chỉ công trình không được để trống";
                                        return Result;
                                    }
                                    if (ThongTinTaiSanModel.TenTaiSan.Trim().Length > 500)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Địa chỉ công trình quá 500 ký tự";
                                        return Result;
                                    }
                                    if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Địa chỉ công trình không được để trống";
                                        return Result;
                                    }
                                    if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Địa chỉ công trình quá 500 ký tự";
                                        return Result;
                                    }
                                    if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                    {

                                    }
                                    else
                                    {
                                        var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                        if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Loại tài sản không tồn tại";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.CapCongTrinhID == null || ThongTinTaiSanModel.CapCongTrinhID <= 0)
                                    {

                                    }
                                    else
                                    {
                                        var capCongTrinh = new DanhMucCapCongTrinhDAL().GetByID(ThongTinTaiSanModel.CapCongTrinhID.Value);
                                        if (capCongTrinh == null || capCongTrinh.CapCongTrinhID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Cấp công trình không tồn tại";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Diện tích công trình không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crDienTich = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Diện tích không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị công trình không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 12:    // đất ở
                                {
                                    if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Địa chỉ không được trống";
                                        return Result;
                                    }
                                    if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Địa chỉ không được quá 500 ký tự";
                                        return Result;
                                    }
                                    if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Diện tích đất không được trống hoặc <=0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crDienTich = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Diện tích không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị đất không được trống hoặc <=0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 13:    // các loại đất khác
                                {
                                    if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Địa chỉ không được trống";
                                        return Result;
                                    }
                                    if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Địa chỉ không được quá 500 ký tự";
                                        return Result;
                                    }
                                    if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Diện tích đất không được trống hoặc <=0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crDienTich = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Diện tích không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị đất không được trống hoặc <=0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 6:     // Tiền
                                {
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 3:     // Phương tiện di chuyển
                                {
                                    if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Loại phương tiện không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                        if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Loại tài sản không tồn tại";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 2:     // Vàng bạc trang sức
                                {
                                    if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Loại tài sản không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                        if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Loại tài sản không tồn tại";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 8:     // các loại tài sản khác
                                {
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    if (ThongTinTaiSanModel.TenTaiSan == null || ThongTinTaiSanModel.TenTaiSan == "")
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Tên tài sản không được bỏ trống";
                                        return Result;
                                    }
                                    break;
                                }
                            case 10:    // tài sản ở nước ngoài
                                {
                                    switch (ThongTinTaiSanModel.NhomTaiSanConID)
                                    {
                                        case 1:     // nhà ở
                                            {
                                                if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Địa chỉ nhà không được để trống";
                                                    return Result;
                                                }
                                                if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Địa chỉ nhà quá 500 ký tự";
                                                    return Result;
                                                }
                                                if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                                {

                                                }
                                                else
                                                {
                                                    var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                                    if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Loại tài sản không tồn tại";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Diện tích nhà không được trống hoặc nhỏ hơn 0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crDienTich = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Diện tích không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị nhà không được trống hoặc nhỏ hơn 0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crGiaTri = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Giá trị không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Người đứng tên không được trống";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                    {
                                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                        {
                                                            Result.Status = 0;
                                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                            return Result;
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 11:    // công trình xây dựng
                                            {
                                                if (ThongTinTaiSanModel.TenTaiSan.Trim().Length < 1)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Địa chỉ công trình không được để trống";
                                                    return Result;
                                                }
                                                if (ThongTinTaiSanModel.TenTaiSan.Trim().Length > 500)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Địa chỉ công trình quá 500 ký tự";
                                                    return Result;
                                                }
                                                if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Địa chỉ công trình không được để trống";
                                                    return Result;
                                                }
                                                if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Địa chỉ công trình quá 500 ký tự";
                                                    return Result;
                                                }
                                                if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                                {

                                                }
                                                else
                                                {
                                                    var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                                    if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Loại tài sản không tồn tại";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.CapCongTrinhID == null || ThongTinTaiSanModel.CapCongTrinhID <= 0)
                                                {

                                                }
                                                else
                                                {
                                                    var capCongTrinh = new DanhMucCapCongTrinhDAL().GetByID(ThongTinTaiSanModel.CapCongTrinhID.Value);
                                                    if (capCongTrinh == null || capCongTrinh.CapCongTrinhID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Cấp công trình không tồn tại";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Diện tích công trình không được trống hoặc nhỏ hơn 0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crDienTich = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Diện tích không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị công trình không được trống hoặc nhỏ hơn 0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crGiaTri = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Giá trị không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Người đứng tên không được trống";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                    {
                                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                        {
                                                            Result.Status = 0;
                                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                            return Result;
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 12:    // đất ở
                                            {
                                                if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Địa chỉ không được trống";
                                                    return Result;
                                                }
                                                if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Địa chỉ không được quá 500 ký tự";
                                                    return Result;
                                                }
                                                if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Diện tích đất không được trống hoặc <=0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crDienTich = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Diện tích không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị đất không được trống hoặc <=0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crGiaTri = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Giá trị không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Người đứng tên không được trống";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                    {
                                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                        {
                                                            Result.Status = 0;
                                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                            return Result;
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 13:    // các loại đất khác
                                            {
                                                if (ThongTinTaiSanModel.DiaChi.Trim().Length < 1)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Địa chỉ không được trống";
                                                    return Result;
                                                }
                                                if (ThongTinTaiSanModel.DiaChi.Trim().Length > 500)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Địa chỉ không được quá 500 ký tự";
                                                    return Result;
                                                }
                                                if (ThongTinTaiSanModel.DienTich == null || ThongTinTaiSanModel.DienTich <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Diện tích đất không được trống hoặc <=0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crDienTich = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.DienTich.ToString(), out crDienTich))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Diện tích không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị đất không được trống hoặc <=0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crGiaTri = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Giá trị không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Người đứng tên không được trống";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                    {
                                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                        {
                                                            Result.Status = 0;
                                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                            return Result;
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 6:     // Tiền
                                            {
                                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crGiaTri = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Giá trị không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Người đứng tên không được trống";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                    {
                                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                        {
                                                            Result.Status = 0;
                                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                            return Result;
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 2:     // Vàng bạc trang sức
                                            {
                                                if (ThongTinTaiSanModel.LoaiTaiSanID == null || ThongTinTaiSanModel.LoaiTaiSanID < 1)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Loại tài sản không được trống";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(ThongTinTaiSanModel.LoaiTaiSanID.Value);
                                                    if (loaiTaiSan == null || loaiTaiSan.LoaiTaiSanID < 1)
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Loại tài sản không tồn tại";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crGiaTri = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Giá trị không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Người đứng tên không được trống";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                    {
                                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                        {
                                                            Result.Status = 0;
                                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                            return Result;
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 8:     // các loại tài sản khác
                                            {
                                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crGiaTri = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Giá trị không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Người đứng tên không được trống";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                    {
                                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                        {
                                                            Result.Status = 0;
                                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                            return Result;
                                                        }
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.TenTaiSan == null || ThongTinTaiSanModel.TenTaiSan == "")
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Tên tài sản không được bỏ trống";
                                                    return Result;
                                                }
                                                break;
                                            }
                                        case 14:     // Tài sản gắn liến với đất
                                            {
                                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crGiaTri = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Giá trị không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.SoLuong == null || ThongTinTaiSanModel.SoLuong <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Số lượng không được trống hoặc nhỏ hơn 0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crSoLuong = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.SoLuong.ToString(), out crSoLuong))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Số lượng không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Người đứng tên không được trống";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                    {
                                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                        {
                                                            Result.Status = 0;
                                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                            return Result;
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 15:     // Cổ phiếu, trái phiếu
                                            {
                                                if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crGiaTri = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Giá trị không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.SoLuong == null || ThongTinTaiSanModel.SoLuong <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Số lượng không được trống hoặc nhỏ hơn 0";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    var crSoLuong = 0f; ;
                                                    if (!float.TryParse(ThongTinTaiSanModel.SoLuong.ToString(), out crSoLuong))
                                                    {
                                                        Result.Status = 0;
                                                        Result.Message = "Số lượng không đúng định dạng";
                                                        return Result;
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Người đứng tên không được trống";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                    {
                                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                        {
                                                            Result.Status = 0;
                                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                            return Result;
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 16:     // Tài khoản nước ngoài
                                            {
                                                if (ThongTinTaiSanModel.ThongTinKhac == null || ThongTinTaiSanModel.ThongTinKhac == "")
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Số tài khoản không được trống";
                                                    return Result;
                                                }
                                                if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Người đứng tên không được trống";
                                                    return Result;
                                                }
                                                else
                                                {
                                                    if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                                    {
                                                        var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                                        if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                                        {
                                                            Result.Status = 0;
                                                            Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                            return Result;
                                                        }
                                                    }
                                                }
                                                if (ThongTinTaiSanModel.GiayChungNhanQuyenSuDung == null || ThongTinTaiSanModel.GiayChungNhanQuyenSuDung == "")
                                                {
                                                    Result.Status = 0;
                                                    Result.Message = "Tên ngân hàng, chi nhánh không được trống";
                                                    return Result;
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 7:     // các khoản nợ
                                {
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 9:     // tổng thu nhập trong năm
                                {
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 14:     // Tài sản gắn liến với đất
                                {
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.SoLuong == null || ThongTinTaiSanModel.SoLuong <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Số lượng không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crSoLuong = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.SoLuong.ToString(), out crSoLuong))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Số lượng không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 15:     // Cổ phiếu, trái phiếu
                                {
                                    if (ThongTinTaiSanModel.GiaTri == null || ThongTinTaiSanModel.GiaTri <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Giá trị không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crGiaTri = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.GiaTri.ToString(), out crGiaTri))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Giá trị không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.SoLuong == null || ThongTinTaiSanModel.SoLuong <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Số lượng không được trống hoặc nhỏ hơn 0";
                                        return Result;
                                    }
                                    else
                                    {
                                        var crSoLuong = 0f; ;
                                        if (!float.TryParse(ThongTinTaiSanModel.SoLuong.ToString(), out crSoLuong))
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Số lượng không đúng định dạng";
                                            return Result;
                                        }
                                    }
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 16:     // Tài khoản nước ngoài
                                {
                                    if (ThongTinTaiSanModel.NguoiDungTen == null || ThongTinTaiSanModel.NguoiDungTen <= 0)
                                    {
                                        Result.Status = 0;
                                        Result.Message = "Người đứng tên không được trống";
                                        return Result;
                                    }
                                    else
                                    {
                                        if (ThongTinTaiSanModel.NguoiDungTenLaCanBo == null || ThongTinTaiSanModel.NguoiDungTenLaCanBo == false)// thân nhân
                                        {
                                            var crThanNhan = new KeKhaiThanNhanDAL().GetByID(ThongTinTaiSanModel.NguoiDungTen.Value);
                                            if (crThanNhan == null || crThanNhan.ThanNhanID < 1)
                                            {
                                                Result.Status = 0;
                                                Result.Message = "Người đứng tên không tồn tại (Thân nhân)";
                                                return Result;
                                            }
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    Result.Status = 0;
                                    Result.Message = "Vui lòng chọn nhóm tài sản trước khi thêm mới";
                                    return Result;
                                }
                        }
                        //Edited by ChungNN 20/1/2021
                        KeKhaiModel kekhaiModel = new KeKhaiDAL().GetByID(ThongTinTaiSanModel.KeKhaiID.Value);
                        if (kekhaiModel.TrangThai > 100)
                        {
                            ThongTinTaiSanLogModel thongTinTaiSanlogModelInsert = ThongTinTaiSan_GetByThongTinTaiSanIDForInsertLogEncrypt(ThongTinTaiSanModel.ThongTinTaiSanID);
                            if (thongTinTaiSanlogModelInsert.TenTaiSanCu != TenTaiSanUpdateEncrypt)
                            {
                                thongTinTaiSanlogModelInsert.TenTaiSanMoi = TenTaiSanUpdateEncrypt;
                            }
                            else thongTinTaiSanlogModelInsert.TenTaiSanCu = null;
                            if (thongTinTaiSanlogModelInsert.DienTichCu != DienTichUpdateEncrypt)
                            {
                                thongTinTaiSanlogModelInsert.DienTichMoi = DienTichUpdateEncrypt;
                            }
                            else thongTinTaiSanlogModelInsert.DienTichCu = null;
                            if (thongTinTaiSanlogModelInsert.GiaTriCu != GiaTriUpdateEncrypt)
                            {
                                thongTinTaiSanlogModelInsert.GiaTriMoi = GiaTriUpdateEncrypt;
                            }
                            else thongTinTaiSanlogModelInsert.GiaTriCu = null;
                            if (thongTinTaiSanlogModelInsert.GiayChungNhanQuyenSuDungCu != GiayChungNhanQuyenSuDungUpdateEncrypt)
                            {
                                thongTinTaiSanlogModelInsert.GiayChungNhanQuyenSuDungMoi = GiayChungNhanQuyenSuDungUpdateEncrypt;
                            }
                            else thongTinTaiSanlogModelInsert.GiayChungNhanQuyenSuDungCu = null;
                            if (thongTinTaiSanlogModelInsert.GiaiTrinhNguonGocCu != GiaiTrinhNguonGocUpdateEncrypt)
                            {
                                thongTinTaiSanlogModelInsert.GiaiTrinhNguonGocMoi = GiaiTrinhNguonGocUpdateEncrypt;
                            }
                            else thongTinTaiSanlogModelInsert.GiaiTrinhNguonGocCu = null;
                            if (thongTinTaiSanlogModelInsert.ThongTinKhacCu != ThongTinKhacUpdateEncrypt)
                            {
                                thongTinTaiSanlogModelInsert.ThongTinKhacMoi = ThongTinKhacUpdateEncrypt;
                            }
                            else thongTinTaiSanlogModelInsert.ThongTinKhacCu = null;
                            if (thongTinTaiSanlogModelInsert.NguoiDungTenLaCanBoCu != ThongTinTaiSanModel.NguoiDungTenLaCanBo)
                            {
                                thongTinTaiSanlogModelInsert.NguoiDungTenLaCanBoMoi = ThongTinTaiSanModel.NguoiDungTenLaCanBo;
                            }
                            else thongTinTaiSanlogModelInsert.NguoiDungTenLaCanBoCu = null;
                            if (thongTinTaiSanlogModelInsert.NguoiDungTenCu != ThongTinTaiSanModel.NguoiDungTen.ToString())
                            {
                                thongTinTaiSanlogModelInsert.NguoiDungTenMoi = ThongTinTaiSanModel.NguoiDungTen.ToString();
                            }
                            else thongTinTaiSanlogModelInsert.NguoiDungTenCu = null;
                            if (thongTinTaiSanlogModelInsert.SoLuongCu != SoLuongUpdateEncrypt)
                            {
                                thongTinTaiSanlogModelInsert.SoLuongMoi = SoLuongUpdateEncrypt;
                            }
                            else thongTinTaiSanlogModelInsert.SoLuongCu = null;
                            if (thongTinTaiSanlogModelInsert.DiaChiCu != DiaChiUpdateEncrypt)
                            {
                                thongTinTaiSanlogModelInsert.DiaChiMoi = DiaChiUpdateEncrypt;
                            }
                            else thongTinTaiSanlogModelInsert.DiaChiCu = null;
                            if (thongTinTaiSanlogModelInsert.LoaiTaiSanIDCu != ThongTinTaiSanModel.LoaiTaiSanID && thongTinTaiSanlogModelInsert.LoaiTaiSanIDCu != 0)
                            {
                                thongTinTaiSanlogModelInsert.LoaiTaiSanIDMoi = ThongTinTaiSanModel.LoaiTaiSanID;
                            }
                            else thongTinTaiSanlogModelInsert.LoaiTaiSanIDCu = null;
                            int temp = new ThongTinTaiSanLogDAL().Insert(thongTinTaiSanlogModelInsert, (int)EnumBanKeKhailog.Update, null);
                        }
                    }
                }

                SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter(THONG_TIN_TAI_SAN_ID,SqlDbType.Int),
                        new SqlParameter(NHOM_TAI_SAN_ID,SqlDbType.Int),
                        new SqlParameter(TEN_TAI_SAN,SqlDbType.NVarChar),
                        new SqlParameter(DIEN_TICH,SqlDbType.NVarChar),
                        new SqlParameter(GIA_TRI,SqlDbType.NVarChar),
                        new SqlParameter(GIAY_CHUNG_NHAN_QUYEN_SU_DUNG,SqlDbType.NVarChar),
                        new SqlParameter(LOAI_TAI_SAN_ID,SqlDbType.Int),
                        new SqlParameter(CAP_CONG_TRINH_ID,SqlDbType.Int),
                        new SqlParameter(GIAI_TRINH_NGUON_GOC,SqlDbType.NVarChar),
                        new SqlParameter(THONG_TIN_KHAC,SqlDbType.NVarChar),
                        new SqlParameter(NGUOI_DUNG_TEN,SqlDbType.NVarChar),
                        new SqlParameter(NGUOI_DUNG_TEN_LA_CAN_BO,SqlDbType.Bit),
                        new SqlParameter(SO_LUONG, SqlDbType.NVarChar),
                        new SqlParameter(NHOM_TAI_SAN_CON_ID, SqlDbType.Int),
                        new SqlParameter(DIA_CHI,SqlDbType.NVarChar),
                        //new SqlParameter(KE_KHAI_ID_THONGTINTAISAN,SqlDbType.Int)
                    };
                parameters[0].Value = ThongTinTaiSanModel.ThongTinTaiSanID;
                parameters[1].Value = ThongTinTaiSanModel.NhomTaiSanID ?? Convert.DBNull;
                parameters[2].Value = string.IsNullOrEmpty(ThongTinTaiSanModel.TenTaiSan) ? Convert.DBNull : TenTaiSanUpdateEncrypt;
                parameters[3].Value = Utils.ConvertToIntDouble(ThongTinTaiSanModel.DienTich, 0) == 0 ? Convert.DBNull : DienTichUpdateEncrypt;
                parameters[4].Value = Utils.ConvertToIntDouble(ThongTinTaiSanModel.GiaTri, 0) == 0 ? Convert.DBNull : GiaTriUpdateEncrypt;
                parameters[5].Value = string.IsNullOrEmpty(ThongTinTaiSanModel.GiayChungNhanQuyenSuDung) ? Convert.DBNull : GiayChungNhanQuyenSuDungUpdateEncrypt;
                parameters[6].Value = ThongTinTaiSanModel.LoaiTaiSanID ?? Convert.DBNull;
                parameters[7].Value = ThongTinTaiSanModel.CapCongTrinhID ?? Convert.DBNull;
                parameters[8].Value = string.IsNullOrEmpty(ThongTinTaiSanModel.GiaiTrinhNguonGoc) ? Convert.DBNull : GiaiTrinhNguonGocUpdateEncrypt;
                parameters[9].Value = string.IsNullOrEmpty(ThongTinTaiSanModel.ThongTinKhac) ? Convert.DBNull : ThongTinKhacUpdateEncrypt;
                parameters[10].Value = Utils.ConvertToInt32(ThongTinTaiSanModel.NguoiDungTen, 0) == 0 ? Convert.DBNull : ThongTinTaiSanModel.NguoiDungTen;
                parameters[11].Value = ThongTinTaiSanModel.NguoiDungTenLaCanBo ?? Convert.DBNull;
                parameters[12].Value = Utils.ConvertToInt32(ThongTinTaiSanModel.SoLuong, 0) == 0 ? Convert.DBNull : SoLuongUpdateEncrypt;
                parameters[13].Value = ThongTinTaiSanModel.NhomTaiSanConID ?? Convert.DBNull;
                parameters[14].Value = string.IsNullOrEmpty(ThongTinTaiSanModel.DiaChi) ? Convert.DBNull : DiaChiUpdateEncrypt;
                //parameters[15].Value = ThongTinTaiSanModel.KeKhaiID;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATE_THONG_TIN_TAI_SAN, parameters);
                            trans.Commit();
                            Result.Status = 1;
                            Result.Message = "Sửa Thông tin kê khai tài sản thành công";
                        }
                        catch (Exception ex)
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }

        public BaseResultModel Update(List<ThongTinTaiSanModelPartial> ListThongTinTaiSanModel, int CanBoID, bool? LaBanTam)
        {
            var Result = new BaseResultModel();
            try
            {
                var crKeKhai = new KeKhaiDAL().GetByID(GetByID(ListThongTinTaiSanModel[0].ThongTinTaiSanID).KeKhaiID.Value);
                if (crKeKhai == null || crKeKhai.KeKhaiID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Bản kê khai không tồn tại";
                    return Result;
                }
                if (crKeKhai.TrangThai >= 200)
                {
                    Result.Status = 0;
                    Result.Message = "Bản kê khai đã được gửi, không thể sửa thông tin tài sản!";
                    return Result;
                }

                for (int i = 0; i < ListThongTinTaiSanModel.Count; i++)
                {
                    ListThongTinTaiSanModel[i].LaBanTam = LaBanTam;
                    Result = UpdateOne(ListThongTinTaiSanModel[i]);
                }
                //Update trạng thái bản tạm thành bản kê khai
                if (crKeKhai != null && crKeKhai.TrangThai == EnumTrangThaiDuyet.KeKhai_BanTam.GetHashCode() && LaBanTam != null && LaBanTam == false)
                {
                    crKeKhai.TrangThai = EnumTrangThaiDuyet.KeKhai.GetHashCode();
                    UpdateTrangThaiBanKeKhai(crKeKhai);
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }

        public BaseResultModel UpdateTrangThaiBanKeKhai(KeKhaiModel KeKhaiModel)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                   new SqlParameter(KE_KHAI_ID, SqlDbType.Int),
                   new SqlParameter(TRANG_THAI_BAN_KE_KHAI, SqlDbType.Int),
                };
                parameters[0].Value = KeKhaiModel.KeKhaiID;
                parameters[1].Value = KeKhaiModel.TrangThai;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            var query = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATE_TRANG_THAI_BAN_KE_KHAI, parameters), 0);
                            trans.Commit();
                            if (query > 0)
                            {
                                Result.Status = 1;
                                Result.Message = "Update bản kê khai thành công";
                                Result.Data = query;
                            }
                            else
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                            }
                        }
                        catch
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }

        public BaseResultModel Delete(List<int> ListThongTinTaiSanID)
        {
            var Result = new BaseResultModel();
            if (ListThongTinTaiSanID.Count <= 0)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                var crKeKhai = new KeKhaiDAL().GetByID(ListThongTinTaiSanID[0]);
                if (crKeKhai != null && crKeKhai.KeKhaiID > 0 && crKeKhai.TrangThai >= 200) //check trạng thái bản kê khai
                {
                    Result.Status = 0;
                    Result.Message = "Bản kê khai đã được gửi, không thể xóa thông tin tài sản!";
                    return Result;
                }
                var crDotKeKhai = new DotKeKhaiDAL().GetByID(crKeKhai.DotKeKhaiID);
                if (crDotKeKhai.TrangThai == false)
                {
                    Result.Status = 0;
                    Result.Message = "Đợt kê khai đã hết thời hạn!";
                    return Result;
                }
                for (int i = 0; i < ListThongTinTaiSanID.Count; i++)
                {
                    int val = 0;
                    if (!int.TryParse(ListThongTinTaiSanID[i].ToString(), out val))
                    {
                        Result.Status = 0;
                        Result.Message = "ThongTinTaiSanID '" + ListThongTinTaiSanID[i].ToString() + "' không đúng định dạng";
                        return Result;
                    }
                    else
                    {
                        var ctThongTinTaiSab = GetByID(ListThongTinTaiSanID[i]);
                        if (ctThongTinTaiSab == null || ctThongTinTaiSab.ThongTinTaiSanID < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "ThongTinTaiSanID '" + ListThongTinTaiSanID[i].ToString() + "' không tồn tại";
                            return Result;
                        }
                        else
                        {
                            //Edited by ChungNN 20/1/2021
                            int KeKhaiID = new KeKhaiDAL().GetKeKhaiIDByThongTinTaiSanID(ListThongTinTaiSanID[i]);
                            KeKhaiModel kekhaiModel = new KeKhaiDAL().GetByID(KeKhaiID);
                            if (kekhaiModel.TrangThai > 100)
                            {
                                ThongTinTaiSanLogModel thongTinTaiSanlogModel = ThongTinTaiSan_GetByThongTinTaiSanIDForInsertLogEncrypt(ListThongTinTaiSanID[i]);
                                int temp = new ThongTinTaiSanLogDAL().Insert(thongTinTaiSanlogModel, (int)EnumBanKeKhailog.Delete, null);
                            }
                            ////////////////////////////////////////////////////////////////////////////////////////////
                            SqlParameter[] parameters = new SqlParameter[]
                            {
                                new SqlParameter(THONG_TIN_TAI_SAN_ID, SqlDbType.Int)
                            };
                            parameters[0].Value = ListThongTinTaiSanID[i];
                            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                            {
                                conn.Open();
                                using (SqlTransaction trans = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE_THONG_TIN_TAI_SAN, parameters);
                                        trans.Commit();
                                        if (val < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Không thể xóa thông tin tài sản có ThongTinTaiSanID = " + ListThongTinTaiSanID[i];
                                            return Result;
                                        }

                                        // nếu bản kê khai không còn tt tài sản thì xóa  bản kê khai
                                        var thongTinTaiSan = ThongTinTaiSan_GetByKeKhaiID(crKeKhai.KeKhaiID);
                                        if (thongTinTaiSan == null || thongTinTaiSan.Count < 1)
                                        {
                                            var dlBanKeKhai = Delete_BanKeKhai(new List<int>() { crKeKhai.KeKhaiID });
                                        }
                                    }
                                    catch
                                    {
                                        Result.Status = -1;
                                        Result.Message = ConstantLogMessage.API_Error_System;
                                        trans.Rollback();
                                        return Result;
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Thông tin tài sản");
                return Result;
            }
        }

        public BaseResultModel DeleteAllByKeKhaiID(int KeKhaiID)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                           {
                                new SqlParameter(KE_KHAI_ID_THONGTINTAISAN, SqlDbType.Int)
                           };
                parameters[0].Value = KeKhaiID;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE_THONG_TIN_TAI_SAN_BY_KEKHAIID, parameters);
                            trans.Commit();
                            if (val < 0)
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                            }
                        }
                        catch (Exception ex)
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            return Result;
                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                return Result;
            }
            Result.Status = 1;
            Result.Message = ConstantLogMessage.Alert_Delete_Success("Thông tin tài sản");
            return Result;

        }

        public ThongTinTaiSanModelPartial GetByID(int ThongTinTaiSanID)
        {
            var Result = new ThongTinTaiSanModelPartial();
            if (ThongTinTaiSanID < 1)
            {
                return Result;
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter(THONG_TIN_TAI_SAN_ID, SqlDbType.Int),
            };
            parameters[0].Value = ThongTinTaiSanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_THONGTINKEKHAI_GET_BY_ID, parameters))
                {
                    while (dr.Read())
                    {
                        Result.ThongTinTaiSanID = Utils.ConvertToInt32(dr[THONG_TIN_TAI_SAN_ID], 0);
                        Result.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_THONGTINTAISAN], 0);
                        Result.NhomTaiSanID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_ID], 0);
                        Result.TenTaiSan = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[TEN_TAI_SAN], string.Empty));
                        Result.DienTich = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIEN_TICH], string.Empty)), null);
                        Result.GiaTri = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIA_TRI], string.Empty)), null);
                        Result.GiayChungNhanQuyenSuDung = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAY_CHUNG_NHAN_QUYEN_SU_DUNG], string.Empty));
                        Result.LoaiTaiSanID = Utils.ConvertToInt32(dr[LOAI_TAI_SAN_ID], 0);
                        Result.CapCongTrinhID = Utils.ConvertToInt32(dr[CAP_CONG_TRINH_ID], 0);
                        Result.GiaiTrinhNguonGoc = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAI_TRINH_NGUON_GOC], string.Empty));
                        Result.ThongTinKhac = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[THONG_TIN_KHAC], string.Empty));
                        Result.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        Result.NguoiDungTenID = Utils.ConvertToInt32(dr[NGUOI_DUNG_TEN], 0);
                        Result.NguoiDungTenLaCanBo = Utils.ConvertToBoolean(dr[NGUOI_DUNG_TEN_LA_CAN_BO], false);
                        Result.SoLuong = Utils.ConvertToNullableInt32(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[SO_LUONG], string.Empty)), null);
                        Result.NhomTaiSanConID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_CON_ID], 0);
                        break;
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        // Lấy danh sách  thông tin tài sản cho bản kê khai by KeKhaiID
        public List<ThongTinTaiSanModelPartial> ThongTinTaiSan_GetByKeKhaiID(int KeKhaiID)
        {
            var Result = new List<ThongTinTaiSanModelPartial>();
            if (KeKhaiID < 1)
            {
                return Result;
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter(KE_KHAI_ID_THONGTINTAISAN, SqlDbType.Int),
            };
            parameters[0].Value = KeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_THONGTINKEKHAI_GET_BY_KEKHAIID, parameters))
                {
                    while (dr.Read())
                    {
                        var crThongTinTaiSan = new ThongTinTaiSanModelPartial();
                        crThongTinTaiSan.ThongTinTaiSanID = Utils.ConvertToInt32(dr[THONG_TIN_TAI_SAN_ID], 0);
                        crThongTinTaiSan.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_THONGTINTAISAN], 0);
                        crThongTinTaiSan.NhomTaiSanID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_ID], 0);
                        crThongTinTaiSan.TenTaiSan = Utils.ConvertToString(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[TEN_TAI_SAN], string.Empty)), string.Empty);
                        crThongTinTaiSan.DienTich = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIEN_TICH], string.Empty)), null);
                        crThongTinTaiSan.GiaTri = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIA_TRI], string.Empty)), null);
                        crThongTinTaiSan.GiayChungNhanQuyenSuDung = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAY_CHUNG_NHAN_QUYEN_SU_DUNG], string.Empty));
                        crThongTinTaiSan.LoaiTaiSanID = Utils.ConvertToInt32(dr[LOAI_TAI_SAN_ID], 0);
                        crThongTinTaiSan.CapCongTrinhID = Utils.ConvertToInt32(dr[CAP_CONG_TRINH_ID], 0);
                        crThongTinTaiSan.GiaiTrinhNguonGoc = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAI_TRINH_NGUON_GOC], string.Empty));
                        crThongTinTaiSan.ThongTinKhac = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[THONG_TIN_KHAC], string.Empty));
                        crThongTinTaiSan.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        crThongTinTaiSan.NguoiDungTenID = Utils.ConvertToInt32(dr[NGUOI_DUNG_TEN], 0);
                        crThongTinTaiSan.NguoiDungTenLaCanBo = Utils.ConvertToBoolean(dr[NGUOI_DUNG_TEN_LA_CAN_BO], false);
                        crThongTinTaiSan.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        crThongTinTaiSan.SoLuong = Utils.ConvertToNullableInt32(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[SO_LUONG], string.Empty)), null);
                        crThongTinTaiSan.NhomTaiSanConID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_CON_ID], 0);
                        crThongTinTaiSan.DiaChi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIA_CHI], string.Empty));
                        crThongTinTaiSan.TenLoaiTaiSan = Utils.ConvertToString(dr["TenLoaiTaiSan"], string.Empty);
                        crThongTinTaiSan.TenCapCongTrinh = Utils.ConvertToString(dr["TenCapCongTrinh"], string.Empty);
                        Result.Add(crThongTinTaiSan);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        // Lấy danh sách  thông tin tài sản cho bản kê khai by KeKhaiID phục vụ cho insert dữ liệu bảng ThongTinTaiSanLog(Edited by ChungNN 21/1/2021)
        public List<ThongTinTaiSanModel> ThongTinTaiSan_GetByKeKhaiIDForInsertLog(int KeKhaiID)
        {
            var Result = new List<ThongTinTaiSanModel>();
            if (KeKhaiID < 1)
            {
                return Result;
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter(KE_KHAI_ID_THONGTINTAISAN, SqlDbType.Int),
            };
            parameters[0].Value = KeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_THONGTINKEKHAI_GET_BY_KEKHAIID, parameters))
                {
                    while (dr.Read())
                    {
                        var crThongTinTaiSan = new ThongTinTaiSanModel();
                        crThongTinTaiSan.ThongTinTaiSanID = Utils.ConvertToInt32(dr[THONG_TIN_TAI_SAN_ID], 0);
                        crThongTinTaiSan.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_THONGTINTAISAN], 0);
                        crThongTinTaiSan.NhomTaiSanID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_ID], 0);
                        crThongTinTaiSan.TenTaiSan = Utils.ConvertToString(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[TEN_TAI_SAN], string.Empty)), string.Empty);
                        crThongTinTaiSan.DienTich = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIEN_TICH], string.Empty)), null);
                        crThongTinTaiSan.GiaTri = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIA_TRI], string.Empty)), null);
                        crThongTinTaiSan.GiayChungNhanQuyenSuDung = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAY_CHUNG_NHAN_QUYEN_SU_DUNG], string.Empty));
                        crThongTinTaiSan.LoaiTaiSanID = Utils.ConvertToInt32(dr[LOAI_TAI_SAN_ID], 0);
                        crThongTinTaiSan.CapCongTrinhID = Utils.ConvertToInt32(dr[CAP_CONG_TRINH_ID], 0);
                        crThongTinTaiSan.GiaiTrinhNguonGoc = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAI_TRINH_NGUON_GOC], string.Empty));
                        crThongTinTaiSan.ThongTinKhac = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[THONG_TIN_KHAC], string.Empty));
                        crThongTinTaiSan.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        crThongTinTaiSan.NguoiDungTen = Utils.ConvertToInt32(dr[NGUOI_DUNG_TEN], 0);
                        crThongTinTaiSan.NguoiDungTenLaCanBo = Utils.ConvertToBoolean(dr[NGUOI_DUNG_TEN_LA_CAN_BO], false);
                        crThongTinTaiSan.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        crThongTinTaiSan.SoLuong = Utils.ConvertToNullableInt32(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[SO_LUONG], string.Empty)), null);
                        crThongTinTaiSan.NhomTaiSanConID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_CON_ID], 0);
                        crThongTinTaiSan.DiaChi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIA_CHI], string.Empty));
                        Result.Add(crThongTinTaiSan);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        //Lấy chi tiết thông tin tài sản bởi ThongTinTaiSanID (Edited by ChungNN 21/1/2021)
        public ThongTinTaiSanLogModel ThongTinTaiSan_GetByThongTinTaiSanIDForInsertLogEncrypt(int ThongTinTaiSanID)
        {
            var crThongTinTaiSan = new ThongTinTaiSanLogModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter(THONG_TIN_TAI_SAN_ID, SqlDbType.Int),
            };
            parameters[0].Value = ThongTinTaiSanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_THONGTINKEKHAI_GET_BY_ID, parameters))
                {
                    if (dr.Read())
                    {
                        crThongTinTaiSan.ThongTinTaiSanID = Utils.ConvertToInt32(dr[THONG_TIN_TAI_SAN_ID], 0);
                        crThongTinTaiSan.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_THONGTINTAISAN], 0);
                        crThongTinTaiSan.NhomTaiSanID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_ID], 0);
                        crThongTinTaiSan.TenTaiSanCu = Utils.ConvertToString(dr[TEN_TAI_SAN], string.Empty);
                        crThongTinTaiSan.DienTichCu = Utils.ConvertToString(dr[DIEN_TICH], string.Empty);
                        crThongTinTaiSan.GiaTriCu = Utils.ConvertToString(dr[GIA_TRI], string.Empty);
                        crThongTinTaiSan.GiayChungNhanQuyenSuDungCu = Utils.ConvertToString(dr[GIAY_CHUNG_NHAN_QUYEN_SU_DUNG], string.Empty);
                        crThongTinTaiSan.LoaiTaiSanIDCu = Utils.ConvertToInt32(dr[LOAI_TAI_SAN_ID], 0);
                        crThongTinTaiSan.CapCongTrinhID = Utils.ConvertToInt32(dr[CAP_CONG_TRINH_ID], 0);
                        crThongTinTaiSan.GiaiTrinhNguonGocCu = Utils.ConvertToString(dr[GIAI_TRINH_NGUON_GOC], string.Empty);
                        crThongTinTaiSan.ThongTinKhacCu = Utils.ConvertToString(dr[THONG_TIN_KHAC], string.Empty);
                        crThongTinTaiSan.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        crThongTinTaiSan.NguoiDungTenCu = Utils.ConvertToString(dr[NGUOI_DUNG_TEN], string.Empty);
                        crThongTinTaiSan.NguoiDungTenLaCanBoCu = Utils.ConvertToBoolean(dr[NGUOI_DUNG_TEN_LA_CAN_BO], false);
                        crThongTinTaiSan.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        crThongTinTaiSan.SoLuongCu = Utils.ConvertToString(dr[SO_LUONG], string.Empty);
                        crThongTinTaiSan.NhomTaiSanConID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_CON_ID], 0);
                        crThongTinTaiSan.DiaChiCu = Utils.ConvertToString(dr[DIA_CHI], string.Empty);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return crThongTinTaiSan;
        }

        // Lấy danh sách  thông tin tài sản cho bản kê khai ở trang quản lý bản kê khai by KeKhaiID
        public List<ThongTinTaiSanModelPartial> ThongTinTaiSan_GetByKeKhaiID_ForQuanLyBanKeKhai(int KeKhaiID)
        {
            var Result = new List<ThongTinTaiSanModelPartial>();
            if (KeKhaiID < 1)
            {
                return Result;
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter(KE_KHAI_ID_THONGTINTAISAN, SqlDbType.Int),
            };
            parameters[0].Value = KeKhaiID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_THONGTINKEKHAI_GET_BY_KEKHAIID_FOR_QUANLY_BAN_KE_KHAI, parameters))
                {
                    while (dr.Read())
                    {
                        var crThongTinTaiSan = new ThongTinTaiSanModelPartial();
                        crThongTinTaiSan.ThongTinTaiSanID = Utils.ConvertToInt32(dr[THONG_TIN_TAI_SAN_ID], 0);
                        crThongTinTaiSan.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_THONGTINTAISAN], 0);
                        crThongTinTaiSan.NhomTaiSanID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_ID], 0);
                        crThongTinTaiSan.GiaTri = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIA_TRI], string.Empty)), null);
                        crThongTinTaiSan.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        crThongTinTaiSan.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        crThongTinTaiSan.NhomTaiSanChaID = Utils.ConvertToInt32(dr["NhomTaiSanChaID"], 0);
                        crThongTinTaiSan.CapCongTrinhID = Utils.ConvertToInt32(dr[CAP_CONG_TRINH_ID], 0);
                        crThongTinTaiSan.DienTich = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIEN_TICH], string.Empty)), null);
                        crThongTinTaiSan.GiaiTrinhNguonGoc = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAI_TRINH_NGUON_GOC], string.Empty));
                        crThongTinTaiSan.GiayChungNhanQuyenSuDung = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAY_CHUNG_NHAN_QUYEN_SU_DUNG], string.Empty));
                        crThongTinTaiSan.LoaiTaiSanID = Utils.ConvertToInt32(dr[LOAI_TAI_SAN_ID], 0);
                        crThongTinTaiSan.NguoiDungTen = Utils.ConvertToInt32(dr[NGUOI_DUNG_TEN], 0);
                        crThongTinTaiSan.SoLuong = Utils.ConvertToNullableInt32(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[SO_LUONG], string.Empty)), null);
                        crThongTinTaiSan.NhomTaiSanConID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_CON_ID], 0);
                        crThongTinTaiSan.NguoiDungTenLaCanBo = Utils.ConvertToBoolean(dr[NGUOI_DUNG_TEN_LA_CAN_BO], false);
                        crThongTinTaiSan.TenTaiSan = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[TEN_TAI_SAN], string.Empty));
                        crThongTinTaiSan.ThongTinKhac = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[THONG_TIN_KHAC], string.Empty));
                        crThongTinTaiSan.DiaChi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIA_CHI], string.Empty));
                        Result.Add(crThongTinTaiSan);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        // Lấy danh sách  thông tin tài sản cho bản kê khai by CapCongTrinhID
        public List<ThongTinTaiSanModelPartial> GetByIDCapCongTrinh(int CapCongTrinhID)
        {
            List<ThongTinTaiSanModelPartial> List = new List<ThongTinTaiSanModelPartial>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter(CAP_CONG_TRINH_ID,SqlDbType.Int)
                };
                parameters[0].Value = CapCongTrinhID;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_THONGTINKEKHAI_GET_BY_CAPCONGTRINHID, parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinTaiSanModelPartial ThongTinTaiSanModelPartial = new ThongTinTaiSanModelPartial();
                        ThongTinTaiSanModelPartial.ThongTinTaiSanID = Utils.ConvertToInt32(dr[THONG_TIN_TAI_SAN_ID], 0);
                        ThongTinTaiSanModelPartial.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_THONGTINTAISAN], 0);
                        ThongTinTaiSanModelPartial.NhomTaiSanID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_ID], 0);
                        ThongTinTaiSanModelPartial.TenTaiSan = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[TEN_TAI_SAN], string.Empty));
                        ThongTinTaiSanModelPartial.DienTich = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIEN_TICH], string.Empty)), null);
                        ThongTinTaiSanModelPartial.GiaTri = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIA_TRI], string.Empty)), null);
                        ThongTinTaiSanModelPartial.GiayChungNhanQuyenSuDung = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAY_CHUNG_NHAN_QUYEN_SU_DUNG], string.Empty));
                        ThongTinTaiSanModelPartial.LoaiTaiSanID = Utils.ConvertToInt32(dr[LOAI_TAI_SAN_ID], 0);
                        ThongTinTaiSanModelPartial.CapCongTrinhID = Utils.ConvertToInt32(dr[CAP_CONG_TRINH_ID], 0);
                        ThongTinTaiSanModelPartial.GiaiTrinhNguonGoc = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAI_TRINH_NGUON_GOC], string.Empty));
                        ThongTinTaiSanModelPartial.ThongTinKhac = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[THONG_TIN_KHAC], string.Empty));
                        ThongTinTaiSanModelPartial.SoLuong = Utils.ConvertToNullableInt32(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[SO_LUONG], string.Empty)), null);
                        ThongTinTaiSanModelPartial.NhomTaiSanConID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_CON_ID], 0);
                        ThongTinTaiSanModelPartial.DiaChi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIA_CHI], string.Empty));

                        List.Add(ThongTinTaiSanModelPartial);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return List;
        }

        // Lấy danh sách thông tin tài sản của cán bộ có bản kê khai đã được duyệt lần 2 (Trangthai >= 400)
        public List<ThongTinTaiSanModelPartial> ThongTinTaiSan_Get_HoSoPhapLy_By_CanBoID(int CanBoID)
        {
            var Result = new List<ThongTinTaiSanModelPartial>();
            if (CanBoID < 1)
            {
                return Result;
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter(CAN_BO_ID, SqlDbType.Int),
            };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_THONGTINKEKHAI_GET_HSPL_BY_CANBOID, parameters))
                {
                    while (dr.Read())
                    {
                        var crThongTinTaiSan = new ThongTinTaiSanModelPartial();
                        crThongTinTaiSan.ThongTinTaiSanID = Utils.ConvertToInt32(dr[THONG_TIN_TAI_SAN_ID], 0);
                        crThongTinTaiSan.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_THONGTINTAISAN], 0);
                        crThongTinTaiSan.NhomTaiSanID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_ID], 0);
                        crThongTinTaiSan.GiaTri = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIA_TRI], string.Empty)), null);
                        crThongTinTaiSan.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        crThongTinTaiSan.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        crThongTinTaiSan.NhomTaiSanChaID = Utils.ConvertToInt32(dr["NhomTaiSanChaID"], 0);
                        crThongTinTaiSan.CapCongTrinhID = Utils.ConvertToInt32(dr[CAP_CONG_TRINH_ID], 0);
                        crThongTinTaiSan.DienTich = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIEN_TICH], string.Empty)), null);
                        crThongTinTaiSan.GiaiTrinhNguonGoc = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAI_TRINH_NGUON_GOC], string.Empty));
                        crThongTinTaiSan.GiayChungNhanQuyenSuDung = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAY_CHUNG_NHAN_QUYEN_SU_DUNG], string.Empty));
                        crThongTinTaiSan.LoaiTaiSanID = Utils.ConvertToInt32(dr[LOAI_TAI_SAN_ID], 0);
                        crThongTinTaiSan.NguoiDungTen = Utils.ConvertToInt32(dr[NGUOI_DUNG_TEN], 0);
                        crThongTinTaiSan.SoLuong = Utils.ConvertToNullableInt32(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[SO_LUONG], string.Empty)), null);
                        crThongTinTaiSan.NhomTaiSanConID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_CON_ID], 0);
                        crThongTinTaiSan.NguoiDungTenLaCanBo = Utils.ConvertToBoolean(dr[NGUOI_DUNG_TEN_LA_CAN_BO], false);
                        crThongTinTaiSan.TenTaiSan = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[TEN_TAI_SAN], string.Empty));
                        crThongTinTaiSan.DiaChi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIA_CHI], string.Empty));
                        crThongTinTaiSan.ThongTinKhac = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[THONG_TIN_KHAC], string.Empty));
                        crThongTinTaiSan.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        crThongTinTaiSan.TrangThaiSuDung = Utils.ConvertToBoolean(dr["TrangThaiSuDung"], false);

                        Result.Add(crThongTinTaiSan);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        // Lấy danh sách thông tin tài sản của cán bộ
        public List<ThongTinTaiSanModelPartial> ThongTinTaiSan_Get_HoSoPhapLy_By_CanBoID(int CanBoID, int NamKeKhai, int TrangThai)
        {
            var Result = new List<ThongTinTaiSanModelPartial>();
            if (CanBoID < 1)
            {
                return Result;
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter(CAN_BO_ID, SqlDbType.Int),
               new SqlParameter(NAM_KE_KHAI, SqlDbType.Int),
               new SqlParameter(TRANG_THAI_BAN_KE_KHAI, SqlDbType.Int),
            };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_THONGTINKEKHAI_GET_HSPL_BY_CANBOID_NEW, parameters))
                {
                    while (dr.Read())
                    {
                        var crThongTinTaiSan = new ThongTinTaiSanModelPartial();
                        crThongTinTaiSan.ThongTinTaiSanID = Utils.ConvertToInt32(dr[THONG_TIN_TAI_SAN_ID], 0);
                        crThongTinTaiSan.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        crThongTinTaiSan.NhomTaiSanID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_ID], 0);
                        crThongTinTaiSan.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        crThongTinTaiSan.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        crThongTinTaiSan.NhomTaiSanChaID = Utils.ConvertToInt32(dr["NhomTaiSanChaID"], 0);
                        crThongTinTaiSan.CapCongTrinhID = Utils.ConvertToInt32(dr[CAP_CONG_TRINH_ID], 0);
                        crThongTinTaiSan.LoaiTaiSanID = Utils.ConvertToInt32(dr[LOAI_TAI_SAN_ID], 0);
                        crThongTinTaiSan.NhomTaiSanConID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_CON_ID], 0);
                        crThongTinTaiSan.NguoiDungTenLaCanBo = Utils.ConvertToBoolean(dr[NGUOI_DUNG_TEN_LA_CAN_BO], false);
                        crThongTinTaiSan.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        crThongTinTaiSan.TrangThaiSuDung = Utils.ConvertToBoolean(dr["TrangThaiSuDung"], false);
                        crThongTinTaiSan.TenTaiSan = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[TEN_TAI_SAN], string.Empty));
                        crThongTinTaiSan.DienTich = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIEN_TICH], string.Empty)), null);
                        crThongTinTaiSan.GiaTri = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIA_TRI], string.Empty)), null);
                        crThongTinTaiSan.GiayChungNhanQuyenSuDung = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAY_CHUNG_NHAN_QUYEN_SU_DUNG], string.Empty));
                        crThongTinTaiSan.GiaiTrinhNguonGoc = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAI_TRINH_NGUON_GOC], string.Empty));
                        crThongTinTaiSan.ThongTinKhac = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[THONG_TIN_KHAC], string.Empty));
                        crThongTinTaiSan.SoLuong = Utils.ConvertToNullableInt32(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[SO_LUONG], string.Empty)), null);
                        crThongTinTaiSan.DiaChi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIA_CHI], string.Empty));

                        Result.Add(crThongTinTaiSan);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        // Lấy danh sách thông tin tài sản cảu các bản kê khai của cán bộ
        public List<ThongTinTaiSanModelPartial> ThongTinTaiSan_GetAll_By_CanBoID(int CanBoID)
        {
            var Result = new List<ThongTinTaiSanModelPartial>();
            if (CanBoID < 1)
            {
                return Result;
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter(CAN_BO_ID, SqlDbType.Int),
            };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, KEKHAI_THONGTINKEKHAI_GET_BY_CANBOID, parameters))
                {
                    while (dr.Read())
                    {
                        var crThongTinTaiSan = new ThongTinTaiSanModelPartial();
                        crThongTinTaiSan.ThongTinTaiSanID = Utils.ConvertToInt32(dr[THONG_TIN_TAI_SAN_ID], 0);
                        crThongTinTaiSan.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_THONGTINTAISAN], 0);
                        crThongTinTaiSan.NhomTaiSanID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_ID], 0);
                        crThongTinTaiSan.GiaTri = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIA_TRI], string.Empty)), null);
                        crThongTinTaiSan.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        crThongTinTaiSan.NamKeKhai = Utils.ConvertToInt32(dr[NAM_KE_KHAI], 0);
                        crThongTinTaiSan.NhomTaiSanChaID = Utils.ConvertToInt32(dr["NhomTaiSanChaID"], 0);
                        crThongTinTaiSan.CapCongTrinhID = Utils.ConvertToInt32(dr[CAP_CONG_TRINH_ID], 0);
                        crThongTinTaiSan.DienTich = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIEN_TICH], string.Empty)), null);
                        crThongTinTaiSan.GiaiTrinhNguonGoc = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAI_TRINH_NGUON_GOC], string.Empty));
                        crThongTinTaiSan.GiayChungNhanQuyenSuDung = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIAY_CHUNG_NHAN_QUYEN_SU_DUNG], string.Empty));
                        crThongTinTaiSan.LoaiTaiSanID = Utils.ConvertToInt32(dr[LOAI_TAI_SAN_ID], 0);
                        crThongTinTaiSan.NguoiDungTen = Utils.ConvertToInt32(dr[NGUOI_DUNG_TEN], 0);
                        crThongTinTaiSan.NguoiDungTenID = Utils.ConvertToInt32(dr[NGUOI_DUNG_TEN], 0);
                        crThongTinTaiSan.SoLuong = Utils.ConvertToNullableInt32(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[SO_LUONG], string.Empty)), null);
                        crThongTinTaiSan.NhomTaiSanConID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_CON_ID], 0);
                        crThongTinTaiSan.NguoiDungTenLaCanBo = Utils.ConvertToBoolean(dr[NGUOI_DUNG_TEN_LA_CAN_BO], false);
                        crThongTinTaiSan.TenTaiSan = Utils.ConvertToString(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[TEN_TAI_SAN], string.Empty)), string.Empty);
                        crThongTinTaiSan.DiaChi = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIA_CHI], string.Empty));
                        crThongTinTaiSan.ThongTinKhac = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[THONG_TIN_KHAC], string.Empty));
                        crThongTinTaiSan.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        crThongTinTaiSan.TrangThaiBanKeKhai = Utils.ConvertToInt32(dr[TRANG_THAI_BAN_KE_KHAI], 0);
                        Result.Add(crThongTinTaiSan);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        #endregion

        #region thống kê, báo cáo.

        public DataTable ThongKeBienDongTaiSan(int CanBoID)
        {
            var Result = new DataTable();
            Result.Columns.Add("ChiTieu");
            Result.Columns.Add(NHOM_TAI_SAN_ID);
            try
            {
                var listTaiSan = ThongTinTaiSan_Get_HoSoPhapLy_By_CanBoID(CanBoID);
                var listNamKeKhai = new List<int>();
                listNamKeKhai.AddRange(listTaiSan.Where(x => !listNamKeKhai.Contains(x.NamKeKhai)).Select(y => y.NamKeKhai));
                listNamKeKhai.OrderBy(x => x);
                var listNhomTaiSan = new DanhMucNhomTaiSanDAL().GetAllNhomTaiSanCha();
                listNamKeKhai.ForEach(x => Result.Columns.Add(x.ToString()));
                for (int i = 0; i < listNhomTaiSan.Count; i++)
                {
                    var crRow = Result.NewRow();
                    crRow["ChiTieu"] = listNhomTaiSan[i].TenNhomTaiSan;
                    if (listNhomTaiSan[i].NhomTaiSanChaID == null || listNhomTaiSan[i].NhomTaiSanChaID < 1)
                        crRow[NHOM_TAI_SAN_ID] = listNhomTaiSan[i].NhomTaiSanID;
                    else crRow[NHOM_TAI_SAN_ID] = listNhomTaiSan[i].NhomTaiSanChaID;

                    for (int j = 0; j < listNamKeKhai.Count; j++)
                    {
                        var KeKhaiIDMoiNhat = listTaiSan.Where(x => x.NamKeKhai == listNamKeKhai[j]).ToList().OrderByDescending(x => x.KeKhaiID).ToList().FirstOrDefault().KeKhaiID;
                        crRow[listNamKeKhai[j].ToString()] = listTaiSan.Where(x => (x.KeKhaiID == KeKhaiIDMoiNhat && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID))).ToList().Sum(y => y.GiaTri).ToString();
                    }
                    Result.Rows.Add(crRow);

                }

            }
            catch (Exception)
            {

                throw;
            }
            return Result;
        }

        public BienDongTaiSanModel ThongKe_BienDongTaiSan1(NewParams p)
        {
            var Result = new BienDongTaiSanModel();
            var Data = new DataTable();
            Data.Columns.Add("ChiTieu");
            Data.Columns.Add(NHOM_TAI_SAN_ID);
            try
            {
                var listTaiSan = ThongTinTaiSan_Get_HoSoPhapLy_By_CanBoID(p.CanBoID).Where(x => (p.DanhSachNhomTaiSanID != null && p.DanhSachNhomTaiSanID.Count > 0) ? p.DanhSachNhomTaiSanID.Contains(x.NhomTaiSanID.Value) || p.DanhSachNhomTaiSanID.Contains(x.NhomTaiSanChaID.Value) : (x.NhomTaiSanID == x.NhomTaiSanID && x.TrangThaiSuDung == true)).ToList();
                var listNamKeKhai = new List<int>();
                listNamKeKhai.AddRange(listTaiSan.Where(x => !listNamKeKhai.Contains(x.NamKeKhai)).Select(y => y.NamKeKhai));
                listNamKeKhai.OrderBy(x => x);
                var listNhomTaiSan = new DanhMucNhomTaiSanDAL().GetAllNhomTaiSanCha()
                    .Where(x => (p.DanhSachNhomTaiSanID != null && p.DanhSachNhomTaiSanID.Count > 0) ? p.DanhSachNhomTaiSanID.Contains(x.NhomTaiSanID) || p.DanhSachNhomTaiSanID.Contains(x.NhomTaiSanChaID.Value) : (x.NhomTaiSanID == x.NhomTaiSanID && x.TrangThaiSuDung == true))
                    .ToList();
                listNamKeKhai.ForEach(x => Data.Columns.Add(x.ToString()));
                for (int i = 0; i < listNhomTaiSan.Count; i++)
                {
                    var crRow = Data.NewRow();
                    crRow["ChiTieu"] = listNhomTaiSan[i].TenNhomTaiSan;
                    crRow[NHOM_TAI_SAN_ID] = listNhomTaiSan[i].NhomTaiSanID;
                    for (int j = 0; j < listNamKeKhai.Count; j++)
                    {
                        var KeKhaiIDMoiNhat = listTaiSan.Where(x => x.NamKeKhai == listNamKeKhai[j]).ToList().OrderByDescending(x => x.KeKhaiID).ToList().FirstOrDefault().KeKhaiID;
                        crRow[listNamKeKhai[j].ToString()] = listTaiSan.Where(x => (x.KeKhaiID == KeKhaiIDMoiNhat && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID))).ToList().Sum(y => y.GiaTri).ToString();
                    }
                    Data.Rows.Add(crRow);
                    Result.Data = Data;
                    Result.DanhSachNam = listNamKeKhai;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return Result;
        }

        public BienDongTaiSanModel ThongKe_BienDongTaiSan(NewParams p)
        {
            var Result = new BienDongTaiSanModel();
            var Data = new DataTable();
            Data.Columns.Add("ChiTieu");
            Data.Columns.Add(NHOM_TAI_SAN_ID);
            try
            {
                var listTaiSan = ThongTinTaiSan_Get_HoSoPhapLy_By_CanBoID(p.CanBoID).Where(x => (p.DanhSachNhomTaiSanID != null && p.DanhSachNhomTaiSanID.Count > 0) ? p.DanhSachNhomTaiSanID.Contains(x.NhomTaiSanID.Value) || p.DanhSachNhomTaiSanID.Contains(x.NhomTaiSanChaID.Value) : (x.NhomTaiSanID == x.NhomTaiSanID && x.TrangThaiSuDung == true)).ToList();
                var listNamKeKhai = new List<int>();
                listNamKeKhai.AddRange(listTaiSan.Where(x => !listNamKeKhai.Contains(x.NamKeKhai)).Select(y => y.NamKeKhai));
                listNamKeKhai.OrderBy(x => x);
                var listNhomTaiSan = new DanhMucNhomTaiSanDAL().GetAllNhomTaiSanCha()
                    .Where(x => (p.DanhSachNhomTaiSanID != null && p.DanhSachNhomTaiSanID.Count > 0) ? p.DanhSachNhomTaiSanID.Contains(x.NhomTaiSanID) || p.DanhSachNhomTaiSanID.Contains(x.NhomTaiSanChaID.Value) : (x.NhomTaiSanID == x.NhomTaiSanID && x.TrangThaiSuDung == true))
                    .ToList();


                if (listNamKeKhai != null && listNamKeKhai.Count > 0)
                {
                    if (listNamKeKhai.Count == 1)
                    {
                        List<int> listKeKhaiID = new List<int>();
                        listKeKhaiID.AddRange(listTaiSan.Where(x => !listKeKhaiID.Contains(x.KeKhaiID.Value)).Select(y => y.KeKhaiID.Value));
                        listKeKhaiID = listKeKhaiID.OrderBy(x => x).ToList();
                        if (listKeKhaiID != null && listKeKhaiID.Count > 1)
                        {
                            Data.Columns.Add(listNamKeKhai.FirstOrDefault().ToString() + "1");
                            Data.Columns.Add(listNamKeKhai.FirstOrDefault().ToString() + "2");
                            var KeKhaiIDFirst = listKeKhaiID.FirstOrDefault();
                            var KeKhaiIDLast = listKeKhaiID.LastOrDefault();
                            for (int i = 0; i < listNhomTaiSan.Count; i++)
                            {
                                var crRow = Data.NewRow();
                                crRow["ChiTieu"] = listNhomTaiSan[i].TenNhomTaiSan;
                                crRow[NHOM_TAI_SAN_ID] = listNhomTaiSan[i].NhomTaiSanID;
                                for (int j = 0; j < listNamKeKhai.Count; j++)
                                {
                                    var KeKhaiFist = listTaiSan.Where(x => (x.KeKhaiID == KeKhaiIDFirst && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID))).ToList().Sum(y => y.GiaTri).ToString();
                                    var KeKhaiLast = listTaiSan.Where(x => (x.KeKhaiID == KeKhaiIDLast && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID))).ToList().Sum(y => y.GiaTri).ToString();
                                    crRow[listNamKeKhai.FirstOrDefault().ToString() + "1"] = KeKhaiFist.ToString();
                                    crRow[listNamKeKhai.FirstOrDefault().ToString() + "2"] = KeKhaiLast.ToString();
                                }
                                Data.Rows.Add(crRow);

                            }
                            Result.Data = Data;
                            Result.DanhSachNam = new List<int>();
                            Result.DanhSachNam.Add(Utils.ConvertToInt32(listNamKeKhai.FirstOrDefault().ToString() + "1", 0));
                            Result.DanhSachNam.Add(Utils.ConvertToInt32(listNamKeKhai.FirstOrDefault().ToString() + "2", 0));
                        }
                        else
                        {
                            listNamKeKhai.ForEach(x => Data.Columns.Add(x.ToString()));
                            for (int i = 0; i < listNhomTaiSan.Count; i++)
                            {
                                var crRow = Data.NewRow();
                                crRow["ChiTieu"] = listNhomTaiSan[i].TenNhomTaiSan;
                                crRow[NHOM_TAI_SAN_ID] = listNhomTaiSan[i].NhomTaiSanID;
                                for (int j = 0; j < listNamKeKhai.Count; j++)
                                {
                                    var KeKhaiIDMoiNhat = listTaiSan.Where(x => x.NamKeKhai == listNamKeKhai[j]).ToList().OrderByDescending(x => x.KeKhaiID).ToList().FirstOrDefault().KeKhaiID;
                                    crRow[listNamKeKhai[j].ToString()] = listTaiSan.Where(x => (x.KeKhaiID == KeKhaiIDMoiNhat && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID))).ToList().Sum(y => y.GiaTri).ToString();
                                }
                                Data.Rows.Add(crRow);
                            }
                            Result.Data = Data;
                            Result.DanhSachNam = listNamKeKhai;
                        }
                    }
                    else
                    {
                        listNamKeKhai.ForEach(x => Data.Columns.Add(x.ToString()));
                        for (int i = 0; i < listNhomTaiSan.Count; i++)
                        {
                            var crRow = Data.NewRow();
                            crRow["ChiTieu"] = listNhomTaiSan[i].TenNhomTaiSan;
                            crRow[NHOM_TAI_SAN_ID] = listNhomTaiSan[i].NhomTaiSanID;
                            for (int j = 0; j < listNamKeKhai.Count; j++)
                            {
                                var KeKhaiIDMoiNhat = listTaiSan.Where(x => x.NamKeKhai == listNamKeKhai[j]).ToList().OrderByDescending(x => x.KeKhaiID).ToList().FirstOrDefault().KeKhaiID;
                                crRow[listNamKeKhai[j].ToString()] = listTaiSan.Where(x => (x.KeKhaiID == KeKhaiIDMoiNhat && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID))).ToList().Sum(y => y.GiaTri).ToString();
                            }
                            Data.Rows.Add(crRow);

                        }
                        Result.Data = Data;
                        Result.DanhSachNam = listNamKeKhai;
                    }

                }



            }
            catch (Exception)
            {

                throw;
            }
            return Result;
        }

        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_By_CanBoID(int CanBoID, int? NamKeKhai)
        {
            var Result = new ChiTietThongTinKeKhaiModel();
            if (CanBoID < 1)
            {
                return Result;
            }
            try
            {
                // thông tin bản thân
                var ThongTinBanThan = new HeThongCanBoDAL().GetChiTietCanBoByID(CanBoID);
                Result.BanThan = ThongTinBanThan;
                // thông tin vợ chồng
                var ThongTinThanNhan = new KeKhaiThanNhanDAL().GetThanNhanCanBo_By_CanBoID(CanBoID);
                Result.VoChong = ThongTinThanNhan.VoChong;
                // thông tin con chưa thành niên
                Result.ConChuaThanhNien = ThongTinThanNhan.ConChuaThanhNien;

                // thông tin tài sản
                Result.ThongTinTaiSan = ThongTinTaiSan_Get_HoSoPhapLy_By_CanBoID(CanBoID).Where(x => x.NamKeKhai == NamKeKhai || NamKeKhai == null || NamKeKhai == 0).ToList();
                Result.DanhSachBanKeKhai = Result.ThongTinTaiSan.GroupBy(c => new { c.KeKhaiID, c.TenBanKeKhai }).Select(x => new KeKhaiModel(x.Key.KeKhaiID.Value, x.Key.TenBanKeKhai)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        /// <summary>
        /// Thông tin bản kê khai lưu tạm
        /// </summary>
        /// <param name="CanBoID"></param>
        /// <param name="NamKeKhai"></param>
        /// <param name="TrangThai"></param>
        /// <returns></returns>
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_By_CanBoID_BanTam(int CanBoID, int? NamKeKhai, int? TrangThai)
        {
            var Result = new ChiTietThongTinKeKhaiModel();
            if (CanBoID < 1)
            {
                return Result;
            }
            try
            {
                // thông tin bản thân
                var ThongTinBanThan = new HeThongCanBoDAL().GetChiTietCanBoByID(CanBoID);
                Result.BanThan = ThongTinBanThan;
                // thông tin vợ chồng
                var ThongTinThanNhan = new KeKhaiThanNhanDAL().GetThanNhanCanBo_By_CanBoID(CanBoID);
                Result.VoChong = ThongTinThanNhan.VoChong;
                // thông tin con chưa thành niên
                Result.ConChuaThanhNien = ThongTinThanNhan.ConChuaThanhNien;

                // thông tin tài sản
                Result.ThongTinTaiSan = ThongTinTaiSan_Get_HoSoPhapLy_By_CanBoID(CanBoID, NamKeKhai ?? 0, TrangThai ?? 0);
                Result.DanhSachBanKeKhai = Result.ThongTinTaiSan.GroupBy(c => new { c.KeKhaiID, c.TenBanKeKhai }).Select(x => new KeKhaiModel(x.Key.KeKhaiID.Value, x.Key.TenBanKeKhai)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        /// <summary>
        /// lấy chi tiết thông tin kê khai bao gồm
        /// 1. thông tin bản thân: tên, ngày sinh....
        /// 2. thông tin thân nhân (2.1. vợ chồng; 2.2. con chưa thành niên)
        /// 3. thông tin tài sản của bản kê khai hiện tại
        /// 4. gải trình biến động tài sản (của bản kê khai hiện tại so với bản kê khai trước đó gần nhất)
        /// 5. loại kê khai
        /// </summary>
        /// <param name=KeKhaiID></param>
        /// <param name=NAM_KE_KHAI></param>
        /// <returns></returns>
        public ChiTietThongTinKeKhaiModel ChiTietThongTinKeKhai_AndBienDong_By_KeKhaiID(int KeKhaiID)
        {
            var Result = new ChiTietThongTinKeKhaiModel();
            if (KeKhaiID < 1)
            {
                return Result;
            }
            try
            {
                var crKeKhai = new KeKhaiDAL().GetByID(KeKhaiID);
                if (crKeKhai == null || crKeKhai.CanBoID < 1 || crKeKhai.KeKhaiID < 1) return Result;
                //Loại đợt kê khai
                Result.LoaiDotKeKhai = crKeKhai.LoaiDotKeKhaiID;
                // thông tin bản thân
                var ThongTinBanThan = new HeThongCanBoDAL().GetChiTietCanBoByID(crKeKhai.CanBoID);
                Result.BanThan = ThongTinBanThan;
                // thông tin vợ chồng
                var ThongTinThanNhan = new KeKhaiThanNhanDAL().GetThanNhanCanBo_By_CanBoID(crKeKhai.CanBoID);
                Result.VoChong = ThongTinThanNhan.VoChong;

                // thông tin con chưa thành niên
                Result.ConChuaThanhNien = ThongTinThanNhan.ConChuaThanhNien;

                // thông tin tài sản
                var ThongTinTaiSanHienTai = ThongTinTaiSan_GetByKeKhaiID(KeKhaiID);
                Result.ThongTinTaiSan = ThongTinTaiSanHienTai;

                //thông tin tài san log
                Result.DanhSachThongTinTaiSanLog = new ThongTinTaiSanLogDAL().GetThongtinTaiSanLogByKeKhaiID(KeKhaiID);

                //file Đính kèm log
                Result.DanhSachFileDinhKemLog = new FileDinhKemLogDAL().GetFileDinhKemLogByKekhaiID(KeKhaiID);

                // giải trình biến động tài sản
                var BanKeKhaiGanNhat = new KeKhaiDAL().GetList_By_CanBoID(crKeKhai.CanBoID)
                    .Where(x => x.KeKhaiID != KeKhaiID && x.KeKhaiID < KeKhaiID).OrderByDescending(x => x.KeKhaiID).FirstOrDefault();

                if (BanKeKhaiGanNhat != null && BanKeKhaiGanNhat.KeKhaiID > 0)
                {
                    var ThongTinTaiSanGanNhat = ThongTinTaiSan_GetByKeKhaiID(BanKeKhaiGanNhat.KeKhaiID);
                    //Lấy thông tin tài sản tăng thêm nếu là loại bổ sung
                    if (Result.LoaiDotKeKhai == EnumLoaiDotKeKhai.BoSung.GetHashCode())
                    {
                        int NamKeKhai = ThongTinTaiSanHienTai[0].NamKeKhai;
                        Result.ThongTinTaiSan = GetTaiSanTangThem(ThongTinTaiSanHienTai, ThongTinTaiSanGanNhat);
                        Result.NamKeKhai = NamKeKhai;
                    }
                    /////
                    if (ThongTinTaiSanHienTai.Count > 0 && ThongTinTaiSanGanNhat.Count > 0)
                    {
                        var TaiSanHienTai = (from x in ThongTinTaiSanHienTai
                                             group x by x.NhomTaiSanID into NhomTaiSan
                                             select new ThongTinTaiSanModelPartial()
                                             {
                                                 NhomTaiSanID = NhomTaiSan.Key.Value,
                                                 GiaTri = NhomTaiSan.Sum(x => x.GiaTri),
                                                 GiaiTrinhNguonGoc = string.Concat(
                                                     NhomTaiSan.Select(
                                                         x => (x.GiaiTrinhNguonGoc != null && x.GiaiTrinhNguonGoc.Length > 0) ? (x.GiaiTrinhNguonGoc + ", ") : x.GiaiTrinhNguonGoc))
                                                            .TrimEnd().TrimEnd(','),
                                                 SoLuong = NhomTaiSan.Count()
                                             }).ToList();
                        List<int> ListFilterNhomTaiSan = new List<int> { 1, 11, 12, 13, 2, 6, 10, 9 }; //List nhóm tài sản thống kê (Lấy từ db_nhomtaisan)
                        TaiSanHienTai = TaiSanHienTai.Where(x => ListFilterNhomTaiSan.Contains(x.NhomTaiSanID.Value)).ToList();


                        var LoaiTaiSanHienTai = (from x in ThongTinTaiSanHienTai.Where(x => x.NhomTaiSanID != 10).ToList()
                                                 group x by x.LoaiTaiSanID into LoaiTaiSan
                                                 select new ThongTinTaiSanModelPartial()
                                                 {
                                                     LoaiTaiSanID = LoaiTaiSan.Key.Value,
                                                     GiaTri = LoaiTaiSan.Sum(x => x.GiaTri),
                                                     GiaiTrinhNguonGoc = string.Concat(
                                                         LoaiTaiSan.Select(
                                                             x => (x.GiaiTrinhNguonGoc != null && x.GiaiTrinhNguonGoc.Length > 0) ? (x.GiaiTrinhNguonGoc + ", ") : x.GiaiTrinhNguonGoc))
                                                                .TrimEnd().TrimEnd(','),
                                                     SoLuong = LoaiTaiSan.Count()
                                                 }).ToList();
                        List<int> ListFilterLoaiTaiSan = new List<int> { 16, 17, 1, 2, 3, 12, 13, 14, 15 }; //List loại tài sản thống kê (Lấy từ db_loaitaisan)
                        LoaiTaiSanHienTai = LoaiTaiSanHienTai.Where(x => ListFilterLoaiTaiSan.Contains(x.LoaiTaiSanID.Value)).ToList();


                        var TaiSanGanNhat = (from x in ThongTinTaiSanGanNhat
                                             group x by x.NhomTaiSanID into NhomTaiSan
                                             select new ThongTinTaiSanModelPartial()
                                             {
                                                 NhomTaiSanID = NhomTaiSan.Key.Value,
                                                 GiaTri = NhomTaiSan.Sum(x => x.GiaTri),
                                                 SoLuong = NhomTaiSan.Count()
                                             }).ToList();
                        TaiSanGanNhat = TaiSanGanNhat.Where(x => ListFilterNhomTaiSan.Contains(x.NhomTaiSanID.Value)).ToList();

                        var LoaiTaiSanGanNhat = (from x in ThongTinTaiSanGanNhat.Where(x => x.NhomTaiSanID != 10).ToList()
                                                 group x by x.LoaiTaiSanID into LoaiTaiSan
                                                 select new ThongTinTaiSanModelPartial()
                                                 {
                                                     LoaiTaiSanID = LoaiTaiSan.Key.Value,
                                                     GiaTri = LoaiTaiSan.Sum(x => x.GiaTri),
                                                     SoLuong = LoaiTaiSan.Count()
                                                 }).ToList();
                        LoaiTaiSanGanNhat = LoaiTaiSanGanNhat.Where(x => ListFilterLoaiTaiSan.Contains(x.LoaiTaiSanID.Value)).ToList();
                        //Biến động tài sản theo nhóm tài sản
                        for (int i = 0; i < TaiSanHienTai.Count; i++)
                        {
                            var NhomTaiSanGanNhat = TaiSanGanNhat.FirstOrDefault(x => x.NhomTaiSanID == TaiSanHienTai[i].NhomTaiSanID);
                            if (NhomTaiSanGanNhat != null && NhomTaiSanGanNhat.NhomTaiSanID > 0)
                            {
                                var giaTriTaiSanGianNhatCuaNhomTaiSan = TaiSanGanNhat.Where(x => x.NhomTaiSanID == TaiSanHienTai[i].NhomTaiSanID).Sum(x => x.GiaTri);
                                var soLuongTaiSanGanNhatCuaNhomTaiSan = TaiSanGanNhat.Where(x => x.NhomTaiSanID == TaiSanHienTai[i].NhomTaiSanID).FirstOrDefault().SoLuong;
                                //Tăng giảm giá trị
                                if (giaTriTaiSanGianNhatCuaNhomTaiSan > TaiSanHienTai[i].GiaTri)
                                {
                                    TaiSanHienTai[i].TangGiam = EnumBienDongTaiSan.Giam.GetHashCode();
                                    TaiSanHienTai[i].GiaTri = giaTriTaiSanGianNhatCuaNhomTaiSan - TaiSanHienTai[i].GiaTri;
                                }
                                else if (giaTriTaiSanGianNhatCuaNhomTaiSan < TaiSanHienTai[i].GiaTri)
                                {
                                    TaiSanHienTai[i].TangGiam = EnumBienDongTaiSan.Tang.GetHashCode();
                                    TaiSanHienTai[i].GiaTri = TaiSanHienTai[i].GiaTri - giaTriTaiSanGianNhatCuaNhomTaiSan;
                                }
                                else
                                {
                                    TaiSanHienTai[i].TangGiam = EnumBienDongTaiSan.KhongBienDong.GetHashCode();
                                    TaiSanHienTai[i].GiaTri = 0;
                                }

                                //Tăng giảm số lượng
                                if (soLuongTaiSanGanNhatCuaNhomTaiSan > TaiSanHienTai[i].SoLuong)
                                {
                                    TaiSanHienTai[i].SoLuongTangGiam = EnumBienDongTaiSan.Giam.GetHashCode();
                                    TaiSanHienTai[i].SoLuong = soLuongTaiSanGanNhatCuaNhomTaiSan - TaiSanHienTai[i].SoLuong;
                                }
                                else if (soLuongTaiSanGanNhatCuaNhomTaiSan < TaiSanHienTai[i].SoLuong)
                                {
                                    TaiSanHienTai[i].SoLuongTangGiam = EnumBienDongTaiSan.Tang.GetHashCode();
                                    TaiSanHienTai[i].SoLuong = TaiSanHienTai[i].SoLuong - soLuongTaiSanGanNhatCuaNhomTaiSan;
                                }
                                else
                                {
                                    TaiSanHienTai[i].SoLuongTangGiam = EnumBienDongTaiSan.KhongBienDong.GetHashCode();
                                    TaiSanHienTai[i].SoLuong = 0;
                                }
                            }
                            else
                            {
                                TaiSanHienTai[i].TangGiam = EnumBienDongTaiSan.Tang.GetHashCode();
                                TaiSanHienTai[i].SoLuongTangGiam = EnumBienDongTaiSan.Tang.GetHashCode();
                            }
                        }
                        // tài sản có trong quá khứ mà không có trong hiện tại
                        var TaiSanHienTaiKhongCo = TaiSanGanNhat.Where(x => !TaiSanHienTai.Select(y => y.NhomTaiSanID).ToList().Contains(x.NhomTaiSanID)).ToList();
                        if (TaiSanHienTaiKhongCo.Count > 0)
                        {
                            for (int i = 0; i < TaiSanHienTaiKhongCo.Count; i++)
                            {
                                var item = new ThongTinTaiSanModelPartial();
                                item.NhomTaiSanID = TaiSanHienTaiKhongCo[i].NhomTaiSanID;
                                item.TangGiam = EnumBienDongTaiSan.Giam.GetHashCode();
                                item.GiaTri = TaiSanHienTaiKhongCo[i].GiaTri;
                                item.SoLuongTangGiam = EnumBienDongTaiSan.Giam.GetHashCode();
                                item.SoLuong = TaiSanHienTaiKhongCo[i].SoLuong;
                                TaiSanHienTai.Add(item);
                            }
                        }

                        //Biến động tài sản theo loại tài sản
                        for (int i = 0; i < LoaiTaiSanHienTai.Count; i++)
                        {
                            var LoaiTaiSanGanNhatSoSanh = LoaiTaiSanGanNhat.FirstOrDefault(x => x.LoaiTaiSanID == LoaiTaiSanHienTai[i].LoaiTaiSanID);
                            if (LoaiTaiSanGanNhatSoSanh != null && LoaiTaiSanGanNhatSoSanh.LoaiTaiSanID > 0)
                            {
                                var giaTriTaiSanGianNhatCuaLoaiTaiSan = LoaiTaiSanGanNhat.Where(x => x.LoaiTaiSanID == LoaiTaiSanHienTai[i].LoaiTaiSanID).Sum(x => x.GiaTri);
                                var soLuongTaiSanGanNhatCuaLoaiTaiSan = LoaiTaiSanGanNhat.Where(x => x.LoaiTaiSanID == LoaiTaiSanHienTai[i].LoaiTaiSanID).Count();
                                //Tăng giảm giá trị
                                if (giaTriTaiSanGianNhatCuaLoaiTaiSan > LoaiTaiSanHienTai[i].GiaTri)
                                {
                                    LoaiTaiSanHienTai[i].TangGiam = EnumBienDongTaiSan.Giam.GetHashCode();
                                    LoaiTaiSanHienTai[i].GiaTri = giaTriTaiSanGianNhatCuaLoaiTaiSan - LoaiTaiSanHienTai[i].GiaTri;
                                }
                                else if (giaTriTaiSanGianNhatCuaLoaiTaiSan < LoaiTaiSanHienTai[i].GiaTri)
                                {
                                    LoaiTaiSanHienTai[i].TangGiam = EnumBienDongTaiSan.Tang.GetHashCode();
                                    LoaiTaiSanHienTai[i].GiaTri = LoaiTaiSanHienTai[i].GiaTri - giaTriTaiSanGianNhatCuaLoaiTaiSan;
                                }
                                else
                                {
                                    LoaiTaiSanHienTai[i].TangGiam = EnumBienDongTaiSan.KhongBienDong.GetHashCode();
                                    LoaiTaiSanHienTai[i].GiaTri = 0;
                                }
                                //Tăng giảm số lượng
                                if (soLuongTaiSanGanNhatCuaLoaiTaiSan > LoaiTaiSanHienTai[i].SoLuong)
                                {
                                    LoaiTaiSanHienTai[i].SoLuongTangGiam = EnumBienDongTaiSan.Giam.GetHashCode();
                                    LoaiTaiSanHienTai[i].SoLuong = soLuongTaiSanGanNhatCuaLoaiTaiSan - LoaiTaiSanHienTai[i].SoLuong;
                                }
                                else if (soLuongTaiSanGanNhatCuaLoaiTaiSan < LoaiTaiSanHienTai[i].SoLuong)
                                {
                                    LoaiTaiSanHienTai[i].SoLuongTangGiam = EnumBienDongTaiSan.Tang.GetHashCode();
                                    LoaiTaiSanHienTai[i].SoLuong = LoaiTaiSanHienTai[i].SoLuong - soLuongTaiSanGanNhatCuaLoaiTaiSan;
                                }
                                else
                                {
                                    LoaiTaiSanHienTai[i].SoLuongTangGiam = EnumBienDongTaiSan.KhongBienDong.GetHashCode();
                                    LoaiTaiSanHienTai[i].SoLuong = 0;
                                }
                            }
                            else
                            {
                                LoaiTaiSanHienTai[i].TangGiam = EnumBienDongTaiSan.Tang.GetHashCode();
                                LoaiTaiSanHienTai[i].SoLuongTangGiam = EnumBienDongTaiSan.Tang.GetHashCode();
                            }
                        }
                        // tài sản có trong quá khứ mà không có trong hiện tại
                        var LoaiTaiSanHienTaiKhongCo = LoaiTaiSanGanNhat.Where(x => !LoaiTaiSanHienTai.Select(y => y.LoaiTaiSanID).ToList().Contains(x.LoaiTaiSanID)).ToList();
                        if (LoaiTaiSanHienTaiKhongCo.Count > 0)
                        {
                            for (int i = 0; i < LoaiTaiSanHienTaiKhongCo.Count; i++)
                            {
                                var item = new ThongTinTaiSanModelPartial();
                                item.LoaiTaiSanID = LoaiTaiSanHienTaiKhongCo[i].LoaiTaiSanID;
                                item.TangGiam = EnumBienDongTaiSan.Giam.GetHashCode();
                                item.GiaTri = LoaiTaiSanHienTaiKhongCo[i].GiaTri;
                                item.SoLuongTangGiam = EnumBienDongTaiSan.Giam.GetHashCode();
                                item.SoLuong = LoaiTaiSanHienTaiKhongCo[i].SoLuong;
                                LoaiTaiSanHienTai.Add(item);
                            }
                        }


                        Result.BienDongTaiSan = TaiSanHienTai;
                        Result.BienDongTaiSan.AddRange(LoaiTaiSanHienTai.Select(x => x));


                    }
                    else
                        Result.BienDongTaiSan = new List<ThongTinTaiSanModelPartial>();


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        #endregion


        #region Bản kê khai
        public List<KeKhaiModel> GetPagingBySearch(BasePagingParamsForFilter p, int CanBoID, ref int TotalRow)
        {
            List<KeKhaiModel> list = new List<KeKhaiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("Keyword",SqlDbType.NVarChar),
                new SqlParameter("OrderByName",SqlDbType.NVarChar),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter(CAN_BO_ID,SqlDbType.Int),
                new SqlParameter(NAM,SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),
                new SqlParameter(LOAI_DOT_KE_KHAI,SqlDbType.Int),
                new SqlParameter(TRANG_THAI_BAN_KE_KHAI,SqlDbType.Int),
              };
            parameters[7].Direction = ParameterDirection.Output;
            parameters[7].Size = 8;
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Value = CanBoID;
            parameters[6].Value = p.Nam ?? Convert.DBNull;
            parameters[8].Value = p.LoaiDotKeKhai ?? Convert.DBNull;
            parameters[9].Value = p.TrangThai ?? Convert.DBNull;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KEKHAI_KEKHAI_GETPAGINGBYSEARCH, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModel item = new KeKhaiModel();
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        item.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        item.NamKeKhai = Utils.ConvertToInt32(dr[NAM], 0);
                        item.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI_BAN_KE_KHAI], 0);
                        item.TenTrangThai = getTenTrangThai(Utils.ConvertToInt32(dr[TRANG_THAI_BAN_KE_KHAI], 0));
                        item.LoaiDotKeKhaiID = Utils.ConvertToInt32(dr[LOAI_DOT_KE_KHAI], 0);
                        item.TenLoaiDotKeKhai = CommonDAL.getTenLoaiDotKeKhai(item.LoaiDotKeKhaiID);
                        item.DenNgay = Utils.ConvertToDateTime(dr[DEN_NGAY], DateTime.Now.Date);
                        item.TrangThaiDotKeKhai = Utils.ConvertToBoolean(dr["TrangThaiDotKeKhai"], false);
                        if (item.TrangThaiDotKeKhai == false || DateTime.Now.Date > item.DenNgay)
                            item.DongKeKhai = true;
                        else
                            item.DongKeKhai = false;
                        list.Add(item);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[7].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //add file kê khai và duyệt
            foreach (var item in list)
            {
                item.DanhSachFileDinhKem = new List<FileDinhKemModel>();
                item.DanhSachFileDuyetDinhKem = new List<FileDinhKemModel>();
                var DanhSachFileDinhKem = new FileDinhKemDAL().GetAllField_FileDinhKem_ByNghiepVuID_AndType(item.KeKhaiID, EnumLoaiFileDinhKem.FileKeKhai.GetHashCode()).Where(x => x.TrangThai > 0).ToList();
                if (DanhSachFileDinhKem.Count > 0)
                {
                    item.DanhSachFileDinhKem.Add(DanhSachFileDinhKem[0]);
                }
                var DuyetKeKhai = new QuanLyBanKeKhaiDAL().GetDuyetBanKeKhaiByKeKhaiID(item.KeKhaiID);
                if (DuyetKeKhai != null && DuyetKeKhai.DuyetBanKeKhaiID > 0)
                {
                    var DanhSachFileDuyetDinhKem = new FileDinhKemDAL().GetAllField_FileDinhKem_ByNghiepVuID_AndType(DuyetKeKhai.DuyetBanKeKhaiID, EnumLoaiFileDinhKem.FileDuyetKeKhai.GetHashCode()).Where(x => x.TrangThai > 0).ToList();
                    if (DanhSachFileDuyetDinhKem.Count > 0)
                    {
                        item.DanhSachFileDuyetDinhKem.Add(DanhSachFileDuyetDinhKem[0]);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// Xóa bản kê khai
        /// </summary>
        /// <param name="ListKeKhaiID"></param>
        /// <returns></returns>
        public BaseResultModel Delete_BanKeKhai(List<int> ListKeKhaiID)
        {
            var Result = new BaseResultModel();
            if (ListKeKhaiID.Count <= 0)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                for (int i = 0; i < ListKeKhaiID.Count; i++)
                {
                    int val = 0;
                    if (!int.TryParse(ListKeKhaiID[i].ToString(), out val))
                    {
                        Result.Status = 0;
                        Result.Message = "KeKhaiID '" + ListKeKhaiID[i].ToString() + "' không đúng định dạng";
                        return Result;
                    }
                    else
                    {
                        var crKeKhai = new KeKhaiDAL().GetByID(ListKeKhaiID[i]);
                        var crDotKeKhai = new DotKeKhaiDAL().GetByID(crKeKhai.DotKeKhaiID);

                        if (crKeKhai == null || crKeKhai.KeKhaiID < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "KeKhaiID '" + ListKeKhaiID[i].ToString() + "' không tồn tại";
                            return Result;
                        }
                        else if (crKeKhai.TrangThai >= 200)
                        {
                            Result.Status = 0;
                            Result.Message = "Bản kê khai đã được gửi đi, không thể xóa Kê Khai có KeKhaiId '" + ListKeKhaiID[i].ToString() + "'";
                            return Result;
                        }
                        else if (crDotKeKhai.TrangThai == false)
                        {
                            Result.Status = 0;
                            Result.Message = "Đợt kê khai đã hết thời hạn!";
                            return Result;
                        }
                        else
                        {
                            //xóa thông tin tài sản
                            var listThongTinTaiSan = new ThongTinTaiSanDAL().ThongTinTaiSan_GetByKeKhaiID(ListKeKhaiID[i]);
                            if (listThongTinTaiSan != null && listThongTinTaiSan.Count > 0)
                            {
                                Result = new ThongTinTaiSanDAL().DeleteAllByKeKhaiID(ListKeKhaiID[i]);
                            }
                            //xóa bản kê khai

                            SqlParameter[] parameters = new SqlParameter[]
                            {
                                new SqlParameter(KE_KHAI_ID_THONGTINTAISAN, SqlDbType.Int)
                            };
                            parameters[0].Value = ListKeKhaiID[i];
                            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                            {
                                conn.Open();
                                using (SqlTransaction trans = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE_KEKHAI, parameters);
                                        trans.Commit();
                                        if (val < 1)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Không thể xóa bản kê khai có KeKhaiID " + ListKeKhaiID[i];
                                            return Result;
                                        }
                                    }
                                    catch
                                    {
                                        Result.Status = -1;
                                        Result.Message = ConstantLogMessage.API_Error_System;
                                        trans.Rollback();
                                        return Result;
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Thông tin tài sản");
                return Result;
            }
        }

        // Lấy tên trạng thái bản kê khai
        public string getTenTrangThai(int trangThai)
        {
            switch (trangThai)
            {
                case 10:
                    {
                        return "Bản tạm";
                    }
                case 100:
                    {
                        return "Chưa gửi";
                    }
                case 200:
                    {
                        return "Đã gửi";
                    }
                case 101:
                    {
                        return "Kê khai lại";
                    }
                case 300:
                    {
                        return "Đã gủi";
                    }
                case 201:
                    {
                        return "Kê khai lại";
                    }
                case 400:
                case 500:
                    {
                        return "Hồ sơ pháp lý";
                    }

                default:
                    break;
            }
            return "";
        }

        public BaseResultModel GuiBanKeKhai(int BanKeKhaiID)
        {
            var Result = new BaseResultModel();
            try
            {
                int crID;
                if (!int.TryParse(BanKeKhaiID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "BanKeKhaiID không đúng định dạng";
                }
                else if (BanKeKhaiID == null || BanKeKhaiID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "BanKeKhaiID không được trống";
                }
                else
                {
                    var crObj = new KeKhaiDAL().GetByID(BanKeKhaiID);
                    if (crObj == null || crObj.KeKhaiID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Bản kê khai không tồn tại";
                    }
                    else if (crObj.TrangThai > 200)
                    {
                        Result.Status = 0;
                        Result.Message = "Bản kê khai đã được gửi đi hoặc duyệt!";
                    }
                    else
                    {
                        ////edited by ChungNN 20/1/2021
                        //int LanGui = new ThongTinTaiSanLogDAL().GetSoLanGuiBanKeKhai();
                        //if (crObj.TrangThai == 101)
                        //{
                        //    LanGui++;
                        //}
                        ///////////////////////////////////////////////////////////////////
                        SqlParameter[] parameters = new SqlParameter[]
                         {
                            new SqlParameter(KE_KHAI_ID_THONGTINTAISAN, SqlDbType.Int),
                         };
                        parameters[0].Value = BanKeKhaiID;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, KEKHAI_GUI_BANKEKHAI, parameters);
                                    trans.Commit();
                                    Result.Message = "Gửi bản kê khai thành công";
                                }
                                catch
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                    trans.Rollback();
                                    throw;
                                }
                            }
                        }
                        ////Edit by ChungNN 20/01/2021
                        //if (Result.Status > 0)
                        //{
                        //    List<ThongTinTaiSanModel> lstThongTinTaiSan = ThongTinTaiSan_GetByKeKhaiIDForInsertLog(BanKeKhaiID);
                        //    foreach (ThongTinTaiSanModel item in lstThongTinTaiSan)
                        //    {
                        //        int temp = new ThongTinTaiSanLogDAL().InsertEncrypt(item, 1, LanGui);
                        //    }
                        //}
                        //////////////////////////////////////////////////////////////////////////////////////////////////
                        if (Result.Status > 0)
                        {
                            try
                            {
                                ////thêm thông báo
                                //ThongBaoModel ThongBaoModel = new ThongBaoModel();
                                //ThongBaoModel.TieuDe = "Gửi hồ sơ kê khai";
                                //ThongBaoModel.NoiDung = crObj.TenBanKeKhai + " đã được gửi";
                                //ThongBaoModel.ThoiGianBatDau = DateTime.Now;
                                //ThongBaoModel.ThoiGianKetThuc = DateTime.Now;
                                //ThongBaoModel.LoaiThongBao = EnumLoaiThongBao.GuiHoSoKeKhai.GetHashCode();
                                //ThongBaoModel.NghiepVuID = crObj.KeKhaiID;
                                //ThongBaoModel.TenNghiepVu = crObj.TenBanKeKhai;
                                //ThongBaoModel.HienThi = true;
                                //ThongBaoModel.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                                ////lấy các đối tượng là thanh tra tỉnh
                                //var listThanhTraTinh = new SystemConfigDAL().GetByKey("Thanh_Tra_Tinh_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
                                //if (listThanhTraTinh != null && listThanhTraTinh.Count > 0)
                                //{
                                //    foreach (var CoQuanID in listThanhTraTinh)
                                //    {
                                //        var listCanBo = new HeThongCanBoDAL().GetAllCanBoByCoQuanID(CoQuanID, 0);
                                //        if (listCanBo != null && listCanBo.Count > 0)
                                //        {
                                //            foreach (var cb in listCanBo)
                                //            {
                                //                DoiTuongThongBaoModel dt = new DoiTuongThongBaoModel();
                                //                dt.CanBoID = cb.CanBoID;
                                //                dt.CoQuanID = cb.CoQuanID;
                                //                dt.TenCanBo = cb.TenCanBo;
                                //                dt.GioiTinh = cb.GioiTinh;
                                //                dt.Email = cb.Email;
                                //                ThongBaoModel.DoiTuongThongBao.Add(dt);
                                //            }
                                //        }
                                //    }
                                //}

                                //new NhacViecDAL().Edit_ThongBao(ThongBaoModel);

                                //thông báo cho lãnh đạo đơn vị
                                var CanBo = new HeThongCanBoDAL().GetCanBoByID(crObj.CanBoID);
                                ThongBaoModel TBDuyetCongKhai = new ThongBaoModel();
                                TBDuyetCongKhai.TieuDe = "Thông báo duyệt công khai hồ sơ kê khai";
                                TBDuyetCongKhai.NoiDung = crObj.TenBanKeKhai + " cần duyệt công khai tại đơn vị";
                                TBDuyetCongKhai.ThoiGianBatDau = DateTime.Now;
                                TBDuyetCongKhai.ThoiGianKetThuc = DateTime.Now;
                                TBDuyetCongKhai.LoaiThongBao = EnumLoaiThongBao.DuyetCongKhai.GetHashCode();
                                TBDuyetCongKhai.NghiepVuID = crObj.KeKhaiID;
                                TBDuyetCongKhai.TenNghiepVu = crObj.TenBanKeKhai;
                                TBDuyetCongKhai.HienThi = true;
                                TBDuyetCongKhai.DoiTuongThongBao = new List<DoiTuongThongBaoModel>();
                                //lấy đối tượng là lãnh đạo đơn vị
                                var listCanBo = new HeThongCanBoDAL().GetCanBoByChucNang(CanBo.CoQuanID, ChucNangEnum.KeKhai_DuyetKeKhaiCongKhai.GetHashCode());
                                if (listCanBo != null && listCanBo.Count > 0)
                                {
                                    foreach (var cb in listCanBo)
                                    {
                                        DoiTuongThongBaoModel doituong = new DoiTuongThongBaoModel();
                                        doituong.CanBoID = cb.CanBoID;
                                        doituong.CoQuanID = cb.CoQuanID;
                                        doituong.TenCanBo = cb.TenCanBo;
                                        doituong.GioiTinh = cb.GioiTinh;
                                        doituong.Email = cb.Email;
                                        TBDuyetCongKhai.DoiTuongThongBao.Add(doituong);
                                    }
                                }
                                new NhacViecDAL().Edit_ThongBao(TBDuyetCongKhai);

                            }
                            catch (Exception)
                            {
                                //throw;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }
        #endregion

        /// <summary>
        /// /////////////
        /// </summary>
        /// <param name=KE_KHAI_ID_THONGTINTAISAN></param>
        /// <returns></returns>
        public List<NguoiDungTenModel> GetAllNguoiDungTen(int KeKhaiID)
        {
            List<NguoiDungTenModel> List = new List<NguoiDungTenModel>();
            try
            {
                List = (from c in (new KeKhaiThanNhanDAL().GetAll())
                        where c.CanBoID == new KeKhaiTaiSanDAL().GetByID(KeKhaiID).CanBoID
                        select new NguoiDungTenModel
                        {
                            ID = int.Parse(string.Concat("2", c.ThanNhanID.ToString())),
                            Ten = c.HoTen
                        }).ToList();
                var CanBoID = string.Concat("1", new HeThongCanBoDAL().GetCanBoByID(new KeKhaiTaiSanDAL().GetByID(KeKhaiID).CanBoID).ToString());
                var TenCanBo = new HeThongCanBoDAL().GetCanBoByID(new KeKhaiTaiSanDAL().GetByID(KeKhaiID).CanBoID).TenCanBo.ToString();
                List.Add(new NguoiDungTenModel
                {
                    ID = int.Parse(CanBoID),
                    Ten = TenCanBo
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return List;
        }

        public List<ThongTinTaiSanModel> GetAll()
        {

            List<ThongTinTaiSanModel> List = new List<ThongTinTaiSanModel>();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KEKHAI_THONGTINKEKHAI_GETALL))
                {
                    while (dr.Read())
                    {
                        ThongTinTaiSanModel item = new ThongTinTaiSanModel();
                        item.ThongTinTaiSanID = Utils.ConvertToInt32(dr[THONG_TIN_TAI_SAN_ID], 0);
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID_THONGTINTAISAN], 0);
                        item.NhomTaiSanID = Utils.ConvertToInt32(dr[NHOM_TAI_SAN_ID], 0);
                        item.TenTaiSan = Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[TEN_TAI_SAN], string.Empty));
                        item.DienTich = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[DIEN_TICH], string.Empty)), null);
                        item.GiaTri = Utils.ConvertToNullAbleDouble(Encrypt_Decrypt.DecryptString_Aes(Utils.ConvertToString(dr[GIA_TRI], string.Empty)), null);
                        item.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);

                        List.Add(item);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return List;
        }

        public CheckKeKhaiTaiSan CheckKeKhaiTaiSan(int CanBoID)
        {
            try
            {
                var Result = new CheckKeKhaiTaiSan();
                //var listDotKeKhai = new DotKeKhaiDAL().GetByNamKeKhai(DateTime.Now.Year)
                //                   .Where(x => x.TuNgay <= DateTime.Now.Date
                //                   && x.DenNgay >= DateTime.Now.Date
                //                   && x.TrangThai == true
                //                   && x.CanBoID == CanBoID)
                //                   .ToList();
                var listDotKeKhai = new DotKeKhaiDAL().GetByCanBoID_NamKeKhai(DateTime.Now.Year, CanBoID);

                // check có được kê khai hay không
                if (listDotKeKhai.Count > 0)
                {
                    Result.KeKhai = true; //Dùng để xác định có đợt kê khai hay không
                    // check có được thêm kê khai hay không
                    var BanKeKhai = new KeKhaiDAL().GetByDotKeKhaiIDAndCanBoID(listDotKeKhai.FirstOrDefault().DotKeKhaiID, listDotKeKhai.FirstOrDefault().CanBoID.Value);
                    if (BanKeKhai != null && BanKeKhai.KeKhaiID > 0)
                    {
                        Result.ThemKeKhai = false;
                        if (BanKeKhai.TrangThai == 10)
                        { 
                            Result.TrangThaiBanKeKhai = 2;
                        }
                        else Result.TrangThaiBanKeKhai = 3;
                        Result.KeKhaiID = BanKeKhai.KeKhaiID;
                    }
                    else
                    {
                        Result.ThemKeKhai = true;
                        Result.TrangThaiBanKeKhai = 1;
                    }
                    Result.LoaiDotKeKhaiID = listDotKeKhai.FirstOrDefault().LoaiDotKeKhai;
                    Result.TenDotLoaiKeKhai = CommonDAL.getTenLoaiDotKeKhai(listDotKeKhai.FirstOrDefault().LoaiDotKeKhai);
                }
                else
                {
                    Result.KeKhai = false;
                    Result.ThemKeKhai = false;
                }

                return Result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // lấy Thong tin tai san của bản kê khai cuối cùng
        public List<ThongTinTaiSanModelPartial> GetLastThongTinTaiSan(int CanBoID)
        {
            List<ThongTinTaiSanModelPartial> ListThongTinTaiSan = new List<ThongTinTaiSanModelPartial>();
            try
            {
                var listTaiSan = ThongTinTaiSan_GetAll_By_CanBoID(CanBoID);
                if (listTaiSan.Count <= 0)
                {
                    return ListThongTinTaiSan;
                }
                var TaiSanGanNhat = listTaiSan.OrderByDescending(x => x.NamKeKhai).OrderByDescending(x => x.KeKhaiID).FirstOrDefault();
                ListThongTinTaiSan = listTaiSan.Where(x => x.NamKeKhai == TaiSanGanNhat.NamKeKhai && x.KeKhaiID == TaiSanGanNhat.KeKhaiID.Value).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListThongTinTaiSan;
        }

        public List<ThongTinTaiSanModelPartial> GetLastThongTinTaiSan1(int CanBoID)
        {
            List<ThongTinTaiSanModelPartial> ListThongTinTaiSan = new List<ThongTinTaiSanModelPartial>();
            try
            {
                var listTaiSan = ThongTinTaiSan_GetAll_By_CanBoID(CanBoID);
                if (listTaiSan.Count <= 0)
                {
                    return ListThongTinTaiSan;
                }
                var listNamKeKhai = new List<int>();
                listNamKeKhai.AddRange(listTaiSan.Where(x => !listNamKeKhai.Contains(x.NamKeKhai)).Select(y => y.NamKeKhai));
                listNamKeKhai.OrderBy(x => x);
                var listNhomTaiSan = new DanhMucNhomTaiSanDAL().GetAllNhomTaiSanCha();
                for (int i = 0; i < listNhomTaiSan.Count; i++)
                {
                    List<int> list = new List<int>();
                    var GetLastThongTinTaiSan = listTaiSan.Where(x => (x.NamKeKhai == listNamKeKhai.FirstOrDefault() && (x.NhomTaiSanChaID == listNhomTaiSan[i].NhomTaiSanID || x.NhomTaiSanID == listNhomTaiSan[i].NhomTaiSanID) && !list.Contains(x.NhomTaiSanID.Value))).ToList().OrderBy(x => x.NhomTaiSanID).Distinct().LastOrDefault();
                    if (GetLastThongTinTaiSan == null)
                    {
                        ThongTinTaiSanModelPartial ThongTinTaiSanModel = new ThongTinTaiSanModelPartial();
                        ThongTinTaiSanModel.ThongTinTaiSanID = listNhomTaiSan[i].NhomTaiSanID;
                        ThongTinTaiSanModel.TenTaiSan = listNhomTaiSan[i].TenNhomTaiSan;
                        ThongTinTaiSanModel.NamKeKhai = listNamKeKhai.FirstOrDefault();
                        ThongTinTaiSanModel.CanBoID = CanBoID;
                        ThongTinTaiSanModel.KeKhaiID = listTaiSan.Where(x => (x.NamKeKhai == listNamKeKhai.FirstOrDefault() && (x.KeKhaiID != 0 || x.KeKhaiID != null))).ToList().FirstOrDefault().KeKhaiID;
                        ThongTinTaiSanModel.NguoiDungTen = CanBoID;
                        ThongTinTaiSanModel.NguoiDungTenLaCanBo = listTaiSan.Where(x => (x.NamKeKhai == listNamKeKhai.FirstOrDefault() && (x.KeKhaiID != 0 || x.KeKhaiID != null))).ToList().FirstOrDefault().NguoiDungTenLaCanBo;
                        ThongTinTaiSanModel.NhomTaiSanID = listNhomTaiSan[i].NhomTaiSanID;
                        ThongTinTaiSanModel.NhomTaiSanChaID = listNhomTaiSan[i].NhomTaiSanChaID;
                        ListThongTinTaiSan.Add(ThongTinTaiSanModel);
                        list.Add(listNhomTaiSan[i].NhomTaiSanID);
                    }
                    else
                    {
                        ListThongTinTaiSan.Add(GetLastThongTinTaiSan);
                        list.Add(listNhomTaiSan[i].NhomTaiSanID);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListThongTinTaiSan;
        }

        public List<ThongTinTaiSanModelPartial> GetTaiSanTangThem(List<ThongTinTaiSanModelPartial> TaiSanHienTai, List<ThongTinTaiSanModelPartial> TaiSanGanNhat)
        {
            List<int> ListFilterLoaiTaiSan = new List<int> { 16, 17, 1, 2, 3, 12, 13, 14, 15 };
            List<int> ListFilterNhomTaiSan = new List<int> { 1, 11, 12, 13, 2, 6, 10, 9 };
            var result = new List<ThongTinTaiSanModelPartial>();
            //So sánh biến động với Nhóm tài sản
            foreach (int NhomTaiSanID in ListFilterNhomTaiSan)
            {
                var nhomtaisanhientai = TaiSanHienTai.Where(x => x.NhomTaiSanID == NhomTaiSanID).ToList();
                var nhomtaisangannhat = TaiSanGanNhat.Where(x => x.NhomTaiSanID == NhomTaiSanID).ToList();
                if (nhomtaisanhientai.Count > nhomtaisangannhat.Count)
                {
                    result.Add(nhomtaisanhientai.OrderByDescending(x => x.ThongTinTaiSanID).FirstOrDefault());
                }
            }
            //So sánh biến động với Loại tài sản
            foreach (int LoaiTaiSanID in ListFilterLoaiTaiSan)
            {
                var loaitaisanhientai = TaiSanHienTai.Where(x => x.LoaiTaiSanID == LoaiTaiSanID).ToList();
                var loaitaisangannhat = TaiSanGanNhat.Where(x => x.LoaiTaiSanID == LoaiTaiSanID).ToList();
                if (loaitaisanhientai.Count > loaitaisangannhat.Count)
                {
                    loaitaisanhientai = loaitaisanhientai.OrderByDescending(x => x.ThongTinTaiSanID).ToList();
                    int slTang = loaitaisanhientai.Count - loaitaisangannhat.Count;
                    for (int i = 0; i < slTang; i++)
                    {
                        result.Add(loaitaisanhientai[i]);
                    }
                }
            }
            return result;
        }

        public BaseResultModel GetImages(string filename)
        {
            var Result = new BaseResultModel();
            //int pageNum = 1;

            //PdfReader pdf = new PdfReader(filename);
            //PdfDictionary pg = pdf.GetPageN(pageNum);
            //PdfDictionary res = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            //PdfDictionary xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
            //if (xobj == null) {
            //    Result.Status = 0;
            //    return Result; 
            //}
            //foreach (PdfName name in xobj.Keys)
            //{
            //    PdfObject obj = xobj.Get(name);
            //    if (!obj.IsIndirect()) { continue; }
            //    PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
            //    PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
            //    if (!type.Equals(PdfName.IMAGE)) { continue; }
            //    int XrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
            //    PdfObject pdfObj = pdf.GetPdfObject(XrefIndex);
            //    PdfStream pdfStrem = (PdfStream)pdfObj;
            //    byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
            //    if (bytes == null) { continue; }
            //    using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(bytes))
            //    {
            //        memStream.Position = 0;
            //        System.Drawing.Image img = System.Drawing.Image.FromStream(memStream);

            //        string path = Path.Combine(String.Format(@"result-{0}.jpg", pageNum));
            //        System.Drawing.Imaging.EncoderParameters parms = new System.Drawing.Imaging.EncoderParameters(1);
            //        parms.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 0);
            //        var jpegEncoder = ImageCodecInfo.GetImageEncoders().ToList().Find(x => x.FormatID == ImageFormat.Jpeg.Guid);
            //        img.Save(path, jpegEncoder, parms);
            //        Result.Data = path;
            //    }
            //}
            Result.Status = 1;
            return Result;
        }

        public BaseResultModel UpdateBarcode(KeKhaiModel KeKhaiModel)
        {
            var Result = new KeKhaiDAL().UpdateBarcode(KeKhaiModel);
            return Result;
        }

        public int CheckBarcode(int KeKhaiID, string Barcode)
        {
            var ID = 0;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("KeKhaiID", SqlDbType.Int),
                new SqlParameter("Barcode", SqlDbType.NVarChar),
            };
            parameters[0].Value = KeKhaiID;
            parameters[1].Value = Barcode;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, CHECK_BARCODE, parameters))
                {
                    while (dr.Read())
                    {
                        ID = Utils.ConvertToInt32(dr["ID"], 0);
                    }
                    dr.Close();
                }
                return ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

