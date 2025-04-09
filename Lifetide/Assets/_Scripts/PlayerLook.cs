using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float lookSpeed = 10f;
    public float offset;

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; 

        Vector3 direction = mouseWorldPos - transform.position;
      
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + offset;

        float rotation = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * lookSpeed);

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));
    }
}