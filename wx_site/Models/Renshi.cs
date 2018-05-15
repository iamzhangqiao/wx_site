using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wx_site.Models
{
    public class Renshi
    {

    }
    /// <summary>
    /// 职员档案类
    /// </summary>
    public class Dangan
    {
        /// <summary>
        /// 职员工号
        /// </summary>
        public string Emp_no { get; set; }
        /// <summary>
        /// 职员姓名
        /// </summary>
        public string Emp_nm { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        public string Entr_dt { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string Dept_nm { get; set; }
        /// <summary>
        /// 职员请假申请后返回的信息代码
        /// </summary>
        public string msg_cd { get; set; }
        /// <summary>
        /// 职员请假申请后返回的信息内容
        /// </summary>
        public string msg_text { get; set; }
    }

    /// <summary>
    /// 工资信息中的支付项目类
    /// </summary>
    public class Gongzi_Zhifu
    {
        /// <summary>
        /// 职员档案
        /// </summary>
        public string Emp_no { get; set; }
        /// <summary>
        /// 津贴代码
        /// </summary>
        public string Allow_cd { get; set; }
        /// <summary>
        /// 津贴名称
        /// </summary>
        public string Allow_nm1 { get; set; }
        /// <summary>
        /// 津贴金额
        /// </summary>
        public decimal Allow { get; set; }
    }

    /// <summary>
    /// 工资信息中的扣除项目类
    /// </summary>
    public class Gongzi_Kouchu
    {
        /// <summary>
        /// 职员档案
        /// </summary>
        public string Emp_no { get; set; }
        /// <summary>
        /// 津贴代码
        /// </summary>
        public string Sub_cd { get; set; }
        /// <summary>
        /// 津贴名称
        /// </summary>
        public string Allow_nm1 { get; set; }
        /// <summary>
        /// 津贴金额
        /// </summary>
        public decimal Sub_amt { get; set; }
    }

    /// <summary>
    /// 工资合计类
    /// </summary>
    public class Gongzi_Heji
    {
        /// <summary>
        /// 支付总额
        /// </summary>
        public decimal PROV_TOT_AMT { get; set; }
        /// <summary>
        /// 扣除总额
        /// </summary>
        public decimal SUB_TOT_AMT { get; set; }
        /// <summary>
        /// 实付金额
        /// </summary>
        public decimal REAL_PROV_AMT { get; set; }
    }

    /// <summary>
    /// 考勤查询中的加班信息
    /// </summary>
    public class Kaoqin_Jiaban
    {
        /// <summary>
        /// 出勤日期
        /// </summary>
        public string Rt_date { get; set; }
        /// <summary>
        /// 星期
        /// </summary>
        public string Wd_day { get; set; }
        /// <summary>
        /// 出勤状态码
        /// </summary>
        public string Wd_dilig_cd { get; set; }
        /// <summary>
        /// 考勤名称
        /// </summary>
        public string Dilig_nm { get; set; }
        /// <summary>
        /// 班次名称
        /// </summary>
        public string Fq_name { get; set; }
        /// <summary>
        /// 上班刷卡时间
        /// </summary>
        public string Rt_btime { get; set; }
        /// <summary>
        /// 下班刷卡时间
        /// </summary>
        public string Rt_etime { get; set; }
        /// <summary>
        /// 加班时间
        /// </summary>
        public decimal Rt_hh { get; set; }
    }

    /// <summary>
    /// 考勤查询中的请假信息
    /// </summary>
    public class Kaoqin_Qingjia
    {
        /// <summary>
        /// 出勤日期
        /// </summary>
        public string Ex_date { get; set; }
        /// <summary>
        /// 星期
        /// </summary>
        public string Wd_day { get; set; }
        /// <summary>
        /// 考勤名称
        /// </summary>
        public string Dilig_nm { get; set; }
        /// <summary>
        /// 班次名称
        /// </summary>
        public string Fq_name { get; set; }
        /// <summary>
        /// 请假开始时间
        /// </summary>
        public string Ex_btime { get; set; }
        /// <summary>
        /// 请假结束时间
        /// </summary>
        public string Ex_etime { get; set; }
        /// <summary>
        /// 请假时间
        /// </summary>
        public decimal Ex_hh { get; set; }
    }

    /// <summary>
    /// 考勤查询中的异常出勤信息，包含迟到早退旷工
    /// </summary>
    public class Kaoqin_Yichang
    {
        /// <summary>
        /// 出勤日期
        /// </summary>
        public string Rt_date { get; set; }
        /// <summary>
        /// 星期
        /// </summary>
        public string Wd_day { get; set; }
        /// <summary>
        /// 出勤状态码
        /// </summary>
        public string Wd_dilig_cd { get; set; }
        /// <summary>
        /// 考勤名称
        /// </summary>
        public string Dilig_nm { get; set; }
        /// <summary>
        /// 班次名称
        /// </summary>
        public string Fq_name { get; set; }
        /// <summary>
        /// 上班刷卡时间
        /// </summary>
        public string Rt_btime { get; set; }
        /// <summary>
        /// 下班刷卡时间
        /// </summary>
        public string Rt_etime { get; set; }
        /// <summary>
        /// 异常考勤时间
        /// </summary>
        public decimal Rt_hh { get; set; }
    }

    /// <summary>
    /// 请假申请表Model对象
    /// </summary>
    public class HEX001TB
    {
        /// <summary>
        /// 职员工号
        /// </summary>
        public string emp_no { get; set; }
        /// <summary>
        /// 职员姓名
        /// </summary>
        public string emp_nm { get; set; }
        /// <summary>
        /// 所在部门
        /// </summary>
        public string dept_nm { get; set; }
        /// <summary>
        /// 入社日期
        /// </summary>
        public string entr_dt { get; set; }
        /// <summary>
        /// 请假日期
        /// </summary>
        public string ex_date { get; set; }
        /// <summary>
        /// 请假种类
        /// </summary>
        public string dilig_nm { get; set; }
        /// <summary>
        /// 请假开始时间
        /// </summary>
        public string ex_btime { get; set; }
        /// <summary>
        /// 请假结束时间
        /// </summary>
        public string ex_etime { get; set; }
        /// <summary>
        /// 请假原因
        /// </summary>
        public string ex_remark { get; set; }
        /// <summary>
        /// 合计小时
        /// </summary>
        public string ex_hh { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public string ck_flg { get; set; }
        /// <summary>
        /// 审核状态名
        /// </summary>
        public string ck_flg_nm { get; set; }
        /// <summary>
        /// 审核拒绝原因
        /// </summary>
        public string ck_remark { get; set; }

    }
}