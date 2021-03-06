﻿using System;
using System.Text;
using Arsenalcn.Core;
using iArsenal.Service;

namespace iArsenal.Web
{
    public partial class iArsenalOrderView_AsiaTrophy2015 : MemberPageBase
    {
        private readonly IRepository repo = new Repository();

        private int OrderID
        {
            get
            {
                int _orderID;
                if (!string.IsNullOrEmpty(Request.QueryString["OrderID"]) &&
                    int.TryParse(Request.QueryString["OrderID"], out _orderID))
                {
                    return _orderID;
                }
                return int.MinValue;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitForm();
            }
        }

        private void InitForm()
        {
            try
            {
                lblMemberName.Text = $"<b>{MemberName}</b> (<em>NO.{Mid}</em>)";

                if (OrderID > 0)
                {
                    var o = (OrdrTravel) Order.Select(OrderID);

                    if (ConfigGlobal.IsPluginAdmin(Uid) && o != null)
                    {
                        lblMemberName.Text = $"<b>{o.MemberName}</b> (<em>NO.{o.MemberID}</em>)";
                    }
                    else
                    {
                        if (o == null || !o.MemberID.Equals(Mid) || !o.IsActive)
                            throw new Exception("此订单无效或非当前用户订单");
                    }

                    #region Bind OrderView Status Workflow

                    if (ucPortalWorkflowInfo != null)
                    {
                        ucPortalWorkflowInfo.JSONOrderStatusList = $"[ {string.Join(",", o.StatusWorkflowInfo)} ]";
                        ucPortalWorkflowInfo.CurrOrderStatus = o.Status;
                    }

                    #endregion

                    var m = repo.Single<Member>(o.MemberID);

                    lblOrderMobile.Text = $"<em>{o.Mobile}</em>";

                    #region Set Member Nation & Region

                    if (!string.IsNullOrEmpty(m.Nation))
                    {
                        if (m.Nation.Equals("中国"))
                        {
                            lblMemberRegion.Text = "中国 ";

                            var region = m.Region.Split('|');
                            var _regionID = int.MinValue;

                            for (var i = 0; i < region.Length; i++)
                            {
                                if (int.TryParse(region[i], out _regionID))
                                {
                                    lblMemberRegion.Text += DictionaryItem.Cache.Load(_regionID).Name + " ";
                                }
                                else
                                    continue;
                            }
                        }
                        else
                        {
                            lblMemberRegion.Text = m.Nation;
                        }
                    }
                    else
                    {
                        lblMemberRegion.Text = "无";
                    }

                    #endregion

                    lblMemberIDCardNo.Text = m.IDCardNo;
                    lblMemberPassportNo.Text = m.PassportNo;
                    lblMemberPassportName.Text = m.PassportName;
                    lblMemberQQ.Text = $"<em>{m.QQ}</em>";
                    lblMemberEmail.Text = $"<em>{m.Email}</em>";

                    lblOrderID.Text = $"<em>{o.ID}</em>";
                    lblOrderCreateTime.Text = o.CreateTime.ToString("yyyy-MM-dd HH:mm");
                    lblOrderDescription.Text = o.Description;

                    if (!string.IsNullOrEmpty(o.Remark))
                    {
                        lblOrderRemark.Text = o.Remark.Replace("\r\n", "<br />");
                        phOrderRemark.Visible = true;
                    }
                    else
                    {
                        phOrderRemark.Visible = false;
                    }

                    // Should be Calculator in this Page
                    var price = default(double);
                    var priceInfo = string.Empty;

                    var oiAsiaTrophy = o.OITravelPlan.MapTo<OrdrItmTravelPlan, OrdrItmTravelPlan2015AsiaTrophy>();
                    oiAsiaTrophy.Init();

                    if (oiAsiaTrophy.IsActive)
                    {
                        // Set Order Match Option
                        var _strMatchInfo = string.Empty;

                        if (oiAsiaTrophy.TravelOption.MatchOption.Equals(MatchOption.First))
                        {
                            _strMatchInfo = "7月15日（周三）比赛";
                        }
                        else if (oiAsiaTrophy.TravelOption.MatchOption.Equals(MatchOption.Second))
                        {
                            _strMatchInfo = "7月18日（周六）比赛";
                        }
                        else
                        {
                            _strMatchInfo = "7月15日（周三）比赛 + 7月18日（周六）比赛";
                        }

                        lblOrderItem_TravelInfo.Text =
                            $"<em>{(oiAsiaTrophy.IsTicketOnly ? "仅购票" : "观赛团")}：</em>{_strMatchInfo}";

                        // Set Order Travel Option
                        var sb = new StringBuilder();

                        if (oiAsiaTrophy.TravelOption.IsVisa)
                        {
                            sb.Append("【代办签证】");
                        }
                        if (oiAsiaTrophy.TravelOption.IsFlight)
                        {
                            sb.Append("【购买机票】");
                        }
                        if (oiAsiaTrophy.TravelOption.IsHotel)
                        {
                            sb.Append("【预订宾馆】");
                        }
                        if (oiAsiaTrophy.TravelOption.IsTraining)
                        {
                            sb.Append("【训练课】");
                        }
                        if (oiAsiaTrophy.TravelOption.IsParty)
                        {
                            sb.Append("【球员见面会】");
                        }
                        ;
                        if (oiAsiaTrophy.TravelOption.IsSingapore)
                        {
                            sb.Append("【当地团】");
                        }
                        ;

                        lblOrderItem_TravelOption.Text = sb.ToString();
                    }
                    else
                    {
                        throw new Exception("此订单未有正确观赛信息");
                    }

                    // Set Travel Partner
                    var listPartner = o.OITravelPartnerList.FindAll(oi =>
                        oi.IsActive && oi.Partner != null);

                    if (listPartner != null && listPartner.Count > 0)
                    {
                        var oiPartner = listPartner[0];
                        var pa = oiPartner.Partner;

                        if (pa != null)
                        {
                            var _strParterRelation = "（{0}）";
                            if (pa.Relation.Equals(1))
                            {
                                _strParterRelation = string.Format(_strParterRelation, "亲属");
                            }
                            else if (pa.Relation.Equals(2))
                            {
                                _strParterRelation = string.Format(_strParterRelation, "朋友");
                            }
                            else
                            {
                                _strParterRelation = string.Empty;
                            }

                            lblOrderItem_TravelPartner.Text = string.Format("<em>{0}</em>{5}，{1}，{2}；护照：（{3}）{4}",
                                pa.Name, pa.Gender ? "男" : "女", pa.IDCardNo, pa.PassportNo, pa.PassportName,
                                _strParterRelation);
                        }

                        phOrderPartner.Visible = true;
                    }
                    else
                    {
                        phOrderPartner.Visible = false;
                    }

                    // Set Travel Price

                    if (listPartner != null && listPartner.Count > 0)
                    {
                        var oiPartner = listPartner[0];

                        price = oiPartner.TotalPrice + oiAsiaTrophy.TotalPrice;
                        priceInfo =
                            $"观赛团费用（如球票、训练课定金等）：{oiAsiaTrophy.TotalPrice.ToString("f0")}+ 同伴1名：{oiPartner.TotalPrice.ToString("f0")} = <em>{price.ToString("f2")}</em>元 (CNY)";

                        phOrderPrice.Visible = true;
                    }
                    else
                    {
                        price = oiAsiaTrophy.TotalPrice;
                        priceInfo = $"观赛团费用（如球票、训练课定金等）：<em>{price.ToString("f2")}</em>元 (CNY)";

                        phOrderPrice.Visible = true;
                    }

                    tbOrderPrice.Text = price.ToString();
                    lblOrderPrice.Text = priceInfo;

                    if (o.Status.Equals(OrderStatusType.Draft))
                    {
                        btnSubmit.Visible = true;
                        btnModify.Visible = true;
                        btnCancel.Visible = true;

                        phOrderPrice.Visible = false;
                    }
                    else if (o.Status.Equals(OrderStatusType.Submitted))
                    {
                        btnSubmit.Visible = false;
                        btnModify.Visible = false;
                        btnCancel.Visible = true;

                        phOrderPrice.Visible = false;
                    }
                    else
                    {
                        btnSubmit.Visible = false;
                        btnModify.Visible = false;
                        btnCancel.Visible = false;

                        phOrderPrice.Visible = true;
                    }
                }
                else
                {
                    throw new Exception("此订单不存在");
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof (string), "failed",
                    $"alert('{ex.Message}');window.location.href = 'iArsenalOrder.aspx'", true);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (OrderID > 0)
                {
                    var o = repo.Single<Order>(OrderID);

                    if (o == null || !o.MemberID.Equals(Mid) || !o.IsActive)
                        throw new Exception("此订单无效或非当前用户订单");

                    o.Status = OrderStatusType.Submitted;
                    o.UpdateTime = DateTime.Now;
                    o.Price = Convert.ToSingle(tbOrderPrice.Text.Trim());

                    repo.Update(o);

                    ClientScript.RegisterClientScriptBlock(typeof (string), "succeed",
                        $"alert('谢谢您的预订报名，您的订单已经提交成功。\\r\\n请耐心等待审核，并由观赛组织人会与您联系。\\r\\n订单号为：{o.ID}'); window.location.href = window.location.href",
                        true);
                }
                else
                {
                    throw new Exception("此订单无效或非当前用户订单");
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof (string), "failed", $"alert('{ex.Message}');", true);
            }
        }

