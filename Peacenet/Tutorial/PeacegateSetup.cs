﻿using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Peacenet.Tutorial
{
    /// <summary>
    /// The user-interface for the Tutorial Setup screen.
    /// </summary>
    public class PeacegateSetup : Window
    {
        private float _welcomeAnim = 0f;
        private float _cornerAnim = 0f;

        private Label _setupTitle = new Label();
        private Label _setupMode = new Label();
        private Button _next = new Button();
        private Button _back = new Button();
        private float _uiAnim = 0;

        private ScrollView _mainView = new ScrollView();

        private int _uiState = 0;
        private int _animState = 0;

        private double _animRide = 0;

        private TutorialBgmEntity _tutorial = null;
        private Label _introHeader = new Label();
        private Label _introText = new Label();

        private Panel _introPanel = new Panel();

        private void _resetUI()
        {
            _mainView.Clear();
            switch(_uiState)
            {
                case 0:
                    _back.Enabled = false;
                    _mainView.AddChild(_introPanel);
                    break;
                case 1:
                    _back.Enabled = true;
                    break;
            }
        }

        /// <inheritdoc/>
        public PeacegateSetup(WindowSystem _winsys, TutorialBgmEntity tutorial) : base(_winsys)
        {
            _tutorial = tutorial;
            SetWindowStyle(WindowStyle.NoBorder);
            Width = _winsys.Width;
            Height = _winsys.Height;
            AddChild(_setupTitle);
            AddChild(_setupMode);
            AddChild(_back);
            AddChild(_next);

            _back.Text = "Back";
            _next.Text = "Next";
            _setupMode.AutoSize = true;
            _setupMode.Text = "Introduction";

            _introPanel.AddChild(_introHeader);
            _introPanel.AddChild(_introText);
            _introPanel.AutoSize = true;

            _introHeader.AutoSize = true;
            _introText.AutoSize = true;
            _introHeader.FontStyle = Plex.Engine.Themes.TextFontStyle.Header3;

            _introHeader.Text = "Welcome to Peacegate OS.";
            _introText.Text = @"Peacegate OS is the gateway to the Peacenet. You will use it to run programs, interact with other members of the network, and perform other tasks. It is your primary user interface.

We will guide you through how to use Peacegate OS and the Peacegate Desktop, but first we must set some things up for first use. This installer program will guide you through the setup process and prepare your new environment for you.

Click 'Next' to get started.";

            _back.Click += (o, a) =>
            {
                if (_animState == 6)
                    return;
                _uiState--;
                _animState = 6;
            };
            _next.Click += (o, a) =>
            {
                if (_animState == 6)
                    return;
                _uiState++;
                _animState = 6;
            };

            AddChild(_mainView);
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            switch(_animState)
            {
                case 0:
                    _welcomeAnim += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if(_welcomeAnim>=1.0f)
                    {
                        _welcomeAnim = 1;
                        _animState++;
                    }
                    break;
                case 1:
                    _animRide += time.ElapsedGameTime.TotalSeconds;
                    if(_animRide >= 0.5)
                    {
                        _animRide = 0;
                        _animState++;
                    }
                    break;
                case 2:
                    _cornerAnim += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_cornerAnim >= 1.0f)
                    {
                        _cornerAnim = 1;
                        _animState++;
                    }
                    break;
                case 3:
                    _resetUI();
                    _animState++;
                    break;
                case 4:
                    _uiAnim += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_uiAnim >= 1.0f)
                    {
                        _uiAnim = 1;
                        _animState++;
                    }
                    break;
                case 6:
                    _uiAnim -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_uiAnim <= 0f)
                    {
                        _uiAnim = 0;
                        _animState = 3;
                    }
                    break;

            }
            Width = (int)MathHelper.Lerp(WindowSystem.Width - 50, WindowSystem.Width, _welcomeAnim);
            Height = (int)MathHelper.Lerp(WindowSystem.Height - 50, WindowSystem.Height, _welcomeAnim);
            Parent.X = (int)MathHelper.Lerp(25, 0, _welcomeAnim);
            Parent.Y = (int)MathHelper.Lerp(25, 0, _welcomeAnim);

            _setupTitle.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _setupTitle.Text = "Peacegate OS Setup";
            _setupTitle.AutoSize = true;

            //first we calculate where the title should ACTUALLY BE
            var titleLocMax = new Vector2(15, 15);
            var titleLocMin = new Vector2((Width - _setupTitle.Width) / 2, (Height - _setupTitle.Height) / 2);
            var titleLoc = Vector2.Lerp(titleLocMin, titleLocMax, this._cornerAnim);

            //Next, we calculate the proper Y coordinate.
            int titleLocY = (int)MathHelper.Lerp(titleLoc.Y + (Width * 0.25F), titleLoc.Y, _welcomeAnim);
            _setupTitle.X = (int)titleLoc.X;
            _setupTitle.Y = titleLocY;
            _setupTitle.Opacity = _welcomeAnim;

            //Align the setup category title.
            _setupMode.X = _setupMode.X;
            int setupModeYMax = _setupTitle.Y + _setupTitle.Height + 5;
            _setupMode.X = _setupTitle.X;
            _setupMode.Y = (int)MathHelper.Lerp(setupModeYMax + (WindowSystem.Height * 0.25f), setupModeYMax, _uiAnim);
            _setupMode.Opacity = _uiAnim;

            int buttonY = (Height - _next.Height) - 15;
            _next.Y = (int)MathHelper.Lerp(buttonY + (WindowSystem.Height * 0.25F), buttonY, _uiAnim);
            _back.Y = _next.Y;
            _next.Opacity = _uiAnim;
            _back.Opacity = _uiAnim;

            _next.X = (Width - _next.Width) - 15;
            _back.X = (_next.X - _back.Width) - 5;

            _setupMode.FontStyle = Plex.Engine.Themes.TextFontStyle.Header2;

            _mainView.X = 0;
            _mainView.Y = _setupMode.Y + _setupMode.Height + 25;
            _mainView.Opacity = _uiAnim;
            _mainView.Height = (_next.Y-15) - _mainView.Y;
            _mainView.MinWidth = Width;
            _mainView.MaxWidth = Width;
            _mainView.Width = Width;

            Opacity = _welcomeAnim;

            switch(_uiState)
            {
                case 0:
                    _introHeader.X = 30;
                    _introHeader.Y = 30;
                    _introHeader.MaxWidth = (_introPanel.Width - 60);
                    _introText.X = 30;
                    _introText.Y = _introHeader.Y + _introHeader.Height + 10;
                    _introText.MaxWidth = _introHeader.MaxWidth;
                    _introPanel.Width = Width;
                    break;
            }

            base.OnUpdate(time);
        }
    }
}
