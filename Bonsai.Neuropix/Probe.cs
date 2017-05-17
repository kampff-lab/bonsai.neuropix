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

        public string GainCalibration { get; set; }

        public string ComparatorCalibration { get; set; }

        public string AdcOffsetCalibration { get; set; }

        public string AdcSlopeCalibration { get; set; }

        public FilterBandwidth FilterBandwidth { get; set; }

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
                        var comparatorCalibration = ComparatorCalibration;
                        var adcOffsetCalibration = AdcOffsetCalibration;
                        var adcSlopeCalibration = AdcSlopeCalibration;
                        if(!string.IsNullOrEmpty(comparatorCalibration) &&
                           !string.IsNullOrEmpty(adcOffsetCalibration) &&
                           !string.IsNullOrEmpty(adcSlopeCalibration))
                        {
                            basestation.ApplyAdcCalibrationFromCsv(comparatorCalibration, adcOffsetCalibration, adcSlopeCalibration);
                        }
                        else basestation.ApplyAdcCalibrationFromEeprom();
                        Console.WriteLine("OK");

                        basestation.SetFilter(FilterBandwidth);

                        Console.Write("Neuropix Gain calibration... ");
                        var gainCalibration = GainCalibration;
                        if(!string.IsNullOrEmpty(gainCalibration))
                        {
                            basestation.ApplyGainCalibrationFromCsv(gainCalibration);
                        }
                        else basestation.ApplyGainCalibrationFromEeprom();
                        Console.WriteLine("OK");

                        basestation.LedOff(true);
                        basestation.Mode = AsicMode.Recording;
                        basestation.DataMode = true;
                        basestation.TriggerMode = false;
                        basestation.SetNrst(false);
                        basestation.ResetDatapath();
                        var fileName = FileName;
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            basestation.StartRecording(FileName);
                        }
                        Console.WriteLine("Neuropix recording armed.");

                        basestation.SetNrst(true);
                        basestation.NeuralStart();
                        Console.WriteLine("Neuropix recording start...");

                        var packet = new ElectrodePacket();
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            basestation.ReadElectrodeData(packet);
                            var result = new NeuropixDataFrame(packet, 0);
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
