using Assets.Scripts.GameResources.MapCreatures;
using Assets.Scripts.MVC.Game;
using Assets.Scripts.MVC.Game.Views;
using Assets.Scripts.MVC.Ground;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameModel : IGameDataHandler
{
    public event Action<Player> OnEnteredInTurn;
    public event Action OnUpdatedTurn;
    public event Action OnExitFromTurn;
    public event Action<int> OnTickTimer;
    public event Action OnStatedTurn;
    public event Action<Player> OnPlayerEnterInTurn;
    public event Action OnEnterInBattleScene;
    public event Action OnEnterInGameFromBattleScene;
    public event Action OnUpdateGameDate;

    public string GameSessionID { get; private set; }

    private List<Castle> _castlesTurn = new List<Castle>();
    private List<HeroModelObject> _heroModelObjectsTurn = new List<HeroModelObject>();
    private List<Player> _players = new List<Player>();

    private GameAndBattleCommandsSender _gameAndBattleCommandsSender;
    public HeroModelObject SelectedHero { get; private set; }
    public HeroModelObject HeroInFight { get; private set; }
    private Cell[,] _cells;
    private Cell[,] _cellsIninversionSpace;
    private List<ResourceSturcture> _resourcesSturctures;
    private List<MineStructure> _mineSturctures;
    private List<Castle> _castles;
    private List<HeroModelObject> _heroModelObjects;
    private List<CreatureModelObject> _mapCreatures;
    private GroundModel _groundModel;
    private GameMapObject _creatureModelObject;
    private Cell _cellHasFightedCreature;
    private GameTurnView _gameTurnView;
    public bool IsLasFightWin { get; private set; }
    public bool IsAttacked { get; private set; }

    public int DaysCounter { get; private set; }
    public int WeeksCounter { get; private set; }
    public int MonthsCounter { get; private set; }

    public List<List<string>> InteractiveObjectIdToGameObjectType => new List<List<string>>
                                {
                                    _castles.Select(item => item.MapObjectID).ToList(),
                                    _mapCreatures.Select(item => item.MapObjectID).ToList(),
                                    _heroModelObjects.Select(item => item.MapObjectID).ToList(),
                                    _resourcesSturctures.Select(item => item.MapObjectID).ToList(),
                                    _mineSturctures.Select(item => item.MapObjectID).ToList()
                                };

    public bool IsCurrentTurn { get; private set; }
    public bool IsHeroMove { get; private set; }
    public Transform SceneObjectsParent { get; private set; }
    public Transform TerrainObjectsParent { get; private set; }
    public Vector2Int MapSize { get; private set; }

    public Vector2Int PositionSelectedHeroOnGrid
    {
        get
        {
            if (SelectedHero != null)
            {
                return SelectedHero.transform.position.ToVector2IntInHeroPosition();
            }
            return Vector2Int.zero;
        }
    }
    public IReadOnlyList<Castle> CastlesTurn => _castlesTurn;
    public IReadOnlyList<HeroModelObject> HeroModelObjectsTurn => _heroModelObjectsTurn;
    public IReadOnlyCollection<Player> Players => _players;
    public IReadOnlyList<Castle> Castles => _castles;
    public Cell[,] Cells => _cells;

    public GameModel(GroundModel groundModel)
    {
        _groundModel = groundModel;
    }

    public void SetLastFightResult(bool result)
    {
        IsLasFightWin = result;
    }

    public void SetMapSize(Vector2Int mapSize)
    {
        MapSize = mapSize;
    }

    public void Init(GameTurnView gameTurnView, GameAndBattleCommandsSender gameAndBattleCommandsSender)
    {
        _gameTurnView = gameTurnView;
        _gameAndBattleCommandsSender = gameAndBattleCommandsSender;

    }

    public void SetHeroInFight(HeroModelObject heroModelObject)
    {
        HeroInFight = heroModelObject;
    }

    public void ResetHeroInFight()
    {
        HeroInFight = null;
    }

    public void StartAttack()
    {
        IsAttacked = true;
    }

    public void ExitFromFight()
    {
        IsAttacked = false;
    }

    public void Surrender()
    {
        _gameAndBattleCommandsSender.SendEndTurnRequest();
    }

    public void SetDate(int daysCounter, int weeksCounter, int monthsCounter)
    {
        DaysCounter = daysCounter;
        WeeksCounter = weeksCounter;
        MonthsCounter = monthsCounter;
        OnUpdateGameDate?.Invoke();
    }

    public void InitedData()
    {
        foreach (var castle in _castles)
        {
            TryGetCellByPosition(new Vector2Int(castle.GatePosition.x, castle.GatePosition.y * -1), out Cell cell);
            cell.SetInteractiveMapObjectId(castle.MapObjectID);
            cell.GetComponent<MeshRenderer>().material.color = Color.black;
            cell.SetBaseInteractiveMapObjectId(castle.MapObjectID);
            for (int x = castle.GatePosition.x; x < castle.Width + castle.GatePosition.x; x++)
            {
                for (int y = castle.GatePosition.y; y > castle.GatePosition.y - castle.Height; y--)
                {
                    int rows = _cells.GetUpperBound(0) + 1;    // количество строк
                    int columns = _cells.Length / rows;        // количество столбцов
                    _cells[x, y * -1].SetParentObjectId(castle.MapObjectID);

                    if (x < rows && y < columns)
                        _cells[x, y * -1].SetParentObjectId(castle.MapObjectID);
                }
            }
            //_cells[castle.GatePosition.x + 1, castle.GatePosition.y * -1].SetParentObjectId(castle.MapObjectID);
            //_cells[castle.GatePosition.x + 1, (castle.GatePosition.y + 1) * -1].SetParentObjectId(castle.MapObjectID);
            _cells[castle.GatePosition.x + 1, (castle.GatePosition.y - 1) * -1].SetInteractiveMapObjectId(castle.MapObjectID);
            castle.SetGatePosition(new Vector2Int(castle.GatePosition.x + 1, (castle.GatePosition.y - 1)));
            //_cells[castle.GatePosition.x - 1, (castle.GatePosition.y + 1) * -1].SetParentObjectId(castle.MapObjectID);
            //_cells[castle.GatePosition.x - 1, castle.GatePosition.y * -1].SetParentObjectId(castle.MapObjectID);
            castle.SetCellPlace(cell);
        }
        foreach (var hero in _heroModelObjects)
        {
            var position = hero.transform.position.ToVector2IntInHeroPosition();
            TryGetCellByPosition(new Vector2Int(position.x, position.y * -1), out Cell cell);
            hero.SetCellStayed(cell);
            cell.SetInteractiveMapObjectId(hero.MapObjectID);
            cell.SetBaseInteractiveMapObjectId(hero.MapObjectID);
            hero.SetCellPlace(cell);
        }
        if (_heroModelObjects.Count > 0 && _heroModelObjects[0] != null)
        {
            SetSelectedHero(_heroModelObjects[0]);
            //_gameTurnView.SelectHeroObject(SelectedHero);
        }
        foreach (var creature in _mapCreatures)
        {
            var position = creature.transform.position.ToVector2IntInHeroPosition();
            TryGetCellByPosition(new Vector2Int(position.x, position.y * -1), out Cell cell);
            cell.SetInteractiveMapObjectId(creature.MapObjectID);
            cell.SetModelObject(creature);
            cell.SetBaseInteractiveMapObjectId(creature.MapObjectID);
            creature.SetCellPlace(cell);
        }
        foreach (var resource in _resourcesSturctures)
        {
            var position = resource.transform.position.ToVector2IntInHeroPosition();
            TryGetCellByPosition(new Vector2Int(position.x, position.y * -1), out Cell cell);
            cell.SetGameMapObject(resource);
            cell.SetInteractiveMapObjectId(resource.MapObjectID);
            resource.SetCellPlace(cell);
        }
        foreach (var mine in _mineSturctures)
        {
            TryGetCellByPosition(new Vector2Int(mine.GatePosition.x, mine.GatePosition.y * -1), out Cell cell);
            cell.SetInteractiveMapObjectId(mine.MapObjectID);
            cell.GetComponent<MeshRenderer>().material.color = Color.black;
            cell.SetBaseInteractiveMapObjectId(mine.MapObjectID);

            for (int x = mine.GatePosition.x; x <= mine.Width + mine.GatePosition.x; x++)
            {
                for (int y = mine.GatePosition.y; y > mine.GatePosition.y - mine.Height; y--)
                {
                    int rows = _cells.GetUpperBound(0) + 1;    // количество строк
                    int columns = _cells.Length / rows;        // количество столбцов
                    if (x < rows && y < columns)
                    {
                        _cells[x, y * -1].SetParentObjectId(mine.MapObjectID);
                        _cells[x, y * -1].SetGameMapObjectType(GameMapObjectType.MINE);
                    }
                }
            }
            _cells[mine.GatePosition.x + 1, (mine.GatePosition.y - 1) * -1].SetInteractiveMapObjectId(mine.MapObjectID);
            mine.SetGatePosition(new Vector2Int(mine.GatePosition.x + 1, (mine.GatePosition.y - 1)));
            mine.SetCellPlace(cell);
        }
    }

    public void SetSceneObjectsParent(Transform sceneObjectsParent, Transform terrainParent)
    {
        SceneObjectsParent = sceneObjectsParent;
        TerrainObjectsParent = terrainParent;
    }

    public void InitGameSession(IEnumerable<SessionParticipant> sessionParticipants)
    {
        foreach (var particicpant in sessionParticipants)
        {
            Player player = new Player(particicpant.UserInfo.UserName, particicpant.UserInfo.Email, particicpant.Ordinal,
                    particicpant.DicCastleId, particicpant.DicHeroId, particicpant.UserId);

            _players.Add(player);
        }
    }

    public bool TryGetResourceByResourceObjectID(string resourcesID, out ResourceSturcture resourceSturcture)
    {
        resourceSturcture = _resourcesSturctures.FirstOrDefault(item => item.MapObjectID == resourcesID);
        if (resourceSturcture != null)
            return true;
        return false;
    }

    public void EnterInBattleScene()
    {
        SceneObjectsParent.gameObject.SetActive(false);
        OnEnterInBattleScene?.Invoke();
    }

    public void EnterInGameFromBattleScene()
    {
        SceneObjectsParent.gameObject.SetActive(true);
        if (_creatureModelObject is CreatureModelObject creature)
            _mapCreatures.Remove(creature);
        if (_creatureModelObject != null && _creatureModelObject.gameObject != null)
            MonoBehaviour.Destroy(_creatureModelObject.gameObject);
        if(_cellHasFightedCreature != null)
            _cellHasFightedCreature.ResetInteractiveMapObject();
        if (HeroInFight != null && !IsLasFightWin)
        {
            HeroInFight.CellPlace.ResetInteractiveMapObject();
            _heroModelObjects.Remove(HeroInFight);
            _heroModelObjectsTurn.Remove(HeroInFight);
            if (HeroInFight.gameObject != null)
                MonoBehaviour.Destroy(HeroInFight.gameObject);
            OnUpdatedTurn?.Invoke();
        }
        ResetHeroInFight();
        ExitFromFight();
        OnEnterInGameFromBattleScene?.Invoke();
    }

    public void SetGameSessionID(string sessionID)
    {
        GameSessionID = sessionID;
    }

    public void HeroEndedMove(HeroModelObject heroModelObject)
    {
        IsHeroMove = false;
        //var position = heroModelObject.transform.position.ToVector2IntInHeroPosition();
        //Debug.Log(position);
        //TryGetCellByPosition(new Vector2Int(position.x, position.y * -1), out Cell cell);
        //cell.SetInteractiveMapObjectId(heroModelObject.MapObjectID);
        //heroModelObject.SetCellStayed(cell);
        heroModelObject.Idle();
        Debug.Log("End move");
    }

    public void HeroStartMove()
    {
        IsHeroMove = true;
    }

    public void EnterInTurn(TurnNotificationInfo turnNotificationInfo)
    {
        _heroModelObjectsTurn.Clear();
        _castlesTurn.Clear();
        if (TryGetPlayerByOrdinal(turnNotificationInfo.ordinal, out Player player))
        {
            player.heroesInfo = turnNotificationInfo.heroesInfo;
            foreach (var hero in turnNotificationInfo.heroesInfo)
            {
                if (TryGetHeroModelObject(hero.Key, out HeroModelObject heroModelObject))
                {
                    heroModelObject.SetMovePointsLeft(hero.Value.movePoints);
                    heroModelObject.Init(hero.Value);
                    _heroModelObjectsTurn.Add(heroModelObject);
                }
            }

            foreach (var castle in turnNotificationInfo.castlesInfo.Values)
                if (TryGetCastleByID(castle.mapObjectId, out Castle castleObject))
                    _castlesTurn.Add(castleObject);
            IsCurrentTurn = true;
            OnUpdatedTurn?.Invoke();
            OnEnteredInTurn?.Invoke(player);
        }
    }

    public void SetFightedCreatureSettings(GameMapObject creatureModelObject, Cell cell)
    {
        _creatureModelObject = creatureModelObject;
        _cellHasFightedCreature = cell;
    }

    public void ExitFromTurn()
    {
        IsCurrentTurn = false;
        OnExitFromTurn?.Invoke();
    }

    public void PlayerEnterInTurn(int ordinalPlayer)
    {
        if (TryGetPlayerByOrdinal(ordinalPlayer, out Player player))
        {
            OnPlayerEnterInTurn?.Invoke(player);
        }
    }

    public void PlayerStartTurn()
    {
        OnStatedTurn?.Invoke();
    }

    public void SetCells(Cell[,] cells, Cell[,] cellsInInversionSpace)
    {
        _cells = cells;
        _cellsIninversionSpace = cellsInInversionSpace;
        _groundModel.SetCells(_cellsIninversionSpace);
    }

    public void SetCastless(List<Castle> castles)
    {
        _castles = castles;
    }

    public void SetResourcesStructures(List<ResourceSturcture> resourcesSturctures)
    {
        _resourcesSturctures = resourcesSturctures;
    }
    public void SetMinesStructures(List<MineStructure> mineStructures)
    {
        _mineSturctures = mineStructures;
    }

    public void SetHeroModelObjects(List<HeroModelObject> heroModelObjects)
    {
        _heroModelObjects = heroModelObjects;
    }



    public void SetMapCreatures(List<CreatureModelObject> mapCreatures)
    {
        _mapCreatures = mapCreatures;
    }


    public void SetSelectedHero(HeroModelObject heroModelObject)
    {
        if (SelectedHero != null)
            SelectedHero.UnselectHero();
        SelectedHero = heroModelObject;   
        SelectedHero.SelectHero();
    }

    public bool TryGetPlayerByCastleID(int id, out Castle castle)
    {
        castle = _castles.FirstOrDefault(item => item.DicCastleID == id);
        if (castle != null)
            return true;
        return false;
    }

    public bool TryGetCastleByID(string id, out Castle castle)
    {
        castle = _castles.FirstOrDefault(item => item.MapObjectID == id);
        if (castle != null)
            return true;
        return false;
    }

    public bool TryGetPlayerByOrdinal(int ordinal, out Player player)
    {
        player = _players.FirstOrDefault(item => item.Ordinal == ordinal);
        if (player != null)
            return true;
        return false;
    }

    public bool TryGetCellByPosition(Vector2Int position, out Cell cell)
    {
        int rows = _cells.GetUpperBound(0) + 1;
        int columns = _cells.Length / rows;
        if (rows >= position.x && columns >= position.y)
        {
            cell = _cells[position.x, position.y];
            return true;
        }
        cell = null;
        return false;
    }

    public bool TryGetHeroModelObject(string id, out HeroModelObject heroModelObject)
    {

        heroModelObject = _heroModelObjects.FirstOrDefault(item => item.MapObjectID == id);
        if (heroModelObject != null)
            return true;
        return false;

    }
    public bool TryGetHeroModelObjectForCoursor(string id, out HeroModelObject heroModelObject)
    {

        heroModelObject = _heroModelObjectsTurn.FirstOrDefault(item => item.MapObjectID == id);
        if (heroModelObject != null)
            return true;
        return false;
    }
    public bool TryGetCastle(string id, out Castle castle)
    {

        castle = _castles.FirstOrDefault(item => item.MapObjectID == id);
        if (castle != null)
            return true;
        return false;

    }
}