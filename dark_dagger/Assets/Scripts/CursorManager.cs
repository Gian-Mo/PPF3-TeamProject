using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class CursorManager : MonoBehaviour
{
     public static CursorManager instance;
    [SerializeField] Texture2D[] texture;


    private void Awake()
    {
        if (instance == null) { instance = this; }
        // SetMenusCursor();
        SetAimCursor();
    }

    public void SetAimCursor()
    {
        Cursor.SetCursor(texture[1], new Vector2(texture[1].width / 2, texture[1].height / 2), CursorMode.Auto);
      
    }

    public void SetMenusCursor()
    {
        Cursor.SetCursor(texture[0], new Vector2(3,3), CursorMode.Auto);
    }

    private void FadeTexture()
    {
      
    }
    
}
