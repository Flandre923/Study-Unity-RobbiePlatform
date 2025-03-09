using UnityEngine;

public class DoorScript : MonoBehaviour
{    
    
    private Animator anim;
    private int doorOpenID;

    void Start()
    {
        anim = GetComponent<Animator>();
        doorOpenID = Animator.StringToHash("Open");
        GameManager.registerDoorScript(this);
    }
    
    public void DoorUp()
    {
        anim.SetTrigger(doorOpenID);
        //
        AudioManager.PlayDoorOpenAudio();
    }
    
}
