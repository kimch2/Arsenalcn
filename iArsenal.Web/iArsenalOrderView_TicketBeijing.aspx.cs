﻿using System;
using Arsenalcn.Core;
using iArsenal.Service;

namespace iArsenal.Web
{
    public partial class iArsenalOrderView_TicketBeijing : MemberPageBase
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
                    var o = (OrdrTicket) Order.Select(OrderID);

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

                    if (m == null || !m.IsActive)
                        throw new Exception("无此会员信息");

                    lblMemberIDCardNo.Text = $"<em>{m.IDCardNo}</em>";
                    lblMemberEmail.Text = $"<em>{m.Email}</em>";
                    lblMemberRegion.Text = m.RegionInfo;
                    lblOrderMobile.Text = $"<em>{o.Mobile}</em>";
                    lblOrderPayment.Text = o.PaymentInfo;
                    lblOrderDescription.Text = o.Description;
                    lblOrderID.Text = $"<em>{o.ID}</em>";
                    lblOrderCreateTime.Text = o.CreateTime.ToString("yyyy-MM-dd HH:mm");

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

                    var oiTicket = o.OITicketBeijing;
                    if (oiTicket != null && oiTicket.IsActive)
                    {
                        lblOrderItem_TicketBeijing.Text = $"<em>{oiTicket.ProductName}</em>";
                        tbOrderItem_TicketBeijing.Text = oiTicket.ProductGuid.ToString();
                        lblOrderItemQuantity.Text = oiTicket.Quantity.ToString();

                        if (oiTicket.Size.Equals("1"))
                            lblOrderItemSize.Text = "一层看台";
                        else if (oiTicket.Size.Equals("2"))
                            lblOrderItemSize.Text = "二层看台";
                        else
                            lblOrderItemSize.Text = "不介意";

                        lblOrderItemRemak.Text = oiTicket.SeatLevel;
                    }
                    else
                    {
                        throw new Exception("此订单未购买球票商品");
                    }

                    // Set Order Price

                    price = oiTicket.TotalPrice;
                    priceInfo = $"<合计> {oiTicket.UnitPrice.ToString("f2")} × {oiTicket.Quantity}";

                    if (!o.Sale.HasValue)
                        lblOrderPrice.Text = $"{priceInfo} = <em>{price.ToString("f2")}</em>元 (CNY)";
                    else
                        lblOrderPrice.Text =
                            $"{priceInfo} = <em>{price.ToString("f2")}</em>元<br /><结算价>：<em>{o.Sale.Value.ToString("f2")}</em>元 (CNY)";

                    tbOrderPrice.Text = price.ToString();

                    if (o.Status.Equals(OrderStatusType.Draft))
                    {
                        btnSubmit.Visible = true;
                        btnModify.Visible = true;
                        btnCancel.Visible = true;
                    }
                    else if (o.Status.Equals(OrderStatusType.Submitted))
                    {
                        btnSubmit.Visible = false;
                        btnModify.Visible = false;
                        btnCancel.Visible = true;
                    }
                    else
                    {
                        btnSubmit.Visible = false;
                        btnModify.Visible = false;
                        btnCancel.Visible = false;
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
                        $"alert('谢谢您的订购，您的订单已经提交成功。\\r\\n请尽快付款以完成订单确认，订单号为：{o.ID}'); window.location.href = window.location.href",
                        true);
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
                        $"window.location.href = 'iArsenalOrder_TicketBeijing.aspx?OrderID={o.ID}'", true);
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
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof (string), "failed", $"alert('{ex.Message}');", true);
            }
        }
    }
}