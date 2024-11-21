using System;

namespace Match3
{
    public partial class GridObject<T>
    {
        internal GridSystem2D<GridObject<T>> grid;
        internal int x;
        internal int y;
        T item;

        public GridObject(GridSystem2D<GridObject<T>> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetItem(T item)
        {
            this.item = item;
        }

        public T GetItem() => item;


    }
}
