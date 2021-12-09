using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    public Camera c;

    public NavMeshAgent agt;
    public bool clicked;
    public Vector3 dest;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clicked = true;
            Ray ray = c.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agt.SetDestination(hit.point);
            }
        }
    }
}