using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wx_api
{
    public class SendMessage
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="corpid">公司ID</param>
        /// <param name="secret">密匙</param>
        /// <param name="json">要发送的JSON数据包</param>
        /// <returns>返回一个JSON包</returns>
        public string SendMsg(string corpid, string secret,string json)
        {
            //先获取到access_token
            string accessToken = Get_access_token.GetAccessToken(corpid, secret);

            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}";
            var url = string.Format(urlFormat, accessToken);

            string str_json = HttpRequestUtil.RequestUrlSendMsg(url, "POST", json);

            return str_json;
        }
    }
}
