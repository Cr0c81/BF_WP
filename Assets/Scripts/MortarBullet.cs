using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarBullet : Bullet {

    private int ammoID = 1;
    private float timer = 0f;
    private float dist;
    private float speed;
    private float height = 4f;
    private Rigidbody rb; // снаряд
    private Transform tr_shadow; // тень снаряда
    private Vector3 direction;
    private float time;
    
    [SerializeField]
    private AnimationCurve trajectory;

    // Use this for initialization
    void Start ()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        tr_shadow = transform.GetChild(0);
    }

    /// <summary>
    /// Задание параметров снаряда
    /// </summary>
    /// <param name="_direction">Направление по XZ</param>
    /// <param name="_dist">Дальность</param>
    /// <param name="_speed">Скорость полета (передем из параметров снаряда)</param>
    /// <param name="_height">Высота траектории</param>
    /// <param name="_parent">Хозяин снаряда</param>
    override public void SetParams(Vector3 _direction, float _dist, float _speed, float _height, Transform _parent)
    {
        dist = _dist;
        speed = _speed;
        time = dist / speed; // рассчитываем время полета
        direction = _direction * dist; // расстояние приземления снаряда
        height = _height;
        parent = _parent;
    }

    // Update is called once per frame
    void Update () {
        float f1 = timer / time; // Коэфициент [0;1] положенияна траектории
        float f2 = trajectory.Evaluate(f1) * height; // расчёт высоты снаряда на траектории по аним кривой
        rb.MovePosition(startPos + new Vector3(0f, f2, 0f) + direction * f1); // двигаем через РБ
        tr_shadow.position = startPos + direction * f1; // двигаем через РБ тень снаряда

        timer += Time.deltaTime;
        if (timer > time)
            Destroy(this.gameObject);

    }

    void OnTriggerEnter(Collider _col)
    {
        TriggerEnter(_col, ammoID);
    }

}

