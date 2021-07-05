using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Newtonsoft;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

public class ProgressManager : MonoBehaviour
{
    [SerializeField] private SpawnZoneGenerator _spawnZoneGenerator;

    [SerializeField] private string _fileForDashBoardSave;
    [SerializeField] private string _fileForHistorySave;

    [SerializeField] private string _json;

    private void OnEnable()
    {
        LoadAllDataFromFile();
        LoadHistoryDataFromFile();
        _spawnZoneGenerator.isLoadSaveFile = true;
    }

    private void OnApplicationQuit()
    {
        SaveAllDataToFile();
        SaveHistoreDataToFile();
    }

    private void LoadFiles()
    {
#if UNITY_EDITOR_OSX
        if (!File.Exists(Application.dataPath.Remove(Application.dataPath.Length - 6) + "/fileForDashBoardSave.json"))
        {
            CreateFilesForSave("/fileForDashBoardSave.json");
        }
        _fileForDashBoardSave = Application.dataPath.Remove(Application.dataPath.Length - 6) + "/fileForDashBoardSave.json";

        if (!File.Exists(Application.dataPath.Remove(Application.dataPath.Length - 6) + "/fileForHistorySave.json"))
        {
            CreateFilesForSave("/fileForHistorySave.json");
        }
        _fileForHistorySave = Application.dataPath.Remove(Application.dataPath.Length - 6) + "/fileForHistorySave.json";
#elif UNITY_ANDROID
        if (!File.Exists(Application.persistentDataPath + "/fileForDashBoardSave.json"))
        {
            CreateFilesForSave("/fileForDashBoardSave.json");
        }
        _fileForDashBoardSave = Application.persistentDataPath + "/fileForDashBoardSave.json";

        if (!File.Exists(Application.persistentDataPath + "/fileForHistorySave.json"))
        {
            CreateFilesForSave("/fileForHistorySave.json");
        }
        _fileForHistorySave = Application.persistentDataPath + "/fileForHistorySave.json";
#endif
    }

    private void CreateFilesForSave(string nameOfFile)
    {
#if UNITY_EDITOR_OSX
        FileStream newFile = File.Open(Application.dataPath.Remove(Application.dataPath.Length - 6) + nameOfFile, FileMode.OpenOrCreate);
        newFile.Close();
        Debug.Log("create" + nameOfFile);
#elif UNITY_ANDROID
        FileStream newFile = File.Open(Application.persistentDataPath + nameOfFile, FileMode.OpenOrCreate);
        newFile.Close();
        Debug.Log("create" + nameOfFile);
#endif
    }

    public void SaveAllDataToFile()
    {
        JSONArray cellsArray = new JSONArray();

        if (_spawnZoneGenerator._cells == null) return;

        foreach (Cell cell in _spawnZoneGenerator._cells)
        {
            JSONObject cellData = new JSONObject();

            //public int id;
            //public Vector2 position;
            //public bool selected;
            //public bool blocked;
            //public Color color;
            //public int floor;

            cellData.Add("id", cell.id);
            JSONArray cellPos = new JSONArray();
            cellPos.Add("x", cell.position.x);
            cellPos.Add("y", cell.position.y);

            cellData.Add("cellPos", cellPos);

            cellData.Add("selected", cell.selected);
            cellData.Add("blocked", cell.blocked);
            JSONArray cellColor = new JSONArray();
            cellColor.Add(cell.color.r);
            cellColor.Add(cell.color.g);
            cellColor.Add(cell.color.b);
            cellColor.Add(cell.color.a);

            cellData.Add("cellColor", cellColor);

            cellData.Add("floor", cell.floor);

            cellsArray.Add(cellData);
        }

        Debug.Log("Save data");

        if (File.Exists(_fileForDashBoardSave))
        {
            File.WriteAllText(_fileForDashBoardSave, cellsArray.ToString());
        }
    }

    public void LoadAllDataFromFile()
    {
        LoadFiles();

        SpawnZoneGenerator.CellStruct cell;

        if ((JSONArray)JSON.Parse(File.ReadAllText(_fileForDashBoardSave)) != null)
        {
            JSONArray cellArray = (JSONArray)JSON.Parse(File.ReadAllText(_fileForDashBoardSave));

            if (cellArray != null)
            {
                //if (cellArray.Count == 0) return;
                for(int i = 0; i < cellArray.Count; i++)
                {
                    cell = new SpawnZoneGenerator.CellStruct();
                    JSONObject cellObject = cellArray.AsArray[i].AsObject;

                    cell.id = cellObject["id"];
                    cell.position = new Vector2(cellObject["cellPos"].AsArray[0], cellObject["cellPos"].AsArray[1]);
                    cell.selected = cellObject["selected"];
                    cell.blocked = cellObject["blocked"];
                    cell.color = new Color(cellObject["cellColor"].AsArray[0], cellObject["cellColor"].AsArray[1], cellObject["cellColor"].AsArray[2], cellObject["cellColor"].AsArray[3]);
                    cell.floor = cellObject["floor"];

                    _spawnZoneGenerator.cellStructs.Add(cell);
                }
            }
        }
    }

    public void SaveHistoreDataToFile()
    {
        JSONArray cellsArray = new JSONArray();

        foreach (SpawnZoneGenerator.CellStruct cell in _spawnZoneGenerator.historyOfCellClicked)
        {
            JSONObject cellData = new JSONObject();

            //public int id;
            //public Vector2 position;
            //public bool selected;
            //public bool blocked;
            //public Color color;
            //public int floor;

            cellData.Add("id", cell.id);
            JSONArray cellPos = new JSONArray();
            cellPos.Add("x", cell.position.x);
            cellPos.Add("y", cell.position.y);

            cellData.Add("cellPos", cellPos);

            cellData.Add("selected", false);
            cellData.Add("blocked", false);
            JSONArray cellColor = new JSONArray();
            cellColor.Add(cell.color.r);
            cellColor.Add(cell.color.g);
            cellColor.Add(cell.color.b);
            cellColor.Add(cell.color.a);

            cellData.Add("cellColor", cellColor);

            cellData.Add("floor", cell.floor);

            cellsArray.Add(cellData);
        }

        Debug.Log("Save data");

        if (File.Exists(_fileForHistorySave))
        {
            File.WriteAllText(_fileForHistorySave, cellsArray.ToString());
        }
    }

    public void LoadHistoryDataFromFile()
    {
        LoadFiles();

        SpawnZoneGenerator.CellStruct cell;

        if ((JSONArray)JSON.Parse(File.ReadAllText(_fileForHistorySave)) != null)
        {
            JSONArray cellArray = (JSONArray)JSON.Parse(File.ReadAllText(_fileForHistorySave));

            if (cellArray != null)
            {
                //if (cellArray.Count == 0) return;
                for (int i = 0; i < cellArray.Count; i++)
                {
                    cell = new SpawnZoneGenerator.CellStruct();
                    JSONObject cellObject = cellArray.AsArray[i].AsObject;

                    cell.id = cellObject["id"];
                    cell.position = new Vector2(cellObject["cellPos"].AsArray[0], cellObject["cellPos"].AsArray[1]);
                    cell.selected = false;
                    cell.blocked = false;
                    cell.color = new Color(cellObject["cellColor"].AsArray[0], cellObject["cellColor"].AsArray[1], cellObject["cellColor"].AsArray[2], cellObject["cellColor"].AsArray[3]);
                    cell.floor = cellObject["floor"];

                    _spawnZoneGenerator.historyOfCellClicked.Add(cell);
                }
            }
        }
    }
}
