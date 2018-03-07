﻿/*













































using System;

namespace Plex.Engine
            {
                if (scanned)
                    throw new Exception("PythonAPI.Scan() called multiple times");
                scopes = new Dictionary<string, ScriptScope>();
                var resman = new System.Resources.ResourceManager("Plex.Engine.Properties.Resources", typeof(Properties.Resources).Assembly);
                var provider = new CSharpCodeProvider();
                var refs = AppDomain.CurrentDomain.GetAssemblies().Select(f => f.Location).Concat(new string[] { "Microsoft.CSharp.dll" }).ToArray();
                var types = new List<Type>();
                var sha = new SHA512Managed();
                var oldcache = new Dictionary<string, AsmCacheEntry>();
                var newcache = new Dictionary<string, AsmCacheEntry>();
                if (File.Exists("pyasmcache.dat"))
                    using (var stream = File.OpenRead("pyasmcache.dat"))
                        try
                        {
                            oldcache = AsmCache.Load(stream);
                        }
                        catch (Exception ex)
                        {


#if DEBUG
                            Console.WriteLine(ex.ToString());


#endif
                foreach (var fname in Directory.GetFiles(Environment.CurrentDirectory, "*.py").Select(Path.GetFileName))
                {
                    byte[] checksum;
                    using (FileStream stream = File.OpenRead(fname))
                        checksum = sha.ComputeHash(stream);
                    var script = File.ReadAllText(fname);
                    try
                    {
                        scopes[fname] = PythonHelper.RunCode(script);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[dev] Failed to execute Python script " + fname);
                        Console.WriteLine(ex.ToString());
                        continue;
                    }
                    if (oldcache.ContainsKey(fname))
                    {
                        var oldentry = oldcache[fname];
                        if (checksum.SequenceEqual(oldentry.checksum))
                        {
                            try
                            {
                                foreach (var asm in oldentry.asms)
                                    types.AddRange(Assembly.Load(asm).GetTypes());
                                newcache.Add(fname, oldentry);
                                continue;
                            }
                            catch (Exception ex)
                            {


#if DEBUG
                                Console.WriteLine(ex.ToString());


#endif
                        }
                    }
                    var scriptlines = script.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n'); // line-ending independent...
                    int pos = 0;
                    var entry = new AsmCacheEntry();
                    entry.checksum = checksum;
                    var parameters = new CompilerParameters();
                    parameters.ReferencedAssemblies.AddRange(refs);
                    parameters.GenerateInMemory = false; // We need to keep the temporary file around long enough to copy it to the cache.
                    parameters.GenerateExecutable = false;
                    try
                    {
                        while (pos < scriptlines.Length)
                        {
                            while (!scriptlines[pos].StartsWith("#Plex"))
                                pos++;
                            var templatename = scriptlines[pos].Split(':')[1];
                            pos++;
                            string decorators = "";
                            while (scriptlines[pos].StartsWith("#"))
                            {
                                decorators += scriptlines[pos].Substring(1) + Environment.NewLine; // remove # and add to string
                                pos++;
                            }
                            if (!scriptlines[pos].StartsWith("class "))
                                throw new Exception("Plex decorators without matching global class");
                            var classname = scriptlines[pos].Split(' ')[1];
                            if (classname.Contains("(")) // derived class
                                classname = classname.Split('(')[0];
                            else
                                classname = classname.Remove(classname.Length - 1); // remove :
                            var code = String.Format(resman.GetString(templatename), decorators, classname, fname.Replace("\\", "\\\\")); // generate the C# wrapper class from template
#if DEBUG

Console.WriteLine(code);


#endif
                            if (results.Errors.HasErrors)
                            {
                                string except = "The wrapper class failed to compile.";
                                foreach (CompilerError error in results.Errors)
                                    except += Environment.NewLine + error.ErrorText;
                                throw new Exception(except);
                            }
                            types.AddRange(results.CompiledAssembly.GetTypes()); // We did it!
                            entry.asms.Add(File.ReadAllBytes(results.PathToAssembly));
                            pos++; // keep scanning the file for more classes
                        }
                    }
                    catch (Exception ex) // Skip any file that has issues
                    {


#if DEBUG
                        Console.WriteLine(ex.ToString());


#endif
                    newcache.Add(fname, entry);
                }


#if DEBUG


#endif
                {
                    ReflectMan.AddTypes(types.ToArray());
                    using (var stream = File.OpenWrite("pyasmcache.dat"))
                        AsmCache.Save(stream, newcache);
                }
                scanned = true;
            }
            {

            }