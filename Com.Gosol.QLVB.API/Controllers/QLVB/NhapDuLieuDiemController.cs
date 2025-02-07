using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.DanhMuc;
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
    [Route("api/v1/NhapDuLieuDiem")]
    [ApiController]
    public class NhapDuLieuDiemController : BaseApiController
    {
        private IDuLieuDiemThiBUS _DuLieuDiemThiBUS;
        private IHostingEnvironment _host;
        private readonly ILogger _logger;
        public NhapDuLieuDiemController(IDuLieuDiemThiBUS DuLieuDiemThiBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<NhapDuLieuDiemController> logger) : base(_LogHelper, logger)
        {
            this._DuLieuDiemThiBUS = DuLieuDiemThiBUS;
            this._host = hostingEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetListPaging([FromQuery] BasePagingParams p, [FromQuery] int? HoiDongID, [FromQuery] int? KhoaThiID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinToChucThi> Data;
                    Data = _DuLieuDiemThiBUS.GetPagingBySearch_NhapDuLieuDiem(p, HoiDongID ?? 0, KhoaThiID ?? 0, ref TotalRow);
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
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(DuLieuDiemThiModel DuLieuDiemThiModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_DuLieuDiemThi_Them, EnumLogType.Insert, () =>
                {
                    var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                    var Result = _DuLieuDiemThiBUS.Insert(DuLieuDiemThiModel, CanBoID);
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
        public IActionResult Update(DuLieuDiemThiModel DuLieuDiemThiModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Update, () =>
                {
                    var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                    var Result = _DuLieuDiemThiBUS.Update(DuLieuDiemThiModel, CanBoID);
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
        public IActionResult Delete(ThongTinToChucThi p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Xoa, EnumLogType.Delete, () =>
                {
                    var Result = _DuLieuDiemThiBUS.DeleteDuLieuTep(p.KyThiID);
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

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Delete)]
        [Route("DeleteAll")]
        public IActionResult Delete(List<int> ListID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Xoa, EnumLogType.Delete, () =>
                {
                    string Message = "";
                    foreach (var ID in ListID)
                    {
                        var Result = _DuLieuDiemThiBUS.DeleteDuLieuTep(ID);
                        if (Result.Status != 1) Message += Result.Message + " ";
                    }
                   
                    base.Message = Message;
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

        [HttpGet]
        [Route("GetByID")]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        public IActionResult GetByID([FromQuery] int? KyThiID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _DuLieuDiemThiBUS.GetByID(KyThiID ?? 0);
                    if (Data != null && Data.ThongTinToChucThi != null && Data.ThongTinToChucThi.KyThiID > 0)
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

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Delete)]
        [Route("Duyet")]
        public IActionResult Update_TrangThai(List<ThongTinToChucThi> ListThongTinToChucThi)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Delete, () =>
                {
                    var Result = _DuLieuDiemThiBUS.Update_TrangThai(ListThongTinToChucThi);
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