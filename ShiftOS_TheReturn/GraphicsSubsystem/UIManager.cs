﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine;
using Plex.Engine.TextRenderers;
using Plex.Engine.GUI;
using Plex.Objects;

namespace Plex.Engine.GraphicsSubsystem
{
    public static class UIManager
    {
        private static List<GUI.Control> topLevels = new List<GUI.Control>();

        public static void ClearTopLevels()
        {
            while(topLevels.Count > 0)
            {
                StopHandling(topLevels[0]);
            }
        }

        private static List<GUI.Control> hudctrls = new List<GUI.Control>();
        public static System.Drawing.Size Viewport { get; set; }
        public static GUI.Control FocusedControl = null;
        private static Plexgate _game = null;


        public static void Crash()
        {
            _game.Crash();
        }

        public static void SetTutorialOverlay(Rectangle mouserect, string text, Action complete)
        {
            _game.TutorialOverlayText = text;
            _game.MouseEventBounds = mouserect;
            _game.TutorialOverlayCompleted = complete;
            _game.IsInTutorial = true;
        }

        public static void Init(Plexgate sentience)
        {
            _game = sentience;
        }

        public static bool Fullscreen
        {
            get
            {
                return _game.graphicsDevice.IsFullScreen;
            }
            set
            {
                var uconf = Objects.UserConfig.Get();
                uconf.Fullscreen = value;
                System.IO.File.WriteAllText("config.json", Newtonsoft.Json.JsonConvert.SerializeObject(uconf, Newtonsoft.Json.Formatting.Indented));
                _game.graphicsDevice.IsFullScreen = value;
                _game.graphicsDevice.ApplyChanges();
            }
        }

        public static System.Drawing.Size ScreenSize
        {
            get
            {
                try
                {
                    return new System.Drawing.Size(_game.graphicsDevice.PreferredBackBufferWidth, _game.graphicsDevice.PreferredBackBufferHeight);
                }
                catch
                {
                    var conf = UserConfig.Get();
                    return new System.Drawing.Size(conf.ScreenWidth, conf.ScreenHeight);
                }
            }
        }

        public static void BringToFront(GUI.Control ctrl)
        {
            topLevels.Remove(ctrl);
            topLevels.Add(ctrl);
        }

        public static void LayoutUpdate(GameTime gameTime)
        {
            foreach (var toplevel in topLevels.ToArray())
                toplevel.Layout(gameTime);
            foreach (var toplevel in hudctrls.ToArray())
                toplevel.Layout(gameTime);
        }

        public static Dictionary<int, RenderTarget2D> TextureCaches = new Dictionary<int, RenderTarget2D>();
        public static Dictionary<int, RenderTarget2D> HUDCaches = new Dictionary<int, RenderTarget2D>();


        public static void DrawTArgets(SpriteBatch batch)
        {
            DrawTargetsInternal(batch, ref topLevels, ref TextureCaches);
        }


        public static void DrawHUD(SpriteBatch batch)
        {
            DrawTargetsInternal(batch, ref hudctrls, ref HUDCaches);
        }

        private static void DrawTargetsInternal(SpriteBatch batch, ref List<Control> controls, ref Dictionary<int, RenderTarget2D> targets)
        {
            foreach (var ctrl in controls.ToArray())
            {
                if (ctrl.Visible == true)
                {
                    int hc = ctrl.GetHashCode();
                    if (!targets.ContainsKey(hc))
                    {
                        ctrl.Invalidate();
                        continue;
                    }
                    var _target = targets[hc];
                    if (_target.Width != ctrl.Width || _target.Height != ctrl.Height)
                    {
                        ctrl.Invalidate();
                        DrawControlsToTargets(batch.GraphicsDevice, batch);
                    }
                    batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                            SamplerState.LinearWrap, DepthStencilState.Default,
                            RasterizerState.CullNone);

                    batch.Draw(_target, new Rectangle(ctrl.X, ctrl.Y, ctrl.Width, ctrl.Height), _game.UITint);
                    batch.End();
                }
            }
        }


        public static void SendToBack(Control ctrl)
       { 
            topLevels.Remove(ctrl);
            topLevels.Insert(0, ctrl);
        }

