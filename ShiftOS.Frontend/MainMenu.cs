﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Frontend.GUI;
using Plex.Frontend.GraphicsSubsystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Objects;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Plex.Frontend
{
    public class MainMenu : GUI.Control
    {
        private TextControl _mainTitle = new TextControl();
        private TextControl _menuTitle = new TextControl();
        private Button _campaign = new Button();
        private Button _sandbox = new Button();
        private Button _options = new Button();
        private Button _resDown = new Button();
        private Button _resUp = new Button();
        private TextControl _resDisplay = new TextControl();

        private TextControl _bodyTitle = new TextControl();
        private TextControl _bodySubtitle = new TextControl();
        private TextControl _bodyText = new TextControl();

        private Button _optionsSave = new Button();
        private CheckBox _fullscreen = new CheckBox();
        private Button _community = new Button();
        private Button _close = new Button();
        private TextControl _tips = new TextControl();
        
        public MainMenu()
        {
            X = 0;
            Y = 0;
            Width = UIManager.Viewport.Width;
            Height = UIManager.Viewport.Height;

            int _bodystart = Width / 4;
            AddControl(_bodyTitle);
            AddControl(_bodySubtitle);
            AddControl(_bodyText);
            AddControl(_mainTitle);
            AddControl(_menuTitle);
            AddControl(_campaign);
            AddControl(_sandbox);
            AddControl(_options);
            AddControl(_resUp);
            AddControl(_resDown);
            AddControl(_resDisplay);
            AddControl(_optionsSave);
            AddControl(_fullscreen);
            AddControl(_community);
            AddControl(_tips);

            _bodyTitle.X = _bodystart + 45;
            _bodyTitle.Y = 45;
            _bodyTitle.Font = SkinEngine.LoadedSkin.HeaderFont;
            _bodyTitle.Height = SkinEngine.LoadedSkin.HeaderFont.Height;
            _bodyTitle.AutoSize = true;
            _bodyTitle.Text = "Loading latest announcement...";

            _bodySubtitle.X = _bodyTitle.X;
            _bodySubtitle.Y = _bodyTitle.Y + _bodyTitle.Height + 15;
            _bodySubtitle.Font = SkinEngine.LoadedSkin.Header2Font;
            _bodySubtitle.Height = _bodySubtitle.Font.Height;
            _bodySubtitle.AutoSize = true;
            _bodySubtitle.Text = "Plex is connecting to servers to load the latest announcement.";

            _bodyText.X = _bodyTitle.X;
            _bodyText.Y = _bodySubtitle.Y + _bodySubtitle.Height + 15;
            _bodyText.Width = ((Width - _bodystart) - 90);
            _bodyText.Height = ((Height - (_bodySubtitle.Y + _bodySubtitle.Height + 15)) - 45);
            _optionsSave.Text = "Save sentience settings";
            _optionsSave.Width = (Width / 4) - 60;
            _optionsSave.X = 30;
            _optionsSave.Y = 300;
           
            _optionsSave.Height = 6 + _optionsSave.Font.Height;

            _campaign.Text = "Campaign";
            _campaign.Font = new System.Drawing.Font("Lucida Console", 10F);
            _campaign.Width = 200;
            _campaign.Height = 6 + _campaign.Font.Height;
            _campaign.X = 30;

            _optionsSave.Font = _campaign.Font;
            _optionsSave.Height = 6 + _optionsSave.Font.Height;


            _sandbox.Text = "Multiplayer";
            _sandbox.Width = 200;
            _sandbox.Height = 6 + _campaign.Font.Height;
            _sandbox.Font = _campaign.Font;
            _sandbox.X = 30;

            _community.Text = "Community";
            _community.Width = 200;
            _community.Height = 6 + _campaign.Font.Height;
            _community.Font = _campaign.Font;
            _community.X = 30;


            _fullscreen.X = 30;
            _fullscreen.Y = _campaign.Y;


            _options.Text = "Options";
            _options.Width = 200;
            _options.Height = 6 + _campaign.Font.Height;
            _options.Font = _campaign.Font;
            _options.X = 30;

            _mainTitle.X = 30;
            _mainTitle.Y = 30;
            _mainTitle.Font = new System.Drawing.Font("Lucida Console", 48F);
            _mainTitle.AutoSize = true;
            _mainTitle.Text = "Plex";

            _menuTitle.Text = "Main menu";
            _menuTitle.Font = new System.Drawing.Font(_menuTitle.Font.Name, 16F);
            _menuTitle.X = 30;
            _menuTitle.Y = _mainTitle.Y + _mainTitle.Font.Height + 10;
            _menuTitle.AutoSize = true;

            _campaign.Y = _menuTitle.Y + _menuTitle.Font.Height + 15;
            _sandbox.Y = _campaign.Y + _campaign.Font.Height + 15;
            _community.Y = _sandbox.Y + _sandbox.Font.Height + 15;
            _options.Y = _community.Y + _community.Font.Height + 15;
            _campaign.Click += () =>
            {
                UIManager.ConnectToServer("localhost", 62252);
                Close();
            };
            _sandbox.Click += () =>
            {
                var mpServerList = new Apps.MultiplayerServerList(Close);
                AppearanceManager.SetupDialog(mpServerList);

            };
            _community.Click += () =>
            {
                AppearanceManager.SetupDialog(new Apps.Community());
            };
            _options.Click += () =>
            {
                _menuTitle.Text = "Options";
                ShowOptions();
            };

            _resDown.Text = "<<";
            _resUp.Text = ">>";


            _resDown.Font = _campaign.Font;
            _resDown.Height = _campaign.Height;
            _resDown.Width = 40;
            _resUp.Font = _resDown.Font;
            _resUp.Height = _resDown.Height;
            _resUp.Width = _resDown.Width;
            _resDown.Y = _campaign.Y;
            _resUp.Y = _campaign.Y;
            _resDown.X = 30;
            _resUp.X = ((Width / 4) - _resUp.Width) - 30;
            _resDisplay.X = _resDown.X + _resDown.Width;
            _resDisplay.Width = (_resUp.X - _resDown.X);
            _resDisplay.Y = _resDown.Y;
            _resDisplay.Height = _resDown.Height;
            _resDisplay.TextAlign = TextAlign.MiddleCenter;
            _resDown.Click += () =>
            {
                if (_resIndex > 0)
                {
                    _resIndex--;
                    var res = _screenResolutions[_resIndex];
                    _resDisplay.Text = $"{res.Width}x{res.Height}";
                }
            };
            _resUp.Click += () =>
            {
                if(_resIndex < _screenResolutions.Count - 1)
                {
                    _resIndex++;
                    var res = _screenResolutions[_resIndex];
                    _resDisplay.Text = $"{res.Width}x{res.Height}";

                }
            };

            _fullscreen.CheckedChanged += () =>
            {
                UIManager.Fullscreen = _fullscreen.Checked;
            };

            _optionsSave.Click += () =>
            {

                if (UIManager.ScreenSize != _screenResolutions[_resIndex])
                {

                    Engine.Infobox.PromptYesNo("Confirm sentience edit", "Performing this operation requires your sentience to be re-established which may cause you to go unconscious. Do you wish to continue?", (sleep) =>
                    {
                        if (sleep == true)
                        {
                            SaveOptions();
                            System.Diagnostics.Process.Start("Plex.exe");
                            Environment.Exit(-1);
                        }
                    });
                }
                else
                {
                    SaveOptions();
                    _menuTitle.Text = "Main Menu";
                    _campaign.Visible = true;
                    _close.Visible = true;
                    _sandbox.Visible = true;
                    _options.Visible = true;
                }
            };

            foreach(var mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.OrderBy(x=>x.Width * x.Height))
            {
                    _screenResolutions.Add(new System.Drawing.Size(mode.Width, mode.Height));
                    if (UIManager.ScreenSize == _screenResolutions.Last())
                        _resIndex = _screenResolutions.Count - 1;
            }
            _fullscreen.Y = _resDown.Y + _resDown.Height + 5;

            _close.X = 30;
            _close.Font = _campaign.Font;
            _close.Y = _options.Y + _options.Font.Height + 15;
            _close.Text = "Close";
            _close.Height = _campaign.Height;
            _close.Width = _campaign.Width;
            AddControl(_close);
            _close.Click += () =>
            {
                SaveSystem.IsSandbox = true;
                PlexCommands.Shutdown();
            };
            var t = new System.Threading.Thread(() =>
            {
                var wc = new System.Net.WebClient();
                string json = wc.DownloadString("https://getshiftos.net/api/announcements");
                var announcement = JsonConvert.DeserializeObject<List<Announcement>>(json).First();
                _bodyTitle.Text = "Latest announcement";
                _bodySubtitle.Text = announcement.title[0].value;
                _bodyText.Text = Regex.Replace(announcement.body[0].value, "<.*?>", String.Empty);
            });
            t.Start();
        }

        public void SaveOptions()
        {
            var uconf = Objects.UserConfig.Get();
            var res = _screenResolutions[_resIndex];
            uconf.ScreenWidth = res.Width;
            uconf.ScreenHeight = res.Height;
            System.IO.File.WriteAllText("config.json", Newtonsoft.Json.JsonConvert.SerializeObject(uconf, Newtonsoft.Json.Formatting.Indented));

        }

        public void ShowOptions()
        {
            _campaign.Visible = false;
            _sandbox.Visible = false;
            _options.Visible = false;
            _close.Visible = false;
            var res = _screenResolutions[_resIndex];
            _resDisplay.Text = $"{res.Width}x{res.Height}";

        }

        private string _tipText = "This is some tip text.";
        private float _tipFade = 0.0f;
        private int _tipTime = 0;
        private List<System.Drawing.Size> _screenResolutions = new List<System.Drawing.Size>();
        private int _resIndex = 0;

        public void Close()
        {
            UIManager.StopHandling(this);
        }

        private Color _redbg = new Color(180, 0, 0, 255);
        private Color _bluebg = new Color(0, 0, 180, 255);
        private float _bglerp = 0.0f;
        private int _lerpdir = 1;
        private bool _tipVisible = false;
        private Random _rnd = new Random();
        private int _tipWidth = 0;
        private int _tipHeight = 0;
        

		private static readonly string[] funnyXD = new string[]
		{
			"Sentience choppy? Try adjusting sentience quality settings in\nOptions.",
			"Want to hear about the latest Plex news and events? Check out the Community menu!",
			"What's a 'tip of advice'?",
			"Welcome to the Digital\nSociety. Do you wish to continue?",
			"Open-source projects are pretty\ncool. This isn't one.",
			"Sure, you can toggle fullscreen in Options, but you can also use your F11 key to toggle it on and off in-game!",
			"Multithreading is the mayonnaise in the sandwich of\nPlex\u00ae.",
			"Ammunition is precious, so don't waste it.",
			"Your experienceing a lethal virus which i like to call (death)",
			"We ran out of things to say."
		};
		
		private static List<string> remainingTips = null;

        public string GetRandomString()
        {
			if (remainingTips == null || remainingTips.Count == 0)
				remainingTips = new List<string>(funnyXD);
            int r = _rnd.Next(0, remainingTips.Count);
            string ret = remainingTips[r];
            remainingTips.RemoveAt(r);
            return ret;
        }

        protected override void OnLayout(GameTime gameTime)
        {
            if (_lerpdir == 1)
                _bglerp += 0.00075f;
            else
                _bglerp -= 0.00075f;
            if (_bglerp <= 0.0)
                _lerpdir = 1;
            else if (_bglerp >= 1)
                _lerpdir = -1;

            if (_tipVisible == false)
            {


                _tipTime = 0;
                if (_tipFade == 0.0f)
                {
                    _tipText = GetRandomString();
                    var measure = GraphicsContext.MeasureString(_tipText, _campaign.Font, (Width / 4) - 30);
                    _tipWidth = (int)measure.X;
                    _tipHeight = (int)measure.Y;

                }
                _tipFade += 0.01f;
                if(_tipFade >= 1.0f)
                {
                    _tipVisible = true;
                }
            }
            else
            {
                _tipTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if(_tipTime >= 10000)
                {
                    _tipFade -= 0.01f;
                    if(_tipFade <= 0.0f)
                    {
                        _tipVisible = false;
                        _tipFade = 0.0f;
                    }
                }
            }

            _tips.Visible = true;
            _tips.Opacity = _tipFade;
            _tips.Text = _tipText;
            _tips.Width = _tipWidth;
            _tips.Height = _tipHeight;
            _tips.X = 30;
            _tips.Y = (Height - _tips.Height) - 30;
            _tips.Height = Height - _tips.Y; // ugh
            _tips.Font = _campaign.Font;

            _resDown.Visible = (_menuTitle.Text == "Options" && _resIndex > 0);

            _resUp.Visible = (_menuTitle.Text == "Options" && _resIndex < _screenResolutions.Count - 1);
            _resDisplay.Visible = _menuTitle.Text == "Options";
            _optionsSave.Visible = _resDisplay.Visible;
            _community.Visible = !_resDisplay.Visible;
            _fullscreen.Visible = _optionsSave.Visible;

            if (_fullscreen.Visible)
            {
                _fullscreen.Checked = UIManager.Fullscreen;
            }

            _bodySubtitle.AutoSize = true;
            _bodySubtitle.MaxWidth = _bodyText.Width;
            _bodyText.Y = _bodySubtitle.Y + _bodySubtitle.Height + 15;

            Invalidate();
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.DrawRectangle(0, 0, Width / 4, Height, Color.White * 0.35F);

            //var measure = GraphicsContext.MeasureString(_tipText, _campaign.Font, (Width / 4) - 30);
            //int _height = (Height - (int)measure.Y) - 30;
            //gfx.DrawString(_tipText, 30, _height, Color.White * _tipFade, _campaign.Font, (Width / 4) - 30);
        }
    }
}
