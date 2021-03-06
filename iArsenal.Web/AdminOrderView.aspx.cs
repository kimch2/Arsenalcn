﻿using System;
using System.Web.UI.WebControls;
using Arsenalcn.Core;
using iArsenal.Service;
using iArsenal.Web.Control;

namespace iArsenal.Web
{
    public partial class AdminOrderView : AdminPageBase
    {
        private readonly IRepository repo = new Repository();

        private int OrderID
        {
            get
            {
                int _OrderID;
                if (!string.IsNullOrEmpty(Request.QueryString["OrderID"]) &&
                    int.TryParse(Request.QueryString["OrderID"], out _OrderID))
                {
                    return _OrderID;
                }
                return int.MinValue;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ctrlAdminFieldToolBar.AdminUserName = Username;
            ctrlCustomPagerInfo.PageChanged += ctrlCustomPagerInfo_PageChanged;

            if (!IsPostBack)
            {
                InitForm();
            }
        }

        private void InitForm()
        {
            if (OrderID > 0)
            {
                var o = repo.Single<Order>(OrderID);

                lblOrderInfo.Text =
                    string.Format(
                        "更新会员的订单 ID:<a href=\"ServerOrderView.ashx?OrderID={0}\" target=\"_blank\"><em>{0}</em></a>",
                        OrderID);

                tbMemberID.Text = o.MemberID.ToString();
                tbMemberName.Text = o.MemberName;
                cbIsActive.Checked = o.IsActive;
                ddlStatus.SelectedValue = ((int)o.Status).ToString();
                tbRate.Text = o.Rate.ToString();
                tbCreateTime.Text = o.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                tbUpdateTime.Text = o.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss");
                tbMobile.Text = o.Mobile;
                tbPayment.Text = o.Payment;
                tbAddress.Text = o.Address;

                if (o.Deposit.HasValue)
                    tbDeposit.Text = o.Deposit.Value.ToString("f2");
                else
                    tbDeposit.Text = string.Empty;

                tbPostage.Text = o.Postage.ToString("f2");
                tbPrice.Text = o.Price.ToString("f2");

                if (o.Sale.HasValue)
                    tbSale.Text = o.Sale.Value.ToString("f2");
                else
                    tbSale.Text = string.Empty;

                tbDescription.Text = o.Description;
                tbRemark.Text = o.Remark;

                // Bind OrderItem data of this Order
                BindItemData();
            }
            else
            {
                lblOrderInfo.Text = "添加会员的订单";
                tbCreateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                tbUpdateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        private void BindItemData()
        {
            var list = repo.Query<OrderItem>(x => x.OrderID == OrderID);

            gvOrderItem.DataSource = list;
            gvOrderItem.DataBind();

            #region set Control Custom Pager

            if (gvOrderItem.BottomPagerRow != null)
            {
                gvOrderItem.BottomPagerRow.Visible = true;
                ctrlCustomPagerInfo.Visible = true;

                ctrlCustomPagerInfo.PageIndex = gvOrderItem.PageIndex;
                ctrlCustomPagerInfo.PageCount = gvOrderItem.PageCount;
                ctrlCustomPagerInfo.RowCount = list.Count;
                ctrlCustomPagerInfo.InitComponent();
            }
            else
            {
                ctrlCustomPagerInfo.Visible = false;
            }

            #endregion
        }

        protected void gvOrderItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOrderItem.PageIndex = e.NewPageIndex;

            BindItemData();
        }

        protected void ctrlCustomPagerInfo_PageChanged(object sender, CustomPagerInfo.DataNavigatorEventArgs e)
        {
            if (e.PageIndex > 0)
            {
                gvOrderItem.PageIndex = e.PageIndex;
            }

            BindItemData();
        }

        protected void gvOrderItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvOrderItem.SelectedIndex != -1)
            {
                var key = gvOrderItem.DataKeys[gvOrderItem.SelectedIndex];
                if (key != null)
                { Response.Redirect($"AdminOrderItemView.aspx?OrderItemID={key.Value}"); }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var o = new Order();

                if (OrderID > 0)
                {
                    o = repo.Single<Order>(OrderID);
                }

                if (!string.IsNullOrEmpty(tbMemberID.Text.Trim()))
                    o.MemberID = Convert.ToInt32(tbMemberID.Text.Trim());
                else
                    throw new Exception("Member ID can't be empty");

                o.MemberName = tbMemberName.Text.Trim();
                o.IsActive = cbIsActive.Checked;
                o.Status = (OrderStatusType)Enum.Parse(typeof(OrderStatusType), ddlStatus.SelectedValue);
                o.Rate = Convert.ToInt16(tbRate.Text.Trim());
                o.CreateTime = DateTime.Parse(tbCreateTime.Text.Trim());
                o.UpdateTime = DateTime.Parse(tbUpdateTime.Text.Trim());
                o.Mobile = tbMobile.Text.Trim();
                o.Payment = tbPayment.Text.Trim();
                o.Address = tbAddress.Text.Trim();

                if (!string.IsNullOrEmpty(tbDeposit.Text.Trim()))
                    o.Deposit = Convert.ToSingle(tbDeposit.Text.Trim());
                else
                    o.Deposit = null;

                o.Postage = Convert.ToSingle(tbPostage.Text.Trim());
                o.Price = Convert.ToSingle(tbPrice.Text.Trim());

                if (!string.IsNullOrEmpty(tbSale.Text.Trim()))
                    o.Sale = Convert.ToSingle(tbSale.Text.Trim());
                else
                    o.Sale = null;

                o.Description = tbDescription.Text.Trim();
                o.Remark = tbRemark.Text.Trim();

                o.RefreshOrderType();

                if (OrderID > 0)
                {
                    repo.Update(o);

                    ClientScript.RegisterClientScriptBlock(typeof(string), "succeed",
                        "alert('更新成功');window.location.href = window.location.href", true);
                }
                else
                {
                    repo.Insert(o);

                    ClientScript.RegisterClientScriptBlock(typeof(string), "succeed",
                        "alert('添加成功');window.location.href = 'AdminOrder.aspx'", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof(string), "failed", $"alert('{ex.Message}')", true);
            }
        }

        protected void btnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                if (OrderID > 0)
                {
                    var o = repo.Single<Order>(OrderID);

                    o.CalcOrderPrice();

                    ClientScript.RegisterClientScriptBlock(typeof(string), "succeed",
                        $"alert('重新计算总价为{o.Price.ToString("f2")}元');window.location.href=window.location.href", true);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof(string), "failed", $"alert('{ex.Message}')", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (OrderID > 0)
            {
                Response.Redirect("AdminOrder.aspx?OrderID=" + OrderID);
            }
            else
            {
                Response.Redirect("AdminOrder.aspx");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (OrderID > 0)
                {
                    var count = repo.Query<OrderItem>(x => x.OrderID == OrderID).Delete();

                    repo.Delete<Order>(OrderID);

                    ClientScript.RegisterClientScriptBlock(typeof(string), "succeed",
                        $"alert('删除成功(包括{count}个许愿)');window.location.href='AdminOrder.aspx'", true);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof(string), "failed", $"alert('{ex.Message}')", true);
            }
        }
    }
}