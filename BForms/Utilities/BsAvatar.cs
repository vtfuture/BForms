using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace BForms.Utilities
{
    public class BsAvatar
    {
        private int _width;
        private int _height;
        private string _text;
        private bool _circle;
        private Color _backgroundColor;
        private Color _fontColor;
        private Font _font;
        private List<string> FlatUIColors = new List<string>() { "#1abc9c", "#2ecc71", "#3498db", "#9b59b6", "#34495e", "#16a085", "#27ae60", "#2980b9", "#8e44ad", "#f1c40f", "#e67e22", "#e74c3c", "#f39c12", "#d35400", "#c0392b" };
        private List<string> MonochromColors = new List<string>() { "#ecf0f1", "#ffffff", "#95a5a6", "#bdc3c7", "#7f8c8d" };
        private static Random rnd = new Random();

        public BsAvatar() 
        {
            this._width = 64;
            this._height = 64;
        }

        public BsAvatar(string text) : this()
        {
            this._text = text;
        }

        public BsAvatar Size(int size)
        {
            this._width = size;
            this._height = size;

            return this;
        }

        public BsAvatar Size(int width, int height)
        {
            this._width = width;
            this._height = height;

            return this;
        }

        public BsAvatar Circle()
        {
            this._circle = true;

            return this;
        }

        public BsAvatar BackgroundColor(string hex)
        {
            this._backgroundColor = ColorTranslator.FromHtml(hex);

            return this;
        }

        public BsAvatar BackgroundColor(Color color)
        {
            this._backgroundColor = color;

            return this;
        }

        public BsAvatar Font(string fontFamily, float fontSize, FontStyle fontStyle)
        {
            this._font = new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Pixel);

            return this;
        }

        public BsAvatar Font(Font font)
        {
            this._font = font;

            return this;
        }

        public BsAvatar FontColor(Color fontColor)
        {
            this._fontColor = fontColor;

            return this;
        }

        public BsAvatar FontColor(string hex)
        {
            this._fontColor = ColorTranslator.FromHtml(hex);

            return this;
        }

        public BsAvatar Text(string text)
        {
            this._text = text;

            return this;
        }

        public BsAvatar Name(string fullName)
        {
            Regex regexInitials = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
            this._text = regexInitials.Replace(fullName, "$1").Replace(".", "");

            return this;
        }

        public MemoryStream GenerateStream()
        {
            using (var bitmap = new Bitmap(this._width, this._height))
            {
                DrawAvatar(bitmap);

                using (var memStream = new System.IO.MemoryStream())
                {
                    bitmap.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);

                    return memStream;
                }
            }
        }

        public byte[] Generate()
        {
            return GenerateStream().ToArray();
        }

        private void DrawAvatar(Bitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.Transparent);

                Rectangle rect = DrawRectangle(g);

                DrawText(g, rect);
            }
        }

        private Rectangle DrawRectangle(Graphics g)
        {
            Rectangle rect = new Rectangle(0, 0, this._width - 1, this._height - 1);

            if (this._backgroundColor == Color.Empty)
            {
                this._backgroundColor = GetRandomBackgroundColor();
            }

            using (Brush brush = new SolidBrush(this._backgroundColor))
            {
                if (this._circle)
                {
                    g.FillEllipse(brush, rect);
                }
                else
                {
                    g.FillRectangle(brush, rect);
                }
            }

            return rect;
        }

        private void DrawText(Graphics g, Rectangle rect)
        {
            if (!string.IsNullOrEmpty(this._text))
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                if (this._fontColor == Color.Empty)
                {
                    this._fontColor = GetRandomFontColor();
                }

                if (this._font == null)
                {
                    this._font = GetDefaultFont();
                }

                g.DrawString(this._text, this._font, new SolidBrush(this._fontColor), rect, stringFormat);
            }
        }

        private Color GetRandomBackgroundColor()
        {
            int r = rnd.Next(FlatUIColors.Count);

            return ColorTranslator.FromHtml(FlatUIColors[r]);
        }

        private Color GetRandomFontColor()
        {
            int r = rnd.Next(MonochromColors.Count);

            return ColorTranslator.FromHtml(MonochromColors[r]);
        }

        private Font GetDefaultFont()
        {
            return new Font("Arial", 20, FontStyle.Bold, GraphicsUnit.Pixel);
        }
    }
}