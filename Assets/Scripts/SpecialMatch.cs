using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class SpecialMatch
    {
        int markCellcountV;
        int markCellcountH;

        CellPrefabs targetCell;
        ElementState _eleState;
       public static bool IsSpecialMatch(CellPrefabs _cell, int v, int h)
        {
            SpecialMatch _sm = new SpecialMatch();
            _sm.markCellcountV = v;
            _sm.markCellcountH = h;
            _sm.targetCell = _cell;
            _sm.CheckEleState();
            if (_sm._eleState > ElementState.Normal)
            {
                _sm.targetCell.bandingElement.IsEffected = false;
                _sm.targetCell.bandingElement.ChangeElementState(_sm._eleState);
                return true;
            }
            return false;
        }
        void CheckEleState()
        {
            if (markCellcountV == 5 || markCellcountH == 5)
            {
                _eleState = ElementState.Free;
            }
            else if (markCellcountV > 2 && markCellcountH > 2)
            {
                _eleState = ElementState.Bomb;
            }
            else if (markCellcountV == 4)
            {
                _eleState = ElementState.VBomb;
            }
            else if (markCellcountH == 4)
            {
                _eleState = ElementState.HBomb;
            }
            else
            {
                _eleState = ElementState.Normal;
            }
        }
    }
}

