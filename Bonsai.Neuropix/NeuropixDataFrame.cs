using Neuropix.Net;
using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Neuropix
{
    public class NeuropixDataFrame
    {
        const int ChannelCount = 384;
        const int SampleCount = 12;

        public NeuropixDataFrame(ElectrodePacket packet, float bufferCapacity)
        {
            var startTrigger = new Mat(1, SampleCount, Depth.U8, 1, packet.StartTrigger);
            var synchronization = new Mat(1, SampleCount, Depth.U16, 1, packet.Synchronization);
            var counters = new Mat(SampleCount, SampleCount + 1, Depth.S32, 1, packet.Counters);
            var lfpData = new Mat(ChannelCount, 1, Depth.F32, 1, packet.LfpData);
            var apData = new Mat(SampleCount, ChannelCount, Depth.F32, 1, packet.ApData);

            StartTrigger = startTrigger.Clone();
            Synchronization = synchronization.Clone();
            Counters = Transpose(counters);
            LfpData = lfpData.Clone();
            ApData = Transpose(apData);
            BufferCapacity = bufferCapacity;
        }

        public Mat StartTrigger { get; private set; }

        public Mat Synchronization { get; private set; }

        public Mat Counters { get; private set; }

        public Mat LfpData { get; private set; }

        public Mat ApData { get; private set; }

        public float BufferCapacity { get; private set; }

        static Mat Transpose(Mat source)
        {
            var result = new Mat(source.Cols, source.Rows, source.Depth, source.Channels);
            CV.Transpose(source, result);
            return result;
        }
    }
}
