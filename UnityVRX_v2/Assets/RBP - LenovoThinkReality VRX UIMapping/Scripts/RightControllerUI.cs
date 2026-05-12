using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// this script process and print values over the left controller interface
public class RightControllerUI : MonoBehaviour
{
    public bool TriggerButton = false;
    public bool GripButton = true;
    public bool AButton = false;
    public bool BButton = true;
    public bool MenuButton = false;
    public bool StickButton = false;
    public bool StickNorthPos = false;
    public bool StickSouthPos = false;
    public bool StickWestPos = false;
    public bool StickEastPos = false;

    public float StickNorthDepth;
    public float StickSouthDepth;
    public float StickWestDepth;
    public float StickEastDepth;

    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Velocity;
    public float VelocityMagnitud; // not used



    [SerializeField] private float StickRadius = 1.4f;

    [Range(-1, 1)]
    public float Stick_XPos = 0f;

    [Range(-1, 1)]
    public float Stick_YPos = 0f;

    [SerializeField] private TextMeshProUGUI TMP_TriggerDepth;
    [SerializeField] private TextMeshProUGUI TMP_GripDepth;

    [SerializeField] private TextMeshProUGUI StickNorthDepth_Text;
    [SerializeField] private TextMeshProUGUI StickSouthDepth_Text;
    [SerializeField] private TextMeshProUGUI StickWestDepth_Text;
    [SerializeField] private TextMeshProUGUI StickEastDepth_Text;

    [Range(0f, 1f)]
    public float TriggerDepth = 0f;

    [Range(0f, 1f)]
    public float GripDepth = 0f;

    [SerializeField] private TextMeshProUGUI TMP_Position;
    [SerializeField] private TextMeshProUGUI TMP_Rotation;
    [SerializeField] private TextMeshProUGUI TMP_Velocity;
    [SerializeField] private TextMeshProUGUI TMP_Velocity_Manitude;

    [SerializeField] private GameObject[] RightControllerButtonsStates_GOs;
    [SerializeField] private GameObject[] RightControllerSlidersDepth_GOs;

    [SerializeField] private GameObject RightControllerStick_GO;

    [SerializeField] private TextMeshProUGUI TMP_Stick_NorthSector;
    [SerializeField] private TextMeshProUGUI TMP_Stick_SouthSector;
    [SerializeField] private TextMeshProUGUI TMP_Stick_WestSector;
    [SerializeField] private TextMeshProUGUI TMP_Stick_EastSector;

    private void Awake()
    {
        UpdateValues();
        UpdateButtonsStates();
        UpdateScrollSlidersStates();
        UpdateSectorsTilt();
        UpdateTriggerPosition(); /// hola
    }

    private void UpdateSectorsTilt()
    {
        // NORTH
        if (Stick_YPos >= 0 && Stick_YPos <= StickRadius)
        {
            TMP_Stick_NorthSector.text = Mathf.Abs(Stick_YPos).ToString("F2");
            TMP_Stick_SouthSector.text = "0,00";
        }

        // SOUTH
        if (Stick_YPos <= 0 && Stick_YPos >= -StickRadius)
        {
            TMP_Stick_SouthSector.text = Mathf.Abs(Stick_YPos).ToString("F2");
            TMP_Stick_NorthSector.text = "0,00";
        }

        // EAST
        if (Stick_XPos >= 0 && Stick_XPos <= StickRadius)
        {
            TMP_Stick_EastSector.text = Mathf.Abs(Stick_XPos).ToString("F2");
            TMP_Stick_WestSector.text = "0,00";
        }

        // WEST
        if (Stick_XPos <= 0 && Stick_XPos >= -StickRadius)
        {
            TMP_Stick_WestSector.text = Mathf.Abs(Stick_XPos).ToString("F2");
            TMP_Stick_EastSector.text = "0,00";
        }
    }

    private void LateUpdate()
    {
        UpdateValues();
        UpdateButtonsStates();
        UpdateScrollSlidersStates();
        UpdateSectorsTilt();
        UpdateTriggerPosition();
        UpdateTriggerDepths();
    }

    private void UpdateTriggerDepths()
    {
        StickNorthDepth_Text.text = StickNorthDepth.ToString("F2");
        StickSouthDepth_Text.text = StickSouthDepth.ToString("F2");
        StickWestDepth_Text.text = StickWestDepth.ToString("F2");
        StickEastDepth_Text.text = StickEastDepth.ToString("F2");
    }

