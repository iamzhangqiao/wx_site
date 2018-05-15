using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace wx_api
{
    public class Check_OAuth2
    {
        #region 进行OAuth2.0授权确认
        /// <summary>
        /// 进行OAuth2.0授权确认
        /// </summary>
        public static string GetCode(string corpid, string redirect_uri, string agentid)
        {
            redirect_uri = HttpUtility.UrlEncode(redirect_uri);
            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_privateinfo&agentid={2}&state=2#wechat_redirect", corpid, redirect_uri, agentid);
        }
        #endregion
    }
}
