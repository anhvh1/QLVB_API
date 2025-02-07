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
    [Route("api/v1/TraCuuVanBang")]
    [ApiController]
    public class TraCuuVanBangController : BaseApiController
    {
        private IThongTinCapBangBUS _ThongTinCapBangBUS;
        private IHostingEnvironment _host;
        private readonly ILogger _logger;
        public TraCuuVanBangController(IThongTinCapBangBUS ThongTinCapBangBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<TraCuuVanBangController> logger) : base(_LogHelper, logger)
        {
            this._ThongTinCapBangBUS = ThongTinCapBangBUS;
            this._host = hostingEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult TraCuuVanBangChungChi([FromQuery]TraCuuParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinThiSinh> Data;
                    if((p.SoHieuBang != null && p.SoHieuBang.Length > 0) 
                    || (p.CMND != null && p.CMND.Length >0)
                    || (p.HoTen != null && p.HoTen.Length > 0)
                    || (p.DonViDaoTao != null && p.DonViDaoTao > 0)
                    || (p.Lop != null && p.Lop.Length > 0)
                    || (p.Keyword != null && p.Keyword.Length > 0))
                    { 
                        Data = _ThongTinCapBangBUS.TraCuuVanBangChungChi(p, ref TotalRow);
                    }
                    else Data = new List<ThongTinThiSinh>();

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
        [Route("TraCuuVanBangChungChi")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult TraCuuVanBangChungChi_New([FromBody] TraCuuParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinThiSinh> Data;
                    //if ((p.SoHieuBang != null && p.SoHieuBang.Length > 0)
                    //|| (p.NamCapBang != null && p.NamCapBang > 0)
                    //|| (p.CMND != null && p.CMND.Length > 0)
                    //|| (p.HoTen != null && p.HoTen.Length > 0)
                    //|| (p.DonViDaoTao != null && p.DonViDaoTao > 0)
                    //|| (p.Lop != null && p.Lop.Length > 0)
                    //|| (p.Keyword != null && p.Keyword.Length > 0))
                    //{
                       
                        p.TrangThai = 0;
                        Data = _ThongTinCapBangBUS.TraCuuVanBangChungChi(p, ref TotalRow);
                    //}
                    //else Data = new List<ThongTinThiSinh>();

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
        [Route("TraCuuVanBangChungChiPublish")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult TraCuuVanBangChungChiPublish([FromBody] TraCuuParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinThiSinh> Data;
                    //if ((p.SoHieuBang != null && p.SoHieuBang.Length > 0)
                    //|| (p.NamCapBang != null && p.NamCapBang > 0)
                    //|| (p.CMND != null && p.CMND.Length > 0)
                    //|| (p.HoTen != null && p.HoTen.Length > 0)
                    //|| (p.DonViDaoTao != null && p.DonViDaoTao > 0)
                    //|| (p.Lop != null && p.Lop.Length > 0)
                    //|| (p.Keyword != null && p.Keyword.Length > 0))
                    //{
                        p.TrangThai = 1;
                        Data = _ThongTinCapBangBUS.TraCuuVanBangChungChi(p, ref TotalRow);
                    //}
                    //else Data = new List<ThongTinThiSinh>();

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
                //_logger.LogError(ex.ToString());
                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();
                //return base.GetActionResultErrorAPI();
            }
        }

        [HttpPost]
        [Route("TraCuuVanBangChungChiNuocNgoaiCap")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult TraCuuVanBangChungChiNuocNgoaiCap([FromBody] TraCuuParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_KhoaThi_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinThiSinh> Data;
                    //if ((p.SoHieuBang != null && p.SoHieuBang.Length > 0)
                    //|| (p.NamCapBang != null && p.NamCapBang > 0)
                    //|| (p.CMND != null && p.CMND.Length > 0)
                    //|| (p.HoTen != null && p.HoTen.Length > 0)
                    //|| (p.DonViDaoTao != null && p.DonViDaoTao > 0)
                    //|| (p.Lop != null && p.Lop.Length > 0)
                    //|| (p.Keyword != null && p.Keyword.Length > 0))
                    //{
                        p.TrangThai = 1;
                        Data = _ThongTinCapBangBUS.TraCuuVanBangChungChiNuocNgoaiCap(p, ref TotalRow);
                    //}
                    //else Data = new List<ThongTinThiSinh>();

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
            }
        }
    }
}