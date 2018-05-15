using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wx_site.Models
{
    public class SendMsg
    {
        
    }

    /// <summary>
    /// 发送的文本消息中的文本内容
    /// </summary>
    public class Msg_Text_content
    {
        /// <summary>
        /// 消息的内容
        /// </summary>
        public string content { get; set; }
    }
    /// <summary>
    /// 发送纯文本消息的参数类
    /// </summary>
    public class Msg_Text
    {
        /// <summary>
        ///成员ID列表（消息接收者，多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为 @all，则向该企业应用的全部成员发送
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 部门ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为 @all时忽略本参数
        /// </summary>
        public string toparty { get; set; }
        /// <summary>
        /// 标签ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为 @all时忽略本参数
        /// </summary>
        public string totag { get; set; }
        /// <summary>
        /// 消息类型，此时固定为：text
        /// </summary>
        public string msgtype { get; set; }
        /// <summary>
        /// 企业应用的id，整型。可在应用的设置页面查看
        /// </summary>
        public int agentid { get; set; }
        /// <summary>
        /// 消息内容，最长不超过2048个字节
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 表示是否是保密消息，0表示否，1表示是，默认0
        /// </summary>
        public int safe { get; set; }
    }


    /// <summary>
    /// 发送文本卡片消息中文本的内容
    /// </summary>
    public class Msg_Text_card_content
    {
        /// <summary>
        /// 标题，不超过128个字节，超过会自动截断
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 描述，不超过512个字节，超过会自动截断
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 点击后跳转的链接。
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 按钮文字。 默认为“详情”， 不超过4个文字，超过自动截断。
        /// </summary>
        public string btntxt { get; set; }
    }
    /// <summary>
    /// 发送卡片文本消息的参数类
    /// </summary>
    public class Msg_Text_card
    {
        /// <summary>
        ///成员ID列表（消息接收者，多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为 @all，则向该企业应用的全部成员发送
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 部门ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为 @all时忽略本参数
        /// </summary>
        public string toparty { get; set; }
        /// <summary>
        /// 标签ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为 @all时忽略本参数
        /// </summary>
        public string totag { get; set; }
        /// <summary>
        /// 消息类型，此时固定为：textcard
        /// </summary>
        public string msgtype { get; set; }
        /// <summary>
        /// 企业应用的id，整型。可在应用的设置页面查看
        /// </summary>
        public int agentid { get; set; }
        /// <summary>
        /// 消息内容，最长不超过2048个字节
        /// </summary>
        public string textcard { get; set; }
    }
}