using System;
using R3;
using UnityEngine;
public class PitchEventDetector : MonoBehaviour
{
    [Header("解析設定")]
    [Tooltip("FFT に使うサンプル数 (2のべき乗). 1024 or 2048 あたりがおすすめ")]
    public int sampleSize = 2048;

    [Tooltip("解析する最小振幅。この値より小さいと無音とみなす")]
    public float minVolumeThreshold = 0.001f;

    [Header("ピッチ判定 [Hz]")]
    public float lowMaxHz = 200f;     // 〜200Hz を低音
    public float midMaxHz = 1000f;    // 〜1000Hz を中音
    // それ以上を高音とする

    [Header("イベント")] 
    public Subject<Unit> lowEvent = new Subject<Unit>();
    public Subject<Unit> midEvent = new Subject<Unit>();
    public Subject<Unit> highEvent  = new Subject<Unit>();
    
    public AudioSource _audioSource;
    private float[] _spectrum;
    private int _sampleRate;

    public float CurrentPitchHz { get; private set; }  // 現在推定しているピッチ

    
    public void Awake()
    {
        _spectrum = new float[sampleSize];
        _sampleRate = AudioSettings.outputSampleRate;
    }
    
    
    
    public void DetectPitchAndInvokeEvents()
    {
        // スペクトル（周波数解析）
        _audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

        int maxIndex = 0;
        float maxValue = 0f;

        // もっとも強い周波数成分を探す
        for (int i = 0; i < sampleSize; i++)
        {
            if (_spectrum[i] > maxValue)
            {
                maxValue = _spectrum[i];
                maxIndex = i;
            }
        }

        // 音が小さすぎるときは無音とみなす
        if (maxValue < minVolumeThreshold)
        {
            CurrentPitchHz = 0f;
            return;
        }

        // インデックス -> 周波数[Hz] に変換
        // Nyquist周波数 = sampleRate / 2
        float freqN = (float)maxIndex / sampleSize;      // 0〜0.5くらいの正規化周波数
        float freqHz = freqN * _sampleRate;

        CurrentPitchHz = freqHz;

        // 周波数帯域でイベント振り分け
        if (freqHz <= lowMaxHz)
        {
            lowEvent.OnNext(Unit.Default);
        }
        else if (freqHz <= midMaxHz)
        {
            midEvent.OnNext(Unit.Default);
        }
        else
        {
            highEvent.OnNext(Unit.Default);
        }
    }

    public void Dispose()
    {
        lowEvent.Dispose();
        midEvent.Dispose();
        highEvent.Dispose();
    }
}