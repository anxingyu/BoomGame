using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace game
{
    public class MenuScene : MonoBehaviour
    {
        AsyncOperation ao;
        
        // Use this for initialization
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {
            if (ao != null)
                Debug.Log(ao.progress);
        }
        void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 20), "cheng"))
            {
                //SceneManager.LoadScene(1);
                SceneLoader.LoadScene("game");
            }
        }
    }
}
