using System.Collections;
using UnityEngine;

public class TerrainLoader
{
    private TerrainCells _terrainCells;
    private GameModel _gameModel;
    private FalloffMap _falloffMap;


    public TerrainLoader(TerrainCells terrainCells, GameModel gameModel)
    {
        _terrainCells = terrainCells;
        _gameModel = gameModel;
        
    }

    public Cell[,] CreateTerrain(TerrainCell[][] terrainCells, Vector2Int size, out Cell[,] rightPositionsCells)
    {
        Cell[,] cells = new Cell[size.x, size.y];
        Cell[,] cellsInInvertSpace = new Cell[size.x, size.y];
        int currentX = 0;
        int currentY = 0;
        _gameModel.SetMapSize(size);
        Debug.Log("terrainCells " + terrainCells.Length + " " + terrainCells[0].Length);
        foreach (TerrainCell[] cellArr in terrainCells)
        {
            foreach (TerrainCell cell in cellArr)
            {

                Cell createdCell = MonoBehaviour.Instantiate(_terrainCells.GetCellByType(terrainCells[currentX][currentY * -1].type), new Vector3(currentX, 0, currentY), Quaternion.identity, _gameModel.TerrainObjectsParent);
                createdCell.SetGameModel(_gameModel);
                cells[currentX, currentY * -1] = createdCell;
                cellsInInvertSpace[(size.y - 1) + currentY , currentX] = createdCell;
                currentX++;
            }
            currentY--;
            currentX = 0;
        }

        rightPositionsCells = cellsInInvertSpace;
        return cells;
    }


    //public Cell[,] CreateTerrain(TerrainCell[][] terrainCells, Vector2Int size)
    //{
    //    Cell[,] cells = new Cell[size.x,size.y];
    //    //for(int x = 0; x < terrainCells.Length; x++)
    //    //{
    //    //    for(int y = 0; y < terrainCells[x].Length; y++)
    //    //    {
    //    //        Cell cell = MonoBehaviour.Instantiate(_terrainCells.GetCellByType(terrainCells[x][y].type), new Vector3(x, 0, y), Quaternion.identity);
    //    //        cell.SetGameModel(_gameModel);
    //    //        cells[x, y] = cell;
    //    //    }
    //    //}
    //    int currentX = 0;
    //    int currentY = 0;
    //    foreach (TerrainCell[] cellArr in terrainCells)
    //    {
    //        foreach (TerrainCell cell in cellArr)
    //        {
    //            Cell createdCell = MonoBehaviour.Instantiate(_terrainCells.GetCellByType(terrainCells[currentY * -1][currentX].type), new Vector3(currentX , 0, currentY), Quaternion.identity);
    //            createdCell.SetGameModel(_gameModel);
    //            cells[currentX , currentY * -1] = createdCell;
    //            currentX++;
    //        }
    //        currentY--;
    //        currentX = 0;
    //    }

    //    return cells;
    //} 
}