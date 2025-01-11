using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]Transform target;
    Vector3 offset;
    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - target.position;
    }
    void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
