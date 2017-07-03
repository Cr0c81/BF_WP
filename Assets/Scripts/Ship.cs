using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour{

    public ShipData data;

    void Start()
    {
        data.tr = transform;
    }
}
