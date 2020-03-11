using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace DownloadManager
{
    public static class SimpleOperations<T>
    {
        //изменение размера массива 
        public static Array ResizeArray(Array arr, int[] newSizes)
        {
            if (newSizes.Length != arr.Rank)
                throw new ArgumentException("arr must have the same number of dimensions " +
                                            "as there are elements in newSizes", "newSizes");

            var temp = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
            int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
            Array.ConstrainedCopy(arr, 0, temp, 0, length);
            return temp;
        } 
    }
    static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        static public extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static public extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
    }
    class Program
    { 
        public static string ConfFile = "./cfg.txt";
        public static string[] ControlFolders;

        public static string[] ReadAllFile(string path)
        {
            string _line = "";
            using (StreamReader _sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                _line = _sr.ReadToEnd();
                _sr.Close();
            }
            return _line.Split('\n');
        }
        public static string[] GetDirectoryInfo(string path = "root")
        {
            try
            {
                if (path == "root")
                {
                    return Directory.GetLogicalDrives();
                }
                else
                {
                    return Directory.GetFiles(path);
                }
            }
            catch
            {
                return new string[1] { "Error. Incorrect Data." };
            }
        }
        [STAThread]
        static void Main(string[] args)
        {
            var handle = NativeMethods.GetConsoleWindow();
            NativeMethods.ShowWindow(handle, NativeMethods.SW_HIDE); // убрать 
            ControlFolders = ReadAllFile(ConfFile);
            string tmp;
            string[] files;
            string movepath = "";
            while (true)
            {
                for(int i=0; i<ControlFolders.Length; i++)
                {
                    try {
                        files = GetDirectoryInfo(ControlFolders[i]);
                        for (int k = 0; k < files.Length; k++)
                        {
                            movepath = "";
                            if (files[k] != "Error. Incorrect Data.")
                            {
                                tmp = files[k].Substring(files[k].LastIndexOf(".") + 1);

                                if ((ControlFolders[i])[ControlFolders[i].Length - 1] == '\\'|| (ControlFolders[i])[ControlFolders[i].Length - 1]=='/') { 
                                Directory.CreateDirectory(ControlFolders[i] + tmp);
                                    movepath = ControlFolders[i] + tmp;
                                }
                                else
                                {
                                    Directory.CreateDirectory(ControlFolders[i] +"\\"+ tmp);
                                    movepath = ControlFolders[i] + "\\" + tmp;
                                }
                                movepath += "\\" + files[k].Substring(files[k].LastIndexOf("\\")+1);
                                File.Move(files[k], movepath);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message); 
                    }
                }
                Thread.Sleep(3600);
            }
        }
    }
}
