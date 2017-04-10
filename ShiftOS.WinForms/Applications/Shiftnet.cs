/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using Newtonsoft.Json;
using static ShiftOS.Engine.SkinEngine;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    //OH MY GOD. I HAD TO CORRECT HEAPS OF BADLY PLACED CURLY BRACES.
    //
    //I DO NOT CARE WHAT THE BLUE SMILEY FACE SAYS. CURLY BRACES BELONG ON THEIR OWN LINE.
    //
    //READ THE DAMN SHIFTOS CODING GUIDELINES.

    [Launcher("Shiftnet", true, "al_shiftnet", "Networking")]
    [MultiplayerOnly]
    [DefaultTitle("Shiftnet")]
    [WinOpen("shiftnet")]
    [RequiresUpgrade("victortran_shiftnet")]
    [DefaultIcon("iconShiftnet")]
    public partial class Shiftnet : UserControl, IShiftOSWindow
    {
        public Shiftnet()
        {
            InitializeComponent();
            ServerManager.MessageReceived += (msg) =>
            {
                try
                {
                    if (msg.Name == "shiftnet_file")
                    {
                        if (Objects.ShiftFS.Utils.FileExists("0:/md.txt"))
                        {
                            this.Invoke(new Action(() =>
                            {
                                wbcanvas.DocumentText = ConstructHtml(Objects.ShiftFS.Utils.ReadAllText("0:/md.txt"));
                            }));
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                wbcanvas.DocumentText = ConstructHtml(msg.Contents);
                            }));
                        }
                    }
                }
                catch
                {

                }
            };
        }

        public string ConstructHtml(string markdown)
        {
            var TerminalForeColor = ControlManager.ConvertColor(SkinEngine.LoadedSkin.TerminalForeColorCC);
            var TerminalBackColor = ControlManager.ConvertColor(SkinEngine.LoadedSkin.TerminalBackColorCC);
            string html = $@"<html>
    <head>
        <style>
            body {{
                background-color: rgb({LoadedSkin.ControlColor.R}, {LoadedSkin.ControlColor.G}, {LoadedSkin.ControlColor.B});
                color: rgb({LoadedSkin.ControlTextColor.R}, {LoadedSkin.ControlTextColor.G}, {LoadedSkin.ControlTextColor.B});
                font-family: ""{LoadedSkin.MainFont.Name}"";
                font-size: {LoadedSkin.MainFont.SizeInPoints}pt;
            }}

            h1 {{
                font-family: ""{LoadedSkin.HeaderFont.Name}"";
                font-size: {LoadedSkin.HeaderFont.SizeInPoints}pt;
            }}

            h2 {{
                font-family: ""{LoadedSkin.Header2Font.Name}"";
                font-size: {LoadedSkin.Header2Font.SizeInPoints}pt;
            }}

            h3 {{
                font-family: ""{LoadedSkin.Header3Font.Name}"";
                font-size: {LoadedSkin.Header3Font.SizeInPoints}pt;
            }}

            pre, code {{
                font-family: ""{LoadedSkin.TerminalFont.Name}"";
                font-size: {LoadedSkin.TerminalFont.SizeInPoints}pt;
                color: rgb({TerminalForeColor.R}, {TerminalForeColor.G}, {TerminalForeColor.B});
                background-color: rgb({TerminalBackColor.R}, {TerminalBackColor.G}, {TerminalBackColor.B});                
            }}
        </style>
    </head>
    <body>
        <markdown/>
    </body>
</html>";

            string body = CommonMark.CommonMarkConverter.Convert(markdown);
            for (int i = 0; i <= Encoding.UTF8.GetBytes(body).Length; i += DownloadManager.GetDownloadSpeed())
            {
                //halt the page load until 'download' finishes.
            }
            html = html.Replace("<markdown/>", body);
            return html;
        }

        public string CurrentUrl { get; set; }


        private void wbcanvas_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string Url = e.Url.ToString().Replace("http://", "");
            if (CurrentUrl != Url.ToString() && !Url.ToString().StartsWith("about:"))
            {
                e.Cancel = true;
                Future.Clear();
                if (Url.StartsWith("runsyscmd/"))
                {
                    ProcessShiftnetCmd(Url.Replace("runsyscmd/", ""));
                }

                ShiftnetNavigate(Url.ToString());
            }
        }

        public void ProcessShiftnetCmd(string cmd)
        {
            var args = cmd.Split('/');
            switch (args[0])
            {
                case "setsnsub":
                    for (int i = 0; i < DownloadManager.GetAllSubscriptions().Length; i++)
                    {
                        if (DownloadManager.GetAllSubscriptions()[i].Name == args[1])
                        {
                            var sub = DownloadManager.GetAllSubscriptions()[i];
                            Infobox.PromptYesNo("Shiftnet", $"Are you sure you want to switch your system's Shiftnet subscription to {sub.Name} by {sub.Company}?{Environment.NewLine}{Environment.NewLine}Cost per month: {sub.CostPerMonth} CP{Environment.NewLine}Download speed: {sub.DownloadSpeed} bytes per second", new Action<bool>((answer) =>
                            {
                                if (answer == true)
                                {
                                    if (SaveSystem.CurrentSave.Codepoints >= sub.CostPerMonth)
                                    {
                                        //Initial fee gets deducted.
                                        SaveSystem.CurrentSave.Codepoints -= sub.CostPerMonth;
                                        //Then we set the subscription.
                                        SaveSystem.CurrentSave.ShiftnetSubscription = i;
                                        //Then we say that we have paid this month.
                                        SaveSystem.CurrentSave.LastMonthPaid = DateTime.Now.Month;
                                        //Then we send our save to the MUD.
                                        SaveSystem.SaveGame();

                                    }
                                    else
                                    {
                                        //User can't afford this subscription.
                                        Infobox.Show("Shiftnet - Not enough Codepoints", $"You cannot afford to pay for this subscription at this time. You need {sub.CostPerMonth - SaveSystem.CurrentSave.Codepoints} more Codepoints.");
                                    }
                                }
                            }));
                        }
                    }
                    return;
            }
        }

        public Stack<string> History = new Stack<string>();
        public Stack<string> Future = new Stack<string>();

        public void ShiftnetNavigate(string Url, bool pushHistory = true)
        {
            if (Url.EndsWith(".rnp") || !Url.Contains("."))
            {
                if (!string.IsNullOrEmpty(CurrentUrl) && pushHistory)
                    History.Push(CurrentUrl);
                CurrentUrl = Url;
                ServerManager.SendMessage("shiftnet_get", JsonConvert.SerializeObject(new
                {
                    url = Url
                }));
                txturl.Text = Url;

            }
            else
            {
                ServerMessageReceived smr = null;
                smr = (msg) =>
                {
                    if (msg.Name == "download_meta")
                    {
                        var bytes = JsonConvert.DeserializeObject<byte[]>(msg.Contents);
                        string destPath = null;
                        string ext = Url.Split('.')[Url.Split('.').Length - 1];
                        this.Invoke(new Action(() =>
                        {
                            FileSkimmerBackend.GetFile(new[] { ext }, FileOpenerStyle.Save, new Action<string>((file) =>
                            {
                                destPath = file;
                            }));
                        }));
                        while (string.IsNullOrEmpty(destPath))
                        {

                        }
                        var d = new Download
                        {
                            ShiftnetUrl = Url,
                            Destination = destPath,
                            Bytes = bytes,
                            Progress = 0,
                        };
                        DownloadManager.StartDownload(d);
                        this.Invoke(new Action(() =>
                        {
                            AppearanceManager.SetupWindow(new Downloader());
                        }));
                        ServerManager.MessageReceived -= smr;
                    }
                };
                ServerManager.MessageReceived += smr;
                ServerManager.SendMessage("download_start", Url);
            }
        }

        public void OnLoad()
        {
            ShiftnetNavigate("shiftnet/main");
        }

        public void OnSkinLoad()
        {
            ShiftnetNavigate(CurrentUrl);
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            try
            {
                string hist = History.Pop();
                if (!string.IsNullOrEmpty(hist))
                {
                    Future.Push(hist);
                    ShiftnetNavigate(hist, false);
                }
            }
            catch
            {

            }
        }

        private void btnforward_Click(object sender, EventArgs e)
        {
            try
            {
                string fut = Future.Pop();
                if (!string.IsNullOrEmpty(fut))
                {
                    ShiftnetNavigate(fut);
                }
            }
            catch
            {

            }
        }

        private void btngo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txturl.Text))
            {
                Future.Clear();

                ShiftnetNavigate(txturl.Text);
            }
        }

        private void txturl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btngo_Click(sender, EventArgs.Empty);
                e.SuppressKeyPress = true;
            }
        }

        private void wbcanvas_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
        }
    }
}

