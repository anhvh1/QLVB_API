using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.DAL.KeKhai;
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
using Com.Gosol.QLVB.Security;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Com.Gosol.QLVB.Models.DanhMuc;

namespace Com.Gosol.QLVB.DAL
{
    public interface INhacViecDAL
    {
        public List<NhacViecModel> GetViecLam(int? NguoiDungID, int? CanBoID, int CoQuanID);
        public BaseResultModel Edit_ThongBao(ThongBaoModel ThongBaoModel);
        public BaseResultModel SendMail(List<string> MailTo, string TieuDe, string NoiDung);
        public BaseResultModel SendMailAuto();
        public List<ThongBaoModel> GetAllThongBao();
    }
    public class NhacViecDAL : INhacViecDAL
    {
        // Lấy danh sách thông báo việc cần làm
        public List<NhacViecModel> GetViecLam(int? NguoiDungID, int? CanBoID, int CoQuanID)
        {
            var CanBo = new HeThongCanBoDAL().GetCanBoByID(CanBoID);
            var DuyetCongKhai = "duyet-cong-khai-ban-ke-khai";
            var QuanLyBanKeKhai = "quan-ly-ban-ke-khai";
            var keKhaiTaiSan = "ke-khai-tai-san";
            var BackUpDB = "quan-tri-du-lieu";
            BackupService BackupService = new BackupService();
            List<NhacViecModel> ListNhacViec = new List<NhacViecModel>();
            string message = "";
            try
            {
                if (UserRole.CheckAdmin(NguoiDungID.Value))
                {
                    var LastBackUp = BackupService.GetLastBackUpDB(CanBoID.Value);
                    if (LastBackUp.SystemLogid <= 0)
                    {
                        NhacViecModel NhacViecModel = new NhacViecModel();
                        NhacViecModel.Name = "Bạn cần back up database định kỳ !";
                        NhacViecModel.NoiDung = "Bạn cần back up database định kỳ !";
                        NhacViecModel.Key = BackUpDB;
                        ListNhacViec.Add(NhacViecModel);
                    }
                    else
                    {
                        DateTime LastBackUpTime = LastBackUp.LogTime;
                        DateTime DateTimeNow = DateTime.Now;
                        string ConfigKey = "Backup_Date";
                        var value = new SystemConfigDAL().GetByKey(ConfigKey);
                        TimeSpan timeSpan = DateTimeNow.Subtract(LastBackUpTime);
                        if (timeSpan.TotalDays >= int.Parse(value.ConfigValue))
                        {
                            NhacViecModel NhacViecModel = new NhacViecModel();
                            NhacViecModel.Name = "Bạn cần back up database định kỳ !";
                            NhacViecModel.NoiDung = "Bạn cần back up database định kỳ !";
                            NhacViecModel.Key = BackUpDB;
                            ListNhacViec.Add(NhacViecModel);
                        }
                    }
                }
                else
                {


                    //var Exdate = DateTime.Now;

                    //new DotKeKhaiDAL().CheckTrangThai();

                    //var ListQuanLyBanKeKhai = new QuanLyBanKeKhaiDAL().GetListQuanLyBanKeKhai(CoQuanID, CanBoID.Value);
                    //var listBanKeKhaiChuaPheDuyet = ListQuanLyBanKeKhai.Where(x => x.TenTrangThai.Trim().ToLower() == "chưa duyệt").ToList();
                    //if (listBanKeKhaiChuaPheDuyet.Count > 0 && CanBo.VaiTro == 2)
                    //{
                    //    NhacViecModel NhacViecModel = new NhacViecModel();
                    //    NhacViecModel.Name = "Có " + listBanKeKhaiChuaPheDuyet.Count + " bản kê khai chưa phê duyệt";
                    //    NhacViecModel.NoiDung = "Có " + listBanKeKhaiChuaPheDuyet.Count + " bản kê khai chưa phê duyệt";
                    //    NhacViecModel.Key = QuanLyBanKeKhai;
                    //    ListNhacViec.Add(NhacViecModel);
                    //}
                    //var DotKeKhaiByCanBo = new DotKeKhaiDAL().GetDotKeKhaiByCanBoID(CanBoID.Value).Where(x => x.TrangThai == true).ToList();
                    //List<KeKhaiModel> ListKeKhai = new List<KeKhaiModel>();
                    //DotKeKhaiByCanBo.ForEach(x => ListKeKhai.Add(new KeKhaiDAL().GetByDotKeKhaiIDAndCanBoID(x.DotKeKhaiID, x.CanBoID.Value)));
                    //ListKeKhai = ListKeKhai.Where(x => x.KeKhaiID > 0).ToList();
                    //if (ListKeKhai.Count < DotKeKhaiByCanBo.Count /*&& QuyenXemQuanLyDotKeKhai.Count > 0*/)
                    //{
                    //    NhacViecModel NhacViecModel = new NhacViecModel();
                    //    NhacViecModel.Name = "Bạn có đợt kê khai cần kê khai";
                    //    NhacViecModel.NoiDung = "Bạn có đợt kê khai cần kê khai";
                    //    NhacViecModel.Key = keKhaiTaiSan;
                    //    ListNhacViec.Add(NhacViecModel);

                    //    //var ChucNangByNguoiDungID = new ChucNangDAL().GetListChucNangByNguoiDungID(NguoiDungID.Value);
                    //    //var QuyenXemQuanLyDotKeKhai = ChucNangByNguoiDungID.Where(x => x.MaChucNang == "quan-ly-dot-ke-khai" && x.Quyen >= 1).ToList();
                    //    var DotKeKhaiCuoiCung = DotKeKhaiByCanBo.OrderByDescending(x => x.DotKeKhaiID).FirstOrDefault();
                    //    TimeSpan time = DotKeKhaiCuoiCung.DenNgay.Subtract(DateTime.Now);
                    //    if (time.TotalDays <= int.Parse(new SystemConfigDAL().GetByKey("Alert_Exdate").ConfigValue))
                    //    {
                    //        NhacViecModel HetHanKeKhai = new NhacViecModel();
                    //        HetHanKeKhai.Name = "Có đợt kê khai sắp hết hạn";
                    //        HetHanKeKhai.NoiDung = "Có đợt kê khai sắp hết hạn";
                    //        HetHanKeKhai.Key = keKhaiTaiSan;
                    //        ListNhacViec.Add(HetHanKeKhai);
                    //    }
                    //}
                    //var GetListDuyetCongKhai = new DuyetCongKhaiBanKeKhaiDAL().GetListDuyetCongKhai(CoQuanID, CanBoID.Value);
                    //var ListChuaDuyetCongKhai = GetListDuyetCongKhai.Where(x => x.TenTrangThai.Trim().ToLower() == "chưa công khai").ToList();
                    //if (ListChuaDuyetCongKhai.Count > 0 && CanBo.VaiTro == 2)
                    //{
                    //    NhacViecModel NhacViecModel = new NhacViecModel();
                    //    NhacViecModel.Name = "Có " + ListChuaDuyetCongKhai.Count + " bản kê khai cần duyệt công khai";
                    //    NhacViecModel.NoiDung = "Có " + ListChuaDuyetCongKhai.Count + " bản kê khai cần duyệt công khai";
                    //    NhacViecModel.Key = DuyetCongKhai;
                    //    ListNhacViec.Add(NhacViecModel);
                    //}
                }


            }


            catch (Exception ex)
            {
                throw ex;
            }
            return ListNhacViec;

        }

