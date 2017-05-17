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

		public value class AsicID {
		public:
			unsigned int SerialNumber;
			unsigned char ProbeType;

			AsicID(unsigned int serialNumber, char probeType):
				SerialNumber(serialNumber),
				ProbeType(probeType)
			{
			}
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
			static void ThrowExceptionForConfigAccessErrorCode(ConfigAccessErrorCode error, String ^message);
			static void ThrowExceptionForEepromErrorCode(EepromErrorCode error, String ^message);
			static void ThrowExceptionForDigitalControlErrorCode(DigitalControlErrorCode error, String ^message);
			static void ThrowExceptionForReadCsvErrorCode(ReadCsvErrorCode error, String ^message);
			static void ThrowExceptionForBaseConfigErrorCode(BaseConfigErrorCode error, String ^message);
			~NeuropixBasestation() { this->!NeuropixBasestation(); }
			!NeuropixBasestation();
		public:
			NeuropixBasestation();

			void Open();
			void Open(Byte headstageSelect);
			void Open(String ^playbackFile);
			void Close();

			VersionNumber GetHardwareVersion();
			VersionNumber GetAPIVersion();
			VersionNumber GetBSVersion();

			AsicID ReadID();
			void WriteID(AsicID id);
			unsigned char GetOption();

			void StartLog();
			void LedOff(bool ledOff);

			void ApplyAdcCalibrationFromEeprom();
			void ApplyAdcCalibrationFromCsv(String ^comparatorCalibrationFileName, String ^adcOffsetCalibrationFileName, String ^adcSlopeCalibrationFileName);

			void ApplyGainCalibrationFromEeprom();
			void ConfigureDeserializer();
			void ConfigureSerializer();

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