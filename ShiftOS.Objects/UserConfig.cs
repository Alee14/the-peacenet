﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Plex.Objects
{

    public class FriendlyDescription : Attribute
    {
        public FriendlyDescription(string desc)
        {
            Description = desc;
        }

        public string Description { get; private set; }
    }

    public class UserConfig
    {
        public List<ServerDetails> Servers { get; set; }
        public Dictionary<string, string> SessionCache { get; set; }

        public string Language { get; set; }
        public string DigitalSocietyAddress { get; set; }
        public int DigitalSocietyPort { get; set; }
        public int ScreenWidth = 1920;
        public int ScreenHeight = 1080;
        public bool Fullscreen = true;
        public bool SuppressTypeLoadErrors = true;

        private static UserConfig def = new UserConfig
        {
            SessionCache = new Dictionary<string, string>(),
            Language = "english",
            DigitalSocietyAddress = "getPlex.net",
            DigitalSocietyPort = 13370,
            Fullscreen = true,
            ScreenWidth = 1920,
            ScreenHeight = 1080,
            Servers = new List<ServerDetails>(),
#if DEBUG
            SuppressTypeLoadErrors = false
#else
            SuppressTypeLoadErrors = true
#endif
        };

        public static UserConfig current = null;

    public static UserConfig Get()
        {
            if (current != null)
                return current;
            if (File.Exists("config.json"))
                current = JsonConvert.DeserializeObject<UserConfig>(File.ReadAllText("config.json"));
            else
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(def, Formatting.Indented));
                current = def;
            }
            if (current.SessionCache == null)
                current.SessionCache = new Dictionary<string, string>();
            return current;
        }
    }
}
