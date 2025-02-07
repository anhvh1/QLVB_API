using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Models.KeKhai;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Com.Gosol.QLVB.DAL.KeKhai
{
    public interface IDuyetCongKhaiBanKeKhaiDAL
    {
        //public List<KeKhaiModel> GetListTaiSan();
        public List<KeKhaiModelPartial> GetPagingBySearch(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, int NguoiDungID, ref int TotalRow);
        public BaseResultModel DuyetCongKhaiBanKeKhai(DuyetCongKhaiBanKeKhaiPartial objInsert, int CanBoID, int CoQuanID);
        public ChiTietCongKhaiBanKeKhai ChieTietCongKhaiBanKeKhai(int KeKhaiID);
        public BaseResultModel ThemCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objInsert, int CanBoID, int CoQuanID);
        public BaseResultModel XoaCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objDelete, int CanBoID, int CoQuanID);
        public BaseResultModel CapNhatCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objInsert, int CanBoID, int CoQuanID);
        public BaseResultModel HuyDuyetCongKhaiBanKeKhai(int? KeKhaiID);
        public BaseResultModel CapNhatTrangThaiNhacViec(int? DuyetBanKeKhaiID);
        public BaseResultModel CapNhatTrangThaiNhacViec_Multi(List<int> DanhSachDuyetKeKhaiID);
    }
    public class DuyetCongKhaiBanKeKhaiDAL : IDuyetCongKhaiBanKeKhaiDAL
    {
        //tên các store procedure
        private const string KEKHAI_BANKEKHAI_GET_PAGING_BY_SEARCH = @"v1_KeKhai_BanKeKhai_GetPagingBySearch_New";
        private const string KEKHAI_BANKEKHAI_GET_LIST_DUYETCONGKHAI = @"v1_KeKhai_BanKeKhai_GetListDuyetCongKhai";
        private const string KEKHAI_DUYETCONGKHAI = @"v1_KeKhai_DuyetCongKhaiBanKeKhai";
        private const string KEKHAI_DUYETCONGKHAI_THEMCANBOXEMBANKEKHAI = @"v1_KeKhai_DuyetCongKhaiBanKeKhai_ThemCanBoXemBanKeKhai";
        private const string KEKHAI_DUYETBANKEKHAI_GET_LIST_XOA_CANBOXEM = @"v1_KeKhai_DuyetCongKhaiBanKeKhai_XoaCanBoXemKeKhai";
        private const string KEKHAI_DUYETCONGKHAI_CAPNHATCANBOXEM = @"v1_KeKhai_DuyetCongKhaiBanKeKhai_CapNhatCanBoXemBanKeKhai";
        private const string KEKHAI_DUYETBANKEKHAI_GET_LIST_CANBOXEM_BY_KEKHAIID = @"v1_KeKhai_DuyetBanKeKhai_GetListCanBoXem_ByKeKhaiID";
        private const string KEKHAI_CAPNHAT_DUYETCONGKHAI = @"v1_KeKhai_CapNhatDuyetCongKhaiBanKeKhai";
        private const string KEKHAI_CONGKHAIBANKEKHAI_HUYCONGKHAI = @"v1_KeKhai_CongKhaiBanKeKhai_HuyCongKhai";
        private const string UPDATE_DUYETBANKEKHAI = @"v1_UpdateDuyetBanKeKhai";
        private const string UPDATE_DUYETBANKEKHAI_MULTIL = @"v1_UpdateDuyetBanKeKhai_Multi";


        //Ten các params
        private const string KE_KHAI_ID = "NV00301";
        private const string DOT_KE_KHAI_ID = "NV00302";
        private const string CAN_BO_ID = "NV00303";
        private const string NAM = "NV00304";
        private const string TRANG_THAI = "NV00305";
        private const string TEN_BAN_KE_KHAI = "NV00306";
        private const string BIEN_DONG = "NV00307";
        private const string TRANG_THAI_NHAC_VIEC_DASBOARD = "NV00308";
        private const string TRANG_THAI_CONG_KHAI = "NV00309";
        private const string LOAI_DOT_KE_KHAI = "NV00106";
        private const string DUYET_BAN_KE_KHAI_ID = "NV00501";
        private const string NGUOI_DUYET_ID = "NV00502";
        private const string NGAY_DUYET = "NV00503";
        private const string GHI_CHU = "NV00504";
        private const string DUYETBANKEKHAI_KEKHAIID = "NV00505";
        private const string PHE_DUYET = "NV00506";
        private const string TRANG_THAI_DUYET = "NV00507";
        private const string TRANG_THAI_NHAC_VIEC = "NV00508";
        private const string CONG_KHAI_BAN_KE_KHAI_BAN_KE_KHAI_ID = "NV00801";
        private const string CONG_KHAI_BAN_KE_KHAI_ID = "NV00701";
        private const string NGUOI_DUYET_ID_CONG_KHAI = "NV00702";
        private const string NGAY_DUYET_CONG_KHAI = "NV00703";
        private const string NGAY_HET_HAN_CONG_KHAI = "NV00704";
        private const string TRANG_THAI_CUA_CONG_KHAI = "NV00705";
        private const string GHI_CHU_CUA_CONG_KHAI = "NV00706";
        private const string KEKHAIID_CONG_KHAI = "NV00802";
        private const string CONG_KHAI_BAN_KE_KHAI_ID_CANBOXEMKEKHAI = "NV00901";

        /// <summary>
        /// danh sách bản kê khai được phép kê khai
        /// </summary>
        /// <param name="p"></param>
        /// <param name="CoQuanID"></param>
        /// <param name=CanBoID></param>
        /// <param name="TotalRow"></param>
        /// <returns></returns>
        public List<KeKhaiModelPartial> GetPagingBySearch(BasePagingParamsForFilter p, int CoQuanID, int CanBoID, int NguoiDungID, ref int TotalRow)
        {
            Boolean quyenDuyetCongKhai = new PhanQuyenDAL().CheckQuyenDuyetCongKhaiBanKeKhai(CanBoID);

            new CongKhaiBanKeKhaiDAL().CheckHetHanCongKhaiBanKeKhai();
            List<KeKhaiModelPartial> list = new List<KeKhaiModelPartial>();
            var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            // var TrangThai = 400;
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            List<int> listCoQuan = new List<int>();
            if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())
            {
                listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanID).Select(x => x.CoQuanID).ToList();
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())
            {
                var coQuanCha = new DanhMucCoQuanDonViDAL().GetByID(crCoQuan.CoQuanChaID);
                if (coQuanCha.CapID == EnumCapCoQuan.CapSo.GetHashCode())
                {
                    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID).Select(x => x.CoQuanID).ToList();
                }
                else listCoQuan.Add(crCoQuan.CoQuanID);
            }
            else
            {
                listCoQuan.Add(crCoQuan.CoQuanID);
            }

            if (UserRole.CheckAdmin(NguoiDungID))
            {
                listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).Select(x => x.CoQuanID).ToList();
            }

            var listCanBoAll = new HeThongCanBoDAL().GetAllByListCoQuanID(listCoQuan);
            if (p.CoQuanID != null && p.CoQuanID > 0)
            {
                listCanBoAll = new HeThongCanBoDAL().GetListByCoQuanID(p.CoQuanID.Value);
            }

            listCanBoAll.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            //////////////////////////////////
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("Keyword",SqlDbType.NVarChar),
                new SqlParameter("OrderByName",SqlDbType.NVarChar),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),
                new SqlParameter(CAN_BO_ID,SqlDbType.Int),
                new SqlParameter(NAM,SqlDbType.Int),
                new SqlParameter(TRANG_THAI,SqlDbType.Int),
                pList,
                 new SqlParameter("@CanBoIDDangNhap",SqlDbType.Int)
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.CanBoID ?? Convert.DBNull;
            parameters[7].Value = p.NamKeKhai ?? Convert.DBNull;
            parameters[8].Value = p.TrangThai ?? Convert.DBNull;
            parameters[9].Value = tbCanBoID;
            parameters[10].Value = CanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KEKHAI_BANKEKHAI_GET_PAGING_BY_SEARCH, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModelPartial item = new KeKhaiModelPartial();
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        item.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        item.NamKeKhai = Utils.ConvertToInt32(dr[NAM], 0);
                        item.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        //item.TenTrangThai = item.TrangThai == 500 ? "Đã duyệt" : "Chưa duyệt";
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        item.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        item.CongKhaiBanKeKhaiID = Utils.ConvertToInt32(dr[CONG_KHAI_BAN_KE_KHAI_BAN_KE_KHAI_ID], 0);
                        item.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        item.CapCoQuan = Utils.ConvertToInt32(dr["CapId"], 0);
                        item.NgayDuyet = Utils.ConvertToDateTime(dr[NGAY_DUYET_CONG_KHAI], DateTime.MinValue);
                        item.NgayHetHan = Utils.ConvertToDateTime(dr[NGAY_HET_HAN_CONG_KHAI], DateTime.MinValue);
                        item.isPheDuyet = false;
                        if (item.NgayDuyet <= DateTime.Now.Date && item.NgayHetHan >= DateTime.Now.Date && item.CongKhaiBanKeKhaiID != 0)
                        {
                            item.TenTrangThai = "Đã công khai";
                        }
                        else
                        {
                            item.TenTrangThai = "Chưa công khai";
                            //trường hợp có quyền duyệt công khai
                            if (quyenDuyetCongKhai)
                            {
                                item.isPheDuyet = true;
                            }
                        }

                        item.DanhSachFileDinhKem = new FileDinhKemDAL().GetListFileDinhKemByKeKhaiID(item.KeKhaiID).Where(x => x.TrangThai > 0).ToList();
                        list.Add(item);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        // Get Duyệt công khai không phân trang
        public List<KeKhaiModelPartial> GetListDuyetCongKhai(int CoQuanID, int CanBoID)
        {
            List<KeKhaiModelPartial> list = new List<KeKhaiModelPartial>();
            var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            // var TrangThai = 400;
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            var CapQuanLy = 2;
            var CoQuanQuanLy = 0;

            var QuyenDuyet = new PhanQuyenDAL().CheckQuyenDuyetBanKeKhai(CanBoID);

            var listCanBoAll = new HeThongCanBoDAL().GetAllByListCoQuanID(new List<int>() { CoQuanID });
            listCanBoAll.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            //////////////////////////////////
            SqlParameter[] parameters = new SqlParameter[]
              {

                pList

              };
            parameters[0].Value = tbCanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KEKHAI_BANKEKHAI_GET_LIST_DUYETCONGKHAI, parameters))
                {
                    while (dr.Read())
                    {
                        KeKhaiModelPartial item = new KeKhaiModelPartial();
                        item.KeKhaiID = Utils.ConvertToInt32(dr[KE_KHAI_ID], 0);
                        item.DotKeKhaiID = Utils.ConvertToInt32(dr[DOT_KE_KHAI_ID], 0);
                        item.TenBanKeKhai = Utils.ConvertToString(dr[TEN_BAN_KE_KHAI], string.Empty);
                        item.NamKeKhai = Utils.ConvertToInt32(dr[NAM], 0);
                        item.TrangThai = Utils.ConvertToInt32(dr[TRANG_THAI], 0);
                        item.TenTrangThai = item.TrangThai == 500 ? "Đã duyệt" : "Chưa duyệt";
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        item.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        item.CongKhaiBanKeKhaiID = Utils.ConvertToInt32(dr[CONG_KHAI_BAN_KE_KHAI_BAN_KE_KHAI_ID], 0);
                        item.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        item.CapCoQuan = Utils.ConvertToInt32(dr["CapId"], 0);
                        if (UserRole.CheckAdmin(CanBoID))
                        {
                            item.isPheDuyet = false;
                            if (item.TrangThai == 500) item.TenTrangThai = "Đã công khai";
                            else item.TenTrangThai = "Chưa công khai";
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
                        {
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
                        {
                            if (item.TrangThai == 400)
                            {
                                if (item.CapQuanLy == 1) // tỉnh
                                {
                                    item.isPheDuyet = true;
                                }
                                else if (item.CapQuanLy == 2)//huyện
                                {
                                    item.isPheDuyet = false;
                                }
                                item.TenTrangThai = "Chưa công khai";
                            }
                            else
                            {
                                if (item.CapQuanLy == 1) // tỉnh
                                {
                                    item.isPheDuyet = true;
                                }
                                else if (item.CapQuanLy == 2)//huyện
                                {
                                    item.isPheDuyet = false;
                                }
                                item.TenTrangThai = "Đã công khai";

                            }
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
                        {
                            if (item.TrangThai == 400)
                            {
                                item.TenTrangThai = "Chưa công khai";
                            }
                            else
                            {
                                item.TenTrangThai = "Đã công khai";
                            }
                            item.isPheDuyet = false;
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
                        {
                            if (item.TrangThai == 400)
                            {
                                if (item.CapQuanLy == 2
                                    && (item.CapCoQuan == EnumCapCoQuan.CapHuyen.GetHashCode()
                                        || item.CapCoQuan == EnumCapCoQuan.CapPhong.GetHashCode()))//huyện
                                {
                                    item.isPheDuyet = true;
                                }
                                else
                                    item.isPheDuyet = false;
                                item.TenTrangThai = "Chưa công khai";
                            }
                            else
                            {
                                item.TenTrangThai = "Đã công khai";
                                if (item.CapQuanLy == 2
                                    && (item.CapCoQuan == EnumCapCoQuan.CapHuyen.GetHashCode()
                                        || item.CapCoQuan == EnumCapCoQuan.CapPhong.GetHashCode()))//huyện
                                {
                                    item.isPheDuyet = true;
                                }
                                else
                                    item.isPheDuyet = false;
                            }
                        }
                        else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
                        {
                            if (item.TrangThai == 400)
                            {
                                item.TenTrangThai = "Chưa công khai";
                            }
                            else
                            {
                                item.TenTrangThai = "Đã công khai";
                            }
                            item.isPheDuyet = false;
                        }
                        else                                                             // cấp xã
                        {
                            if (item.TrangThai == 400)
                            {
                                if (item.CapQuanLy == 2
                                    && item.CapCoQuan == EnumCapCoQuan.CapXa.GetHashCode())//huyện
                                {

                                    item.isPheDuyet = true;
                                }
                                else
                                    item.isPheDuyet = false;
                                item.TenTrangThai = "Chưa công khai";
                            }
                            else
                            {
                                item.TenTrangThai = "Đã công khai";
                                if (item.CapQuanLy == 2
                                    && item.CapCoQuan == EnumCapCoQuan.CapXa.GetHashCode())//huyện
                                {

                                    item.isPheDuyet = true;
                                }
                                else
                                    item.isPheDuyet = false;
                            }
                        }
                        if (!QuyenDuyet)
                        {
                            item.isPheDuyet = false;
                        }
                        item.DanhSachFileDinhKem = new FileDinhKemDAL().GetListFileDinhKemByKeKhaiID(item.KeKhaiID).Where(x => x.TrangThai > 0).ToList();
                        list.Add(item);

                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public BaseResultModel DuyetCongKhaiBanKeKhai(DuyetCongKhaiBanKeKhaiPartial objInsert, int CanBoID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (!(new PhanQuyenDAL().CheckQuyenDuyetCongKhaiBanKeKhai(CanBoID)))
                {
                    Result.Status = 0;
                    Result.Message = "Người sử dụng không có quyền thực hiện chức năng này";
                    return Result;
                }
                //duyệt mới
                if (objInsert.DuyetBanKeKhaiCongKhai.CongKhaiBanKeKhaiID == null || objInsert.DuyetBanKeKhaiCongKhai.CongKhaiBanKeKhaiID < 1)
                {
                    if (objInsert == null)
                    {
                        Result.Status = 0;
                        Result.Message = "Vui lòng nhập dữ liệu trước khi duyệt";
                        return Result;
                    }
                    else if (objInsert.DuyetBanKeKhaiCongKhai.NgayDuyet == null)
                    {
                        Result.Status = 0;
                        Result.Message = "Ngày duyệt không được trống";
                        return Result;
                    }
                    else if (objInsert.DuyetBanKeKhaiCongKhai.NgayHetHan == null)
                    {
                        Result.Status = 0;
                        Result.Message = "Ngày hết hạn không được trống";
                        return Result;
                    }
                    else if (objInsert.DuyetBanKeKhaiCongKhai.TrangThai == null)
                    {
                        Result.Status = 0;
                        Result.Message = "Trạng thái không được trống";
                        return Result;
                    }
                    else if (objInsert.DanhSachBanKeKhaiID == null || objInsert.DanhSachBanKeKhaiID.Count < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Vui lòng chọn bản kê khai để công khai";
                        return Result;
                    }
                    else if (objInsert.DanhSachCanBoXemBanKeKhaiID == null || objInsert.DanhSachCanBoXemBanKeKhaiID.Count < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Vui lòng chọn cán bộ xem bản kê khai";
                        return Result;
                    }
                    else
                    {
                        // check tồn tại của cán bộ
                        var hethongcanboDAL = new HeThongCanBoDAL();
                        var banKeKhaiDAL = new KeKhaiDAL();
                        if (objInsert.DanhSachCanBoXemBanKeKhaiID.Any(
                            x => hethongcanboDAL.GetCanBoByID(x) == null)
                            || objInsert.DanhSachCanBoXemBanKeKhaiID.Any(x => hethongcanboDAL.GetCanBoByID(x).CanBoID < 1))
                        {
                            Result.Status = 0;
                            Result.Message = "Cán bộ không tồn tại";
                            return Result;
                        }
                        if (objInsert.DanhSachBanKeKhaiID.Any(
                            x => banKeKhaiDAL.GetByID(x) == null)
                            || objInsert.DanhSachBanKeKhaiID.Any(x => banKeKhaiDAL.GetByID(x).KeKhaiID < 1))
                        {
                            Result.Status = 0;
                            Result.Message = "Bản kê khai không tồn tại";
                            return Result;
                        }
                        var crkk = banKeKhaiDAL.GetByID(objInsert.DanhSachBanKeKhaiID[0]);
                        // tạo params @DanhSachKeKhaiID
                        var tbBanKeKhai = new DataTable();
                        tbBanKeKhai.Columns.Add("ID", typeof(string));
                        objInsert.DanhSachBanKeKhaiID.ForEach(x => tbBanKeKhai.Rows.Add(x));
                        var pListBanKeKhai = new SqlParameter("@DanhSachKeKhaiID", SqlDbType.Structured);
                        pListBanKeKhai.TypeName = "dbo.list_ID";

                        // tạo params @DanhSachCanBoID 
                        var tbCanBoXemBanKeKhai = new DataTable();
                        tbCanBoXemBanKeKhai.Columns.Add("ID", typeof(string));
                        objInsert.DanhSachCanBoXemBanKeKhaiID.ForEach(x => tbCanBoXemBanKeKhai.Rows.Add(x));
                        var pListCanBoXemBanKeKhai = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
                        pListCanBoXemBanKeKhai.TypeName = "dbo.list_ID";
                        SqlParameter[] parameters = new SqlParameter[]
                            {
                            new SqlParameter(NGUOI_DUYET_ID_CONG_KHAI, SqlDbType.Int),
                            new SqlParameter(NGAY_DUYET_CONG_KHAI, SqlDbType.DateTime2),
                            new SqlParameter(GHI_CHU_CUA_CONG_KHAI, SqlDbType.NText),
                            pListBanKeKhai,
                            pListCanBoXemBanKeKhai,
                            new SqlParameter(NGAY_HET_HAN_CONG_KHAI, SqlDbType.DateTime2),
                            new SqlParameter(TRANG_THAI_CUA_CONG_KHAI, SqlDbType.Bit)
                            };
                        parameters[0].Value = CanBoID;
                        parameters[1].Value = objInsert.DuyetBanKeKhaiCongKhai.NgayDuyet;
                        parameters[2].Value = objInsert.DuyetBanKeKhaiCongKhai.GhiChu ?? Convert.DBNull;
                        parameters[3].Value = tbBanKeKhai;
                        parameters[4].Value = tbCanBoXemBanKeKhai;
                        parameters[5].Value = objInsert.DuyetBanKeKhaiCongKhai.NgayHetHan;
                        parameters[6].Value = objInsert.DuyetBanKeKhaiCongKhai.TrangThai;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            int query = 0;
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    query = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, KEKHAI_DUYETCONGKHAI, parameters);
                                    trans.Commit();
                                    if (query > 0)
                                    {
                                        Result.Status = 1;
                                        Result.Message = ConstantLogMessage.Alert_Insert_Success("Duyệt công khai bản kê khai");
                                        new NhacViecDAL().UpdateTrangThaiHienThiThongBao(EnumLoaiThongBao.DuyetCongKhai.GetHashCode(), objInsert.DanhSachBanKeKhaiID);
                                    }
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
                }
                else  //cập nhật
                {
                    if (objInsert == null)
                    {
                        Result.Status = 0;
                        Result.Message = "Vui lòng nhập dữ liệu trước khi duyệt";
                        return Result;
                    }
                    else if (objInsert.DuyetBanKeKhaiCongKhai.NgayDuyet == null)
                    {
                        Result.Status = 0;
                        Result.Message = "Ngày duyệt không được trống";
                        return Result;
                    }
                    else if (objInsert.DuyetBanKeKhaiCongKhai.NgayHetHan == null)
                    {
                        Result.Status = 0;
                        Result.Message = "Ngày hết hạn không được trống";
                        return Result;
                    }
                    else if (objInsert.DuyetBanKeKhaiCongKhai.TrangThai == null)
                    {
                        Result.Status = 0;
                        Result.Message = "Trạng thái không được trống";
                        return Result;
                    }
                    else if (objInsert.DanhSachBanKeKhaiID == null || objInsert.DanhSachBanKeKhaiID.Count < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Vui lòng chọn bản kê khai để công khai";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                           {
                            new SqlParameter(CONG_KHAI_BAN_KE_KHAI_ID, SqlDbType.Int),
                            new SqlParameter(NGUOI_DUYET_ID_CONG_KHAI, SqlDbType.Int),
                            new SqlParameter(NGAY_DUYET_CONG_KHAI, SqlDbType.DateTime2),
                            new SqlParameter(GHI_CHU_CUA_CONG_KHAI, SqlDbType.NText),
                            new SqlParameter(NGAY_HET_HAN_CONG_KHAI, SqlDbType.DateTime2),
                            new SqlParameter(TRANG_THAI_CUA_CONG_KHAI, SqlDbType.Bit)
                           };
                        parameters[0].Value = objInsert.DuyetBanKeKhaiCongKhai.CongKhaiBanKeKhaiID.Value;
                        parameters[1].Value = CanBoID;
                        parameters[2].Value = objInsert.DuyetBanKeKhaiCongKhai.NgayDuyet;
                        parameters[3].Value = objInsert.DuyetBanKeKhaiCongKhai.GhiChu ?? Convert.DBNull;
                        parameters[4].Value = objInsert.DuyetBanKeKhaiCongKhai.NgayHetHan;
                        parameters[5].Value = objInsert.DuyetBanKeKhaiCongKhai.TrangThai;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            int query = 0;
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    query = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, KEKHAI_CAPNHAT_DUYETCONGKHAI, parameters);
                                    trans.Commit();
                                    if (query > 0)
                                    {
                                        Result.Status = 1;
                                        Result.Message = ConstantLogMessage.Alert_Insert_Success("Cập nhật Duyệt công khai bản kê khai");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                    trans.Rollback();
                                    throw ex;
                                }
                            }
                            var crCanBoXemBanKeKhai = new ThemCanBoXemBanKeKhaiModel();
                            crCanBoXemBanKeKhai.CongKhaiBanKeKhaiID = objInsert.DuyetBanKeKhaiCongKhai.CongKhaiBanKeKhaiID.Value;
                            crCanBoXemBanKeKhai.DanhSachCanBoXemBanKeKhaiID = objInsert.DanhSachCanBoXemBanKeKhaiID;
                            var capNhatCanBoXemBanKeKhai = CapNhatCanBoXemBanKeKhai(crCanBoXemBanKeKhai, CanBoID, CoQuanID);
                            if (capNhatCanBoXemBanKeKhai.Status < 1) return capNhatCanBoXemBanKeKhai;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public BaseResultModel HuyDuyetCongKhaiBanKeKhai(int? KeKhaiID)
        {
            var Result = new BaseResultModel();
            if (KeKhaiID == null || KeKhaiID <= 0)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước cập nhật";
                return Result;
            }
            else
            {
                int crID;
                if (!int.TryParse(KeKhaiID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "KeKhaiID không đúng định dạng";
                    return Result;
                }
                else
                {
                    // check bản kê khai đã được duyệt công khai chưa
                    SqlParameter[] parameters = new SqlParameter[]
                     {
                                new SqlParameter("KeKhaiID", SqlDbType.Int)
                     };
                    parameters[0].Value = KeKhaiID;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                int val = 0;
                                val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, KEKHAI_CONGKHAIBANKEKHAI_HUYCONGKHAI, parameters);
                                trans.Commit();
                                if (val < 0)
                                {
                                    Result.Status = 0;
                                    Result.Message = "Không thể hủy duyệt công khai";
                                    return Result;
                                }
                            }
                            catch (Exception ex)
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                                trans.Rollback();
                                return Result;
                                throw ex;
                            }
                        }
                    }
                    //}
                }

                Result.Status = 1;
                Result.Message = "Hủy Công khai bản kê khai thành công";
                return Result;
            }
        }

        public ChiTietCongKhaiBanKeKhai ChieTietCongKhaiBanKeKhai(int KeKhaiID)
        {
            var Result = new ChiTietCongKhaiBanKeKhai();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(KEKHAIID_CONG_KHAI,SqlDbType.Int)
              };
            parameters[0].Value = KeKhaiID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, KEKHAI_DUYETBANKEKHAI_GET_LIST_CANBOXEM_BY_KEKHAIID, parameters))
                {
                    var query = new List<KeKhaiCanBoModel>();
                    while (dr.Read())
                    {
                        var crObj = new KeKhaiCanBoModel();
                        crObj.CongKhaiBanKeKhaiID = Utils.ConvertToInt32(dr[CONG_KHAI_BAN_KE_KHAI_ID], 0);
                        crObj.KeKhaiID = Utils.ConvertToInt32(dr[KEKHAIID_CONG_KHAI], 0);
                        crObj.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        crObj.TenChucVu = Utils.ConvertToString(dr["TenChucVu"], string.Empty);
                        crObj.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        crObj.CanBoID = Utils.ConvertToInt32(dr[CAN_BO_ID], 0);
                        crObj.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        crObj.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        crObj.NgayDuyet = Utils.ConvertToDateTime(dr[NGAY_DUYET_CONG_KHAI], DateTime.Now.Date);
                        crObj.NgayHetHan = Utils.ConvertToDateTime(dr[NGAY_HET_HAN_CONG_KHAI], DateTime.Now.Date);
                        crObj.TrangThai = Utils.ConvertToBoolean(dr[TRANG_THAI_CUA_CONG_KHAI], false);
                        crObj.GhiChu = Utils.ConvertToString(dr[GHI_CHU_CUA_CONG_KHAI], string.Empty);

                        query.Add(crObj);
                    }
                    dr.Close();
                    Result = (from m in query
                              group m by new { m.CanBoID, m.TenCanBo, m.KeKhaiID, m.CongKhaiBanKeKhaiID } into canBo
                              select new ChiTietCongKhaiBanKeKhai()
                              {
                                  KeKhaiID = canBo.Key.KeKhaiID,
                                  CongKhaiBanKeKhaiID = canBo.Key.CongKhaiBanKeKhaiID,
                                  NgayDuyet = Utils.ConvertToDateTime(canBo.Select(x => x.NgayDuyet).FirstOrDefault(), DateTime.Now),
                                  NgayHetHan = Utils.ConvertToDateTime(canBo.Select(x => x.NgayHetHan).FirstOrDefault(), DateTime.Now),
                                  TrangThai = Utils.ConvertToBoolean(canBo.Select(x => x.TrangThai).FirstOrDefault(), false),
                                  GhiChu = Utils.ConvertToString(canBo.Select(x => x.GhiChu).FirstOrDefault(), string.Empty),
                                  DanhSachCanBoXemBanKeKhai = (from x in query
                                                               group x by new { x.CanBoID, x.TenCanBo } into chucVu
                                                               select new CongKhaiCanBoModel()
                                                               {
                                                                   CanBoID = chucVu.Key.CanBoID,
                                                                   TenCanBo = chucVu.Key.TenCanBo,
                                                                   CoQuanID = chucVu.Select(x => x.CoQuanID).FirstOrDefault(),
                                                                   TenCoQuan = chucVu.Select(x => x.TenCoQuan).FirstOrDefault(),
                                                                   DanhSachChucVuID = chucVu.Select(n => n.ChucVuID).ToList(),
                                                                   DanhSachTenChucVu = chucVu.Select(n => n.TenChucVu).ToList(),

                                                               }).ToList()

                              }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }


        /// <summary>
        /// thêm cán bộ được xem bản kê khai vào bản công khai
        /// </summary>
        /// <param name="objInsert"></param>
        /// <param name=CAN_BO_ID></param>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public BaseResultModel ThemCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objInsert, int CanBoID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            if (!(new PhanQuyenDAL().CheckQuyenDuyetBanKeKhai(CanBoID)))
            {
                Result.Status = 0;
                Result.Message = "Người sử dụng không có quyền thực hiện chức năng này";
                return Result;
            }
            try
            {
                if (objInsert == null)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập dữ liệu trước khi thực hiện thao tác";
                    return Result;
                }
                else if (objInsert.DanhSachCanBoXemBanKeKhaiID == null || objInsert.DanhSachCanBoXemBanKeKhaiID.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng chọn cán bộ xem bản kê khai";
                    return Result;
                }
                else
                {
                    // check tồn tại của cán bộ
                    var hethongcanboDAL = new HeThongCanBoDAL();
                    var banKeKhaiDAL = new KeKhaiDAL();
                    if (objInsert.DanhSachCanBoXemBanKeKhaiID.Any(
                        x => hethongcanboDAL.GetCanBoByID(x) == null)
                        || objInsert.DanhSachCanBoXemBanKeKhaiID.Any(x => hethongcanboDAL.GetCanBoByID(x).CanBoID < 1))
                    {
                        Result.Status = 0;
                        Result.Message = "Cán bộ không tồn tại";
                        return Result;
                    }

                    // tạo params @DanhSachCanBoID 
                    var tbCanBoXemBanKeKhai = new DataTable();
                    tbCanBoXemBanKeKhai.Columns.Add("ID", typeof(string));
                    objInsert.DanhSachCanBoXemBanKeKhaiID.ForEach(x => tbCanBoXemBanKeKhai.Rows.Add(x));
                    var pListCanBoXemBanKeKhai = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
                    pListCanBoXemBanKeKhai.TypeName = "dbo.list_ID";
                    SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter(CONG_KHAI_BAN_KE_KHAI_ID_CANBOXEMKEKHAI, SqlDbType.Int),
                            pListCanBoXemBanKeKhai
                        };
                    parameters[0].Value = objInsert.CongKhaiBanKeKhaiID;
                    parameters[1].Value = tbCanBoXemBanKeKhai;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        int query = 0;
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                query = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, KEKHAI_DUYETCONGKHAI_THEMCANBOXEMBANKEKHAI, parameters);
                                trans.Commit();
                                if (query > 0)
                                {
                                    Result.Status = 1;
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Thêm cán bộ xem bản kê khai");
                                }
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public BaseResultModel XoaCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objDelete, int CanBoID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            if (!(new PhanQuyenDAL().CheckQuyenDuyetBanKeKhai(CanBoID)))
            {
                Result.Status = 0;
                Result.Message = "Người sử dụng không có quyền thực hiện chức năng này";
                return Result;
            }
            if (objDelete == null)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else if (objDelete.CongKhaiBanKeKhaiID == null || objDelete.CongKhaiBanKeKhaiID < 1)
            {
                Result.Status = 0;
                Result.Message = "Bản kê khai không tồn tại";
                return Result;
            }
            else if (objDelete.DanhSachCanBoXemBanKeKhaiID == null || objDelete.DanhSachCanBoXemBanKeKhaiID.Count < 1)
            {
                Result.Status = 0;
                Result.Message = "Cán bộ không được trống";
                return Result;
            }
            else
            {
                // tạo params @DanhSachCanBoID 
                var tbCanBoXemBanKeKhai = new DataTable();
                tbCanBoXemBanKeKhai.Columns.Add("ID", typeof(string));
                objDelete.DanhSachCanBoXemBanKeKhaiID.ForEach(x => tbCanBoXemBanKeKhai.Rows.Add(x));
                var pListCanBoXemBanKeKhai = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
                pListCanBoXemBanKeKhai.TypeName = "dbo.list_ID";
                SqlParameter[] parameters = new SqlParameter[]
                {
                   new SqlParameter(CONG_KHAI_BAN_KE_KHAI_ID_CANBOXEMKEKHAI, SqlDbType.Int),
                   pListCanBoXemBanKeKhai
                };
                parameters[0].Value = objDelete.CongKhaiBanKeKhaiID;
                parameters[1].Value = tbCanBoXemBanKeKhai;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, KEKHAI_DUYETBANKEKHAI_GET_LIST_XOA_CANBOXEM, parameters);
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            return Result;
                            throw ex;
                        }
                    }
                    Result.Status = 1;
                    Result.Message = ConstantLogMessage.Alert_Delete_Success("Cán bộ xem bản kê khai");
                    return Result;
                }
            }
        }

        public BaseResultModel CapNhatCanBoXemBanKeKhai(ThemCanBoXemBanKeKhaiModel objInsert, int CanBoID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            if (!(new PhanQuyenDAL().CheckQuyenDuyetBanKeKhai(CanBoID)))
            {
                Result.Status = 0;
                Result.Message = "Người sử dụng không có quyền thực hiện chức năng này";
                return Result;
            }
            try
            {
                if (objInsert == null)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập dữ liệu trước khi thực hiện thao tác";
                    return Result;
                }
                else if (objInsert.DanhSachCanBoXemBanKeKhaiID == null || objInsert.DanhSachCanBoXemBanKeKhaiID.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng chọn cán bộ xem bản kê khai";
                    return Result;
                }
                else
                {
                    // check tồn tại của cán bộ
                    var hethongcanboDAL = new HeThongCanBoDAL();
                    var banKeKhaiDAL = new KeKhaiDAL();
                    if (objInsert.DanhSachCanBoXemBanKeKhaiID.Any(
                        x => hethongcanboDAL.GetCanBoByID(x) == null)
                        || objInsert.DanhSachCanBoXemBanKeKhaiID.Any(x => hethongcanboDAL.GetCanBoByID(x).CanBoID < 1))
                    {
                        Result.Status = 0;
                        Result.Message = "Cán bộ không tồn tại";
                        return Result;
                    }

                    // tạo params @DanhSachCanBoID 
                    var tbCanBoXemBanKeKhai = new DataTable();
                    tbCanBoXemBanKeKhai.Columns.Add("ID", typeof(string));
                    objInsert.DanhSachCanBoXemBanKeKhaiID.ForEach(x => tbCanBoXemBanKeKhai.Rows.Add(x));
                    var pListCanBoXemBanKeKhai = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
                    pListCanBoXemBanKeKhai.TypeName = "dbo.list_ID";
                    SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter(CONG_KHAI_BAN_KE_KHAI_ID_CANBOXEMKEKHAI, SqlDbType.Int),
                            pListCanBoXemBanKeKhai
                        };
                    parameters[0].Value = objInsert.CongKhaiBanKeKhaiID;
                    parameters[1].Value = tbCanBoXemBanKeKhai;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        int query = 0;
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                query = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, KEKHAI_DUYETCONGKHAI_CAPNHATCANBOXEM, parameters);
                                trans.Commit();
                                if (query > 0)
                                {
                                    Result.Status = 1;
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Cập nhật cán bộ xem bản kê khai");
                                }
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        ///
        public BaseResultModel CapNhatTrangThaiNhacViec(int? DuyetBanKeKhaiID)
        {
            BaseResultModel Result = new BaseResultModel();
            SqlParameter[] parameters = new SqlParameter[]
               {
                            new SqlParameter(DUYET_BAN_KE_KHAI_ID, SqlDbType.Int)

               };
            parameters[0].Value = DuyetBanKeKhaiID ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                int query = 0;
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        query = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATE_DUYETBANKEKHAI, parameters);
                        trans.Commit();
                        if (query > 0)
                        {
                            Result.Status = 1;
                            Result.Message = "Update trạng thái nhắc việc thành công!";
                        }
                    }
                    catch (Exception ex)
                    {
                        Result.Status = -1;
                        Result.Message = ConstantLogMessage.API_Error_System;
                        trans.Rollback();
                        throw ex;
                    }

                }

                return Result;
            }
        }

        public BaseResultModel CapNhatTrangThaiNhacViec_Multi(List<int> DanhSachDuyetKeKhaiID)
        {
            BaseResultModel Result = new BaseResultModel();
            var table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            DanhSachDuyetKeKhaiID.ForEach(x => table.Rows.Add(x));
            var ListKeKhaiID = new SqlParameter("@DanhSachKeKhaiID", SqlDbType.Structured);
            ListKeKhaiID.TypeName = "dbo.list_ID";
            SqlParameter[] parameters = new SqlParameter[]
             {
                             ListKeKhaiID
             };

            parameters[0].Value = table;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        var query = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATE_DUYETBANKEKHAI_MULTIL, parameters);
                        trans.Commit();
                        if (query > 0)
                        {
                            Result.Status = 1;
                            Result.Message = "Cập nhật trạng thái nhắc viêc thành công!";
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


            return Result;
        }
    }
}





