using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;
    /// <summary>
    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    private bool isMovingDown = false;
    private float moveDelay = 0.13f;  // Интервал задержки между перемещениями
    private float moveTimer = 0f;
    /// </summary>
   

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        if (this.cells == null) {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++) {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }
    
    private void Update()
    {
        this.board.Clear(this);

        this.lockTime += Time.deltaTime;

        // повороты фигур налево, направо
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Rotate(1);
        }
        // Обработка нажатий клавиш
        if (Input.GetKeyDown(KeyCode.A))
        {
            isMovingLeft = true;
            Move(Vector2Int.left);
            moveTimer = 0f;  // Сбрасываем таймер при нажатии клавиши
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            isMovingRight = true;
            Move(Vector2Int.right);
            moveTimer = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            isMovingDown = true;
            Move(Vector2Int.down);
            moveTimer = 0f;
        }

        // Обработка удержания клавиш
        if (isMovingLeft)
        {
            moveTimer += Time.deltaTime;
            if (moveTimer >= moveDelay)
            {
                Move(Vector2Int.left);
                moveTimer = 0f;
            }
        }
        else if (isMovingRight)
        {
            moveTimer += Time.deltaTime;
            if (moveTimer >= moveDelay)
            {
                Move(Vector2Int.right);
                moveTimer = 0f;
            }
        }
        else if (isMovingDown)
        {
            moveTimer += Time.deltaTime;
            if (moveTimer >= moveDelay)
            {
                Move(Vector2Int.down);
                moveTimer = 0f;
            }
        }

        // Обработка отпускания клавиш
        if (Input.GetKeyUp(KeyCode.A))
        {
            isMovingLeft = false;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            isMovingRight = false;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            isMovingDown = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if (Time.time >= this.stepTime) {
            Step();
        }

        this.board.Set(this);
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;

        Move(Vector2Int.down);

        if (this.lockTime >= this.lockDelay) {
            Lock();
        }


    }

    // функция для моментального падения фигуры
    private void HardDrop()
    {
        while (Move(Vector2Int.down)) {
            continue;
        }

        Lock();
    }

    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
        this.board.AddScore(10);
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition);

        if (valid) {
            this.position = newPosition;
            this.lockTime = 0f;
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        int originalRotation = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(this.rotationIndex, direction))
        {
            this.rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
   
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];

            int x, y;

            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

            if (Move(translation)) {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0) {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min) {
            return max - (min - input) % (max - min);
        } else {
            return min + (input - min) % (max - min);
        }
    }

}
