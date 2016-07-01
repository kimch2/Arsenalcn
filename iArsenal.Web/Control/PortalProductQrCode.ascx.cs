﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace iArsenal.Web.Control
{
    public partial class PortalProductQrCode : System.Web.UI.UserControl
    {
        public string QrCodeUrl { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(QrCodeUrl))
            {
                using (var ms = new MemoryStream())
                {
                    if (GetQrCode(QrCodeUrl, ms))
                    {
                        //Response.ContentType = "image/png";
                        //Response.OutputStream.Write(ms.GetBuffer(), 0, (int)ms.Length);
                        const string fileUrl = "~/UploadFiles/QrCode/";

                        var img = Image.FromStream(ms);

                        var filename = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var path = $"{Server.MapPath(fileUrl)}{filename}.png";

                        img.Save(path);

                        imgQrCode.ImageUrl = $"{fileUrl}{filename}.png";

                        //Response.End();
                    }
                    else
                    {
                        pnlQrCode.Visible = false;
                    }
                }
            }
            else
            {
                pnlQrCode.Visible = false;
            }
        }

        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="content">待编码的字符</param>
        /// <param name="ms">输出流</param>
        ///<returns>True if the encoding succeeded, false if the content is empty or too large to fit in a QR code</returns>

        private static bool GetQrCode(string content, MemoryStream ms)
        {
            const ErrorCorrectionLevel ecl = ErrorCorrectionLevel.M; //误差校正水平 
            const QuietZoneModules quietZones = QuietZoneModules.Two; //空白区域 
            const int moduleSize = 12; //大小

            QrCode qr;

            var encoder = new QrEncoder(ecl);

            if (encoder.TryEncode(content, out qr))//对内容进行编码，并保存生成的矩阵
            {
                var render = new GraphicsRenderer(new FixedModuleSize(moduleSize, quietZones));

                render.WriteToStream(qr.Matrix, ImageFormat.Png, ms);

                return true;
            }

            return false;
        }
    }
}