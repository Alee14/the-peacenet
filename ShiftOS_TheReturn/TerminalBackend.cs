using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;
using static Plex.Engine.SaveSystem;

namespace Plex.Engine
{
    /// <summary>
    /// Backend for the Plex terminal.
    /// </summary>
    public static class TerminalBackend
    {
        private static string _shellOverrideString = "";
        public static string RawShellOverride
        {
            get
            {
                return _shellOverrideString;
            }
        }
        
        public static string GetSystemShell()
        {
            return $"{SaveSystem.GetUsername()}@{GetSystemName()}:~$ ";
        }

        /// <summary>
        /// Gets the current shell prompt override string.
        /// </summary>
        public static string ShellOverride
        {
            get
            {
                return (string.IsNullOrWhiteSpace(_shellOverrideString)) ? GetSystemShell() : _shellOverrideString;
            }
        }

        /// <summary>
        /// Sets the shell override string to the specified value. Empty string or <see cref="null"/> to use the default Plex string. 
        /// </summary>
        /// <param name="value">The string to use as a shell prompt.</param>
        public static void SetShellOverride(string value)
        {
            _shellOverrideString = value;
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
        public class AllowInMultiplayerAttribute : Attribute
        {
            public bool Allow { get; private set; }

            public AllowInMultiplayerAttribute(bool allow)
            {
                Allow = allow;
            }
        }

        /// <summary>
        /// Occurs when a command is processed.
        /// </summary>
        public static event Action<string, string> CommandProcessed;

        /// <summary>
        /// Gets or sets whether the current command is elevated.
        /// </summary>
        public static bool Elevated { get; set; }

        /// <summary>
        /// Parses command-line arguments using the Plex syntax and stores them in a <see cref="Dictionary{string, string}"/>, removing the parsed text from the original string.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns><see cref="Dictionary{string, string}"/> containing the parsed arguments.</returns>
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

        /// <summary>
        /// String representing the last command entered by the user.
        /// </summary>
        public static string LastCommand = "";

        /// <summary>
        /// Gets the output of the last command.
        /// </summary>
        public static string LastCommandBuffer { get; private set; }

        /// <summary>
        /// Invokes a Plex terminal command.
        /// </summary>
        /// <param name="command">The command name.</param>
        /// <param name="arguments">The command arguments.</param>
        /// <param name="isRemote">Whether the command should be sent through Remote Terminal Session (RTS).</param>
        public static void InvokeCommand(string command, Dictionary<string, string> arguments, bool isRemote = false)
        {
            try
            {
                bool commandWasClient = RunClient(command, arguments, isRemote);

                if (!commandWasClient)
                {
                    Console.WriteLine("{ERR_COMMANDNOTFOUND}");
                }

                CommandProcessed?.Invoke(command, JsonConvert.SerializeObject(arguments));
                CommandFinished?.Invoke(command, JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(arguments)));
            }
            catch (Exception ex)
            {
                Console.WriteLine("{ERR_SYNTAXERROR}");
                PrefixEnabled = true;

            }
        }

        /// <summary>
        /// Transforms a <see cref="Dictionary{String, String}"/> of arguments to a <see cref="Dictionary{String, Object}"/>.  
        /// </summary>
        /// <param name="argss">The original argument dictionary to convert.</param>
        /// <returns>The converted dictionary.</returns>
        public static string GetSentArgs(Dictionary<string, string> argss)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            foreach (KeyValuePair<string, string> arg in argss)
            {
                args[arg.Key] = arg.Value;
            }
            return JsonConvert.SerializeObject(args);
        }

        [MetaCommand]
        [Command("exit")]
        public static void Exit()
        {
            if (_shellOverrideString != "")
                _shellOverrideString = "";
            else
            {
                Console.WriteLine("error: cannot exit system shell");
            }
        }


        public class WinOpenCommand : TerminalCommand
        {
            public Type PlexWindow { get; set; }

            public override bool MatchShell(string shell)
            {
                return (shell == "");
            }


            public override void Invoke(Dictionary<string, object> args, string shell)
            {
                AppearanceManager.SetupWindow((IPlexWindow)Activator.CreateInstance(PlexWindow, null));
            }


        }

        public static event Action<string, Dictionary<string, object>> CommandFinished;


        public static List<TerminalCommand> Commands { get; private set; }

