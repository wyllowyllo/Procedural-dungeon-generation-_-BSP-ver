public class Block
{
    int gridX, gridY;
    gridTile blockTile;
    Block prevBlock;

    int totalWeight;
    int fromStartPointWeight, fromDestPointWeight;
    

    public Block(int gridX,int gridY,gridTile tile)
    {
        this.gridX = gridX;
        this.gridY=gridY;
        this.blockTile = tile;
        fromStartPointWeight = 0;
        fromDestPointWeight = 0;
        totalWeight = fromStartPointWeight + fromDestPointWeight;
    }

    public gridTile BlockTile { get => blockTile; }
    
    public int GridX { get => gridX; }
    public int GridY { get => gridY; }
    public int FromStartPointWeight { get => fromStartPointWeight; set => fromStartPointWeight = value; }
    public int FromDestPointWeight { get => fromDestPointWeight; set => fromDestPointWeight = value; }
    public Block PrevBlock { get => prevBlock; set => prevBlock = value; }
    public int TotalWeight { get => totalWeight; set => totalWeight = value; }

    public void UpdateTotalWeight()
    {
        totalWeight = fromStartPointWeight + fromDestPointWeight;
    }
}