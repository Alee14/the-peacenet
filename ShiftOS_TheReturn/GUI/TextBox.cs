﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class TextBox : Control
    {
        private string _text = "";
        private string _label = "This is a text box.";
        private int _index = 0;
        private int _drawOffset = 0;
        private int _caretX = 0;

        public event EventHandler TextChanged;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text == value)
                    return;
                _text = value;
                _index = (int)MathHelper.Clamp(_index, 0, _text.Length);
                Invalidate();
            }
        }

        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                if (_label == value)
                    return;
                _label = value;
                Invalidate();
            }
        }

        private System.Drawing.Font _drawFont
        {
            get
            {
                return Theme.GetFont(Themes.TextFontStyle.System);
            }
        }

        private string _lastText = "";

        protected override void OnUpdate(GameTime time)
        {
            if(_lastText != _text)
            {
                _lastText = _text;
                TextChanged?.Invoke(this, EventArgs.Empty);
            }

            var hashMeasure = TextRenderer.MeasureText("#", _drawFont, int.MaxValue, TextAlignment.TopLeft, TextRenderers.WrapMode.None);
            Height = Math.Max((int)hashMeasure.Y + 4, Height);

            if (string.IsNullOrEmpty(_text))
            {
                if (_drawOffset != 0)
                {
                    _drawOffset = 0;
                    Invalidate();
                }
            }
            string toCaret = _text.Substring(0, _index);
            var measure = TextRenderer.MeasureText(toCaret, _drawFont, int.MaxValue, TextAlignment.TopLeft, TextRenderers.WrapMode.None);
            if (_caretX != (int)measure.X)
            {
                _caretX = (int)measure.X;
                Invalidate();
            }

            //calculate offset
            int realCaretX = _caretX - _drawOffset;
            if(realCaretX > Width-4)
            {
                _drawOffset = _caretX - (Width-4);
            }
            else if(realCaretX < 0)
            {
                _drawOffset = _caretX + (Width - 4);
            }
            base.OnUpdate(time);
        }

        protected override void OnKeyEvent(KeyboardEventArgs e)
        {
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                return;
            }
            if(e.Key == Microsoft.Xna.Framework.Input.Keys.Left)
            {
                if(_index > 0)
                {
                    _index--;
                    Invalidate();
                }
                return;
            }
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Right)
            {
                if (_index < _text.Length)
                {
                    _index++;
                    Invalidate();
                }
                return;
            }
            if (e.Character != null)
            {
                if (e.Character == '\b')
                {
                    if (_index > 0)
                    {
                        _text = _text.Remove(_index - 1, 1);
                        _index--;
                        Invalidate();
                    }
                    return;
                }
                _text = _text.Insert(_index, e.Character.ToString());
                _index++;
                Invalidate();
            }
            base.OnKeyEvent(e);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
            var colorBar = Color.Gray;
            if (ContainsMouse)
            {
                colorBar = Color.White;
            }
            if (IsFocused)
            {
                colorBar = Theme.GetAccentColor();
            }
            gfx.DrawRectangle(0, Height - 2, Width, 2, colorBar);
            if (string.IsNullOrWhiteSpace(_text))
            {
                if (!IsFocused)
                {
                    gfx.DrawString(_label, 2, 2, Color.Gray, _drawFont, TextAlignment.TopLeft, int.MaxValue, TextRenderers.WrapMode.None);
                }
            }
            else
            {
                gfx.DrawString(_text, 2 - _drawOffset, 2, Color.White, _drawFont, TextAlignment.TopLeft, int.MaxValue, TextRenderers.WrapMode.None);
                
            }
            if (IsFocused)
            {
                gfx.DrawRectangle(2 + (_caretX - _drawOffset), 4, 2, Height - 8, Color.White);
            }
        }

    }
}