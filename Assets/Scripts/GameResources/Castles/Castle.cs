using System.Collections;
using UnityEngine;

public class Castle : GameMapObject
{
    [SerializeField] private Sprite _castleIcon;
    public int DicCastleID { get; private set; }
    public string ObjectID => MapObjectID;
    public Sprite CastleIcon => _castleIcon;
    public Vector2Int GatePosition { get; private set; }
    public CastleObjectFullInfo CastleInfo { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public void SetCastleObjectFullInfo(CastleObjectFullInfo castleObjectFullInfo)
    {
        CastleInfo = castleObjectFullInfo;
    }

    public void SetSize(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public void SetGatePosition(Vector2Int vector2Int)
    {
        GatePosition = vector2Int;
    }

    public void SetCastleID()
    {

    }

    public void SetDicCastleID(int castleID)
    {
        //if (castleID < 0)
        //    castleID = 0;
        DicCastleID = castleID;
    }
}