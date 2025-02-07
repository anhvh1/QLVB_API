using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Com.Gosol.QLVB.API.Authorization;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.BUS;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Security;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Com.Gosol.QLVB.API.Controllers
{
    [Route("api/v1/NhacViec")]
    [ApiController]
    public class NhacViecController : BaseApiController
    {
        private INhacViecBUS _INhacViecBUS;

        public NhacViecController(ILogHelper logHelper, INhacViecBUS _CongKhaiBanKeKhaiTaiSanBUS, ILogger<NhacViecController> logger) : base(logHelper, logger)
        {
            this._INhacViecBUS = _CongKhaiBanKeKhaiTaiSanBUS;
        }
        [HttpGet]
        [Route("GetViecLam")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetViecLam()
        {
            try
            {
                return CreateActionResult("Lấy danh sách nhắc việc", EnumLogType.Other, () =>
                {
                    //var Data = _INhacViecBUS.GetViecLam(Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0), Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0),
                    //    Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0));

                    int NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    int CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                    int CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    BasePagingParams p = new BasePagingParams();
                    p.PageSize = 1000;
                    int TotalRow = 0;
                    List<ThongBaoModel> Data = new List<ThongBaoModel>();
                    if (NguoiDungID != 1)
                    {
                        //Data = _INhacViecBUS.GetDSThongBaoHienThi(p, ref TotalRow, CanBoID, CoQuanID);
                    }

                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Data = Data;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = 1;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
            }
        }

        [HttpGet]
        [Route("SendMail")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult SendMail()
        {
            try
            {
                return CreateActionResult("SendMail", EnumLogType.Other, () =>
                {
                    //SmtpClient client = new SmtpClient("mysmtpserver");
                    //client.UseDefaultCredentials = false;
                    //client.Credentials = new NetworkCredential("username", "password");

                    //MailMessage mailMessage = new MailMessage();
                    //mailMessage.From = new MailAddress("whoever@me.com");
                    //mailMessage.To.Add("receiver@me.com");
                    //mailMessage.Body = "body";
                    //mailMessage.Subject = "subject";
                    //client.Send(mailMessage);

                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("vietjetkyc.team@gmail.com");
                        mail.To.Add("sonth@gosol.com.vn");
                        mail.Subject = "Hello World";
                        mail.Body = "<h1>Hello</h1>";
                        mail.IsBodyHtml = true;
                        //mail.Attachments.Add(new Attachment("C:\\file.zip"));

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new NetworkCredential("vietjetkyc.team@gmail.com", "vietjet@123");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }

                    base.Status = 1;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();
            }
        }

        [HttpPost]
        [Route("Edit_ThongBao")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult Edit_ThongBao(ThongBaoModel ThongBaoModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.KK_KeKhai_ThemDotKeKhai, EnumLogType.Insert, () =>
                {
                    var Result = _INhacViecBUS.Edit_ThongBao(ThongBaoModel);
                    base.Message = Result.Message;
                    base.Status = Result.Status;
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
        [Route("GetAllThongBao")]
        [CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult GetAllThongBao()
        {
            try
            {
                return CreateActionResult("GetAllThongBao", EnumLogType.Other, () =>
                {
                    var Data = _INhacViecBUS.GetAllThongBao();
                    base.Status = 1;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();
            }
        }

        [HttpGet]
        [Route("SendMailAuto")]
        //[CustomAuthAttribute(0, AccessLevel.Read)]
        public IActionResult SendMailAuto()
        {
            try
            {
                return CreateActionResult("SendMail", EnumLogType.Other, () =>
                {
                    var Data = _INhacViecBUS.SendMailAuto();

                    base.Status = 1;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ex.Message;
                return base.GetActionResult();
            }
        }
    }
}