        public BaseResultModel Edit_ThongBao(ThongBaoModel ThongBaoModel)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parms = new SqlParameter[]{
                    new SqlParameter("ThongBaoID", SqlDbType.Int),
                    new SqlParameter("TieuDe", SqlDbType.NVarChar),
                    new SqlParameter("NoiDung", SqlDbType.NVarChar),
                    new SqlParameter("ThoiGianBatDau", SqlDbType.DateTime),
                    new SqlParameter("ThoiGianKetThuc", SqlDbType.DateTime),
                    new SqlParameter("HienThi", SqlDbType.Bit),
                    new SqlParameter("LoaiThongBao", SqlDbType.Int),
                    new SqlParameter("NghiepVuID", SqlDbType.Int),
                };
                parms[0].Value = ThongBaoModel.ThongBaoID ?? Convert.DBNull;
                parms[1].Value = ThongBaoModel.TieuDe ?? Convert.DBNull;
                parms[2].Value = ThongBaoModel.NoiDung ?? Convert.DBNull;
                parms[3].Value = ThongBaoModel.ThoiGianBatDau ?? Convert.DBNull;
                parms[4].Value = ThongBaoModel.ThoiGianKetThuc ?? Convert.DBNull;
                parms[5].Value = ThongBaoModel.HienThi ?? Convert.DBNull;
                parms[6].Value = ThongBaoModel.LoaiThongBao ?? Convert.DBNull;
                parms[7].Value = ThongBaoModel.NghiepVuID ?? Convert.DBNull;

