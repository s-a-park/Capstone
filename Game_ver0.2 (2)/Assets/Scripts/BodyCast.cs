using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

//Compare between 
public class BodyCast : BodySourceView
{

    static int count = 0;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void Update()
    {
        
    }

    /*
    private void Awake()
    {
        StartCoroutine(CheckPoint());
    }
    private IEnumerator CheckPoint()
    {
       foreach (KeyValuePair<string, Vector3> items in Points)
        {
            // Debug.Log("Point    " + items.Key + "            Value" + items.Value);
            Debug.Log("Count   " + count.ToString());
            count++;
        }
        yield return new WaitForSeconds(10.0f);
    }
    */
    
}
