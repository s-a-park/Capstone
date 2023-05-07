using UnityEngine;
using System.Collections;

public class InfraredSourceView : MonoBehaviour 
{
    public GameObject InfraredSourceManager;         //적외선 프레임 매니저 파일 유니티에 연동된거
    private InfraredSourceManager _InfraredManager;     //매니저 파일 연동
    
    void Start () 
    {
        gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));   //가져온 데이터들을 렌더링 2차원 벡터 형식
    }
    
    void Update()
    {
        if (InfraredSourceManager == null)                         //매니저파일로 가져온 적외선 프레임 데이터를 유니티 상에 보여지게 함
        {
            return;
        }
        
        _InfraredManager = InfraredSourceManager.GetComponent<InfraredSourceManager>();
        if (_InfraredManager == null)
        {
            return;
        }
    
        gameObject.GetComponent<Renderer>().material.mainTexture = _InfraredManager.GetInfraredTexture();
    }
}