    private void UpdateValues()
    {
        TMP_Position.text = $"{Position.x:F2}  {Position.y:F2}  {Position.z:F2}";
        TMP_Rotation.text = $"{Rotation.x:F0}  {Rotation.y:F0}  {Rotation.z:F0}";
        TMP_Velocity.text = $"{Velocity.x:F2}  {Velocity.y:F2}  {Velocity.z:F2}";
        TMP_Velocity_Manitude.text = $"{VelocityMagnitud:F2}";

        TMP_TriggerDepth.text = $"{TriggerDepth:F2}";
        TMP_GripDepth.text = $"{GripDepth:F2}";
    }

    public void UpdateButtonsStates()
    {
        bool[] ButtonStatesArray =
        {
            TriggerButton,
            GripButton,
            AButton,
            BButton,
            MenuButton,
            StickButton,
            StickButton,     // esta ultima esta repedita para que el Stick cambie al tiempo que lo hace el boton de la interfaz,
            StickNorthPos,
            StickSouthPos,
            StickWestPos,
            StickEastPos
        };

        for (int i = 0; i < RightControllerButtonsStates_GOs.Length; i++)
        {
            if (ButtonStatesArray[i] == true)
            {
                RightControllerButtonsStates_GOs[i].transform.Find("On").gameObject.SetActive(true);
                RightControllerButtonsStates_GOs[i].transform.Find("Off").gameObject.SetActive(false);
            }
            else if (ButtonStatesArray[i] == false)
            {
                RightControllerButtonsStates_GOs[i].transform.Find("On").gameObject.SetActive(false);
                RightControllerButtonsStates_GOs[i].transform.Find("Off").gameObject.SetActive(true);
            }
        }
    }

    public void UpdateScrollSlidersStates()
    {
        float[] DepthArray = { TriggerDepth, GripDepth };

        for (int i = 0; i < RightControllerSlidersDepth_GOs.Length; i++)
        {
            // esta linea no pude deducirla por mi mismo -- estudiar
            RightControllerSlidersDepth_GOs[i].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = DepthArray[i];
        }
    }

    public void UpdateTriggerPosition()
    {
        Vector2 Stick_XYPos;

        Stick_XYPos.x = Stick_XPos;
        Stick_XYPos.y = Stick_YPos;

        Vector2 targetPos = Stick_XYPos * StickRadius;

        // esta es una de las lineas mas interesantes de este codigo, usando la funcion matemarica LERP - primera vez utilizandola
        //LeftControllerStick_GO.GetComponent<RectTransform>().anchoredPosition = (Stick_XYPos * StickRadius).normalized; // Este limita el movimiento a un circulo sin incrementos


        targetPos = Vector2.ClampMagnitude(targetPos, StickRadius); // limita el radio con incrementos
        RightControllerStick_GO.GetComponent<RectTransform>().anchoredPosition = targetPos;
    }

    //***************

    public void RC_TriggerButton_Fnc(bool value)
    {
        TriggerButton = value;
    }

    public void RC_GripButton_Fnc(bool value)
    {
        GripButton = value;
    }

    public void RC_AButton_Fnc(bool value)
    {
        AButton = value;
    }

    public void RC_BButton_Fnc(bool value)
    {
        BButton = value;
    }

    public void RC_MenuButton_Fnc(bool value)
    {
        MenuButton = value;
    }

    public void RC_StickButton_Fnc(bool value)
    {
        StickButton = value;
    }

    public void RC_StickNorthPos_Fnc(bool value)
    {
        StickNorthPos = value;
    }

    public void RC_StickSouthPos_Fnc(bool value)
    {
        StickSouthPos = value;
    }

    public void RC_StickWestPos_Fnc(bool value)
    {
        StickWestPos = value;
    }

    public void RC_StickEastPos_Fnc(bool value)
    {
        StickEastPos = value;
    }

    public void RC_Position_Fnc(Vector3 value)
    {
        Position = value;
    }

    public void RC_Rotation_Fnc(Vector3 value)
    {
        Rotation = value;
    }

    public void RC_Velocity_Fnc(Vector3 value)
    {
        Velocity = value;
    }

    public void RC_VelocityMagnitud_Fnc(float value) // velocity.magnitud
    {
        VelocityMagnitud = value;
    }

    public void RC_TriggerDepth_Fnc(float value)
    {
        TriggerDepth = value;
    }

