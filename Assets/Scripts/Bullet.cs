using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    internal Transform parent;
    internal Vector3 startPos;
    protected int ammoID = -1;
    protected AmmoItem ai;

    /// <summary>
    /// Задание параметров снаряда
    /// </summary>
    /// <param name="_direction">Направление по XZ</param>
    /// <param name="_dist">Дальность</param>
    /// <param name="_speed">Скорость полета (передем из параметров снаряда)</param>
    /// <param name="_height">Высота траектории</param>
    /// <param name="_parent">Хозяин снаряда</param>
    public virtual void SetParams(Vector3 _direction, float _dist, float _speed, float _height, Transform _parent)
    {
        parent = _parent;
    }

    virtual protected void Start()
    {
        ai = WeaponData.Instance.ammos[ammoID];
    }

    protected void TriggerEnter(Collider _col, int _ammoID)
    {
        TriggerEnter(_col, WeaponData.Instance.ammos[_ammoID]);
    }

    protected void TriggerEnter(Collider _col, AmmoItem _ai)
    {
        if (_col.transform != parent && !_col.CompareTag("water"))
        {
            _col.gameObject.GetComponent<Ship>().data.SetDamage(_ai);
            if (_col.gameObject.GetComponent<Ship>().data.health_body < 0)
            {
                if (_col.gameObject.GetComponent<Ship>().data.playerControlled)
                { UI_script.Instance.ShowVictoryScreen(); }
                else
                { UI_script.Instance.ShowLoseScreen(); }
            }
        }
    }

    void FixedUpdate ()
    {
        if (Vector3.SqrMagnitude(startPos - transform.position) > 400f)
        {
            SelfDestroy();
        }
    }

    // Для кастомизации самоуничтожения
    virtual protected void SelfDestroy()
    {
        Destroy(this.gameObject);
    }
}
