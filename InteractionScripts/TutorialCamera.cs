using UnityEngine;

public class TutorialCamera : MonoBehaviour
{
    private CameraFollow _cameraFollow;
    private Animator _anim;

    void Awake()
    {
        _cameraFollow = GetComponent<CameraFollow>();
        _anim = GetComponent<Animator>();
    }

    public void NormalCam()
    {
        _anim.enabled = false;
        _cameraFollow.target = GameManager.Instance.player.transform;
        _cameraFollow.enabled = true;
    }
}
