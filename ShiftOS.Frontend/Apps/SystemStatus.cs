﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Frontend.Desktop;
using ShiftOS.Frontend.GraphicsSubsystem;

namespace ShiftOS.Frontend.Apps
{
    [WinOpen("systemstatus")]
    [Launcher("System Status", false, null, "System")]
    [DefaultTitle("System Status")]
    [SidePanel]
    public class SystemStatus : GUI.Control, IShiftOSWindow
    {
        GUI.TextControl _header = null;
        GUI.TextControl _mainstatus = null;

        public SystemStatus()
        {
            Width = 720;
            Height = 480;
            _header = new GUI.TextControl();
            _mainstatus = new GUI.TextControl();
            AddControl(_header);
            AddControl(_mainstatus);
            _header.AutoSize = true;
            _header.Text = "System Status";
            
        }

        public void OnLoad()
        {
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        protected override void OnLayout()
        {
            _header.Font = SkinEngine.LoadedSkin.HeaderFont;
            _header.X = 20;
            _header.Y = 20;
            _mainstatus.X = 20;
            _mainstatus.Y = _header.Y + _header.Height + 10;
            _mainstatus.Width = Width - 40;
            _mainstatus.Height = Height - (_header.Y + _header.Height) - 40;
            _mainstatus.Text = $@"Codepoints: {SaveSystem.CurrentSave.Codepoints}
Upgrades: {SaveSystem.CurrentSave.CountUpgrades()} installed, {Shiftorium.GetDefaults().Count} available
Filesystems:
";

            foreach(var mount in Objects.ShiftFS.Utils.Mounts)
            {
                _mainstatus.Text += $" - {Objects.ShiftFS.Utils.Mounts.IndexOf(mount)}:/ ({mount.Name}){Environment.NewLine}"; 
            }
            _mainstatus.Text += $@"

Username: {SaveSystem.CurrentSave.Username}
System name: {SaveSystem.CurrentSave.SystemName}
RAM usage: 0MB/0MB <nyi>
Open programs: {AppearanceManager.OpenForms.Count}";
        }

        protected override void OnPaint(GraphicsContext gfx)
        {
            base.OnPaint(gfx);

        }
    }
}