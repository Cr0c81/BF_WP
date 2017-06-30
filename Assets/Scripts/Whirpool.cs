using UnityEngine;
using System.Collections;

public class Whirpool : MonoBehaviour {

    // ссылка на себя, типа как синглтон, но это чтобы не искать его в других скриптах
    public static Whirpool Instance { get; private set; }
    [Header("Ссылки на водоворот")]
    public Transform tr_whirpool;
    [Range(0f, 720f)]
    public float speed_whirpool = 0.3f;
    [Header("Ссылки на корабли")]
    public Material mat_ship_red;
    public Material mat_ship_blue;
    public Transform tr_ship1;
    public ShipData ship1;
    public Transform tr_ship2;
    public ShipData ship2;
    [Header("Состояния корабля")]
    [SerializeField]
    private enum_control ship_move;

    private enum enum_control
    {
        none = 0,
        accelerate = 1,
        brake = 2
    }

#region постоянный сдвиг UV текстуры воды
    [Header("Ссылки и настройки сдвига UV фона")]
    public Renderer rend_water;
    public Vector2 water_dir = new Vector2(1f, 0f);
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
    public float water_speed = 45f;
    private void WaterRotate()
    {
        tr_water.Rotate(Vector3.up, water_speed * Time.deltaTime, Space.World);
    }
#endregion

    void Awake(){
        if (!Instance) Instance = this;
    }
    //void OnEnable(){}
    //void OnDisable(){}
    //void OnDestroy(){}
    //void OnGUI(){}
    void Start () {
        
    }

    void Update () {
        WaterShift();
        WaterRotate();
        float player_move = 0f;
        if (ship_move == enum_control.accelerate) player_move = ship1.accelerate;
        if (ship_move == enum_control.brake) player_move = -ship1.brake;
        tr_whirpool.Rotate(Vector3.up, speed_whirpool * Time.deltaTime, Space.World);
        tr_ship1.Rotate(Vector3.up, (ship1.speed + player_move) * Time.deltaTime, Space.World);
        tr_ship2.Rotate(Vector3.up, (ship2.speed) * Time.deltaTime, Space.World);
    }
    void LateUpdate () {
        UI_script.Instance.SetHealthShip(UI_script.ShipParent.Player, ship1);
        UI_script.Instance.SetHealthShip(UI_script.ShipParent.Enemy, ship2);
    }
    //void FixedUpdate () {}

    public void ImageDown(int value)
    {
        if (value == 0) ship_move = enum_control.accelerate;
        if (value == 1) ship_move = enum_control.brake;
    }
    public void ImageUp(int value)
    {
        ship_move = enum_control.none;
    }
}

