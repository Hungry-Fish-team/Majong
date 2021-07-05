using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] private GameplayController _gameplayController;

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _logo;
    [SerializeField] private Image _shadow;
    [SerializeField] private TextMeshProUGUI _titleID;
    [SerializeField] private Button _button;

    public int id;
    public Vector2 position;
    public bool selected;
    public bool blocked;
    public Color color;
    public int floor;

    [SerializeField] private Color _blockColor;
    [SerializeField] private Color _selectColor;

    public void InitializeCell(int id, Vector2 position, bool selected, bool blocked, Color color, int floor, GameplayController gameplayController)
    {
        this.id = id;
        this.position = position;
        this.selected = selected;
        this.blocked = blocked;
        //this.color = color;
        this.color = new Color(1f, 1f, 1f);
        this.floor = floor;
        _gameplayController = gameplayController;

        _logo.sprite = Resources.Load("food/" + (id + 1).ToString() + "@8x", typeof(Sprite)) as Sprite;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            Debug.Log(_rectTransform == null);
            if (_rectTransform != null)
            {
                _rectTransform.DOAnchorPos(position, 1f);
            }
        });

        ChangeInitColor();
        ChangeInitShadow();
        //ChangeInitTitle();

        _button.onClick.AddListener(() =>
        {
            SelectedOrNot();
            _gameplayController.MatchCell();
            if (!blocked)
            {
            }
        });
    }

    public void SelectedOrNot()
    {
        if (!blocked)
        {
            if (!selected)
            {
                CellIsSelected();
            }
            else
            {
                CellIsUnSelected();
            }

            ChangeInitColor();
        }
    }


    public void ChangeInitColor()
    {
        _rectTransform.GetComponent<Image>().color = color;
    }

    public void ChangeInitShadow()
    {
        if (blocked)
        {
            _shadow.enabled = true;
        }
        else
        {
            _shadow.enabled = false;
        }
    }

    public void ChangeInitTitle()
    {
        _titleID.text = id.ToString();
    }

    public void CellIsBlocked()
    {
        Color buff = color;
        color = _blockColor;
        _blockColor = buff;
        blocked = true;
    }

    public void CellIsSelected()
    {
        Color buff = _selectColor;
        _selectColor = color;
        color = buff;
        selected = true;
    }

    public void CellIsUnBlocked()
    {

        Color buff = _blockColor;
        _blockColor = color;
        color = buff;
        blocked = false;
    }

    public void CellIsUnSelected()
    {
        Color buff = color;
        color = _selectColor;
        _selectColor = buff;
        selected = false;
    }

    public Tween ShowCell()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            _rectTransform.DOAnchorPos(position, 1f);
        });

        return sequence;
    }


    public Tween HideCell()
    {
        SelectedOrNot();
        ChangeInitColor();

        _button.interactable = false;

        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
        {
            if (_rectTransform.GetComponent<CanvasGroup>() != null)
            {
                _rectTransform.GetComponent<CanvasGroup>().DOFade(0f, 1f);
            }
        });

        return sequence;
    }

    private void OnDestroy()
    {
        DOTween.Kill(_rectTransform);
        DOTween.Kill(this);
        DOTween.Kill(_rectTransform.GetComponent<CanvasGroup>());
    }
}
