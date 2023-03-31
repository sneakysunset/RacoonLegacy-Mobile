using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class ReloadScene : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI iterationNumber;
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }


    private void Update()
    {
        int timer = Mathf.RoundToInt(player.levelTimer);
        timerText.text = timer.ToString();
        iterationNumber.text = "Loop : " + player.iterationNumber.ToString();
    }

    public void ReloadScener()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
