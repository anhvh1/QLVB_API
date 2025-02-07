using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.QLVB;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Com.Gosol.QLVB.API.Controllers.QLVB
{
    [Route("api/v1/ThongTinCapBang")]
    [ApiController]
    public class ThongTinCapBangController : BaseApiController
    {
        private IThongTinCapBangBUS _ThongTinCapBangBUS;
        private IQuanLyThiSinhBUS _QuanLyThiSinhBUS;
        private IHostingEnvironment _host;
        private readonly ILogger _logger;
        public ThongTinCapBangController(IThongTinCapBangBUS ThongTinCapBangBUS, IQuanLyThiSinhBUS QuanLyThiSinhBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<ThongTinCapBangController> logger) : base(_LogHelper, logger)
        {
            this._ThongTinCapBangBUS = ThongTinCapBangBUS;
            this._QuanLyThiSinhBUS = QuanLyThiSinhBUS;
            this._host = hostingEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetListPaging([FromQuery]BasePagingParams p, [FromQuery] DateTime? NgayCapBang, [FromQuery] int? DonViDaoTao)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinThiSinh> Data;
                    Data = _ThongTinCapBangBUS.GetPagingBySearch(p, NgayCapBang, DonViDaoTao ?? 0, ref TotalRow);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Data;

                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //base.GetActionResult();
                //throw;
            }
        }


        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("DanhSachThiSinhCapBang")]
        public IActionResult DanhSachThiSinhCapBang([FromQuery] BasePagingParams p, int? NamThi)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    p.Nam = p.Nam ?? NamThi;
                    IList<ThongTinThiSinh> Data;
                    Data = _ThongTinCapBangBUS.DanhSachThiSinhCapBang(p, ref TotalRow);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Data;

                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //base.GetActionResult();
                //throw;
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Edit)]
        [Route("UpdateThongTinCapBang")]
        public IActionResult UpdateThongTinCapBang(ThongTinThiSinh ThongTinThiSinh)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Update, () =>
                {
                    var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                    var Result = _ThongTinCapBangBUS.UpdateThongTinCapBang(ThongTinThiSinh, CanBoID);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //base.Message = ConstantLogMessage.API_Error_System;
                //return base.GetActionResult();
                //throw ex;
            }
        }

        [HttpGet]
        [Route("DanhSachThiSinhTrung")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult DanhSachThiSinhTrung([FromQuery] TraCuuParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinThiSinh> Data;
 
                    p.TrangThai = 0;
                    Data = _ThongTinCapBangBUS.DanhSachThiSinhTrung(p, ref TotalRow);
                   
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Data;

                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                //return base.GetActionResultErrorAPI();

                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(ThongTinThiSinh ThongTinThiSinh)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_DuLieuDiemThi_Them, EnumLogType.Insert, () =>
                {
                    var Result = _ThongTinCapBangBUS.Insert(ThongTinThiSinh);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    base.Data = Result.Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //base.Message = ConstantLogMessage.API_Error_System;
                //return base.GetActionResult();
                //throw ex;
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(ThongTinThiSinh ThongTinThiSinh)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Update, () =>
                {
                    var Result = _ThongTinCapBangBUS.Update(ThongTinThiSinh);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //base.Message = ConstantLogMessage.API_Error_System;
                //return base.GetActionResult();
                //throw ex;
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete(ThongTinThiSinh p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Xoa, EnumLogType.Delete, () =>
                {
                    var Result = _QuanLyThiSinhBUS.Delete(p.ThiSinhID);
                    base.Message = Result.Message;
                    base.Status = Result.Status;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //base.GetActionResult();
                //throw;
            }
        }

        [HttpGet]
        [Route("GetByID")]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        public IActionResult GetByID([FromQuery]int? ThiSinhID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _ThongTinCapBangBUS.GetByID(ThiSinhID ?? 0);
                    if (Data != null && Data.ThiSinhID > 0)
                    {
                        base.Message = ConstantLogMessage.API_Success;
                        base.Data = Data;
                    }
                    else
                    {
                        base.Message = ConstantLogMessage.API_NoData;
                    }
                    base.Status = 1;

                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //return base.GetActionResult();
                //throw;
            }
        }

        [HttpGet]
        [Route("GetThongTinCapBang")]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        public IActionResult GetThongTinCapBang([FromQuery] int? ThiSinhID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _ThongTinCapBangBUS.GetThongTinCapBang(ThiSinhID ?? 0);
                    if (Data != null && Data.ThiSinhID > 0)
                    {
                        if(Data.ThiSinhTrung1 > 0)
                        {
                            Data.ThiSinhTrung = _QuanLyThiSinhBUS.GetByID(Data.ThiSinhTrung1 ?? 0);
                        }
                        base.Message = ConstantLogMessage.API_Success;
                        base.Data = Data;
                    }
                    else
                    {
                        base.Message = ConstantLogMessage.API_NoData;
                    }
                    base.Status = 1;

                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Delete)]
        [Route("Duyet")]
        public IActionResult Update_TrangThaiBangCap(List<ThongTinThiSinh> ListThongTinThiSinh)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Delete, () =>
                {
                    var Result = _ThongTinCapBangBUS.Update_TrangThaiCapBang(ListThongTinThiSinh);
                    base.Message = Result.Message;
                    base.Status = Result.Status;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();

                //base.Status = -1;
                //base.GetActionResult();
                //throw;
            }
        }
    }
}