 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using DG;
using DG.Tweening;

public class RadialMenuController : MonoBehaviour
{
    public Camera cam;
    public GameObject menu;
    public GameObject squad1, squad2, squad3;
    public followPlayer squad1AI, squad2AI, squad3AI;
    public Vector2 mouseInput;
    public TextMeshProUGUI[] options;
    public GameObject[] optionsImages;
    public Color normalColor;
    public Color highlightColor;

    public InputManager inputManager;
    public int selectedOption;

    public PlayerCommands playerCommands;

    public TextMeshProUGUI squad1StatusText;
    public TextMeshProUGUI squad2StatusText;
    public TextMeshProUGUI squad3StatusText;

    public GameObject selectedSquadIndicator;
    public NotificationManager notification;

    private GameObject[] collectables;
    private GameObject findButton;
    private Vector3 originalScale;

    private bool coroutineAllowed = true;

    public bool menuOpen;

    private void Start()
    {
        menu.SetActive(false);
        squad1AI = squad1.GetComponent<followPlayer>();
        squad2AI = squad2.GetComponent<followPlayer>();
        squad3AI = squad3.GetComponent<followPlayer>();
        selectedSquadIndicator = GameObject.Find("SelectedSquadIndicator");
        selectedSquadIndicator.SetActive(false);
        notification = GameObject.Find("Game Manager").GetComponent<NotificationManager>();

        
        coroutineAllowed = true;
        
    }

    void Update()
    {
        if (!menu.activeInHierarchy && inputManager.RightClickPressed())
        {
            //Debug.Log("Menu not active and right click pressed");
            inputManager.RightMouseClicked();
            menu.SetActive(true); 
        }
        else if (menu.activeInHierarchy)
        {
            selectedSquadIndicator.SetActive(true);
            var mousePos = Mouse.current.position.ReadValue();
            mousePos.x -= (Screen.width / 2f);
            mousePos.y -= (Screen.height / 2f);
            mousePos.Normalize();

            if (mousePos != Vector2.zero)
            {
                float angle = Mathf.Atan2(mousePos.y, -mousePos.x) / Mathf.PI;
                angle *= 180;
                angle += 90f; // Used to set starter position of 0
                if (angle < 0)
                {
                    angle += 360;
                }
                //Debug.Log(angle);

                for (int i = 0; i < options.Length; i++)
                {
                    if (angle > i * 90 && angle < (i + 1) * 90) // Angle per option
                    {
                        options[i].color = highlightColor;
                        optionsImages[i].transform.localScale = Vector3.one * 20f;
                        selectedOption = i;
                    }
                    else
                    {
                        optionsImages[i].transform.localScale = Vector3.one * 10f;
                        options[i].color = normalColor;
                    }

                }
            }

            if (inputManager.GetLeftMouseClicked())
            {
                switch (selectedOption)
                {
                    case 0: // Find
                        playerCommands.SetToFind();
                        //Debug.Log("Find Clicked");
                        break;

                    case 1: // Recall
                        playerCommands.SetToRecall();
                        //Debug.Log("Recall Clicked");
                        break;

                    case 2: // Go Here
                        playerCommands.ReceiveInput();
                        //Debug.Log("Go Here Clicked");
                        break;

                    case 3: // Attack
                        playerCommands.SetToEnemyFind();
                        //Debug.Log("Hunt Clicked");
                        break;
                }
                selectedSquadIndicator.SetActive(false);
                menu.SetActive(false);
            }

            if (inputManager.RightClickPressed())
            {
                //Debug.Log("Menu active and right click pressed");
                selectedSquadIndicator.SetActive(false);
                menu.SetActive(false);
                inputManager.RightMouseClicked();
            }

            collectables = GameObject.FindGameObjectsWithTag("Collectable");
            if (collectables == null)
            {
                StopCoroutine(StartPulsing(findButton));
                notification.CallSend("No collectables remaining", 5);
            }
            else if (collectables != null)
            {
                findButton = GameObject.Find("FindButton");
                if (coroutineAllowed)
                {
                    StartCoroutine(StartPulsing(findButton));
                }
            }

        }
        SetText();
    }

