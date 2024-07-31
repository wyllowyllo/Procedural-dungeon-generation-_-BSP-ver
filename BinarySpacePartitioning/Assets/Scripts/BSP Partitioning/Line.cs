using UnityEngine;


public enum Orientation
{
    Horizontal = 0, Vertical = 1
}
public class Line
{
    Orientation orientation;
    Vector2Int coordinates;

    public Line(Orientation orientation, Vector2Int coordinates)
    {
        this.orientation = orientation;
        this.coordinates = coordinates;
    }

    public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
    public Orientation Orientation { get => orientation; set => orientation = value; }
}
