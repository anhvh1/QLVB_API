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
    [Route("api/v1/DanhMucChung")]
    [ApiController]
    public class DanhMucChungController : BaseApiController
    {
        private IDanhMucChungBUS _DanhMucChungBUS;
        private IHostingEnvironment _host;
        private readonly ILogger _logger;
        public DanhMucChungController(IDanhMucChungBUS DanhMucChungBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<DanhMucChungController> logger) : base(_LogHelper, logger)
        {
            this._DanhMucChungBUS = DanhMucChungBUS;
            this._host = hostingEnvironment;
            _logger = logger;
        }

        [HttpPost]
        [CustomAuthAttribute(0, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(DanhMucChungModel DanhMucChungModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_Chung_Them, EnumLogType.Insert, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var Result = _DanhMucChungBUS.Insert(DanhMucChungModel);
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
        [CustomAuthAttribute(0, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(DanhMucChungModel DanhMucChungModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_Chung_Sua, EnumLogType.Update, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var Result = _DanhMucChungBUS.Update(DanhMucChungModel);
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
        [CustomAuthAttribute(0, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete([FromBody] BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_Chung_Xoa, EnumLogType.Delete, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var Result = _DanhMucChungBUS.Delete(p.ID ?? 0);
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
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetByID([FromQuery] int? ID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_Chung_GetByID, EnumLogType.GetByID, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var Data = _DanhMucChungBUS.GetByID(ID ?? 0);
                    if (Data != null && Data.ID > 0)
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
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetListPaging([FromQuery] BasePagingParams p, [FromQuery] int? LoaiDanhMuc)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_Chung_GetListPaging, EnumLogType.GetList, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    int TotalRow = 0;
                    IList<DanhMucChungModel> Data;
                    Data = _DanhMucChungBUS.GetPagingBySearch(p, LoaiDanhMuc ?? 0, ref TotalRow);
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
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetAll")]
        public IActionResult GetAll([FromQuery] int? LoaiDanhMuc, int? Nam)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_Chung_GetListPaging, EnumLogType.GetList, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    IList<DanhMucChungModel> Data;
                    Data = _DanhMucChungBUS.GetAll(LoaiDanhMuc ?? 0, Nam ?? 0);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        base.Data = new List<DanhMucChungModel>();
                        return base.GetActionResult();
                    }
                    base.Status = 1;
                    base.TotalRow = Data.Count;
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
        [Route("GetTruongByNam")]
        public IActionResult GetListPaging([FromQuery] int? NamThi)
        {
            try
            {
                var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                int TotalRow = 0;
                IList<DanhMucChungModel> Data;
                Data = _DanhMucChungBUS.GetTruongByNam(NamThi ?? 0);
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

                //base.Status = -1;
                //base.GetActionResult();
                //throw;
            }
        }
    }
}