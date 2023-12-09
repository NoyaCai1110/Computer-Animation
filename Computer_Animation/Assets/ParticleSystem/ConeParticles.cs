using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeParticles : MonoBehaviour
{
    // Start is called before the first frame update
    float timer;
    float cycle;
    float deg1, deg2;
    Vector3 previous_pos;
    Vector3 dir, r_dir, v_dir;
    GameObject particle;
    //GameObject[] particles = new GameObject[5];
    void Start()
    {
        timer = 0;
        cycle = 16;
        transform.position = new Vector3(0, 200, 0);
        previous_pos = transform.position;
        dir = transform.forward;
        r_dir = transform.forward;
        v_dir = transform.forward;
        deg1 = 0;
        deg2 = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.position = new Vector3(40*Mathf.Sin(360 * Mathf.Deg2Rad * timer / cycle), 200 + 80*Mathf.Sin(360 * timer * Mathf.Deg2Rad / cycle), 80 - 80*Mathf.Cos(360 * timer * Mathf.Deg2Rad / cycle)); 
        dir = (transform.position - previous_pos).normalized;
        for (int i = 0; i < 1; i++)
        {
            deg1 = Random.Range(0, 45);
            deg2 = Random.Range(0, 360);
            r_dir = dir;
            r_dir = Quaternion.AngleAxis(deg1,transform.right) * r_dir;
            r_dir = Quaternion.AngleAxis(deg2, dir) * r_dir;
            r_dir = r_dir.normalized;
            particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            particle.transform.position = transform.position - 4.5f * r_dir;
            v_dir = dir;
            v_dir = Quaternion.AngleAxis(deg1/2, transform.right) * v_dir;
            v_dir = Quaternion.AngleAxis(deg2, dir) * v_dir;
            v_dir = v_dir.normalized;
            particle.AddComponent<Particle>().dir = v_dir;
            particle.GetComponent<MeshRenderer>().material = transform.GetComponent<MeshRenderer>().material;
        }
        previous_pos = transform.position;
    }
}
