using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;
using System.IO;

public class WallMaper : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public float[][] wallmap = null;
    public LayerMask voxel;
    public void Initializewallmap(int rowsize, int columnsize, float rowwidth, float columnwidth)
    {
        LayerMask alayer = LayerMask.NameToLayer("Market");
        Vector3 m_startingpos = new Vector3(0, 0, 0);
        Vector3 m_direction = new Vector3(0, -1, 0);
        Ray m_ray = new Ray(m_startingpos, m_direction);
        RaycastHit hit;
        float m_max = 0;
        float m_min = 0;

 
        wallmap = new float[rowsize][];
        for (int i = 0; i < columnsize; i++)
        {

            float[] wallcolumnmap = new float[columnsize];
            for (int j = 0; j < columnsize; j++)
            {
                m_startingpos.x = (rowsize - i) * 0.5f * rowwidth;
                m_startingpos.y = 6.99f;
                m_startingpos.z = (columnsize - j) * 0.5f * columnwidth;

                m_ray.origin = m_startingpos;

                if (Physics.Raycast(m_startingpos, m_direction, out hit))
                {
                    if(!(hit.transform.name.IndexOf("heat")>0))
                    {
                    float m_wallcolumnmap = (float)(6.99f - hit.point.y) / 6.99f; ;
                    if (m_max < hit.point.y)
                        m_max = hit.point.y;
                    if (m_min > hit.point.y)
                        m_min = hit.point.y;
                    wallcolumnmap[j] = m_wallcolumnmap;
                    if (wallcolumnmap[j] < 0)
                    {
                        wallcolumnmap[j] = 0;
                    }
                    if (wallcolumnmap[j] == 0)
                    {
                        wallcolumnmap[j] = 0;
                    }
                    else if (wallcolumnmap[j] > 1)
                    {
                        wallcolumnmap[j] = 1;
                    }
                    else if (wallcolumnmap[j] == 1)
                    {
                        wallcolumnmap[j] = 1;
                    }
                    else
                    {
                        wallcolumnmap[j] = wallcolumnmap[j];
                    }

                    }

                }

            }
            wallmap[i] = wallcolumnmap;

        }
       

    }
}
