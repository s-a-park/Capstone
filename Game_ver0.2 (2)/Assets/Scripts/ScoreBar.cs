using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreBar : MonoBehaviour
{
    public Slider TotalBar;
    public Text text;
    public Text message;

    public void ChangeScene()
    {
        SceneManager.LoadScene("Finish_Game");
    }

    public void ChangeScene2()
    {
        SceneManager.LoadScene("Failed_Game");
    }

    // Start is called before the first frame update
    void Start()
    {
        TotalBar.value = 0;
        message.text = "시작합니다!";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            TotalBar.value += 5;
            float t = TotalBar.value;
            text.text = t+"/100";
            message.text = "잘하고 있어요!";

        }
        else if (Input.GetKey(KeyCode.H)){
            message.text = "동작을 더 정확히 하세요!";
        }
      
        if(TotalBar.value == 100)
        {
            message.text = "성공했어요!";
            ChangeScene();

        }

        /*if(TotalBar.value != 100)
        {                                                     동작이 끝날 때까지 100못채우면 failed 씬으로 보냄
            message.text = "실패했어요..";
            ChangeScene2();
        }*/
    }

    
}
