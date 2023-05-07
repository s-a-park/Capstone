using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;


    public static Vector3 FootLeft;
    public static Vector3 AnkleLeft;
    public static Vector3 KneeLeft;
    public static Vector3 HipLeft;
    public static Vector3 FootRight;
    public static Vector3 AnkleRight;
    public static Vector3 KneeRight;
    public static Vector3 HipRight;
    public static Vector3 HandTipLeft;
    public static Vector3 ThumbLeft;
    public static Vector3 HandLeft;
    public static Vector3 WristLeft;
    public static Vector3 ElbowLeft;
    public static Vector3 ShoulderLeft;
    public static Vector3 HandTipRight;
    public static Vector3 ThumbRight;
    public static Vector3 HandRight;
    public static Vector3 WristRight;
    public static Vector3 ElbowRight;
    public static Vector3 ShoulderRight;
    public static Vector3 SpineBase;
    public static Vector3 SpineMid;
    public static Vector3 SpineShoulder;
    public static Vector3 Neck;
    public static Vector3 Head;


    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
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
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>(); // Older body 
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
              }
                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys); //Configure newer body and older body
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)  //Update newiest body body
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
               
            }
        }
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube); //Primi
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>(); //Component for 3D line 
            lr.SetVertexCount(2);  //Set the number of line segments. segemnt = 선분
            //선분갯수를 정함. 1이면 선이 존재하지 않고 10이면 여러개의 선이 생김 -> 2로 설정 -> point로 추정
            lr.material = BoneMaterial; // Set Material of lr
            lr.SetWidth(0.05f, 0.05f); // Line width between Start Point and End Point
            
            jointObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Set Scale of Lr Object
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
            //transform.parent
            //The parent of the transform.
            //Changing the parent will modify the parent-relative position, scale and rotation but keep the world space position, rotation and scale the same.
        }

        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        //GameObject bodyObject는 Unity내에서 생성된 점에 대한 Object
        //Kinect.Body는 Kinect에서 새로들어오는 joint에 대한 객체 

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) //Keep joint 
            //JointType은enum형태
        {
            Kinect.Joint sourceJoint = body.Joints[jt]; //temp같은 역할 같음
            Kinect.Joint? targetJoint = null;//targetJoint가 mainJoint
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]]; //Body의 ThumbRight값을 targetJoint에다가 대입
            }
            
            Transform jointObj = bodyObject.transform.Find(jt.ToString());  //jointObj는 bodyObject에서 해당 key값을 string으로 찾는다.
            jointObj.localPosition = GetVector3FromJoint(sourceJoint); //jointObj의 포지션을 sourceJoint로 바꿈
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }

            string a = jt.ToString();

            if (Points.ContainsKey(a))
            {
                Points[a] = GetVector3FromJoint(sourceJoint);
               // Debug.Log("Point : " + a + "   Value:" + Points[a]);
            }
            
            
           
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    public Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }


    public Dictionary<string, Vector3> Points = new Dictionary<string, Vector3>()
    {
        {"FootLeft",FootLeft },
        {"AnkleLeft",AnkleLeft },
        {"KneeLeft",KneeLeft },
        {"HipLeft",HipLeft },
        {"FootRight", FootRight},
        { "AnkleRight",AnkleRight},
        { "KneeRight",KneeRight},
        {"HipRight",HipRight },
        {"HandTipLeft",HandTipLeft },
        {"ThumbLeft",ThumbLeft },
        { "HandLeft",HandLeft},
        {"WristLeft" ,WristLeft},
        {"ElbowLeft",ElbowLeft },
        { "ShoulderLeft",ShoulderLeft},
        { "HandTipRight",HandTipRight},
        {"ThumbRight",ThumbRight },
        { "HandRight",HandRight},
        {"WristRight",WristRight },
        {"ElbowRight",ElbowRight},
        {"ShoulderRight" ,ShoulderRight},
        {"SpineBase",SpineBase },
        { "SpineMid",SpineMid},
        {"SpineShoulder",SpineShoulder },
        { "Neck",Neck},
        { "Head",Head}


    };





}