        public static void PopulateTerminalCommands()
        {
            Commands = new List<TerminalCommand>();
            foreach (var type in ReflectMan.Types)
            {
                if (type.GetInterfaces().Contains(typeof(IPlexWindow)))
                {
                    var winopenattrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is WinOpenAttribute) as WinOpenAttribute;
                    if (winopenattrib != null)
                    {
                        var winc = new WinOpenCommand();
                        winc.CommandType = type;
                        var rupg = type.GetCustomAttributes().FirstOrDefault(x => x is RequiresUpgradeAttribute) as RequiresUpgradeAttribute;
                        if (rupg != null)
                            winc.Dependencies = rupg.Upgrade;
                        winc.CommandInfo = new Command(winopenattrib.ID, "", "Opens the \"" + winopenattrib.ID + " program.");
                        winc.RequiredArguments = new List<string>();
                        winc.RequiresElevation = false;
                        winc.PlexWindow = type;

                        var ambiguity = Commands.FirstOrDefault(x => x.CommandInfo.name == winc.CommandInfo.name);
                        if (ambiguity != null)
                            throw new Exception("Ambiguity error. The program " + winc.CommandInfo.name + " collides with another program or terminal command with the same name. Please either change the already-existing program/command, or change this one's WinOpenAttribute value to compensate.");
                        Commands.Add(winc);
                    }
                
                }
                foreach (var mth in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    
                    var cmd = mth.GetCustomAttributes(false).FirstOrDefault(x => x.GetType() ==  typeof(Command)); //If we don't do it this shitty way, the game picks up server commands.
                    if (cmd != null)
                    {
                        var tc = new TerminalCommand();
                        var mpattr = mth.GetCustomAttributes(false).FirstOrDefault(x => x is AllowInMultiplayerAttribute) as AllowInMultiplayerAttribute;
                        if (mpattr != null)
                            tc.AllowInMP = mpattr.Allow;
                        else
                            tc.AllowInMP = true;

                        tc.RequiresElevation = !(type.GetCustomAttributes(false).FirstOrDefault(x => x is KernelModeAttribute) == null);

                        var shellConstraint = mth.GetCustomAttributes(false).FirstOrDefault(x => x is ShellConstraintAttribute) as ShellConstraintAttribute;
                        tc.ShellMatch = (shellConstraint == null) ? "" : shellConstraint.Shell;

                        if(mth.GetCustomAttributes(false).FirstOrDefault(x=>x is MetaCommandAttribute) != null)
                        {
                            tc.ShellMatch = "metacmd";
                        }

                        tc.CommandInfo = cmd as Command;
                        tc.RequiresElevation = tc.RequiresElevation || !(mth.GetCustomAttributes(false).FirstOrDefault(x => x is KernelModeAttribute) == null);
                        tc.RequiredArguments = new List<string>();
                        foreach (var arg in mth.GetCustomAttributes(false).Where(x => x is RequiresArgument))
                        {
                            var rarg = arg as RequiresArgument;
                            tc.RequiredArguments.Add(rarg.argument);
                        }
                        var rupg = mth.GetCustomAttributes(false).FirstOrDefault(x => x is RequiresUpgradeAttribute) as RequiresUpgradeAttribute;
                        if (rupg != null)
                            tc.Dependencies = rupg.Upgrade;
                        else
                            tc.Dependencies = "";
                        tc.CommandType = type;
                        tc.CommandHandler = mth;

                        var ambiguity = Commands.FirstOrDefault(x => x.CommandInfo.name == tc.CommandInfo.name);
                        if (ambiguity != null)
                            throw new Exception("Command ambiguity error. You can't have two commands with the same name: " + $"{tc} == {ambiguity}");

                        if (!Commands.Contains(tc))
                            Commands.Add(tc);
                    }
                }

            }
            Console.WriteLine("[termdb] " + Commands.Count + " commands found.");
        }

        /// <summary>
        /// Invokes a Plex terminal command.
        /// </summary>
        /// <param name="text">The full command text in regular Plex syntax</param>
        /// <param name="isRemote">Whether the command should be sent through Remote Terminal Session (RTS).</param>
        public static void InvokeCommand(string text, bool isRemote = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            var args = GetArgs(ref text);
            var oargs = JsonConvert.DeserializeObject<Dictionary<string, object>>(GetSentArgs(args));
            try
            {

                bool commandWasClient = RunClient(text, args, isRemote);
                if (!commandWasClient)
                {
                    Console.WriteLine("Error: Command not found.");
                    
                }
                CommandProcessed?.Invoke(text, GetSentArgs(args));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Command parse error: {ex.Message}");
                PrefixEnabled = true;

            }
        }

        /// <summary>
        /// Gets or sets whether the user prefix is printed after a command completes.
        /// </summary>
        public static bool PrefixEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the user is in a story plot, and thus, the terminal input should be disabled.
        /// </summary>
        public static bool InStory { get; set; }

        /// <summary>
        /// Another latest command string.
        /// </summary>
        public static string latestCommmand = "";

        /// <summary>
        /// Occurs when the engine requests a Terminal to be open.
        /// </summary>
        public static event EmptyEventHandler TerminalRequested;

        /// <summary>
        /// Opens a Terminal.
        /// </summary>
        internal static void OpenTerminal()
        {
            TerminalRequested?.Invoke();
        }

        /// <summary>
        /// Determines if the specified command method can be ran in RTS
        /// </summary>
        /// <param name="mth">The method to scan</param>
        /// <param name="isRemote">Is the user in an RTS session?</param>
        /// <returns>Whether the command can be run.</returns>
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

        private static string _terminal_forward_session_id = "";

        /// <summary>
        /// Runs a command on the client.
        /// </summary>
        /// <param name="text">The command text.</param>
        /// <param name="argss">The command arguments.</param>
        /// <param name="isRemote">Whether the command should be ran through RTS.</param>
        /// <returns>Whether the command ran successfully.</returns>
        public static bool RunClient(string text, Dictionary<string, string> argss, bool isRemote = false)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            foreach (KeyValuePair<string, string> arg in argss)
            {
                args[arg.Key] = arg.Value;
            }
            return RunClient(text, args, isRemote);
        }

        [ClientMessageHandler("trm_write")]
        public static void TRMWrite(string content, string ip)
        {
            if (AppearanceManager.ConsoleOut != null)
                AppearanceManager.ConsoleOut.Write(content);
        }

        [ClientMessageHandler("trm_writeline")]
        public static void TRMWriteLine(string content, string ip)
        {
            if (AppearanceManager.ConsoleOut != null)
                AppearanceManager.ConsoleOut.WriteLine(content);
        }


        [ClientMessageHandler("trm_done")]
        public static void TerminalDone(string content, string ip)
        {
            InStory = false;
            PrefixEnabled = true;
            PrintPrompt();
            CommandFinished?.Invoke(_ranCMD, _ranArgs);
        }

        [ClientMessageHandler("trm_esyschange")]
        public static void ExitSyschange(string content, string ip)
        {
            _terminal_forward_session_id = null;
        }

        [ClientMessageHandler("trm_syschange")]
        public static void SysChange(string content, string ip)
        {
            _terminal_forward_session_id = content;
        }

        private static string _ranCMD = "";
        private static Dictionary<string, object> _ranArgs = null;

        /// <summary>
        /// Runs a command on the client.
        /// </summary>
        /// <param name="text">The command text.</param>
        /// <param name="args">The command arguments.</param>
        /// <param name="isRemote">Whether the command should be run in RTS.</param>
        /// <returns>Whether the command ran successfully.</returns>
        public static bool RunClient(string text, Dictionary<string, object> args, bool isRemote = false)
        {
            latestCommmand = text;

            //Console.WriteLine(text + " " + "{" + string.Join(",", args.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}" + " " + isRemote);
            bool value = true;

            var cmd = Commands.FirstOrDefault(x => Localization.Parse(x.CommandInfo.name) == text);
            if (cmd == null)
                value = false;
            else if (!Upgrades.UpgradeInstalled(cmd.Dependencies))
                value = false;
            if (!string.IsNullOrWhiteSpace(_terminal_forward_session_id))
                value = false;
            if (value == true)
            {
                bool res = false;
                foreach (var arg in cmd.RequiredArguments)
                {
                    if (!args.ContainsKey(arg))
                    {
                        res = true;
                        Console.WriteLine("You are missing an argument with the key \"" + arg + "\".");
                    }
                }
                if (res == true)
                    return true;
                try
                {
                    cmd.Invoke(args, _shellOverrideString);
                    CommandFinished?.Invoke(text, args);
                }
                catch (TargetInvocationException ex)
                {
                    Console.WriteLine("Command error: " + ex.InnerException.Message);
                }

                return true;
            }
            else
            {
                if (isRemote == false)
                {
                    InStory = true;
                    PrefixEnabled = false;
                    _ranCMD = text;
                    _ranArgs = args;
                    using (ServerStream sstr = new ServerStream(ServerMessageType.TRM_INVOKE))
                    {
                        sstr.Write(JsonConvert.SerializeObject(new
                        {
                            cmd = text,
                            args = args,
                            shell = _shellOverrideString,
                            sessionfwd = _terminal_forward_session_id
                        }));

                        var result = sstr.Send();
                        if (result.Message == (byte)ServerResponseType.REQ_SUCCESS)
                        {
                            using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                            {
                                while (true)
                                {

                                    string ln = reader.ReadString();
                                    if (ln == "\u0013\u0014")
                                        break;
                                    Console.Write(ln);
                                }
                            }
                        }
                        return true;
                    }
                }
            }
            return value;
        }

#if DEBUG
        [Command("setshell", hide = true)]
        [RequiresArgument("id")]
        public static void Debug_SetShellOverrideCMD(Dictionary<string, object> args)
        {
            SetShellOverride(args["id"].ToString());
        }
#endif

        /// <summary>
        /// Prints the user prompt to the terminal.
        /// </summary>
        public static void PrintPrompt()
        {
            if (PrefixEnabled)
            {
                Console.Write(ShellOverride);
                ConsoleEx.Flush();
            }
        }

        /// <summary>
        /// Gets whether the terminal backend is forwarding console write requests through RTS to a remote client.
        /// </summary>
        public static bool IsForwardingConsoleWrites { get; internal set; }

        /// <summary>
        /// Gets the RTS forward GUID.
        /// </summary>
        public static string ForwardGUID { get; internal set; }

        /// <summary>
        /// Occurs when the user inputs text in a Terminal.
        /// </summary>
        public static event TextSentEventHandler TextSent;

        /// <summary>
        /// Fakes the user inputting text to a Terminal.
        /// </summary>
        /// <param name="text">The text to input.</param>
        public static void SendText(string text)
        {
            TextSent?.Invoke(text);
        }

    }

}
