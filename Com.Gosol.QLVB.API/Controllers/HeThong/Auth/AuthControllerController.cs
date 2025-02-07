using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Com.Gosol.QLVB.DAL.HeThong;
using Microsoft.Extensions.Configuration;

namespace Com.Gosol.QLVB.API.Controllers.HeThong
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthControllerController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

    }
}