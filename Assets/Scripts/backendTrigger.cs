using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class backendTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.gameObject.tag);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is here!");
        }
    } 
}
