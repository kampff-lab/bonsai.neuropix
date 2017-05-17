using Neuropix.Net;
using OpenCV.Net;
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
            return Observable.Create<NeuropixDataFrame>((observer, cancellationToken) =>
            {
                return Task.Factory.StartNew(() =>
                {
                    using (var basestation = new NeuropixBasestation())
                    {
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

                        Console.Write("Neuropix ADC calibration... ");
                        basestation.ApplyAdcCalibrationFromEeprom();
                        Console.WriteLine("OK");

                        //Console.Write("Neuropix Gain calibration... ");
                        //basestation.ApplyGainCalibrationFromEeprom();
                        //Console.WriteLine("OK");

                        basestation.Mode = AsicMode.Recording;
                        basestation.TriggerMode = false;
                        Console.WriteLine("Neuropix start recording...");

                        //basestation.ApplyAdcCalibrationFromCsv(
                        //    @"C:\Repos\Bonsai.Neuropix\Externals\probecalib\15039702372\Comparator calibration.csv",
                        //    @"C:\Repos\Bonsai.Neuropix\Externals\probecalib\15039702372\Offset calibration.csv",
                        //    @"C:\Repos\Bonsai.Neuropix\Externals\probecalib\15039702372\Slope calibration.csv");

                        basestation.LedOff(true);
                        var fileName = FileName;
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            basestation.StartRecording(FileName);
                        }
                        basestation.NeuralStart();

                        var packet = new ElectrodePacket();
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            basestation.ReadElectrodeData(packet);
                            var result = new NeuropixDataFrame(packet, basestation.FifoFilling);
                            observer.OnNext(result);
                        }

                        Console.WriteLine("Neuropix stop recording...");
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            basestation.StopRecording();
                        }
                        basestation.Close();
                    }
                },
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            });
        }
    }
}
