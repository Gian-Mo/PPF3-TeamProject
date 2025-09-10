using UnityEngine;


public class PickUp : MonoBehaviour
{
    enum typeOfPickUp {health, ammo}
    [SerializeField] typeOfPickUp type;
    [SerializeField] int amount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        IPickUp pickUp = other.GetComponent<IPickUp>();
        if (pickUp != null)
        {
            if (type == typeOfPickUp.health) { 
            
                pickUp.pickUp(amount, (int)type);
            }
          
        }
    }
}
