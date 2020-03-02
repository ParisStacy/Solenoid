using UnityEngine;

public class CameraAnimate : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    private Vector3 _anchor;

    void Start()
    {
        
    }


    void Update()
    {
        _anchor = _player.transform.position;
        _anchor.y += .5f;
        transform.position = Vector3.Slerp(transform.position, _anchor, .1f);
    }
}
