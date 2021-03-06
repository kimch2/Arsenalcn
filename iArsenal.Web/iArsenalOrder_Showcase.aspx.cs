﻿using System;
using System.Linq;
using System.Web.UI.WebControls;
using iArsenal.Service;

namespace iArsenal.Web
{
    public partial class iArsenalOrder_Showcase : AcnPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //ctrlCustomPagerInfo.PageChanged += ctrlCustomPagerInfo_PageChanged;
            tbProductName.Attributes["placeholder"] = "--商品名称或编号--";
            btnFilter.Text = "<i class=\"fa fa-search\" aria-hidden=\"true\"></i>搜索商品";

            if (!IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            try
            {
                var showcases = Showcase.Cache.ShowcaseList.FindAll(x => x.IsActive);

                // 如果橱窗商品，则直接跳转下单页面
                if (showcases.Count == 0)
                {
                    Response.Redirect("~/iArsenalOrder_ArsenalDirect.aspx");
                }

                var products = Product.Cache.ProductList.FindAll(x => x.IsActive && x.ProductType == ProductType.Other)
                    .FindAll(x =>
                    {
                        var returnValue = true;
                        string tmpString;

                        if (ViewState["ProductName"] != null)
                        {
                            tmpString = ViewState["ProductName"].ToString().ToLower();
                            if (!string.IsNullOrEmpty(tmpString))
                                returnValue = x.Code.ToLower().Contains(tmpString) ||
                                              x.Name.ToLower().Contains(tmpString) ||
                                              x.DisplayName.ToLower().Contains(tmpString);
                        }

                        if (ViewState["IsSale"] != null)
                        {
                            tmpString = ViewState["IsSale"].ToString();
                            if (!string.IsNullOrEmpty(tmpString))
                                returnValue = returnValue && x.Sale.HasValue.Equals(Convert.ToBoolean(tmpString));
                        }

                        return returnValue;
                    });

                var query = from p in products
                            join s in showcases on p.ID equals s.ProductGuid
                            where p.Code == s.ProductCode
                            orderby s.OrderNum
                            select p;

                rptShowcase.DataSource = query;
                rptShowcase.DataBind();

                //#region set Control Custom Pager

                //if (gvMatch.BottomPagerRow != null)
                //{
                //    gvMatch.BottomPagerRow.Visible = true;
                //    ctrlCustomPagerInfo.Visible = true;

                //    ctrlCustomPagerInfo.PageIndex = gvMatch.PageIndex;
                //    ctrlCustomPagerInfo.PageCount = gvMatch.PageCount;
                //    ctrlCustomPagerInfo.RowCount = list.Count;
                //    ctrlCustomPagerInfo.InitComponent();
                //}
                //else
                //{
                //    ctrlCustomPagerInfo.Visible = false;
                //}

                //#endregion
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof(string), "failed", $"alert('{ex.Message}')", true);
            }
        }

        protected void rptShowcase_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                var p = e.Item.DataItem as Product;

                var imgItemThumbnail = e.Item.FindControl("imgItemThumbnail") as Image;
                var ltrlProductName = e.Item.FindControl("ltrlProductName") as Literal;
                var ltrlProductInfo = e.Item.FindControl("ltrlProductInfo") as Literal;
                var ltrlProductPrice = e.Item.FindControl("ltrlProductPrice") as Literal;

                if (p != null && imgItemThumbnail != null && ltrlProductName != null && ltrlProductInfo != null &&
                    ltrlProductPrice != null)
                {
                    imgItemThumbnail.ImageUrl = Request.ApplicationPath + p.ImageUrl;
                    imgItemThumbnail.AlternateText = p.Code;

                    ltrlProductName.Text = $"<h3>{p.Name}</h3>";

                    ltrlProductInfo.Text = $"<h3><em>【{p.Code}】</em>{p.DisplayName}</h3>";

                    ltrlProductPrice.Text = p.Sale.HasValue ?
                        $"<p class=\"item-price\" title=\"{p.Sale.Value}\"><em>{p.SaleInfo}</em><span style=\"text-decoration: line-through\">({p.PriceInfo})</span></p>"
                        : $"<p class=\"item-price\" title=\"{p.Price}\">{p.PriceInfo}</p>";
                }
            }
        }

        protected void ddlIsSale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlIsSale.SelectedValue))
                ViewState["IsSale"] = ddlIsSale.SelectedValue;
            else
                ViewState["IsSale"] = string.Empty;

            BindData();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbProductName.Text.Trim()))
                ViewState["ProductName"] = tbProductName.Text.Trim();
            else
                ViewState["ProductName"] = string.Empty;

            BindData();
        }
    }
}