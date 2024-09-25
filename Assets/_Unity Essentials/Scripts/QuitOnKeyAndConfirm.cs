using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class QuitOnKeyAndConfirm : MonoBehaviour
{
    [SerializeField] private KeyCode quitKey = KeyCode.Q;
    [SerializeField] private TextMeshProUGUI quitConfirmTextObject;
    [SerializeField] private ToggleActiveOnKey objectToggler;
    [SerializeField] private float secondsToConfirm = 3f;
    [SerializeField] private int sceneIndexToQuitTo = 0;

    bool awatingConfirm = false;
    float awaitingConfirmTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (quitConfirmTextObject == null)
        {
            Debug.LogError("QuitOnKeyAndConfirm script requires a TextMeshProUGUI component.");
            return;
        }

        if (objectToggler == null) GetComponent<ToggleActiveOnKey>();
        if (objectToggler != null) objectToggler.SetListenToKey(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (awatingConfirm)
        {
            UpdateConfirmTimer();
        }

        if (Input.GetKeyDown(quitKey))
        {
            StartConfirmTimer();
        }

    }

    private void StartConfirmTimer()
    {
        awatingConfirm = true;
        awaitingConfirmTimer = secondsToConfirm;
        objectToggler.ToggleActive();
    }

    private void UpdateConfirmTimer()
    {
        awaitingConfirmTimer -= Time.deltaTime;
        var timerTextValue = Mathf.Round(awaitingConfirmTimer);
        quitConfirmTextObject.text = $"Press [Q] again in {timerTextValue} seconds";

        if (awaitingConfirmTimer <= 0)
        {
            StopAndResetConfirmTimer();
            return;
        }

        if (Input.GetKeyDown(quitKey))
        {
            SceneManager.LoadScene(sceneIndexToQuitTo);
            return;
        }
    }

    private void StopAndResetConfirmTimer()
    {
        objectToggler.ToggleActive();
        awatingConfirm = false;
    }
}
