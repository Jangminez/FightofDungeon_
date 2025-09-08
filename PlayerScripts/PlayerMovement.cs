using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public Joystick _joystickMovement;
    public float _speed;
    Rigidbody2D _playerRb;
    Animator _anim;
    Transform _canvas;
    Player player;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            this.enabled = false;
            return;
        }

        player = GetComponent<Player>();
        _playerRb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _speed = player.Speed;
        _canvas = transform.GetChild(1);

        
        if(_joystickMovement == null)
            _joystickMovement = GameObject.FindWithTag("JoyStick").GetComponent<Joystick>();
    }
    
    void FixedUpdate()
    {
        if (!IsOwner) return;

        Movement();
    }

    void LateUpdate()
    {
        if (!IsOwner) return;

        Movement_Anim();
    }

    public void Movement()
    {

        if (player.Die)
        {
            _playerRb.velocity = Vector2.zero;
            return;
        }

        if (_joystickMovement.Direction.y != 0)
        {
            Vector2 nextVec = new Vector2(_joystickMovement.Direction.x * _speed, _joystickMovement.Direction.y * _speed);
            _playerRb.velocity = nextVec;

            SetDirection();

            if (nextVec == Vector2.zero)
            {
                _anim.SetFloat("RunState", 0f);
            }
        }

        else
        {
            _playerRb.velocity = Vector2.zero;
        }
    }

    // 플레이어 이동 애니메이션
    public void Movement_Anim()
    {
        if (_joystickMovement.Direction.x != 0 || _joystickMovement.Direction.y != 0)
        {
            _anim.SetFloat("RunState", 0.5f);
        }

        else
        {
            _anim.SetFloat("RunState", 0f);
        }
    }

    void SetDirection()
    {
        if (_joystickMovement.Direction.x > 0)
        {
            _anim.transform.localScale = new Vector3(-1.7f, 1.7f, 1.7f);
            _canvas.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
        }

        else if (_joystickMovement.Direction.x < 0)
        {
            _anim.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
            _canvas.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

}
