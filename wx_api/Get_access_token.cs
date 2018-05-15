using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using Newtonsoft.Json;
using wx_api.Models;

namespace wx_api
{
    public class Get_access_token
    {
        #region 获取access_token
        /// <summary>
        /// 获取access_token的方法
        /// </summary>
        /// <param name="corpid">企业ID</param>
        /// <param name="secret">应用的凭证密钥</param>
        /// <returns></returns>
        public static string GetAccessToken(string corpid, string secret)
        {
            //先判断配置文件web.config中是否已经有了Access_Token信息，并且判断距离上次获取的时间是否超过了2小时
            //没有超过的话，不需要获取，直接使用。超出的话，重新获取，并修改web.config中的配置信息。

            //先读取时间信息
            //最后一次获取Access_Token的时间
            string dllPath = string.Format(
                "{0}\\{1}.dll", AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory, "wx_api");
            Configuration config = ConfigurationManager.OpenExeConfiguration(dllPath);
            AppSettingsSection oSection = null;
            oSection = config.GetSection("appSettings") as AppSettingsSection;
            string dt = oSection.Settings["Access_Token_3_Time"].Value.ToString();

            DateTime token_dt = Convert.ToDateTime(oSection.Settings["Access_Token_3_Time"].Value.ToString());
            //现在的时间
            DateTime now_dt = DateTime.Now;
            //获取两个时间相差的秒数
            TimeSpan timeSpan = now_dt - token_dt;
            double c = timeSpan.TotalSeconds;

            //如果相差大于7200秒的话，重新获取，否则直接使用上次获取的Access_Token值
            if (c > 7200.00)
            {
                string strJson = HttpRequestUtil.RequestUrl(string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}", corpid, secret));

                //反序列化json，把json串序列化成model包，使用了json.net工具包。
                Access_token access_Token = JsonConvert.DeserializeObject<Access_token>(strJson);
                if (access_Token.errcode == 0)
                {
                    //修改配置文件中的节点为新的数据
                    oSection.Settings["Access_Token_3_Time"].Value = now_dt.ToString("yyy-MM-dd HH:mm:ss");
                    oSection.Settings["Access_Token_3"].Value = access_Token.access_token;
                    config.Save(ConfigurationSaveMode.Modified);
                    return access_Token.access_token;
                }
                else
                {
                    return access_Token.errmsg;
                }
            }
            else
            {
                return oSection.Settings["Access_Token_3"].Value.ToString();
            }
        }
        #endregion
    }
}
