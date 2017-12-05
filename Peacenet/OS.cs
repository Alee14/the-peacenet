﻿using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Cutscene;
using Plex.Engine;
using Plex.Engine.Saves;
using Plex.Engine.GUI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Plex.Engine.Themes;

namespace Peacenet
{
    public class OS : IEngineComponent
    {
        [Dependency]
        private CutsceneManager _cutscene = null;
        [Dependency]
        private UIManager _ui = null;
        [Dependency]
        private SaveManager _save = null;
        [Dependency]
        private WindowSystem _winmgr = null;
        [Dependency]
        private InfoboxManager _infobox = null;
        [Dependency]
        private Plexgate _plexgate = null;
        [Dependency]
        private ThemeManager _theme = null;


        private Label _statusLabel = null;


        private IEnumerable<string> _previouslyValues
        {
            get
            {
                yield return $"Cash: ${_save.GetValue<double>("game.cash", 0)}";
                yield return $"XP: {_save.GetValue<int>("game.xp", 0)}";
                yield return $"Rank: {_save.GetValue<int>("game.rank", 0)}";
            }
        }

        private int _statusIndex = -1;
        private int _statusCount = 0;

        private int _osIntroState = -1;

        private SoundEffect[] AmbientMusic = null;
        private SoundEffectInstance[] AmbientInstances = null;
        private int _trackIndex = -1;
        private bool _hasPlayedCHosenSong = false;

        private Random _rnd = new Random();

        private PictureBox _peacenet = null;
        private Label _previously = null;

        private float _peacenetScaleAnim = 0;

        private double _previouslyRide = 0;

        private Label _startingDesktop = new Label();

        public void Initiate()
        {
            _osIntroState = -1;
            _peacenet = new PictureBox();
            _peacenet.Texture = _plexgate.Content.Load<Texture2D>("Splash/Peacenet");
            _peacenet.AutoSize = true;
            _peacenet.Visible = false;
            _ui.Add(_peacenet);
            _previously = new Label();
            _previously.FontStyle = Plex.Engine.Themes.TextFontStyle.Header1;
            _previously.CustomFont = new System.Drawing.Font("Monda", 15F);
            _previously.CustomColor = new Color(191, 191, 191, 255);
            _previously.Visible = false;
            _previously.AutoSize = true;
            _ui.Add(_previously);

            AmbientMusic = _plexgate.Content.LoadAllIn<SoundEffect>("Music/Ambient");
            AmbientInstances = new SoundEffectInstance[AmbientMusic.Length];
            for(int i = 0; i < AmbientInstances.Length; i++)
            {
                AmbientInstances[i] = AmbientMusic[i].CreateInstance();
            }
            
            _statusLabel = new Label();
            _statusLabel.Visible = false;
            _statusLabel.FontStyle = TextFontStyle.Header2;
            _statusLabel.AutoSize = true;
            _statusLabel.Opacity = 0;
            _ui.Add(_statusLabel);

            _startingDesktop = new Label();
            _startingDesktop.Text = "Starting Peacegate session...";
            _startingDesktop.AutoSize = true;
            _startingDesktop.FontStyle = TextFontStyle.Header1;
            _startingDesktop.Visible = false;
            _ui.Add(_startingDesktop);
        }

        public void OnReady()
        {
            _osIntroState = 0;
            _statusCount = _previouslyValues.Count();
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
            ctx.BeginDraw();
            ctx.DrawRectangle(0, 30 + (_peacenet.Texture.Height / 2), (int)MathHelper.Lerp(0, _ui.ScreenWidth, _peacenetScaleAnim), 2, _theme.Theme.GetAccentColor());
            ctx.EndDraw();
        }

        private double _statusRide = 0;
        private double _startupRide = 0;


