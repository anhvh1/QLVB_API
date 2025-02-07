using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Com.Gosol.QLVB.API.Formats;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.BUS.HeThong;
using Microsoft.Extensions.Options;
using Com.Gosol.QLVB.API.Config;
using Com.Gosol.QLVB.Ultilities;
using System.Security.Claims;
using Com.Gosol.QLVB.API.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Com.Gosol.QLVB.Security;
using Microsoft.Extensions.Logging;
using Com.Gosol.QLVB.BUS.FileDinhKem;
using Com.Gosol.QLVB.BUS;

namespace Com.Gosol.QLVB.API.Controllers.HeThong
{
    [Route("api/v1/Nguoidung")]
    [ApiController]
    public class NguoidungController : ControllerBase
    {
        private IOptions<AppSettings> _AppSettings;
        private INguoiDungBUS _NguoiDungBUS;
        private IPhanQuyenBUS _PhanQuyenBUS;
        private ILogHelper _ILogHelper;
        private ILogger logger;
        private IFileDinhKemBUS _FileDinhKemBUS;
        private ISystemConfigBUS _systemConfigBUS;
        private INhacViecBUS _nhacViecBUS;
        private IHeThongCanBoBUS _HeThongCanBoBUS;
        public NguoidungController(IOptions<AppSettings> Settings, INguoiDungBUS NguoiDungBUS, IPhanQuyenBUS PhanQuyenBUS, IFileDinhKemBUS FileDinhKemBUS, IHeThongCanBoBUS HeThongCanBoBUS, ILogHelper LogHelper, ILogger<NguoidungController> logger, ISystemConfigBUS systemConfigBUS, INhacViecBUS nhacViecBUS)
        {
            _AppSettings = Settings;
            _NguoiDungBUS = NguoiDungBUS;
            _PhanQuyenBUS = PhanQuyenBUS;
            _ILogHelper = LogHelper;
            this.logger = logger;
            this._FileDinhKemBUS = FileDinhKemBUS;
            this._systemConfigBUS = systemConfigBUS;
            this._nhacViecBUS = nhacViecBUS;
            this._HeThongCanBoBUS = HeThongCanBoBUS;
        }
        [Route("DangNhap")]
        [HttpPost]
        public IActionResult Login(LoginModel User)
        {
            try
            {
                string Password = Cryptor.EncryptPasswordUser(User.UserName.Trim().ToLower(), User.Password);
                NguoiDungModel NguoiDung = null;
                if (_NguoiDungBUS.VerifyUser(User.UserName.Trim(), Password, User.Email, ref NguoiDung))
                {
                    Task.Run(() => _ILogHelper.Log(NguoiDung.CanBoID, "Đăng nhập hệ thống", (int)EnumLogType.DangNhap));
                    var claims = new List<Claim>();
                    var ListChucNang = _PhanQuyenBUS.GetListChucNangByNguoiDungID(NguoiDung.NguoiDungID);
                    //string ClaimFull = "," + string.Join(",", ListChucNang.Where(t => t.Quyen == (int)AccessLevel.FullAccess).Select(t => new { t.ChucNangID }).ToList()) + ",";

                    string ClaimRead = "," + string.Join(",", ListChucNang.Where(t => t.Xem == 1).Select(t => t.ChucNangID).ToArray()) + ",";
                    string ClaimCreate = "," + string.Join(",", ListChucNang.Where(t => t.Them == 1).Select(t => t.ChucNangID).ToArray()) + ",";
                    string ClaimEdit = "," + string.Join(",", ListChucNang.Where(t => t.Sua == 1).Select(t => t.ChucNangID).ToArray()) + ",";
                    string ClaimDelete = "," + string.Join(",", ListChucNang.Where(t => t.Xoa == 1).Select(t => t.ChucNangID).ToArray()) + ",";
                    string ClaimFullAccess = "," + string.Join(",", ListChucNang.Where(t => t.Xem == 1 && t.Them == 1 && t.Sua == 1 && t.Xoa == 1).Select(t => t.ChucNangID).ToArray()) + ",";

                    //claims.Add(new Claim(PermissionLevel.FULLACCESS, ClaimFull));
                    claims.Add(new Claim(PermissionLevel.READ, ClaimRead));
                    claims.Add(new Claim(PermissionLevel.CREATE, ClaimCreate));
                    claims.Add(new Claim(PermissionLevel.EDIT, ClaimEdit));
                    claims.Add(new Claim(PermissionLevel.DELETE, ClaimDelete));
                    claims.Add(new Claim(PermissionLevel.FULLACCESS, ClaimFullAccess));

                    claims.Add(new Claim("CanBoID", NguoiDung?.CanBoID.ToString()));
                    claims.Add(new Claim("NguoiDungID", NguoiDung?.NguoiDungID.ToString()));
                    claims.Add(new Claim("CoQuanID", NguoiDung?.CoQuanID.ToString()));
                    claims.Add(new Claim("CapCoQuan", NguoiDung?.CapCoQuan.ToString()));
                    //claims.Add(new Claim("VaiTro", NguoiDung?.VaiTro.ToString()));
                    claims.Add(new Claim("expires_at", Utils.ConvertToDateTime(DateTime.UtcNow.AddDays(_AppSettings.Value.NumberDateExpire).ToString(), DateTime.Now.Date).ToString()));
                    claims.Add(new Claim("TenCanBo", NguoiDung?.TenCanBo.ToString()));
                    //claims.Add(new Claim("QuanLyThanNhan", NguoiDung?.QuanLyThanNhan.ToString()));
                    //claims.Add(new Claim("expires_at", new DateTime(2020,01,07,13,45,00).ToString()));
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes("ZHVuZ2hhY2tlbW5oYWFuaA_DuNghaCktoinhA");
                    //var key = Encoding.ASCII.GetBytes(_AppSettings.Value.AudienceSecret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddDays(_AppSettings.Value.NumberDateExpire),
                        //new DateTime(2020, 01, 07, 13, 45, 00),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        //,Issuer = _AppSettings.Value.ApiUrl
                        //, Audience = _AppSettings.Value.AudienceSecret

                    };
                    if (NguoiDung.NguoiDungID == 1) NguoiDung.MapThongTin = 1;
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    NguoiDung.Token = tokenHandler.WriteToken(token);
                    NguoiDung.expires_at = DateTime.UtcNow.AddDays(_AppSettings.Value.NumberDateExpire);
                    //tokenDescriptor.Expires;
                    var clsCommon = new Commons();
                    //var listFile = _FileDinhKemBUS.GetAllField_FileDinhKem_ByNghiepVuID_AndType(NguoiDung.CanBoID, EnumLoaiFileDinhKem.AnhHoSo.GetHashCode());
                    //if (listFile.Count > 0)
                    //    NguoiDung.AnhHoSo = clsCommon.GetServerPath(HttpContext) + listFile[0].FileUrl;
                    var cb = _HeThongCanBoBUS.GetCanBoByID(NguoiDung.CanBoID);
                    if(cb.AnhHoSo != null)
                    {
                        NguoiDung.AnhHoSo = clsCommon.GetServerPath(HttpContext) + cb.AnhHoSo;
                    }
                    HttpContext.Response.Headers.Add("X-Logged-in", "true");
                    return Ok(new
                    {
                        Token = NguoiDung.Token,
                        Status = 1,
                        User = NguoiDung,
                        ListRole = ListChucNang
                    });
                }
                //else
                //{
                //    string message = Constant.NOT_ACCOUNT;
                //    if (User.Email != null && User.Email != "")
                //    {
                //        message = Constant.NOT_ACCOUNT_CAS;
                //    }
                //    return Ok(new
                //    {
                //        Status = -1,
                //        Message = message
                //    });
                //}
                else
                {
                    var _nguoiDung = _NguoiDungBUS.GetByTenNguoiDung(User.UserName.Trim());
                    string mess = "";
                    string messErr = "";
                    //var thamSoGioiHan = _systemConfigBUS.GetByKey("Gioi_Han_So_Lan_Login_Sai").ConfigValue.Split("/");
                    var thamSoGioiHan = new string[] { };
                    var _systemConfig = _systemConfigBUS.GetByKey("Gioi_Han_So_Lan_Login_Sai");
                    if (_systemConfig != null && _systemConfig.SystemConfigID > 0)
                    {
                        thamSoGioiHan = _systemConfig.ConfigValue.Split("/");

                        int SoLanGioiHanLogin = 0;
                        int KhoangPhutGioiHan = 0;
                        if (thamSoGioiHan.Length > 0)
                        {
                            SoLanGioiHanLogin = Utils.ConvertToInt32(thamSoGioiHan[0], 0);
                            KhoangPhutGioiHan = Utils.ConvertToInt32(thamSoGioiHan[1], 0);
                        }
                        if (_nguoiDung.NguoiDungID > 0) /*&& _nguoiDung.SoLanLogin < SoLanGioiHanLogin)*/
                        {
                            if (_nguoiDung.TrangThai == 0)
                            {
                                mess = "Tài khoản đã bị khoá!";
                            }
                            else
                            {
                                if (_nguoiDung.ThoiGianLogin == null)
                                {
                                    //trường hợp đăng nhập lần đầu sai
                                    //update trườg ThoiGianLogin và số lần login
                                    _nguoiDung.ThoiGianLogin = DateTime.Now;
                                    _nguoiDung.SoLanLogin = 1;
                                    mess = "Mật khẩu không đúng! Vui lòng thử lại!";
                                }
                                else
                                {
                                    if (DateTime.Now >= _nguoiDung.ThoiGianLogin.Value && DateTime.Now <= _nguoiDung.ThoiGianLogin.Value.AddMinutes(KhoangPhutGioiHan))
                                    {
                                        _nguoiDung.SoLanLogin++;
                                        mess = "Mật khẩu không đúng! Vui lòng thử lại!";
                                    }
                                    else
                                    {
                                        _nguoiDung.ThoiGianLogin = DateTime.Now;
                                        _nguoiDung.SoLanLogin = 1;
                                        mess = "Mật khẩu không đúng! Vui lòng thử lại!";
                                    }
                                }
                                if (_nguoiDung.SoLanLogin > SoLanGioiHanLogin)
                                {
                                    _nguoiDung.TrangThai = 0;
                                    //mess = "Tài khoản đã bị khoá do nhập sai mật khẩu quá " + SoLanGioiHanLogin + " lần trong " + KhoangPhutGioiHan + " phút!";
                                    //if (_nguoiDung.TenNguoiDung.Contains("@") && _nguoiDung.TenNguoiDung.Contains(".com"))
                                    //{
                                    //    string TieuDe = "Thông báo bảo mật!";
                                    //    string NoiDung = ("<p>" + "Bạn đã nhập sai mật khẩu " + SoLanGioiHanLogin + " liên tiếp trong " + KhoangPhutGioiHan + " phút!" + "</p>" +
                                    //                     "<p>" + "Để đảm bảo tài khoản của bạn dược an toàn, chúng tôi tạm khoá tài khoản của bạn!" + "</p>" +
                                    //                     "<p>" + "Vui lòng liên hệ với quản trị viên để được hỗ trợ mở lại tài khoản!" + "</p>" +
                                    //                     "<p>" + "Cảm ơn!" + "</p>"
                                    //                    ).ToString();
                                    //    List<string> MailTo = new List<string>();
                                    //    MailTo.Add(_nguoiDung.TenNguoiDung);
                                    //    _nhacViecBUS.SendMail(MailTo, TieuDe, NoiDung);
                                    //}
                                }
                                var temp = _NguoiDungBUS.UpdateThoiGianlogin(_nguoiDung, ref messErr);
                            }
                        }
                        else
                        {
                            mess = "Tài khoản không tồn tại!";
                        }
                    }
                    else
                    {
                        if (_nguoiDung.NguoiDungID > 0)
                        {
                            mess = "Mật khẩu không đúng! Vui lòng thử lại!";
                        }
                        else
                        {
                            mess = "Tài khoản không tồn tại!";
                        }
                    }
                    return Ok(new
                    {
                        Status = 0,
                        Message = string.IsNullOrEmpty(messErr) ? mess : messErr
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex.Message, "Đăng nhập hệ thống");
                throw;
            }


        }
    }
}