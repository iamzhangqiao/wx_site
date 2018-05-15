using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Configuration;
using System.Web.Mvc;
using wx_api;
using wx_site.Models;

namespace wx_site.Controllers
{
    public class RenshiController : Controller
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

                //从web.config文件中读取配置信息
                string corpid = ConfigurationManager.AppSettings["CorpId"]; //企业ID
                string secret = ConfigurationManager.AppSettings["Secret_3"];//应用的凭证密钥 每个应用都不一样

                string strJson = Get_userinfo.GetUserinfo(corpid, secret, code);
                //反序列化json，把json串序列化成model包，使用了json.net工具包。
                Userinfo userinfo = JsonConvert.DeserializeObject<Userinfo>(strJson);

                //把用户信息保存到Cookie中
                Response.Cookies["userName"].Value = userinfo.UserId;
                Response.Cookies["userName"].Expires = DateTime.Now.AddDays(365);
                //返回视图
                return View("Index", userinfo);
            }
            else if (Request.Cookies["userName"] != null)
            {
                Userinfo userinfo = new Userinfo();
                userinfo.UserId = Request.Cookies["userName"].Value;
                return View("Index", userinfo);
            }
            return RedirectToAction("OAuth", "Renshi");

            //////测试使用下面三行代码
            //////Userinfo userinfo = new Userinfo();
            //////userinfo.UserId = "abc";
            //////return View(userinfo);

        }

        /// <summary>
        /// 进行OAuth2授权注册
        /// </summary>
        /// <returns></returns>
        public ActionResult OAuth()
        {
            string corpid = ConfigurationManager.AppSettings["CorpId"];
            string redirect_url = ConfigurationManager.AppSettings["Redirect_url_3"];
            string agentid = ConfigurationManager.AppSettings["AgentId_3"];
            var url = Check_OAuth2.GetCode(corpid, redirect_url, agentid);
            return Redirect(url);
        }

        /// <summary>
        /// 职员档案信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Dangan()
        {
            string emp_no = Request.QueryString["id"];
            RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();
            DataSet ds = renshiService.GetEmpinfo(emp_no);
            Dangan dangan = new Dangan();
            if (ds.Tables[0].Rows.Count > 0)
            {
                dangan.Emp_no = ds.Tables[0].Rows[0]["emp_no"].ToString();
                dangan.Emp_nm = ds.Tables[0].Rows[0]["emp_nm"].ToString();
                dangan.Entr_dt = ds.Tables[0].Rows[0]["entr_dt"].ToString();
                dangan.Dept_nm = ds.Tables[0].Rows[0]["dept_nm"].ToString();
            }

            return View(dangan);
        }

        /// <summary>
        /// 工资明细查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Gongzi")]
        public ActionResult Gongzi()
        {
            List<Gongzi_Zhifu> ltzhifu = new List<Gongzi_Zhifu>();
            Gongzi_Zhifu gongzi_Zhifu = new Gongzi_Zhifu();
            ltzhifu.Add(gongzi_Zhifu);
            ViewData["zhifu"] = ltzhifu;

            List<Gongzi_Kouchu> ltkouchu = new List<Gongzi_Kouchu>();
            Gongzi_Kouchu gongzi_Kouchu = new Gongzi_Kouchu();
            ltkouchu.Add(gongzi_Kouchu);
            ViewData["kouchu"] = ltkouchu;

            List<Gongzi_Heji> ltheji = new List<Gongzi_Heji>();
            Gongzi_Heji gongzi_Heji = new Gongzi_Heji();
            ltheji.Add(gongzi_Heji);
            ViewData["heji"] = ltheji;

            return View();
        }

        [HttpPost]
        [ActionName("Gongzi")]
        public ActionResult Gongzi(FormCollection collection)
        {
            string month = collection["combox_year"];
            month = month + collection["combox_month"];
            string emp_no = Request.QueryString["id"];
            RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();
            DataSet ds = renshiService.GetGongZi(emp_no, month);
            
            //津贴项目
            if(ds.Tables[0].Rows.Count > 0)
            {
                List<Gongzi_Zhifu> ltzhifu = new List<Gongzi_Zhifu>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {                   
                    Gongzi_Zhifu gongzi_Zhifu = new Gongzi_Zhifu();
                    gongzi_Zhifu.Emp_no = ds.Tables[0].Rows[i]["emp_no"].ToString();
                    gongzi_Zhifu.Allow_cd = ds.Tables[0].Rows[i]["allow_cd"].ToString();
                    gongzi_Zhifu.Allow_nm1 = ds.Tables[0].Rows[i]["allow_nm1"].ToString();
                    gongzi_Zhifu.Allow = Convert.ToDecimal(ds.Tables[0].Rows[i]["allow"].ToString());
                    ltzhifu.Add(gongzi_Zhifu);
                    ViewData["zhifu"] = ltzhifu;
                }
            }
            else
            {
                List<Gongzi_Zhifu> ltzhifu = new List<Gongzi_Zhifu>();
                Gongzi_Zhifu gongzi_Zhifu = new Gongzi_Zhifu();
                ltzhifu.Add(gongzi_Zhifu);
                ViewData["zhifu"] = ltzhifu;
            }
            //扣除项目
            if (ds.Tables[1].Rows.Count > 0)
            {
                List<Gongzi_Kouchu> ltkouchu = new List<Gongzi_Kouchu>();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    Gongzi_Kouchu gongzi_Kouchu = new Gongzi_Kouchu();
                    gongzi_Kouchu.Emp_no = ds.Tables[1].Rows[i]["emp_no"].ToString();
                    gongzi_Kouchu.Sub_cd = ds.Tables[1].Rows[i]["sub_cd"].ToString();
                    gongzi_Kouchu.Allow_nm1 = ds.Tables[1].Rows[i]["allow_nm1"].ToString();
                    gongzi_Kouchu.Sub_amt = Convert.ToDecimal(ds.Tables[1].Rows[i]["sub_amt"].ToString());
                    ltkouchu.Add(gongzi_Kouchu);
                    ViewData["kouchu"] = ltkouchu;
                }
            }
            else
            {
                List<Gongzi_Kouchu> ltkouchu = new List<Gongzi_Kouchu>();
                Gongzi_Kouchu gongzi_Kouchu = new Gongzi_Kouchu();
                ltkouchu.Add(gongzi_Kouchu);
                ViewData["kouchu"] = ltkouchu;
            }
            //合计金额
            if (ds.Tables[2].Rows.Count > 0)
            {
                List<Gongzi_Heji> ltheji = new List<Gongzi_Heji>();
                for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                {                   
                    Gongzi_Heji gongzi_Heji = new Gongzi_Heji();
                    gongzi_Heji.PROV_TOT_AMT = Convert.ToDecimal(ds.Tables[2].Rows[i]["PROV_TOT_AMT"].ToString());
                    gongzi_Heji.REAL_PROV_AMT = Convert.ToDecimal(ds.Tables[2].Rows[i]["REAL_PROV_AMT"].ToString());
                    gongzi_Heji.SUB_TOT_AMT = Convert.ToDecimal(ds.Tables[2].Rows[i]["SUB_TOT_AMT"].ToString());
                    ltheji.Add(gongzi_Heji);
                    ViewData["heji"] = ltheji;
                }
            }
            else
            {
                List<Gongzi_Heji> ltheji = new List<Gongzi_Heji>();
                Gongzi_Heji gongzi_Heji = new Gongzi_Heji();
                ltheji.Add(gongzi_Heji);
                ViewData["heji"] = ltheji;
            }

            return View();
        }


        /// <summary>
        /// 考勤明细查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Kaoqin")]
        public ActionResult Kaoqin()
        {
            List<Kaoqin_Jiaban> ltjiaban = new List<Kaoqin_Jiaban>();
            Kaoqin_Jiaban kaoqin_Jiaban = new Kaoqin_Jiaban();
            ltjiaban.Add(kaoqin_Jiaban);
            ViewData["jiaban"] = ltjiaban;

            List<Kaoqin_Qingjia> ltqingjia = new List<Kaoqin_Qingjia>();
            Kaoqin_Qingjia kaoqin_Qingjia = new Kaoqin_Qingjia();
            ltqingjia.Add(kaoqin_Qingjia);
            ViewData["qingjia"] = ltqingjia;

            List<Kaoqin_Yichang> ltyichang = new List<Kaoqin_Yichang>();
            Kaoqin_Yichang kaoqin_Yichang = new Kaoqin_Yichang();
            ltyichang.Add(kaoqin_Yichang);
            ViewData["yichang"] = ltyichang;

            return View();
        }
        /// <summary>
        /// 考勤明细查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Kaoqin")]
        public ActionResult Kaoqin(FormCollection collection)
        {
            string month = collection["combox_year"];
            month = month + collection["combox_month"];
            string emp_no = Request.QueryString["id"];
            RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();
            DataSet ds = renshiService.GetHRT010TB(emp_no, month);

            //加班项目
            if (ds.Tables[0].Rows.Count > 0)
            {
                List<Kaoqin_Jiaban> ltjiaban = new List<Kaoqin_Jiaban>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {                   
                    Kaoqin_Jiaban kaoqin_Jiaban = new Kaoqin_Jiaban();
                    kaoqin_Jiaban.Rt_date = ds.Tables[0].Rows[i]["rt_date"].ToString();
                    kaoqin_Jiaban.Wd_day = ds.Tables[0].Rows[i]["wd_day"].ToString();
                    kaoqin_Jiaban.Dilig_nm = ds.Tables[0].Rows[i]["dilig_nm"].ToString();
                    kaoqin_Jiaban.Fq_name = ds.Tables[0].Rows[i]["fq_name"].ToString();
                    kaoqin_Jiaban.Rt_btime = ds.Tables[0].Rows[i]["rt_btime"].ToString();
                    kaoqin_Jiaban.Rt_etime = ds.Tables[0].Rows[i]["rt_etime"].ToString();
                    kaoqin_Jiaban.Rt_hh = Convert.ToDecimal(ds.Tables[0].Rows[i]["rt_hh"].ToString());
                    ltjiaban.Add(kaoqin_Jiaban);
                    ViewData["jiaban"] = ltjiaban;
                }
            }
            else
            {
                List<Kaoqin_Jiaban> ltjiaban = new List<Kaoqin_Jiaban>();
                Kaoqin_Jiaban kaoqin_Jiaban = new Kaoqin_Jiaban();
                ltjiaban.Add(kaoqin_Jiaban);
                ViewData["jiaban"] = ltjiaban;
            }
            //请假项目
            if (ds.Tables[1].Rows.Count > 0)
            {
                List<Kaoqin_Qingjia> ltqingjia = new List<Kaoqin_Qingjia>();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {                  
                    Kaoqin_Qingjia kaoqin_Qingjia = new Kaoqin_Qingjia();
                    kaoqin_Qingjia.Ex_date = ds.Tables[1].Rows[i]["ex_date"].ToString();
                    kaoqin_Qingjia.Wd_day = ds.Tables[1].Rows[i]["wd_day"].ToString();
                    kaoqin_Qingjia.Dilig_nm = ds.Tables[1].Rows[i]["dilig_nm"].ToString();
                    kaoqin_Qingjia.Fq_name = ds.Tables[1].Rows[i]["fq_name"].ToString();
                    kaoqin_Qingjia.Ex_btime = ds.Tables[1].Rows[i]["ex_btime"].ToString();
                    kaoqin_Qingjia.Ex_etime = ds.Tables[1].Rows[i]["ex_etime"].ToString();
                    kaoqin_Qingjia.Ex_hh = Convert.ToDecimal(ds.Tables[1].Rows[i]["ex_hh"].ToString());
                    ltqingjia.Add(kaoqin_Qingjia);
                    ViewData["qingjia"] = ltqingjia;
                }
            }
            else
            {
                List<Kaoqin_Qingjia> ltqingjia = new List<Kaoqin_Qingjia>();
                Kaoqin_Qingjia kaoqin_Qingjia = new Kaoqin_Qingjia();
                ltqingjia.Add(kaoqin_Qingjia);
                ViewData["qingjia"] = ltqingjia;
            }
            //异常出勤
            if (ds.Tables[2].Rows.Count > 0)
            {
                List<Kaoqin_Yichang> ltyichang = new List<Kaoqin_Yichang>();
                for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                {                   
                    Kaoqin_Yichang kaoqin_Yichang = new Kaoqin_Yichang();
                    kaoqin_Yichang.Rt_date = ds.Tables[2].Rows[i]["rt_date"].ToString();
                    kaoqin_Yichang.Wd_day = ds.Tables[2].Rows[i]["wd_day"].ToString();
                    kaoqin_Yichang.Dilig_nm = ds.Tables[2].Rows[i]["dilig_nm"].ToString();
                    kaoqin_Yichang.Fq_name = ds.Tables[2].Rows[i]["fq_name"].ToString();
                    kaoqin_Yichang.Rt_btime = ds.Tables[2].Rows[i]["rt_btime"].ToString();
                    kaoqin_Yichang.Rt_etime = ds.Tables[2].Rows[i]["rt_etime"].ToString();
                    kaoqin_Yichang.Rt_hh = Convert.ToDecimal(ds.Tables[2].Rows[i]["rt_hh"].ToString());
                    ltyichang.Add(kaoqin_Yichang);
                    ViewData["yichang"] = ltyichang;
                }
            }
            else
            {
                List<Kaoqin_Yichang> ltyichang = new List<Kaoqin_Yichang>();
                Kaoqin_Yichang kaoqin_Yichang = new Kaoqin_Yichang();
                ltyichang.Add(kaoqin_Yichang);
                ViewData["yichang"] = ltyichang;
            }

            return View();
        }

        /// <summary>
        /// 自助请假申请
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("qingjia")]
        public ActionResult Qingjia()
        {
            //窗体加载时要先显示职员的部分档案信息
            string emp_no = Request.QueryString["id"];
            RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();
            DataSet ds = renshiService.GetEmpinfo(emp_no);
            Dangan dangan = new Dangan();
            if (ds.Tables[0].Rows.Count > 0)
            {
                dangan.Emp_no = ds.Tables[0].Rows[0]["emp_no"].ToString();
                dangan.Emp_nm = ds.Tables[0].Rows[0]["emp_nm"].ToString();
                dangan.Entr_dt = ds.Tables[0].Rows[0]["entr_dt"].ToString();
                dangan.Dept_nm = ds.Tables[0].Rows[0]["dept_nm"].ToString();
            }

            return View(dangan);
        }

        /// <summary>
        /// 自助请假申请 POST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("qingjia")]
        public ActionResult Qingjia(FormCollection collection)
        {
            string emp_no = Request.QueryString["id"];//职员工号
            RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();
            DataSet ds = renshiService.GetEmpinfo(emp_no);
            Dangan dangan = new Dangan();
            if (ds.Tables[0].Rows.Count > 0)
            {
                dangan.Emp_no = ds.Tables[0].Rows[0]["emp_no"].ToString();
                dangan.Emp_nm = ds.Tables[0].Rows[0]["emp_nm"].ToString();
                dangan.Entr_dt = ds.Tables[0].Rows[0]["entr_dt"].ToString();
                dangan.Dept_nm = ds.Tables[0].Rows[0]["dept_nm"].ToString();
            }

            //把职员请假的消息发送给应用接受并返回一个提示消息给管理员
            string corpid = ConfigurationManager.AppSettings["CorpId"]; //企业ID
            string secret = ConfigurationManager.AppSettings["Secret_3"];//应用的凭证密钥 每个应用都不一样

            //把用户提交的请假申请insert到数据表中，然后接收返回的信息
            wx_site.RenshiService.HEX001TB hex001tb = new RenshiService.HEX001TB();
            //生成一个随机串，用作主键
            string ss = System.Guid.NewGuid().ToString();
            string s2 = ss.Substring(24, 12);
            hex001tb.id = s2;
            hex001tb.emp_no = emp_no;
            hex001tb.ex_date = Convert.ToDateTime(collection["ex_date"]);
            hex001tb.dilig_cd = collection["combox_dilig"];
            hex001tb.ex_btime = collection["ex_btime"];
            hex001tb.ex_etime = collection["ex_etime"];
            hex001tb.ex_remark = collection["ex_remark"];
            hex001tb.ins_no = emp_no;
            ds = renshiService.AddHEX001TB(hex001tb);

            if (ds.Tables["MSG"].Rows.Count > 0)
            {
                string msg_cd = ds.Tables["MSG"].Rows[0]["msg_cd"].ToString();
                string msg_text = ds.Tables["MSG"].Rows[0]["msg_text"].ToString();
                //如果返回0，则说明保存成功，此时需要发送消息给管理员审核
                if (msg_cd == "0")
                {
                    //要把用户提交的信息封装为一个model，并转换成json包
                    //首先发送的消息内容需要封装成一个json包
                    //消息以卡片文本的形式发送，其中需要参数。
                    Msg_Text_card_content text_card = new Msg_Text_card_content();
                    text_card.title = ds.Tables["MSG"].Rows[0]["title"].ToString();
                    text_card.description = ds.Tables["MSG"].Rows[0]["description"].ToString();
                    text_card.url = ds.Tables["MSG"].Rows[0]["url"].ToString();
                    text_card.btntxt = ds.Tables["MSG"].Rows[0]["btntxt"].ToString();
                    //转换成json包之前先处理一下反斜杠 \
                    Json_Replace json_Replace = new Json_Replace();
                    text_card.description = json_Replace.Json_add_4(text_card.description);
                    //转换成json包
                    string text_content_json = JsonConvert.SerializeObject(text_card);

                    //再封装POST提交需要的JSON包
                    Msg_Text_card msg_Text_Card = new Msg_Text_card();
                    msg_Text_Card.touser = ds.Tables["MSG"].Rows[0]["touser"].ToString();
                    msg_Text_Card.toparty = ds.Tables["MSG"].Rows[0]["toparty"].ToString();
                    msg_Text_Card.totag = ds.Tables["MSG"].Rows[0]["totag"].ToString();
                    msg_Text_Card.msgtype = ds.Tables["MSG"].Rows[0]["msgtype"].ToString();
                    msg_Text_Card.agentid = int.Parse(ds.Tables["MSG"].Rows[0]["agentid"].ToString());
                    msg_Text_Card.textcard = text_content_json;
                    //Json.NET序列化
                    string jsonData = JsonConvert.SerializeObject(msg_Text_Card);
                    //过滤掉其中的一些特殊符号，使其试用于微信接口的规范。
                    jsonData = json_Replace.Json_rep(jsonData);
                    //推送消息给管理员审核
                    SendMessage sendMessage = new SendMessage();
                    string strjson = sendMessage.SendMsg(corpid, secret, jsonData);

                }
                dangan.msg_cd = msg_cd;
                dangan.msg_text = msg_text;
            }
            return View(dangan);
        }

        /// <summary>
        /// 自助请假审核
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("check")]
        public ActionResult Check()
        {
            //窗体加载时要先显示职员的请假信息
            string id = Request.QueryString["id"];//请假信息的主键
            RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();
            wx_site.RenshiService.HEX001TB hex001tb = new RenshiService.HEX001TB();
            hex001tb.id = id;
            hex001tb.ex_date = DateTime.Now;
            DataSet ds = renshiService.SearchHEX001TB(hex001tb);
            HEX001TB hEX001TB = new HEX001TB();
            if (ds.Tables[0].Rows.Count > 0)
            {
                hEX001TB.emp_no = ds.Tables[0].Rows[0]["emp_no"].ToString();
                hEX001TB.emp_nm = ds.Tables[0].Rows[0]["emp_nm"].ToString();
                hEX001TB.entr_dt = ds.Tables[0].Rows[0]["entr_dt"].ToString();
                hEX001TB.dept_nm = ds.Tables[0].Rows[0]["dept_nm"].ToString();
                hEX001TB.ex_date = ds.Tables[0].Rows[0]["ex_date"].ToString();
                hEX001TB.dilig_nm = ds.Tables[0].Rows[0]["dilig_nm"].ToString();
                hEX001TB.ex_btime = ds.Tables[0].Rows[0]["ex_btime"].ToString();
                hEX001TB.ex_etime = ds.Tables[0].Rows[0]["ex_etime"].ToString();
                hEX001TB.ex_hh = ds.Tables[0].Rows[0]["ex_hh"].ToString();
                hEX001TB.ex_remark = ds.Tables[0].Rows[0]["ex_remark"].ToString();
                hEX001TB.ck_flg = ds.Tables[0].Rows[0]["ck_flg"].ToString();
                hEX001TB.ck_flg_nm = ds.Tables[0].Rows[0]["ck_flg_nm"].ToString();
                hEX001TB.ck_remark = ds.Tables[0].Rows[0]["ck_remark"].ToString();
            }
            return View(hEX001TB);
        }

        /// <summary>
        /// 自助请假审核 POST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("check")]
        public ActionResult Check(FormCollection collection)
        {
            string id = Request.QueryString["id"];//请假信息的主键
            RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();

            //把职员请假的消息发送给应用接受并返回一个提示消息给管理员
            string corpid = ConfigurationManager.AppSettings["CorpId"]; //企业ID
            string secret = ConfigurationManager.AppSettings["Secret_3"];//应用的凭证密钥 每个应用都不一样

            //把用户提交的请假申请insert到数据表中，然后接收返回的信息
            wx_site.RenshiService.HEX001TB hex001tb = new RenshiService.HEX001TB();

            hex001tb.id = id;
            hex001tb.ex_date = DateTime.Now;
            hex001tb.ck_remark = collection["ck_remark"];
            hex001tb.ck_flg = collection["check"];
            DataSet ds = renshiService.CheckHEX001TB(hex001tb);

            if (ds.Tables["MSG"].Rows.Count > 0)
            {
                //要把用户提交的信息封装为一个model，并转换成json包
                //首先发送的消息内容需要封装成一个json包
                //消息以卡片文本的形式发送，其中需要参数。
                Msg_Text_card_content text_card = new Msg_Text_card_content();
                text_card.title = ds.Tables["MSG"].Rows[0]["title"].ToString();
                text_card.description = ds.Tables["MSG"].Rows[0]["description"].ToString();
                text_card.url = ds.Tables["MSG"].Rows[0]["url"].ToString();
                text_card.btntxt = ds.Tables["MSG"].Rows[0]["btntxt"].ToString();
                //转换成json包之前先处理一下反斜杠 \
                Json_Replace json_Replace = new Json_Replace();
                text_card.description = json_Replace.Json_add_4(text_card.description);
                //转换成json包
                string text_content_json = JsonConvert.SerializeObject(text_card);

                //再封装POST提交需要的JSON包
                Msg_Text_card msg_Text_Card = new Msg_Text_card();
                msg_Text_Card.touser = ds.Tables["MSG"].Rows[0]["touser"].ToString();
                msg_Text_Card.toparty = ds.Tables["MSG"].Rows[0]["toparty"].ToString();
                msg_Text_Card.totag = ds.Tables["MSG"].Rows[0]["totag"].ToString();
                msg_Text_Card.msgtype = ds.Tables["MSG"].Rows[0]["msgtype"].ToString();
                msg_Text_Card.agentid = int.Parse(ds.Tables["MSG"].Rows[0]["agentid"].ToString());
                msg_Text_Card.textcard = text_content_json;
                //Json.NET序列化
                string jsonData = JsonConvert.SerializeObject(msg_Text_Card);
                jsonData = json_Replace.Json_rep(jsonData);
                //推送消息给管理员审核
                SendMessage sendMessage = new SendMessage();
                string strjson = sendMessage.SendMsg(corpid, secret, jsonData);

            }

            ds = renshiService.SearchHEX001TB(hex001tb);
            HEX001TB hEX001TB = new HEX001TB();
            if (ds.Tables[0].Rows.Count > 0)
            {
                hEX001TB.emp_no = ds.Tables[0].Rows[0]["emp_no"].ToString();
                hEX001TB.emp_nm = ds.Tables[0].Rows[0]["emp_nm"].ToString();
                hEX001TB.entr_dt = ds.Tables[0].Rows[0]["entr_dt"].ToString();
                hEX001TB.dept_nm = ds.Tables[0].Rows[0]["dept_nm"].ToString();
                hEX001TB.ex_date = ds.Tables[0].Rows[0]["ex_date"].ToString();
                hEX001TB.dilig_nm = ds.Tables[0].Rows[0]["dilig_nm"].ToString();
                hEX001TB.ex_btime = ds.Tables[0].Rows[0]["ex_btime"].ToString();
                hEX001TB.ex_etime = ds.Tables[0].Rows[0]["ex_etime"].ToString();
                hEX001TB.ex_hh = ds.Tables[0].Rows[0]["ex_hh"].ToString();
                hEX001TB.ex_remark = ds.Tables[0].Rows[0]["ex_remark"].ToString();
                hEX001TB.ck_flg = ds.Tables[0].Rows[0]["ck_flg"].ToString();
                hEX001TB.ck_flg_nm = ds.Tables[0].Rows[0]["ck_flg_nm"].ToString();
                hEX001TB.ck_remark = ds.Tables[0].Rows[0]["ck_remark"].ToString();
            }
            return View(hEX001TB);
        }

        /// <summary>
        /// 审核完成后，返回给用户的链接action
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("queryqj")]
        public ActionResult Queryqj()
        {
            //窗体加载时要先显示职员的请假信息
            string id = Request.QueryString["id"];//请假信息的主键
            RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();
            wx_site.RenshiService.HEX001TB hex001tb = new RenshiService.HEX001TB();
            hex001tb.id = id;
            hex001tb.ex_date = DateTime.Now;
            DataSet ds = renshiService.SearchHEX001TB(hex001tb);
            HEX001TB hEX001TB = new HEX001TB();
            if (ds.Tables[0].Rows.Count > 0)
            {
                hEX001TB.emp_no = ds.Tables[0].Rows[0]["emp_no"].ToString();
                hEX001TB.emp_nm = ds.Tables[0].Rows[0]["emp_nm"].ToString();
                hEX001TB.entr_dt = ds.Tables[0].Rows[0]["entr_dt"].ToString();
                hEX001TB.dept_nm = ds.Tables[0].Rows[0]["dept_nm"].ToString();
                hEX001TB.ex_date = ds.Tables[0].Rows[0]["ex_date"].ToString();
                hEX001TB.dilig_nm = ds.Tables[0].Rows[0]["dilig_nm"].ToString();
                hEX001TB.ex_btime = ds.Tables[0].Rows[0]["ex_btime"].ToString();
                hEX001TB.ex_etime = ds.Tables[0].Rows[0]["ex_etime"].ToString();
                hEX001TB.ex_hh = ds.Tables[0].Rows[0]["ex_hh"].ToString();
                hEX001TB.ex_remark = ds.Tables[0].Rows[0]["ex_remark"].ToString();
                hEX001TB.ck_flg = ds.Tables[0].Rows[0]["ck_flg"].ToString();
                hEX001TB.ck_flg_nm = ds.Tables[0].Rows[0]["ck_flg_nm"].ToString();
                hEX001TB.ck_remark = ds.Tables[0].Rows[0]["ck_remark"].ToString();
            }
            return View(hEX001TB);
        }

        /// <summary>
        /// 查询工资前需要先登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("login")]
        public ActionResult Login()
        {
            return View(1);
        }

        [HttpPost]
        [ActionName("login")]
        public ActionResult Login(FormCollection collection)
        {
            string id = Request.QueryString["id"];
            string pwd = collection["password"];
            RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();
            bool bl = renshiService.UserCheck(id, pwd);
            if (bl == true)
            {
                return Redirect("~/renshi/gongzi?id=" + id);
            }
            else
            {
                return View(0);
            }
        }

        /// <summary>
        /// 职员确认考勤事件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("qrkq")]
        public ActionResult Qrkq()
        {
            string id = Request.QueryString["id"];
            List<Kaoqin_Jiaban> ltjiaban = new List<Kaoqin_Jiaban>();
            Kaoqin_Jiaban kaoqin_Jiaban = new Kaoqin_Jiaban();
            ltjiaban.Add(kaoqin_Jiaban);
            ViewData["jiaban"] = ltjiaban;

            List<Kaoqin_Qingjia> ltqingjia = new List<Kaoqin_Qingjia>();
            Kaoqin_Qingjia kaoqin_Qingjia = new Kaoqin_Qingjia();
            ltqingjia.Add(kaoqin_Qingjia);
            ViewData["qingjia"] = ltqingjia;

            List<Kaoqin_Yichang> ltyichang = new List<Kaoqin_Yichang>();
            Kaoqin_Yichang kaoqin_Yichang = new Kaoqin_Yichang();
            ltyichang.Add(kaoqin_Yichang);
            ViewData["yichang"] = ltyichang;

            return View();
        }

        [HttpPost]
        [ActionName("qrkq")]
        public ActionResult Qrkq(FormCollection collection)
        {
            string month = collection["combox_year"];
            month = month + collection["combox_month"];
            string emp_no = Request.QueryString["id"];
            RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();
            DataSet ds = renshiService.GetHRT010TB(emp_no, month);

            //加班项目
            if (ds.Tables[0].Rows.Count > 0)
            {
                List<Kaoqin_Jiaban> ltjiaban = new List<Kaoqin_Jiaban>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Kaoqin_Jiaban kaoqin_Jiaban = new Kaoqin_Jiaban();
                    kaoqin_Jiaban.Rt_date = ds.Tables[0].Rows[i]["rt_date"].ToString();
                    kaoqin_Jiaban.Wd_day = ds.Tables[0].Rows[i]["wd_day"].ToString();
                    kaoqin_Jiaban.Dilig_nm = ds.Tables[0].Rows[i]["dilig_nm"].ToString();
                    kaoqin_Jiaban.Fq_name = ds.Tables[0].Rows[i]["fq_name"].ToString();
                    kaoqin_Jiaban.Rt_btime = ds.Tables[0].Rows[i]["rt_btime"].ToString();
                    kaoqin_Jiaban.Rt_etime = ds.Tables[0].Rows[i]["rt_etime"].ToString();
                    kaoqin_Jiaban.Rt_hh = Convert.ToDecimal(ds.Tables[0].Rows[i]["rt_hh"].ToString());
                    ltjiaban.Add(kaoqin_Jiaban);
                    ViewData["jiaban"] = ltjiaban;
                }
            }
            else
            {
                List<Kaoqin_Jiaban> ltjiaban = new List<Kaoqin_Jiaban>();
                Kaoqin_Jiaban kaoqin_Jiaban = new Kaoqin_Jiaban();
                ltjiaban.Add(kaoqin_Jiaban);
                ViewData["jiaban"] = ltjiaban;
            }
            //请假项目
            if (ds.Tables[1].Rows.Count > 0)
            {
                List<Kaoqin_Qingjia> ltqingjia = new List<Kaoqin_Qingjia>();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    Kaoqin_Qingjia kaoqin_Qingjia = new Kaoqin_Qingjia();
                    kaoqin_Qingjia.Ex_date = ds.Tables[1].Rows[i]["ex_date"].ToString();
                    kaoqin_Qingjia.Wd_day = ds.Tables[1].Rows[i]["wd_day"].ToString();
                    kaoqin_Qingjia.Dilig_nm = ds.Tables[1].Rows[i]["dilig_nm"].ToString();
                    kaoqin_Qingjia.Fq_name = ds.Tables[1].Rows[i]["fq_name"].ToString();
                    kaoqin_Qingjia.Ex_btime = ds.Tables[1].Rows[i]["ex_btime"].ToString();
                    kaoqin_Qingjia.Ex_etime = ds.Tables[1].Rows[i]["ex_etime"].ToString();
                    kaoqin_Qingjia.Ex_hh = Convert.ToDecimal(ds.Tables[1].Rows[i]["ex_hh"].ToString());
                    ltqingjia.Add(kaoqin_Qingjia);
                    ViewData["qingjia"] = ltqingjia;
                }
            }
            else
            {
                List<Kaoqin_Qingjia> ltqingjia = new List<Kaoqin_Qingjia>();
                Kaoqin_Qingjia kaoqin_Qingjia = new Kaoqin_Qingjia();
                ltqingjia.Add(kaoqin_Qingjia);
                ViewData["qingjia"] = ltqingjia;
            }
            //异常出勤
            if (ds.Tables[2].Rows.Count > 0)
            {
                List<Kaoqin_Yichang> ltyichang = new List<Kaoqin_Yichang>();
                for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                {
                    Kaoqin_Yichang kaoqin_Yichang = new Kaoqin_Yichang();
                    kaoqin_Yichang.Rt_date = ds.Tables[2].Rows[i]["rt_date"].ToString();
                    kaoqin_Yichang.Wd_day = ds.Tables[2].Rows[i]["wd_day"].ToString();
                    kaoqin_Yichang.Dilig_nm = ds.Tables[2].Rows[i]["dilig_nm"].ToString();
                    kaoqin_Yichang.Fq_name = ds.Tables[2].Rows[i]["fq_name"].ToString();
                    kaoqin_Yichang.Rt_btime = ds.Tables[2].Rows[i]["rt_btime"].ToString();
                    kaoqin_Yichang.Rt_etime = ds.Tables[2].Rows[i]["rt_etime"].ToString();
                    kaoqin_Yichang.Rt_hh = Convert.ToDecimal(ds.Tables[2].Rows[i]["rt_hh"].ToString());
                    ltyichang.Add(kaoqin_Yichang);
                    ViewData["yichang"] = ltyichang;
                }
            }
            else
            {
                List<Kaoqin_Yichang> ltyichang = new List<Kaoqin_Yichang>();
                Kaoqin_Yichang kaoqin_Yichang = new Kaoqin_Yichang();
                ltyichang.Add(kaoqin_Yichang);
                ViewData["yichang"] = ltyichang;
            }

            return View();
        }

        /// <summary>
        /// 职员确认工资
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("qrgz")]
        public ActionResult Qrgz()
        {
            List<Gongzi_Zhifu> ltzhifu = new List<Gongzi_Zhifu>();
            Gongzi_Zhifu gongzi_Zhifu = new Gongzi_Zhifu();
            ltzhifu.Add(gongzi_Zhifu);
            ViewData["zhifu"] = ltzhifu;

            List<Gongzi_Kouchu> ltkouchu = new List<Gongzi_Kouchu>();
            Gongzi_Kouchu gongzi_Kouchu = new Gongzi_Kouchu();
            ltkouchu.Add(gongzi_Kouchu);
            ViewData["kouchu"] = ltkouchu;

            List<Gongzi_Heji> ltheji = new List<Gongzi_Heji>();
            Gongzi_Heji gongzi_Heji = new Gongzi_Heji();
            ltheji.Add(gongzi_Heji);
            ViewData["heji"] = ltheji;

            return View();
        }

        [HttpPost]
        [ActionName("qrgz")]
        public ActionResult Qrgz(FormCollection collection)
        {
            string month = collection["combox_year"];
            month = month + collection["combox_month"];
            string emp_no = Request.QueryString["id"];
            RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();
            DataSet ds = renshiService.GetGongZi(emp_no, month);

            //津贴项目
            if (ds.Tables[0].Rows.Count > 0)
            {
                List<Gongzi_Zhifu> ltzhifu = new List<Gongzi_Zhifu>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Gongzi_Zhifu gongzi_Zhifu = new Gongzi_Zhifu();
                    gongzi_Zhifu.Emp_no = ds.Tables[0].Rows[i]["emp_no"].ToString();
                    gongzi_Zhifu.Allow_cd = ds.Tables[0].Rows[i]["allow_cd"].ToString();
                    gongzi_Zhifu.Allow_nm1 = ds.Tables[0].Rows[i]["allow_nm1"].ToString();
                    gongzi_Zhifu.Allow = Convert.ToDecimal(ds.Tables[0].Rows[i]["allow"].ToString());
                    ltzhifu.Add(gongzi_Zhifu);
                    ViewData["zhifu"] = ltzhifu;
                }
            }
            else
            {
                List<Gongzi_Zhifu> ltzhifu = new List<Gongzi_Zhifu>();
                Gongzi_Zhifu gongzi_Zhifu = new Gongzi_Zhifu();
                ltzhifu.Add(gongzi_Zhifu);
                ViewData["zhifu"] = ltzhifu;
            }
            //扣除项目
            if (ds.Tables[1].Rows.Count > 0)
            {
                List<Gongzi_Kouchu> ltkouchu = new List<Gongzi_Kouchu>();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    Gongzi_Kouchu gongzi_Kouchu = new Gongzi_Kouchu();
                    gongzi_Kouchu.Emp_no = ds.Tables[1].Rows[i]["emp_no"].ToString();
                    gongzi_Kouchu.Sub_cd = ds.Tables[1].Rows[i]["sub_cd"].ToString();
                    gongzi_Kouchu.Allow_nm1 = ds.Tables[1].Rows[i]["allow_nm1"].ToString();
                    gongzi_Kouchu.Sub_amt = Convert.ToDecimal(ds.Tables[1].Rows[i]["sub_amt"].ToString());
                    ltkouchu.Add(gongzi_Kouchu);
                    ViewData["kouchu"] = ltkouchu;
                }
            }
            else
            {
                List<Gongzi_Kouchu> ltkouchu = new List<Gongzi_Kouchu>();
                Gongzi_Kouchu gongzi_Kouchu = new Gongzi_Kouchu();
                ltkouchu.Add(gongzi_Kouchu);
                ViewData["kouchu"] = ltkouchu;
            }
            //合计金额
            if (ds.Tables[2].Rows.Count > 0)
            {
                List<Gongzi_Heji> ltheji = new List<Gongzi_Heji>();
                for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                {
                    Gongzi_Heji gongzi_Heji = new Gongzi_Heji();
                    gongzi_Heji.PROV_TOT_AMT = Convert.ToDecimal(ds.Tables[2].Rows[i]["PROV_TOT_AMT"].ToString());
                    gongzi_Heji.REAL_PROV_AMT = Convert.ToDecimal(ds.Tables[2].Rows[i]["REAL_PROV_AMT"].ToString());
                    gongzi_Heji.SUB_TOT_AMT = Convert.ToDecimal(ds.Tables[2].Rows[i]["SUB_TOT_AMT"].ToString());
                    ltheji.Add(gongzi_Heji);
                    ViewData["heji"] = ltheji;
                }
            }
            else
            {
                List<Gongzi_Heji> ltheji = new List<Gongzi_Heji>();
                Gongzi_Heji gongzi_Heji = new Gongzi_Heji();
                ltheji.Add(gongzi_Heji);
                ViewData["heji"] = ltheji;
            }

            return View();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("change_pwd")]
        public ActionResult Change_pwd()
        {
            return View();
        }

        [HttpPost]
        [ActionName("change_pwd")]
        public ActionResult Change_pwd(FormCollection collection)
        {
            string id = Request.QueryString["id"];
            string old = collection["old"];
            string pwd = collection["password"];
            string pwd1 = collection["password1"];
            if (pwd != pwd1)
            {
                return View(2);
            }
            else
            {
                RenshiService.renshiSoapClient renshiService = new RenshiService.renshiSoapClient();
                bool bl = renshiService.UserCheck(id, old);
                if (bl == true)
                {
                    bl = false;
                    bl = renshiService.Change_pwd(id, pwd);
                    if (bl == true)
                    {
                        return View(1);
                    }
                    else
                    {
                        return View(0);
                    }
                }
                else
                {
                    return View(3);
                }
            }
        }

        //public void Abc()
        //{

        //    //要把用户提交的信息封装为一个model，并转换成json包
        //    //首先发送的消息内容需要封装成一个json包
        //    //消息以卡片文本的形式发送，其中需要参数。
        //    //Msg_Text_card_content text_card = new Msg_Text_card_content();
        //    //text_card.title = "收到";

        //    Msg_Text_card_content text_card = new Msg_Text_card_content();
        //    text_card.title = "有新的请假申请";
        //    text_card.description = "<div class=$\"gray$\">" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "</div> <div class=$\"normal$\">恭喜你抽中iPhone 7一台，领奖码：xxxx</div><div class=$\"highlight$\">请于2016年10月10日前联系行政同事领取</div>";
        //    text_card.url = "http://unierp.sungteuk.com:88/renshi/dangan";
        //    text_card.btntxt = "点击进行审核";
        //    string text_content_json = JsonConvert.SerializeObject(text_card);
        //    //再封装POST提交需要的JSON包

        //    Msg_Text_card msg_Text_Card = new Msg_Text_card();
        //    msg_Text_Card.touser = "";
        //    msg_Text_Card.toparty = "1";
        //    msg_Text_Card.totag = "";
        //    msg_Text_Card.msgtype = "textcard";
        //    msg_Text_Card.agentid = 1000003;
        //    msg_Text_Card.textcard = text_content_json;
        //    //Json.NET序列化
        //    string jsonData = JsonConvert.SerializeObject(msg_Text_Card);
        //    jsonData = jsonData.Replace("\\", "");
        //    jsonData = jsonData.Replace("\"{", "{");
        //    jsonData = jsonData.Replace("}\"", "}");
        //    jsonData = jsonData.Replace("$", "\\");

        //    Response.Write(jsonData);
        //}
    }
}