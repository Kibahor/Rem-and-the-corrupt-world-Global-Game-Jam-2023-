using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameEngine : MonoBehaviour, IPauser
{
    private List<IPausable> pausables = new List<IPausable>();
    private bool paused = false;

    public Camera mainCamera;

	public GameObject canvas = null;
	public GameObject player;
	private TextMeshProUGUI[] texts = null;// canvas.GetComponentsInChildren<TextMeshProUGUI>();
	private int refreshrate = 0;
	private int refreshsum = 0;
	private const int TEXTOFFSET = 1;

    public static GameEngine Instance { get; private set; }

    private bool cameraFollowPlayer = false;
    private Vector3 cameraPos;
    bool haveWon = false;
    string nextScene;

    int i = 0;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (mainCamera)
            cameraPos = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
        cameraFollowPlayer = true;
        Instance = this;
        if(canvas!=null)
		    texts = canvas.GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void WinLevel(string scene)
    {
        cameraFollowPlayer = false;
        haveWon = true;
        nextScene = scene;

        if (player != null)
            player.GetComponent<PlayerControler>().winLevel();
    }

    public void AddPausable(IPausable pausable)
    {
        pausables.Add(pausable);
    }

    public void Pause()
    {
        paused = true;
        pausables.ForEach(pausable => pausable.OnPause());
    }

    public void UnPause()
    {
        paused = false;
        pausables.ForEach(pausable => pausable.OnUnPause());
    }

	public void checkDisplayText()
	{
		foreach (TextMeshProUGUI text in texts)
		{
			text.color = new Color(text.color.r, text.color.g, text.color.b, (float)0.9 - ((float)0.2* (float)Math.Abs(text.transform.position.x - player.transform.position.x)));
		}
	}
    IEnumerator WaitAndChangeScene()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(5);
        SceneManager.LoadSceneAsync(nextScene);
    }

    IEnumerator CameraPan()
    {
        while (i < 5)
        {
            mainCamera.orthographicSize += 0.3f;
            i++;
            // wait for seconds
            yield return new WaitForSeconds(1f);
        }
    }

    // Update is called once per frame
    void Update()
	{
		// Check pause
		if (Input.GetButtonDown("Cancel"))
        {
            if (paused)
                UnPause();
            else
                Pause();
        }
		if (canvas != null)
		{
			if (refreshsum++ >= refreshrate)
			{
				refreshsum = 0;			
				checkDisplayText();
			}
		}
		else
		{
			Debug.Log("Canvas NULL : Corriger vite");
		}

        if (cameraFollowPlayer)
        {
            mainCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y+1.5f, cameraPos.z);
        }else if (haveWon)
        {
            StartCoroutine(CameraPan());
            StartCoroutine(WaitAndChangeScene());
        }
    }
}
