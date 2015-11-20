﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiNet.Gpio
{
    public class PinManager : IDisposable
    {
        private string GPIO_FOLDER = "/sys/class/gpio/";

        private readonly IList<Pin.PinType> _activepins;

        public PinManager()
        {
            _activepins = new List<Pin.PinType>();
        }

        public void Export(Pin.PinType pin)
        {
            if (_activepins.Contains(pin))
                return;
            var folder = string.Format("{0}export", GPIO_FOLDER);
            Execute(folder, pin.ToString().Substring(4));
            _activepins.Add(pin);
        }

        public void UnExport(Pin.PinType pin)
        {
            if (!_activepins.Contains(pin))
                return;
            var folder = string.Format("{0}unexport", GPIO_FOLDER);
            Execute(folder, pin.ToString().Substring(4));
            _activepins.Remove(pin);
        }

        public void Write(Pin.PinType pin, bool on)
        {
            if (!_activepins.Contains(pin))
                return;
            var val = on ? 1 : 0;
            var folder = string.Format("{0}gpio{1}/direction", GPIO_FOLDER, pin.ToString().Substring(4));
            Execute(folder, "out");
            folder = string.Format("{0} > {1}gpio{2}/value", val, GPIO_FOLDER, pin.ToString().Substring(4));
            Execute(folder, val.ToString());
        }

        public bool Read(Pin.PinType pin)
        {
            return false;
        }

        private void Execute(string folder, string text)
        {
            Console.WriteLine("Executing: " + folder + ":"+ text);
            Console.ReadLine();
            File.WriteAllText(folder, text);

            Console.WriteLine("Current LS:");
            var ls = new System.Diagnostics.Process();
            ls.EnableRaisingEvents = true;
            ls.StartInfo.FileName = "ls";
            ls.StartInfo.Arguments = "/sys/class/gpio";
            ls.Start();
            ls.WaitForExit();

        }





        public void Dispose()
        {
            foreach(var pin in _activepins)
            {
                UnExport(pin);
            }
        }
    }
}
