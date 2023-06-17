using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Transform[] pieces;
    public int nextPiece;
    public List<GameObject> samplePiece;


    private void Start()
    {
        nextPiece = Random.Range(0, pieces.Length);
        NextPiece();
    }

    public void NextPiece()
    {
        Instantiate(pieces[nextPiece], transform.position, Quaternion.identity);

        nextPiece = Random.Range(0, pieces.Length);

        for (int i = 0; i < samplePiece.Count; i++)
        {
            samplePiece[i].SetActive(false);
            samplePiece[nextPiece].SetActive(true);
        }
    }
}
