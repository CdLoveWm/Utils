using SkiaSharp;
using SkiaSharp.QrCode;
using System;
using System.IO;

namespace Utils.QrCode
{
    /// <summary>
    /// 跨平台生成二维码工具类
    /// </summary>
    public class QrCodeUtil
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <param name="qrCodeSrc">二维码图片地址</param>
        public static void GenerateQrCode(string content, string qrCodeSrc)
        {
            //创建生成器
            using (var generator = new QRCodeGenerator())
            {
                // 设置错误校正能力（ECC）级别
                var qr = generator.CreateQrCode(content, ECCLevel.H);

                // 创建一个Canvas
                var info = new SKImageInfo(512, 512);
                using (var surface = SKSurface.Create(info))
                {
                    var canvas = surface.Canvas;

                    // 渲染二维码到Canvas
                    canvas.Render(qr, info.Width, info.Height);

                    // 输出到文件。SKEncodedImageFormat.Png可以指定二维码图片格式
                    using (var image = surface.Snapshot())
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var stream = File.OpenWrite(qrCodeSrc))
                    {
                        data.SaveTo(stream);
                    }
                }
            }
        }
    }
}
