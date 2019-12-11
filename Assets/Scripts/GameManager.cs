using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   private bool _gameOver;

    private void Update()
    {
        if (_gameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }
    }
   public void GameOver()
   {
       _gameOver = true;
   }
}
