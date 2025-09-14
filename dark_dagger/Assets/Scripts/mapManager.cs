using System.Collections.Generic;
using UnityEngine;

public class mapManager : MonoBehaviour
{

    System.Random rand = new System.Random();
    [Header("+ Pieces")]
    [SerializeField] private GameObject[] crossPieces;
    [Header("L Pieces")]
    [SerializeField] private GameObject[] lPieces;
    [Header("T Pieces")]
    [SerializeField] private GameObject[] tPieces;
    [Header("I Pieces")]
    [SerializeField] private GameObject[] linePieces;
    [Header("End Pieces")]
    [SerializeField] private GameObject[] endPieces;
    [Header("Start Pieces")]
    [SerializeField] private GameObject[] startPieces;
    [Header("Win Pieces")]
    [SerializeField] private GameObject[] winPieces;
    [Header("Boss Levels")]
    [SerializeField] private GameObject[] bossPieces;

    public int gridSize = 3;
    [SerializeField] private int blockSize = 20;

    private List<List<tile>> currMap;
    private List<GameObject> mapObj = new List<GameObject>();

    public List<Vector3> enemySpawns = new List<Vector3>();
    private enum direction { UP = 0, RIGHT, DOWN, LEFT };

    private class tile
    {
        public string type;
        public List<bool> open; //Up, right, down, left
        public tile(string t, List<bool> o)
        {
            type = t;
            open = o;
        }
    };

    private struct position
    {
        public int row, col;
        public position(int r, int c)
        {
            row = r;
            col = c;
        }
    };

    private direction opposite(direction d)
    {
        return (direction)(((int)d + 2) % 4);
    }

    List<direction> shuffleDirections()
    {
        List<direction> dirs = new List<direction> { direction.UP, direction.RIGHT, direction.DOWN, direction.LEFT };
        System.Random rand = new System.Random();
        int n = dirs.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n+1);
            direction val = dirs[k];
            dirs[k] = dirs[n];
            dirs[n] = val;  
        }
        return dirs;
    }

    private tile getTile(List<bool> open) {
        int count = 0;
        foreach (bool o in open)
            if (o)
                count++;
        if (count == 4)
            return new tile("+", open);
        if(count == 3)
            return new tile("T", open);
        if (count == 2)
        {
            if ((open[(int)direction.UP] && open[(int)direction.DOWN]) || (open[(int)direction.LEFT] && open[(int)direction.RIGHT]))
                return new tile("I", open);
            else
                return new tile("L", open);
        }
        if (count == 1)
            return new tile("C", open);
        return new tile("D", open);
    }


    private bool inBounds(int r, int c, int rows, int cols)
{
    return (r >= 0 && r < rows && c >= 0 && c < cols);
}