    public void SetText()
    {
        if (squad1AI.GetHunt())
        {
            squad1StatusText.SetText("Hunting");
        }
        if (squad2AI.GetHunt())
        {
            squad2StatusText.SetText("Hunting");
        }
        if (squad3AI.GetHunt())
        {
            squad3StatusText.SetText("Hunting");
        }

        if (squad1AI.GetFind())
        {
            squad1StatusText.SetText("Finding");
        }
        if (squad2AI.GetFind())
        {
            squad2StatusText.SetText("Finding");
        }
        if (squad3AI.GetFind())
        {
            squad3StatusText.SetText("Finding");
        }

        if (squad1AI.GetGo())
        {
            squad1StatusText.SetText("Going");
        }
        if (squad2AI.GetGo())
        {
            squad2StatusText.SetText("Going");
        }
        if (squad3AI.GetGo())
        {
            squad3StatusText.SetText("Going");
        }

        if (squad1AI.GetRecall())
        {
            squad1StatusText.SetText("Following");
        }
        if (squad2AI.GetRecall())
        {
            squad2StatusText.SetText("Following");
        }
        if (squad3AI.GetRecall())
        {
            squad3StatusText.SetText("Following");
        }
    }

    private  IEnumerator StartPulsing(GameObject button)
    {

        coroutineAllowed = false;

        for (float i = 0f; i <= 5f; i += 0.01f)
        {
            button.transform.localScale = new Vector3(
                (Mathf.Lerp(button.transform.localScale.x, button.transform.localScale.x + 2.5f, Mathf.SmoothStep(0f, 5f, i))),
                (Mathf.Lerp(button.transform.localScale.y, button.transform.localScale.y + 2.5f, Mathf.SmoothStep(0f, 5f, i))),
                (Mathf.Lerp(button.transform.localScale.z, button.transform.localScale.z + 2.5f, Mathf.SmoothStep(0f, 5f, i)))
            );
            yield return new WaitForSeconds(0.15f);
            Debug.Log("Scaled Up");
        }

        for (float i = 0f; i <= 5f; i += 0.01f)
        {
            button.transform.localScale = new Vector3(
                (Mathf.Lerp(button.transform.localScale.x, button.transform.localScale.x - 2.5f, Mathf.SmoothStep(0f, 5f, i))),
                (Mathf.Lerp(button.transform.localScale.y, button.transform.localScale.y - 2.5f, Mathf.SmoothStep(0f, 5f, i))),
                (Mathf.Lerp(button.transform.localScale.z, button.transform.localScale.z - 2.5f, Mathf.SmoothStep(0f, 5f, i)))
            );
            yield return new WaitForSeconds(0.15f);
            Debug.Log("Scaled Down");
        }

        coroutineAllowed = true;
    }

    public void Pulse(GameObject button)
    {
        //Vector3 originalScale = button.transform.localScale;
        //button.transform.localScale = Mathf.PingPong(Time.time, ratio) * Vector3.one;

        /*        DOTween.Sequence()
                    .Append(button.transform.DOScale(new Vector2(10f, 10f), 0.3f).SetDelay(0.3f))
                    .Append(button.transform.DOScale(Vector2.one, 0.3f))
                    .SetLoops(-1, LoopType.Restart);*/

        /*        originalScale = button.transform.localScale;
                button.transform.DOLocalRotate(new Vector3(0, 0, 360), rotationSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental).SetRelative();
                var sequence = DOTween.Sequence()
                    .Append(button.transform.DOScale(new Vector3(originalScale.x + 0.5f, originalScale.y + 0.5f, originalScale.z + 0.5f), scaleSpeed))
                    .Append(button.transform.DOScale(originalScale, scaleSpeed));

                sequence.SetLoops(-1, LoopType.Restart);*/

    }
}
