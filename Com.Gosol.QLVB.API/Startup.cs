using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Gosol.QLVB.API.Config;
using Com.Gosol.QLVB.BUS.DanhMuc;
using Com.Gosol.QLVB.BUS.HeThong;
using Com.Gosol.QLVB.DAL.DanhMuc;
using Com.Gosol.QLVB.DAL.EFCore;
using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Ultilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Com.Gosol.QLVB.API.Formats;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Com.Gosol.QLVB.BUS.KeKhai;
using Com.Gosol.QLVB.DAL.KeKhai;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Com.Gosol.QLVB.BUS.FileDinhKem;
//using Com.Gosol.QLVB.BUS.BaoCao;
//using Com.Gosol.QLVB.DAL.BaoCao;
using Com.Gosol.QLVB.BUS;
using Com.Gosol.QLVB.DAL;
using Com.Gosol.QLVB.API.Controllers;
using Com.Gosol.QLVB.BUS.QLVB;
using Com.Gosol.QLVB.DAL.QLVB;
using Com.Gosol.QLVB.BUS.ThongKe;
using Com.Gosol.QLVB.DAL.ThongKe;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using System.Net.Http;

namespace Com.Gosol.QLVB.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //Com.Gosol.QLVB.DAL.Connection.appConnectionStrings = configuration.GetValue<String>("AppSetting:appConnectionStrings");
            // Com.Gosol.QLVB.Ultilities.SQLHelper.appConnectionStrings= configuration.GetValue<String>("AppSetting:appConnectionStrings");
            Com.Gosol.QLVB.Ultilities.SQLHelper.appConnectionStrings = configuration.GetConnectionString("DefaultConnection");
            Com.Gosol.QLVB.Ultilities.SQLHelper.backupPath = configuration.GetConnectionString("BackupPath");
            Com.Gosol.QLVB.Ultilities.SQLHelper.dbName = configuration.GetConnectionString("DBName");
            Com.Gosol.QLVB.Security.Com.Gosol.QLVB.Security.Encrypt_Decrypt.vector_IV_AES = configuration.GetValue<string>("AppSettings:SecretKey");
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
            .AddNewtonsoftJson();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });
            services.AddControllers()
        .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);
            services.AddMvc()
            .AddNewtonsoftJson(
            options => options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddMvc()
             .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver
                                       = new DefaultContractResolver());
            //inject option appsettings in appsettings.json
            services.AddOptions();
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 99999999999;
            });

            services.AddSwaggerGen(options =>
            {
                //thông tin
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    TermsOfService = new Uri("https://gosol.com.vn"),
                    Contact = new OpenApiContact
                    {
                        Name = "Contact",
                        Url = new Uri("https://gosol.com.vn")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License",
                        Url = new Uri("https://gosol.com.vn")
                    }
                });
                // add Authen - cho phép nhập Bearer và dùng cho tất cả các api
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                // bắt buộc phải có Bearer
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            // He thong
            services.AddScoped<ILogHelper, LogHelper>();
            services.AddScoped<ISystemLogBUS, SystemLogBUS>();
            services.AddScoped<ISystemLogDAL, SystemLogDAL>();
            services.AddScoped<INguoiDungBUS, NguoiDungBUS>();
            services.AddScoped<INguoiDungDAL, NguoiDungDAL>();
            services.AddScoped<IPhanQuyenBUS, PhanQuyenBUS>();
            services.AddScoped<IPhanQuyenDAL, PhanQuyenDAL>();
            services.AddScoped<IChucNangDAL, ChucNangDAL>();
            services.AddScoped<ISystemConfigBUS, SystemConfigBUS>();
            services.AddScoped<ISystemConfigDAL, SystemConfigDAL>();
            services.AddScoped<IHeThongNguoidungBUS, HeThongNguoidungBUS>();
            services.AddScoped<IHeThongNguoiDungDAL, HeThongNguoiDungDAL>();
            services.AddScoped<IQuanTriDuLieuBUS, QuanTriDuLieuBUS>();
            services.AddScoped<IQuanTriDuLieuDAL, QuanTriDuLieuDAL>();
            services.AddScoped<IQuanTriDuLieuBUS, QuanTriDuLieuBUS>();
            services.AddScoped<IQuanTriDuLieuDAL, QuanTriDuLieuDAL>();
            services.AddScoped<IChucNangBUS, ChucNangBUS>();
            services.AddScoped<IChucNangDAL, ChucNangDAL>();
            services.AddScoped<IHuongDanSuDungBUS, HuongDanSuDungBUS>();
            services.AddScoped<IHuongDanSuDungDAL, HuongDanSuDungDAL>();



            //Danh muc
            services.AddScoped<IDanhMucChucVuBUS, DanhMucChucVuBUS>();
            services.AddScoped<IDanhMucChucVuDAL, DanhMucChucVuDAL>();
            services.AddScoped<IHeThongCanBoBUS, HeThongCanBoBUS>();
            services.AddScoped<IHeThongCanBoDAL, HeThongCanBoDAL>();
            services.AddScoped<IDanhMucDiaGioiHanhChinhDAL, DanhMucDiaGioiHanhChinhDAL>();
            services.AddScoped<IDanhMucDiaGioiHanhChinhBUS, DanhMucDiaGioiHanhChinhBUS>();
            services.AddScoped<IDanhMucCoQuanDonViBUS, DanhMucCoQuanDonViBUS>();
            services.AddScoped<IDanhMucCoQuanDonViBUS, DanhMucCoQuanDonViBUS>();
            services.AddScoped<IDanhMucChungBUS, DanhMucChungBUS>();
            services.AddScoped<IDanhMucChungDAL, DanhMucChungDAL>();
            services.AddScoped<IDanhMucHoiDongThiBUS, DanhMucHoiDongThiBUS>();
            services.AddScoped<IDanhMucHoiDongThiDAL, DanhMucHoiDongThiDAL>();
            services.AddScoped<IDanhMucKhoaThiBUS, DanhMucKhoaThiBUS>();
            services.AddScoped<IDanhMucKhoaThiDAL, DanhMucKhoaThiDAL>();

            services.AddScoped<IDanhMucCoQuanDonViDAL, DanhMucCoQuanDonViDAL>();
            services.AddScoped<IDanhMucCoQuanDonViBUS, DanhMucCoQuanDonViBUS>();

            //QLVB
            services.AddScoped<IDuLieuDiemThiBUS, DuLieuDiemThiBUS>();
            services.AddScoped<IDuLieuDiemThiDAL, DuLieuDiemThiDAL>();
            services.AddScoped<IQuanLyThiSinhBUS, QuanLyThiSinhBUS>();
            services.AddScoped<IQuanLyThiSinhDAL, QuanLyThiSinhDAL>();
            services.AddScoped<IThongTinCapBangBUS, ThongTinCapBangBUS>();
            services.AddScoped<IThongTinCapBangDAL, ThongTinCapBangDAL>();
            services.AddScoped<ICapNhatPhuLucChinhSuaBUS, CapNhatPhuLucChinhSuaBUS>();
            services.AddScoped<ICapNhatPhuLucChinhSuaDAL, CapNhatPhuLucChinhSuaDAL>();

            services.AddScoped<INhacViecBUS, NhacViecBUS>();
            services.AddScoped<INhacViecDAL, NhacViecDAL>();
            services.AddScoped<IFileDinhKemDAL, FileDinhKemDAL>();
            services.AddScoped<IFileDinhKemBUS, FileDinhKemBUS>();
            services.AddScoped<IFileDinhKemLogDAL, FileDinhKemLogDAL>();
            services.AddScoped<IFileDinhKemLogBUS, FileDinhhKemLogBUS>();

            // báo cáo
            services.AddScoped<IMauPhieuBUS, MauPhieuBUS>();
            services.AddScoped<IMauPhieuDAL, MauPhieuDAL>();

            services.AddScoped<IThongKeBUS, ThongKeBUS>();
            services.AddScoped<IThongKeDAL, ThongKeDAL>();

            services.AddScoped<IDashBoardBUS, DashBoardBUS>();
            services.AddScoped<IDashBoardDAL, DashBoardDAL>();

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddCors(options => options.AddPolicy("myDomain", builder =>
            {
                builder.WithOrigins("http://gocheckin.gosol.com.vn")
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            //var key = Encoding.ASCII.GetBytes(appSettings.AudienceSecret);
            var key = Encoding.ASCII.GetBytes("ZHVuZ2hhY2tlbW5oYWFuaA_DuNghaCktoinhA");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver() { NamingStrategy = new Newtonsoft.Json.Serialization.DefaultNamingStrategy() });
            services.AddMvc().AddNewtonsoftJson();
            services.AddHttpContextAccessor();
            services.AddScoped(sp => sp.GetRequiredService<HttpContext>().Request);
            services.AddHttpClient("HttpClientWithSSLUntrusted").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            },
                MaxRequestContentBufferSize = int.MaxValue,
            });
            services.AddMvc();
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();
            services.Configure<FormOptions>(options =>
            {
                // Set the limit to 500 MB
                options.MultipartBodyLengthLimit = 536870912;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.AspNetCore.Hosting.IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("AllowOrigin");
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI();

            // register lifetime event
            applicationLifetime.ApplicationStarted.Register(InitializeApplication);
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors 'self' https://qlvbtest.gosol.com.vn;");
                await next();
            });
        }

        public void InitializeApplication()
        {
            new NhacViecDAL().SendMailAuto();
        }
    }
}
