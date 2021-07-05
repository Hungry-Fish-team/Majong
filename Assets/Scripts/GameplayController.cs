using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private SpawnZoneGenerator _spawnZoneGenerator;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private ProgressManager _progressManager;

    private IEnumerator Start()
    {
        _spawnZoneGenerator = GetComponent<SpawnZoneGenerator>();
        yield return new WaitUntil(() => _spawnZoneGenerator.isGameReady);

        Debug.Log("GameStart");
    }

    public void MatchCell()
    {
        var selectedCell = _spawnZoneGenerator._cells.Where(x => x.selected == true).ToArray();

        //Debug.Log(selectedCell.Count());

        if (selectedCell.Count() > 1)
        {
            for (int i = 0; i < selectedCell.Count() - 1; i++)
            {
                for (int j = i + 1; j < selectedCell.Count(); j++)
                {
                    if (selectedCell.ToArray()[i].id == selectedCell.ToArray()[j].id)
                    {
                        Sequence sequence = DOTween.Sequence();

                        sequence.AppendCallback(() =>
                        {
                            selectedCell.ToArray()[i].HideCell();
                            selectedCell.ToArray()[j].HideCell();

                            _spawnZoneGenerator.historyOfCellClicked.Insert(0, ReturnCellFromStruct(selectedCell.ToArray()[i]));
                            _spawnZoneGenerator.historyOfCellClicked.Insert(0, ReturnCellFromStruct(selectedCell.ToArray()[j]));

                            SaveProgress();
                        });

                        sequence.AppendInterval(1.05f);

                        sequence.AppendCallback(() =>
                        {
                            _spawnZoneGenerator._cells.Remove(selectedCell.ToArray()[i]);
                            _spawnZoneGenerator._cells.Remove(selectedCell.ToArray()[j]);

                            if(selectedCell.ToArray()[i] != null) Destroy(selectedCell.ToArray()[i].gameObject);
                            if (selectedCell.ToArray()[j] != null) Destroy(selectedCell.ToArray()[j].gameObject);

                            _spawnZoneGenerator.IsCellBlocked();

                            SaveProgress();

                            LoseGame();

                        });

                        return;
                    }
                    else
                    {
                        selectedCell.ToArray()[i].SelectedOrNot();
                        selectedCell.ToArray()[j].SelectedOrNot();
                    }
                }
            }
        }       
    }

    public bool LoseGame()
    {
        var openedCell = _spawnZoneGenerator._cells.Where(x => x.blocked == false).ToArray();
        int countMathces = 0;

        for(int i = 0; i < openedCell.Count() - 1; i++)
        {
            for (int j = i + 1; j < openedCell.Count(); j++)
            {
                if(openedCell[i].id == openedCell[j].id)
                {
                    countMathces++;
                    _gameManager.ReturnInfoMathces("not lose");
                    return false;
                }
            }
        }

        if(countMathces == 0)
        {
            _gameManager.ReturnInfoMathces("lose");
            StartCoroutine(_spawnZoneGenerator.ReturnAllCellsFromHistory());
            return true;
        }
        else
        {
            _gameManager.ReturnInfoMathces("not lose");
            return false;
        }
    }

    public void SaveProgress()
    {
        _progressManager.SaveAllDataToFile();
        _progressManager.SaveHistoreDataToFile();
    }

    private SpawnZoneGenerator.CellStruct ReturnCellFromStruct(Cell cell)
    {
        SpawnZoneGenerator.CellStruct cellStruct = new SpawnZoneGenerator.CellStruct();

        cellStruct.id = cell.id;
        cellStruct.color = cell.color;
        cellStruct.selected = cell.selected;
        cellStruct.blocked = cell.blocked;
        cellStruct.position = cell.position;
        cellStruct.floor = cell.floor;

        return cellStruct;
    }
}