    public void RC_GripDepth_Fnc(float value)
    {
        GripDepth = value;
    }

    private void RC_StickXPos_Fnc(float value)
    {
        Stick_XPos = value;
    }

    private void RC_StickYPos_Fnc(float value)
    {
        Stick_YPos = value;
    }

    private void RC_StickNorthDepth_Fnc(float value)
    {
        StickNorthDepth = value;
    }

    private void RC_StickSouthDepth_Fnc(float value)
    {
        StickSouthDepth = value;
    }

    private void RC_StickWestDepth_Fnc(float value)
    {
        StickWestDepth = value;
    }

    private void RC_StickEastDepth_Fnc(float value)
    {
        StickEastDepth = value;
    }

    //***************
    //***************
    private void OnEnable()
    {
        UISObserver.RC_TriggerButton += RC_TriggerButton_Fnc;
        UISObserver.RC_GripButton += RC_GripButton_Fnc;
        UISObserver.RC_AButton += RC_AButton_Fnc;
        UISObserver.RC_BButton += RC_BButton_Fnc;
        UISObserver.RC_MenuButton += RC_MenuButton_Fnc;
        UISObserver.RC_StickButton += RC_StickButton_Fnc;
        UISObserver.RC_StickNorthPos += RC_StickNorthPos_Fnc;
        UISObserver.RC_StickSouthPos += RC_StickSouthPos_Fnc;
        UISObserver.RC_StickWestPos += RC_StickWestPos_Fnc;
        UISObserver.RC_StickEastPos += RC_StickEastPos_Fnc;
        UISObserver.RC_Position += RC_Position_Fnc;
        UISObserver.RC_Rotation += RC_Rotation_Fnc;
        UISObserver.RC_Velocity += RC_Velocity_Fnc;
        UISObserver.RC_VelocityMagnitud += RC_VelocityMagnitud_Fnc;
        UISObserver.RC_TriggerDepth += RC_TriggerDepth_Fnc;
        UISObserver.RC_GripDepth += RC_GripDepth_Fnc;

        UISObserver.RC_StickNorthDepth += RC_StickNorthDepth_Fnc;
        UISObserver.RC_StickSouthDepth += RC_StickSouthDepth_Fnc;
        UISObserver.RC_StickWestDepth += RC_StickWestDepth_Fnc;
        UISObserver.RC_StickEastDepth += RC_StickEastDepth_Fnc;

        UISObserver.RC_StickXPos += RC_StickXPos_Fnc;
        UISObserver.RC_StickYPos += RC_StickYPos_Fnc;

    }

    private void OnDisable()
    {
        UISObserver.RC_TriggerButton -= RC_TriggerButton_Fnc;
        UISObserver.RC_GripButton -= RC_GripButton_Fnc;
        UISObserver.RC_AButton -= RC_AButton_Fnc;
        UISObserver.RC_BButton -= RC_BButton_Fnc;
        UISObserver.RC_MenuButton -= RC_MenuButton_Fnc;
        UISObserver.RC_StickButton -= RC_StickButton_Fnc;
        UISObserver.RC_StickNorthPos -= RC_StickNorthPos_Fnc;
        UISObserver.RC_StickSouthPos -= RC_StickSouthPos_Fnc;
        UISObserver.RC_StickWestPos -= RC_StickWestPos_Fnc;
        UISObserver.RC_StickEastPos -= RC_StickEastPos_Fnc;
        UISObserver.RC_Position -= RC_Position_Fnc;
        UISObserver.RC_Rotation -= RC_Rotation_Fnc;
        UISObserver.RC_Velocity -= RC_Velocity_Fnc;
        UISObserver.RC_VelocityMagnitud -= RC_VelocityMagnitud_Fnc;
        UISObserver.RC_TriggerDepth -= RC_TriggerDepth_Fnc;
        UISObserver.RC_GripDepth -= RC_GripDepth_Fnc;

        UISObserver.RC_StickNorthDepth -= RC_StickNorthDepth_Fnc;
        UISObserver.RC_StickSouthDepth -= RC_StickSouthDepth_Fnc;
        UISObserver.RC_StickWestDepth -= RC_StickWestDepth_Fnc;
        UISObserver.RC_StickEastDepth -= RC_StickEastDepth_Fnc;

        UISObserver.RC_StickXPos -= RC_StickXPos_Fnc;
        UISObserver.RC_StickYPos -= RC_StickYPos_Fnc;

    }
}
