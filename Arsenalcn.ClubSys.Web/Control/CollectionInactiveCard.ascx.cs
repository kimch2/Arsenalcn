﻿using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Arsenalcn.ClubSys.Service;
using Arsenalcn.ClubSys.Entity;

namespace Arsenalcn.ClubSys.Web.Control
{
    public partial class CollectionInactiveCard : Common.CollectionBase
    {
        private Gamer _playerInfo = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ProfileUserID > 0)
            {
                _playerInfo = PlayerStrip.GetPlayerInfo(ProfileUserID);

                var items = PlayerStrip.GetMyNumbers(ProfileUserID);
                items.RemoveAll(delegate(Card un) { return un.IsActive; });
                items.RemoveAll(delegate(Card un) { return !un.ArsenalPlayerGuid.HasValue; });

                rptCard.DataSource = items;
                rptCard.DataBind();
            }
        }
        protected void rptCard_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                var un = e.Item.DataItem as Card;

                var lblPlayerInactiveCardID = e.Item.FindControl("lblPlayerInactiveCardID") as Label;
                var lblPlayerInactiveCardPath = e.Item.FindControl("lblPlayerInactiveCardPath") as Label;
                var btnActive = e.Item.FindControl("btnActive") as LinkButton;

                lblPlayerInactiveCardID.Text = un.ID.ToString();
                lblPlayerInactiveCardPath.Text =
                    $"swf/PlayerCard.swf?XMLURL=ServerXml.aspx%3FPlayerGuid={un.ArsenalPlayerGuid.ToString()}";

                if (ProfileUserID == CurrentUserID)
                {
                    //active button
                    if (_playerInfo == null)
                    {
                        btnActive.Visible = false;
                    }
                    else
                    {
                        if (_playerInfo.Shirt >= 5 && _playerInfo.Shorts >= 5 && _playerInfo.Sock >= 5)
                        {
                            btnActive.Visible = true;

                            //btnActive.CommandArgument = un.ID.ToString();

                            //postback to another url
                            //btnActive.PostBackUrl = "MyPlayerCardActive.aspx?unID=" + un.ID.ToString();
                            btnActive.OnClientClick = "GenFlashFrame('swf/ShowCardActive.swf?XMLURL=ServerXml.aspx%3FCardID=" + un.ID.ToString() + "', '160', '200', true); return false";
                        }
                        else
                        {
                            btnActive.Visible = false;
                        }
                    }

                }
                else
                {
                    btnActive.Visible = false;
                }
            }
        }
    }
}