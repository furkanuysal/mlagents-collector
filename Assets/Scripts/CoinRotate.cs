using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotate : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up, 70f * Time.deltaTime, Space.World);
    }
}
