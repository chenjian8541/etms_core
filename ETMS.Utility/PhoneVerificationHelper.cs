using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Utility
{
    public class PhoneVerificationHelper
    {
        private static List<SKColor> colors { get; set; }

        public static string GetPhoneVerificationCode()
        {
            var rNum = new Random();
            return $"{rNum.Next(0, 9)}{rNum.Next(0, 9)}{rNum.Next(0, 9)}{rNum.Next(0, 9)}";
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="captchaText">验证码文字</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <param name="lineNum">干扰线数量</param>
        /// <param name="lineStrookeWidth">干扰线宽度</param>
        /// <returns></returns>
        public static string GetCaptcha(string captchaText, int width, int height, int lineNum = 5, int lineStrookeWidth = 2)
        {
            using (SKBitmap image2d = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul))
            {
                using (SKCanvas canvas = new SKCanvas(image2d))
                {
                    canvas.DrawColor(SKColors.White);
                    //将文字写到画布上
                    using (SkiaSharp.SKTypeface font = SKTypeface.FromFamilyName(null, SKFontStyleWeight.Medium, SKFontStyleWidth.ExtraCondensed, SKFontStyleSlant.Upright))
                    {
                        using (SKPaint paint = new SKPaint())
                        {
                            paint.IsAntialias = true;
                            paint.Color = SKColors.Black;
                            paint.Typeface = font;
                            paint.TextSize = height;
                            canvas.DrawText(captchaText, 1, height - 1, paint);
                        }
                    }

                    //画随机干扰线
                    using (SKPaint drawStyle = new SKPaint())
                    {
                        var random = new Random();
                        for (int i = 0; i < lineNum; i++)
                        {
                            drawStyle.Color = colors[random.Next(colors.Count)];
                            drawStyle.StrokeWidth = lineStrookeWidth;
                            canvas.DrawLine(random.Next(0, width), random.Next(0, height), random.Next(0, width), random.Next(0, height), drawStyle);
                        }
                    }
                    //返回图片byte
                    using (SKImage img = SKImage.FromBitmap(image2d))
                    {
                        using (SKData p = img.Encode(SKEncodedImageFormat.Png, 100))
                        {
                            return Convert.ToBase64String(p.ToArray());
                        }
                    }
                }
            }
        }

        static PhoneVerificationHelper()
        {
            colors = new List<SKColor>();
            colors.Add(SKColors.AliceBlue);
            colors.Add(SKColors.Orchid);
            colors.Add(SKColors.OrangeRed);
            colors.Add(SKColors.Orange);
            colors.Add(SKColors.OliveDrab);
            colors.Add(SKColors.Olive);
            colors.Add(SKColors.OldLace);
            colors.Add(SKColors.Navy);
            colors.Add(SKColors.NavajoWhite);
            colors.Add(SKColors.Moccasin);
            colors.Add(SKColors.LightSlateGray);
            colors.Add(SKColors.LightSteelBlue);
            colors.Add(SKColors.LightYellow);
            colors.Add(SKColors.Lime);
            colors.Add(SKColors.LimeGreen);
            colors.Add(SKColors.Linen);
            colors.Add(SKColors.Maroon);
            colors.Add(SKColors.PaleVioletRed);
            colors.Add(SKColors.PapayaWhip);
            colors.Add(SKColors.PeachPuff);
            colors.Add(SKColors.Snow);
            colors.Add(SKColors.SpringGreen);
            colors.Add(SKColors.SteelBlue);
            colors.Add(SKColors.Tan);
            colors.Add(SKColors.Thistle);
            colors.Add(SKColors.SlateGray);
            colors.Add(SKColors.Tomato);
            colors.Add(SKColors.Wheat);
            colors.Add(SKColors.Yellow);
            colors.Add(SKColors.YellowGreen);
            colors.Add(SKColors.SlateBlue);
            colors.Add(SKColors.Silver);
            colors.Add(SKColors.Peru);
            colors.Add(SKColors.Pink);
            colors.Add(SKColors.Plum);
            colors.Add(SKColors.PowderBlue);
            colors.Add(SKColors.Red);
            colors.Add(SKColors.SkyBlue);
            colors.Add(SKColors.RosyBrown);
            colors.Add(SKColors.SaddleBrown);
            colors.Add(SKColors.Salmon);
            colors.Add(SKColors.SandyBrown);
            colors.Add(SKColors.SeaGreen);
            colors.Add(SKColors.SeaShell);
            colors.Add(SKColors.Sienna);
            colors.Add(SKColors.RoyalBlue);
            colors.Add(SKColors.CornflowerBlue);
            colors.Add(SKColors.Chocolate);
            colors.Add(SKColors.AntiqueWhite);
        }
    }
}
