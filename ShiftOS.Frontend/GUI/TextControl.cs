﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Frontend.GUI
{
    public class TextControl : Control
    {
        private string _text = "";
        private TextAlign _textAlign = TextAlign.TopLeft;
        private Font _font = new Font("Tahoma", 9f);
        private RenderTarget2D _textBuffer = null;
        bool requiresTextRerender = true;

        protected void RequireTextRerender()
        {
            requiresTextRerender = true;
        }


        protected virtual void RenderText(GraphicsContext gfx)
        {
            var sMeasure = GraphicsContext.MeasureString(_text, _font, Width);
            PointF loc = new PointF(2, 2);
            float centerH = (Width - sMeasure.X) / 2;
            float centerV = (Height - sMeasure.Y) / 2;
            switch (_textAlign)
            {
                case TextAlign.TopCenter:
                    loc.X = centerH;
                    break;
                case TextAlign.TopRight:
                    loc.X = Width - sMeasure.X;
                    break;
                case TextAlign.MiddleLeft:
                    loc.Y = centerV;
                    break;
                case TextAlign.MiddleCenter:
                    loc.Y = centerV;
                    loc.X = centerH;
                    break;
                case TextAlign.MiddleRight:
                    loc.Y = centerV;
                    loc.X = (Width - sMeasure.Y);
                    break;
                case TextAlign.BottomLeft:
                    loc.Y = (Height - sMeasure.Y);
                    break;
                case TextAlign.BottomCenter:
                    loc.Y = (Height - sMeasure.Y);
                    loc.X = centerH;
                    break;
                case TextAlign.BottomRight:
                    loc.Y = (Height - sMeasure.Y);
                    loc.X = (Width - sMeasure.X);
                    break;

            }

            gfx.DrawString(_text, 0, 0, Engine.SkinEngine.LoadedSkin.ControlTextColor.ToMonoColor(), _font, this.Width);

        }

        protected override void OnLayout(GameTime gameTime)
        {
            if (AutoSize)
            {
                var measure = GraphicsContext.MeasureString(_text, _font);
                Width = (int)measure.X;
                Height = (int)measure.Y;
            }
            if (requiresTextRerender)
                Invalidate();
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value)
                    return;
                _text = value;
                requiresTextRerender = true;
                OnTextChanged();
                Invalidate();
            }
        }

        public event Action TextChanged;
        protected virtual void OnTextChanged()
        {
            TextChanged?.Invoke();
        }

        public Font Font
        {
            get
            {
                return _font;
            }
            set
            {
                if (_font == value)
                    return;
                _font = value;
                requiresTextRerender = true;
            }
        }

        public TextAlign TextAlign
        {
            get { return _textAlign; }
            set { _textAlign = value; }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            if(_textBuffer != null)
            {
                if (_textBuffer.Width != Width || _textBuffer.Height != Height)
                {
                    requiresTextRerender = true;
                    _textBuffer = null;
                }
            }
            if (requiresTextRerender)
            {
                requiresTextRerender = false;
                if(_textBuffer == null)
                    _textBuffer = new RenderTarget2D(gfx.Device, Math.Max(1,Width), Math.Max(1,Height), false, gfx.Device.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
                gfx.Batch.End();
                int x = gfx.X;
                int y = gfx.Y;
                int w = gfx.Width;
                int h = gfx.Height;
                gfx.X = 0;
                gfx.Y = 0;
                gfx.Width = Width;
                gfx.Height = Height;
                gfx.Device.SetRenderTarget(null);
                gfx.Device.SetRenderTarget(_textBuffer);
                gfx.Device.Clear(Microsoft.Xna.Framework.Color.Transparent);
                gfx.Device.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                gfx.Batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, DepthStencilState.Default,
                                    RasterizerState.CullNone);
                RenderText(gfx);
                gfx.Device.SetRenderTarget(target);
                gfx.Batch.End();
                gfx.Device.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                gfx.X = x;
                gfx.Y = y;
                gfx.Width = w;
                gfx.Height = h;
                gfx.Batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, DepthStencilState.Default,
                                    RasterizerState.CullNone);

            }
            gfx.DrawRectangle(0, 0, Width, Height, _textBuffer, Microsoft.Xna.Framework.Color.White * (float)Opacity);
        }
    }

    public enum TextAlign
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}
