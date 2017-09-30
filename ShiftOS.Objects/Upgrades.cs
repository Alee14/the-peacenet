﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Objects
{

    /// <summary>
    /// Marks this command so that it can be run in ANY shell.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MetaCommandAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ShellConstraintAttribute : Attribute
    {
        /// <summary>
        /// Instructs the terminal command interpreter to disallow running of this command unless the user shell override matches up with the value provided here.
        /// </summary>
        /// <param name="shell">The required shell string. Null or whitespace to match with the default Plex shell.</param>
        public ShellConstraintAttribute(string shell)
        {
            Shell = shell;
        }


        /// <summary>
        /// Gets the required shell string for the command.
        /// </summary>
        public string Shell { get; private set; }
    }


    public class ShiftoriumConflictException : Exception
    {
        public ShiftoriumConflictException() : base("An upgrade conflict has occurred while loading Shiftorium Upgrades from an assembly. Is there a duplicate upgrade ID?")
        {

        }

        public ShiftoriumConflictException(string id) : base("An upgrade conflict has occurred while loading Shiftorium Upgrades from an assembly. An upgrade with the ID \"" + id + "\" has already been loaded.")
        {

        }
    }


    public class ShiftoriumUpgradeAttribute : RequiresUpgradeAttribute
    {
        public ShiftoriumUpgradeAttribute(string name, ulong cost, string desc, string dependencies, string category, bool purchasable) : base(name.ToLower().Replace(" ", "_"))
        {
            Name = name;
            Description = desc;
            Dependencies = dependencies;
            Cost = cost;
            Category = category;
            Purchasable = purchasable;
        }

        public bool Purchasable { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ulong Cost { get; private set; }
        public string Dependencies { get; private set; }
        public string Category { get; private set; }
    }

    public class MemoryTextWriter : System.IO.TextWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }

        private StringBuilder sb = null;

        public MemoryTextWriter()
        {
            sb = new StringBuilder();
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        public override void Write(char value)
        {
            sb.Append(value);
        }

        public override void WriteLine()
        {
            sb.AppendLine();
        }

        public override void Write(string value)
        {
            sb.Append(value);
        }

        public override void Close()
        {
            sb.Clear();
            sb = null;
            base.Close();
        }

        public override void WriteLine(string value)
        {
            sb.AppendLine(value);
        }
    }












    /// <summary>






        /// <summary>









        /// <summary>
}