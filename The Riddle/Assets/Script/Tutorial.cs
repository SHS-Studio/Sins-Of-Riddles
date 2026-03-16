using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "Gamemode";

    private bool tutorialFinished = false;
    [Header("UI")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;

    [Header("Skip Button")]
    public GameObject StoryskipButton;
    public GameObject TutorialskipButton;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;

    [Header("Story Slides")]
    [TextArea(3, 5)]
    public string[] storySlides;

    [Header("Tutorial Steps")]
    [TextArea(3, 5)]
    public string[] tutorialSteps;

    private int slideIndex = 0;
    private int stepIndex = 0;

    private bool storyMode = true;
    private bool typing = false;

    void Start()
    {
        tutorialPanel.SetActive(true);

        if (StoryskipButton != null)
            StoryskipButton.SetActive(true);

        StartCoroutine(TypeText(storySlides[slideIndex]));
    }

    void Update()
    {
        if (storyMode)
            HandleStory();

        else
            HandleTutorial();

        if (tutorialFinished && Input.GetKeyDown(KeyCode.Return))
        {
            LoadGameScene();
        }
    }

    void HandleStory()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !typing)
        {
            slideIndex++;

            if (slideIndex >= storySlides.Length)
            {
                storyMode = false;
                stepIndex = 0;

                if (StoryskipButton != null)
                    StoryskipButton.SetActive(false);
                    TutorialskipButton.SetActive(true);


                StartCoroutine(TypeText(tutorialSteps[stepIndex]));
                return;
            }

            StartCoroutine(TypeText(storySlides[slideIndex]));
        }
    }

    void HandleTutorial()
    {
        if (stepIndex == 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                NextStep();
        }

        else if (stepIndex == 1)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                NextStep();
        }

        else if (stepIndex == 2)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                NextStep();
        }

        else if (stepIndex == 3)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                NextStep();
        }

        else if (stepIndex == 4)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                NextStep();
        }

        else if (stepIndex == 5)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                NextStep();
        }
        else if (stepIndex == 6)
       
        {
            if (Input.GetKeyDown(KeyCode.A)|| Input.GetKeyDown(KeyCode.D))
                NextStep();
        }
    }

    void NextStep()
    {
        stepIndex++;

        if (stepIndex >= tutorialSteps.Length)
        {
            tutorialText.text = "Tutorial Complete\n\nPress ENTER to start your journey.";
            tutorialFinished = true;
            return;
        }

        StartCoroutine(TypeText(tutorialSteps[stepIndex]));
    }

    IEnumerator TypeText(string text)
    {
        typing = true;

        tutorialText.text = "";

        foreach (char c in text)
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        typing = false;
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void SkipStory()
    {
        StopAllCoroutines();

        storyMode = false;
        slideIndex = storySlides.Length;

        stepIndex = 0;

        if (StoryskipButton != null)
            StoryskipButton.SetActive(false);
            TutorialskipButton.SetActive(true);


        StartCoroutine(TypeText(tutorialSteps[stepIndex]));
    }

    public void SkipTutorial()
    {
        SceneManager.LoadScene(gameSceneName);
    }

}
