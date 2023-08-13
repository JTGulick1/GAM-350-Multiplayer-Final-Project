using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    float speed = 5;
    public bool isIt = false;

    public Material isNotTagged;
    public Material tagged;
    public Renderer player;

    void Start()
    {
        player.material = isNotTagged; // Material change
    }

    void Update()
    {
        if (GetComponent<NetworkSync>().owned)
        {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime, 0); // Movement
            transform.position += movement;
        }
        if (isIt)
        {
            this.GetComponent<MeshRenderer>().material = tagged; // color changer
            speed = 7; // increase Speed for it
        }
        if(!isIt) // is not it
        {
            this.GetComponent<MeshRenderer>().material = isNotTagged; // color changer 
            speed = 5; // decrease speed for not it
        }
    }
    public void GotTagged(bool tagged) // function for the server to call if they are it or not
    {
        isIt = tagged;
    }
}
