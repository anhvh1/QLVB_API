using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.ThongKe;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.DanhMuc;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.API.Controllers.HeThong.ThongKe
{
    [Route("api/v1/ThongKe")]
    [ApiController]
    public class ThongKeController : BaseApiController
    {
        private IThongKeBUS _thongKeBUS;
        public ThongKeController(IThongKeBUS thongKeBUS, ILogHelper _LogHelper, ILogger<ThongKeController> logger) : base(_LogHelper, logger)
        {
            this._thongKeBUS = thongKeBUS;
        }

        [HttpGet]
        [Route("GetThongKe")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetListPaging([FromQuery] BasePagingParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.NV_ThongKe, EnumLogType.GetList, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    int TotalRow = 0;
                    List<ChiTietDuLieuDiemThiModel> Data;
                    Data = _thongKeBUS.GetThongKe(p, ref TotalRow);
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    if (Data.Count > 0)
                    {
                        base.Data = Data;
                    }
                    else
                    {
                        base.Message = Constant.NO_DATA;
                    }
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
                throw ex;
            }
        }
    }
}
