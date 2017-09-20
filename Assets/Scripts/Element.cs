using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace game
{
    public class Element : MonoBehaviour
    {
        public SpriteRenderer sr;

        public bool IsEffected
        {
            get;
            set;
        }

        public int Type
        {
            get;
            set;
        }
        public ElementState State
        {
            get;
            set;
        }

        public int X
        {
            get;
            private set;
        }
        public int Y
        {
            get;
            private set;
        }

        public void ChangeElementState(ElementState _state)
        {
            State = _state;
            ElementPicChange();
        }

        public void OnElementCreate(int _x, int _y)
        {
            X = _x;
            Y = _y;
            gameObject.name = "Element_" + X + "_" + Y;
        }
        public void ElementPicChange()
        {
            if (sr != null)
            {
                sr.sprite = ElementDatabase.Instance.GetSprite(Type, State);
            }
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}