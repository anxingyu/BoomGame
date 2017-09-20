using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.tool;
namespace game
{
    public enum CellState
    {
        Idel,
        Swap,
        Recycle,
        Fall
    }
    public class CellDatabase : Singletun<CellDatabase>
    {
        public CellPrefabs CellPrefab;
        public float cellSize = 1;
        public float cellDragSwapDuration = 0.2f;
        public float cellDragZonePect = 0.2f;
        public float cellDragZone
        {
            get { return cellSize * cellDragZonePect; }
        }
        public Vector2 XY2Pos(int _x, int _y , Vector2 _offset)
        {
            return new Vector2(_x * cellSize + _offset.x, _y*cellSize *-1 +_offset.y);
        }
    }
}
