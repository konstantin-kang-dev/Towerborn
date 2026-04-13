using UnityEngine;

public class BillboardUI : MonoBehaviour
{

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
