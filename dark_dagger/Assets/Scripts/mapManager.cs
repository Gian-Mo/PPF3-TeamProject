using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UIElements;

public class mapManager : MonoBehaviour
{
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

    private enum direction { UP = 0, RIGHT, DOWN, LEFT };

    private struct tile
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
        System.Random rng = new System.Random();
        int n = dirs.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n+1);
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




void Start()
    {

    }

    void Update()
    {

    }


};
