using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    float timer, lifetime, v;
    public Vector3 dir = new Vector3(0,1,0);
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        lifetime = Random.Range(2, 6);
        v = Random.Range(6, 8);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > lifetime)
        {
            Destroy(gameObject);
        }
        transform.position -= v*dir * Time.deltaTime;
    }
    private void OnDrawGizmos()
    {
        //Debug.DrawLine(transform.position, transform.position + dir);
    }
}
