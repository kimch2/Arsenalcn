﻿using System;
using System.Collections.Generic;

namespace iArsenal.Entity
{
    public class Order_Ticket : OrderBase
    {
        public Order_Ticket() { }

        public Order_Ticket(int id)
            : base(id)
        {
            List<OrderItemBase> oiList = OrderItemBase.GetOrderItems(id).FindAll(oi => oi.IsActive && Product.Cache.Load(oi.ProductGuid) != null);

            if (oiList != null && oiList.Count > 0)
            {
                OrderItemBase oiBase = null;

                oiBase = oiList.Find(oi => Product.Cache.Load(oi.ProductGuid).ProductType.Equals(ProductType.MatchTicket));
                if (oiBase != null) { OIMatchTicket = new OrderItem_MatchTicket(oiBase.OrderItemID); }

                oiBase = oiList.Find(oi => Product.Cache.Load(oi.ProductGuid).ProductType.Equals(ProductType.TicketBeijing));
                if (oiBase != null) { OITicketBeijing = new OrderItem_TicketBeijing(oiBase.OrderItemID); }

                if (OIMatchTicket != null)
                {
                    base.URLOrderView = "iArsenalOrderView_MatchTicket.aspx";
                }
                else if (OITicketBeijing != null)
                {
                    base.URLOrderView = "iArsenalOrderView_TicketBeijing.aspx";
                }
                else
                {
                    throw new Exception("Unable to init Order_Ticket.");
                }
            }

            #region Order Status Workflow Info

            string _strWorkflow = "{{ \"StatusType\": \"{0}\", \"StatusInfo\": \"{1}\" }}";

            string[] _workflowInfo = {
                                      string.Format(_strWorkflow, ((int)OrderStatusType.Draft).ToString(), "未提交"),
                                      string.Format(_strWorkflow, ((int)OrderStatusType.Submitted).ToString(), "审核中"), 
                                      string.Format(_strWorkflow, ((int)OrderStatusType.Confirmed).ToString(), "已付款"), 
                                      string.Format(_strWorkflow, ((int)OrderStatusType.Ordered).ToString(), "已下单"), 
                                      string.Format(_strWorkflow, ((int)OrderStatusType.Delivered).ToString(), "已出票")
                                  };

            base.StatusWorkflowInfo = _workflowInfo;

            #endregion

        }

        #region Members and Properties

        public OrderItem_MatchTicket OIMatchTicket { get; set; }

        public OrderItem_TicketBeijing OITicketBeijing { get; set; }

        #endregion

    }
}
