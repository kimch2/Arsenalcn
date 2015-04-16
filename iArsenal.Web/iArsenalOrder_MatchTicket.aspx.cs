﻿using System;
using System.Data.SqlClient;

using iArsenal.Entity;

namespace iArsenal.Web
{
    public partial class iArsenalOrder_MatchTicket : MemberPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Assign Control Property

            ctrlPortalMatchInfo.MatchGuid = MatchGuid;

            #endregion

            if (!IsPostBack)
            {
                InitForm();
            }
        }

        private int OrderID
        {
            get
            {
                int _orderID;
                if (!string.IsNullOrEmpty(Request.QueryString["OrderID"]) && int.TryParse(Request.QueryString["OrderID"], out _orderID))
                {
                    return _orderID;
                }
                else
                    return int.MinValue;
            }
        }

        private Guid MatchGuid
        {
            get
            {
                if (OrderID > 0)
                {
                    OrdrTicket o = new OrdrTicket(OrderID);

                    if (o.OIMatchTicket != null)
                    { return Guid.Empty; }
                    else
                    { return o.OIMatchTicket.MatchGuid; }
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["MatchGuid"]))
                {
                    try { return new Guid(Request.QueryString["MatchGuid"]); }
                    catch { return Guid.Empty; }
                }
                else
                    return Guid.Empty;
            }
        }

        private void InitForm()
        {
            try
            {
                lblMemberName.Text = string.Format("<b>{0}</b> (<em>NO.{1}</em>)", this.MemberName, this.MID.ToString());
                lblMemberACNInfo.Text = string.Format("<b>{0}</b> (<em>ID.{1}</em>)", this.Username, this.UID.ToString());

                if (MatchGuid.Equals(Guid.Empty))
                {
                    Response.Redirect("iArsenalOrder_MatchList.aspx");
                    Response.Clear();
                }

                MatchTicket mt = MatchTicket.Cache.Load(MatchGuid);

                if (mt == null)
                {
                    throw new Exception("无相关比赛信息，请联系管理员");
                }

                if (OrderID <= 0 && mt.Deadline < DateTime.Now)
                {
                    throw new Exception("此球票预定已过截至时间，请联系管理员");
                }

                //if (!mt.IsMemberCouldPurchase(this.CurrentMemberPeriod))
                //{
                //    ClientScript.RegisterClientScriptBlock(typeof(string), "failed",
                //        string.Format("alert('由于球票数量紧张，所有阿森纳主场球票预订，均只向收费会员开放，请在跳转页面后续费或升级会员资格');window.location.href = 'iArsenalMemberPeriod.aspx'"), true);
                //}

                Product p = Product.Cache.Load(mt.ProductCode);

                if (p == null)
                {
                    throw new Exception("无相关商品信息，请联系管理员");
                }

                lblMatchTicketInfo.Text = string.Format("<em>【{0}】{1}({2})</em>", mt.LeagueName, mt.TeamName, Arsenal_Team.Cache.Load(mt.TeamGuid).TeamEnglishName);
                lblMatchTicketPlayTime.Text = string.Format("<em>【伦敦】{0}</em>", mt.PlayTimeLocal.ToString("yyyy-MM-dd HH:mm"));

                string _strRank = mt.ProductInfo.Trim();
                if (lblMatchTicketRank != null && !string.IsNullOrEmpty(_strRank))
                {
                    lblMatchTicketRank.Text = string.Format("<em>{0} - {1}</em>", _strRank.Substring(_strRank.Length - 7, 7), p.PriceInfo);
                }
                else
                {
                    lblMatchTicketRank.Text = string.Empty;
                }

                if (mt.AllowMemberClass.HasValue && mt.AllowMemberClass.Value == 2)
                {
                    lblAllowMemberClass.Text = "<em>只限高级会员(Premier)</em>";
                }
                else if (mt.AllowMemberClass.HasValue && mt.AllowMemberClass == 1)
                {
                    lblAllowMemberClass.Text = "<em>普通会员(Core)以上</em>";
                }
                else
                {
                    lblAllowMemberClass.Text = "无";
                }

                if (OrderID > 0)
                {
                    //Order o = new Order();
                    //o.OrderID = OrderID;
                    //o.Select();

                    OrdrTicket o = new OrdrTicket(OrderID);

                    if (ConfigAdmin.IsPluginAdmin(UID) && o != null)
                    {
                        lblMemberName.Text = string.Format("<b>{0}</b> (<em>NO.{1}</em>)", o.MemberName, o.MemberID.ToString());

                        Member m = new Member();
                        m.MemberID = o.MemberID;
                        m.Select();

                        if (m == null || !m.IsActive)
                        {
                            throw new Exception("无此会员信息");
                        }
                        else
                        {
                            lblMemberACNInfo.Text = string.Format("<b>{0}</b> (<em>ID.{1}</em>)", m.AcnName, m.AcnID.ToString());

                            #region Set Member Nation & Region
                            if (!string.IsNullOrEmpty(m.Nation))
                            {
                                if (m.Nation.Equals("中国"))
                                {
                                    ddlNation.SelectedValue = m.Nation;

                                    string[] region = m.Region.Split('|');
                                    if (region.Length > 1)
                                    {
                                        tbRegion1.Text = region[0];
                                        tbRegion2.Text = region[1];
                                    }
                                    else
                                    {
                                        tbRegion1.Text = region[0];
                                        tbRegion2.Text = string.Empty;
                                    }
                                }
                                else
                                {
                                    ddlNation.SelectedValue = "其他";
                                    if (m.Nation.Equals("其他"))
                                        tbNation.Text = string.Empty;
                                    else
                                        tbNation.Text = m.Nation;
                                }
                            }
                            else
                            {
                                ddlNation.SelectedValue = string.Empty;
                            }
                            #endregion

                            tbIDCardNo.Text = m.IDCardNo;
                            tbPassportNo.Text = m.PassportNo;
                            tbPassportName.Text = m.PassportName;
                            tbMobile.Text = m.Mobile;
                            tbQQ.Text = m.QQ;
                            tbEmail.Text = m.Email;

                            tbOrderDescription.Text = o.Description;
                        }
                    }
                    else
                    {
                        if (o == null || !o.MemberID.Equals(MID) || !o.IsActive)
                            throw new Exception("此订单无效或非当前用户订单");
                    }

                    OrdrItmMatchTicket oi = o.OIMatchTicket;

                    if (oi != null)
                    {
                        //tbQuantity.Text = oi.Quantity.ToString();
                        tbTravelDate.Text = oi.TravelDate.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        throw new Exception("此订单未填写订票信息");
                    }
                }
                else
                {
                    //Fill Member draft information into textbox
                    Member m = new Member();
                    m.MemberID = this.MID;
                    m.Select();

                    #region Set Member Nation & Region
                    if (!string.IsNullOrEmpty(m.Nation))
                    {
                        if (m.Nation.Equals("中国"))
                        {
                            ddlNation.SelectedValue = m.Nation;

                            string[] region = m.Region.Split('|');
                            if (region.Length > 1)
                            {
                                tbRegion1.Text = region[0];
                                tbRegion2.Text = region[1];
                            }
                            else
                            {
                                tbRegion1.Text = region[0];
                                tbRegion2.Text = string.Empty;
                            }
                        }
                        else
                        {
                            ddlNation.SelectedValue = "其他";
                            if (m.Nation.Equals("其他"))
                                tbNation.Text = string.Empty;
                            else
                                tbNation.Text = m.Nation;
                        }
                    }
                    else
                    {
                        ddlNation.SelectedValue = string.Empty;
                    }
                    #endregion

                    tbIDCardNo.Text = m.IDCardNo;
                    tbPassportNo.Text = m.PassportNo;
                    tbPassportName.Text = m.PassportName;
                    tbMobile.Text = m.Mobile;
                    tbQQ.Text = m.QQ;
                    tbEmail.Text = m.Email;

                    tbTravelDate.Text = mt.PlayTime.AddDays(-2).ToString("yyyy-MM-dd");
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof(string), "failed", string.Format("alert('{0}');window.location.href = 'iArsenalOrder_MatchList.aspx'", ex.Message.ToString()), true);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = ConfigGlobal.SQLConnectionStrings)
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    if (MatchGuid.Equals(Guid.Empty))
                    {
                        Response.Redirect("iArsenalOrder_MatchList.aspx");
                        Response.Clear();
                    }

                    MatchTicket mt = MatchTicket.Cache.Load(MatchGuid);

                    if (mt == null)
                    {
                        throw new Exception("无相关比赛信息，请联系管理员");
                    }

                    Member m = new Member();
                    m.MemberID = this.MID;
                    m.Select();

                    // Update Member Information
                    #region Get Member Nation & Region
                    string _nation = ddlNation.SelectedValue;

                    if (!string.IsNullOrEmpty(_nation))
                    {
                        if (_nation.Equals("中国"))
                        {
                            m.Nation = _nation;
                            if (!string.IsNullOrEmpty(tbRegion1.Text.Trim()))
                            {
                                if (!string.IsNullOrEmpty(tbRegion2.Text.Trim()))
                                {
                                    m.Region = string.Format("{0}|{1}", tbRegion1.Text.Trim(), tbRegion2.Text.Trim());
                                }
                                else
                                {
                                    m.Region = tbRegion1.Text.Trim();
                                }
                            }
                            else
                            {
                                m.Region = string.Empty;
                            }
                        }
                        else if (!string.IsNullOrEmpty(tbNation.Text.Trim()))
                        {
                            m.Nation = tbNation.Text.Trim();
                            m.Region = string.Empty;
                        }
                        else
                        {
                            m.Nation = _nation;
                            m.Region = string.Empty;
                        }
                    }
                    else
                    {
                        m.Nation = string.Empty;
                        m.Region = string.Empty;
                    }
                    #endregion

                    if (!string.IsNullOrEmpty(tbIDCardNo.Text.Trim()))
                        m.IDCardNo = tbIDCardNo.Text.Trim();
                    else
                        throw new Exception("请填写会员身份证号码");

                    if (!string.IsNullOrEmpty(tbPassportNo.Text.Trim()))
                        m.PassportNo = tbPassportNo.Text.Trim();
                    else
                        throw new Exception("请填写会员护照编号");

                    if (!string.IsNullOrEmpty(tbPassportName.Text.Trim()))
                        m.PassportName = tbPassportName.Text.Trim();
                    else
                        throw new Exception("请填写会员护照姓名");

                    if (!string.IsNullOrEmpty(tbMobile.Text.Trim()))
                        m.Mobile = tbMobile.Text.Trim();
                    else
                        throw new Exception("请填写会员手机");

                    if (!string.IsNullOrEmpty(tbQQ.Text.Trim()))
                        m.QQ = tbQQ.Text.Trim();
                    else
                        throw new Exception("请填写会员微信/QQ");

                    if (!string.IsNullOrEmpty(tbEmail.Text.Trim()))
                        m.Email = tbEmail.Text.Trim();
                    else
                        throw new Exception("请填写会员邮箱");

                    m.MemberType = MemberType.Match;

                    m.Update();

                    // New Order
                    Order o = new Order();

                    if (OrderID > 0)
                    {
                        o.OrderID = OrderID;
                        o.Select();
                    }

                    o.Mobile = m.Mobile;
                    o.UpdateTime = DateTime.Now;
                    o.Description = tbOrderDescription.Text.Trim();

                    if (OrderID > 0)
                    {
                        o.Update(trans);
                    }
                    else
                    {
                        o.MemberID = m.MemberID;
                        o.MemberName = m.Name;

                        o.Address = m.Address;
                        o.Payment = string.Empty;

                        o.Price = 0;
                        o.Sale = null;
                        o.Deposit = null;
                        o.Status = OrderStatusType.Draft;
                        o.Rate = 0;
                        o.CreateTime = DateTime.Now;
                        o.IsActive = true;
                        o.Remark = string.Empty;

                        o.Insert(trans);
                        //o.Select();
                    }

                    //Get the Order ID after Insert new one

                    if (o.OrderID > 0)
                    {
                        //Remove Order Item of this Order
                        if (o.OrderID.Equals(OrderID))
                        {
                            int countOrderItem = OrderItem.RemoveOrderItemByOrderID(o.OrderID, trans);
                        }

                        //New Order Items
                        //Product p = Product.Cache.Load(mt.ProductCode);

                        //if (p == null)
                        //    throw new Exception("无相关商品信息，请联系管理员");

                        OrdrItmMatchTicket oi = new OrdrItmMatchTicket();

                        // Genernate Travel Date
                        DateTime _date;
                        if (!string.IsNullOrEmpty(tbTravelDate.Text.Trim()) && DateTime.TryParse(tbTravelDate.Text.Trim(), out _date))
                        {
                            oi.TravelDate = _date;
                        }
                        else
                        {
                            throw new Exception("请正确填写计划出行时间");
                        }

                        // Every Member can only purchase ONE ticket of each match

                        oi.MatchGuid = mt.MatchGuid;

                        oi.OrderID = o.OrderID;
                        oi.Quantity = 1;
                        oi.Sale = null;

                        oi.Place(m, trans);
                    }

                    trans.Commit();

                    //Renew OrderType after Insert OrderItem and transcation commited
                    o.Update();

                    ClientScript.RegisterClientScriptBlock(typeof(string), "succeed", string.Format("alert('订单({0})保存成功');window.location.href = 'iArsenalOrderView_MatchTicket.aspx?OrderID={0}'", o.OrderID.ToString()), true);
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    ClientScript.RegisterClientScriptBlock(typeof(string), "failed", string.Format("alert('{0}')", ex.Message.ToString()), true);
                }

                conn.Close();
            }
        }
    }
}