        public void OnGameUpdate(GameTime time)
        {
            if(_trackIndex != -1)
            {
                if(AmbientInstances[_trackIndex].State == SoundState.Stopped)
                {
                    if (_hasPlayedCHosenSong == false)
                    {
                        _hasPlayedCHosenSong = true;
                        AmbientInstances[_trackIndex].Play();
                    }
                    else
                    {
                        _hasPlayedCHosenSong = false;
                        _trackIndex = _rnd.Next(0, AmbientInstances.Length);
                    }
                }
            }

            switch (_osIntroState)
            {
                case 0:
                    _peacenet.Opacity = 0;
                    _peacenet.Visible = true;
                    _osIntroState++;
                    break;
                case 1:
                    _peacenet.Opacity += (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_peacenet.Opacity >= 1)
                    {
                        _osIntroState++;
                    }
                    break;
                case 2:
                    _previously.Visible = true;
                    _previously.Opacity = 0;
                    if (!_save.GetValue("boot.hasDoneIntro", false))
                    {
                        _previously.Text = "Welcome to";
                    }
                    else
                    {
                        _previously.Text = "Previously on";
                        _trackIndex = _rnd.Next(0, AmbientInstances.Length);
                    }
                    _osIntroState++;
                    break;
                case 3:
                    _previously.Opacity += (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_previously.Opacity >= 1)
                    {
                        _osIntroState++;
                    }
                    break;
                case 4:
                    _previouslyRide += time.ElapsedGameTime.TotalSeconds;
                    if (_previouslyRide >= 3)
                    {
                        _osIntroState++;
                    }
                    break;
                case 5:
                    _previously.Opacity -= (float)time.ElapsedGameTime.TotalSeconds * 3;
                    _peacenetScaleAnim = 1 - _previously.Opacity;
                    if (_peacenetScaleAnim >= 1)
                    {
                        //Let's decide where to go based on the save file.
                        //If the user's played the intro sequence, we'll do one animation.
                        //If they haven't, we'll do another.
                        if (!_save.GetValue("boot.hasDoneIntro", false))
                        {
                            _osIntroState = 16;
                        }
                        else
                        {
                            _osIntroState++;
                        }

                    }
                    break;
                case 6:
                    _statusIndex++;
                    _osIntroState++;
                    _statusLabel.Visible = true;
                    _statusRide = 0;
                    _statusLabel.Text = _previouslyValues.ToArray()[_statusIndex];
                    break;
                case 7:
                    _statusLabel.Opacity += (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_statusLabel.Opacity >= 1)
                    {
                        _osIntroState++;
                    }
                    break;
                case 8:
                    _statusRide += time.ElapsedGameTime.TotalSeconds;
                    if(_statusRide >= 1.5)
                    {
                        _osIntroState++;
                    }
                    break;
                case 9:
                    _statusLabel.Opacity -= (float)time.ElapsedGameTime.TotalSeconds * 4;
                    if (_statusLabel.Opacity <= 0)
                    {
                        if (_statusIndex < _statusCount - 1)
                        {
                            _osIntroState = 6;
                        }
                        else
                        {
                            _osIntroState++;
                        }
                    }

                    break;
                case 10:
                    //RESERVED FOR DESKTOP START ANIMATIONS.
                    _peacenetScaleAnim -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if(_peacenetScaleAnim<=0)
                    {
                        _osIntroState++;
                        _startingDesktop.Visible = true;
                        _startingDesktop.Opacity = 0;
                    }
                    break;
                case 11:
                    _startingDesktop.Opacity += (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_startingDesktop.Opacity >= 1)
                    {
                        _osIntroState++;
                    }
                    break;
                case 12:
                    _startupRide += time.ElapsedGameTime.TotalSeconds;
                    if (_startupRide > 3)
                    {
                        _osIntroState++;
                    }
                    break;
                case 13:
                    _startingDesktop.Opacity -= (float)time.ElapsedGameTime.TotalSeconds * 2;
                    if (_startingDesktop.Opacity <= 0)
                    {
                        _osIntroState++;
                    }

                    break;
                case 14:
                    _peacenet.Opacity -= (float)time.ElapsedGameTime.TotalSeconds * 3;
                    if (_peacenet.Opacity <= 0)
                    {
                        _osIntroState++;
                    }
                    break;
                case 15:
                    var desk = new DesktopWindow(_winmgr);
                    desk.Show();
                    _osIntroState = -1;
                    break;
            }

            int peacenetXMin = (_ui.ScreenWidth - _peacenet.Width) / 2;
            int peacenetXMax = (_ui.ScreenWidth - (_peacenet.Texture.Width/2)) - 15;

            int peacenetYFirstPassMin = (_ui.ScreenHeight - _peacenet.Height) / 2;
            int peacenetYFirstPassMax = 15;

            _peacenet.AutoSizeScale = MathHelper.Lerp(2, 0.5F, _peacenetScaleAnim);

            int peacenetX = (int)MathHelper.Lerp(peacenetXMin, peacenetXMax, _peacenetScaleAnim);
            int peacenetYFirstPass = (int)MathHelper.Lerp(peacenetYFirstPassMin, peacenetYFirstPassMax, _peacenetScaleAnim);

            int peacenetY = (int)MathHelper.Lerp(peacenetYFirstPass + (int)(_ui.ScreenHeight * 0.1), peacenetYFirstPass, _peacenet.Opacity);

            _peacenet.X = peacenetX;
            _peacenet.Y = peacenetY;

            int _previouslyYFirstPassMax = _peacenet.Y - _previously.Height;
            int _previouslyYFirstPassMin = _previouslyYFirstPassMax + (int)(_ui.ScreenHeight * 0.1);
            int _previouslyYFirstPass = (int)MathHelper.Lerp(_previouslyYFirstPassMin, _previouslyYFirstPassMax, _previously.Opacity);
            _previously.Y = _previouslyYFirstPass;
            _previously.X = (_ui.ScreenWidth - (_peacenet.Texture.Width*2)) / 2;

            _statusLabel.X = (_ui.ScreenWidth - _statusLabel.Width) / 2;

            int statusYMax = (_ui.ScreenHeight - _statusLabel.Height) / 2;
            int statusYMin = statusYMax + (int)(_ui.ScreenHeight * 0.1);
            _statusLabel.Y = (int)MathHelper.Lerp(statusYMin, statusYMax, _statusLabel.Opacity);

            _startingDesktop.X = (_ui.ScreenWidth - _startingDesktop.Width) / 2;
            int startYMax = _peacenet.Y + _peacenet.Height;
            int startYMin = startYMax + (int)(_ui.ScreenHeight * 0.1);
            _startingDesktop.Y = (int)MathHelper.Lerp(startYMin, startYMax, _startingDesktop.Opacity);
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
        }
    }

    public static class ContentHelper
    {
        public static T[] LoadAllIn<T>(this ContentManager content, string contentdir)
        {
            List<T> _contentList = new List<T>();
            DirectoryInfo dir = new DirectoryInfo(content.RootDirectory + "/" + contentdir);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);

                try
                {
                    _contentList.Add(content.Load<T>(contentdir + "/" + key));
                }
                catch { }
            }
            return _contentList.ToArray();
        }
    }
}