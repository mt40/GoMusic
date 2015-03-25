#pragma once

#include "DirectXHelper.h"
#include <wrl.h>
#include <mfmediaengine.h>
#include <strsafe.h>
#include <mfapi.h>
#include <agile.h>

using namespace std;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;


// MediaEngineNotifyCallback - Defines the callback method to process media engine events.
struct MediaEngineNotifyCallback abstract
{
	virtual void OnMediaEngineEvent(DWORD meEvent) = 0;
};

class MediaEnginePlayer : public MediaEngineNotifyCallback
{
	ComPtr<IMFMediaEngine>           m_spMediaEngine;
	ComPtr<IMFMediaEngineEx>         m_spEngineEx;

	MFARGB                           m_bkgColor;

public:
	MediaEnginePlayer();
	~MediaEnginePlayer();

	// Media Info
	void GetNativeVideoSize(DWORD *cx, DWORD *cy);
	bool IsPlaying();

	// Initialize/Shutdown
	void Initialize(ComPtr<ID3D11Device> device, DXGI_FORMAT d3dFormat);
	void Shutdown();

	// Media Engine related
	void OnMediaEngineEvent(DWORD meEvent);

	// Media Engine Actions
	void Play();
	void Pause();
	void SetMuted(bool muted);

	// Media Source
	void SetSource(Platform::String^ sourceUri);
	void SetBytestream(IRandomAccessStream^ streamHandle, Platform::String^ szURL);

	// Transfer Video Frame
	void TransferFrame(ComPtr<ID3D11Texture2D> texture, MFVideoNormalizedRect rect, RECT rcTarget);

private:
	bool m_isPlaying;
};


