using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D texture;

    void Start()
    {
        
        Cursor.SetCursor(texture,new Vector2(texture.width / 2, texture.height / 2),CursorMode.Auto);
      
    }

  
    void Update()
    {
        
    }
}
