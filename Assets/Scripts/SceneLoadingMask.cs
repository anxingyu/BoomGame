using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class SceneLoadingMask : MonoBehaviour {

    public CanvasGroup cg;
    public float duration = 0.54f;
    public Image i;
    public Text textC;
    public void Init()
    {

        i.gameObject.SetActive(false);
    }

    public void ShowMask(System.Action _fin)
    {
   
        if (!i.gameObject.activeInHierarchy)
        {
            i.gameObject.SetActive(true);
        }
        cg.alpha = 0f;
        cg.DOFade(1f, duration).OnComplete(() =>
        {
            if (_fin != null)
                _fin();
        }
        );

        //DOTween.To(()=> { return cg.alpha; },(float x) => { cg.alpha = x; },0,1).OnComplete(() =>
        //{
        //    if (_fin != null)
        //        _fin();
        //}
        //);
    }

    public void HideMask(System.Action _fin)
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        cg.alpha = 1f;
        cg.DOFade(0f, duration).OnComplete(() =>
        {
            if (_fin != null)
                _fin();

            gameObject.SetActive(false);
        }
        );
    }
    public void updateTest(float p)
    {
        textC.text = p *100+ "%";
    }
}
