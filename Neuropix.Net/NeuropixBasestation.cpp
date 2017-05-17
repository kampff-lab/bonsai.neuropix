#include "stdafx.h"
#include <msclr/marshal_cppstd.h>
#include "NeuropixBasestation.h"

Neuropix::Net::NeuropixBasestation::NeuropixBasestation():
	api(new Neuropix_basestation_api())
{
}

Neuropix::Net::NeuropixBasestation::!NeuropixBasestation()
{
	delete api;
}

void Neuropix::Net::NeuropixBasestation::ThrowExceptionForErrorCode(ErrorCode error, String ^message)
{
	if (error != SUCCESS)
	{
		throw gcnew System::InvalidOperationException(message);
	}
}

void Neuropix::Net::NeuropixBasestation::Open()
{
	Open(0);
}

void Neuropix::Net::NeuropixBasestation::Open(Byte headstageSelect)
{
	Open(headstageSelect, "10.2.0.1");
}

void Neuropix::Net::NeuropixBasestation::Open(Byte headstageSelect, String ^ipAddress)
{
	std::string _ipAddress = msclr::interop::marshal_as<std::string>(ipAddress);
	OpenErrorCode error = api->neuropix_open(headstageSelect, _ipAddress);
	if (error != OPEN_SUCCESS)
	{
	}
}

void Neuropix::Net::NeuropixBasestation::Open(String ^playbackFile)
{
	std::string _playbackFile = msclr::interop::marshal_as<std::string>(playbackFile);
	OpenErrorCode error = api->neuropix_open(_playbackFile);
	if (error != OPEN_SUCCESS)
	{
	}
}

void Neuropix::Net::NeuropixBasestation::Close()
{
	api->neuropix_close();
}

void Neuropix::Net::NeuropixBasestation::ApplyAdcCalibrationFromEeprom()
{
	ErrorCode error = api->neuropix_applyAdcCalibrationFromEeprom();
	ThrowExceptionForErrorCode(error, "Unable to apply ADC calibration.");
}

void Neuropix::Net::NeuropixBasestation::ApplyGainCalibrationFromEeprom()
{
	ErrorCode error = api->neuropix_applyGainCalibrationFromEeprom();
	ThrowExceptionForErrorCode(error, "Unable to apply Gain calibration.");
}

void Neuropix::Net::NeuropixBasestation::ConfigureDeserializer()
{
	ErrorCode error = api->neuropix_configureDeserializer();
	ThrowExceptionForErrorCode(error, "Unable to configure deserializer.");
}

void Neuropix::Net::NeuropixBasestation::ConfigureSerializer()
{
	ErrorCode error = api->neuropix_configureSerializer();
	ThrowExceptionForErrorCode(error, "Unable to configure serializer.");
}

Neuropix::Net::VersionNumber Neuropix::Net::NeuropixBasestation::GetHardwareVersion()
{
	::VersionNumber version;
	ErrorCode error = api->neuropix_getHardwareVersion(&version);
	ThrowExceptionForErrorCode(error, "No config link connection.");
	return VersionNumber(version.major, version.minor);
}

Neuropix::Net::VersionNumber Neuropix::Net::NeuropixBasestation::GetAPIVersion()
{
	::VersionNumber version = api->neuropix_getAPIVersion();
	return VersionNumber(version.major, version.minor);
}

void Neuropix::Net::NeuropixBasestation::StartRecording(String ^fileName)
{
	std::string _fileName = msclr::interop::marshal_as<std::string>(fileName);
	ErrorCode error = api->neuropix_startRecording(_fileName);
	ThrowExceptionForErrorCode(error, "No tcp/ip data connection.");
}

void Neuropix::Net::NeuropixBasestation::StopRecording()
{
	ErrorCode error = api->neuropix_stopRecording();
	ThrowExceptionForErrorCode(error, "No tcp/ip data connection.");
}