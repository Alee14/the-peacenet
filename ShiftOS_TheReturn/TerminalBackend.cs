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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static ShiftOS.Engine.SaveSystem;

namespace ShiftOS.Engine
{
    public static class TerminalBackend
    {
        public static event Action<string, string> CommandProcessed;

        public static bool Elevated { get; set; }

        public static Dictionary<string, string> GetArgs(ref string text)
        {
            bool shouldParse = false;
            int argStart = 0;
            if (text.Contains("{"))
            {
                shouldParse = true;
                argStart = text.IndexOf('{');
            }

            if (shouldParse == false)
            {
                string replacement = Regex.Replace(text, @"\t|\n|\r", "");
                text = replacement + "{}";
                shouldParse = true;
                argStart = text.IndexOf('{');
            }

            string args = text.Substring(argStart, text.Length - argStart);

            text = text.Remove(argStart, text.Length - argStart).Replace(" ", "");
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(args);
        }

        public static string LastCommand = "";

        public static void InvokeCommand(string ns, string command, Dictionary<string, string> arguments, bool isRemote = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ns))
                    return;


                bool commandWasClient = RunClient(ns, command, arguments, isRemote);

                if (!commandWasClient && !string.IsNullOrWhiteSpace(ns))
                {
                    PrefixEnabled = false;

                    ServerManager.SendMessage("script", $@"{{
    user: ""{ns}"",
    script: ""{command}"",
    args: ""{GetSentArgs(arguments)}""
}}");
                }

                CommandProcessed?.Invoke(ns + "." + command, JsonConvert.SerializeObject(arguments));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Command parse error: {ex.Message}"); // This shouldn't ever be called now
                PrefixEnabled = true;

            }
        }

        public static string GetSentArgs(Dictionary<string, string> argss)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            foreach (KeyValuePair<string, string> arg in argss)
            {
                args[arg.Key] = arg.Value;
            }
            return JsonConvert.SerializeObject(args);
        }

        public static void InvokeCommand(string text, bool isRemote = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return;

                var args = GetArgs(ref text);

                bool commandWasClient = RunClient(text, args, isRemote);

                if (!commandWasClient)
                {
                    PrefixEnabled = false;

                    ServerManager.SendMessage("script", $@"{{
    user: ""{text.Split('.')[0]}"",
    script: ""{text.Split('.')[1]}"",
    args: ""{GetSentArgs(args)}""
}}");
                }
                CommandProcessed?.Invoke(text, GetSentArgs(args));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Command parse error: {ex.Message}");
                PrefixEnabled = true;

            }
        }

        public static bool PrefixEnabled { get; set; }

        public static bool InStory { get; set; }

        public static string latestCommmand = "";

        public static event EmptyEventHandler TerminalRequested;

        internal static void OpenTerminal()
        {
            TerminalRequested?.Invoke();
        }

        public static bool CanRunRemotely(MethodInfo mth, bool isRemote)
        {
            if (!isRemote)
                return true;

            foreach (var attr in mth.GetCustomAttributes(false))
            {
                if (attr is RemoteLockAttribute)
                    return false;
            }

            return true;
        }

        public static bool RunClient(string ns, string cmd, Dictionary<string, string> args, bool isRemote = false)
        {
            return RunClient(ns + "." + cmd, args, isRemote);
        }


        public static bool RunClient(string text, Dictionary<string, string> argss, bool isRemote = false)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            foreach (KeyValuePair<string, string> arg in argss)
            {
                args[arg.Key] = arg.Value;
            }
            return RunClient(text, args, isRemote);
        }

        public static bool RunClient(string text, Dictionary<string, object> args, bool isRemote = false)
        {
            latestCommmand = text;

            //Console.WriteLine(text + " " + "{" + string.Join(",", args.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}" + " " + isRemote);

            foreach (var asmExec in System.IO.Directory.GetFiles(Environment.CurrentDirectory))
            {
                try
                {
                    var asm = Assembly.LoadFile(asmExec);

                    var types = asm.GetTypes();
                    foreach (var type in types)
                    {
                        if (Shiftorium.UpgradeAttributesUnlocked(type))
                        {
                            foreach (var a in type.GetCustomAttributes(false))
                            {
                                if (a is Namespace)
                                {
                                    var ns = a as Namespace;
                                    if (text.Split('.')[0] == ns.name)
                                    {
                                        if (KernelWatchdog.IsSafe(type))
                                        {
                                            if (KernelWatchdog.CanRunOffline(type))
                                            {
                                                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                                                {
                                                    if (Shiftorium.UpgradeAttributesUnlocked(method))
                                                    {
                                                        if (CanRunRemotely(method, isRemote))
                                                        {
                                                            foreach (var ma in method.GetCustomAttributes(false))
                                                            {
                                                                if (ma is Command)
                                                                {
                                                                    var cmd = ma as Command;
                                                                    if (text.Split('.')[1] == cmd.name)
                                                                    {
                                                                        if (KernelWatchdog.IsSafe(method))
                                                                        {
                                                                            if (KernelWatchdog.CanRunOffline(method))
                                                                            {
                                                                                var attr = method.GetCustomAttribute<CommandObsolete>();

                                                                                if (attr != null)
                                                                                {
                                                                                    string newcommand = attr.newcommand;
                                                                                    if (attr.warn)
                                                                                    {
                                                                                        Console.WriteLine(Localization.Parse((newcommand == "" ? "{ERROR}" : "{WARN}") + attr.reason, new Dictionary<string, string>() {
                                                                {"%newcommand", newcommand}
                                                            }));
                                                                                    }
                                                                                    if (newcommand != "")
                                                                                    {
                                                                                        // redo the entire process running newcommand

                                                                                        return RunClient(newcommand, args);
                                                                                    }
                                                                                }

                                                                                var requiresArgs = method.GetCustomAttributes<RequiresArgument>();
                                                                                bool error = false;
                                                                                bool providedusage = false;

                                                                                foreach (RequiresArgument argument in requiresArgs)
                                                                                {
                                                                                    if (!args.ContainsKey(argument.argument))
                                                                                    {

                                                                                        if (!providedusage)
                                                                                        {
                                                                                            string usageparse = "{COMMAND_" + ns.name.ToUpper() + "_" + cmd.name.ToUpper() + "_USAGE}";
                                                                                            if (usageparse == Localization.Parse(usageparse))
                                                                                                usageparse = "";
                                                                                            else
                                                                                                usageparse = Shiftorium.UpgradeInstalled("help_usage") ? Localization.Parse("{ERROR}{USAGE}" + usageparse, new Dictionary<string, string>() {
                                                                        {"%ns", ns.name},
                                                                        {"%cmd", cmd.name}
                                                                    }) : "";

                                                                                            Console.WriteLine(usageparse);

                                                                                            providedusage = true;
                                                                                        }
                                                                                        if (Shiftorium.UpgradeInstalled("help_usage"))
                                                                                        {
                                                                                            Console.WriteLine(Localization.Parse("{ERROR_ARGUMENT_REQUIRED}", new Dictionary<string, string>() {
                                                                    {"%argument", argument.argument}
                                                                }));
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Console.WriteLine(Localization.Parse("{ERROR_ARGUMENT_REQUIRED_NO_USAGE}"));
                                                                                        }

                                                                                        error = true;
                                                                                    }
                                                                                }

                                                                                if (error)
                                                                                {
                                                                                    throw new Exception("{ERROR_COMMAND_WRONG}");
                                                                                }

                                                                                try
                                                                                {
                                                                                    return (bool)method.Invoke(null, new[] { args });
                                                                                }
                                                                                catch (TargetInvocationException e)
                                                                                {
                                                                                    Console.WriteLine(Localization.Parse("{ERROR_EXCEPTION_THROWN_IN_METHOD}"));
                                                                                    Console.WriteLine(e.InnerException.Message);
                                                                                    Console.WriteLine(e.InnerException.StackTrace);
                                                                                    return true;
                                                                                }
                                                                                catch
                                                                                {
                                                                                    return (bool)method.Invoke(null, new object[] { });
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.Write("<");
                                                                                ConsoleEx.Bold = true;
                                                                                ConsoleEx.ForegroundColor = ConsoleColor.DarkRed;
                                                                                Console.Write("session_mgr");
                                                                                ConsoleEx.ForegroundColor = SkinEngine.LoadedSkin.TerminalForeColorCC;
                                                                                ConsoleEx.Bold = false;
                                                                                Console.Write(">");
                                                                                ConsoleEx.Italic = true;
                                                                                ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
                                                                                Console.WriteLine(" You cannot run this command while disconnected from the multi-user domain..");
                                                                                return true;

                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.Write("<");
                                                                            ConsoleEx.Bold = true;
                                                                            ConsoleEx.ForegroundColor = ConsoleColor.DarkRed;
                                                                            Console.Write("watchdog");
                                                                            ConsoleEx.ForegroundColor = SkinEngine.LoadedSkin.TerminalForeColorCC;
                                                                            ConsoleEx.Bold = false;
                                                                            Console.Write(">");
                                                                            ConsoleEx.Italic = true;
                                                                            ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
                                                                            Console.WriteLine(" You cannot run this command.");
                                                                            KernelWatchdog.Log("potential_sys_breach", "user attempted to run kernel mode command " + text + " - watchdog has prevented this, good sir.");
                                                                            return true;
                                                                        }
                                                                    }


                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine(text + " cannot be ran in a remote session");
                                                            return true;
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                Console.Write("<");
                                                ConsoleEx.Bold = true;
                                                ConsoleEx.ForegroundColor = ConsoleColor.DarkRed;
                                                Console.Write("session_mgr");
                                                ConsoleEx.ForegroundColor = SkinEngine.LoadedSkin.TerminalForeColorCC;
                                                ConsoleEx.Bold = false;
                                                Console.Write(">");
                                                ConsoleEx.Italic = true;
                                                ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
                                                Console.WriteLine(" You cannot run this command while disconnected from the multi-user domain..");
                                                return true;

                                            }
                                        }
                                        else
                                        {

                                            Console.Write("<");
                                            ConsoleEx.Bold = true;
                                            ConsoleEx.ForegroundColor = ConsoleColor.DarkRed;
                                            Console.Write("watchdog");
                                            ConsoleEx.ForegroundColor = SkinEngine.LoadedSkin.TerminalForeColorCC;
                                            ConsoleEx.Bold = false;
                                            Console.Write(">");
                                            ConsoleEx.Italic = true;
                                            ConsoleEx.ForegroundColor = ConsoleColor.DarkYellow;
                                            Console.WriteLine(" You cannot run this command.");
                                            KernelWatchdog.Log("potential_sys_breach", "user attempted to run kernel mode command " + text + " - watchdog has prevented this, good sir.");
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }
            return false;
        }
        public static void PrintPrompt()
        {
            if (SaveSystem.CurrentSave != null)
            {
                ConsoleEx.BackgroundColor = SkinEngine.LoadedSkin.TerminalBackColorCC;
                ConsoleEx.Italic = false;
                ConsoleEx.Underline = false;

                ConsoleEx.ForegroundColor = ConsoleColor.Magenta;
                ConsoleEx.Bold = true;

                Console.Write(SaveSystem.CurrentUser.Username);
                ConsoleEx.Bold = false;
                ConsoleEx.ForegroundColor = ConsoleColor.Gray;
                Console.Write("@");
                ConsoleEx.Italic = true;
                ConsoleEx.Bold = true;
                ConsoleEx.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(SaveSystem.CurrentSave.SystemName);
                ConsoleEx.Italic = false;
                ConsoleEx.Bold = false;
                ConsoleEx.ForegroundColor = ConsoleColor.Gray;
                Console.Write(":~");
                Console.ForegroundColor = ConsoleColor.White;
                ConsoleEx.Italic = true;
                if (KernelWatchdog.InKernelMode == true)
                    Console.Write("#");
                else
                    Console.Write("$");
                ConsoleEx.Italic = false;
                ConsoleEx.Bold = false;
                ConsoleEx.ForegroundColor = SkinEngine.LoadedSkin.TerminalForeColorCC;
                Console.Write(" ");
            }
        }


        static TerminalBackend()
        {
            ServerMessageReceived onMessageReceived = (msg) =>
            {
                if (msg.Name == "trm_invokecommand")
                {
                    string text3 = "";
                    string text4 = msg.Contents;

                    if (TerminalBackend.PrefixEnabled)
                    {
                        text3 = text4.Remove(0, $"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ".Length);
                    }
                    IsForwardingConsoleWrites = true;
                    if (TerminalBackend.InStory == false)
                    {
                        TerminalBackend.InvokeCommand(text3, true);
                    }
                    if (TerminalBackend.PrefixEnabled)
                    {
                        Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                    }
                    IsForwardingConsoleWrites = false;
                }
                else if (msg.Name == "pleasewrite")
                {
                    Console.Write(msg.Contents);
                }
                else if (msg.Name == "handshake_from")
                {
                    var a = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg.Contents);
                    string uName = a["username"] as string;
                    string pass = a["password"] as string;
                    string sys = a["sysname"] as string;
                    string guid = msg.GUID;
                    if (SaveSystem.CurrentSave.Username == uName && SaveSystem.CurrentSave.Password == pass && CurrentSave.SystemName == sys)
                    {
                        ForwardGUID = guid;
                        ServerManager.SendMessage("trm_handshake_accept", $@"{{
    guid: ""{ServerManager.thisGuid}"",
    target: ""{guid}""
}}");

                        IsForwardingConsoleWrites = true;
                        InvokeCommand("sos.status");
                        Console.Write($"{SaveSystem.CurrentSave.Username}@{SaveSystem.CurrentSave.SystemName}:~$ ");
                        IsForwardingConsoleWrites = false;
                    }
                }
            };

            ServerManager.MessageReceived += onMessageReceived;
        }

        public static bool IsForwardingConsoleWrites { get; internal set; }
        public static string ForwardGUID { get; internal set; }

        public static event TextSentEventHandler TextSent;

        public static void SendText(string text)
        {
            TextSent?.Invoke(text);
        }

    }
}
