using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour{

    [Header("Класс с данными корабля")]
    public ShipData data;
    [Header("Данные для качки")]
    [Tooltip("Время смены наклона")]
    public float timeToChange = 1f;
    [Tooltip("Угол наклона крена")]
    public float maxAngleX = 15f;
    [Tooltip("Сделано для Вики")]
    public bool forToriTest = true;
    [Header("Animate in pause")]
    public bool canAnimateInPause = true;


    private Quaternion oldRotation = Quaternion.identity;
    private Quaternion newRotation = Quaternion.identity;
    private float timer = 0f;

    public void InitShip()
    {
        data.InitShipData(transform);
        timer = Random.Range(0f, timeToChange);
        if (forToriTest)
        {
            oldRotation = Quaternion.Euler(maxAngleX, 0f, 0f);
            newRotation = Quaternion.Euler(0f, 0f, 0f);
        } else
        {
            oldRotation = Quaternion.Euler(0f, 0f, maxAngleX/2f);
            newRotation = Quaternion.Euler(0f, 0f, -maxAngleX/2f);
        }
    }

    private void Update()
    {
        if ( (canAnimateInPause) || ( !Whirpool.pause && !canAnimateInPause) )
            if (timer >= timeToChange)
            {
                timer = 0f;
            }
            float coeff = (Mathf.Sin(2 * Mathf.PI * timer / timeToChange - Mathf.PI / 2) + 1f) / 2f;
            data.tr_ship.localRotation = Quaternion.Lerp(oldRotation, newRotation, coeff);
            timer += Time.deltaTime;
    }
}
