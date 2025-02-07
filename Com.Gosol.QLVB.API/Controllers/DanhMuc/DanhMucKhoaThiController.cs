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
    [Route("api/v1/DanhMucKhoaThi")]
    [ApiController]
    public class DanhMucKhoaThiController : BaseApiController
    {
        private IDanhMucKhoaThiBUS _DanhMucKhoaThiBUS;
        private IHostingEnvironment _host;
        private readonly ILogger _logger;
        public DanhMucKhoaThiController(IDanhMucKhoaThiBUS DanhMucKhoaThiBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<DanhMucKhoaThiController> logger) : base(_LogHelper, logger)
        {
            this._DanhMucKhoaThiBUS = DanhMucKhoaThiBUS;
            this._host = hostingEnvironment;
            _logger = logger;
        }

        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_KhoaThi, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(DanhMucKhoaThiModel DanhMucKhoaThiModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Them, EnumLogType.Insert, () =>
                {
                    var Result = _DanhMucKhoaThiBUS.Insert(DanhMucKhoaThiModel);
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
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_KhoaThi, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(DanhMucKhoaThiModel DanhMucKhoaThiModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Sua, EnumLogType.Update, () =>
                {
                    var Result = _DanhMucKhoaThiBUS.Update(DanhMucKhoaThiModel);
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
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_KhoaThi, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete(DanhMucKhoaThiModel p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_Xoa, EnumLogType.Delete, () =>
                {
                    var Result = _DanhMucKhoaThiBUS.Delete(p.KhoaThiID);
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
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_KhoaThi, AccessLevel.Read)]
        public IActionResult GetByID([FromQuery]int? KhoaThiID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _DanhMucKhoaThiBUS.GetByID(KhoaThiID ?? 0);
                    if (Data != null && Data.KhoaThiID > 0)
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
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_KhoaThi, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetListPaging([FromQuery]BasePagingParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<DanhMucKhoaThiModel> Data;
                    Data = _DanhMucKhoaThiBUS.GetPagingBySearch(p, ref TotalRow);
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
    }
}