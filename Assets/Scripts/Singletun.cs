using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace game.tool
{
    public class Singletun<T> : MonoBehaviour
        where T : MonoBehaviour
    {

        public static T Instance
        {
            get;
            private set;
        }


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
