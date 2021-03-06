using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 1;            // The number of rounds a single player has to win to win the game.
    public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
    public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
    public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
    public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
    public GameObject m_TankPrefab;             // Reference to the prefab the players will control.
    public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks.
    public Color m_RedColor;
    public Color m_BlueColor;
    public Transform m_RedSpawn;
    public Transform m_BlueSpawn;
    public GameObject m_Flag;
    public int m_NumCaptures;
    public AudioSource m_Announcer;
    public Text m_ScoreRed;
    public Text m_ScoreBlue;

    public AudioClip m_BlueWinsRound;
    public AudioClip m_RedWinsRound;
    public AudioClip m_BlueWinsMatch;
    public AudioClip m_RedWinsMatch;
    public AudioClip m_FlawlessWin;
    public AudioClip m_Play;

    
    public GameObject m_FlagDownRed; 
    public GameObject m_FlagDownBlue;  
    public GameObject m_FlagAtBaseRed; 
    public GameObject m_FlagAtBaseBlue;  
    public GameObject m_FlagTakenRed; 
    public GameObject m_FlagTakenBlue; 


    public static int blueCaptures;
    public static int redCaptures;
    private GameObject redFlag; 
    private GameObject blueFlag;
    private int m_RoundNumber;                  // Which round the game is currently on.
    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
    private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
    private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.


    private void Start()
    {
        // Create the delays so they only have to be made once.
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SpawnFlags();
        SetCameraTargets();

        // Once the tanks have been created and the camera is using them as targets, start the game.
        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        // For all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... create them, set their player number and references needed for control.
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }

    private void SpawnFlags() {
        redFlag = Instantiate(m_Flag);
        blueFlag = Instantiate(m_Flag);

        redFlag.transform.position = m_RedSpawn.position;
        blueFlag.transform.position = m_BlueSpawn.position;

        redFlag.transform.Find("WholeFlag").transform.Find("Flag").GetComponent<Renderer>().material.SetColor("_Color", m_RedColor);
        blueFlag.transform.Find("WholeFlag").transform.Find("Flag").GetComponent<Renderer>().material.SetColor("_Color", m_BlueColor);

        redFlag.name = "Red Flag";
        blueFlag.name = "Blue Flag";

        redFlag.tag = "Red";
        blueFlag.tag = "Blue";
    }


    private void SetCameraTargets()
    {
        // Create a collection of transforms the same size as the number of tanks.
        // + 2 for the two flags
        Transform[] targets = new Transform[m_Tanks.Length + 2];

        // For each of these transforms...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... set it to the appropriate tank transform.
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        targets[m_Tanks.Length] = redFlag.transform;
        targets[m_Tanks.Length + 1] = blueFlag.transform; 

        // These are the targets the camera should follow.
        m_CameraControl.m_Targets = targets;
    }


    // This is called from start and will run each phase of the game one after another.
    private IEnumerator GameLoop()
    {
        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundStarting());

        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
        yield return StartCoroutine(RoundEnding());

        // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
        if (m_GameWinner != null)
        {
            // If there is a game winner, restart the level.
            Application.LoadLevel(Application.loadedLevel);
        }
        else
        {
            // If there isn't a winner yet, restart this coroutine so the loop continues.
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        // As soon as the round starts reset the tanks and make sure they can't move.
        ResetAllTanks();
        DisableTankControl();

        redCaptures = blueCaptures = 0;
        m_ScoreBlue.text = m_ScoreRed.text = "0";

        // Snap the camera's zoom and position to something appropriate for the reset tanks.
        m_CameraControl.SetStartPositionAndSize();

        // Increment the round number and display text showing the players what round it is.
        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        // As soon as the round begins playing let the players control the tanks.
        EnableTankControl();
        m_Announcer.clip = m_Play;
        m_Announcer.Play();

        // Clear the text from the screen.
        m_MessageText.text = string.Empty;

        // While there is not one tank left...
        while (!CaptureLimitReached())
        {
            // ... return on the next frame.
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        // Stop tanks from moving.
        //DisableTankControl();

        // Clear the winner from the previous round.
        m_RoundWinner = null;

        // See if there is a winner now the round is over.
        m_RoundWinner = GetRoundWinner();

        // If there is a winner, increment their score.
        if (m_RoundWinner != null) {
            m_RoundWinner.m_Wins++;
        }

        // Now the winner's score has been incremented, see if someone has one the game.
        m_GameWinner = GetGameWinner();

        // Get a message based on the scores and whether or not there is a game winner and display it.
        string message = EndMessage();
        m_MessageText.text = message;

        
        yield return new WaitForSeconds(m_Announcer.clip.length);
        
        if (blueCaptures == 0 || redCaptures == 0) {
            m_Announcer.clip = m_FlawlessWin;
            m_Announcer.Play();
        }

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;
    }

    private bool CaptureLimitReached() {
        return blueCaptures >= m_NumCaptures || redCaptures >= m_NumCaptures;
    }


    // This function is to find out if there is a winner of the round.
    // This function is called with the assumption that 1 or fewer tanks are currently active.
    private TankManager GetRoundWinner()
    {
        // Go through all the tanks...
        if (blueCaptures > redCaptures) {
            for (int i = 0; i < m_Tanks.Length; i++) {
                if (m_Tanks[i].m_Instance.gameObject.tag == "Blue") {
                    return m_Tanks[i];
                }
            }
        } else {
            for (int i = 0; i < m_Tanks.Length; i++) {
                if (m_Tanks[i].m_Instance.gameObject.tag == "Red") {
                    return m_Tanks[i];
                }
            }
        }

        // If none of the tanks are active it is a draw so return null.
        return null;
    }


    // This function is to find out if there is a winner of the game.
    private TankManager GetGameWinner()
    {
        // Go through all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... and if one of them has enough rounds to win the game, return it.
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        // If no tanks have enough rounds to win, return null.
        return null;
    }


    // Returns a string message to display at the end of each round.
    private string EndMessage()
    {
        // By default when a round ends there are no winners so the default end message is a draw.
        string message = "DRAW!";
        m_Announcer.clip = m_Play;

        // If there is a winner then change the message to reflect that.
        if (m_RoundWinner != null) {
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";
            /// Wins round clip
            if (m_RoundWinner.m_Instance.tag == "Red") {
                m_Announcer.clip = m_RedWinsRound;
            } else {
                m_Announcer.clip = m_BlueWinsRound;
            }
        }

        // Add some line breaks after the initial message.
        message += "\n\n\n";

        // Go through all the tanks and add each of their scores to the message.
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        // If there is a game winner, change the entire message to reflect that.
        if (m_GameWinner != null) {
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";
            /// Wins game clip
            if (m_RoundWinner.m_Instance.tag == "Red") {
                m_Announcer.clip = m_RedWinsMatch;
            } else {
                m_Announcer.clip = m_BlueWinsMatch;
            }
        }
        m_Announcer.Play();

        return message;
    }


    // This function is used to turn all the tanks back on and reset their positions and properties.
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    public void PlayAudio(AudioClip ac) {
        m_Announcer.clip = ac;
        m_Announcer.Play();
    }

    
    public void FlagDrop(string team) {
        if (team == "Red") {
            m_FlagAtBaseRed.SetActive(false);
            m_FlagDownRed.SetActive(true);
            m_FlagTakenRed.SetActive(false);
        } else {
            m_FlagAtBaseBlue.SetActive(false);
            m_FlagDownBlue.SetActive(true);
            m_FlagTakenBlue.SetActive(false);
        }
    }

}