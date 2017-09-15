using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public int ammoID = 0;
    private Transform parent;
    private Vector3 startPos;
    [SerializeField]
    private AnimationCurve trajectory;

    [SerializeField]
    private float timer = 0f;
    [SerializeField]
    private float dist;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float height = 4f;
    private Rigidbody rb; // снаряд
    private Transform tr_shadow; // тень снаряда
    private Vector3 direction;
    private float time;

    /// <summary>
    /// Задание параметров снаряда
    /// </summary>
    /// <param name="_direction">Направление по XZ</param>
    /// <param name="_dist">Дальность</param>
    /// <param name="_speed">Скорость полета (передем из параметров снаряда)</param>
    /// <param name="_height">Высота траектории</param>
    /// <param name="_parent">Хозяин снаряда</param>
    public void SetParams(Vector3 _direction, float _dist, float _speed, float _height, Transform _parent)
    {
        dist = _dist;
        speed = _speed;
        time = dist / speed; // рассчитываем время полета
        direction = _direction * dist; // расстояние приземления снаряда
        height = _height;
    }

	//void OnCollisionEnter(Collision __collision) {}
	void OnTriggerEnter(Collider _col)
    {
        if (_col.transform != parent && !_col.CompareTag("water"))
            _col.gameObject.GetComponent<Ship>().data.SetDamage(ammoID);
        Destroy(this.gameObject);
        // запускаем анимашку взрыва
        //
    }

	//void Awake(){}
	//void OnEnable(){}
	//void OnDisable(){}
	//void OnDestroy(){}
	//void OnGUI(){}
	void Start ()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        tr_shadow = transform.GetChild(0);
        //transform.GetChild(0).parent = null;
    }
	void Update ()
    {
        if (ammoID == 1)
        {
            float f1 = timer / time; // Коэфициент [0;1] положенияна траектории
            float f2 = trajectory.Evaluate(f1) * height; // расчёт высоты снаряда на траектории по аним кривой
            rb.MovePosition(startPos + new Vector3(0f, f2, 0f) + direction * f1); // двигаем через РБ
            tr_shadow.position = startPos + direction * f1; // двигаем через РБ тень снаряда

            timer += Time.deltaTime;
            if (timer > time)
                OnBecameInvisible();
        }
    }
	//void LateUpdate () {}
	void FixedUpdate ()
    {
        if (Vector3.SqrMagnitude(startPos - transform.position) > 400f)
        {
            Destroy(this.gameObject);
            // запускаем анимашку взрыва
            //
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
        //Destroy(tr_shadow.gameObject);
    }
}
