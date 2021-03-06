﻿using System;
using System.Web;
using System.Web.Script.Serialization;
using Arsenalcn.Core;
using iArsenal.Service;

namespace iArsenal.Web
{
    public class ServerBulkOrder : IHttpHandler
    {
        private readonly IRepository _repo = new Repository();

        public void ProcessRequest(HttpContext context)
        {
            var responseText = string.Empty;

            if (!string.IsNullOrEmpty(context.Request.QueryString["SelectedOrderIDs"]))
            {
                try
                {
                    var arrayOrderIDs = context.Request.QueryString["SelectedOrderIDs"].Split('|');

                    var countSucceed = 0;
                    var countFailed = 0;

                    if (arrayOrderIDs.Length > 0)
                    {
                        foreach (var strID in arrayOrderIDs)
                        {
                            int _id;
                            if (int.TryParse(strID, out _id))
                            {
                                try
                                {
                                    var o = _repo.Single<Order>(_id);

                                    if (o != null && o.Status.Equals(OrderStatusType.Confirmed))
                                    {
                                        o.Status = OrderStatusType.Ordered;
                                        o.UpdateTime = DateTime.Now;

                                        _repo.Update(o);

                                        countSucceed++;
                                    }
                                    else
                                    {
                                        countFailed++;
                                    }
                                }
                                catch
                                {
                                    countFailed++;
                                }
                            }
                            else
                            {
                                countFailed++;
                            }
                        }

                        var returnObj = new
                        {
                            result = "success",
                            countTotal = arrayOrderIDs.Length,
                            countSucceed,
                            countFailed
                        };

                        var jsonSerializer = new JavaScriptSerializer();
                        responseText = jsonSerializer.Serialize(returnObj);
                    }
                    else
                    {
                        throw new Exception("invalid Selected OrderIDs");
                    }
                }
                catch (Exception ex)
                {
                    responseText = $"{{  \"result\": \"error\", \"error_msg\": \"{ex.Message}\" }}";
                }
            }

            //responseText = "{  \"result\": \"success\", \"countTotal\": \"10\", \"countSucceed\": \"8\", \"countFailed\": \"2\" }";

            context.Response.Clear();
            context.Response.ContentType = "text/plain";
            context.Response.Write(responseText);
            context.Response.End();
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}