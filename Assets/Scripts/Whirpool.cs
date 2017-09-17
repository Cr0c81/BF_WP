using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Whirpool : MonoBehaviour {

    // ссылка на себя, типа как синглтон, но это чтобы не искать его в других скриптах
    public static Whirpool Instance { get; private set; }
    public static bool pause = false;
    [Header("Ссылки на водоворот")]
    public Transform tr_whirpool;
    [Range(0f, 720f)]
    [Tooltip("Скорость вращения водоворота")]
    public float speed_whirpool = 0.3f;
    [Tooltip("Данные кораблей")]
    public ShipData[] ships;
    [Tooltip("Идет ли прицеливание")]
    private bool isAiming = false;
    private Vector3 last_pos;

    // ссылки на контроллеры
    private _CustomInput input;
    private WeaponData wd;
    private UI_script ui;

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
        for (int i = 0; i < s.Length; i++)
        {
            s[i].transform.GetChild(1).GetComponent<Ship>().InitShip();
            ships[i] = s[i].transform.GetChild(1).GetComponent<Ship>().data;
        }
        input = _CustomInput.Instance;
        wd = WeaponData.Instance;
        ui = UI_script.Instance;

        if (ships[1].playerControlled)
        {
            ShipData t = ships[0];
            ships[0] = ships[1];
            ships[1] = t;
        }
        ui.InitPanels();
    }
    void Start()
    {
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
    public float DistanceBetweenShips()
    {
        return (ships[0].tr_ship.position - ships[1].tr_ship.position).magnitude;
    }

    void Update () {
        //WaterShift();
        //WaterRotate();

        if (!pause)
        {
            // отдаём время в корабли, чтобы таймеры перезарядки и переключения работали
            ships[0].ProcessTimers(Time.deltaTime);
            ships[1].ProcessTimers(Time.deltaTime);
            // двигаем корабли
            ships[0].MoveShip();
            ships[1].MoveShip();
            // ------------------

            tr_whirpool.Rotate(Vector3.up, speed_whirpool * Time.deltaTime, Space.World);

            if (ships[0].ship_move == Enum_control.none)
            {
                if (input.isClick && !(ships[0].cannonReload || ships[0].cannonSwitch) && !CursorOverUI() /* */)

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
                            wd.StartAiming(ships[0], v1);
                        }
                    }
                    else
                    {
                        RaycastHit rh;
                        Ray ray = Camera.main.ScreenPointToRay(input.position);
                        LayerMask lm = LayerMask.GetMask("whirpool");
                        if (CustomRaycast(ray, out rh, lm))
                        {
                            Vector3 v1 = rh.point;
                            wd.ProcessAiming(ships[0], v1);
                            last_pos = v1;
                        }
                    }
                }
                else if (isAiming)
                {
                    isAiming = false;
                    wd.EndAiming(ships[0], last_pos);
                }
            }
        }
    }
    void LateUpdate () {
        ui.SetHealthShip(0, ships[0]);
        ui.SetHealthShip(1, ships[1]);
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
        if (value == 0) ships[0].ship_move = Enum_control.accelerate;
        if (value == 1) ships[0].ship_move = Enum_control.brake;
    }
    public void ImageUp(int value)
    {
        ships[0].ship_move = Enum_control.none;
    }
}

