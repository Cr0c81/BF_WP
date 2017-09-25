using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum Enum_control
{
    none = 0,
    accelerate = 1,
    brake = 2
}
[System.Serializable]
public class ShipData {

    public bool playerControlled = false;
    [Tooltip("Минимальное расстояние до противника")]
    public float minDistance = 1f;
    [Header("Скорость")]
    [Tooltip("Замедление от скорости водоворота")]
    public float speed = -5f;
    [Tooltip("Ускорение")]
    public float accelerate = 10f;
    [Tooltip("Замедление")]
    public float brake = 15f;
    [Header("Максимальные значения")]
    public float health_body_max = 100f;
    public float health_team_max = 100f;
    public float health_control_max = 100f;
    [Header("Текущие значения")]
    public float health_body = 100f;
    public float health_team = 100f;
    public float health_control = 100f;

#region Оружие
    [Header("Оружейка")]
    [Tooltip("ID пушки")]
    public int cannonID;
    [Tooltip("Ссылка на снаряд")]
    public AmmoItem ammo = null;
    [Tooltip("Ссылка на пушку")]
    public CannonItem cannon = null;
    [Tooltip("Флаг перезарядки")]
    public bool cannonReload = true;
    [Tooltip("Флаг переключения")]
    public bool cannonSwitch = false;
    [Tooltip("Таймер перезарядки")]
    public float cannonReloadTimer = 0f;
    public float maxReloadTime = 3f;
    [Tooltip("Таймер переключения")]
    public float cannonSwitchTimer = 0f;
    public float maxSwitchTime = 0f;
    
    [Header("Прицел - общее")]
    [Tooltip("Цвет прицела в начале")]
    public Color aim_color_start;
    [Tooltip("Цвет прицела в конце")]
    public Color aim_color_end;

    [Header("Прицел - тип CANNON")]
    [Tooltip("Трансформ прицела пушки")]
    public Transform tr_aim;
    public Transform tr_canvas_aim;
    [Tooltip("Спрайт левой части прицела пушки")]
    public Image aim_left;
    [Tooltip("Спрайт правой части прицела пушки")]
    public Image aim_right;

    [Header("Прицел - тип MORTAR")]
    [Tooltip("Трансформ прицела")]
    public Transform tr_mortar_aim;
    [Tooltip("Картинка прицела")]
    public Image aim_mortar;
    [Tooltip("Поднятие над точкой прицела")]
    public float mortar_aim_overhead = 0.01f;
    [Tooltip("Вращение прицела при сведении")]
    public bool canRotate = true;
    [Tooltip("Скорость вращения прицела")]
    public float rotateSpeed = 30f;
    #endregion

    [Space(10)]
    [Tooltip("Трансформ корабля")]
    public Transform tr_ship;
    [Tooltip("Трансформ метки для UI")]
    public Transform UI_ship;
    [Tooltip("Трансформ пушки")]
    public Transform tr_cannon;
    [Tooltip("Трансформ центра вращения")]
    public Transform tr_center;
    [Tooltip("Енум управления газ/тормоз/ничего")]
    public Enum_control ship_move = Enum_control.none;

    private Whirpool wp;

    public void InitShipData(Transform _parent)
    {
        tr_ship = _parent.parent;
        tr_center = tr_ship.parent;
        tr_cannon = tr_ship.GetChild(1).GetChild(0);
        UI_ship = tr_ship.GetChild(0);
        Transform t = tr_ship.parent;
        tr_canvas_aim = t.GetChild(0);
        tr_aim = t.GetChild(0).GetChild(0).GetChild(0);
        aim_left = tr_aim.GetChild(0).GetChild(1).GetComponent<Image>();
        aim_right = tr_aim.GetChild(0).GetChild(0).GetComponent<Image>();

        tr_mortar_aim = GameObject.FindGameObjectWithTag("mortar_aim").transform.parent.parent;
        aim_mortar = tr_mortar_aim.GetChild(0).GetChild(0).GetComponent<Image>();
        aim_mortar.enabled = false;

        aim_left.enabled = false;
        aim_right.enabled = false;

        wp = Whirpool.Instance;
    }

    public void MoveShip()
    {
        float player_move = 0f;
        if (ship_move == Enum_control.accelerate) player_move = accelerate;
        if (ship_move == Enum_control.brake) player_move = -brake;
        tr_center.Rotate(Vector3.up, (wp.speed_whirpool - speed + player_move) * Time.deltaTime, Space.World);
        if (ship_move != Enum_control.none)
        {
            float range = wp.DistanceBetweenShips();
            if (range < minDistance)
                tr_center.Rotate(Vector3.up, -player_move * Time.deltaTime, Space.World);
        }
    }

    public void ReloadCannon()
    {
        cannonReload = true;
        cannonReloadTimer = 0f;
        maxReloadTime = cannon.reloadTime;
    }

    public void SwitchCannon(int id)
    {
        SwitchCannon(WeaponData.Instance.cannons[id], null);
    }

    public void SwitchCannon(CannonItem ci, AmmoItem ai)
    {
        cannonSwitch = true;
        cannonSwitchTimer = 0f;
        maxSwitchTime = ci.switchTime;
        cannon = ci;
        ammo = WeaponData.Instance.ammos[ci.ammoType];
    }

    public void ProcessTimers(float t)
    {
        if (cannonReload && !cannonSwitch)
        {
            cannonReloadTimer += t;
            if (cannonReloadTimer >= maxReloadTime)
                cannonReload = false;
        }
        if (cannonSwitch)
        {
            cannonSwitchTimer += t;
            if (cannonSwitchTimer >= maxSwitchTime)
                cannonSwitch = false;
        }
    }

    public void SetDamage(int _ammoID)
    {
        AmmoItem ai = WeaponData.Instance.ammos[_ammoID];
        float base_dmg = ai.damage * ai.target.body;
        float team_dmg = ai.damage * ai.target.team;
        float ctrl_dmg = ai.damage * ai.target.control;
        bool isMiss = (Random.Range(0f, 1f) <= ai.missChance) ? true : false;
        bool isCrit = (Random.Range(0f, 1f) <= ai.critChance) ? true : false;
        if (isCrit)
        {
            base_dmg *= ai.critMultiplier;
            team_dmg *= ai.critMultiplier;
            ctrl_dmg *= ai.critMultiplier;
            Debug.Log("Crit!");
        }
        if (!isMiss)
        {
            health_body = health_body - base_dmg;
            health_team = Mathf.Clamp(health_team - team_dmg, 0f, health_team_max);
            health_control = Mathf.Clamp(health_control - ctrl_dmg, 0f, health_control_max);
        }
    }

    public void ResetAllHealth()
    {
        health_body = health_body_max;
        health_control = health_control_max;
        health_team = health_team_max;
    }
}
