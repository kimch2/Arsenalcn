﻿<%@ Page Language="C#" MasterPageFile="iArsenalMaster.Master" AutoEventWireup="true"
    CodeBehind="iArsenalOrderView_AsiaTrophy2015.aspx.cs" Inherits="iArsenal.Web.iArsenalOrderView_AsiaTrophy2015"
    Title="订单信息查看" Theme="iArsenal" %>

<%@ Register Src="Control/PortalSitePath.ascx" TagName="PortalSitePath" TagPrefix="uc1" %>
<%@ Register Src="Control/PortalWorkflowInfo.ascx" TagName="PortalWorkflowInfo" TagPrefix="uc2" %>
<%@ Register Src="Control/PortalPaymentInfo.ascx" TagName="PortalPaymentInfo" TagPrefix="uc3" %>
<asp:Content ID="cphHead" ContentPlaceHolderID="cphHead" runat="server">
    <style type="text/css">
        .OrderPrice {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="cphMain" ContentPlaceHolderID="cphMain" runat="server">
    <div id="ACN_Main">
        <uc1:PortalSitePath ID="ucPortalSitePath" runat="server" />
        <div id="mainPanel">
            <uc2:PortalWorkflowInfo ID="ucPortalWorkflowInfo" runat="server" />
            <table class="DataView">
                <thead>
                    <tr class="Header">
                        <th colspan="4" class="FieldColumn">
                            <a name="anchorBack" id="anchorBack">感谢您报名参加本次观赛活动，请仔细确认并提交预订信息：</a>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr class="CommandRow">
                        <td colspan="4">-- 请核对您的会员信息 --
                        </td>
                    </tr>
                    <tr class="Row">
                        <td class="FieldHeader" style="width: 110px">真实姓名：
                        </td>
                        <td class="FieldColumn">
                            <asp:Label ID="lblMemberName" runat="server"></asp:Label>
                        </td>
                        <td class="FieldHeader" style="width: 110px">手机：
                        </td>
                        <td class="FieldColumn">
                            <asp:Label ID="lblOrderMobile" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="AlternatingRow">
                        <td class="FieldHeader">现居住地：
                        </td>
                        <td class="FieldColumn" colspan="3">
                            <asp:Label ID="lblMemberRegion" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="Row">
                        <td class="FieldHeader">身份证号：
                        </td>
                        <td class="FieldColumn" colspan="3">
                            <asp:Label ID="lblMemberIDCardNo" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="AlternatingRow">
                        <td class="FieldHeader">护照号码：
                        </td>
                        <td class="FieldColumn">
                            <asp:Label ID="lblMemberPassportNo" runat="server"></asp:Label>
                        </td>
                        <td class="FieldHeader">护照姓名：
                        </td>
                        <td class="FieldColumn">
                            <asp:Label ID="lblMemberPassportName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="Row">
                        <td class="FieldHeader">微信/QQ：
                        </td>
                        <td class="FieldColumn">
                            <asp:Label ID="lblMemberQQ" runat="server"></asp:Label>
                        </td>
                        <td class="FieldHeader">邮箱：
                        </td>
                        <td class="FieldColumn">
                            <asp:Label ID="lblMemberEmail" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="CommandRow">
                        <td colspan="4">-- 请核对您的旅行信息 --
                        </td>
                    </tr>
                    <tr class="AlternatingRow">
                        <td class="FieldHeader">订单号：
                        </td>
                        <td class="FieldColumn">
                            <asp:Label ID="lblOrderID" runat="server"></asp:Label>
                        </td>
                        <td class="FieldHeader">创建时间：
                        </td>
                        <td class="FieldColumn">
                            <asp:Label ID="lblOrderCreateTime" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="Row">
                        <td class="FieldHeader">出行选项：
                        </td>
                        <td class="FieldColumn" colspan="3">
                            <asp:Label ID="lblOrderItem_TravelInfo" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="phOrderPartner" runat="server" Visible="false">
                        <tr class="AlternatingRow">
                            <td class="FieldHeader">同伴信息：
                            </td>
                            <td class="FieldColumn" colspan="3">
                                <asp:Label ID="lblOrderItem_TravelPartner" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <tr class="Row">
                        <td class="FieldHeader">观赛选项：
                        </td>
                        <td class="FieldColumn" colspan="3">
                            <asp:Label ID="lblOrderItem_TravelOption" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="AlternatingRow">
                        <td class="FieldHeader">备注：
                        </td>
                        <td class="FieldColumn" colspan="3">
                            <asp:Label ID="lblOrderDescription" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="phOrderRemark" runat="server" Visible="false">
                        <tr class="CommandRow">
                            <td colspan="4">-- 请留意给您的反馈信息 --
                            </td>
                        </tr>
                        <tr class="AlternatingRow">
                            <td class="FieldHeader">订单反馈：
                            </td>
                            <td class="FieldColumn" colspan="3">
                                <asp:Label ID="lblOrderRemark" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phOrderPrice" runat="server" Visible="false">
                        <tr class="CommandRow">
                            <td colspan="4" style="text-align: right; white-space: nowrap; line-height: 1.8;">
                                <asp:Label ID="lblOrderPrice" runat="server" CssClass="OrderTotalPrice" Text="? 元 (CNY)"></asp:Label>
                                <asp:TextBox ID="tbOrderPrice" runat="server" CssClass="OrderPrice"></asp:TextBox>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </tbody>
            </table>
            <div class="FooterBtnBar">
                <asp:Button ID="btnSubmit" runat="server" Text="确认并提交" CssClass="InputBtn SubmitBtn" OnClick="btnSubmit_Click"
                    OnClientClick="return confirm('确定提交此订单?(提交后无法修改)')" />
                <asp:Button ID="btnModify" runat="server" Text="修改" CssClass="InputBtn" OnClick="btnModify_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="取消" CssClass="InputBtn" OnClick="btnCancel_Click"
                    OnClientClick="return confirm('取消此订单?(取消后无法恢复)')" />
                <input id="btnPrint" type="button" value="打印" class="InputBtn" onclick="window.print()" />
                <input id="btnBack" type="button" value="返回列表" class="InputBtn" onclick="window.location.href = 'iArsenalOrder.aspx'" />
            </div>
        </div>
        <div id="rightPanel">
            <uc3:PortalPaymentInfo ID="ucPortalPaymentInfo" runat="server" />
            <div class="InfoPanel">
                <h3 class="Col" onclick="$(this).toggleClass('Col'); $(this).toggleClass('Exp'); $(this).next('div').toggle('normal');">
                    <a>审核确认须知</a></h3>
                <div class="Block">
                    <p>(1). 我们会通过微博、微信：<em>iArsenalcn</em>，进行联系通知，请有意者关注或加入时，注明新加坡观赛与真实姓名即可。</p>
                    <p>(2). 请各位确定自己的行程安排与参与活动项目的信息。</p>
                    <p>(3). 左侧计算出了各位的球票、训练课门票定金费用，分项价格已通过确认函的邮件发给各位。请根据上方的付款方式，在<em>6月15日</em>前转帐付款。</p>
                    <p>(4). <em>6月15日</em>前付款确认的会员可确保比赛门票，并获得优先分配训练课门票的机会。如训练课不对外公开，我们将全额退还定金。</p>
                    <p>(5). 预计将会7月初确定球票领取方式、球迷会联谊活动、赛前训练课和球员见面会（如有）的所有活动细节。</p>
                </div>
            </div>
            <div class="InfoPanel" style="display: none">
                <h3 class="Col" onclick="$(this).toggleClass('Col'); $(this).toggleClass('Exp'); $(this).next('div').toggle('normal');">
                    <a>其他补充说明</a></h3>
                <div class="Block">
                    <p>(1). 所有确认参团者，如有意外情况，需要临时退团，原则上我们会返还定金，但由于时间过晚，导致球票退票或其他问题造成的费用损失，可能会收取一些费用。</p>
                    <p>(2). 10月初，我们会通过网络QQ群，召开出行相关问答沟通会，并公布详细出行日程安排。</p>
                    <p style="font-weight: bold;">(3). 本次伦敦行观赛团属于民间组团游，从本质上还是属于自由行，球迷会将尽力为各位解决签证、机票、住宿方面的问题，但仍需参团者自行保证人身安全、财产安全等事宜。球迷会可能会在10月中下旬出行前，通过邮寄或快递方式，请参团者签署相关免责协议，敬请理解。</p>
                </div>
            </div>
        </div>
        <div class="Clear">
        </div>
    </div>
</asp:Content>
