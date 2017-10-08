﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Frontend.GraphicsSubsystem;
using Plex.Frontend.GUI;

namespace Plex.Frontend.Apps
{
    [DefaultTitle("Graphic viewer")]
    [FileHandler("PNG image", ".png", "image")]
    [FileHandler("JPEG image", ".jpeg", "image")]
    [FileHandler("Bitmap", ".bmp", "image")]
    public class GraphicViewer : Control, IPlexWindow, IFileHandler
    {
        private Texture2D _imgTexture = null;
        private PictureBox _imgPreview = null;

        public GraphicViewer()
        {
            _imgPreview = new PictureBox();

            AddControl(_imgPreview);
        }

        public void OnLoad()
        {
            _imgPreview.Image = _imgTexture;
            _imgPreview.ImageLayout = System.Windows.Forms.ImageLayout.Zoom;
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            _imgTexture.Dispose();
            return true;
        }

        public void OnUpgrade()
        {
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _imgPreview.X = 0;
            _imgPreview.Y = 0;
            _imgPreview.Width = Width;
            _imgPreview.Height = Height;
            base.OnLayout(gameTime);
        }

        public void OpenFile(string file)
        {
            byte[] data = FSUtils.ReadAllBytes(file);
            using(var img = SkinEngine.ImageFromBinary(data))
            {
                _imgTexture = img.ToTexture2D(UIManager.GraphicsDevice);
            }
            AppearanceManager.SetupWindow(this);
        }
    }
}
