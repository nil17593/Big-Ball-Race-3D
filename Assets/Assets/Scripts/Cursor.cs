using UnityEngine;

public class Cursor : MonoBehaviour 
{

    public bool Move;
	void Update () 
	{
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1 << LayerMask.NameToLayer("Ground")))
        {
            transform.position = hit.point;
        }
        this.GetComponent<Cursor>().enabled = false;
	}
}
