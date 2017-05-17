using Neuropix.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Neuropix
{
    public class Probe : Source<NeuropixDataFrame>
    {
        public string FileName { get; set; }

        public override IObservable<NeuropixDataFrame> Generate()
        {
            return Observable.Create<NeuropixDataFrame>(observer =>
            {
                var basestation = new NeuropixBasestation();

                var version = basestation.GetAPIVersion();
                Console.WriteLine("Neuropix API version: {0}.{1}", version.Major, version.Minor);
                Console.WriteLine("Opening basestation data and configuration link...");
                basestation.Open();

                version = basestation.GetHardwareVersion();
                Console.WriteLine("Neuropix FPGA bootcode version: {0}.{1}", version.Major, version.Minor);

                version = basestation.GetBSVersion();
                Console.WriteLine("Neuropix Basestation connect board version: {0}.{1}", version.Major, version.Minor);

                var probeID = basestation.ReadID();
                Console.WriteLine("Neuropix probe S/N: {0} Option {1}", probeID.SerialNumber, probeID.ProbeType);

                basestation.LedOff(true);
                return Disposable.Create(() =>
                {
                    basestation.Close();
                    basestation.Dispose();
                });
            });
        }
    }
}
