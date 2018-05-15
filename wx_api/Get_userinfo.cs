using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wx_api.Models;

namespace wx_api
{
    public class Get_userinfo
    {
        #region 获取 access_token
        /// <summary>
        /// 先获取到access_token 然后使用access_token和传递进来的code去获取用户信息
        /// </summary>
        /// <param name="corpid">企业ID</param>
        /// <param name="secret">应用的凭证密钥</param>
        /// <param name="code">Code</param>
        /// <returns></returns>
        public static string GetUserinfo(string corpid, string secret,string code)
        {
            //先获取到access_token
            string str_token = Get_access_token.GetAccessToken(corpid, secret);

            //使用access_token和code去获取用户信息 返回一个Model对象
            string strJson = string.Empty;
            strJson = HttpRequestUtil.RequestUrl(string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}", str_token, code));
            //反序列化json，把json串序列化成model包，使用了json.net工具包。
            Userinfo userinfo = JsonConvert.DeserializeObject<Userinfo>(strJson);
            if (userinfo.errcode == 0)
                return strJson;
            else
                return userinfo.errmsg;
        }
        #endregion
    }
}
