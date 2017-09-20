using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace game
{
    public abstract class SceneConstuction : MonoBehaviour
    {
        // Use this for initialization
        IEnumerator Start()
        {
            // 进行构造过程
            IEnumerator _it = DoConstruction();
            while (_it.MoveNext())
            {
                yield return null;
            }
            // 构造过程结束的调用
            OnConstructorFinish();
        }

        protected virtual IEnumerator DoConstruction()
        {
            // 所有的加载的资源 要再此地进行
            yield return null;
        }

        void OnConstructorFinish()
        {
            SceneLoader.OnSceneConstructorFinish();
        }

    }
}
