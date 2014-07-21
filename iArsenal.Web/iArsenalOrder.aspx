﻿<%@ Page Language="C#" MasterPageFile="iArsenalMaster.Master" AutoEventWireup="true"
    CodeBehind="iArsenalOrder.aspx.cs" Inherits="iArsenal.Web.iArsenalOrder" Title="订单查询"
    Theme="iArsenal" %>

<%@ Register Src="Control/PortalSitePath.ascx" TagName="PortalSitePath" TagPrefix="uc1" %>
<%@ Register Src="Control/CustomPagerInfo.ascx" TagName="CustomPagerInfo" TagPrefix="uc3" %>
<asp:Content ID="cphHead" ContentPlaceHolderID="cphHead" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            var $tbInfo = $(".DivFloatLeft > .TextBox");
            $tbInfo.each(function () {
                $(this).focus(function () {
                    $(this).val("");
                });
            });
        });
    </script>
</asp:Content>
<asp:Content ID="cphMain" ContentPlaceHolderID="cphMain" runat="server">
    <div id="ACN_Main">
        <uc1:PortalSitePath ID="ucPortalSitePath" runat="server" />
        <div id="mainPanel">
            <div class="FunctionBar">
                <div class="DivFloatLeft">
                    订单查询：
                    <asp:TextBox ID="tbOrderID" runat="server" CssClass="TextBox" Width="200px"></asp:TextBox>
                    <asp:LinkButton ID="btnFilter" runat="server" Text="搜索订单" CssClass="LinkBtn" OnClick="btnFilter_Click"></asp:LinkButton>
                </div>
                <div class="DivFloatRight">
                </div>
                <div class="Clear">
                    <uc3:CustomPagerInfo ID="ctrlCustomPagerInfo" runat="server" />
                </div>
            </div>
            <asp:GridView ID="gvOrder" runat="server" DataKeyNames="OrderID" OnPageIndexChanging="gvOrder_PageIndexChanging"
                PageSize="10" OnSelectedIndexChanged="gvOrder_SelectedIndexChanged" OnRowDataBound="gvOrder_RowDataBound">
                <Columns>
                    <asp:BoundField HeaderText="编号" DataField="OrderID" />
                    <asp:BoundField HeaderText="创建时间" DataField="CreateTime" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <asp:BoundField HeaderText="定金" DataField="Deposit" NullDisplayText="/" DataFormatString="{0:f2}"
                        ItemStyle-HorizontalAlign="Right" Visible="false" />
                    <asp:BoundField HeaderText="类型" DataField="OrderType" />
                    <asp:TemplateField HeaderText="价格" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:Label ID="lblPriceInfo" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="状态">
                        <ItemTemplate>
                            <asp:Label ID="lblOrderStatus" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowSelectButton="true" HeaderText="操作" EditText="修改" SelectText="详细"
                        UpdateText="保存" CancelText="取消" DeleteText="删除" ControlStyle-CssClass="LinkBtn" />
                </Columns>
            </asp:GridView>
        </div>
        <div id="rightPanel">
            <div class="InfoPanel">
                <h3 class="Col" onclick="$(this).toggleClass('Col'); $(this).toggleClass('Exp'); $(this).next('div').toggle('normal');">
                    <a>会员中心</a></h3>
                <div class="Block">
                    <ul>
                        <li><a href="iArsenalMemberRegister.aspx">会员信息</a></li>
                        <li><a href="iArsenalMemberPeriod.aspx">会籍查询</a></li>
                        <li><a href="iArsenalOrder.aspx">订单查询</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="Clear">
        </div>
    </div>
</asp:Content>
