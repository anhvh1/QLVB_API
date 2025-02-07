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

namespace Com.Gosol.QLVB.API.Controllers.DanhMuc
{
    [Route("api/v1/DanhMucHoiDongThi")]
    [ApiController]
    public class DanhMucHoiDongThiController : BaseApiController
    {
        private IDanhMucHoiDongThiBUS _DanhMucHoiDongThiBUS;
        private IHostingEnvironment _host;
        private readonly ILogger _logger;
        public DanhMucHoiDongThiController(IDanhMucHoiDongThiBUS DanhMucHoiDongThiBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<DanhMucHoiDongThiController> logger) : base(_LogHelper, logger)
        {
            this._DanhMucHoiDongThiBUS = DanhMucHoiDongThiBUS;
            this._host = hostingEnvironment;
            _logger = logger;
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_HoiDongThi, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(DanhMucHoiDongThiModel DanhMucHoiDongThiModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_HoiDongThi_Them, EnumLogType.Insert, () =>
                {
                    var Result = _DanhMucHoiDongThiBUS.Insert(DanhMucHoiDongThiModel);
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
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_HoiDongThi, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(DanhMucHoiDongThiModel DanhMucHoiDongThiModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_HoiDongThi_Sua, EnumLogType.Update, () =>
                {
                    var Result = _DanhMucHoiDongThiBUS.Update(DanhMucHoiDongThiModel);
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
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_HoiDongThi, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete(DanhMucHoiDongThiModel p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_HoiDongThi_Xoa, EnumLogType.Delete, () =>
                {
                    var Result = _DanhMucHoiDongThiBUS.Delete(p.HoiDongThiID);
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
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_HoiDongThi, AccessLevel.Read)]
        public IActionResult GetByID([FromQuery]int? HoiDongThiID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_HoiDongThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _DanhMucHoiDongThiBUS.GetByID(HoiDongThiID ?? 0);
                    if (Data != null && Data.HoiDongThiID > 0)
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
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_HoiDongThi, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetListPaging([FromQuery]BasePagingParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_HoiDongThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<DanhMucHoiDongThiModel> Data;
                    Data = _DanhMucHoiDongThiBUS.GetPagingBySearch(p, ref TotalRow);
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
        [Route("GetHoiDongThiByNam")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetHoiDongThiByNam([FromQuery] int? NamThi)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_HoiDongThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var Data = _DanhMucHoiDongThiBUS.GetHoiDongThiByNam(NamThi ?? 0);
                    if (Data != null && Data.Count > 0)
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
    }
}