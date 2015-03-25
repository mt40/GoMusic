#include "pch.h"
#include "MediaEnginePlayer.h"


MediaEnginePlayer::MediaEnginePlayer() :
m_spMediaEngine(nullptr),
m_spEngineEx(nullptr),
m_isPlaying(false)
{
	memset(&m_bkgColor, 0, sizeof(MFARGB));
}

void MediaEnginePlayer::Initialize()
{
	Microsoft::WRL::ComPtr<IMFMediaEngineClassFactory> spFactory;
	Microsoft::WRL::ComPtr<IMFAttributes> spAttributes;
	CoCreateInstance(CLSID_MFMediaEngineClassFactory, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&spFactory));

	//MFCreateAttributes(&spAttributes, 1);

	const DWORD flags = MF_MEDIA_ENGINE_WAITFORSTABLE_STATE;

	spFactory->CreateInstance(flags, spAttributes.Get(), &m_spMediaEngine);
	//m_spMediaEngine.Get()->QueryInterface(__uuidof(IMFMediaEngine), (void **)&m_EngineEx);

	return;
}

double MediaEnginePlayer::GetVol()
{
	return m_spMediaEngine->GetVolume();
}

MediaEnginePlayer::~MediaEnginePlayer()
{
	Shutdown();
	MFShutdown();
}

void MediaEnginePlayer::Shutdown()
{
	if (m_spMediaEngine)
	{
		m_spMediaEngine->Shutdown();
	}
	return;
}


