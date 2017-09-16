using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Whirpool : MonoBehaviour {

    // ссылка на себя, типа как синглтон, но это чтобы не искать его в других скриптах
    public static Whirpool Instance { get; private set; }
    [Header("Ссылки на водоворот")]
    public Transform tr_whirpool;
    [Range(0f, 720f)]
    [Tooltip("Скорость вращения водоворота")]
    public float speed_whirpool = 0.3f;
    [Header("Ссылки на корабли")]
    public Material mat_ship_red;
    public Material mat_ship_blue;
    [Tooltip("Трансформы вращения корабля")]
    public Transform[] tr_ships;
    [Tooltip("Данные кораблей")]
    public ShipData[] ships;
    [Header("Состояния корабля")]
    [SerializeField]
    [Tooltip("Енум управления газ/тормоз/ничего")]
    private Enum_control ship_move;
    [Tooltip("Идет ли прицеливание")]
    private bool isAiming = false;
    private Vector3 last_pos;

    private enum Enum_control
    {
        none = 0,
        accelerate = 1,
        brake = 2
    }
    // ссылка на контроллер ввода
    private _CustomInput input;

    #region постоянный сдвиг UV текстуры воды
    [Header("Ссылки и настройки сдвига UV фона")]
    public Renderer rend_water;
    [Tooltip("Направление сдвига")]
    public Vector2 water_dir = new Vector2(1f, 0f);
    [Tooltip("Скорость сдвига")]
    public float water_shift_speed = 0.5f;
    private void WaterShift()
    {
        Vector2 v = rend_water.material.GetTextureOffset("_MainTex");
        v += water_dir * water_shift_speed * Time.deltaTime;
        if (v.x > 1f) v.x -= 1f;
        if (v.y > 1f) v.y -= 1f;
        rend_water.material.SetTextureOffset("_MainTex", v);
    }
    #endregion

    #region вращение фона (океана)
    [Header("Анимация фона")]
    public Transform tr_water;
    [Tooltip("Скорость вращения")]
    public float water_speed = 45f;
    private void WaterRotate()
    {
        tr_water.Rotate(Vector3.up, water_speed * Time.deltaTime, Space.World);
    }
    #endregion

    void Awake()
    {
        Instance = this;
        GameObject[] s = GameObject.FindGameObjectsWithTag("ship");
        ships = new ShipData[s.Length];
        tr_ships = new Transform[s.Length];
        for (int i = 0; i < s.Length; i++)
        {
            s[i].GetComponent<Ship>().InitShip();
            ships[i] = s[i].GetComponent<Ship>().data;
            tr_ships[i] = s[i].transform.parent.parent;
        }
        input = _CustomInput.Instance;

        if (ships[1].playerControlled)
        {
            ShipData t = ships[0];
            ships[0] = ships[1];
            ships[1] = t;
            Transform tt = tr_ships[0];
            tr_ships[0] = tr_ships[1];
            tr_ships[1] = tt;
        }
        UI_script.Instance.InitPanels();
    }
    void Start()
    {
        input = _CustomInput.Instance;
        Instance = this;
    }

    private bool CursorOverUI()
{ 
#if UNITY_ANDROID || UNITY_IOS
        int cursorID = Input.GetTouch(0).fingerId;
        return EventSystem.current.IsPointerOverGameObject(cursorID);
#else
        return EventSystem.current.IsPointerOverGameObject();
#endif
    }

    void Update () {
        //WaterShift();
        //WaterRotate();

        // отдаём время в корабли, чтобы таймеры перезарядки и переключения работали
        ships[0].ProcessTimers(Time.deltaTime);
        ships[1].ProcessTimers(Time.deltaTime);
        // ------------------

        float player_move = 0f;
        float player_angle = Vector3.Dot(ships[0].tr_ship.position - tr_whirpool.position, ships[1].tr_ship.position - tr_whirpool.position);
        if (ship_move == Enum_control.accelerate) player_move = ships[0].accelerate;
        if (ship_move == Enum_control.brake) player_move = -ships[0].brake;
        if (ship_move != Enum_control.none)
        {
            Vector3 v1 = ships[0].tr_ship.position - tr_whirpool.position;
            Vector3 v2 = ships[1].tr_ship.position - tr_whirpool.position;
            player_angle = Vector3.Angle(v1, v2);
            if ( player_angle < 0 ) player_move = 0f;
        }
        tr_whirpool.Rotate(Vector3.up, speed_whirpool * Time.deltaTime, Space.World);
        tr_ships[0].Rotate(Vector3.up, (speed_whirpool - ships[0].speed + player_move) * Time.deltaTime, Space.World);
        tr_ships[1].Rotate(Vector3.up, (speed_whirpool - ships[1].speed) * Time.deltaTime, Space.World);


        if (ship_move == Enum_control.none)
        {
            if ( input.isClick && !(ships[0].cannonReload || ships[0].cannonSwitch) && !CursorOverUI() /* */)
                
            {
                if (!isAiming)
                {
                    isAiming = true;
                    RaycastHit rh;
                    Ray ray = Camera.main.ScreenPointToRay(input.position);
                    LayerMask lm = LayerMask.GetMask("whirpool");
                    if (CustomRaycast(ray, out rh, lm))
                    {
                        Vector3 v1 = rh.point;
                        WeaponData.Instance.StartAiming(ships[0], v1);
                    }
                } else
                {
                    RaycastHit rh;
                    Ray ray = Camera.main.ScreenPointToRay(input.position);
                    LayerMask lm = LayerMask.GetMask("whirpool");
                    if (CustomRaycast(ray, out rh, lm))
                    {
                        Vector3 v1 = rh.point;
                        WeaponData.Instance.ProcessAiming(ships[0], v1);
                        last_pos = v1;
                    }
                }
            }
            else if (isAiming)
            {
                isAiming = false;
                WeaponData.Instance.EndAiming(ships[0], last_pos);
            }
        }
    }
    void LateUpdate () {
        UI_script.Instance.SetHealthShip(0, ships[0]);
        UI_script.Instance.SetHealthShip(1, ships[1]);
    }
    //void FixedUpdate () {}
    private bool CustomRaycast(Ray _ray, out RaycastHit _rh, LayerMask _lm)
    {
        _rh = new RaycastHit();
        RaycastHit[] rh = Physics.RaycastAll(_ray, 100f, _lm);
        int index = 0;
        if (rh.Length == 0)
            return false;
        if (rh.Length > 1)
        {
            for (int i = 0; i < rh.Length; i++)
                if (rh[i].distance < rh[index].distance) index = i;
        }
        else
        {
            _rh = rh[0];
            return true;
        }
            _rh = rh[index];
            return true;
    }

    public void ImageDown(int value)
    {
        if (value == 0) ship_move = Enum_control.accelerate;
        if (value == 1) ship_move = Enum_control.brake;
    }
    public void ImageUp(int value)
    {
        ship_move = Enum_control.none;
    }
}