                if (ThongBaoModel.ThongBaoID == null)
                {
                    parms[0].Direction = ParameterDirection.Output;
                    parms[0].Size = 8;
                }

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            if (ThongBaoModel.ThongBaoID > 0)
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_ThongBao_Update", parms);

                                //Xóa đối tượng thông báo cũ
                                SqlParameter[] parms_del = new SqlParameter[]{
                                    new SqlParameter("ThongBaoID", SqlDbType.Int),
                                };
                                parms_del[0].Value = ThongBaoModel.ThongBaoID;
                                SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_DoiTuongThongBao_Delete", parms_del);
                                if (Result.Status > 0 && ThongBaoModel.DoiTuongThongBao != null && ThongBaoModel.DoiTuongThongBao.Count > 0)
                                {
                                    //Insert đối tượng thông báo
                                    foreach (var item in ThongBaoModel.DoiTuongThongBao)
                                    {
                                        SqlParameter[] parms_tv = new SqlParameter[]{
                                            new SqlParameter("ThongBaoID", SqlDbType.Int),
                                            new SqlParameter("CanBoID", SqlDbType.Int),
                                            new SqlParameter("CoQuanID", SqlDbType.Int),
                                        };
                                        parms_tv[0].Value = ThongBaoModel.ThongBaoID;
                                        parms_tv[1].Value = item.CanBoID ?? Convert.DBNull;
                                        parms_tv[2].Value = item.CoQuanID ?? Convert.DBNull;

                                        SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_DoiTuongThongBao_Insert", parms_tv);
                                    }
                                }
                                Result.Message = ConstantLogMessage.Alert_Update_Success("thông báo");
                                Result.Data = ThongBaoModel.ThongBaoID;
                            }
                            else
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_ThongBao_Insert", parms);
                                int ThongBaoID = Utils.ConvertToInt32(parms[0].Value, 0);
                                if (ThongBaoModel.DoiTuongThongBao != null && ThongBaoModel.DoiTuongThongBao.Count > 0)
                                {
                                    //Insert đối tượng thông báo
                                    foreach (var item in ThongBaoModel.DoiTuongThongBao)
                                    {
                                        SqlParameter[] parms_tv = new SqlParameter[]{
                                            new SqlParameter("ThongBaoID", SqlDbType.Int),
                                            new SqlParameter("CanBoID", SqlDbType.Int),
                                            new SqlParameter("CoQuanID", SqlDbType.Int),
                                        };
                                        parms_tv[0].Value = ThongBaoID;
                                        parms_tv[1].Value = item.CanBoID ?? Convert.DBNull;
                                        parms_tv[2].Value = item.CoQuanID ?? Convert.DBNull;

                                        SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_DoiTuongThongBao_Insert", parms_tv);
                                    }
                                }
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("thông báo");
                                Result.Data = ThongBaoID;
                            }

                            trans.Commit();
                            Result.Status = 1;
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

            //send mail thông báo
            //GuiThongBaoEmail(ThongBaoModel);

            return Result;
        }

        public BaseResultModel Edit_ThongBao_New(ThongBaoModel ThongBaoModel)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parms = new SqlParameter[]{
                    new SqlParameter("ThongBaoID", SqlDbType.Int),
                    new SqlParameter("TieuDe", SqlDbType.NVarChar),
                    new SqlParameter("NoiDung", SqlDbType.NVarChar),
                    new SqlParameter("ThoiGianBatDau", SqlDbType.DateTime),
                    new SqlParameter("ThoiGianKetThuc", SqlDbType.DateTime),
                    new SqlParameter("HienThi", SqlDbType.Bit),
                    new SqlParameter("LoaiThongBao", SqlDbType.Int),
                    new SqlParameter("NghiepVuID", SqlDbType.Int),
                };
                parms[0].Value = ThongBaoModel.ThongBaoID ?? Convert.DBNull;
                parms[1].Value = ThongBaoModel.TieuDe ?? Convert.DBNull;
                parms[2].Value = ThongBaoModel.NoiDung ?? Convert.DBNull;
                parms[3].Value = ThongBaoModel.ThoiGianBatDau ?? Convert.DBNull;
                parms[4].Value = ThongBaoModel.ThoiGianKetThuc ?? Convert.DBNull;
                parms[5].Value = ThongBaoModel.HienThi ?? Convert.DBNull;
                parms[6].Value = ThongBaoModel.LoaiThongBao ?? Convert.DBNull;
                parms[7].Value = ThongBaoModel.NghiepVuID ?? Convert.DBNull;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_ThongBao_Insert_New", parms);
                            int ThongBaoID = Utils.ConvertToInt32(parms[0].Value, 0);     
                            Result.Message = ConstantLogMessage.Alert_Insert_Success("thông báo");
                            Result.Data = ThongBaoID;

                            trans.Commit();
                            Result.Status = 1;
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

            //send mail thông báo
            //GuiThongBaoEmail(ThongBaoModel);

            return Result;
        }

        public BaseResultModel UpdateTrangThaiHienThiThongBao(int? loaiThongBao, List<int> DanhSachNghiepVuID)
        {
            var Result = new BaseResultModel();
            try
            {
                var pList = new SqlParameter("@DanhSachNghiepVuID", SqlDbType.Structured);
                pList.TypeName = "dbo.list_ID";
                var tbNghiepvuID = new DataTable();
                tbNghiepvuID.Columns.Add("ID", typeof(string));
                DanhSachNghiepVuID.ForEach(x=>tbNghiepvuID.Rows.Add(x));
                SqlParameter[] parms = new SqlParameter[]{
                    pList,
                    new SqlParameter("@LoaiThongbao", SqlDbType.Int),
                };
                parms[0].Value = tbNghiepvuID;
                parms[1].Value = loaiThongBao ?? Convert.DBNull;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        var t = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_ThongBao_UpdateHienThi", parms);
                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }

            return Result;
        }
 
        public List<ThongBaoModel> GetAllThongBao()
        {
            List<ThongBaoChiTietModel> thongBaos = new List<ThongBaoChiTietModel>();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_ThongBao_GetAll", null))
                {
                    while (dr.Read())
                    {
                        ThongBaoChiTietModel info = new ThongBaoChiTietModel();
                        info.ThongBaoID = Utils.ConvertToInt32(dr["ThongBaoID"], 0);
                        info.TieuDe = Utils.ConvertToString(dr["TieuDe"], string.Empty);
                        info.NoiDung = Utils.ConvertToString(dr["NoiDung"], string.Empty);
                        info.ThoiGianBatDau = Utils.ConvertToDateTime(dr["ThoiGianBatDau"], DateTime.MinValue);
                        info.ThoiGianKetThuc = Utils.ConvertToDateTime(dr["ThoiGianKetThuc"], DateTime.MinValue);
                        info.LoaiThongBao = Utils.ConvertToInt32(dr["LoaiThongBao"], 0);
                        info.NghiepVuID = Utils.ConvertToInt32(dr["NghiepVuID"], 0);
                        info.TenNghiepVu = Utils.ConvertToString(dr["TenNghiepVu"], string.Empty);
                        info.HienThi = Utils.ConvertToBoolean(dr["HienThi"], false);

                        info.DoiTuongThongBaoID = Utils.ConvertToInt32(dr["DoiTuongThongBaoID"], 0);
                        info.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        info.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        info.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        info.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        info.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);

                        thongBaos.Add(info);
                    }
                    dr.Close();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            List<ThongBaoModel> Result = new List<ThongBaoModel>();
            Result = thongBaos.GroupBy(p => p.ThongBaoID)
                   .Select(g => new ThongBaoModel
                   {
                       ThongBaoID = g.Key,
                       TieuDe = g.FirstOrDefault().TieuDe,
                       NoiDung = g.FirstOrDefault().NoiDung,
                       LoaiThongBao = g.FirstOrDefault().LoaiThongBao,
                       ThoiGianBatDau = g.FirstOrDefault().ThoiGianBatDau,
                       ThoiGianKetThuc = g.FirstOrDefault().ThoiGianKetThuc,
                       HienThi = g.FirstOrDefault().HienThi,
                       TenNghiepVu = g.FirstOrDefault().TenNghiepVu,
                       DoiTuongThongBao = thongBaos.Where(x => x.ThongBaoID == g.Key && x.DoiTuongThongBaoID > 0).GroupBy(x => x.DoiTuongThongBaoID)
                                       .Select(y => new DoiTuongThongBaoModel
                                       {
                                           DoiTuongThongBaoID = y.FirstOrDefault().DoiTuongThongBaoID,
                                           CanBoID = y.FirstOrDefault().CanBoID,
                                           CoQuanID = y.FirstOrDefault().CoQuanID,
                                           TenCanBo = y.FirstOrDefault().TenCanBo,
                                           Email = y.FirstOrDefault().Email,
                                           GioiTinh = y.FirstOrDefault().GioiTinh,
                                       }
                                       ).ToList(),
                   }
                   ).ToList();

            return Result;
        }

        public BaseResultModel GuiThongBaoEmail(ThongBaoModel ThongBaoModel)
        {
            var DuyetCongKhai = "duyet-cong-khai-ban-ke-khai";
            var QuanLyBanKeKhai = "quan-ly-ban-ke-khai";
            var KeKhaiTaiSan = "ke-khai-tai-san";

            var Result = new BaseResultModel();
            if (ThongBaoModel.DoiTuongThongBao == null || ThongBaoModel.DoiTuongThongBao.Count == 0)
            {
                Result.Status = -1;
                return Result;
            }

            string TieuDe = "";
            string NoiDung = "";
            string Url = new SystemConfigDAL().GetByKey("LinkPhanMem").ConfigValue.ToString();
            foreach (var item in ThongBaoModel.DoiTuongThongBao)
            {
                if (ThongBaoModel.LoaiThongBao == EnumLoaiThongBao.DotKeKhai.GetHashCode())
                {
                    TieuDe = "[QLVB] Thông báo đợt kê khai mới";
                    NoiDung = ("<p>" + "Kính gửi " + (item.GioiTinh == 1 ? "Ông " : "Bà ") + "<strong>" + item.TenCanBo + "</strong>" + "!" + "</p>" +
                                  "<p>" + "Hệ thống phần mềm Quản lý kê khai tài sản, thu nhập trân trọng thông báo ông/bà có đợt kê khai từ ngày " + ThongBaoModel.ThoiGianBatDau.Value.ToString("dd/MM/yyy") + " đến ngày " + "</a>" + ThongBaoModel.ThoiGianKetThuc.Value.ToString("dd/MM/yyy") + "</p>"
                                   + "<p>" + "Vui lòng truy cập đường link: " + Url + KeKhaiTaiSan + " để thực hiện kê khai trong thời gian quy định." + "</p>" +
                                    "<p>" + "Trân Trọng," + "</p>").ToString();
                    List<string> MailTo = new List<string>();
                    MailTo.Add(item.Email);
                    SendMail(MailTo, TieuDe, NoiDung);
                }
                else if (ThongBaoModel.LoaiThongBao == EnumLoaiThongBao.GuiHoSoKeKhai.GetHashCode())
                {
                    TieuDe = "[QLVB] Thông báo tiếp nhận hồ sơ kê khai";
                    NoiDung = ("<p>" + "Kính gửi " + (item.GioiTinh == 1 ? "Ông " : "Bà ") + "<strong>" + item.TenCanBo + "</strong>" + "!" + "</p>" +
                                  "<p>" + "Hệ thống phần mềm Quản lý kê khai tài sản, thu nhập trân trọng thông báo ông/bà có bản kê khai " + ThongBaoModel.TenNghiepVu + " cần tiếp nhận." + "</p>"
                                   + "<p>" + "Vui lòng truy cập đường link: " + Url + QuanLyBanKeKhai + " để thực hiện kê khai trong thời gian quy định." + "</p>" +
                                    "<p>" + "Trân Trọng," + "</p>").ToString();
                    List<string> MailTo = new List<string>();
                    MailTo.Add(item.Email);
                    SendMail(MailTo, TieuDe, NoiDung);
                }
                else if (ThongBaoModel.LoaiThongBao == EnumLoaiThongBao.LuuHoSoKeKhai.GetHashCode())
                {
                    TieuDe = "[QLVB] Thông báo lưu hồ sơ kê khai";
                    NoiDung = ("<p>" + "Kính gửi " + (item.GioiTinh == 1 ? "Ông " : "Bà ") + "<strong>" + item.TenCanBo + "</strong>" + "!" + "</p>" +
                                   "<p>" + "Hệ thống phần mềm Quản lý kê khai tài sản, thu nhập trân trọng thông báo ông/bà có bản kê khai "
                                   + ThongBaoModel.TenNghiepVu + " đã được tiếp nhận và lưu thành hồ sơ pháp lý." + "</p>"
                                   + "<p>" + "Vui lòng truy cập đường link: " + Url + QuanLyBanKeKhai + " để thực hiện kê khai trong thời gian quy định." + "</p>" +
                                     "<p>" + "Trân Trọng," + "</p>").ToString();
                    List<string> MailTo = new List<string>();
                    MailTo.Add(item.Email);
                    SendMail(MailTo, TieuDe, NoiDung);
                }
                else if (ThongBaoModel.LoaiThongBao == EnumLoaiThongBao.BoSungHoSoKeKhai.GetHashCode())
                {
                    TieuDe = "[QLVB] Thông báo yêu cầu kê khai bổ sung";
                    NoiDung = ("<p>" + "Kính gửi " + (item.GioiTinh == 1 ? "Ông " : "Bà ") + item.TenCanBo + "</strong>" + "!" + "</p>" +
                                   "<p>" + "Hệ thống phần mềm Quản lý kê khai tài sản, thu nhập trân trọng thông báo ông/bà có bản kê khai "
                                   + ThongBaoModel.TenNghiepVu + " được yêu cầu kê khai bổ sung." + "</p>"
                                   + "<p>" + "Vui lòng truy cập đường link: " + Url + QuanLyBanKeKhai + " để thực hiện kê khai trong thời gian quy định." + "</p>" +
                                     "<p>" + "Trân Trọng," + "</p>").ToString();
                    List<string> MailTo = new List<string>();
                    MailTo.Add(item.Email);
                    SendMail(MailTo, TieuDe, NoiDung);
                }
                else if (ThongBaoModel.LoaiThongBao == EnumLoaiThongBao.DuyetCongKhai.GetHashCode())
                {
                    TieuDe = "[QLVB] Thông báo duyệt công khai hồ sơ kê khai";
                    NoiDung = ("<p>" + "Kính gửi " + (item.GioiTinh == 1 ? "Ông " : "Bà ") + "<strong>" + item.TenCanBo + "</strong>" + "!" + "</p>" +
                                  "<p>" + "Hệ thống phần mềm Quản lý kê khai tài sản, thu nhập trân trọng thông báo ông/bà có bản kê khai " + ThongBaoModel.TenNghiepVu + " cần duyệt công khai." + "</p>"
                                   + "<p>" + "Vui lòng truy cập đường link: " + Url + DuyetCongKhai + " để thực hiện kê khai trong thời gian quy định." + "</p>" +
                                    "<p>" + "Trân Trọng," + "</p>").ToString();
                    List<string> MailTo = new List<string>();
                    MailTo.Add(item.Email);
                    SendMail(MailTo, TieuDe, NoiDung);
                }
            }

            return Result;
        }

        public BaseResultModel SendMail_New(List<string> MailTo, string TieuDe, string NoiDung)
        {
            var Result = new BaseResultModel();
            if (MailTo == null || MailTo.Count == 0)
            {
                Result.Status = -1;
                Result.Message = "Không có mail người nhận";
                return Result;
            }
            try
            {
                var admin = new SystemConfigDAL().GetByKey("Mail_QuanTri").ConfigValue.ToString().Split(";");
                string tkadmin = admin[0];
                string mkadmin = admin[1];
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(tkadmin, "Kê khai tài sản");
                    foreach (var item in MailTo)
                    {
                        mail.To.Add(item);
                    }
                    mail.Subject = TieuDe;
                    mail.Body = NoiDung;
                    mail.IsBodyHtml = true;
                    //mail.Attachments.Add(new Attachment("C:\\file.zip"));

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(tkadmin, mkadmin);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }

                Result.Status = 1;
                Result.Message = "Gửi mail thành công";
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ex.Message;
                throw;
            }
            return Result;
        }

        public BaseResultModel SendMail(List<string> MailTo, string TieuDe, string NoiDung)
        {
            BaseResultModel BaseResultModel = new BaseResultModel();
            try
            {
                var admin = new SystemConfigDAL().GetByKey("Mail_QuanTri").ConfigValue.ToString().Split(";");
                string tkadmin = admin[0];
                string mkadmin = admin[1];
                var credentials = new NetworkCredential(tkadmin, mkadmin);

                int LastMaDuocGui = 0;
                Random oRandom = new Random();
                LastMaDuocGui = oRandom.Next(1000, 99999);

                var Mail = new MailMessage()
                {
                    From = new MailAddress(tkadmin, "Kê khai tài sản"),
                    Subject = TieuDe,
                    Body = NoiDung
                };
                Mail.IsBodyHtml = true;
                foreach (var item in MailTo)
                {
                    Mail.To.Add(item);
                }
                //Mail.To.Add(new MailAddress(CanBoByNguoiDungID.Email.Trim()));
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials

                };
                client.Timeout = 30000;
                client.Send(Mail);

                BaseResultModel.Message = "Yêu cầu đổi mật khẩu của bạn đã được xử lý! Bạn nhớ kiểm tra mail để tiếp tục các bước còn lại ! Cảm ơn!";
                BaseResultModel.Status = 1;
                return BaseResultModel;

            }
            catch (Exception ex)
            {
                BaseResultModel.Status = 0;
                BaseResultModel.Message = "Vào link này để check quyền truy cập cho ứng dụng kém an toàn !" + "https://myaccount.google.com/lesssecureapps?pli=1";
                return BaseResultModel;
            }
        }

        public List<ThongTinThiSinh> GetAll()
        {
            List<ThongTinThiSinh> list = new List<ThongTinThiSinh>();

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_AutoUpdate", null))
                {
                    while (dr.Read())
                    {
                        ThongTinThiSinh info = new ThongTinThiSinh();
                        info.ThiSinhID = Utils.ConvertToInt32(dr["ThiSinhID"], 0);
                        info.NgaySinhStr = Utils.ConvertToString(dr["NgaySinhStr"], string.Empty);
                        list.Add(info);
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
        public int ConvertNgaySinhStrToInt(string NgaySinhStr)
        {
            int NgaySinhInt = 0;
            try
            {
                // 24/07/2002 // 6/6/1996 12:00:00 AM // /07/1996 // 
                var temp = "";
                if (temp == "")
                {
                    var arr = NgaySinhStr.Split("/");
                    if (arr != null && arr.Length == 3)
                    {
                        //nam
                        if (arr[2] != null)
                        {
                            if (arr[2].Trim().Length == 2)
                            {
                                temp = "19" + arr[2];
                            }
                            else if (arr[2].Trim().Length == 4)
                            {
                                temp = arr[2].Trim();
                            }
                            else if (arr[2].Trim().Length > 4)
                            {
                                var nam = arr[2].Trim().Split(" ");
                                if (nam != null && nam.Length > 0) { temp = nam[0]; }
                            }
                        }
                        //thang
                        if (arr[1] != null)
                        {
                            if (arr[1].Trim().Length == 0)
                            {
                                temp += "00";
                            }
                            else if (arr[1].Trim().Length == 1)
                            {
                                temp += "0" + arr[1].Trim();
                            }
                            else if (arr[1].Trim().Length == 2)
                            {
                                temp += arr[1].Trim();
                            }
                        }
                        //ngay
                        if (arr[0] != null)
                        {
                            if (arr[0].Trim().Length == 0)
                            {
                                temp += "00";
                            }
                            else if (arr[0].Trim().Length == 1)
                            {
                                temp += "0" + arr[0].Trim();
                            }
                            else if (arr[0].Trim().Length == 2)
                            {
                                temp += arr[0].Trim();
                            }
                        }
                    }
                    else if (arr != null && arr.Length == 2)
                    {
                        // /1994
                        //nam
                        if (arr[1] != null)
                        {
                            if (arr[1].Length == 2)
                            {
                                temp = "19" + arr[1];
                            }
                            else if (arr[1].Length == 4)
                            {
                                temp = arr[1];
                            }
                            else if (arr[1].Length > 4)
                            {
                                var nam = arr[1].Split(" ");
                                if (nam != null && nam.Length > 0) { temp = nam[0]; }
                            }
                        }
                        //thang
                        if (arr[0] != null)
                        {
                            if (arr[0].Length == 0)
                            {
                                temp += "00";
                            }
                            else if (arr[0].Length == 1)
                            {
                                temp += "0" + arr[1];
                            }
                            else if (arr[0].Length == 2)
                            {
                                temp += arr[0];
                            }
                        }
                        //ngay
                        temp += "00";
                    }
                }
                    

                // 30-12-1961
                if(temp == "")
                {
                    var arr = NgaySinhStr.Split("-");
                    if (arr != null && arr.Length == 3)
                    {
                        //nam
                        if (arr[2] != null)
                        {
                            if (arr[2].Trim().Length == 2)
                            {
                                temp = "19" + arr[2];
                            }
                            else if (arr[2].Trim().Length == 4)
                            {
                                temp = arr[2].Trim();
                            }
                        }
                        //thang
                        if (arr[1] != null)
                        {
                            if (arr[1].Trim().Length == 0)
                            {
                                temp += "00";
                            }
                            else if (arr[1].Trim().Length == 1)
                            {
                                temp += "0" + arr[1].Trim();
                            }
                            else if (arr[1].Trim().Length == 2)
                            {
                                temp += arr[1].Trim();
                            }
                        }
                        //ngay
                        if (arr[0] != null)
                        {
                            if (arr[0].Trim().Length == 0)
                            {
                                temp += "00";
                            }
                            else if (arr[0].Trim().Length == 1)
                            {
                                temp += "0" + arr[0].Trim();
                            }
                            else if (arr[0].Trim().Length == 2)
                            {
                                temp += arr[0].Trim();
                            }
                        }
                    }
                    else if (arr != null && arr.Length == 2)
                    {
                        // /1994
                        //nam
                        if (arr[1] != null)
                        {
                            if (arr[1].Length == 2)
                            {
                                temp = "19" + arr[1];
                            }
                            else if (arr[1].Length == 4)
                            {
                                temp = arr[1];
                            }
                            else if (arr[1].Length > 4)
                            {
                                var nam = arr[1].Split(" ");
                                if (nam != null && nam.Length > 0) { temp = nam[0]; }
                            }
                        }
                        //thang
                        if (arr[0] != null)
                        {
                            if (arr[0].Length == 0)
                            {
                                temp += "00";
                            }
                            else if (arr[0].Length == 1)
                            {
                                temp += "0" + arr[1];
                            }
                            else if (arr[0].Length == 2)
                            {
                                temp += arr[0];
                            }
                        }
                        //ngay
                        temp += "00";
                    }
                    else if (arr != null && arr.Length == 1)
                    {
                        if (arr[0].Trim().Length == 2)
                        {
                            temp = "19" + arr[0] + "0000";
                        }
                        else if (arr[0].Trim().Length == 4)
                        {
                            temp = arr[0] + "0000";
                        }
                    }
                }
                

                NgaySinhInt = Utils.ConvertToInt32(temp, 0);
            }
            catch (Exception)
            {
            }

            return NgaySinhInt;
        }
        public BaseResultModel Update(ThongTinThiSinh ThongTinThiSinh)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("ThiSinhID", SqlDbType.Int),
                        new SqlParameter("NgaySinh_Int", SqlDbType.Int),
                    };

                parameters[0].Value = ThongTinThiSinh.ThiSinhID;
                parameters[1].Value = ConvertNgaySinhStrToInt(ThongTinThiSinh.NgaySinhStr);   

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_ThongTinThiSinh_UpdateNgaySinhInt", parameters);
                            trans.Commit();
                            Result.Message = ConstantLogMessage.Alert_Update_Success("thí sinh");
                            return Result;
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
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
        }
        public void SendRequest()
        {
            #region update ngày sinh int
            try
            {
                var thiSinhs = GetAll();
                foreach (var item in thiSinhs)
                {
                    Update(item);
                }
            }
            catch (Exception)
            {
                throw;
            }
            #endregion
        }

        public BaseResultModel SendMailAuto()
        {
            BaseResultModel BaseResultModel = new BaseResultModel();
            try
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                Task timerTask = RunPeriodically(SendRequest, TimeSpan.FromMinutes(1), tokenSource.Token);

                BaseResultModel.Status = 1;
                return BaseResultModel;

            }
            catch (Exception ex)
            {
                BaseResultModel.Status = 0;
                BaseResultModel.Message = ex.Message;
                return BaseResultModel;
            }
        }

        async Task RunPeriodically(Action action, TimeSpan interval, CancellationToken token)
        {
            while (true)
            {
                action();
                await Task.Delay(interval, token);
            }
        }

        public BaseResultModel UpdateTrangThaiGuiMail(ThongBaoModel ThongBaoModel)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parms = new SqlParameter[]{
                    new SqlParameter("ThongBaoID", SqlDbType.Int),
                    new SqlParameter("SendEmail", SqlDbType.Int),
                };
                parms[0].Value = ThongBaoModel.ThongBaoID ?? Convert.DBNull;
                parms[1].Value = true;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_ThongBao_UpdateSendEmail", parms);
                            trans.Commit();
                            Result.Status = 1;
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
    }
}

