using UnityEngine;
using System.Collections;

public class TestInput : MonoBehaviour {
    
	void Update ()
    {
        float speed = 10;
        transform.position += new Vector3(Bifrost.Input.HorizontalAxis(), Bifrost.Input.VerticalAxis(), 0) * speed * Time.deltaTime;

        Transform camTransform = Camera.main.transform;
        camTransform.Rotate(-Bifrost.Input.GetMouseAxis().y, Bifrost.Input.GetMouseAxis().x, 0);
    }
}
