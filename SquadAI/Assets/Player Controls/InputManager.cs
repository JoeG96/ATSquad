using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] Movement movement;
    PlayerControls playerControls;
    PlayerControls.GroundMovementActions groundMovement;
    Vector2 horizontalInput;

    [SerializeField] MouseLook mouseLook;
    Vector2 mouseInput;

    [SerializeField] PlayerCommands playerCommands;

    public bool leftClick;
    public bool rightClick;
    [SerializeField] bool oneInput;
    [SerializeField] bool twoInput;
    [SerializeField] bool threeInput;
    [SerializeField] bool recallInput;
    [SerializeField] bool cycleSquadInput;
    [SerializeField] bool roamingInput;
    [SerializeField] bool findInput;
    [SerializeField] bool huntInput;
    [SerializeField] bool escapeInput;

    private RawImage squadIndImage;
    public NotificationManager notification;

    private void Awake()
    {

        squadIndImage = GameObject.Find("SelectedSquadIndicator").GetComponent<RawImage>();

        playerControls = new PlayerControls();
        groundMovement = playerControls.GroundMovement;


        groundMovement.HorizontalMovement.performed += ctx => horizontalInput = ctx.ReadValue <Vector2>();

        groundMovement.Jump.performed += _ => movement.OnJumpPress();

        groundMovement.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        groundMovement.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();

        groundMovement.Recall.performed += i => recallInput = true;
        groundMovement.Recall.canceled += i => recallInput = true;
        
        groundMovement.LeftClick.performed += i => leftClick = true;
        groundMovement.LeftClick.canceled += i => leftClick = false;
        
        groundMovement.RightClick.performed += i => rightClick = true;
        //groundMovement.RightClick.canceled += i => rightClick = false;

        groundMovement.CycleSquad.performed += i => cycleSquadInput = true;
        groundMovement.CycleSquad.canceled += i => cycleSquadInput = false;

        groundMovement.Roaming.performed += i => roamingInput = true;
        groundMovement.Roaming.canceled += i => roamingInput = false;        
        
        groundMovement.Find.performed += i => findInput = true;
        groundMovement.Find.canceled += i => findInput = false;
        
        groundMovement.Hunt.performed += i => huntInput = true;
        groundMovement.Hunt.canceled += i => huntInput = false;

        groundMovement.Escape.performed += ctx => escapeInput = true;
        groundMovement.Escape.canceled += ctx => escapeInput = false;

        oneInput = true; // starts with first squaddie selected
        squadIndImage.color = Color.blue;
    }

    private void Update()
    {
        movement.ReceiveInput(horizontalInput);
        mouseLook.ReceiveInput(mouseInput);

        if (leftClick)
        {
            GetLeftMouseClicked();
            LeftMouseClicked();
            leftClick = false;
        }


/*        if (rightClick)
        {
            GetRightMouseClicked();
            RightMouseClicked();
            rightClick = false;
        }*/

        if (recallInput)
        {
            RecallPressed();
            recallInput = false;
        }

        if(cycleSquadInput)
        {
            CycleSelectedSquad();
            cycleSquadInput = false;
        }

        if (roamingInput)
        {
            playerCommands.SetToRoam();
            roamingInput = false;
        }
        
        if (findInput)
        {
            playerCommands.SetToFind();
            findInput = false;
        }

        if (huntInput)
        {
            playerCommands.SetToEnemyFind();
            huntInput = false;
        }

        if (escapeInput)
        {
            playerCommands.SetToFollow();
            escapeInput = false;
        }

    }

    public bool RightClickPressed()
    {
        //Debug.Log("Right Click Pressed");
        //mouseLook.enabled = !mouseLook.enabled;
        return groundMovement.RightClick.triggered;
    }

    private void CheckSelectedSquad()
    {
        if (oneInput)
        {
            //Debug.Log("Button 1 Selected");
            twoInput = false;
            threeInput = false;
        }
        else if (twoInput)
        {
            //Debug.Log("Button 2 Selected");
            oneInput = false;
            threeInput = false;
        }
        else if (threeInput)
        {
            //Debug.Log("Button 3 Selected");
            oneInput = false;
            twoInput = false;
        }
        else
        {
            return;
        }
    }

    public void LeftMouseClicked()
    {
        //Debug.Log("Left Mouse Clicked");
        //playerCommands.ReceiveInput();
    }

    public void RightMouseClicked()
    {
        Debug.Log("Right Mouse Clicked");
        mouseLook.enabled = !mouseLook.enabled;
        //playerCommands.ReceiveInput();
    }

    public bool GetLeftMouseClicked()
    {
        if (leftClick == true)
        {
            return true;
        } 
        return false;
    }
    public bool GetRightMouseClicked()
    {
        if (rightClick == true)
        {
            return true;
        }
        return false;
    }

    public bool GetEscapePressed()
    {
        if (escapeInput)
        {
            return true;
        }
        return false;
    }

    private void RecallPressed()
    {
        //Debug.Log("Q key pressed");
        playerCommands.SetToRecall();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void CycleSelectedSquad()
    {
        if (oneInput)
        {
            notification.CallSend("Squad Member 2 Selected", 2);
            oneInput = false; 
            twoInput = true;
            threeInput = false;
            squadIndImage.color = Color.green;
        }
        else if (twoInput)
        {
            notification.CallSend("Squad Member 3 Selected", 2);
            oneInput = false;
            twoInput = false;
            threeInput = true;
            squadIndImage.color = Color.red;
        }
        else if (threeInput)
        {
            notification.CallSend("Squad Member 1 Selected", 2);
            oneInput = true;
            twoInput = false;
            threeInput = false;
            squadIndImage.color = Color.blue;
        }
        return;
    }

    public bool SquadMember1Selected()
    {
        if (oneInput)
        {
            return true;
        }
        return false;
    }    
    public bool SquadMember2Selected()
    {
        if (twoInput)
        {
            return true;
        }
        return false;
    }    
    public bool SquadMember3Selected()
    {
        if (threeInput)
        {
            return true;
        }
        return false;
    }
}
