﻿using System;
using System.Linq;

using Arsenalcn.Core;

namespace iArsenal.Service
{
    public class OrdrMembership : Order
    {
        public OrdrMembership() { }

        public void Init()
        {
            IRepository repo = new Repository();

            var list = repo.Query<OrderItem>(x => x.OrderID.Equals(ID) && x.IsActive && Product.Cache.Load(x.ProductGuid) != null).ToList();

            if (list != null && list.Count > 0)
            {
                OrderItem oiBase = null;

                oiBase = list.Find(x => Product.Cache.Load(x.ProductGuid).ProductType.Equals(ProductType.MemberShipCore));
                if (oiBase != null)
                {
                    AutoMapper.Mapper.CreateMap<OrderItem, OrdrItmMemShipCore>().AfterMap((s, d) => d.Init());
                    OIMemberShipCore = AutoMapper.Mapper.Map<OrdrItmMemShipCore>(oiBase);
                }

                oiBase = list.Find(x => Product.Cache.Load(x.ProductGuid).ProductType.Equals(ProductType.MemberShipPremier));
                if (oiBase != null)
                {
                    AutoMapper.Mapper.CreateMap<OrderItem, OrdrItmMemShipPremier>().AfterMap((s, d) => d.Init());
                    OIMemberShipPremier = AutoMapper.Mapper.Map<OrdrItmMemShipPremier>(oiBase);
                }

                if (OIMemberShipCore != null || OIMemberShipPremier != null)
                {
                    base.UrlOrderView = "iArsenalOrderView_MemberShip.aspx";
                }
                else
                {
                    throw new Exception("Unable to init Order_MemberShip.");
                }
            }

            #region Order Status Workflow Info

            string _strWorkflow = "{{ \"StatusType\": \"{0}\", \"StatusInfo\": \"{1}\" }}";

            string[] _workflowInfo = {
                                      string.Format(_strWorkflow, ((int)OrderStatusType.Draft).ToString(), "未提交"),
                                      string.Format(_strWorkflow, ((int)OrderStatusType.Submitted).ToString(), "审核中"), 
                                      string.Format(_strWorkflow, ((int)OrderStatusType.Confirmed).ToString(), "已确认")
                                  };

            base.StatusWorkflowInfo = _workflowInfo;

            #endregion
        }

        #region Members and Properties

        public OrdrItmMemShipCore OIMemberShipCore { get; set; }

        public OrdrItmMemShipPremier OIMemberShipPremier { get; set; }

        #endregion

    }
}
