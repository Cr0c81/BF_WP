using UnityEngine;
using System.Collections;

public class _CustomInput : MonoBehaviour {

    public static _CustomInput Instance { get; private set; }
    private Vector2 cursorPos;
    
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Instance = this;
    }
    //void OnEnable(){}
    //void OnDisable(){}
    //void OnDestroy(){}

    // == >= <= != 

    // Есть ли тап/клик
    public bool isClick
    {
        get
        {
#if UNITY_EDITOR
            bool gmb = Input.GetMouseButton(0);
            cursorPos = (Vector2)Input.mousePosition;
            return gmb;
#endif

#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
            bool tm =  (Input.touchCount > 0 ? (Input.touches[0].phase == TouchPhase.Moved || 
                                                Input.touches[0].phase == TouchPhase.Stationary || 
                                                Input.touches[0].phase == TouchPhase.Began) : false );
            cursorPos = tm ? Input.GetTouch(0).position : -Vector2.one;
            return tm;
#endif
        }
    }

    // координаты тапа/клика
    public Vector2 position
    {
        get
        {
            return cursorPos;
        }
    }
	
}
