using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.FileDinhKem;
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
    [Route("api/v1/QuanLyThiSinh")]
    [ApiController]
    public class QuanLyThiSinhController : BaseApiController
    {
        private IQuanLyThiSinhBUS _QuanLyThiSinhBUS;
        private IFileDinhKemBUS _FileDinhKemBUS;
        private IHostingEnvironment _host;
        private readonly ILogger _logger;
        public QuanLyThiSinhController(IQuanLyThiSinhBUS QuanLyThiSinhBUS, IFileDinhKemBUS FileDinhKemBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<QuanLyThiSinhController> logger) : base(_LogHelper, logger)
        {
            this._QuanLyThiSinhBUS = QuanLyThiSinhBUS;
            this._FileDinhKemBUS = FileDinhKemBUS;
            this._host = hostingEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.BangDiemThi, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetListPaging([FromQuery]BasePagingParams p, [FromQuery] int? NamThi, [FromQuery] int? Truong)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinThiSinh> Data;
                    Data = _QuanLyThiSinhBUS.GetPagingBySearch(p, NamThi ?? 0, Truong ?? 0, ref TotalRow);
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
        public IActionResult Insert(List<ThongTinThiSinh> ListThongTinThiSinh)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_DuLieuDiemThi_Them, EnumLogType.Insert, () =>
                {
                    var Result = _QuanLyThiSinhBUS.Insert(ListThongTinThiSinh);
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
                    var Result = _QuanLyThiSinhBUS.Update(ThongTinThiSinh);
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
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetByID([FromQuery]int? ThiSinhID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _QuanLyThiSinhBUS.GetByID(ThiSinhID ?? 0);
                    if (Data != null && Data.ThiSinhID > 0)
                    {
                        var listFileDinhKem = _FileDinhKemBUS.GetByNghiepVuID(Data.KyThiID ?? 0, EnumLoaiFileDinhKem.FileBangDiem.GetHashCode());
                        if (listFileDinhKem != null && listFileDinhKem.Count > 0)
                        {
                            var clsCommon = new Commons();
                            string serverPath = clsCommon.GetServerPath(HttpContext);
                            foreach (var item in listFileDinhKem)
                            {
                                var path = _host.ContentRootPath + "\\" + item.FileUrl;
                                item.FileUrl = serverPath + item.FileUrl;
                                //try
                                //{
                                //    Byte[] bytes = System.IO.File.ReadAllBytes(path);
                                //    item.Base64String = Convert.ToBase64String(bytes);
                                //    item.TepByte = bytes;
                                //}
                                //catch (Exception) { }
                            }
                        }
                        Data.DSFileDinhKem = listFileDinhKem;
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