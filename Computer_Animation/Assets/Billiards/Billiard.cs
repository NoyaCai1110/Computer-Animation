using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;


public class Billiard : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public float d, r, m, mmi, g;
    [HideInInspector] public float TX, TZ;
    [HideInInspector] public float vs, w, vr, v, tmp_v;
    [HideInInspector] public Vector2 dir, tmp_dir;
    [HideInInspector] public float br_e, bb_e, bc_us, bc_ur;
    [HideInInspector] public float Fs, Fr;
    [HideInInspector] public bool x_hit, z_hit, b_hit, bb_collision;
    [HideInInspector] public Billiard[] Billiards;
    [HideInInspector] public Billiard Cue;

    void Start()
    {
        //Find all balls
        Billiards = GameObject.FindObjectsOfType<Billiard>();
        //ball diameter: 2.25 in
        d = 2.25f;
        r = d / 2;
        g = 9.8f;
        //ball mass: 6 oz
        m = 6f;
        //ball mass moment of inertia: 2 / 5 mR2
        mmi = 0.4f;
        
        if (transform.CompareTag("Cue"))
        {
            //Cueball velocity
            vs = 30;
            dir = new Vector2(2, 3).normalized;
            vr = 0;
            w = vr / r;
            v = vr + vs;
            Cue = this;
            Debug.Log("Hello");
        }
        else
        {
            //Other ball velocity
            vs = 0;
            dir = new Vector2(0, 0);
            vr = 0;
            w = vr / r;
            v = vr + vs;
            Debug.Log("Hi");
        }
        
        //ball-rail coefficient of restitution (e): 0.6-0.9
        br_e = 0.75f;
        //ball-ball coefficient of restitution(e): 0.92 - 0.98
        bb_e = 0;
        //ball-rail hit protection
        x_hit = false;
        z_hit = false;
        b_hit = false;
        bb_collision = false;
        //ball-cloth coefficient of rolling resistance (¦Ì): 0.005 ¨C 0.015
        bc_ur = 0.01f;
        Fr = 0;
        //ball - cloth coefficient of sliding friction(¦Ì): 0.15 - 0.4(typical value: 0.2)
        bc_us = 0.2f;
        Fs = 0;
        //Table x: -50 ~ 50
        TX = 50f;
        //Table z: -25 ~ 25
        TZ = 25f;
    }

    // Update is called once per frame
    void Update()
    {
        //Step1
        if(Mathf.Abs(v) > 0.01f)
        {
            if(Mathf.Abs(vs) < Mathf.Abs(v * 0.01f))
            {
                Fs = 0;
                Fr = bc_ur * m * g;
            }
            else
            {
                Fs = bc_us * m * g;
                Fr = 0;
            }
        }
        else
        {
            Fs = 0;
            Fr = 0;
        }
        //Step2
        transform.position += new Vector3(v * dir.x * Time.deltaTime, 0, v * dir.y * Time.deltaTime);
        transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * w * Time.deltaTime, new Vector3(dir.y, 0, -dir.x)) * transform.rotation;
        //Step3
        //ball-rail collision
        if (transform.position.x < -(TX - r) || transform.position.x > (TX - r))
        {
            if (!x_hit)
            {
                v = Mathf.Sqrt(Mathf.Pow(v * -dir.x * br_e, 2) + Mathf.Pow(v * dir.y * br_e, 2));
                dir = new Vector2 (-dir.x * br_e, dir.y * br_e).normalized;
                x_hit = true;
            }
        }
        else
        {
            x_hit = false;
        }
        if (transform.position.z < -(TZ - r) || transform.position.z > (TZ - r))
        {
            if (!z_hit)
            {
                v = Mathf.Sqrt(Mathf.Pow(v * dir.x * br_e, 2) + Mathf.Pow(v * -dir.y * br_e, 2));
                dir = new Vector2(dir.x * br_e, -dir.y * br_e).normalized;
                z_hit = true;
            }
        }
        else
        {
            z_hit = false;
        }
        //ball-ball collision
        bb_collision = false;
        foreach(Billiard ball in Billiards)
        {
            if (ball != this)
            {
                float l = Vector3.Distance(this.transform.position, ball.transform.position);
                if (l < 2*r)
                {
                    if(!b_hit)
                    {
                        b_hit = true;
                        ball.tmp_dir = dir;
                        ball.tmp_v = v;
                        if ((this.v - ball.v) > 0.1f)
                        {
                            Debug.Log(this.v - ball.v);
                            this.v = ball.v;
                            Debug.Log("1");
                            //Debug.Log(this.v - ball.v);
                        }
                        else
                        {
                            this.v = tmp_v;
                            Debug.Log("2");
                        }
                        if (this.dir != ball.dir)
                        {
                            this.dir = ball.dir;
                            Debug.Log("3");
                        }
                        else
                        {
                            this.dir = tmp_dir;
                            Debug.Log("4");
                        }
                    }
                    bb_collision = true;
                }
            }
        }
        b_hit = bb_collision;
        //Step4

        v += (-(Fs+Fr) / m) * Time.deltaTime;
        if(Fs != 0)
        {
            w += (5 * Fs * Time.deltaTime) / (2 * m * r);
            vr = w * r;
            vs = v - vr;
        }
        else
        {
            vr = v;
            w = vr / r;
        }
        //Debug.Log(v + ", " + vs + ", " + vr + ", " + Fs + ", " + Fr);
    }
}
