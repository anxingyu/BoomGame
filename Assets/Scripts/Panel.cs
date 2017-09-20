using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public enum Dir
    {
        NONE,UP,DOWN,LEFT,RIGHT
    }
    public enum GameState
    {
        Init,
        Ready,
        Swap,
        Fall
    }
    public class Panel : MonoBehaviour
    {
        Transform cellsRoot;
        Transform elementsRoot;
        Transform effectRoot;
        List<CellPrefabs> cellList;
        List<CellPrefabs> dirtyCells = new List<CellPrefabs>();
        List<CellPrefabs> markCells = new List<CellPrefabs>();
        GameState state = GameState.Init;
        // Use this for initialization
        void Start()
        {
            StartCoroutine(PanelInit());
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator PanelInit()
        {
            yield return null;
            CreateRoots();
            yield return null;
            CreateCells();
            yield return null;
            CreateElements();

            state = GameState.Ready;
        }

        void CreateRoots()
        {
            cellsRoot = new GameObject("[CellsRoot]").transform;
            cellsRoot.SetParent(transform);
            elementsRoot = new GameObject("[ElementsRoot]").transform;
            elementsRoot.SetParent(transform);
            effectRoot = new GameObject("[EffectRoot]").transform;
            effectRoot.SetParent(transform);

        }
        void CreateCells()
        {
            int _rowCount = LevelDatabase.Instance.rowCount;
            int _colCount = LevelDatabase.Instance.colCount;
            if (cellList == null)
            {
                cellList = new List<CellPrefabs>();
            }
            cellList.Clear();
            Vector2 _offset = new Vector2(
               -_rowCount / 2f * CellDatabase.Instance.cellSize + CellDatabase.Instance.cellSize / 2f,
                _colCount / 2f * CellDatabase.Instance.cellSize - CellDatabase.Instance.cellSize / 2f
                );
            for (int y = 0, ymax = _colCount; y < ymax; y++)
            {
                for (int x = 0, xmax = _rowCount; x < xmax; x++)
                {
                    var _newCell = Instantiate(CellDatabase.Instance.CellPrefab, cellsRoot);
                    _newCell.OnCellCreate(x, y, CellDatabase.Instance.XY2Pos(x, y, _offset));
                    _newCell.RegisterEvent(OnCellDragCallback);
                    cellList.Add(_newCell);
                }
            }
            
        }
        void CreateElements()
        {
            List<int> _igrore = new List<int>();
            CellPrefabs _tmp1, _tmp2;
            foreach (var _cell in cellList)
            {
                _tmp1 = GetCellByDir(_cell, Dir.LEFT);
                if (_tmp1)
                {
                    _tmp2 = GetCellByDir(_tmp1, Dir.LEFT);
                    if (_tmp2 && _tmp1.bandingElement.Type == _tmp2.bandingElement.Type)
                    {
                        _igrore.Add(_tmp1.bandingElement.Type);
                    }
                }
                _tmp1 = GetCellByDir(_cell, Dir.UP);
                if (_tmp1)
                {
                    _tmp2 = GetCellByDir(_tmp1, Dir.UP);
                    if (_tmp2 && _tmp1.bandingElement.Type == _tmp2.bandingElement.Type)
                    {
                        _igrore.Add(_tmp1.bandingElement.Type);
                    }
                }

                var _newEle = ElementDatabase.Instance.RandomElement(_igrore.ToArray());
                _newEle.transform.SetParent(elementsRoot);
                _newEle.OnElementCreate(_cell.X, _cell.Y);
                _cell.bindElement(_newEle);

                _igrore.Clear();
            }
        }
        CellPrefabs GetCellByDir(CellPrefabs _cell, Dir _dir)
        {
            if (_cell == null) return null;
            int _x = _cell.X;
            int _y = _cell.Y;
            switch (_dir)
            {
                case Dir.UP:
                    _y -= 1;
                    break;
                case Dir.DOWN:
                    _y += 1;
                    break;
                case Dir.LEFT:
                    _x -= 1;
                    break;
                case Dir.RIGHT:
                    _x += 1;
                    break;
            }
            if (_x < 0 || _y < 0 || _x >= LevelDatabase.Instance.colCount || _y >= LevelDatabase.Instance.rowCount) return null;
            return cellList[_y * LevelDatabase.Instance.colCount + _x];
        }

        void SetToDirty(CellPrefabs _cell)
        {
            if(!dirtyCells.Contains(_cell))
            dirtyCells.Add(_cell);
        }
        void ClearDirtyCells()
        {
            dirtyCells.Clear();
        }
        void ClearMark(CellPrefabs _cell)
        {
            if (markCells.Contains(_cell))
                markCells.Remove(_cell);
        }
        void SetToMark(CellPrefabs _cell)
        {
            if (!markCells.Contains(_cell))
                markCells.Add(_cell);
        }
        void SetToMark(List<CellPrefabs> _lis)
        {
            markCells.AddRange(_lis);
        }
        void ClearMarkCells()
        {
            markCells.Clear();
        }
        CellPrefabs endDragCell;
        CellPrefabs endDragFindCell;
        void CancleDragSwap()
        {
            if (endDragCell && endDragFindCell)
            {
                endDragFindCell.SwapCellElement(endDragCell);
                endDragCell = null;
                endDragFindCell = null;
            }
            
        }

        void OnCellDragCallback(CellPrefabs _cell, Vector2 _dirty)
        {
            if (state != GameState.Ready) return;
            float xCos = Vector2.Dot(_dirty.normalized, Vector2.right);
            float yCos = Vector2.Dot(_dirty.normalized, Vector2.up);
            CellPrefabs _findCell = null;
            if (Mathf.Abs(xCos) > Mathf.Abs(yCos))
            {
                if (xCos > 0)
                {
                    _findCell = GetCellByDir(_cell, Dir.RIGHT);
                }
                else
                {
                    _findCell = GetCellByDir(_cell, Dir.LEFT);
                }
            }
            else
            {
                if (yCos > 0)
                {
                    _findCell = GetCellByDir(_cell, Dir.UP);
                }
                else
                {
                    _findCell = GetCellByDir(_cell, Dir.DOWN);
                }
            }
            if (_findCell)
            {
                state = GameState.Swap;
                _cell.SwapCellElement(_findCell);
                endDragCell = _cell;
                endDragFindCell = _findCell;

                SetToDirty(endDragCell);
                SetToDirty(endDragFindCell);

                StartCoroutine(MatchStep());
            }
        }
        IEnumerator MatchStep()
        {
            while (dirtyCells.Count > 0)
            {
                //等待所有dirtycell都idel
                //yield return StartCoroutine(WaitForDirtyCellsToIdel());
                var it = WaitForDirtyCellsToIdel();

                while (it.MoveNext())
                {
                    yield return null;
                }
                //检测是不是要消除元素
                ClearMarkCells();
                DoMatch();
                if (markCells.Count > 0)
                {
                    //要消除
                    //消除，然后继续进行后面的逻辑
                    endDragFindCell = null;
                    endDragCell = null;
                    ClearDirtyCells();
                }
                else
                {
                    //不消除
                    //换位，然后结束判断流程
                    CancleDragSwap();
                    //yield return StartCoroutine(WaitForDirtyCellsToIdel());
                    it = WaitForDirtyCellsToIdel();
                    while (it.MoveNext())
                    {
                        yield return null;
                    }
                    ClearDirtyCells();
                    break;
                }
                ClearDirtyCells();
                //将markCells中的元素回收
                RecyleMarkCell();

                it = WaitForMarkCellsToIdel();
                while (it.MoveNext())
                {
                    yield return null;
                }
                //将回收后空的cell进行上方元素的掉落填充，填充的元素依然是diry状态
                FallElement();
                //go to stop 1
            }
            state = GameState.Ready;
                yield return null;
           
        }
        IEnumerator WaitForDirtyCellsToIdel()
        {
            while (true)
            {
                bool isAllIdel = true;
                foreach (var _cell in dirtyCells)
                {
                    if (!_cell.Isidel)
                    {
                        isAllIdel = false;
                        break;
                    }
                }
                if (isAllIdel) break;
                yield return null;
            }
           
        }
        IEnumerator WaitForMarkCellsToIdel()
        {
            while (true)
            {
                bool isAllIdel = true;
                foreach (var _cell in markCells)
                {
                    if (!_cell.Isidel)
                    {
                        isAllIdel = false;
                        break;
                    }
                }
                if (isAllIdel) break;
                yield return null;
            }

        }
        void RecyleMarkCell()
        {
            List<CellPrefabs> _tmpMark = new List<CellPrefabs>(markCells);
            List<CellPrefabs> _tmpNewMark = new List<CellPrefabs>();
            while (_tmpMark.Count > 0)
            {
                foreach (var _cell in _tmpMark)
                {
                    if (_cell.bandingElement.State == ElementState.HBomb&&!_cell.bandingElement.IsEffected)
                    {
                        _cell.bandingElement.IsEffected = true;
                        List<CellPrefabs> _tmpList;
                        GetsameCellsH(_cell, out _tmpList, true);
                        foreach (var _new in _tmpList)
                        {
                            if(!_tmpNewMark.Contains(_new))
                                _tmpNewMark.Add(_new);
                        }                       
                    }
                    else if (_cell.bandingElement.State == ElementState.VBomb && !_cell.bandingElement.IsEffected)
                    {
                        _cell.bandingElement.IsEffected = true;
                        List<CellPrefabs> _tmpList;
                        GetsameCellsV(_cell, out _tmpList, true);
                        foreach (var _new in _tmpList)
                        {
                            if (!_tmpNewMark.Contains(_new))
                                _tmpNewMark.Add(_new);
                        }
                    }                 
                }
                _tmpMark.Clear();
                if (_tmpNewMark.Count > 0)
                {
                    foreach (var token in _tmpNewMark)
                    {
                        SetToMark(token);
                    }
                    _tmpMark.AddRange(_tmpNewMark);
                }
                _tmpNewMark.Clear();
            }
            foreach (var _cell in markCells)
            {
                _cell.Recycle();
            }     
        }
        void FallElement()
        {
            List<CellPrefabs> _emptyCell = new List<CellPrefabs>(markCells);
            List<CellPrefabs> _tmpNewEmptyCells = new List<CellPrefabs>();
            while (_emptyCell.Count > 0)
            {
                //把_emptyCell排序，保证遍历从最大（右下角）开始
                _emptyCell.Sort((x, y) => y.IndexInPanel - x.IndexInPanel);
                
                foreach (CellPrefabs _empty in _emptyCell)
                {
                    CellPrefabs _tmp = _empty;
                    while (true)
                    {
                        _tmp = GetCellByDir(_tmp, Dir.UP);
                        if (_tmp != null && !_tmp.IsEmpty)
                        {
                            //把现在的empty向上检测哪一个是掉落目标
                            _empty.FallElement(_tmp);
                            _tmpNewEmptyCells.Add(_tmp);

                            SetToDirty(_empty);
                            break;
                        }
                        else if (_tmp == null)
                        {
                            //找不到cell不为空，生产Element并移动
                            Element _newEle = ElementDatabase.Instance.RandomElement();
                            _newEle.transform.SetParent(elementsRoot);
                            _empty.FallElement(_newEle);
                            _newEle.transform.position = _empty.transform.position + new Vector3(0, CellDatabase.Instance.cellSize * 3, 0);
                            SetToDirty(_empty);
                            break;
                        }
                    }                    
                }
                _emptyCell.Clear();
                //掉落后新的empty生产
                if (_tmpNewEmptyCells.Count > 0)
                {
                    _emptyCell.AddRange(_tmpNewEmptyCells);
                    _tmpNewEmptyCells.Clear();
                }

            }
        }
       void DoMatch()
        {
            //将确定要消除哪些元素，将其放入markCells列表中
            foreach (var _cell in dirtyCells)
            {
                int countV;
                int countH;
                List<CellPrefabs> _tmpList;
                GetsameCellsV(_cell, out _tmpList);
                countV = _tmpList.Count;
                if (_tmpList.Count > 2)
                {
                    foreach (var token in _tmpList) SetToMark(token);
                }
                GetsameCellsH(_cell, out _tmpList);
                countH = _tmpList.Count;
                if (_tmpList.Count > 2)
                {
                    foreach (var token in _tmpList) SetToMark(token);
                }
                if (SpecialMatch.IsSpecialMatch(_cell, countV, countH))
                {
                    ClearMark(_cell);
                }

            }
        }
        void GetsameCellsV(CellPrefabs _cell ,out List<CellPrefabs> ret, bool ignoreSame = false)
        {
            List<CellPrefabs> _ret = new List<CellPrefabs>() { _cell };
            int _targetType = _cell.bandingElement.Type;
            CellPrefabs _tmp = _cell;
            while (true)
            {
                _tmp =  GetCellByDir(_tmp, Dir.UP);
                if (_tmp && !_tmp.IsEmpty&& (_tmp.bandingElement.Type == _targetType || ignoreSame))
                    _ret.Add(_tmp);
                else
                    break;             
            }
            _tmp = _cell;
            while (true)
            {
                _tmp = GetCellByDir(_tmp, Dir.DOWN);
                if (_tmp && !_tmp.IsEmpty && (_tmp.bandingElement.Type == _targetType || ignoreSame))
                    _ret.Add(_tmp);
                else
                    break;
            }
            ret = _ret;
        }
        void GetsameCellsH(CellPrefabs _cell, out List<CellPrefabs> ret, bool ignoreSame = false)
        {
            List<CellPrefabs> _ret = new List<CellPrefabs>() { _cell };
            int _targetType = _cell.bandingElement.Type;
            CellPrefabs _tmp = _cell;
            while (true)
            {
                _tmp = GetCellByDir(_tmp, Dir.LEFT);
                if (_tmp && !_tmp.IsEmpty && (_tmp.bandingElement.Type == _targetType || ignoreSame))
                    _ret.Add(_tmp);
                else
                    break;
            }
            _tmp = _cell;
            while (true)
            {
                _tmp = GetCellByDir(_tmp, Dir.RIGHT);
                if (_tmp && !_tmp.IsEmpty && (_tmp.bandingElement.Type == _targetType || ignoreSame))
                    _ret.Add(_tmp);
                else
                    break;
            }
            ret = _ret;
        }
    }
}
