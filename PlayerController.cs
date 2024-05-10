using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float castRadius = 3f;
    
    private void OnClick()
    {
        Pointer pointer = InputSystem.GetDevice<Pointer>();
          if (EventSystem.current.IsPointerOverGameObject())
          {
            Debug.Log("IsPointerOverGameObject");
            return;
          }
        
        RaycastHit2D hit = Physics2D.CircleCast(GetCursorPosition(), castRadius, Vector2.zero);
       
        if (!hit)
        {
            Debug.Log("hit ==null");
            return;
        }
        if(hit.transform.GetComponent<IClickable>() == null)
        {            
            return;
        }
        IClickable hitAction = hit.transform.GetComponent<IClickable>();
        hitAction.ProcessClickAction();
    }

    public Vector2 GetCursorPosition()
    {
       // Debug.Log(playerInput.actions["Look"].ReadValue<Vector2>());
        Debug.Log(Camera.main.ScreenToWorldPoint(playerInput.actions["Look"].ReadValue<Vector2>()));

        return Camera.main.ScreenToWorldPoint(playerInput.actions["Look"].ReadValue<Vector2>());
    }

    public Vector2 GetMousePosition()
    {        
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }


    public Ray GetMouseRay()
    {
        return Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    }
}
