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
        public NeuropixDataFrame(ElectrodePacket packet, float bufferCapacity)
        {
            var mat = new Mat(384, 1, Depth.F32, 1, packet.LfpData);
            LfpData = mat.Clone();
            BufferCapacity = bufferCapacity;
        }

        public Mat LfpData { get; private set; }

        public float BufferCapacity { get; private set; }
    }
}
