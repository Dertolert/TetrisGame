using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static int height = 20, width = 10;
    public static Transform[,] grid;

    public int score;
    public int highScore;
    public Text scoreText;
    public Text highScoreText;

    public int difficultyPoint;
    public float difficulty = 1f;

    [SerializeField] GameObject imageGameOver;
    public bool isGameOver, pause;

    public Spawner spawner;

    private void Start()
    {
        grid = new Transform[width, height];
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "" + highScore.ToString();
    }

    private void Update()
    {
        scoreText.text = score.ToString();

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            highScoreText.text = "" + highScore.ToString();
        }


    }

    public bool InsideGrid(Vector2 posPiece)
    {
        return((int)posPiece.x >= 0 && (int)posPiece.x < width && (int)posPiece.y >= 0);
    }

    public void UpdateGrid(Pieces piece)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(grid[x,y] != null)
                {
                    if(grid[x,y].parent == piece.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform child in piece.transform)
        {         
            Vector2 roundPieces = new Vector2(Mathf.Round(child.position.x), Mathf.Round(child.position.y));

            if (roundPieces.y < height)
            {
                grid[(int)roundPieces.x, (int)roundPieces.y] = child;
            }
        }


    }

    public Transform positionObjectGrid(Vector2 posPieces)
    {
        if (posPieces.y > height - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)posPieces.x, (int)posPieces.y];
        }
    }

    public bool LineFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }

    public void Delobject(int y)
    {
        for (int x = 0; x < width; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveLineDown(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllLinesDown(int y)
    {
        for (int i = y; i < height; i++)
        {
            MoveLineDown(i);
        }
    }

    public void RemoveLine()
    {
        for (int y = 0; y < height; y++)
        {
            if (LineFull(y))
            {
                Delobject(y);
                MoveAllLinesDown(y + 1);
                y--;
                score += 100;
                difficultyPoint += 100;
            }
        }
    }

    public bool UpGrid(Pieces piece)
    {
        for (int x = 0; x < width;  x++)
        {
            foreach (Transform child in piece.transform)
            {
                Vector2 roundPiece = new Vector2(Mathf.Round(child.position.x), Mathf.Round(child.position.y));

                if (roundPiece.y > height - 1)
                {
                    return true;
                }
            }
        }
        return false;    
    }

    public void GameOver()
    {
        imageGameOver.SetActive(true);
        isGameOver = true;
        spawner.enabled = false;

       

    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
