using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class BodySourceManager : MonoBehaviour 
{
    private KinectSensor _Sensor;                   //전체 = 키넥트와 유니티 연결해주는 코드
    private BodyFrameReader _Reader;               //bodyframereader = 바디 프레임 원본 소스
    private Body[] _Data = null;
    
    public Body[] GetData()
    {
        return _Data;
    }
    

    void Start () 
    {
        _Sensor = KinectSensor.GetDefault();                //키넥트 센서를 가져온다

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader();   //센서가 열려있지 않으면 프레임 열어줌
                                                                // openreader = 바디프레임 원본 소스를 읽는 프레임 리더기 생성
            
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }   
    }
    
    void Update () 
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();         //가장 최근의 바디 프레임을 불러온다
            if (frame != null)
            {
                if (_Data == null)
                {
                    _Data = new Body[_Sensor.BodyFrameSource.BodyCount];   // bodycount 바디 갯수(사람 수)를 읽음
                }
                
                frame.GetAndRefreshBodyData(_Data);              //얻은 데이터를 가지고 바디를 프레임에 불러오거나 새로고침
                
                frame.Dispose();
                frame = null;
            }
        }    
    }
    
    void OnApplicationQuit()
    {
        if (_Reader != null)                    //종료 누르면 리더기 꺼짐
        {
            _Reader.Dispose();
            _Reader = null;
        }
        
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }
}
