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
                Console.WriteLine("{0} {1}", version.Major, version.Minor);

                basestation.Open();
                basestation.StartRecording(FileName);
                return Disposable.Create(() =>
                {
                    basestation.StopRecording();
                    basestation.Close();
                    basestation.Dispose();
                });
            });
        }
    }
}
