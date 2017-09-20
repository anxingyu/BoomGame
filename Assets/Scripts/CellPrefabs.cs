using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
namespace game
{
    public class CellPrefabs : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public SpriteRenderer sr;
        public Element bandingElement
        {
            get;
            private set;
        }
        CellState state = CellState.Idel;
        public bool Isidel { get { return state == CellState.Idel; } }
        public bool IsEmpty { get { return bandingElement == null; } }
        event Action<CellPrefabs, Vector2> OnCelllDragEvent;
        public void RegisterEvent(Action<CellPrefabs, Vector2> _call)
        {
            OnCelllDragEvent += _call;
        }
        public void UnRegisterEvent(Action<CellPrefabs, Vector2> _call)
        {
            OnCelllDragEvent -= _call;
        }
        Vector2 BeginDragPos;
        bool OnDraging = false;
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
        public int IndexInPanel
        {
            get { return Y * LevelDatabase.Instance.colCount + X; }
        }
        public void OnCellCreate(int _x, int _y, Vector2 _pos)
        {
            X = _x;
            Y = _y;
            gameObject.name = "Cell_" + X + "_" + Y;
            gameObject.transform.position = _pos;
            if ((X + Y) % 2 == 0 && sr)
                sr.color = new Color(1, 1, 1, 0.5f);
        }
        public void bindElement(Element _ele)
        {
            bandingElement = _ele;
            bandingElement.transform.position = transform.position;
        }

        public void SwapCellElement(CellPrefabs _cell)
        {
            state = CellState.Swap;

            var _old = _cell.bandingElement;
            _cell.SwapElement(bandingElement);
            SwapElement(_old);
        }
        public void SwapElement(Element _ele)
        {
            bandingElement.transform.DOMove(_ele.transform.position, CellDatabase.Instance.cellDragSwapDuration).OnComplete(StateToIdel);
            bandingElement = _ele;
        }

        public void FallElement(CellPrefabs _other)
        {
            bandingElement = _other.bandingElement;
            _other.bandingElement = null;
            DropElement(bandingElement);
        }
        public void FallElement(Element _ele)
        {
            bandingElement = _ele;
            bandingElement.OnElementCreate(X, Y);
            DropElement(bandingElement);
        }
        public void DropElement(Element _ele)
        {
            state = CellState.Fall;
            bandingElement.transform.DOMove(transform.position,5).SetSpeedBased().OnComplete(StateToIdel);
        }

        void StateToIdel()
        {
            state = CellState.Idel;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            BeginDragPos = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (OnDraging || !Isidel) return;
            Vector2 _dirty = eventData.position - BeginDragPos;
            if (_dirty.sqrMagnitude > CellDatabase.Instance.cellDragZone)
            {
                OnDraging = true;
                OnCelllDragEvent(this, _dirty);
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            OnDraging = false;
        }
        public void Recycle()
        {
            state = CellState.Recycle;
            bandingElement.transform.DOScale(0, 0.2f).OnComplete(()=> {
                ElementDatabase.Instance.RecycleElement(bandingElement);
                bandingElement = null;
                state = CellState.Idel;
            });

        }
    }
}
