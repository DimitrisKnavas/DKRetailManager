using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TRMDataManagerLibrary.DataAccess;
using TRMDataManagerLibrary.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        [HttpGet]
        public UserModel GetById() //UserModel is used from library's Models folder(is a DataAccess Model)..
                                         //the reason why  we don't create a UserModel in 
                                         //API's Models is because we don't want a display model
        {
            string userId = RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData();

            return data.GetUserById(userId).First();
        }

        
    }
}
