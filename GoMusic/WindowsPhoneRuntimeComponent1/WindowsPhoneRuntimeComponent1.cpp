// WindowsPhoneRuntimeComponent1.cpp
#include "pch.h"
#include "WindowsPhoneRuntimeComponent1.h"
#include "MediaEnginePlayer.h"

using namespace WindowsPhoneRuntimeComponent1;
using namespace Platform;

WindowsPhoneRuntimeComponent::WindowsPhoneRuntimeComponent()
{
	
}

int WindowsPhoneRuntimeComponent::DoSth()
{
	return 10;
}

double WindowsPhoneRuntimeComponent::GetSystemVolume()
{
	MediaEnginePlayer media;
	media.Initialize();

	double vol = media.GetVol();
	media.Shutdown();
	return vol;
}
