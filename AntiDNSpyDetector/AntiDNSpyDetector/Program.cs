using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using HarmonyLib;


namespace AntiDNSpyDetector
{
    internal class Program
    {
        private static string File = null;
        static void Main(string[] args)
        {
            bool IsValid = false;
            while (IsValid == false) 
            { 
                Log("Please insert a valid executable File: ", "CONFIG", ConsoleColor.Yellow); 
                File = Console.ReadLine();
                IsValid = System.IO.File.Exists(File) && Path.GetExtension(File) != "exe";
                Console.Clear();
            }
            Console.Title = $"[{DateTime.Now}] AntiDNSpyDetector by https://github.com/CabboShiba - https://t.me/CabboShiba";
            try
            {
                var assembly = Assembly.LoadFile(Path.GetFullPath(File));
                var paraminfo = assembly.EntryPoint.GetParameters();
                object[] parameters = new object[paraminfo.Length];
                Harmony patch = new Harmony("antidsnypdetector.cabbo.patch.https://github.com/CabboShiba");
                patch.PatchAll(Assembly.GetExecutingAssembly());
                Log("Patch succesfully loaded.", "CONFIG", ConsoleColor.Yellow);
                Thread.Sleep(1500);
                Console.Clear();
                assembly.EntryPoint.Invoke(null, parameters);
                
            }
            catch (Exception ex)
            {
                Log($"Could not load {File}\n{ex.Message}", "ERROR", ConsoleColor.Red);
            }
            Console.ReadLine();
        }   
        
        public static Type HookedClass = typeof(System.Runtime.InteropServices.Marshal);
        
        private const string HookedMethod = nameof(System.Runtime.InteropServices.Marshal.Copy);
        
        private static string GetMethod = $"{HookedClass.FullName}.{HookedMethod}";
        
        [HarmonyPatch(typeof(System.Runtime.InteropServices.Marshal), HookedMethod, new[] { typeof(IntPtr), typeof(byte[]), typeof(int),typeof(int) })]
        class HookMarshal
        {
            static bool Prefix(IntPtr source, byte[] destination, int startIndex, int length)
            {
                Log($"Succesfully Hooked {GetMethod}", "HOOK", ConsoleColor.Green);
                Log($"IntPtr: {source}", "PARAMS", ConsoleColor.Cyan);
                Log($"StartIndex: {startIndex}", "PARAMS", ConsoleColor.Cyan);
                Log($"Length: {length}", "PARAMS", ConsoleColor.Cyan);
                return false;
            }
        }
        public static void Log(string Data, string Type, ConsoleColor Color)
        {
            Console.ForegroundColor = Color;
            string Log = $"[{DateTime.Now} - {Type}] {Data}";
            Console.WriteLine(Log);
            Console.ResetColor();
        }
        
    }
}
