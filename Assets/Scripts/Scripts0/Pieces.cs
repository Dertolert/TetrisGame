using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour
{
    [SerializeField] bool canRotate, canRotate360;
    GameController gC;
    Spawner spawner;
    [SerializeField] float fallTime, timer, maxTime;
    


    private void Start()
    {
        gC = FindObjectOfType<GameController>();
        spawner = FindObjectOfType<Spawner>();
    }

    void Update()
    {
        if (gC.difficultyPoint > 500)
        {
            gC.difficultyPoint -= 500;
            gC.difficulty += 0.5f;
        }

        MovePiece();
        MoveAutomatic();
    }

    void MovePiece()
    {
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S))
            timer = maxTime;

        // перемещение вправо
        if (Input.GetKey(KeyCode.D))
        {
            PieceSpeed(Vector3.right);
            UpdatePieces(Vector3.left, true, false, true, false, false);
        }
        // перемещение влево
        if (Input.GetKey(KeyCode.A))
        {
            PieceSpeed(Vector3.left);
            UpdatePieces(Vector3.right, true, false, true, false, false);
        }
        // перемещение вниз
        if (Input.GetKey(KeyCode.S))
        {
            PieceSpeed(Vector3.down);
            UpdatePieces(Vector3.up, true, false, false, true, true);
        }
        //поворот налево на 90
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckRotation();
        }

        

    }

    void UpdatePieces(Vector3 direction, bool canPosLimitPiece, bool canRotationLimitPiece, bool statePieces, bool canSpawn, bool canRemoveLine)
    {
        if (PieceInsideGrid())
        {
            gC.UpdateGrid(this);
        }
        else
        {
            if (canPosLimitPiece)
            transform.position += direction;
            if(canRotationLimitPiece)
                transform.Rotate(direction);

            enabled = statePieces;

            if (canSpawn)
                spawner.NextPiece();
            if (canRemoveLine)
            {
                gC.RemoveLine();

                if (gC.UpGrid(this))
                {
                    gC.GameOver();
                }

                gC.score += 10;
                gC.difficultyPoint += 10;
            }
        }
    }

    void CheckRotation()
    {
        if (canRotate)
        {
            if (!canRotate360)
            {
                if (transform.rotation.z < 0)
                {
                    transform.Rotate(Vector3.forward * 90);
                    UpdatePieces(Vector3.forward * -90, false, true, true, false, false);
                }
                else
                {
                    transform.Rotate(Vector3.forward * -90);
                    UpdatePieces(Vector3.forward * 90, false, true, true, false, false);
                }
            }
            else
            {
                transform.Rotate(Vector3.forward * -90);
                UpdatePieces(Vector3.forward * 90, false, true, true, false, false);
            }
        }
    }

    bool PieceInsideGrid()
    {
        foreach (Transform child in transform)
        {
            Vector2 roundPieces = new Vector2(Mathf.Round(child.position.x), Mathf.Round(child.position.y));

            if(!gC.InsideGrid(roundPieces))
            {
                return false;
            }

            if (gC.positionObjectGrid(roundPieces) != null && gC.positionObjectGrid(roundPieces).parent != transform)
            {
                return false;
            }

        }
        return true;
    }

    void MoveAutomatic()
    {

        fallTime += Time.deltaTime;

        if(fallTime >= (1/ gC.difficulty) && !Input.GetKeyDown(KeyCode.S))
        {
            transform.position += Vector3.down;
            UpdatePieces(Vector3.up, true, false, false, true, true);
            fallTime = 0;
        }
    }

    void PieceSpeed(Vector3 direction)
    {
        timer += Time.deltaTime;

        if (timer > maxTime)
        {
            transform.position += direction;
            timer = 0;
        }
    }

}
