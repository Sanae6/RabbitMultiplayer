using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using CommandLine;
using Esprima;
using Jint;
using Jint.Runtime;
using Jint.Runtime.Debugger;
using Jint.Runtime.Interop;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;

public class RabbitMP
{
    public class Options
    {
        [Value(0, HelpText = "Where the data.win for My Rabbits Are Gone is.", MetaValue = "data.win"
            ,Required = true)]
        public string FileLocation { get; set; }
//        [Value(0)]
//        public long RandomizerSeed { get; set; }
        [Option('d',"dont-run"
            ,HelpText = "Don't run the game after patching.", Default = false)]
        public bool DoNotRunLater { get; set; }
    }
    public static void Main(string[] args)
    {
        UndertaleData data;
        Parser.Default.ParseArguments<Options>(args).WithParsed(opts =>
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("MyRabbitsAreGone");
                if (processes.Length != 0)
                {
                    Console.WriteLine("Close My Rabbits Are Gone before continuing.");
                    while (processes.Length != 0)
                    {
                        Thread.Sleep(1000);
                        processes = Process.GetProcessesByName("MyRabbitsAreGone");
                    }

                    Console.WriteLine("Continuing!");
                }

                if (Path.GetFileName(opts.FileLocation) != "data.win")
                    throw new Exception("File name is not data.win!");
                if (File.Exists(opts.FileLocation + ".bak"))
                {
                    Console.WriteLine("Found a backup, overwriting current data.win with it.");
                    File.Delete(opts.FileLocation);
                    opts.FileLocation += ".bak";
                }
                else
                {
                    Console.WriteLine("Backup not found, doing that now.");
                    File.Copy(opts.FileLocation, opts.FileLocation + ".bak");
                }
                if (!File.Exists(opts.FileLocation)) throw new Exception("File does not exist!");

                var read = new UndertaleReader(File.OpenRead(opts.FileLocation));
                data = read.ReadUndertaleData();
                read.Close();
                Engine engine = new Engine(cfg =>
                    cfg.AllowClr(typeof(UndertaleData).Assembly, typeof(UndertaleGameObject).Assembly).DebugMode());
                engine = engine.SetValue("RoomGameObject",
                    TypeReference.CreateTypeReference(engine, typeof(UndertaleRoom.GameObject))).SetValue(
                    "EventAction",
                    TypeReference.CreateTypeReference(engine,typeof(UndertaleGameObject.EventAction))).SetValue(
                    "UEvent",TypeReference.CreateTypeReference(engine,typeof(UndertaleGameObject.Event)));
                engine = engine.SetValue("log", new Action<object>(Console.WriteLine)).SetValue("data", data);
                Console.WriteLine("Passing control to patcher.js");
                engine.Execute(File.ReadAllText("patcher.js")).Invoke("main");
                Console.WriteLine("Exited JavaScript mode.");
                Console.WriteLine("Now writing data.win...");
                if (opts.FileLocation.EndsWith(".bak")) opts.FileLocation = opts.FileLocation.Slice(0, opts.FileLocation.Length - 4);
                Console.WriteLine(opts.FileLocation);
                var write = new UndertaleWriter(File.OpenWrite(opts.FileLocation));
                write.WriteUndertaleData(data);
                write.Close();
                Console.WriteLine("Written!");
                if (!opts.DoNotRunLater)
                {
                    Console.WriteLine("Now starting the game :)");
                    Process.Start(Path.Combine(Path.GetDirectoryName(opts.FileLocation), "MyRabbitsAreGone.exe"));
                    Process.GetCurrentProcess().Kill();
                }

                Console.WriteLine("Seeya!");
            }
            catch (JavaScriptException e)
            {
                Console.Error.WriteLine("Error \"{0}\" at {1}:{2}", e.Message, e.LineNumber, e.Column);
                Console.Error.WriteLine(e.CallStack);
                Environment.Exit(1);
            } catch (Exception e) {
                Console.Error.WriteLine("{0}: \"{1}\"",e.GetType().Name,e.Message);
                Console.Error.WriteLine(e.Source);
                Environment.Exit(1);
            }
        });
    }
}