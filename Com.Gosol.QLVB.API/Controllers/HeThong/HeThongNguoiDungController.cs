using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.FileDinhKem;
using Com.Gosol.QLVB.BUS.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Com.Gosol.QLVB.Models.HeThong.HeThongNguoiDungModelPartial;
//using LogHelper = Com.Gosol.QLVB.API.Formats.LogHelper;

namespace Com.Gosol.QLVB.API.Controllers.HeThong
{
    [Route("api/v1/HeThongNguoiDung")]
    [ApiController]
    public class HeThongNguoidungController : BaseApiController
    {
        private IHeThongNguoidungBUS _HeThongNguoidungBUS;
        private ILogHelper _ILogHelper;
        private IPhanQuyenBUS _PhanQuyenBUS;
        private IFileDinhKemBUS _FileDinhKemBUS;
        private IHeThongCanBoBUS _HeThongCanBoBUS;
        public HeThongNguoidungController(IHeThongNguoidungBUS HeThongNguoidungBUS, IFileDinhKemBUS FileDinhKemBUS, IHeThongCanBoBUS HeThongCanBoBUS, ILogHelper _logHelper, IPhanQuyenBUS PhanQuyenBUS, ILogger<HeThongNguoidungController> logger) : base(_logHelper, logger)
        {
            this._HeThongNguoidungBUS = HeThongNguoidungBUS;
            _PhanQuyenBUS = PhanQuyenBUS;
            this._FileDinhKemBUS = FileDinhKemBUS;
            this._HeThongCanBoBUS = HeThongCanBoBUS;
        }
        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_NguoiDung, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(HeThongNguoiDungModel HeThongNguoiDungModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_Nguoidung_ThemNguoidung, EnumLogType.Insert, () =>
                {
                    string Message = null;
                    int val = 0;
                    val = _HeThongNguoidungBUS.Insert(HeThongNguoiDungModel, ref Message);
                    base.Message = Message;
                    base.Status = val > 0 ? 1 : 0;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_NguoiDung, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(HeThongNguoiDungModel HeThongNguoiDungModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_Nguoidung_SuaNguoidung, EnumLogType.Update, () =>
                {
                    string Message = null;
                    int val = 0;
                    val = _HeThongNguoidungBUS.Update(HeThongNguoiDungModel, ref Message);
                    base.Message = Message;
                    base.Status = val > 0 ? 1 : 0;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_NguoiDung, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete([FromBody] BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_Nguoidung_XoaNguoidung, EnumLogType.Delete, () =>
                 {
                     int Status = 0;
                     var Result = _HeThongNguoidungBUS.Delete(p.ListID, ref Status);
                     //if(Result.Count <= 0)
                     //{
                     //    base.Status = 1;
                     //    base.Message = "Xóa thành công!";
                     //    return base.GetActionResult();
                     //}
                     //else
                     //{
                     base.Status = Status;
                     base.Data = Result;
                     return base.GetActionResult();
                     //}

                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_NguoiDung, AccessLevel.Read)]
        [Route("GetByID")]
        public IActionResult GetByID(int NguoiDungID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_Nguoidung_GetByID, EnumLogType.GetByID, () =>
                 {
                     HeThongNguoiDungModel Data;
                     Data = _HeThongNguoidungBUS.GetByID(NguoiDungID);
                     base.Status = Data.CanBoID > 0 ? 1 : 0;
                     base.Data = Data;
                     return base.GetActionResult();
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_NguoiDung, AccessLevel.Read)]
        [Route("GetListPaging1")]
        public IActionResult GetPagingBySearch1([FromQuery] BasePagingParams p, int? CoQuanID, int? TrangThai)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_Nguoidung_GetListPaging, EnumLogType.GetList, () =>
                 {
                     IList<object> Data;
                     int TotalRow = 0;
                     Data = _HeThongNguoidungBUS.GetPagingBySearch(p, ref TotalRow, CoQuanID, TrangThai);
                     int totalRow = Data.Count();
                     if (totalRow == 0)
                     {
                         base.Message = ConstantLogMessage.API_NoData;
                         base.Status = 1;
                         return base.GetActionResult();
                     }

                     base.Status = TotalRow > 0 ? 1 : 0;
                     base.Data = Data;
                     base.TotalRow = TotalRow;
                     return base.GetActionResult();
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetPagingBySearch([FromQuery] BasePagingParamsForFilter p)
        {
            try
            {
                var crCoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                var crNguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                //var crCanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                return CreateActionResult(ConstantLogMessage.HT_Nguoidung_GetListPaging, EnumLogType.GetList, () =>
                {
                    IList<object> Data;
                    int TotalRow = 0;
                    Data = _HeThongNguoidungBUS.GetPagingBySearch_New(p, ref TotalRow, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0), crNguoiDungID, crCoQuanID);
                    int totalRow = Data.Count();
                    if (totalRow == 0)
                    {
                        base.Message = ConstantLogMessage.API_NoData;
                        base.Status = 1;
                        base.GetActionResult();
                    }
                    base.Status = totalRow > 0 ? 1 : 0;
                    base.Data = Data;
                    base.TotalRow = TotalRow;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("ResetPassword")]
        public IActionResult ResetPassword([FromQuery] int NguoiDungID)
        {
            try
            {
                return CreateActionResult("Reset lại mật khẩu", EnumLogType.Other, () =>
                {
                    //int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                    var Result = _HeThongNguoidungBUS.ResetPassword(NguoiDungID);
                    try
                    {
                        if (Utils.ConvertToInt32(Result.FirstOrDefault().Key, 0) > 0)
                        {
                            var clsCommon = new Commons();
                            var NguoiDung = _HeThongNguoidungBUS.GetByID(NguoiDungID);
                            HeThongNguoiDungModelPartial p = new HeThongNguoiDungModelPartial();
                            p.TenNguoiDung = NguoiDung.TenNguoiDung;
                            p.Url = clsCommon.GetServerPath(HttpContext);
                            SendMail(p);
                        }
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                    base.Message = Result.FirstOrDefault().Value.ToString();
                    base.Status = Utils.ConvertToInt32(Result.FirstOrDefault().Key, 0);
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [Route("GetByIDForPhanQuyen")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [HttpGet]
        public IActionResult GetByIDForPhanQuyen(int? NguoiDungID)
        {
            try
            {

                return CreateActionResult(ConstantLogMessage.HT_Nguoidung_GetByID, EnumLogType.GetList, () =>
                {
                    if (NguoiDungID == null)
                    {
                        return Ok(new
                        {
                            Status = -1,
                            Message = "Param NguoiDungID is NULL",
                        });
                    }
                    else
                    {
                        var NguoiDungIDToken = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                        if (NguoiDungIDToken != NguoiDungID)
                        {
                            return Ok(new
                            {
                                Status = -1,
                                Message = "Sai token",
                            });
                        }
                    }
                    NguoiDungModel NguoiDung = null;
                    NguoiDung = _HeThongNguoidungBUS.GetByIDForPhanQuyen(NguoiDungID.Value);

                    if (NguoiDung != null && NguoiDung.NguoiDungID > 0)
                    {
                        var clsCommon = new Commons();
                        //var listFile = _FileDinhKemBUS.GetAllField_FileDinhKem_ByNghiepVuID_AndType(NguoiDung.CanBoID, EnumLoaiFileDinhKem.AnhHoSo.GetHashCode());
                        //if (listFile.Count > 0)
                        //    NguoiDung.AnhHoSo = clsCommon.GetServerPath(HttpContext) + listFile[0].FileUrl;
                        //   Task.Run(() => _ILogHelper.Log(NguoiDung.CanBoID, "Đăng nhập hệ thống", (int)LogType.Action));
                        var cb = _HeThongCanBoBUS.GetCanBoByID(NguoiDung.CanBoID);
                        if (cb.AnhHoSo != null)
                        {
                            NguoiDung.AnhHoSo = clsCommon.GetServerPath(HttpContext) + cb.AnhHoSo;
                        }
                        var ListChucNang = _PhanQuyenBUS.GetListChucNangByNguoiDungID(NguoiDungID.Value);
                        NguoiDung.expires_at = Utils.ConvertToDateTime(User.Claims.FirstOrDefault(c => c.Type == "expires_at").Value, DateTime.Now.Date);
                        NguoiDung.CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                        if(NguoiDung.NguoiDungID == 1) NguoiDung.MapThongTin = 1;
                        return Ok(new
                        {
                            Status = 1,
                            User = NguoiDung,
                            ListRole = ListChucNang
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            Status = -1,
                            //Message = Constant.NOT_ACCOUNT,
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.GetActionResult();
                throw ex;
            }
        }
        [Route("SendMail")]
        [CustomAuthAttribute(0, AccessLevel.FullAccess)]
        [HttpPost]
        public IActionResult SendMail([FromBody] HeThongNguoiDungModelPartial p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.API_SendMail, EnumLogType.Other, () =>
                {
                    int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                    var Result = _HeThongNguoidungBUS.SendMail(p.TenNguoiDung, p.Url);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
            }
        }
        [Route("UpdateNguoiDung")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [HttpPost]
        public IActionResult UpdateNguoiDung([FromBody] QuenMatKhauModelPar p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.API_SendMail, EnumLogType.Other, () =>
                {
                    int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                    var Result = _HeThongNguoidungBUS.UpdateNguoiDung(p.TenDangNhap, p.MatKhauMoi);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
            }



        }
        [Route("CheckMaMail")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [HttpGet]
        public IActionResult CheckMaMail([FromQuery] string Token)
        {
            try
            {
                return CreateActionResult("Check mã mail", EnumLogType.Other, () =>
                {
                    int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                    var Result = _HeThongNguoidungBUS.CheckMaMail(Token);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    base.Data = Result.Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
            }



        }
        [Route("KiemTraTenNguoiDung")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [HttpGet]
        public IActionResult KiemTraTenNguoiDung([FromQuery] string TenNguoiDung)
        {
            try
            {
                int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                var Result = _HeThongNguoidungBUS.GetByName(TenNguoiDung);
                if(Result.NguoiDungID > 0)
                {
                    base.Status = 0;
                    base.Message = "Tên người dùng đã được sử dụng!";
                }
                else
                {
                    base.Status = 1;
                }
               
              
                return base.GetActionResult();
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
            }
        }
        [HttpPost]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("ChangePassword")]
        public IActionResult ChangePassword([FromBody] DoiMatKhauModel p)
        {
            try
            {
                //var expires_at = Utils.ConvertToDateTime(User.Claims.FirstOrDefault(c => c.Type == "expires_at").Value, DateTime.Now.Date);
                var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                return CreateActionResult("Đổi mật khẩu", EnumLogType.Other, () =>
                {
                    var Result = _HeThongNguoidungBUS.ChangePassword(NguoiDungID, p.OldPassword, p.NewPassword);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("HeThong_NguoiDung_GetListBy_NhomNguoiDungID")]
        public IActionResult HeThong_NguoiDung_GetListBy_NhomNguoiDungID(int NhomNguoiDungID)
        {
            try
            {
                return CreateActionResult("Lấy danh sách người dùng theo nhóm người dùng", EnumLogType.GetList, () =>
                {
                    int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                    List<HeThongNguoiDungModel> Data;
                    int TotalRow = 0;
                    Data = _PhanQuyenBUS.HeThong_NguoiDung_GetListBy_NhomNguoiDungID(NhomNguoiDungID);
                    //Data = _PhanQuyenBUS.HeThong_NguoiDung_GetListBy_NhomNguoiDungID(NhomNguoiDungID, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0));
                    int totalRow = Data.Count();
                    if (totalRow == 0)
                    {
                        base.Message = ConstantLogMessage.API_NoData;
                        base.Status = 1;
                        base.GetActionResult();
                    }
                    base.Status = totalRow >= 0 ? 1 : 0;
                    base.Data = Data;
                    base.TotalRow = TotalRow;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        [Route("UpdateTrangThaiTaiKhoan")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [HttpPost]
        public IActionResult UpdateTrangThaiTaiKhoan([FromBody] HeThongNguoiDungModel HeThongNguoiDungModel)
        {
            try
            {
                return CreateActionResult("Cập nhật trạng thái tài khoản ID " + HeThongNguoiDungModel.NguoiDungID, EnumLogType.Other, () =>
                {
                    int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                    var Result = _HeThongNguoidungBUS.UpdateTrangThaiTaiKhoan(HeThongNguoiDungModel.NguoiDungID, HeThongNguoiDungModel.TrangThai);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
            }



        }
    }
}