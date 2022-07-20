using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public int countdownTime;
    public TextMeshProUGUI countDownText;
    public GameObject playerBlue;
    public GameObject playerYellow;
    public GameObject playerPink;

    public NavMeshAgent blueAgent;
    public NavMeshAgent yellowAgent;

    [Header("Tap To Start Panel")]
    public RectTransform mainMenuPanel;
    public Button tapToPlayButton;

    [Header("WIN Panel")]
    public RectTransform playerWinPanel;
    public Button restartAfterWinButton;
    public Text winnerText;

    [Header("Pause Panel")]
    [SerializeField]
    private Button resumeButton;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private Button menuButton;

    private bool isPaused;
    public bool isTouch = false;
    private bool taptoTouch= true;


    private string[] playerNames;

    public static UIManager Instance;

    void Start()
    {
        Instance = this;
        blueAgent.speed = 0;
        yellowAgent.speed = 0;

        tapToPlayButton.onClick.AddListener(TapToPlayMenu);

        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(Restart);
        menuButton.onClick.AddListener(LoadMenuScene);
        restartAfterWinButton.onClick.AddListener(RestartAfterWinCondition);

        playerYellow.gameObject.GetComponent<Animator>().Play("Idle 0");
        playerBlue.gameObject.GetComponent<Animator>().Play("Idle 0");
    }

    IEnumerator CountDownToStart()
    {     
         //TapToPlayMenu();
        while (countdownTime > 0)
        {
            countDownText.text = countdownTime.ToString();
            blueAgent.speed = 0;
            yellowAgent.speed = 0;
            playerPink.GetComponent<CharacterMovement>().enabled = false;
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }    
        countDownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        countDownText.gameObject.SetActive(false);
        playerYellow.gameObject.GetComponent<Animator>().Play("Running");
        playerBlue.gameObject.GetComponent<Animator>().Play("Running");

        blueAgent.speed = 3.5f;
        yellowAgent.speed = 3.5f;

        playerBlue.gameObject.GetComponent<AIPlayerMovement>().enabled = true;
        playerYellow.gameObject.GetComponent<AIPlayerMovement>().enabled = true;
        playerPink.GetComponent<CharacterMovement>().enabled = true;
    }

    public void TurnOnWinPanel()
    {
        if (BallCollector.bluewWinner)
        {
            winnerText.text= playerBlue.GetComponent<AIPlayerMovement>().GetPlayerName()+ " Player Wins";
            playerWinPanel.DOAnchorPos(new Vector2(0, 0), 1f).OnComplete(() =>
             {
                 Time.timeScale = 0;
             });
        }
        if (BallCollector.redWinner)
        {
            playerWinPanel.DOAnchorPos(new Vector2(0, 0), 1f).OnComplete(() =>
            {
                Time.timeScale = 0;
            });
            winnerText.text = "Player Red Wins!";
        }
        if (BallCollector.yellowWinner)
        {
            winnerText.text = winnerText.text = playerYellow.GetComponent<AIPlayerMovement>().GetPlayerName()+" Player Wins";
            playerWinPanel.DOAnchorPos(new Vector2(0, 0), 1f).OnComplete(() =>
            {
                Time.timeScale = 0;
            });
        }
        if (BallCollector.playerWinner)
        {
            winnerText.text = winnerText.text = playerPink.GetComponent<CharacterMovement>().GetPlayerName() + " Player Wins";
            playerWinPanel.DOAnchorPos(new Vector2(0, 0), 1f).OnComplete(() =>
            {
                Time.timeScale = 0;
            });
        }
    }

    //pause game
    private void PauseGame()
    {
        if (!isPaused)
        {
            Time.timeScale = 0;
            isPaused = true;
            //pausePanel.SetActive(true);
        }
    }

    //resume game
    private void ResumeGame()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
            //pausePanel.SetActive(false);
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    private void TapToPlayMenu()
    {
        mainMenuPanel.DOAnchorPosY(3000,2);
        StartCoroutine(CountDownToStart());
        isTouch = true;      
    }

    //load menu/lobby scene
    private void LoadMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
        //pausePanel.SetActive(false);
    }

    private void RestartAfterWinCondition()
    {
        //playerWinPanel.transform.localPosition = new Vector3(0f, -2895.101f, 0f);
        playerWinPanel.DOAnchorPos(new Vector2(0,3000), 2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }
}
