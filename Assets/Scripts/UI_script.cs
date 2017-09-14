using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class Panel_health_config
{
    public Transform ship_UI;
    public RectTransform panel_ship;
    public Image img_reload;
    public Image img_body;
    public Image img_team;
    public Image img_ctrl;
}

[System.Serializable]
public class Weapon_config_class
{
    [Tooltip("тип визуализации")]
    public WeaponSwitchDrawing canColor = WeaponSwitchDrawing.coloring;
    [Tooltip("Цвет включенного фона")]
    public Color color_active = Color.green;
    [Tooltip("Цвет выключенного фона")]
    public Color color_inactive = Color.gray;
    [Tooltip("Фоны кнопок")]
    public Image[] bgs;
    [Tooltip("Рамки кнопок")]
    public Image[] bgs_borders;
}

public enum WeaponSwitchDrawing
{
    coloring = 0,  // раскраска фона
    bordering = 1, // включение рамок
    color_border = 2 // рамки с раскраской
}

public class UI_script : MonoBehaviour {

// ссылка на себя, типа как синглтон, но это чтобы не искать его в других скриптах
public static UI_script Instance { get; private set; }

#region панели здоровья
    [Header("Панели здоровья кораблей")]
    public Panel_health_config[] panel_health;
    public Transform canvas_panel_holder;
    public GameObject prefab_panel;
    public enum ShipParent
    {
        Player = 0,
        Enemy = 1
    }
    public enum ShipHealth
    {
        Body = 0,
        Team = 1,
        Control = 2
    }
    public void SetHealthShip(int index, ShipData sd)
    {
        panel_health[index].img_reload.fillAmount = sd.cannonReloadTimer / sd.maxReloadTime;
        panel_health[index].img_body.fillAmount = sd.health_body / sd.health_body_max;
        panel_health[index].img_team.fillAmount = sd.health_team / sd.health_team_max;
        panel_health[index].img_ctrl.fillAmount = sd.health_control / sd.health_control_max;
    }

    public void InitPanels()
    {
        panel_health = new Panel_health_config[Whirpool.Instance.ships.Length];
        for (int i=0; i<Whirpool.Instance.ships.Length; i++)
        {
            GameObject go = Instantiate<GameObject>(prefab_panel, canvas_panel_holder);
            panel_health[i] = new Panel_health_config();
            panel_health[i].panel_ship = go.GetComponent< RectTransform>();
            panel_health[i].ship_UI = Whirpool.Instance.ships[i].UI_ship;
            panel_health[i].img_reload = go.transform.GetChild(0).GetComponent<Image>();
            panel_health[i].img_body = go.transform.GetChild(1).GetComponent<Image>();
            panel_health[i].img_team= go.transform.GetChild(2).GetComponent<Image>();
            panel_health[i].img_ctrl= go.transform.GetChild(3).GetComponent<Image>();
        }
    }
    #endregion

#region выбор оружия
    [Header("Выбор оружия")]
    public Weapon_config_class weapon_config;
    private int old_wpn = -1;
    public void WeaponClick(int value)
    {
        switch (weapon_config.canColor)
        {
            case WeaponSwitchDrawing.coloring:
                {
                    if (old_wpn != -1)
                    {
                        weapon_config.bgs[old_wpn].color = weapon_config.color_inactive;
                    }
                    weapon_config.bgs[value].color = weapon_config.color_active;
                    break;
                }
            case WeaponSwitchDrawing.bordering:
                {
                    if (old_wpn != -1)
                    {
                        weapon_config.bgs_borders[old_wpn].enabled = false;
                    }
                    weapon_config.bgs_borders[value].enabled = true;
                    break;
                }
            case WeaponSwitchDrawing.color_border:
                {
                    if (old_wpn != -1)
                    {
                        weapon_config.bgs_borders[old_wpn].enabled = false;
                    }
                    weapon_config.bgs_borders[value].color = weapon_config.color_active;
                    weapon_config.bgs_borders[value].enabled = true;
                    break;
                }
        }
        old_wpn = value;
        Whirpool.Instance.ships[0].cannonID = value;
        Whirpool.Instance.ships[0].SwitchCannon(value);
        Whirpool.Instance.ships[0].ReloadCannon();
    }

#endregion

#region методы монобеха
    void Update()
    {
        panel_health[0].panel_ship.position = Camera.main.WorldToScreenPoint(panel_health[0].ship_UI.position);
        panel_health[1].panel_ship.position = Camera.main.WorldToScreenPoint(panel_health[1].ship_UI.position);
    }

    void Start()
    {
        Instance = this;
        WeaponClick(0);
    }
    void Awake()
    {
        Instance = this;
    }
#endregion
}
