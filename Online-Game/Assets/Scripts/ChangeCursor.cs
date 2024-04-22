using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    [SerializeField] private Texture2D cursor;

    private void Start()
    {
        if (ClientSingleton.Instance == null) { return; }

        // You will see cursor that you put in the 'cursor' variable.
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.Auto);
    }
}