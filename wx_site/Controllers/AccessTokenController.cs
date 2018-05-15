using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using wx_api;

namespace wx_site.Controllers
{
    public class AccessTokenController : Controller
    {
        // GET: AccessToken
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取access_token
        /// access_token是企业后台去企业微信的后台获取信息时的重要票据，由corpid和secret产生。
        /// 所有接口在通信时都需要携带此信息用于验证接口的访问权限
        /// </summary>
        public void GetAccessToken()
        {
            string corpid = ConfigurationManager.AppSettings["CorpId"];
            string secret = ConfigurationManager.AppSettings["Secret_3"];

            string access_token = Get_access_token.GetAccessToken(corpid, secret);
            Response.Write("access_token=" + access_token);
        }


    }
}