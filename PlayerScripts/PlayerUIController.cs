using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : NetworkBehaviour
{
    [SerializeField] private Player _player;

    [Header("HP & MP")]
    [SerializeField] private Image _hpBar;
    [SerializeField] private Image _mpBar;
    private Transform _canvas;
    private Vector3 _initScale;

    public override void OnNetworkSpawn()
    {
        _player = GetComponent<Player>();
        _canvas = _hpBar.transform.parent;
        _initScale = _canvas.localScale;
    }

    void Update()
    {
        if (_canvas == null) return;

        _canvas.localScale = _initScale;
    }

    // HP의 값이 변경될 때  UI 변경
    public void HpChanged(float preValue, float newValue)
    {
        if (_player != null)
            _hpBar.fillAmount = newValue / _player.FinalHp;
    }

    public void MaxHpChanged(float preValue, float newValue)
    {
        if (_player != null)
            _hpBar.fillAmount = _player.Hp / newValue;
    }

    // MP의 값이 변경될 때 UI 변경
    public void MpChanged(float preValue, float newValue)
    {
        if (_player != null)
            _mpBar.fillAmount = newValue / _player.FinalMp;
    }

    public void MaxMpChanged(float preValue, float newValue)
    {
        if (_player != null)
            _mpBar.fillAmount = _player.Mp / newValue;
    }
}
