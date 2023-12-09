using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class Animation2 : MonoBehaviour
{
    // Start is called before the first frame update
    string bvhpath = "D:/RIT/Term1/CSCI712/Unity/Ass2/Stand.txt";
    float[,] m_data = new float[3000, 300];
    int icnt = 0;
    int jcnt = 0;
    int itmp = 0;
    int jtmp = 0;
    int fps = 0;
    HierarchyTree root;
    class HierarchyTree
    {
        public HierarchyTree parent { get; set; }
        public HierarchyTree[] child = new HierarchyTree[3];
        public float offset1, offset2, offset3;
        public float Xposition, Yposition, Zposition;
        public float Zrotation, Xrotation, Yrotation;
        public bool root = false;
        public bool endsite = false;
        public bool visited = false;
        public String jointname;
        public GameObject joint;
    }
    void init_HierarchyTreeDFS(HierarchyTree current_joint)
    {
        if (current_joint == null)
        {
            return;
        }
        //Debug.Log(current_joint.jointname);
        //Debug.Log(new Vector3(current_joint.offset1, current_joint.offset2, current_joint.offset3));
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        item.name = current_joint.jointname;
        item.transform.parent = this.transform;
        current_joint.joint = item;
        if (current_joint.parent != null)
        {
            current_joint.joint.transform.position = current_joint.parent.joint.transform.position;
            current_joint.joint.transform.Translate(current_joint.offset1, current_joint.offset2, current_joint.offset3, current_joint.parent.joint.transform);
            //current_joint.joint.transform.rotation = current_joint.parent.joint.transform.rotation;
            LineRenderer line = current_joint.joint.AddComponent<LineRenderer>();
            line.positionCount = 2;
            //Vector3[] points = { current_joint.joint.transform.position, current_joint.parent.joint.transform.position };
            line.SetPositions(new Vector3[] { current_joint.joint.transform.position, current_joint.parent.joint.transform.position });
            line.material.color = Color.white;
            line.startWidth = 0.4f;
            line.endWidth = 0.4f;
        }
        else
        {
            current_joint.joint.transform.position = new Vector3(current_joint.offset1, current_joint.offset2, current_joint.offset3);
            //current_joint.joint.transform.rotation = Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)).normalized * current_joint.joint.transform.rotation;
        }
        for (int i = 0; i < current_joint.child.Length; i++)
        {
            init_HierarchyTreeDFS(current_joint.child[i]);
        }
    }
    void update_HierarchyTreeDFS(HierarchyTree current_joint)
    {
        if (current_joint == null)
        {
            return;
        }
        if (current_joint.jointname == "End Site")
        {
            current_joint.joint.transform.position = current_joint.parent.joint.transform.position;
            current_joint.joint.transform.Translate(current_joint.offset1, current_joint.offset2, current_joint.offset3, current_joint.parent.joint.transform);
            LineRenderer line = current_joint.joint.GetComponent<LineRenderer>();
            line.SetPositions(new Vector3[] { current_joint.joint.transform.position, current_joint.parent.joint.transform.position });
        }
        else if (current_joint.parent != null)
        {
            current_joint.Zrotation = m_data[itmp, jtmp];
            current_joint.Xrotation = m_data[itmp, jtmp + 1];
            current_joint.Yrotation = m_data[itmp, jtmp + 2];
            jtmp += 3;
            Quaternion m = Quaternion.AngleAxis(current_joint.Zrotation, new Vector3(0, 0, 1)).normalized
                         * Quaternion.AngleAxis(current_joint.Xrotation, new Vector3(1, 0, 0)).normalized
                         * Quaternion.AngleAxis(current_joint.Yrotation, new Vector3(0, 1, 0)).normalized;
            current_joint.joint.transform.rotation = m * current_joint.parent.joint.transform.rotation;
            current_joint.joint.transform.position = current_joint.parent.joint.transform.position;
            current_joint.joint.transform.Translate(current_joint.offset1, current_joint.offset2, current_joint.offset3, current_joint.parent.joint.transform);
            LineRenderer line = current_joint.joint.GetComponent<LineRenderer>();
            line.SetPositions(new Vector3[] { current_joint.joint.transform.position, current_joint.parent.joint.transform.position });
            Debug.Log(current_joint.joint.transform.position);
            Debug.Log(current_joint.joint.transform.rotation);
        }
        else
        {
            current_joint.Xposition = m_data[itmp, jtmp];
            current_joint.Yposition = m_data[itmp, jtmp + 1];
            current_joint.Zposition = m_data[itmp, jtmp + 2];
            current_joint.Zrotation = m_data[itmp, jtmp + 3];
            current_joint.Xrotation = m_data[itmp, jtmp + 4];
            current_joint.Yrotation = m_data[itmp, jtmp + 5];
            jtmp += 6;
            //Quaternion righttoleft = Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)).normalized;
            Quaternion m = Quaternion.AngleAxis(current_joint.Zrotation, new Vector3(0, 0, 1)).normalized
                         * Quaternion.AngleAxis(current_joint.Xrotation, new Vector3(1, 0, 0)).normalized
                         * Quaternion.AngleAxis(current_joint.Yrotation, new Vector3(0, 1, 0)).normalized;
            //current_joint.joint.transform.rotation = righttoleft * m;
            current_joint.joint.transform.rotation = m;
            current_joint.joint.transform.position = new Vector3(current_joint.Xposition, current_joint.Yposition, current_joint.Zposition);
            //Debug.Log(current_joint.joint.transform.position);
            //Debug.Log(current_joint.joint.transform.rotation);
            Debug.Log("New Frame");
        }
        for (int i = 0; i < current_joint.child.Length; i++)
        {
            update_HierarchyTreeDFS(current_joint.child[i]);
        }
    }

    void Start()
    {
        StreamReader bvhreader = new StreamReader(bvhpath);
        int p_count = 0;
        string bvh_line;
        string[] s_bvh_line;
        bvh_line = bvhreader.ReadLine();
        while (bvh_line != "{")
        {
            bvh_line = bvhreader.ReadLine();
        }
        p_count++;
        HierarchyTree current_joint, tmp;
        current_joint = new HierarchyTree();
        root = current_joint;
        root.jointname = "Hips";
        while (p_count != 0)
        {
            bvh_line = bvhreader.ReadLine();
            //Debug.Log(bvh_line);
            s_bvh_line = bvh_line.Split(new char[2] { ' ', '\t' }, options: StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in s_bvh_line)
            {
                //Debug.Log(s);
            }
            switch (s_bvh_line[0])
            {
                case "{":
                    p_count++;
                    //Debug.Log("{");
                    break;
                case "}":
                    p_count--;
                    if (p_count == 0)
                    {
                        break;
                    }
                    current_joint = current_joint.parent;
                    //Debug.Log(current_joint.jointname);
                    //Debug.Log("}");
                    break;
                case "OFFSET":
                    current_joint.offset1 = float.Parse(s_bvh_line[1]);
                    current_joint.offset2 = float.Parse(s_bvh_line[2]);
                    current_joint.offset3 = float.Parse(s_bvh_line[3]);
                    //Debug.Log("OFFSET");
                    break;
                case "CHANNELS":
                    if (s_bvh_line[1] == "6")
                    {
                        current_joint.root = true;
                    }
                    //Debug.Log("CHANNELS");
                    break;
                case "JOINT":
                    int i = 0;
                    while (current_joint.child[i] != null)
                    {
                        i++;
                    }
                    current_joint.child[i] = new HierarchyTree();
                    tmp = current_joint.child[i];
                    tmp.parent = current_joint;
                    current_joint = tmp;
                    current_joint.jointname = s_bvh_line[1];
                    //Debug.Log("JOINT");
                    break;
                case "End":
                    current_joint.endsite = true;
                    current_joint.child[0] = new HierarchyTree();
                    tmp = current_joint.child[0];
                    tmp.parent = current_joint;
                    current_joint = tmp;
                    current_joint.jointname = "End Site";
                    //Debug.Log("End Site");
                    break;
                default: break;
            }
        }
        init_HierarchyTreeDFS(root);
        //Debug.Log(root.child.Length);
        bvhreader.ReadLine();
        //TODO: frame rate
        bvhreader.ReadLine();
        bvhreader.ReadLine();

        while (bvhreader.Peek() > -1)
        {
            bvh_line = bvhreader.ReadLine();
            s_bvh_line = bvh_line.Split(new char[2] { ' ', '\t' }, options: StringSplitOptions.RemoveEmptyEntries);
            //Debug.Log(bvh_line);
            /*
            foreach (string s in s_bvh_line)
            {
                Debug.Log(s);
            }
            */
            //Debug.Log(s_bvh_line.Length);
            jcnt = s_bvh_line.Length;
            for (int j = 0; j < s_bvh_line.Length; j++)
            {
                m_data[icnt, j] = float.Parse(s_bvh_line[j]);
                //Debug.Log(m_data[cnt, i]);
            }
            icnt++;
        }
        Debug.Log(icnt);
    }

    // Update is called once per frame
    void Update()
    {
        fps++;
        if (fps == 5)
        {
            if (icnt - itmp > 1)
            {
                jtmp = 0;
                update_HierarchyTreeDFS(root);
            }
            itmp++;
            fps = 0;
        }

    }
}
