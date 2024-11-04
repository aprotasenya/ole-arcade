﻿using System;

namespace Match3
{
    public partial class GridObject<T>
    {
        GridSystem2D<GridObject<T>> grid;
        int x;
        int y;
        T gem;

        public GridObject(GridSystem2D<GridObject<T>> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        internal void SetValue(T value)
        {
            this.gem = value;
        }
    }
}
