using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuGameManager : MonoBehaviour
{
    Button startButton;

    Text normalModeText;
    Text hardcoreModeText;
    Text realismModeText;
    Color darkRed = new Color32(160, 0, 0, 255);

    public string scene = "_Normal_Mode";

    void Awake()
    {
        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        normalModeText = GameObject.Find("NormalMode").GetComponent<Button>().GetComponentInChildren<Text>();
        hardcoreModeText = GameObject.Find("Hardcore").GetComponent<Button>().GetComponentInChildren<Text>();
        realismModeText = GameObject.Find("Realism").GetComponent<Button>().GetComponentInChildren<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FlashButton(startButton));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator FlashButton(Button button)
    {
        Text buttonText = button.GetComponentInChildren<Text>();

        while (true)
        {
            buttonText.enabled = false;
            yield return new WaitForSeconds(0.5f);
            buttonText.enabled = true;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // sets the scene name to the mode selected
    public void SetMode(string mode)
    {
        if (mode == "_Normal_Mode")
        {
            normalModeText.color = Color.gray;
            hardcoreModeText.color = Color.red;
            realismModeText.color = Color.white;
        }
        else if (mode == "_Hardcore_Mode")
        {
            normalModeText.color = Color.white;
            hardcoreModeText.color = darkRed;
            realismModeText.color = Color.white;
        }
        else if (mode == "_Realism_Mode")
        {
            normalModeText.color = Color.white;
            hardcoreModeText.color = Color.red;
            realismModeText.color = Color.gray;
        }

        scene = mode;
    }


    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}
