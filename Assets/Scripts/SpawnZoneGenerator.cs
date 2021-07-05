using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using System.IO;

public class SpawnZoneGenerator : MonoBehaviour
{
    [SerializeField] private GameplayController _gameplayController;

    [SerializeField] private RectTransform _spawnZone;

    [SerializeField] private int _countFloors;
    [SerializeField] private List<RectTransform> _floors;
    [SerializeField] private GameObject _floorItem;
    [SerializeField] private int _countCells;

    [SerializeField] private GameObject _cellItem;
    [SerializeField] private float _cellSize;
    [SerializeField] private List<Vector2> _cellsPositionOnFloor;
    public List<Cell> _cells;

    private Vector2[] _spawnPosition = new Vector2[9];

    public bool isGameReady = false;

    [SerializeField] private bool _loadLevelFromFile;
    [SerializeField] private bool _loadFromSaveFile;
    public bool isLoadSaveFile;

    private BinaryReader _reader;
    private string path;

    public struct CellStruct
    {
        public int id;
        public Vector2 position;
        public bool selected;
        public bool blocked;
        public Color color;
        public int floor;
    }

    public List<CellStruct> cellStructs = new List<CellStruct>();

    public List<CellStruct> historyOfCellClicked = new List<CellStruct>();

    [SerializeField] private Dictionary<int, Color> _idColors = new Dictionary<int, Color>();

    private IEnumerator Start()
    {
        InitSpawnPosition();

#if UNITY_EDITOR_OSX
        path = Path.Combine(Application.dataPath, "Levels");
#elif UNITY_ANDROID
        path = Path.Combine("jar:file://" + Application.dataPath + "!/assets/" + "Levels");
#endif
        InitReader();

        yield return new WaitUntil(() => isLoadSaveFile == true);

        if(cellStructs == null || cellStructs.Count == 0)
        {
            _loadFromSaveFile = false;
        }

        Debug.Log("Create floors and cells");

        GenerarionSpawnZone();
    }

    private void InitReader()
    {
        int lvlIndex = (int)Random.Range(0f, 20f);
        TextAsset textAsset = Resources.Load("Levels/" + lvlIndex.ToString(), typeof(TextAsset)) as TextAsset;
        Debug.Log(textAsset);
        Stream s = new MemoryStream(textAsset.bytes);
        _reader = new BinaryReader(s);
    }

    public void RestartGame()
    {
        foreach(RectTransform floor in _floors)
        {
            Destroy(floor.gameObject);
        }
        _floors.Clear();
        _cellsPositionOnFloor.Clear();
        if(_cells != null) _cells.Clear();
        cellStructs.Clear();
        historyOfCellClicked.Clear();
        _countFloors = 0;
        _loadFromSaveFile = false;

        _gameplayController.SaveProgress();

        InitReader();

        GenerarionSpawnZone();

        _gameplayController.SaveProgress();
    }

    private void InitSpawnPosition()
    {
        _spawnPosition[0] = new Vector2(-1, +1);
        _spawnPosition[1] = new Vector2(0, +1);
        _spawnPosition[2] = new Vector2(+1, +1);
        _spawnPosition[3] = new Vector2(+1, 0);
        _spawnPosition[4] = new Vector2(+1, -1);
        _spawnPosition[5] = new Vector2(0, -1);
        _spawnPosition[6] = new Vector2(-1, -1);
        _spawnPosition[7] = new Vector2(-1, 0);
    }

    public void GenerarionSpawnZone()
    {
        isGameReady = false;

        if (!_loadFromSaveFile)
        {
            if (!_loadLevelFromFile)
            {
                int countCells = _countCells;
                _cells.Clear();

                while (countCells / 2 != 0)
                {
                    countCells /= 2;
                    _countFloors++;
                    RectTransform floor = CreateFloor();

                    for (int i = 0; i < countCells; i++)
                    {
                        _cells.Add(CreateCellPrefab(floor, i));
                    }
                }
            }
            else
            {
                _cells = ReturnData();
            }
        }
        else
        {
            InitFromFileByOne(cellStructs);
        }

        RemoveBadItems();

        IsCellBlocked();

        if (_gameplayController.LoseGame())
        {
            RestartGame();
        }

        isGameReady = true;
    }

    public List<Cell> ReturnData()
    {
        
        if (_reader != null)
        {
            var list = new List<Cell>();
            int floors = _reader.ReadInt32();
            for (int i = floors - 1; i >= 0; i--)
            {
                RectTransform floor = CreateFloor();
                int count = _reader.ReadInt32();

                //int countId = (int)count / 3;
                //if (countId > 15) countId = 12;
                //Debug.Log(countId);

                int countId = 12;

                for (int j = 0; j < count; j++)
                {
                    float x = _reader.ReadSingle();
                    float y = _reader.ReadSingle();

                    Cell newCell = Instantiate(_cellItem, floor).GetComponent<Cell>();
                    int cellId = Random.Range(0, countId);

                    Color cellColor = FindRandomColor(cellId);

                    newCell.InitializeCell(cellId, new Vector2(x * _cellSize, y * _cellSize), false, false, cellColor, floors - i - 1, _gameplayController);

                    list.Add(newCell);
                }
            }
            _reader.Close();

            return list;
        }

        return null;
    }

