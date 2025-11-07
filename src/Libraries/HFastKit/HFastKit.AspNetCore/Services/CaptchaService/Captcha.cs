using HFastKit.Extensions;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace HFastKit.AspNetCore.Services.Captcha
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class Captcha
    {
        /// <summary>
        /// Guid（唯一标识）
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// 验证码文本
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpirationTime { get; }

        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="codeText">验证码文本</param>
        /// <param name="expirationTime">过期时间</param>
        public Captcha(string codeText, TimeSpan expirationTime)
        {
            Guid = Guid.NewGuid();
            Text = codeText;
            ExpirationTime = DateTime.UtcNow + expirationTime;
            Token = $"{Text}|{Guid}|{ExpirationTime}".DesEncrypt();
        }

        /// <summary>
        /// 生成 Png 验证码图片
        /// </summary>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <param name="isDrawLines">是否画干扰线</param>
        /// <param name="isDrawPoints">是否画噪点</param>
        /// <returns></returns>
        public byte[] GenerateImageAsPng(int width = 100, int height = 40, bool isDrawLines = true, bool isDrawPoints = true)
        {
            using Image<Rgba32> image = new(width, height);
            image.Mutate(context =>
            {
                // 白底背景
                context.Fill(Color.White);

                var charWidth = (image.Width / Text.Length);                // 单个字符宽度
                var baseCharSize = Math.Min(charWidth, image.Height);           // 基础字符大小
                var fontMinSize = (int)(baseCharSize * 0.9);                    // 字体最小大小
                var fontMaxSize = (int)(baseCharSize * 1.3);                    // 字体最大大小
                Array fontStyleArray = Enum.GetValues(typeof(FontStyle));       // 字体风格

                // 画验证码
                for (int i = 0; i < Text.Length; i++)
                {
                    FontStyle style = (FontStyle)(fontStyleArray.GetValue(Random.Shared.Next(fontStyleArray.Length)) ?? FontStyle.Regular);
                    int fontSize = Random.Shared.Next(fontMinSize, fontMaxSize);
                    Font font = SystemFonts.CreateFont("arial", fontSize, FontStyle.Bold);
                    var pointF = new Point(i * charWidth, (image.Height - baseCharSize) / 2);
                    context.DrawText(Text[i].ToString(), font, Color.FromRgba(62, 44, 221, 200), pointF);
                }

                // 画干扰线
                if (isDrawLines)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        Rgba32 penColor = new Rgba32((uint)Random.Shared.Next(int.MinValue, int.MaxValue));
                        int penWidth = Random.Shared.Next(1, 3);
                        var pen = Pens.DashDot(penColor, penWidth);
                        var p1 = new PointF(Random.Shared.Next(width), Random.Shared.Next(height));
                        var p2 = new PointF(Random.Shared.Next(width), Random.Shared.Next(height));
                        while (p1 == p2)
                        {
                            p2.X = Random.Shared.Next(width);
                            p2.Y = Random.Shared.Next(height);
                        }
                        context.DrawLine(pen, p1, p2);
                    }
                }

                // 画噪点
                if (isDrawPoints)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Rgba32 penColor = new Rgba32((uint)Random.Shared.Next(int.MinValue, int.MaxValue));
                        var pen = Pens.DashDot(penColor, 1);
                        var p1 = new PointF(Random.Shared.Next(width), Random.Shared.Next(height));
                        var p2 = new PointF(p1.X + 1.0f, p1.Y + 1.0f);
                        context.DrawLine(pen, p1, p2);
                    }
                }
            });
            using var memoryStream = new MemoryStream();
            image.SaveAsPng(memoryStream);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// 反序列化证码
        /// </summary>
        /// <param name="captchaToken">验证码Token</param>
        /// <param name="captcha">验证码对象</param>
        /// <returns></returns>
        public static bool TryDeserialize(string captchaToken, out Captcha? captcha)
        {
            captcha = null;
            string token = captchaToken.DesDecrypt();
            List<string>? tokenList = token.Split('|')?.ToList();
            if (tokenList is null || tokenList.Count != 3)
            {
                return false;
            }
            if (!Guid.TryParse(tokenList[1], out Guid guid))
            {
                return false;
            }
            if (!DateTime.TryParse(tokenList[2], out DateTime expirationTime))
            {
                return false;
            }
            if (expirationTime < DateTime.UtcNow)
            {
                return false;
            }
            TimeSpan timeSpan = expirationTime - DateTime.UtcNow;
            captcha = new Captcha(tokenList[0], timeSpan);
            return true;
        }
    }
}