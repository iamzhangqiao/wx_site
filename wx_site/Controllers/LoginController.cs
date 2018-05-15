using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wx_api;
using wx_site.Models;

namespace wx_site.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            string code = Request.QueryString["code"];
            //判断地址栏中的code是否为空
            if (!string.IsNullOrEmpty(code))
            {
                //根据code获取成员信息
                //请求方式：GET（HTTPS）
                //请求地址：https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token=ACCESS_TOKEN&code=CODE
                //access_token 	是 	调用接口凭证
                //code 	是 	通过成员授权获取到的code，最大为512字节。每次成员授权带上的code将不一样，code只能使用一次，5分钟未被使用自动过期。
                //获取access_token调用方法
                string corpid = ConfigurationManager.AppSettings["CorpId"]; //企业ID
                string secret = ConfigurationManager.AppSettings["Secret_3"];//应用的凭证密钥 每个应用都不一样
                                                                             //获取到的应用验证令牌
                string strJson = Get_userinfo.GetUserinfo(corpid, secret, code);
                //反序列化json，把json串序列化成model包，使用了json.net工具包。
                Userinfo userinfo = JsonConvert.DeserializeObject<Userinfo>(strJson);

                return View("Index", userinfo);

            }
            return View();
        }

        /// <summary>
        /// 进行OAuth2授权注册
        /// </summary>
        /// <returns></returns>
        public ActionResult Sign()
        {
            string corpid = ConfigurationManager.AppSettings["CorpId"];
            string redirect_url = ConfigurationManager.AppSettings["Redirect_url_3"];
            string agentid = ConfigurationManager.AppSettings["AgentId_3"];

            var url = Check_OAuth2.GetCode(corpid, redirect_url, agentid);

            return Redirect(url);
            //return View();
        }
    }
}