using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.QLVB;
using Com.Gosol.QLVB.Models.QLVB;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Com.Gosol.QLVB.API.Controllers.QLVB
{
    [Route("api/v1/DashBoard")]
    [ApiController]
    public class DashBoardController : BaseApiController
    {
        private IDashBoardBUS _DashBoardBUS;
        private IHostingEnvironment _host;
        private readonly ILogger _logger;
        public DashBoardController(IDashBoardBUS DashBoardBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<DashBoardController> logger) : base(_LogHelper, logger)
        {
            this._DashBoardBUS = DashBoardBUS;
            this._host = hostingEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("SoLuong5NamGanDay")]
        public IActionResult SoLuong5NamGanDay()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    SoLuong5NamGanDay Data;
                    Data = _DashBoardBUS.SoLuong5NamGanDay();
                    if (Data == null)
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
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("SoLuong10NamGanDay")]
        public IActionResult SoLuong10NamGanDay()
        {
            try
            {
                int TotalRow = 0;
                List<SoLuongThiSinhDuThiVaDo> Data = new List<SoLuongThiSinhDuThiVaDo>();
                Data = _DashBoardBUS.SoLuong10NamGanDay();
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return base.GetActionResultErrorAPI();
            }
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("SoLuongThiSinhDuThiVaDo")]
        public IActionResult SoLuongThiSinhDuThiVaDo([FromQuery] int? TuNam, [FromQuery] int? DenNam)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    List<SoLuongThiSinhDuThiVaDo> Data = new List<SoLuongThiSinhDuThiVaDo>();
                    Data = _DashBoardBUS.SoLuongThiSinhDuThiVaDo(TuNam ?? 0, DenNam ?? 0);
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