using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    float moveSpeed = 5f;
    float sprintSpeed = 8f;
    float currentSpeed;
    float sens = 250f;
    Rigidbody rb;
    Camera cam;

    bool sprinting;

    float xRotation = 0f;

    Vector3 move;

    [SerializeField] private DynamicTextData critData;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = transform.GetChild(0).GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift)) sprinting = !sprinting;

        if (sprinting) currentSpeed = sprintSpeed; else currentSpeed = moveSpeed;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        move = transform.right * x + transform.forward * z;
        move = move.normalized;

        float mousex = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mousey = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        xRotation -= mousey;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mousex);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            RaycastHit hit;
            
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f))
            {

                if (hit.transform.CompareTag("Enemy"))
                {

                    DynamicTextData data = hit.transform.GetComponent<Enemy>().textData;

                    Vector3 destination = hit.point + (transform.position - hit.point) / Vector3.Distance(hit.point, transform.position);
                    destination.x += (Random.value - 0.5f) / 3f;
                    destination.y += Random.value;
                    destination.z += (Random.value - 0.5f) / 3f;

                    int damage = Random.Range(80, 100);


                    if(Random.value > 0.9f)
                    {
                        destination.y += 1f;

                        DynamicTextManager.CreateText(destination, "CRIT!", critData);

                        damage *= 2;

                        destination.y -= 1f;
                    }

                    DynamicTextManager.CreateText(destination, damage.ToString(), data);

                }

            }

        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * currentSpeed * Time.fixedDeltaTime);
    }
}
