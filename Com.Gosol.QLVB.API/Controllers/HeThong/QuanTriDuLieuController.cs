using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.Security;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Com.Gosol.QLVB.API.Controllers.HeThong
{
    [Route("api/v1/QuanTriDuLieu")]
    [ApiController]
    public class QuanTriDuLieuController : BaseApiController
    {
        private IQuanTriDuLieuBUS _QuanTriDuLieuBUS;
        private ILogHelper LogHelper;
        private readonly ILogger<QuanTriDuLieuController> _logger;
        private IHostingEnvironment _host;
        public QuanTriDuLieuController(IQuanTriDuLieuBUS QuanTriDuLieuBUS, ILogHelper _LogHelper, ILogger<QuanTriDuLieuController> logger, IHostingEnvironment HostingEnvironment) : base(_LogHelper)
        {
            this._QuanTriDuLieuBUS = QuanTriDuLieuBUS;
            this.LogHelper = _LogHelper;
            _logger = logger;
            this._host = HostingEnvironment;
        }

        [HttpGet]
        [Route("BackupDatabase")]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanTri_DuLieu, AccessLevel.FullAccess)]
        public IActionResult BackupDatabase(string fileName)
        {
            try
            {
                int val = 0;
                val = _QuanTriDuLieuBUS.BackupData(fileName, _host.ContentRootPath);
                if (val == -1)
                {
                    base.Message = ConstantLogMessage.API_Error_System;
                    return base.GetActionResult();
                }
                if (val == 0)
                {
                    base.Message = ConstantLogMessage.Alert_Error_Exist("Tên file sao lưu - " + "\"" + fileName + "\"");
                    return base.GetActionResult();
                }
                else
                {
                    base.Message = "Sao lưu dữ liệu thành công";
                    return CreateActionResult("Sao lưu dữ liệu - QLVB_" + fileName, EnumLogType.BackupDatabase, () =>
                    {
                        // _logger.LogInformation(User.Claims.FirstOrDefault(c => c.Type == "TenCanBo").Value.ToString() + " - Sao lưu dữ liệu ", fileName);
                        base.Status = val;
                        base.Data = Data;
                        return base.GetActionResult();
                    });
                }

                //return CreateActionResult(ConstantLogMessage.HT_QuanTriDuLieu_BackupDatabase, LogType.BackupDatabase, () =>
                //{
                //    int val = 0;
                //    val = _QuanTriDuLieuBUS.BackupData(fileName);
                //    if (val == -1)
                //    {
                //        base.Message = ConstantLogMessage.API_Error_System;
                //    }
                //    if (val == 0)
                //    {
                //        base.Message = ConstantLogMessage.Alert_Error_Exist(fileName);
                //    }
                //    else
                //    {
                //        base.Message = "Sao lưu dữ liệu thành công";
                //    }
                //    base.Status = val;
                //    base.Data = Data;
                //    return base.GetActionResult();
                //});
            }
            catch (Exception)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
                throw;
            }

        }
        [HttpGet]
        [Route("RestoreDatabase")]
        [CustomAuthAttribute(ChucNangEnum.HeThong_QuanTri_DuLieu, AccessLevel.FullAccess)]
        public IActionResult RestoreDatabase(string fileName)
        {
            try
            {
                int val = 0;
                val = _QuanTriDuLieuBUS.RestoreDatabase(fileName);
                if (val == -1)
                {
                    base.Message = ConstantLogMessage.API_Error_System;
                    return base.GetActionResult();
                }
                if (val == 0)
                {
                    base.Message = ConstantLogMessage.Alert_Error_NotExist("File sao lưu - " + "\"" + fileName + "\"");
                    return base.GetActionResult();
                }
                else
                {
                    base.Message = "Phục hồi dữ liệu thành công";
                    base.Status = val;
                    base.Data = Data;
                    return CreateActionResult("Phục hồi dữ liệu - " + fileName, EnumLogType.RestoreDatabase, () =>
                    {
                        // _logger.LogInformation(User.Claims.FirstOrDefault(c => c.Type == "TenCanBo").Value.ToString() + " - Phục hồi dữ liệu ", fileName);
                        return base.GetActionResult();
                    });

                }

                //return CreateActionResult(ConstantLogMessage.HT_QuanTriDuLieu_RestoreDatabase, LogType.RestoreDatabase, () =>
                //{
                //    if (val == -1)
                //    {
                //        base.Message = ConstantLogMessage.API_Error_System;
                //    }
                //    if (val == 0)
                //    {
                //        base.Message = ConstantLogMessage.Alert_Error_NotExist(fileName);
                //    }
                //    else
                //    {
                //        base.Message = "Phục hồi dữ liệu thành công";
                //    }
                //    base.Status = val;
                //    base.Data = Data;
                //    return base.GetActionResult();
                //});
            }
            catch (Exception)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
                throw;
            }

        }


        [HttpGet]
        [Route("GetFileInDerectory")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetFileInDerectory()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_QuanTriDuLieu_GetListFileBackup, EnumLogType.GetList, () =>
                {
                    int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0);
                    int val = 0;
                    var Data = _QuanTriDuLieuBUS.GetFileInDerectory();
                    if (Data.Count < 1)
                    {
                        base.Message = ConstantLogMessage.API_NoData;
                        val = 1;
                    }
                    else if (Data == null)
                    {
                        base.Message = ConstantLogMessage.API_Error_System;
                        val = 0;
                    }
                    else
                    {
                        base.Message = " ";
                        val = 1;
                    }
                    base.Status = val;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                return base.GetActionResult();

                throw;
            }

        }

    }
}