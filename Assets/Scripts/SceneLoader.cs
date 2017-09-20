using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using game.tool;
namespace game
{
    public class SceneLoader : Singletun<SceneLoader>
    {
        static bool stepFin = false;
        public SceneLoadingMask mask;
        float a;
        AsyncOperation _ao;


        public static void LoadScene(string _name)
        {
            if (Instance == null)
            {
                return;
            }

            Instance.StartCoroutine(Instance.DoLoadScene(_name));
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
        }


        IEnumerator DoLoadScene(string _name)
        {            
            // 第一阶段 蒙mask
            stepFin = false;
            if (mask != null)
            {
                mask.ShowMask(() => stepFin = true);
            }
            // mask 蒙好了
            while (!stepFin)
            {
                yield return null;
            }

            stepFin = false;
            // 换场景
            _ao = SceneManager.LoadSceneAsync(_name);
            while (!_ao.isDone)
            {
                //Loading Scene:sc_Game--10%
                Debug.Log("Loading Scene:" + _name + "--" + (_ao.progress * 100f).ToString() + "%");
               
                
               
                yield return null;
            }

            while (!stepFin)
            {
                yield return null;
            }
            Debug.Log("Loading!!!");
            // 去掉 mask
            stepFin = false;
            if (mask != null)
            {
                mask.HideMask(() => stepFin = true);
            }
            while (!stepFin)
            {
                yield return null;
            }
        }

        public static void OnSceneConstructorFinish()
        {
            stepFin = true;
        }

    }
}