private position move(position p, direction d)
{
        switch (d)
        {
            case direction.UP:
                return new position(p.row - 1, p.col);
            case direction.DOWN:
                return new position(p.row + 1, p.col);
            case direction.LEFT:
                return new position(p.row, p.col - 1);
            case direction.RIGHT:
                return new position(p.row, p.col + 1);
        }
        return p;
    }

    private void carve(List<List<tile>> map, position cur, int rows, int cols, List<List<bool>> visited)
    {
        visited[cur.row][cur.col] = true;
        foreach (direction d in shuffleDirections())
        {
            position next = move(cur, d);
            if (next.col == 0)
                continue;
            if (inBounds(next.row, next.col, rows, cols) && !visited[next.row][next.col])
            {
                map[cur.row][cur.col].open[(int)d] = true;
                map[next.row][next.col].open[(int)opposite(d)] = true;
                carve(map, next, rows, cols, visited);
            }
        }
    }

    private List<bool> rotateTile(List<bool> opens, int amt)
    {
        List<bool> end = new List<bool>(opens);
        for (int i = 0; i < amt; i++)
        {
            end = new List<bool> { end[3], end[0], end[1], end[2] };
        }
        return end;
    }

    private bool equalTile(List<bool> a, List<bool> b)
    {
        if (a.Count != b.Count)
            return false;
        for (int i = 0; i < a.Count; i++) {
            if (a[i] != b[i])
                return false;
        }
        return true;
    }

    private int getRotAmt(tile t)
    {
        List<bool> opens = new List<bool>();
        switch (t.type)
        {
            case "I": opens = new List<bool> {true, false, true, false}; break;
            case "L": opens = new List<bool> { true, false, false, true }; break;
            case "T": opens = new List<bool> { true, true, false, true }; break;
            case "+": opens = new List<bool> { true, true, true, true }; break;
            case "C": opens = new List<bool> { false, false, true, false }; break;
            case "O": opens = new List<bool> { false, false, true, false }; break;
            case "X": opens = new List<bool> { false, false, true, false }; break;
            default: return 0;
        }
        for(int i = 0; i < 4; i++)
        {
            List<bool> rotated = rotateTile(opens, i);
            if(equalTile(rotated, t.open))
                return i;
        }
        return 0;
    }

    public void generateMap()
    {
        deleteMap();

        enemySpawns.Clear();

        GameObject floorRef = GameObject.FindGameObjectWithTag("FloorReference");
        Vector3 build = Vector3.zero;
        if (floorRef != null)
        {
            Vector3 floorPos = floorRef.transform.position;
            Vector3 floorScale = floorRef.transform.localScale;
            float width = floorScale.x * 10f;
            float depth = floorScale.y * 10f;
            build = new Vector3(floorPos.x - width/2f, -2 + floorPos.y, floorPos.z + depth/2f);
        }

        List<List<tile>> map = new List<List<tile>>();
        List<List<bool>> visited = new List<List<bool>>();

        for (int r = 0; r < gridSize; r++)
        {
            map.Add(new List<tile>());
            visited.Add(new List<bool>());
            for (int c = 0; c < gridSize + 1; c++)
            {
                map[r].Add(new tile("", new List<bool> { false, false, false, false }));
                visited[r].Add(false);
            }
        }
        carve(map, new position(0, 1), gridSize, gridSize + 1, visited);

        int startRow = rand.Next(gridSize);
        map[startRow][0].type = "O";
        map[startRow][0].open = new List<bool> { false, true, false, false };
        map[startRow][1].open[(int)direction.LEFT] = true;

        int endRow;
        int endCol;

        while (true)
        {
            endRow = rand.Next(gridSize);
            endCol = 1 + rand.Next(gridSize - 1);
            if (!(endCol == 1 && endRow == startRow))
            {
                int openings = 0;
                foreach (bool o in map[endRow][endCol].open) if (o) openings++;
                if (openings == 1)
                {
                    map[endRow][endCol].type = "X";
                    break;
                }
            }
        }
        for (int r = 0; r < gridSize; r++)
        {
            for (int c = 0; c < gridSize + 1; c++)
            {
                if (map[r][c].type == "O" || map[r][c].type == "X")
                    continue;
                map[r][c] = getTile(map[r][c].open);
            }
        }

        currMap = map;

        for (int r = 0; r < gridSize; r++)
        {
            for (int c = 0; c < gridSize + 1; c++)
            {
                Vector3 pos = build + new Vector3(c * blockSize, 0, -r * blockSize);
                GameObject prefab = null;

                switch (map[r][c].type)
                {
                    case "+":
                        if (crossPieces.Length > 0)
                            prefab = crossPieces[rand.Next(crossPieces.Length)]; break;
                    case "T":
                        if (tPieces.Length > 0)
                            prefab = tPieces[rand.Next(tPieces.Length)]; break;
                    case "I":
                        if (linePieces.Length > 0)
                            prefab = linePieces[rand.Next(linePieces.Length)]; break;
                    case "L":
                        if (lPieces.Length > 0)
                            prefab = lPieces[rand.Next(lPieces.Length)]; break;
                    case "C":
                        if (endPieces.Length > 0)
                            prefab = endPieces[rand.Next(endPieces.Length)]; break;
                    case "O":
                        if (crossPieces.Length > 0)
                            prefab = startPieces[rand.Next(startPieces.Length)]; break;
                    case "X":
                        if (winPieces.Length > 0)
                            prefab = winPieces[rand.Next(winPieces.Length)]; break;
                }
                if (prefab != null)
                {
                    if (prefab.CompareTag("Problem"))
                        pos += new Vector3(0, 1.75f, 0);
                    int turn = getRotAmt(map[r][c]);
                    Quaternion rot = Quaternion.Euler(0f, 90f * turn, 0f);
                    GameObject piece = Instantiate(prefab, pos, rot, transform);
                    mapObj.Add(piece);
                }

                if (map[r][c].type != "O" && c > 0)
                    enemySpawns.Add(pos);
            }
        }
    }

    public void bossGen()
    {
        deleteMap();
        enemySpawns.Clear();


        GameObject floorRef = GameObject.FindGameObjectWithTag("FloorReference");
        Vector3 build = Vector3.zero;
        if (floorRef != null)
        {
            Vector3 floorPos = floorRef.transform.position;
            Vector3 floorScale = floorRef.transform.localScale;
            float width = floorScale.x * 10f;
            float depth = floorScale.y * 10f;
            build = new Vector3(floorPos.x - width / 2f + 15, -2 + floorPos.y, floorPos.z + depth / 2f);
        }

        GameObject startPrefab = startPieces[rand.Next(startPieces.Length)];
        GameObject startRoom = Instantiate(startPrefab, build, Quaternion.identity, transform);
        mapObj.Add(startRoom);

        build = build + new Vector3(blockSize - 12, 0, -12);
        GameObject bossPrefab = bossPieces[rand.Next(bossPieces.Length)];
        GameObject bossRoom = Instantiate(bossPrefab, build, Quaternion.identity, transform);
        mapObj.Add(bossRoom);
    }

    public void deleteMap()
    {
        foreach(GameObject obj in mapObj)
        {
            if (obj != null)
                Destroy(obj);
        }
        mapObj.Clear();
    }
    void Start()
    {

    }

    void Update()
    {

    }


};
