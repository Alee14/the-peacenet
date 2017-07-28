﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects
{
    public class Payload
    {
        public string FriendlyName { get; set; }
        public string PayloadName { get; set; }
        public int EffectiveAgainstFirewall { get; set; }
        public SystemType EffectiveAgainst { get; set; }
        public int Function { get; set; }
        public string Dependencies { get; set; }

        public string ID
        {
            get
            {
                return PayloadName.ToLower().Replace(" ", "_");
            }
        }

        public override string ToString()
        {
            return $"{FriendlyName} ({PayloadName})";
        }
    }

}