        protected void btnModify_Click(object sender, EventArgs e)
        {
            try
            {
                if (OrderID > 0)
                {
                    var o = repo.Single<Order>(OrderID);

                    if (o == null || !o.MemberID.Equals(Mid) || !o.IsActive)
                        throw new Exception("此订单无效或非当前用户订单");

                    ClientScript.RegisterClientScriptBlock(typeof (string), "succeed",
                        $"window.location.href = 'iArsenalOrder_AsiaTrophy2015.aspx?OrderID={o.ID}'", true);
                }
                else
                {
                    throw new Exception("此订单无效或非当前用户订单");
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof (string), "failed", $"alert('{ex.Message}');", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (OrderID > 0)
                {
                    var o = repo.Single<Order>(OrderID);

                    if (o == null || !o.MemberID.Equals(Mid) || !o.IsActive)
                        throw new Exception("此订单无效或非当前用户订单");

                    o.IsActive = false;
                    o.UpdateTime = DateTime.Now;
                    o.Price = Convert.ToSingle(tbOrderPrice.Text.Trim());

                    repo.Update(o);

                    ClientScript.RegisterClientScriptBlock(typeof (string), "succeed",
                        $"alert('此订单({o.ID})已经取消');window.location.href = 'iArsenalOrder.aspx'", true);
                }
                else
                {
                    throw new Exception("此订单无效或非当前用户订单");
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof (string), "failed", $"alert('{ex.Message}');", true);
            }
        }
    }
}