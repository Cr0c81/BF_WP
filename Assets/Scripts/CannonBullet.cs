using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBullet : Bullet {

    override protected void Start()
    {
        ammoID = 0;
        base.Start();
    }

    void OnTriggerEnter(Collider _col)
    {
        if (_col.transform != parent)
        {
            TriggerEnter(_col, ai);
            SelfDestroy();
        }
    }

    protected override void SelfDestroy()
    {

        Destroy(this.gameObject);
    }
}



