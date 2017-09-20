using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.tool;

namespace game
{
    public enum ElementState
    {
        None = -1,
        Normal,
        Bomb,
        HBomb,
        VBomb,
        Free
    }
    public class ElementDatabase :Singletun<ElementDatabase>
    {
        [System.Serializable]
        public struct ElementPicInfo
        {
            public Sprite[] sprList;
        }
        Queue<Element> elementPool = new Queue<Element>(40);

        Transform elementPoolRoot;

        public Element GetElementFromPool(int _type, ElementState _state = ElementState.Normal)
        {
            Element _ret = null;
            if (elementPool.Count > 0)
            {
                _ret = elementPool.Dequeue();
                _ret.Type = _type;
                _ret.State = _state;
                _ret.transform.localScale = Vector2.one;
                _ret.gameObject.SetActive(true);
                _ret.ElementPicChange();

            }
            else
            {
                _ret = Instantiate(elementPrefab);
                _ret.Type = _type;
                _ret.State = _state;
                _ret.ElementPicChange();
                
            }
            return _ret;
        }
        public void RecycleElement(Element _ele)
        {
            if (_ele)
            {
                _ele.gameObject.SetActive(false);
                _ele.transform.SetParent(elementPoolRoot);
                elementPool.Enqueue(_ele);
            }
        }
        public Element elementPrefab;
        public ElementPicInfo[] ElementList;

        public Element CreateElement(int _type, ElementState _state = ElementState.Normal)
        {
            if (elementPrefab == null) return null;
            if (_type < 0 || _type >= ElementList.Length) return null;
            if (_state < 0 || _state > ElementState.Free) return null;
            return GetElementFromPool(_type, _state);
        }

        public Sprite GetSprite(int _type, ElementState _state = ElementState.Normal)
        {
            if (_type < 0 || _type >= ElementList.Length) return null;
            if (_state < 0 || _state > ElementState.Free) return null;
            return ElementList[_type].sprList[(int)_state];
        }

        public Element RandomElement( ElementState _state = ElementState.Normal)
        {
            return CreateElement(Random.Range(0, ElementList.Length), _state);
        }

        public Element RandomElement(int[] _igrore, ElementState _state = ElementState.Normal)
        {
            if (_igrore == null || _igrore.Length == 0)
                return RandomElement();

            List<int> randomList = new List<int>();

            for (int i = 0; i < ElementList.Length; ++i)
            {
                bool Isigrore = false;
                foreach (var token in _igrore)
                {
                    if (token == i)
                    {
                        Isigrore = true;
                        break;
                    }
                }
                if (!Isigrore)
                {
                    randomList.Add(i);
                }
            }
            return CreateElement(randomList[Random.Range(0, randomList.Count)], _state);
        }
        // Use this for initialization
        void Start()
        {
            if (elementPoolRoot == null)
                elementPoolRoot = new GameObject("[ElementPoolRoot]").transform;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}