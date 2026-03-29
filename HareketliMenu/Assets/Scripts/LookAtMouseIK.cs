using UnityEngine;

public class LookAtMouseIK : MonoBehaviour
{
    private Animator anim;
    //[SerializeField] private Transform headBone;
   // [SerializeField] private Transform rightarm;
    [SerializeField] private float lookWeight = 1.0f; 

    private void Start() => anim = GetComponent<Animator>();

   
    private void OnAnimatorIK(int layerIndex)
    {
        if (anim)
        {
           
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 2f; 
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

           
            anim.SetLookAtWeight(lookWeight);
            anim.SetLookAtPosition(worldMousePos);
        }
    }
}