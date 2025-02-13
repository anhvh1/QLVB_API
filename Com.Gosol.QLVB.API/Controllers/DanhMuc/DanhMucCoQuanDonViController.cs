﻿using System;
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
using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.BUS.DanhMuc;
using Com.Gosol.QLVB.Models.DanhMuc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Workflow;

namespace Com.Gosol.QLVB.API.Controllers.DanhMuc
{
    [Route("api/v1/DanhMucCoQuanDonVi")]
    [ApiController]
    public class DanhMucCoQuanDonViController : BaseApiController
    {
        private IDanhMucCoQuanDonViBUS _DanhMucCoQuanDonViBUS;
        private IHostingEnvironment _host;
        public DanhMucCoQuanDonViController(IDanhMucCoQuanDonViBUS DanhMucCoQuanDonViBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<DanhMucCoQuanDonViController> logger) : base(_LogHelper, logger)
        {
            this._DanhMucCoQuanDonViBUS = DanhMucCoQuanDonViBUS;
            this._host = hostingEnvironment;
        }
        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_ThemCoQuanDonVi, EnumLogType.Insert, () =>
                 {

                     string Message = null;
                     int val = 0;
                     int CoQuanID = 0;
                     val = _DanhMucCoQuanDonViBUS.Insert(DanhMucCoQuanDonViModel, ref CoQuanID, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0), ref Message);
                     base.Message = Message;
                     base.Status = val;
                     //base.Data = Data;
                     return base.GetActionResult();
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_SuaCoQuanDonVi, EnumLogType.Update, () =>
                 {
                     string Message = null;
                     int val = 0;
                     val = _DanhMucCoQuanDonViBUS.Update(DanhMucCoQuanDonViModel, ref Message);
                     base.Message = Message;
                     base.Status = val;
                     //base.Data = Data;
                     return base.GetActionResult();
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        // CHưa Sửa
        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete([FromBody] BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_XoaCoQuanDonVi, EnumLogType.Delete, () =>
                {
                    Dictionary<int, string> dic = new Dictionary<int, string>();
                    dic = _DanhMucCoQuanDonViBUS.Delete(p.ListID);
                    base.Status = dic.FirstOrDefault().Key;
                    base.Message = dic.FirstOrDefault().Value;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("FilterByName")]
        public IActionResult FilterByName(string TenCoQuan)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_FilterByName, EnumLogType.GetList, () =>
                 {
                     var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                     int val = 0;
                     List<DanhMucCoQuanDonViModel> Data;
                     Data = _DanhMucCoQuanDonViBUS.FilterByName(TenCoQuan);
                     base.Status = val > 0 ? 1 : 0;
                     base.Data = Data;
                     return base.GetActionResult();
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetByIDAndCap")]
        public IActionResult GetByIDAndCap(int CoQuanID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetByID, EnumLogType.GetList, () =>
                 {
                     var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                     DanhMucCoQuanDonViPartialNew Data;
                     Data = _DanhMucCoQuanDonViBUS.GetByID(CoQuanID);
                     base.Status = Data.CoQuanID > 0 ? 1 : 0;
                     base.Data = Data;
                     base.Message = Data.CoQuanID > 0 ? ConstantLogMessage.API_Success : ConstantLogMessage.API_Error;
                     return base.GetActionResult();
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetListByidAndCap")]
        public IActionResult GetListByidAndCap()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetByID, EnumLogType.GetList, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    int val = 0;
                    List<DanhMucCoQuanDonViModel> Data;
                    Data = _DanhMucCoQuanDonViBUS.GetListByidAndCap();
                    base.Status = val > 0 ? 1 : 0;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }

            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetAllByCap")]
        public IActionResult GetAllByCap([FromQuery]int ID, int Cap, string Keyword)
        {
            try
            {

                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetAllByCap, EnumLogType.GetList, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    IList<DanhMucCoQuanDonViModelPartial> Data;
                    Data = _DanhMucCoQuanDonViBUS.GetAllByCap(ID, Cap, Keyword);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = Data.Count > 0 ? 1 : 0;
                    base.Data = Data;
                    base.TotalRow = Data.Count;
                    return base.GetActionResult();
                });
            }
            catch
            {
                base.Status = -1;
                return base.GetActionResult();
            }
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetAll")]
        public IActionResult GetAll([FromQuery]int ID, string Keyword)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetAllByCap, EnumLogType.GetList, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    IList<DanhMucCoQuanDonViModelPartial> Data;
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                    Data = _DanhMucCoQuanDonViBUS.GetALL(ID, CoQuanID, Keyword);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = Data.Count > 0 ? 1 : 0;
                    base.Data = Data;
                    base.TotalRow = Data.Count;
                    return base.GetActionResult();
                });
            }
            catch
            {
                base.Status = -1;
                return base.GetActionResult();
            }
        }
        [HttpGet]
        [Route("GetListByUser")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult PhanQuyen_GetDanhMucCoQuan()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetForPhanQuyen, EnumLogType.GetList, () =>
                {
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var result = _DanhMucCoQuanDonViBUS.GetListByUser(CoQuanID, NguoiDungID);
                    base.Status = 1;
                    base.Data = result;
                    base.Message = result.Count < 1 ? "Không có dữ liệu Cơ quan" : "";
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
        [Route("GetListByUser_FoPhanQuyen")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetListByUser_FoPhanQuyen(string Keyword)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetForPhanQuyen, EnumLogType.GetList, () =>
                {
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var result = _DanhMucCoQuanDonViBUS.GetByUser_FoPhanQuyen(CoQuanID, NguoiDungID, Keyword);
                    base.Status = 1;
                    base.Data = result;
                    base.Message = result.Count < 1 ? "Không có dữ liệu Cơ quan" : "";
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
        [Route("GetByUser_FoPhanQuyen")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetByUser_FoPhanQuyen(string Keyword)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetForPhanQuyen, EnumLogType.GetList, () =>
                {
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var result = _DanhMucCoQuanDonViBUS.GetByUser_FoPhanQuyen(CoQuanID, NguoiDungID, Keyword);
                    base.Status = 1;
                    base.Data = result;
                    base.Message = result.Count < 1 ? "Không có dữ liệu Cơ quan" : "";
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
        [CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Read)]
        [Route("ImportCoQuan")]
        public async Task<IActionResult> ImportCoQuan([FromBody]Files file)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_ChucVu_ImportFile, EnumLogType.Other, () =>
                {
                    string Message = "";
                    string SavePath = _host.ContentRootPath + "\\Upload\\" + "Import_ChucVu.xlsx";
                    using (FileStream stream = System.IO.File.Create(SavePath))
                    {
                        byte[] byteArray = Convert.FromBase64String(file.files);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }

                    var Result = _DanhMucCoQuanDonViBUS.ImportFile(SavePath, ref Message, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0));
                    base.Status = Result;
                    base.Message = Message;
                    base.Data = Data;
                    return base.GetActionResult();
                });

            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("CheckMaCQ")]
        public IActionResult CheckMaCQ([FromQuery] int? CoQuanID,string MaCQ)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetByID, EnumLogType.GetList, () =>
                {
                   
                    
                    var Data = _DanhMucCoQuanDonViBUS.CheckMaCQ(CoQuanID,MaCQ);
                    base.Status = Data.Status ;
                    base.Message = Data.Message;
                    //base.Data = Data;
                    return base.GetActionResult();
                });
            }

            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetAllByCoQuanDangNhap")]
        public IActionResult GetAllByCoQuanDangNhap()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetAllByCap, EnumLogType.GetList, () =>
                {
                    IList<DanhMucCoQuanDonViModel> Data;
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    Data = _DanhMucCoQuanDonViBUS.GetAll(CoQuanID, NguoiDungID);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = Data.Count > 0 ? 1 : 0;
                    base.Data = Data;
                    base.TotalRow = Data.Count;
                    return base.GetActionResult();
                });
            }
            catch
            {
                base.Status = -1;
                return base.GetActionResult();
            }
        }

        [HttpGet]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        [Route("GetAllWorkflow")]
        public IActionResult GetAllWorkflow()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetAllByCap, EnumLogType.GetList, () =>
                {
                    WorkflowInstance wf = new WorkflowInstance();
                    var Data = wf.GetAllWorkflow();
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = Data.Count > 0 ? 1 : 0;
                    base.Data = Data;
                    base.TotalRow = Data.Count;
                    return base.GetActionResult();
                });
            }
            catch
            {
                base.Status = -1;
                return base.GetActionResult();
            }
        }
    }
}