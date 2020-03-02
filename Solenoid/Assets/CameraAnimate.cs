using UnityEngine;

public class CameraAnimate : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;
    private PlayerControl _controller;

    void Start()
    {
        _controller = _player.GetComponent<PlayerControl>();
    }


    void Update()
    {
        transform.position = Vector3.Slerp(transform.position, _controller.cameraTarget, .1f);
    }
}
