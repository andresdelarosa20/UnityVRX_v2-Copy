using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// this script process and print values over the left controller interface
public class LeftControllerUI : MonoBehaviour
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

    [SerializeField] private GameObject[] LeftControllerButtonsStates_GOs;
    [SerializeField] private GameObject[] LeftControllerSlidersDepth_GOs;

    [SerializeField] private GameObject LeftControllerStick_GO;

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
        UpdateTriggerPosition(); // actualizar el nombre de este metodo no es correcto
        UpdateTriggerDepths(); // actualizar el nombre de este metodo no es correcto
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

        for (int i = 0; i < LeftControllerButtonsStates_GOs.Length; i++)
        {
            if (ButtonStatesArray[i] == true)
            {
                LeftControllerButtonsStates_GOs[i].transform.Find("On").gameObject.SetActive(true);
                LeftControllerButtonsStates_GOs[i].transform.Find("Off").gameObject.SetActive(false);
            }
            else if (ButtonStatesArray[i] == false)
            {
                LeftControllerButtonsStates_GOs[i].transform.Find("On").gameObject.SetActive(false);
                LeftControllerButtonsStates_GOs[i].transform.Find("Off").gameObject.SetActive(true);
            }
        }
    }

    public void UpdateScrollSlidersStates()
    {
        float[] DepthArray = { TriggerDepth, GripDepth };

        for (int i = 0; i < LeftControllerSlidersDepth_GOs.Length; i++)
        {
            // esta linea no pude deducirla por mi mismo -- estudiar
            LeftControllerSlidersDepth_GOs[i].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = DepthArray[i];
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
        LeftControllerStick_GO.GetComponent<RectTransform>().anchoredPosition = targetPos;

    }

    //***************
    //***************

    public void LC_TriggerButton_Fnc(bool value)
    {
        TriggerButton = value;
    }

    public void LC_GripButton_Fnc(bool value)
    {
        GripButton = value;
    }

    public void LC_AButton_Fnc(bool value)
    {
        AButton = value;
    }

    public void LC_BButton_Fnc(bool value)
    {
        BButton = value;
    }

    public void LC_MenuButton_Fnc(bool value)
    {
        MenuButton = value;
    }

    public void LC_StickButton_Fnc(bool value)
    {
        StickButton = value;
    }

    public void LC_StickNorthPos_Fnc(bool value)
    {
        StickNorthPos = value;
    }

    public void LC_StickSouthPos_Fnc(bool value)
    {
        StickSouthPos = value;
    }

    public void LC_StickWestPos_Fnc(bool value)
    {
        StickWestPos = value;
    }

    public void LC_StickEastPos_Fnc(bool value)
    {
        StickEastPos = value;
    }

    public void LC_Position_Fnc(Vector3 value)
    {
        Position = value;
    }

    public void LC_Rotation_Fnc(Vector3 value)
    {
        Rotation = value;
    }

    public void LC_Velocity_Fnc(Vector3 value)
    {
        Velocity = value;
    }

    public void LC_VelocityMagnitud_Fnc(float value) // velocity.magnitud
    {
        VelocityMagnitud = value;
    }

    public void LC_TriggerDepth_Fnc(float value)
    {
        TriggerDepth = value;
    }

    public void LC_GripDepth_Fnc(float value)
    {
        GripDepth = value;
    }

    private void LC_StickXPos_Fnc(float value)
    {
        Stick_XPos = value;
    }

    private void LC_StickYPos_Fnc(float value)
    {
        Stick_YPos = value;
    }

    private void LC_StickNorthDepth_Fnc(float value)
    {
        StickNorthDepth = value;
    }

    private void LC_StickSouthDepth_Fnc(float value)
    {
        StickSouthDepth = value;
    }

    private void LC_StickWestDepth_Fnc(float value)
    {
        StickWestDepth = value;
    }

    private void LC_StickEastDepth_Fnc(float value)
    {
        StickEastDepth = value;
    }

    //***************
    //***************
    private void OnEnable()
    {
        UISObserver.LC_TriggerButton += LC_TriggerButton_Fnc;
        UISObserver.LC_GripButton += LC_GripButton_Fnc;
        UISObserver.LC_AButton += LC_AButton_Fnc;
        UISObserver.LC_BButton += LC_BButton_Fnc;
        UISObserver.LC_MenuButton += LC_MenuButton_Fnc;
        UISObserver.LC_StickButton += LC_StickButton_Fnc;
        UISObserver.LC_StickNorthPos += LC_StickNorthPos_Fnc;
        UISObserver.LC_StickSouthPos += LC_StickSouthPos_Fnc;
        UISObserver.LC_StickWestPos += LC_StickWestPos_Fnc;
        UISObserver.LC_StickEastPos += LC_StickEastPos_Fnc;
        UISObserver.LC_Position += LC_Position_Fnc;
        UISObserver.LC_Rotation += LC_Rotation_Fnc;
        UISObserver.LC_Velocity += LC_Velocity_Fnc;
        UISObserver.LC_VelocityMagnitud += LC_VelocityMagnitud_Fnc;

        UISObserver.LC_TriggerDepth += LC_TriggerDepth_Fnc;
        UISObserver.LC_GripDepth += LC_GripDepth_Fnc;

        UISObserver.LC_StickNorthDepth += LC_StickNorthDepth_Fnc;
        UISObserver.LC_StickSouthDepth += LC_StickSouthDepth_Fnc;
        UISObserver.LC_StickWestDepth += LC_StickWestDepth_Fnc;
        UISObserver.LC_StickEastDepth += LC_StickEastDepth_Fnc;

        UISObserver.LC_StickXPos += LC_StickXPos_Fnc;
        UISObserver.LC_StickYPos += LC_StickYPos_Fnc;
    }

    private void OnDisable()
    {
        UISObserver.LC_TriggerButton -= LC_TriggerButton_Fnc;
        UISObserver.LC_GripButton -= LC_GripButton_Fnc;
        UISObserver.LC_AButton -= LC_AButton_Fnc;
        UISObserver.LC_BButton -= LC_BButton_Fnc;
        UISObserver.LC_MenuButton -= LC_MenuButton_Fnc;
        UISObserver.LC_StickButton -= LC_StickButton_Fnc;
        UISObserver.LC_StickNorthPos -= LC_StickNorthPos_Fnc;
        UISObserver.LC_StickSouthPos -= LC_StickSouthPos_Fnc;
        UISObserver.LC_StickWestPos -= LC_StickWestPos_Fnc;
        UISObserver.LC_StickEastPos -= LC_StickEastPos_Fnc;
        UISObserver.LC_Position -= LC_Position_Fnc;
        UISObserver.LC_Rotation -= LC_Rotation_Fnc;
        UISObserver.LC_Velocity -= LC_Velocity_Fnc;
        UISObserver.LC_VelocityMagnitud -= LC_VelocityMagnitud_Fnc;

        UISObserver.LC_TriggerDepth -= LC_TriggerDepth_Fnc;
        UISObserver.LC_GripDepth -= LC_GripDepth_Fnc;

        UISObserver.LC_StickNorthDepth -= LC_StickNorthDepth_Fnc;
        UISObserver.LC_StickSouthDepth -= LC_StickSouthDepth_Fnc;
        UISObserver.LC_StickWestDepth -= LC_StickWestDepth_Fnc;
        UISObserver.LC_StickEastDepth -= LC_StickEastDepth_Fnc;

        UISObserver.LC_StickXPos -= LC_StickXPos_Fnc;
        UISObserver.LC_StickYPos -= LC_StickYPos_Fnc;
    }

}
