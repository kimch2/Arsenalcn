﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="iArsenalHeader.ascx.cs"
    Inherits="iArsenal.Web.Control.iArsenalHeader" %>
<div id="header">
    <h1>
        <a href="http://www.arsenalcn.com/">
            <img src="uploadfiles/logo_ArsenalChina.png" alt="阿森纳中国官方球迷会" />
        </a>
        <a href="#">
            <img src="uploadfiles/qrcode-arsenalcn.gif" alt="OfficialArsenalCN" />
        </a>
    </h1>
    <div id="userPanel">
        <h2 onclick="window.location.href = 'default.aspx'"></h2>
        <asp:Panel ID="pnlLoginUser" CssClass="UserInfo" runat="server">
            <asp:Label ID="lblUserInfo" runat="server"></asp:Label>
            -
            <a href="http://bbs.arsenalcn.com/usercpinbox.aspx" target="_blank">短消息</a> -
            <a href="iArsenalMemberRegister.aspx">会员中心</a> -
            <asp:Literal ID="ltrlAdminConfig" runat="server"></asp:Literal>
            <a href="default.aspx?method=logout">退出</a>
        </asp:Panel>
        <asp:Panel ID="pnlAnonymousUser" CssClass="UserInfo" runat="server">
            <asp:HyperLink ID="hlLogin" runat="server" CssClass="BtnLogin">登录</asp:HyperLink>
            - <a href="http://bbs.arsenalcn.com/register.aspx">注册</a> - <a href="#">帮助中心</a>
        </asp:Panel>
    </div>
</div>
<div id="menubar">
    <ul>
        <li class="nol">
            <a href="default.aspx">首页</a>
        </li>
        <li>
            <a href="iArsenalOrder_ReplicaKit.aspx?Type=Home">1617主场球衣</a>
        </li>
        <li>
            <a href="iArsenalOrder_ReplicaKit.aspx?Type=Away">1617客场球衣</a>
        </li>
        <li>
            <a href="iArsenalOrder_ReplicaKit.aspx?Type=Cup">1617杯赛球衣</a>
        </li>
        <li>
            <a href="iArsenalOrder_Printing.aspx">个性化印字印号</a>
        </li>
        <li>
            <a href="iArsenalOrder_MatchList.aspx">主场球票预订</a>
        </li>
        <li>
            <a href="iArsenalOrder_Showcase.aspx">官方纪念品团购</a>
        </li>
        <li class="nor">
            <a href="iArsenalOrder.aspx">订单查询</a>
        </li>
    </ul>
</div>
