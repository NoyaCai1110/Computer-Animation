using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SplineAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    float timer = 0;
    float elapse = 0;
    float t = 0;
    Vector3 v0 = new Vector3(0, 0, 0),
            v1 = new Vector3(4, 0, 0),
            v2 = new Vector3(8, 0, 0),
            v3 = new Vector3(12, 12, 12),
            v4 = new Vector3(12, 18, 18),
            v5 = new Vector3(18, 18, 18),
            v6 = new Vector3(18, 18, 18),
            v7 = new Vector3(25, 12, 12),
            v8 = new Vector3(25, 0, 18),
            v9 = new Vector3(25, 1, 18),
            vs = new Vector3(-4, 0, 0),
            ve = new Vector3(25, 2, 18);
    Quaternion q0 = Quaternion.AngleAxis(0, new Vector3(1, 1, -1)),
               q1 = Quaternion.AngleAxis(30, new Vector3(1, 1, -1)),
               q2 = Quaternion.AngleAxis(90, new Vector3(1, 1, -1)),
               q3 = Quaternion.AngleAxis(180, new Vector3(1, 1, -1)),
               q4 = Quaternion.AngleAxis(270, new Vector3(1, 1, -1)),
               q5 = Quaternion.AngleAxis(90, new Vector3(0, 1, 0)),
               q6 = Quaternion.AngleAxis(90, new Vector3(0, 0, 1)),
               q7 = Quaternion.AngleAxis(0, new Vector3(1, 0, 0)),
               q8 = Quaternion.AngleAxis(0, new Vector3(1, 0, 0)),
               q9 = Quaternion.AngleAxis(0, new Vector3(1, 0, 0));

    void Start()
    {
        timer = Time.time;
        transform.position = Vector3.zero;
        //transform.rotation = Quaternion.LookRotation(new Vector3(1, 1, -1));
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    // Update is called once per frame
    void catmull_rom(Vector3 vs, Vector3 v0, Vector3 v1, Vector3 v2, float t)
    {
        Vector3 c0 = v0,
                c1 = -vs / 2 + v1 / 2,
                c2 = vs - 5 * v0 / 2 + 2 * v1 - v2 / 2,
                c3 = -vs / 2 + 3 * v0 / 2 - 3 * v1 / 2 + v2 / 2;
        transform.position = c0 + c1 * t + c2 * Mathf.Pow(t, 2) + c3 * Mathf.Pow(t, 3);
    }
    void Bezier_deCasteljau_s(Vector3 v0, Vector3 v1, Vector3 v2, float t)
    {
        Vector3 a0 = 2 * v1 - v2,
                b1 = v0 / 2 + v1 - v2 / 2;
        Vector3 R0 = Vector3.Lerp(v0, a0, t),
                R1 = Vector3.Lerp(a0, b1, t),
                R2 = Vector3.Lerp(b1, v1, t);
        Vector3 r0 = Vector3.Lerp(R0, R1, t),
                r1 = Vector3.Lerp(R1, R2, t);
        transform.position = Vector3.Lerp(r0, r1, t);
    }
    void Bezier_deCasteljau(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float t)
    {
        Vector3 a1 = -v0 / 2 + v1 + v2 / 2,
                b2 = v1 / 2 + v2 - v3 / 2;
        Vector3 R0 = Vector3.Lerp(v1, a1, t),
                R1 = Vector3.Lerp(a1, b2, t),
                R2 = Vector3.Lerp(b2, v2, t);
        Vector3 r0 = Vector3.Lerp(R0, R1, t),
                r1 = Vector3.Lerp(R1, R2, t);
        transform.position = Vector3.Lerp(r0, r1, t);
    }
    void Bezier_deCasteljau_e(Vector3 v7, Vector3 v8, Vector3 v9, float t)
    {
        Vector3 a8 = -v7 / 2 + v8 + v9 / 2,
                    b9 = 2 * v9 - v8;
        Vector3 R0 = Vector3.Lerp(v8, a8, t),
                R1 = Vector3.Lerp(a8, b9, t),
                R2 = Vector3.Lerp(b9, v9, t);
        Vector3 r0 = Vector3.Lerp(R0, R1, t),
                r1 = Vector3.Lerp(R1, R2, t);
        transform.position = Vector3.Lerp(r0, r1, t);
    }

    void slerp_rotate(Quaternion q0, Quaternion q1, float t)
    {
        q0.Normalize();
        q1.Normalize();
        transform.rotation = Quaternion.Slerp(q0, q1, t);
        transform.rotation.Normalize();
    }

    void Update()
    {
        //Timer in sec
        elapse = Time.time - timer;
        timer = Time.time;

        if (Time.time > 0 && Time.time <= 1)
        {
            t = Time.time;
            //TRANSFORM
            //transform.position += new Vector3(4 * elapse, 0, 0);
            //transform.position = Vector3.Lerp(v0, v1, t);
            //catmull_rom(vs, v0, v1, v2, t);
            Bezier_deCasteljau_s(v0, v1, v2, t);
            //ROTATION
            slerp_rotate(q0, q1, t);
        }
        else if (Time.time > 1 && Time.time <= 2)
        {
            t = Time.time - 1;
            //TRANSFORM
            //transform.position += new Vector3(4 * elapse, 0 * elapse, 0 * elapse);
            //transform.position = Vector3.Lerp(v1, v2, t);
            //catmull_rom(v0, v1, v2, v3, t);
            Bezier_deCasteljau(v0, v1, v2, v3, t);
            //ROTATION
            slerp_rotate(q1, q2, t);
        }
        else if (Time.time > 2 && Time.time <= 3)
        {
            t = Time.time - 2;
            //TRANSFORM
            //transform.position += new Vector3(4 * elapse, 12 * elapse, 12 * elapse);
            //transform.position = Vector3.Lerp(v2, v3, t);
            //catmull_rom(v1, v2, v3, v4, t);
            Bezier_deCasteljau(v1, v2, v3, v4, t);
            //ROTATION
            slerp_rotate(q2, q3, t);
        }
        else if (Time.time > 3 && Time.time <= 4)
        {
            t = Time.time - 3;
            //TRANSFORM
            //transform.position += new Vector3(4 * elapse, 12 * elapse, 12 * elapse);
            //transform.position = Vector3.Lerp(v3, v4, t);
            //catmull_rom(v2, v3, v4, v5, t);
            Bezier_deCasteljau(v2, v3, v4, v5, t);
            //ROTATION
            slerp_rotate(q3, q4, t);
        }
        else if (Time.time > 4 && Time.time <= 5)
        {
            t = Time.time - 4;
            //TRANSFORM
            //transform.position += new Vector3(6 * elapse, 0 * elapse, 0 * elapse);
            //transform.position = Vector3.Lerp(v4, v5, t);
            //catmull_rom(v3, v4, v5, v6, t);
            Bezier_deCasteljau(v3, v4, v5, v6, t);
            //ROTATION
            slerp_rotate(q4, q5, t);
        }
        else if (Time.time > 5 && Time.time <= 6)
        {
            t = Time.time - 5;
            //TRANSFORM
            //transform.position += new Vector3(0 * elapse, 0 * elapse, 0 * elapse);
            //transform.position = Vector3.Lerp(v5, v6, t);
            //catmull_rom(v4, v5, v6, v7, t);
            Bezier_deCasteljau(v4, v5, v6, v7, t);
            //ROTATION
            slerp_rotate(q5, q6, t);
        }
        else if (Time.time > 6 && Time.time <= 7)
        {
            t = Time.time - 6;
            //TRANSFORM
            //transform.position += new Vector3(7 * elapse, -6 * elapse, -6 * elapse);
            //transform.position = Vector3.Lerp(v6, v7, t);
            //catmull_rom(v5, v6, v7, v8, t);
            Bezier_deCasteljau(v5, v6, v7, v8, t);
            //ROTATION
            slerp_rotate(q6, q7, t);
        }
        else if (Time.time > 7 && Time.time <= 8)
        {
            t = Time.time - 7;
            //TRANSFORM
            //transform.position += new Vector3(0 * elapse, -12 * elapse, 6 * elapse);
            //transform.position = Vector3.Lerp(v7, v8, t);
            //catmull_rom(v6, v7, v8, v9, t);
            Bezier_deCasteljau(v6, v7, v8, v9, t);
            //ROTATION
            slerp_rotate(q7, q8, t);
        }
        else if (Time.time > 8 && Time.time <= 9)
        {
            t = Time.time - 8;
            //TRANSFORM
            //transform.position += new Vector3(0 * elapse, 1 * elapse, 0 * elapse);
            //transform.position = Vector3.Lerp(v8, v9, t);
            //catmull_rom(v7, v8, v9, ve, t);
            Bezier_deCasteljau_e(v7, v8, v9, t);
            //ROTATION
            slerp_rotate(q8, q9, t);
        }
        else
        {

        }
    }
}
