using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;                   //전체적인 키포인트값, 뎁스값 등을 조절

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;      // 바디소스매니저 파일이랑 연결
    
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },            //키넥트의 기본 관절이름과 값들, 0-24까지의 번호있음
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    
    
    void Update () 
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();   //바디소스매니저에서 설정한 값들을 연동
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();     //바디 트래킹
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
              }
                
            if(body.IsTracked)      //트래킹 된 아이디(값)을 저장
            {
                trackedIds.Add (body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))     //트래킹 안된 값들 버림
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);  //트래킹이 되었으면 바디오브젝트와 연동
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);  //연동된 바디오브젝트 업데이트
            }
        }
    }
    
    private GameObject CreateBodyObject(ulong id)     //바디오브젝트 생성
    {
        GameObject body = new GameObject("Body:" + id);
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)  //키넥트 척추,오른쪽 엄지(뼈들의 중심, 가장 끝을 설정)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);   //유니티 게임오브젝트 큐브랑 바디랑 연결
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();   //각 관절 포인트를 잇는 선(유니티에 나타나는 그 선) 표현
            lr.SetVertexCount(2);                                   //점과 점사이의 거리로 선 길이 구하고 나타냄
            lr.material = BoneMaterial;                            //이것을 '뼈'와 동일시
            lr.SetWidth(0.05f, 0.05f);
            
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);    //3차원 백터로 값들을 저장함 (사람이니까 3차원)
            jointObj.name = jt.ToString();                         //각 뼈 이름들을 키넥트 안에있는 뼈 이름들이랑 맞춰줌
            jointObj.transform.parent = body.transform;             //키넥트 바디를 부모 객체로
        }
        
        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)         //각 뼈들 값들을 업데이트 유니티에서 움직임 표현
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];            //키넥트의 조인트 값들을 원본 조인트라고 설정 
            Kinect.Joint? targetJoint = null;                      //변동하는 조인트값 일단 null로 생성
            
            if(_BoneMap.ContainsKey(jt))             //
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            
            Transform jointObj = bodyObject.transform.Find(jt.ToString());  //키넥트 바디 객체에서 뼈들 가져옴
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);      //원본 조인트를 3차원 벡터연산으로 평면상에 나타날 수 있도록함
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)                         //이부분 잘 모르겠는데 가져온 조인트값들을 밑에 나올 각각의 클래스, 함수에 넣어주는듯
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state) //트래킹 된 '뼈'의 색상을 나타내줌 유니티상에서 볼 수 있음
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:   //잘 트래킹되면 초록
            return Color.green;

        case Kinect.TrackingState.Inferred:  //예측되는 (시시각각 변하는) 트래킹 값 빨강
            return Color.red;

        default:
            return Color.black;     //기본  검정
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)   //3차원 벡터연산으로 조인트 값 가져옴 == 3차원의 값
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
         
         
    }
}   //알아보니 우리가 원하는 x,y,z값은 cameraspacepoint 에서 가져와야함 == 2차원에서 xyz값 가져오는 것임