using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 100.0f;
    private float cameraYrotation, inputX, inputY;
    // private bool lockCursor = true;
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        inputX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        inputY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        cameraYrotation -= inputY;
        cameraYrotation = Mathf.Clamp(cameraYrotation, -90.0f, 90.0f);
        
        transform.localRotation = Quaternion.Euler(cameraYrotation, 0.0f, 0.0f);
        player.Rotate(Vector3.up * inputX);
        
        // transform.position=player.position;
    }
}
