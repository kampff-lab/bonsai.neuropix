#pragma once
using namespace System;
#include <ElectrodePacket.h>

namespace Neuropix
{
	namespace Net
	{
		public ref class ElectrodePacket
		{
		internal:
			::ElectrodePacket *packet;
		private:
			~ElectrodePacket() { this->!ElectrodePacket(); }
			!ElectrodePacket();
		public:
			ElectrodePacket();

			property IntPtr LfpData {
				IntPtr get() { return (IntPtr)packet->lfpData; }
			}
		};
	}
}

