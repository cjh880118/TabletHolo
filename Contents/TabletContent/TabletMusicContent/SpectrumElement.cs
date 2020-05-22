using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpectrumElement : MonoBehaviour {
    [Header("Audio Source")]
    public AudioSource _audioSource;
    float[] spectrum = new float[2048];

    [Header("Spectrum Object")]
    //생성할 오브젝트
    public GameObject _spectrumImagePrefab;
    
    //생성한 오브젝트를 담는 배열
    GameObject[] _spectrumImage;

    //오브젝트의 갯수
    public int _spectrumObjNum = 72;

    //중심과 오브젝트 사이의 거리
    public int _spectrumRadius = 10;

    //오브젝트의 너비 폭
    public float _spectrumXZScale = 15;

    //오브젝트의 최대 높이
    public float _spectrumYScale = 10000;

    //오브젝트의 각도
    private float _spectrumAngle;


    public void InitAudioSpectrum()
    {

        _audioSource = GameObject.Find("TabletMusicContent").GetComponent<AudioSource>();
        //audioSpectrumData = _audioObj.GetComponent<AudioSpectrumData>();

        //오브젝트의 갯수만큼 오브젝트를 담는 배열 초기화
        _spectrumImage = new GameObject[_spectrumObjNum];

        //오브젝트의 각도 계산 (360도 / 오브젝트의 갯수)
        _spectrumAngle = 360.0f / _spectrumObjNum;

        for (int i = 0; i < _spectrumImage.Length; i++)
        {
            //오브젝트 생성
            GameObject _instanceImage = Instantiate(_spectrumImagePrefab);

            //생성된 오브젝트의 위치 및 각도 조정
            _instanceImage.transform.position = transform.position;
            _instanceImage.transform.parent = transform;
            transform.eulerAngles = new Vector3(0, 0, _spectrumAngle * i);
            _instanceImage.transform.position = -Vector3.down * _spectrumRadius;
            _instanceImage.GetComponent<Image>().color = Color.HSVToRGB((1.0f / _spectrumObjNum) * i, 1, 1);
            _spectrumImage[i] = _instanceImage;
        }
    }

    void Update()
    {
        this.gameObject.transform.Rotate(new Vector3(0, 0, 10f * Time.deltaTime));

        _audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Triangle);
        for (int i = 0; i < _spectrumImage.Length; i++)
        {
            if (_spectrumImage != null)
            {
                _spectrumImage[i].transform.localScale = new Vector3(_spectrumXZScale, spectrum[i] * _spectrumYScale + 2, _spectrumXZScale);
            }
        }
    }
}