    private Color FindRandomColor(int id)
    {
        if (_idColors != null && _idColors.ContainsKey(id))
        {
            return _idColors[id];
        }
        else
        {
            _idColors.Add(id, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            return _idColors[id];
        }
    }

    public void InitFromFileByOne(List<CellStruct> cellStruct)
    {
        int floorCount = InitAllFloors(cellStruct);

        for (int i = 0; i < floorCount; i++)
        {
            RectTransform floor = CreateFloor();
        }

        int count = cellStruct.Count();
        for (int j = 0; j < count; j++)
        {
            Cell newCell = Instantiate(_cellItem, _floors[cellStruct[j].floor]).GetComponent<Cell>();
            newCell.InitializeCell(cellStruct[j].id, cellStruct[j].position, cellStruct[j].selected, cellStruct[j].blocked, cellStruct[j].color, cellStruct[j].floor, _gameplayController);

            _cells.Add(newCell);
        }
    }

    private int InitAllFloors(List<CellStruct> cellStruct)
    {
        Debug.Log(cellStruct.Select(x => x.floor).Max() + 1);
        return cellStruct.Select(x => x.floor).Max() + 1;
    }

    private Cell CreateCellPrefab(RectTransform floor, int numberOfCell)
    {
        Cell newCell = Instantiate(_cellItem, floor).GetComponent<Cell>();
        //int countId = (int)_countCells / 3;
        //if (countId > 15) countId = 15;

        int countId = 12;

        int cellId = Random.Range(0, countId);
        Color cellColor = FindRandomColor(cellId);

        newCell.InitializeCell(cellId, SetPosition(numberOfCell), false, false, cellColor, _countFloors - 1, _gameplayController);
        return newCell;
    }

    private RectTransform CreateFloor()
    {
        RectTransform newFloor = Instantiate(_floorItem, _spawnZone).GetComponent<RectTransform>();
        _floors.Add(newFloor);
        return newFloor;
    }

    private Vector2 SetPosition(int numberOfCell)
    {
        int roundCount = numberOfCell / 9 + 1;
        int roundNumber = numberOfCell % 8;

        Vector2 newPosition;
        //Debug.Log(numberOfCell.ToString() + " " + roundCount.ToString() + " " +roundNumber.ToString());
        if (numberOfCell != 0)
        {
            newPosition = new Vector2(0f, 0f) + _cellSize * (roundCount) * _spawnPosition[roundNumber] - new Vector2(1f, 1f) * _cellSize / 2 * (_floors.Count() - 1);
        }
        else
        {
            newPosition = new Vector2(0f, 0f);
        }

        return newPosition;
    }

    private void RemoveBadItems()
    {
        var countId = _cells.Select(x => x.id).Max() + 1;

        for(int i = 0; i < countId; i++)
        {
            var countOneId = _cells.Where(x => x.id == i).Count();

            if(countOneId % 2 == 1)
            {
                var deleteCell = _cells.Where(x => x.id == i).First();
                _cells.Remove(deleteCell);
                Destroy(deleteCell.gameObject);
            }
        }
    }

    public void IsCellBlocked()
    {
        Debug.Log("reblocked");
        for (int i = 0; i < _floors.Count(); i++)
        {
            var onFloorCells = _cells.Where(x => x.floor == i).ToArray();

            foreach(Cell checkCell in onFloorCells)
            {
                Vector2 leftPos = new Vector2(checkCell.position.x - _cellSize, checkCell.position.y);
                Vector2 rightPos = new Vector2(checkCell.position.x + _cellSize, checkCell.position.y);

                var leftCell = onFloorCells.Where(x => x.position == leftPos);
                var rigthCell = onFloorCells.Where(x => x.position == rightPos);

                //Debug.Log(i.ToString() + " " + checkCell.name + " " + leftCell.Count()+ " " + rigthCell.Count());

                if((leftCell != null && leftCell.Count() > 0) && (rigthCell != null && rigthCell.Count() > 0))
                {
                    checkCell.blocked = true;
                }
                else
                {
                    checkCell.blocked = false;
                }
            }
        }

        for (int i = _floors.Count(); i > 0; i--)
        {
            var noOnFloorCells = _cells.Where(x => x.floor < i).ToArray();
            var onFloorCells = _cells.Where(x => x.floor == i).ToArray();

            foreach (Cell cell in onFloorCells)
            {
                Vector2 cellPos = cell.position;

                var cellNear = noOnFloorCells.Where(x => Vector2.Distance(cellPos, x.position) <= _cellSize * 0.95);

                //Debug.Log(i.ToString() + " " + checkCell.name);

                foreach(Cell underCell in cellNear)
                {
                    underCell.blocked = true;
                }
            }
        }

        foreach(Cell cell in _cells)
        {
            cell.ChangeInitShadow();
        }
    }

    public IEnumerator ReturnAllCellsFromHistory()
    {
        if (historyOfCellClicked.Count != 0)
        {
            isGameReady = false;

            foreach (CellStruct cellStruct in historyOfCellClicked)
            {
                //Debug.Log(cellStruct.floor);
                Cell newCell = Instantiate(_cellItem, _floors[cellStruct.floor]).GetComponent<Cell>();
                newCell.InitializeCell(cellStruct.id, cellStruct.position, cellStruct.selected, cellStruct.blocked, cellStruct.color, cellStruct.floor, _gameplayController);
                newCell.ChangeInitTitle();

                _cells.Add(newCell);

                yield return new WaitForSeconds(0.15f);
            }

            IsCellBlocked();

            historyOfCellClicked.Clear();

            _gameplayController.SaveProgress();

            isGameReady = true;
        }
    }

}
