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
    private Rigidbody rb;
    private Vector3 direction;
    public float ttt;

    public void SetParams(Vector3 _direction, float _dist, float _speed, float _height, Transform _parent)
    {
        dist = _dist;
        speed = _speed;
        direction = _direction * _speed;
        height = _height;
    }

	//void OnCollisionEnter(Collision __collision) {}
	void OnTriggerEnter(Collider _col)
    {
        if (_col.transform != parent)
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
    }
	void Update ()
    {
        if (ammoID == 1)
        {
            float f1 = timer * speed / dist;
            ttt = f1;
            f1 = trajectory.Evaluate(f1) * height * Time.deltaTime;
            rb.MovePosition(startPos + new Vector3(0f, f1, 0f) + direction * Time.deltaTime);
            timer += Time.deltaTime;

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
    }
}
