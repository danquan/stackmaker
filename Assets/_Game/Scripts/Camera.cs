using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Player target;
    [SerializeField] public Vector3 offset;
    [SerializeField] private Vector3 defaultRotation;
    public float speed = 20;
    // Start is called before the first frame update
    void Start()
    {
        //target = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(target.transform.position.x + offset.x,
                                          transform.position.y,
                                          target.transform.position.z + offset.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speed);
        //transform.rotation = target.transform.rotation;
        /*transform.rotation = Quaternion.Euler(new Vector3(defaultRotation.x,
                                                          defaultRotation.y + target.GetDirection() * 90,
                                                          defaultRotation.z));
        */
    }
}
