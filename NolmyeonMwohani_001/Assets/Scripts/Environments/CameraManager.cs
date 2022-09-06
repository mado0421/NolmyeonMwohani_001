using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CameraManager : MonoBehaviour
{
    private void Start()
    {
        _followOffset = new Vector3(0, 13.3f, -8.3f);
    }

    void Update()
    {
        Follow();
    }

    private void Follow()
    {
        transform.position = _followTarget.position + _followOffset;
    }

    [SerializeField] private Transform _followTarget;
    [SerializeField] private Vector3 _followOffset;
}
