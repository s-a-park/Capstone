using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class InfraredSourceManager : MonoBehaviour 
{
    private KinectSensor _Sensor;
    private InfraredFrameReader _Reader;
    private ushort[] _Data;
    private byte[] _RawData;
    
    // I'm not sure this makes sense for the Kinect APIs
    // Instead, this logic should be in the VIEW
    private Texture2D _Texture;

    public Texture2D GetInfraredTexture()       //infrared~~는 적외선 카메라 보여주는 것임 흑백으로 나오지만 어느곳에서나 밝기 판별가능
    {                                       //적외선으로 나오는 2D 텍스쳐 가져온다 (유니티에서 왼쪽 밑에 있는 프레임)
        return _Texture;
    }
    
    void Start()
    {
        _Sensor = KinectSensor.GetDefault();      //마찬가지로 키넥트 센서 값 가져옴
        if (_Sensor != null) 
        {
            _Reader = _Sensor.InfraredFrameSource.OpenReader();    //마찬가지로 적외선 프레임 실행
            var frameDesc = _Sensor.InfraredFrameSource.FrameDescription;    //적외선 프레임에 대한 설명 얻어옴
            _Data = new ushort[frameDesc.LengthInPixels];
            _RawData = new byte[frameDesc.LengthInPixels * 4];         //가져온 동영상(?) 촬영물을 픽셀 사용 2D화면으로 나타나게함
            _Texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.BGRA32, false);
                                                                     //프레임 넓이 길이 포맷 정해줌
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
            var frame = _Reader.AcquireLatestFrame();         //마찬가지로 최근의 프레임 불러옴
            if (frame != null)
            {
                frame.CopyFrameDataToArray(_Data);          //적외선 프레임 데이터를 배열에 복사함
                
                int index = 0;
                foreach(var ir in _Data)
                {
                    byte intensity = (byte)(ir >> 8);
                    _RawData[index++] = intensity;
                    _RawData[index++] = intensity;            //데이터를 복사하는 과정인듯
                    _RawData[index++] = intensity;
                    _RawData[index++] = 255; // Alpha
                }
                
                _Texture.LoadRawTextureData(_RawData);       //촬영물 데이터 가져옴 (위에서 넣은 데이터들)
                _Texture.Apply();
                
                frame.Dispose();
                frame = null;
            }
        }
    }
    
    void OnApplicationQuit()
    {
        if (_Reader != null) 
        {
            _Reader.Dispose();      //마찬가지로 종료할때 프레임 리더기 꺼지게함
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
