using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    internal Transform parent;
    internal Vector3 startPos;

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
    }

	//void OnCollisionEnter(Collision __collision) {}
	protected void TriggerEnter(Collider _col, int _ammoID)
    {
        if (_col.transform != parent && !_col.CompareTag("water"))
        {
            _col.gameObject.GetComponent<Ship>().data.SetDamage(_ammoID);
            if (_col.gameObject.GetComponent<Ship>().data.health_body < 0)
            {
                if (_col.gameObject.GetComponent<Ship>().data.playerControlled)
                    { UI_script.Instance.ShowVictoryScreen(); }
                else
                    { UI_script.Instance.ShowLoseScreen(); }
            }
        }
        Destroy(this.gameObject);
        // запускаем анимашку взрыва

    }

	void FixedUpdate ()
    {
        if (Vector3.SqrMagnitude(startPos - transform.position) > 400f)
        {
            Destroy(this.gameObject);
            // запускаем анимашку взрыва
            //
        }
    }

    void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
