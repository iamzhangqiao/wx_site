using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wx_api
{
    /// <summary>
    /// 把json数据包里面的字符串做个替换
    /// </summary>
    public class Json_Replace
    {
        /// <summary>
        /// 使用微信接口发布信息的时候有一些反斜杠需要包含在字符串中
        /// 使用json.net把model类转换成json包的时候，系统会把反斜杠\替换成双斜杠//
        /// 所以处理的时候先在反斜杠前面添加一个$符号，后期再处理回来
        /// </summary>
        /// <param name="str_json">需要处理的json串</param>
        /// <returns>返回处理好的Json串</returns>
        public string Json_add_4(string str_json)
        {
            str_json = str_json.Replace("\\", "\\$");
            return str_json;
        }

        /// <summary>
        /// 把json串中的一些字符串替换掉
        /// 其中有反斜杠替换成空
        /// 前面大括号前面的双引号和后面大括号后面的双引号要去掉
        /// 把上面一个方法添加的$符号再替换回反斜杠\
        /// </summary>
        /// <param name="str_json">传入的字符串</param>
        /// <returns>返回的字符串</returns>
        public string Json_rep(string str_json)
        {
            str_json = str_json.Replace("\\", "");
            str_json = str_json.Replace("\"{", "{");
            str_json = str_json.Replace("}\"", "}");
            str_json = str_json.Replace("$", "\\");
            return str_json;
        }
    }
}