        public static void DrawControlsToTargetsInternal(GraphicsDevice graphics, SpriteBatch batch, int width, int height, ref List<Control> controls, ref Dictionary<int, RenderTarget2D> targets)
        {
            foreach (var ctrl in controls.ToArray().Where(x => x.Visible == true))
            {
                RenderTarget2D _target;
                int hc = ctrl.GetHashCode();
                if (!targets.ContainsKey(hc))
                {
                    _target = new RenderTarget2D(
                                    graphics,
                                    Math.Max(1, ctrl.Width),
                                    Math.Max(1, ctrl.Height),
                                    false,
                                    graphics.PresentationParameters.BackBufferFormat,
                                    DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
                    targets.Add(hc, _target);
                    ctrl.Invalidate();
                }
                else
                {
                    _target = targets[hc];
                    if (_target.Width != ctrl.Width || _target.Height != ctrl.Height)
                    {
                        _target.Dispose();
                        _target = new RenderTarget2D(
                graphics,
                Math.Max(1, ctrl.Width),
                Math.Max(1, ctrl.Height),
                false,
                graphics.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
                        targets[hc] = _target;
                        //ENSURE the target gets repainted
                        ctrl.Invalidate();
                    }
                }
                if (ctrl.RequiresPaint)
                {
                    try
                    {
                        QA.Assert(_target == null, false, "Null render target in UI subsystem");
                        QA.Assert(_target.IsDisposed, false, "Attempting to paint disposed render target");
                        graphics.SetRenderTarget(_target);
                        batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                        SamplerState.LinearWrap, GraphicsDevice.DepthStencilState,
                                        RasterizerState.CullNone);
                        graphics.Clear(Color.Transparent);
                        var gfxContext = new GraphicsContext(graphics, batch, 0, 0, ctrl.Width, ctrl.Height);
                        ctrl.Paint(gfxContext, _target);
                        QA.Assert(_target.IsContentLost, false, "A render target has lost its contents.");
                        QA.Assert(_target.RenderTargetUsage == RenderTargetUsage.PreserveContents, true, "A render target whose usage is not set to RenderTargetUsage.PreserveContents is being rendered to. This is not allowed.");


                        batch.End();
                        QA.Assert(_target.IsContentLost, false, "A render target has lost its contents.");
                        QA.Assert(_target.RenderTargetUsage == RenderTargetUsage.PreserveContents, true, "A render target whose usage is not set to RenderTargetUsage.PreserveContents is being rendered to. This is not allowed.");
                        graphics.SetRenderTarget(_game.GameRenderTarget);
                        QA.Assert(_target.IsContentLost, false, "A render target has lost its contents.");
                        QA.Assert(_target.RenderTargetUsage == RenderTargetUsage.PreserveContents, true, "A render target whose usage is not set to RenderTargetUsage.PreserveContents is being rendered to. This is not allowed.");
                        targets[hc] = _target;
                    }
                    catch (AccessViolationException)
                    {

                    }
                }
            }
        }

        public static class FourthWall
        {
            public static bool GetFilePath(string title, string wfFilter, FileOpenerStyle style, out string resPath)
            {
                string initDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                bool isFullscreen = _game.graphicsDevice.IsFullScreen;
                string p = "";
                if (isFullscreen)
                {
                    //knock the player out of fullscreen
                    _game.graphicsDevice.IsFullScreen = false;
                    _game.graphicsDevice.ApplyChanges();
                }
                switch (style)
                {
                    case FileOpenerStyle.Open:
                        var opener = new System.Windows.Forms.OpenFileDialog();
                        opener.Filter = wfFilter;
                        opener.Title = title;
                        opener.InitialDirectory = initDir;
                        if (opener.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            p = opener.FileName;
                        }
                        break;
                    case FileOpenerStyle.Save:
                        var saver = new System.Windows.Forms.SaveFileDialog();
                        saver.Filter = wfFilter;
                        saver.Title = title;
                        saver.InitialDirectory = initDir;
                        if (saver.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            p = saver.FileName;
                        }
                        break;
                }
                resPath = p;
                if (isFullscreen)
                {
                    //boot the player back into fullscreen
                    _game.graphicsDevice.IsFullScreen = true;
                    _game.graphicsDevice.ApplyChanges();
                }

                return !string.IsNullOrWhiteSpace(resPath);
            }
        }


        public static long Average(this byte[] bytes)
        {
            long total = 0;
            foreach (var b in bytes)
                total += b;
            return total / bytes.Length;
        }

        public static event Action SinglePlayerStarted;

        public static void StartSPServer()
        {
            SinglePlayerStarted?.Invoke();
        }

        public static void DrawControlsToTargets(GraphicsDevice device, SpriteBatch batch)
        {
            DrawControlsToTargetsInternal(device, batch, Viewport.Width, Viewport.Height, ref topLevels, ref TextureCaches);
        }

        public static void DrawHUDToTargets(GraphicsDevice device, SpriteBatch batch)
        {
                DrawControlsToTargetsInternal(device, batch, Viewport.Width, Viewport.Height, ref hudctrls, ref HUDCaches);
        }

        public static void ShowCloudUpload()
        {
            _game.uploading = true;
        }

        public static void HideCloudUpload()
        {
            _game.uploading = false;
        }

        public static void ShowCloudDownload()
        {
            _game.downloading = true;
        }

        public static void HideCloudDownload()
        {
            _game.downloading = false;
        }



        public static void AddTopLevel(GUI.Control ctrl)
        {
            Desktop.InvokeOnWorkerThread(() =>
            {
                if (!topLevels.Contains(ctrl))
                    topLevels.Add(ctrl);
                FocusedControl = ctrl;
                ctrl.Invalidate();
            });
        }

        public static void AddHUD(GUI.Control ctrl)
        {
            Desktop.InvokeOnWorkerThread(() =>
            {
                if (!hudctrls.Contains(ctrl))
                    hudctrls.Add(ctrl);
                ctrl.Invalidate();
            });
        }


        public static void InvalidateAll()
        {
            Desktop.InvokeOnWorkerThread(() =>
            {
                foreach (var ctrl in topLevels)
                {
                    ctrl.Invalidate();
                }
                foreach (var ctrl in hudctrls)
                {
                    ctrl.Invalidate();
                }
            });
        }

        public static Control[] TopLevels
        {
            get
            {
                return topLevels.ToArray();
            }
        }

        public static void ProcessMouseState(MouseState state, double lastLeftClickMS)
        {
            bool rclick = true;
            bool hidemenus = true;
            foreach (var ctrl in topLevels.ToArray().Where(x=>x != null).Where(x=>x.Visible == true).OrderByDescending(x=>topLevels.IndexOf(x)))
            {
                if (ctrl.ProcessMouseState(state, lastLeftClickMS))
                {
                    if(ctrl is Menu||ctrl is MenuItem)
                    {
                        hidemenus = false;
                    }
                    rclick = false;
                    break;
                }
            }
            if (hidemenus == true)
            {
                if (!(state.LeftButton == ButtonState.Released && state.MiddleButton == ButtonState.Released && state.RightButton == ButtonState.Released))
                {
                    var menus = topLevels.Where(x => x is Menu || x is MenuItem);
                    foreach (var menu in menus.ToArray())
                        (menu as Menu).Hide();
                }
            }
            if (rclick == true)
            {
                if (rmouselast == false && state.RightButton == ButtonState.Pressed)
                {
                    rmouselast = true;
                    ScreenRightclicked?.Invoke(state.X, state.Y);
                    return;
                }
                if (rmouselast == true && state.RightButton == ButtonState.Released)
                {
                    rmouselast = false;
                    return;
                }
            }
        }

        public static ContentManager ContentLoader
        {
            get
            {
                return _game.Content;
            }
        }

        public static event Action<int, int> ScreenRightclicked;

        private static bool rmouselast = false;

        public static void ProcessKeyEvent(KeyEvent e)
        {
            if (e.ControlDown && e.Key == Keys.T)
            {
                TerminalBackend.OpenTerminal();
                return;
            }
            FocusedControl?.ProcessKeyEvent(e);
        }

        public static void SetUITint(Color color)
        {
            _game.UITint = color;
        }


        public static bool ExperimentalEffects = true;

        public static Queue<Action> CrossThreadOperations = new Queue<Action>();
        public static GraphicsDevice GraphicsDevice
        {
            get
            {
                return _game.graphicsDevice.GraphicsDevice;
            }
        }


        public static void DrawBackgroundLayer(GraphicsDevice graphics, SpriteBatch batch, int width, int height)
        {
            graphics.Clear(Color.Black);
        }

        public static Color ToMonoColor(this System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        public static void StopHandling(GUI.Control ctrl)
        {
            if (topLevels.Contains(ctrl))
                topLevels.Remove(ctrl);

            int hc = ctrl.GetHashCode();
            if (TextureCaches.ContainsKey(hc))
            {
                TextureCaches[hc].Dispose();
                TextureCaches.Remove(hc);
            }
            ctrl.Dispose();
            ctrl = null;
        }

        public static void ConnectToServer(string host, int port)
        {
            if (!ServerManager.ConnectToServer(host, port))
                return;
            bool isMP = true;
            
            using(var sstr = new ServerStream(ServerMessageType.U_CONF))
            {
                var result = sstr.Send();
                using(var reader= new BinaryReader(ServerManager.GetResponseStream(result)))
                {
                    isMP = reader.ReadBoolean();
                }
            }

            _game.Port = port;
            //Start session management for this server...
            SaveSystem.IsSandbox = isMP; //If we're on a multiplayer server then disable the story system.
            if (isMP)
            {
                ServerManager.StartSessionManager(host, port);
            }
            else
            {
                ServerManager.StartSinglePlayer(host, port);
            }
        }

        public static Plexgate Game
        {
            get
            {
                return _game;
            }
        }

        internal static void StopHandlingHUD(GUI.Control ctrl)
        {
            if (hudctrls.Contains(ctrl))
                hudctrls.Remove(ctrl);

            int hc = ctrl.GetHashCode();
            if (HUDCaches.ContainsKey(hc))
            {
                HUDCaches[hc].Dispose();
                HUDCaches.Remove(hc);
            }
            ctrl.Dispose();

            ctrl = null;
        }

    }

    public class KeyEvent
    {

        public KeyEvent(KeyboardEventArgs e)
        {
            ControlDown = false;
            ShiftDown = e.Modifiers.HasFlag(KeyboardModifiers.Shift);
            ControlDown = e.Modifiers.HasFlag(KeyboardModifiers.Control);
            AltDown = e.Modifiers.HasFlag(KeyboardModifiers.Alt);
            Key = e.Key;
            KeyChar = e.Character ?? '\0' ;
        }


        public bool ControlDown { get; private set; }
        public bool AltDown { get; private set; }
        public bool ShiftDown { get; set; }
        public Keys Key { get; private set; }

        public char KeyChar { get; private set; }
    }
}
