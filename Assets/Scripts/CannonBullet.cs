using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBullet : Bullet {

    private int ammoID = 0;

    void OnTriggerEnter(Collider _col)
    {
        TriggerEnter(_col, ammoID);
    }
}


