using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    void Update()
    {
        transform.position = _player.transform.position;
    }

    [SerializeField] private GameObject _player;
}
