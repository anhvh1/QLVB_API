using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.QLVB;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.QLVB;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.API.Controllers.HeThong
{
    [Route("api/v1/MauPhieu")]
    [ApiController]
    public class MauPhieuController : BaseApiController
    {
        private IMauPhieuBUS _mauPhieuBUS;
        private IHostingEnvironment _host;
        public MauPhieuController(IMauPhieuBUS mauPhieuBUS, ILogHelper _LogHelper, ILogger<ChucNangController> logger, IHostingEnvironment host) : base(_LogHelper, logger)
        {
            this._mauPhieuBUS = mauPhieuBUS;
            this._host = host;
        }

        [HttpPost]
        [Route("Insert")]
        [CustomAuthAttribute(ChucNangEnum.MauPhieu, AccessLevel.Create)]
        public IActionResult Insert(MauPhieuModel _mauPhieuModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.NV_MauPhieu_Them, EnumLogType.Insert, () =>
                {
                    var Result = _mauPhieuBUS.Insert(_mauPhieuModel);
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
                throw ex;
            }

        }

        [HttpPost]
        [Route("Update")]
        [CustomAuthAttribute(ChucNangEnum.MauPhieu, AccessLevel.Edit)]
        public IActionResult Update(MauPhieuModel _mauPhieuModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.NV_MauPhieu_Sua, EnumLogType.Update, () =>
                {
                    var Result = _mauPhieuBUS.Update(_mauPhieuModel);
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
                throw ex;
            }
        }

        [HttpPost]
        [Route("Delete")]
        [CustomAuthAttribute(ChucNangEnum.MauPhieu, AccessLevel.Delete)]
        public IActionResult Delete([FromBody] BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.NV_MauPhieu_Xoa, EnumLogType.Delete, () =>
                {
                    var Result = _mauPhieuBUS.Delete(p.ListID);
                    if (Result.Count > 0)
                    {
                        base.Status = 0;
                        string Message = "";
                        foreach (var item in Result)
                        {
                            Message += item + "\n";
                        }
                        base.Message = Message;
                    }
                    else
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_Delete_Success;
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

        [HttpGet]
        [Route("GetListPaging")]
        [CustomAuthAttribute(ChucNangEnum.MauPhieu, AccessLevel.Read)]
        public IActionResult GetListPaging([FromQuery] BasePagingParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.NV_MauPhieu_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    List<MauPhieuModel> Data;
                    //var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    //var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    Data = _mauPhieuBUS.GetListPaging(p, ref TotalRow);
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Data;
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

        [HttpGet]
        [Route("GetByID")]
        [CustomAuthAttribute(ChucNangEnum.MauPhieu, AccessLevel.Read)]
        public IActionResult GetByID(int MauPhieuID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_SystemConfig_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _mauPhieuBUS.GetByID(MauPhieuID);
                    if (Data == null || Data.MauPhieuID == null || Data.MauPhieuID <= 0)
                    {
                        base.Message = "Không có Dữ liệu";
                        base.Status = 0;
                    }
                    else
                    {
                        base.Message = " ";
                        base.Status = 1;
                        base.Data = Data;
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

        [HttpGet]
        [Route("GetChiTietByNam")]
        [CustomAuthAttribute(ChucNangEnum.MauPhieu, AccessLevel.Read)]
        public IActionResult GatChiTietByNam(int Nam, string TenMauPhieu)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_SystemConfig_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _mauPhieuBUS.GetChiTietByNam(Nam, TenMauPhieu);
                    if (Data == null || Data.MauPhieuID == null || Data.MauPhieuID <= 0)
                    {
                        base.Message = "Không có Dữ liệu";
                        base.Status = 0;
                    }
                    else
                    {
                        base.Message = " ";
                        base.Status = 1;
                        base.Data = Data;
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

        [HttpGet]
        [Route("DownloadExcel")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public async Task<IActionResult> DownloadExcel(int? MauPhieuID)
        {
            try
            {
                int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                //return CreateActionResult(ConstantLogMessage.HT_CanBo_ExportFile, EnumLogType.Other, () =>
                //{
                //var host = _host.ContentRootPath;
                var RootPath = _host.ContentRootPath;
                    var cls = new Commons();
                    //string path = @"Upload\Temp\" + TenMauPhieu + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";
                    var result = _mauPhieuBUS.ImportDataToExcel(RootPath, MauPhieuID);
                    if (!string.IsNullOrEmpty(result))
                    {
                        base.Data = cls.GetServerPath(HttpContext) + result;
                        base.Status = 1;
                    }
                    else
                    {
                        base.Data = "";
                        base.Status = 0;
                        base.Message = "Dowload file mẫu lỗi!";
                    }
                    return base.GetActionResult();

                //});
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();
                //throw ex;
            }
        }

        [HttpPost]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("ReadExcelFile")]
        public async Task<IActionResult> ReadExcelFile(IFormFile file, int? Nam)
        {
            try
            {
                int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                //return CreateActionResult(ConstantLogMessage.HT_CanBo_ImportFile, EnumLogType.Other, async () =>
                //{
                string SavePath = "";
                if (file != null)
                {
                    SavePath = _host.ContentRootPath + "\\Upload\\" + "BieuMauRead.xlsx";
                    if (System.IO.File.Exists(SavePath))
                    {
                        System.IO.File.Delete(SavePath);
                    }
                    using (FileStream output = System.IO.File.Create(SavePath))
                        await file.CopyToAsync(output);
                }
                //var coQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                //var CanBoDangNhapID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                string Mess = "";
                var Result = _mauPhieuBUS.ReadFileExcel(SavePath, ref Mess, Nam);
                if (Result != null)
                {
                    base.Status = 1;
                    base.Message = Mess;
                    base.Data = Result;
                }
                else
                {
                    base.Status = 0;
                    base.Message = Mess;
                }
                return base.GetActionResult();
                //});

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("ReadFileExcelUpdate")]
        public async Task<IActionResult> ReadFileExcelUpdate(IFormFile file)
        {
            try
            {
                int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                string SavePath = "";
                if (file != null)
                {
                    SavePath = _host.ContentRootPath + "\\Upload\\" + "BieuMauRead.xlsx";
                    if (System.IO.File.Exists(SavePath))
                    {
                        System.IO.File.Delete(SavePath);
                    }
                    using (FileStream output = System.IO.File.Create(SavePath))
                        await file.CopyToAsync(output);
                }
                
                string Mess = "";
                var Result = _mauPhieuBUS.ReadFileExcelUpdate(SavePath, ref Mess);
                if (Result != null)
                {
                    base.Status = 1;
                    base.Message = Mess;
                    base.Data = Result;
                }
                else
                {
                    base.Status = 0;
                    base.Message = Mess;
                }
                return base.GetActionResult();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("ReadFileExcel_AllPages")]
        public async Task<IActionResult> ReadFileExcel_AllPages(IFormFile file, int? NamTotNghiep)
        {
            try
            {
                int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                string SavePath = "";
                if (file != null)
                {
                    SavePath = _host.ContentRootPath + "\\Upload\\" + "BieuMauRead.xlsx";
                    if (System.IO.File.Exists(SavePath))
                    {
                        System.IO.File.Delete(SavePath);
                    }
                    using (FileStream output = System.IO.File.Create(SavePath))
                        await file.CopyToAsync(output);
                }

                string Mess = "";
                var Result = _mauPhieuBUS.ReadFileExcel_AllPages(SavePath, ref Mess, NamTotNghiep);
                if (Result != null)
                {
                    base.Status = 1;
                    base.Message = Mess;
                    base.Data = Result;
                }
                else
                {
                    base.Status = 0;
                    base.Message = Mess;
                }
                return base.GetActionResult();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.NV_MauPhieu_GetAll, EnumLogType.GetList, () =>
                {
                    int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                    int TotalRow = 0;
                    List<MauPhieuModel> Data;
                    Data = _mauPhieuBUS.GetAll(ref TotalRow);
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Data;
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

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetAll_VBNN")]
        public IActionResult GetAll_VBNN()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.NV_MauPhieu_GetAll, EnumLogType.GetList, () =>
                {
                    int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                    int TotalRow = 0;
                    List<MauPhieuModel> Data;
                    Data = _mauPhieuBUS.GetAll_VBNN(ref TotalRow);
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Data;
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
