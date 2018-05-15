using System;

namespace wx_api.Models
{
    /// <summary>
    /// 获取Access_token时返回的实体类
    /// </summary>
    public class Access_token
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 错误文本
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 获取到的凭证,最长为512字节
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 凭证的有效时间（秒）
        /// </summary>
        public int expires_in { get; set; }
    }
}
