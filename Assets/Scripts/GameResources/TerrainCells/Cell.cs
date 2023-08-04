using Assets.Scripts.GameResources.MapCreatures;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private GameObject _arrow;
    [SerializeField] private GameObject _point;
    [SerializeField] private TerrainTypes _type;

    [field: SerializeField] public GameMapObjectType GameMapObjectType { get; private set; }
    public bool IsTargetPoint { get; private set; }
    public TerrainTypes Type => _type;
    [field: SerializeField] public string InteractiveMapObjectId { get; private set; } = "";
    [field: SerializeField] public string ParentObjectId { get; private set; } = "";
    public int CellPathCost { get; private set; }
    public string StartCellId { get; set; }

    private Color _arrowColor;
    private GameModel _gameModel;
    public GameMapObject CreatureModelObject { get; private set; }
    public HeroModelObject HeroModelObject { get; private set; }
    public GameMapObject GameMapObject { get; private set; }
    public Transform Arrow => _arrow.transform;
    private string _baseInteractiveMapObjectId = "";
    public int x { get; set; }
    public int y { get; set; }  

    public void SetGameMapObject(GameMapObject gameMapObject)
    {
        GameMapObject = gameMapObject;
    }

    public void SetBaseInteractiveMapObjectId(string interactiveMapObjectId)
    {
        _baseInteractiveMapObjectId = interactiveMapObjectId;
    }

    public void BackToBaseInteractiveMapObjectID()
    {
        SetInteractiveMapObjectId(_baseInteractiveMapObjectId);
    }

    public void SetInteractiveMapObjectId(string interactiveMapObjectId)
    {

        if (InteractiveMapObjectId != "")
            return;
        InteractiveMapObjectId = interactiveMapObjectId;
        if (interactiveMapObjectId != "")
        {
            int index = _gameModel.InteractiveObjectIdToGameObjectType.FindIndex(x => x.Contains(InteractiveMapObjectId));
            GameMapObjectType = (GameMapObjectType)(index + 1);
        }
        else
        {
            GameMapObjectType = GameMapObjectType.NULL;
        }
    }

    public void SetGameMapObjectType(GameMapObjectType gameMapObjectType)
    {
        GameMapObjectType = gameMapObjectType;
    }

    public void SetParentObjectId(string id)
    {
        GameMapObjectType = GameMapObjectType.NULL;
        int index = _gameModel.InteractiveObjectIdToGameObjectType.FindIndex(x => x.Contains(ParentObjectId));
        GameMapObjectType = (GameMapObjectType)(index + 1);
        ParentObjectId = id;
    }

    public void ResetInteractiveMapObject()
    { 
        GameMapObjectType = GameMapObjectType.NULL;
        CreatureModelObject = null;
        InteractiveMapObjectId = "";
    }

    public void SetModelObject(GameMapObject creatureModelObject)
    {
        CreatureModelObject = creatureModelObject;
    }

    public void ResetHeroModelObject()
    {
        CreatureModelObject = null;
    }

    public void SetGameModel(GameModel gameModel)
    {
        _gameModel = gameModel;
    }

    public void SetCost(int cost)
    {
        CellPathCost = cost;
    }

    public void DrawArrow(Transform target)
    {
        _arrow.SetActive(true);
        _arrow.transform.LookAt(target);
        _arrow.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = _arrowColor;
    }

    public void SetColorArrow(Color color)
    {
        _arrowColor = color;
    }

    public void OnTargetMovePoint()
    {
        _point.SetActive(true);
        IsTargetPoint = true;
    }

    public void ResetCell()
    {
        IsTargetPoint = false;
        _arrowColor = Color.green;
        _arrow.SetActive(false);
        _point.SetActive(false);
    }

}