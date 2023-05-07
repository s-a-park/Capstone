using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CStoOptions : MonoBehaviour
{
    public void ChangeSceneBtn()
    {
        SceneManager.LoadScene("Scene_Options");
    }
}
