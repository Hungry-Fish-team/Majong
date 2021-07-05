using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private GameplayController _gameplayController;

    [SerializeField] private Touch[] _touches;
    private int _countPause = 0;

    public void Update()
    {
        if (_countPause == 0)
        {
#if UNITY_ANDROID
            _touches = Input.touches;

            //Debug.Log(_touches[0].rawPosition);
            //#elif UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                //_gameplayController.MatchCell();
                //Debug.Log(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            }
#endif
        }
    }

    public void OnPause()
    {
        _countPause++;
    }

    public void DisPause()
    {
        _countPause--;
    }
}
