using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace FastSwitchDNS
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            for (int iAdapter = 0; iAdapter < adapters.Length; iAdapter++)
            {
                NetworkInterface iterAdapter = adapters[iAdapter];
                Console.WriteLine($"{iAdapter}. {iterAdapter.Name} | {iterAdapter.Description}");
            }
            int selectedAdapter = 0;
            while (true)
            {
                Console.WriteLine($"请选择网络适配器(0~{adapters.Length - 1}):");
                if (int.TryParse(Console.ReadLine(), out selectedAdapter)
                    && selectedAdapter >= 0 && selectedAdapter < adapters.Length)
                {
                    break;
                }
                Console.WriteLine("无效输入！");
            }
            NetworkInterface adapter = adapters[selectedAdapter];
            Console.Clear();
            Console.WriteLine("0. 自动获取");
            Console.WriteLine("1. 北京乐游远程");
            int selectedDNS;
            while (true)
            {
                Console.WriteLine("设置DNS为(0~1)：");
                if (int.TryParse(Console.ReadLine(), out selectedDNS)
                    && selectedDNS >= 0 && selectedDNS <= 1)
                {
                    break;
                }
                Console.WriteLine("无效输入！");
            }

            Console.Clear();
            switch (selectedDNS)
            {
                case 0:
                    SetDNS_DHCP(adapter.Name);
                    break;
                case 1:
                    SetDNS(adapter.Name, "10.53.0.31", "");
                    break;
            }
            Console.WriteLine("完成");
            Console.ReadLine();
        }

        private static void SetDNS(string adapterName, string primaryDNS, string secondaryDNS)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            if (!string.IsNullOrEmpty(primaryDNS))
            {
                p.StandardInput.WriteLine($"netsh interface ip set dns name=\"{adapterName}\" source=static addr={primaryDNS} register=primary");
            }
            if (!string.IsNullOrEmpty(secondaryDNS))
            {
                p.StandardInput.WriteLine($"netsh interface ip set dns name=\"{adapterName}\" source=static addr={secondaryDNS} index=2");
            }
            p.StandardInput.WriteLine("exit");
            p.WaitForExit();

            Console.WriteLine("Output:");
            Console.WriteLine($"{p.StandardOutput.ReadToEnd()}");
            Console.WriteLine();
            Console.WriteLine("Errot:");
            Console.WriteLine($"{p.StandardError.ReadToEnd()}");
        }

        public static void SetDNS_DHCP(string adapterName)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine($"netsh interface ip set dns name=\"{adapterName}\" source=dhcp");
            p.StandardInput.WriteLine("exit");
            p.WaitForExit();

            Console.WriteLine("Output:");
            Console.WriteLine($"{p.StandardOutput.ReadToEnd()}");
            Console.WriteLine();
            Console.WriteLine("Errot:");
            Console.WriteLine($"{p.StandardError.ReadToEnd()}");
        }
    }
}
