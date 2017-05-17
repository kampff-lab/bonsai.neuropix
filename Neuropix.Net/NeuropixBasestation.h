#pragma once

using namespace System;
#include "Neuropix_basestation_api.h"

namespace Neuropix
{
	namespace Net
	{
		public value class ElectrodePacket
		{
		public:
		};

		public value class VersionNumber
		{
		public:
			unsigned short Major;
			unsigned short Minor;

			VersionNumber(unsigned short major, unsigned short minor):
				Major(major),
				Minor(minor)
			{
			}
		};

		public ref class NeuropixBasestation
		{
		private:
			Neuropix_basestation_api *api;
			static void ThrowExceptionForErrorCode(ErrorCode error, String ^message);
			~NeuropixBasestation() { this->!NeuropixBasestation(); }
			!NeuropixBasestation();
		public:
			NeuropixBasestation();

			void Open();
			void Open(Byte headstageSelect);
			void Open(Byte headstageSelect, String ^ipAddress);
			void Open(String ^playbackFile);
			void Close();

			void ApplyAdcCalibrationFromEeprom();
			void ApplyGainCalibrationFromEeprom();
			void ConfigureDeserializer();
			void ConfigureSerializer();

			VersionNumber GetHardwareVersion();
			VersionNumber GetAPIVersion();

			void StartRecording(String ^fileName);
			void StopRecording();

			property bool IsConnected {
				bool get()
				{
					return api->neuropix_isConnected();
				}
			}

			property float FifoFilling {
				float get()
				{
					return api->neuropix_fifoFilling();
				}
			}

			property float ScaleFactorToVoltage {
				float get()
				{
					return api->neuropix_getScaleFactorToVoltage();
				}
			}
		};
	}
}