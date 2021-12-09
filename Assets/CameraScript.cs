using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public float msd = 10f;
    public float fls = 3f;
    public float zs = 10f;
    private bool lk = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
            Camera.main.transform.position = Camera.main.transform.position + (-Camera.main.transform.up * msd * Time.deltaTime);

        if (Input.GetKey(KeyCode.R))
            Camera.main.transform.position = Camera.main.transform.position + (Vector3.up * msd * Time.deltaTime);

        if (Input.GetKey(KeyCode.F))
            Camera.main.transform.position = Camera.main.transform.position + (-Vector3.up * msd * Time.deltaTime);

        if (Input.GetKey(KeyCode.S))
            Camera.main.transform.position = Camera.main.transform.position + (-Camera.main.transform.forward * msd * Time.deltaTime);

        if (Input.GetKey(KeyCode.E))
            Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.up * msd * Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
            Camera.main.transform.position = Camera.main.transform.position + (-Camera.main.transform.right * msd * Time.deltaTime);

        if (Input.GetKey(KeyCode.D))
            Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.right * msd * Time.deltaTime);

        if (Input.GetKey(KeyCode.W))
            Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * msd * Time.deltaTime);

        if (lk)
        {
            float nrx = Camera.main.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * fls;
            float nry = Camera.main.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * fls;
            Camera.main.transform.localEulerAngles = new Vector3(nry, nrx, 0f);
        }

        float a = Input.GetAxis("Mouse ScrollWheel");
        if (a != 0)
        {
            var zs = this.zs;
            Camera.main.transform.position = Camera.main.transform.position + Camera.main.transform.forward * a * zs;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartLooking();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopLooking();
        }
    }

    void OnDisable()
    {
        StopLooking();
    }

    public void StartLooking()
    {
        lk = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void StopLooking()
    {
        lk = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
