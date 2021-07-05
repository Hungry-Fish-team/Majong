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
    [SerializeField] private AnalyticsManager _analyticsManager;

    private void Start()
    {
        _analyticsManager.SendAppOpen();

        _restartButton.onClick.AddListener(() =>
        {
            _analyticsManager.SendRestartGame();
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
