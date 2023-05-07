using UnityEngine;
using UnityEngine.Networking;
using System.collections.Generic;

using Windows.Kinect;
using Joint=Windows.Kinect.Joint;

public class BodySourceView : MonoBehaviour 
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;

    private Dictionary<ulong,GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType>
    {
      JointType.HandLeft,
      JointType.HandRight,

    };

    void Update(){
        // get kinect Data
        Body[] data = mBodySourceManager.GetData();
        if(data == null) return;

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if(body == null) continue;

            if(body.IsTracked)
             trackedIds.Add(body.TrackingId);
        }

        //delete kinect data
        List<ulong> KnownIds = new List<ulong>(mbodies.Keys);
        foreach(ulong TrackingId in KnownIds){
            if(!trackedIds.Contains(TrackingId)){
                Destroy(mBodies[TrackingId]);

                mBodies.Remove(trackedId);
            }
        }

        //create kinect body
        foreach (var body in data){
            if(body == null) continue;

            if(body.IsTracked){
                if(!mBodies.ContainsKey(body.TrackingId))
                mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);

                UpdateBodyObject(body, mBodies[body.TrackingId]);
            }
        }
    }

    private GameObject CreateBodyObject(ulong id){
        //부모 몸체 생성
        GameObject body = new GameObject("Body : "+ id);


        //joint 생성
        foreach(JointType joint in _joints){
        GameObject newJoint = Instantiate(mJointObject);
        newJoint.name = joint.ToString();


        newJoint.transform.parent = body.transform;
        }
        return body;
    }

    private void UpdateBodyObject(Body body, GameObject bodyObject){
        //joint 업뎃
        foreach(JointType _joints in _joints){
            Joint sourceJoint = body.Joints[_joints];
            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
            targetPosition.z = 0;

            Transform jointObject = bodyObject.transform.Find(_joints.ToString());
            jointObject.position = targetPosition;
        }
    }

    private Vector3 GetVector3FromJoint(Joint joint){

        return new Vector3(joint.position.X *10, joint.position.Y * 10, joint.position.Z * 10);
    }
}
