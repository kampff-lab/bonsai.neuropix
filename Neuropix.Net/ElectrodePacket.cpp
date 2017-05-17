#include "stdafx.h"
#include <string.h>
#include "ElectrodePacket.h"
using namespace System::Runtime::InteropServices;

Neuropix::Net::ElectrodePacket::ElectrodePacket():
	packet(new ::ElectrodePacket())
{
}

Neuropix::Net::ElectrodePacket::!ElectrodePacket()
{
	delete packet;
}

