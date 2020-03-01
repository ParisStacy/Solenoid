using UnityEngine;

/*
 * THIS SCRIPT IS ORIGINALLY FROM Jian-Kai Wang:
 * https://github.com/jiankaiwang/FirstPersonController
 */


public class CameraLook : MonoBehaviour
{
    [SerializeField]
    float sensitivity = 5.0f;
    [SerializeField]
    float smoothing = 2.0f;

    private GameObject controller;

    private Vector2 mouseLook;
    private Vector2 smoothV;

    void Start() {
        controller = this.transform.parent.gameObject;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {

        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        mouseLook += smoothV * Time.deltaTime;

        mouseLook.y = Mathf.Clamp(mouseLook.y, -85, 90);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        controller.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, controller.transform.up);

        if (Input.GetKeyDown("escape")) {
            Cursor.lockState = CursorLockMode.None;
        }

    }

}
