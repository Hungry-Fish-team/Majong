using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _returnButton;
    [SerializeField] private SpawnZoneGenerator _spawnZoneGenerator;
    [SerializeField] private TextMeshProUGUI _countMathcesText;

    private void Start()
    {
        _restartButton.onClick.AddListener(() =>
        {
            _spawnZoneGenerator.RestartGame();
            ReturnInfoMathces("");
        });

        _returnButton.onClick.AddListener(() =>
        {
            StartCoroutine(_spawnZoneGenerator.ReturnAllCellsFromHistory());
            StartCoroutine(WaitLoad());
        });
    }

    public void ReturnInfoMathces(string text)
    {
        _countMathcesText.text = text;
    }

    public IEnumerator WaitLoad()
    {
        _returnButton.interactable = false;
        yield return new WaitUntil(() => _spawnZoneGenerator.isGameReady);
        _returnButton.interactable = true;
    }
}
