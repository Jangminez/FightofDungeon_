using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CanvasSortingLayer : MonoBehaviour
{
    public Canvas _canvas;
    private SortingGroup _sg;
    public string _currentLayer;
    public float _sortY;

    private void Awake()
    {
        _sg = GetComponent<SortingGroup>();
        _currentLayer = _sg.sortingLayerName;
    }


    // Update is called once per frame
    void Update()
    {
        // SortingLayer가 변경되면 현재 오브젝트와 같은 레이어로 변경
        if (_sg.sortingLayerName != _currentLayer)
        {
            _currentLayer = _sg.sortingLayerName;
            _canvas.sortingLayerName = _currentLayer;
        }

        _canvas.transform.position = transform.position + new Vector3(0f, _sortY, 0f);
    }
}
