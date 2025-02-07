using Com.Gosol.QLVB.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.API.Authorization
{
    public static class PermissionLevel
    {
        public const string READ = "Read";

        public const string CREATE = "Create";

        public const string EDIT = "Edit";

        public const string DELETE = "Delete";

        public const string FULLACCESS = "FullAccess";
    }
    public class CustomAuthAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public ChucNangEnum ChucNangID { get; set; }
        public AccessLevel Quyen { get; set; }
   
        public CustomAuthAttribute() { }
        public CustomAuthAttribute(ChucNangEnum chucNangID, AccessLevel quyen)
        {
            ChucNangID = chucNangID;
            Quyen = quyen;
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (CheckAccessLevelByClaims(filterContext))
            {

            }
            else
            {
                filterContext.Result =
                  new ObjectResult(new { Status = -98, Message = "Người dùng không có quyền sử dụng chức năng này." });
            }
        }

        public bool CheckAccessLevelByClaims(AuthorizationFilterContext filterContext)
        {
            try
            {
                var user = filterContext.HttpContext.User as ClaimsPrincipal;
                string type = "";
                string value = "," + (int)ChucNangID + ",";
                switch (Quyen)
                {
                    case AccessLevel.Read:
                        type = PermissionLevel.READ;
                        break;
                    case AccessLevel.Create:
                        type = PermissionLevel.CREATE;
                        break;
                    case AccessLevel.Edit:
                        type = PermissionLevel.EDIT;
                        break;
                    case AccessLevel.Delete:
                        type = PermissionLevel.DELETE;
                        break;
                    case AccessLevel.FullAccess:
                        type = PermissionLevel.FULLACCESS;
                        break;
                }
                if (user.Claims.Where(c => c.Type.Equals(type) || c.Type.Equals(PermissionLevel.FULLACCESS)).Any(x => x.Value.Contains(value)))
                {
                    return true;
                }
                else if (ChucNangID == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var isAuth = await Task.Run(() => CheckAccessLevelByClaims(context));
            if (!isAuth)
            {
                context.Result = new ObjectResult(new { Status = -98, Message = "Người dùng không có quyền sử dụng chức năng này." });
            }
            
        }
    }
}
