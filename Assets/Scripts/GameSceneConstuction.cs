using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace game
{
    public class GameSceneConstuction : SceneConstuction
    {
        protected override IEnumerator DoConstruction()
        {
            for (int i = 0; i < 1000; i++)
            {
                Debug.Log("asdasdasdas"+i);
                yield return null;
            }
        }
    }
}
