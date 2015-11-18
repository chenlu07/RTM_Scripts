using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;
using System.IO;

public class HeatCell
{
    public float selfheatvalue = 20;
    public bool IsHeatSourceOrNot = false;
    public float defaultheatvalue = 20;

    public HeatCell ()
    {
        this.selfheatvalue = 30;
        this.IsHeatSourceOrNot = false;
        this.defaultheatvalue = 30;
    }


    public void SetHeat(float m_value)
    {
        selfheatvalue = m_value;
    }
    public float GetHeat()
    {
        return selfheatvalue;
    }

    public Vector2 centerposition;
    public void SetCenterPosition(Vector2 m_value)
    {
        centerposition = m_value;
    }
    public Vector2 GetCenterPosition()
    {
        return centerposition;
    }

}


public struct HeatSourceObject
{
    public float initialheatvalue;
    public Vector3 m_positon;
    public string m_objectname;
}

[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]

public class HeatManager : MonoBehaviour {

    //private HeatGrid_raw[];
    public int Heatmapsize_row =128;
    public int Heatmapsize_column = 128;
    public static float Heatmapwidth_row = 0.5f;
    public static float Heatmapwidth_column = 0.5f;
    public float center_x = 0;
    public float center_y = 0;
    private int timer = 0;
    static GameObject myTextObject;

    //The params in CLAUDIO's model
    static public float dt = 0.4f;//constant temperature
    static public float c = 1; //heat constant propagation in the environment
    static public float beta = 0.1f; //heat transfer constant from source to environment
    static public float T = 20.0f; //heat transfer constant from source to environment

    HeatCell[][] Heatmap;
    //Vector3[] vertices;
    //int[] triangles;
    //Vector3[] normals;
    //Vector2[] uvs;
    //Color32[] colors32;
    //List<Color32> colorlist;
    Mesh mesh;
    public Texture2D generatedTexture;//texture data
    float[][] c_initializing_array; //heat constant propagation in the environment
    float[][] c_array; //heat constant propagation in the environment

    public WallMaper wallmap;

    [System.NonSerialized]

    private static HeatManager instance = null;
    public static HeatManager Instance
    {
        get
        {
            if (instance != null)
                return instance;
            else
                return null;
        }
    }


    public List<HeatSourceObject> HeatSourceObjectList;

    // Use this for initialization
    void Start () {
        if (instance == null)
            instance = this;
    }

    void OnEnable()
    {
        if (instance != null)
            throw new System.Exception("Multiple Managers found");
        instance = this;
        this.HeatSourceObjectList =  new List<HeatSourceObject>() ;
    }

    // Update is called once per frame
    void Update () {
        if(Heatmap == null)
        {
            this.InitMapWithInitializeTemperature_Zero();
            //this.CreatheatmapRender();
            wallmap = new WallMaper();
            wallmap.Initializewallmap(Heatmapsize_row, Heatmapsize_column, Heatmapwidth_row, Heatmapwidth_column);
            InitCarray();
        }
        this.CaluHeatMap();
        //this.ChangeHeatMapTexture();
        //Draw();
    }

    public void InitMapWithInitializeTemperature_Zero()
    {
        Heatmap = new HeatCell[Heatmapsize_row][];
        for (int i = 0; i < Heatmapsize_row; i++)
        {
            HeatCell[] tempz = new HeatCell[Heatmapsize_column];

            for (int j = 0; j < Heatmapsize_column; j++)
            {
                tempz[j] = new HeatCell();
                Vector2 m_center = CaluPosition(i, j);
                tempz[j].SetCenterPosition(m_center);
                //if(i< Heatmapsize_column-j)
                //{
                //    tempz[j].defaultheatvalue = 3f;
                //    tempz[j].SetHeat(3f);
                //}
                //else
                //{
                //    tempz[j].defaultheatvalue = 6f;
                //    tempz[j].SetHeat(6f);
                //}

                tempz[j].defaultheatvalue = 20f;
                tempz[j].SetHeat(20f);
                Vector3 m_3Dpositon;
                m_3Dpositon.x = m_center.x;
                m_3Dpositon.y = 2;
                m_3Dpositon.z = m_center.y;
                /////////////////////ceshi
                //if (i > Heatmapsize_column - j)
                //{
                    float m_value = this.FindHeatValueForCell(m_3Dpositon);
                    if (m_value != 0)
                    {
                        tempz[j].SetHeat(m_value);
                        tempz[j].IsHeatSourceOrNot = true;
                        tempz[j].defaultheatvalue = m_value;
                    }
                //}
                /////////////////////////////


            }

            this.Heatmap[i] = tempz;
        }
        
    }

    public Vector2 CaluPosition(int rowindex, int columnindex)
    {
        float m_rowpositon = 0;
        float m_columnpositon = 0;

        //if (this.Heatmapsize_row %2 != 0)
        //{
        //    float m_inttofloat = this.Heatmapsize_row / 2 - rowindex;
        //    m_rowpositon = this.center_x - m_inttofloat * Heatmapwidth_row;
        //}
        //else
        //{
        //    float m_inttofloat = this.Heatmapsize_row / 2 - rowindex;
        //    m_rowpositon = this.center_x - (m_inttofloat-0.5f) * this.Heatmapwidth_row;
        //}

        //if (Heatmapsize_column % 2 != 0)
        //{
        //    float m_inttofloat = Heatmapsize_column / 2 - columnindex;
        //    m_columnpositon = center_y - m_inttofloat * Heatmapwidth_column;
        //}
        //else
        //{
        //    float m_inttofloat = Heatmapsize_column / 2 - columnindex;
        //    m_columnpositon = center_y - (m_inttofloat - 0.5f) * Heatmapwidth_column;
        //}
        float m_inttofloat = (float)this.Heatmapsize_row / 2.0f - rowindex;
        m_rowpositon = this.center_x - m_inttofloat * (float)Heatmapwidth_row;
        m_inttofloat = (float)Heatmapsize_column / 2.0f - columnindex;
        m_columnpositon = center_y - m_inttofloat * (float)Heatmapwidth_column;

        Vector2 m_position = new Vector2(m_rowpositon, m_columnpositon);

        return m_position;
    }
    public static  void AddHeatSourceObject(Vector3 m_pos, string m_name, float m_value)
    {
        HeatSourceObject m_newobj = new HeatSourceObject();
        m_newobj.initialheatvalue = m_value;
        m_newobj.m_objectname = m_name;
        m_newobj.m_positon = m_pos;

        Instance.HeatSourceObjectList.Add(m_newobj);
    }


    public float FindHeatValueForCell(Vector3 m_position)
    {
        float m_value = 0;
        foreach (HeatSourceObject obj in this.HeatSourceObjectList)
        {
            Vector3 m_distance = m_position - obj.m_positon;
            if(Math.Abs(m_distance.x)<Heatmapwidth_row && Math.Abs(m_distance.z) < Heatmapwidth_column)
            {
                return obj.initialheatvalue;
            }
        }
        return m_value;
    }


    public void CaluHeatMap()
    {
        timer++;
        //if(timer <= 200)
        //{
        //    return;
        //}
        if(timer > 500)
        {
           timer = 201;
           ResetHeatMap();
        }
        
        for (int i = 1; i < Heatmapsize_row-1; i++)
        {
            for (int j = 1; j < Heatmapsize_column-1; j++)
            {
                if (Heatmap[i][j].IsHeatSourceOrNot)
                {
                    Heatmap[i][j].SetHeat(Heatmap[i][j].defaultheatvalue);
                    continue;
                }
                float laplacian = -4.0f * Heatmap[i][j].GetHeat();
                if (i > 0)
                    laplacian = laplacian + Heatmap[i-1][j].GetHeat();
                else
                    laplacian = laplacian + T;

                if (i < Heatmapsize_row)
                    laplacian = laplacian + Heatmap[i + 1][j].GetHeat();
                else
                    laplacian = laplacian + T;

                if (j > 0)
                    laplacian = laplacian + Heatmap[i][j-1].GetHeat();
                else
                    laplacian = laplacian + T;

                if (j < Heatmapsize_column)
                    laplacian = laplacian + Heatmap[i][j+1].GetHeat();
                else
                    laplacian = laplacian + T;

                if (Heatmap[i][j].IsHeatSourceOrNot)
                    Heatmap[i][j].SetHeat(Heatmap[i][j].GetHeat() + dt * c_array[i][j] * laplacian + beta * (Heatmap[i][j].defaultheatvalue - Heatmap[i][j].GetHeat()));
                else
                    Heatmap[i][j].SetHeat(Heatmap[i][j].GetHeat() + dt * c_array[i][j] * laplacian);
            }
        }
        //
    }

    //public void UpdateColorofheatmap(/*Vector2 start, Vector2 end*/)
    //{
    //    //MeshFilter mFilter = myTextObject.GetComponent<MeshFilter>();
    //    //Mesh m_newmesh = mFilter.mesh;
    //    //m_newmesh.Clear();
    //    colorlist.Clear();
    //    Material amaterial = myTextObject.GetComponent<Renderer>().material;
    //    amaterial.SetPass(0);
    //    GL.PushMatrix();
    //    GL.LoadIdentity();

    //    for (int i = 0; i < Heatmapsize_row; i++)
    //    {
    //        for (int j = 0; j < Heatmapsize_column; j++)
    //        {
    //            //Color32 m_color = Heatmap[i][j].GetHeat() / 8.0f * Color.red + (8.0f - Heatmap[i][j].GetHeat()) / 8.0f * Color.blue;
    //            Color32 m_color = new Color32(0,0,255,255);
    //            colors32[i * Heatmapsize_column + j] = m_color;
    //            //colorlist.Add(m_color);
    //        }
    //    }

    //    //m_newmesh.hideFlags = HideFlags.DontSave;
    //    //m_newmesh.vertices = vertices;
    //    //m_newmesh.triangles = triangles;
    //    //m_newmesh.uv = uvs;
    //    //m_newmesh.normals = normals;
    //    //m_newmesh.colors32 = colors32;

    //    for(int i=3; i<6*(Heatmapsize_column-1)*(Heatmapsize_row - 1); i = i+3)
    //    {

    //        Color32 m_aaaaa;
    //        Vector3 m_p;
    //        int index1, index2, index3;
    //        index1 = triangles[i - 1];
    //        index2 = triangles[i - 2];
    //        index3 = triangles[i-3];

    //        GL.Begin(GL.TRIANGLES);

    //        GL.Color(colors32[index1]);
    //        m_aaaaa = colors32[index1];
    //        GL.Vertex(vertices[index1]);
    //        m_p = vertices[index1];

    //        GL.Color(colors32[index2]);
    //        m_aaaaa = colors32[index2];
    //        GL.Vertex(vertices[index2]);
    //        m_p = vertices[index2];

    //        GL.Color(colors32[index3]);
    //        m_aaaaa = colors32[index3];
    //        GL.Vertex(vertices[index3]);
    //        m_p = vertices[index3];

    //        GL.End();
            
    //    }
    //    //amaterial.SetPass(0);
    //    //LayerMask alayer = LayerMask.NameToLayer("Default");
    //    //Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, amaterial, alayer);
    //    GL.PopMatrix();
    //}

    public void CreatheatmapRender()
    {
        myTextObject = new GameObject("HeatAreaRender");
        myTextObject.AddComponent<MeshFilter>();
        myTextObject.AddComponent<MeshRenderer>();
        MeshFilter mFilter = myTextObject.GetComponent<MeshFilter>();
        MeshRenderer mRen = myTextObject.GetComponent<MeshRenderer>();
        Material material1 = new Material(Shader.Find("Specular"));
        mRen.material = material1;

        //矩形的四个顶点坐标

        int rowmaxindex = Heatmapsize_row;
        int columnmaxindex = Heatmapsize_column;

        //vertices = new Vector3[rowmaxindex * columnmaxindex];
        //triangles = new int[6 * (Heatmapsize_column-1) * (Heatmapsize_row-1)];
        //normals = new Vector3[rowmaxindex * columnmaxindex];
        //uvs = new Vector2[rowmaxindex * columnmaxindex];
        //colors32 = new Color32[rowmaxindex * columnmaxindex];
        //colorlist = new List<Color32>();

        //for (int i = 0; i < rowmaxindex; i++)
        //{
        //    for (int j = 0; j < columnmaxindex; j++)
        //    {
        //        Vector2 m_pos = Heatmap[i][j].GetCenterPosition();
        //        vertices[i * columnmaxindex + j] = new Vector3(m_pos.x, 20.0f, m_pos.y);
        //        normals[i * columnmaxindex + j] = new Vector3(-5,0,0);
        //        switch((i * columnmaxindex + j) %4)
        //        {
        //            case 0:
        //                uvs[i * columnmaxindex + j] = new Vector2(0, 0);
        //                break;
        //            case 1:
        //                uvs[i * columnmaxindex + j] = new Vector2(1, 0);
        //                break;
        //            case 2:
        //                uvs[i * columnmaxindex + j] = new Vector2(1, 1);
        //                break;
        //            case 3:
        //                uvs[i * columnmaxindex + j] = new Vector2(0, 1);
        //                break;
        //        }
                
        //        //colors32[i * columnmaxindex + j] = new Color32(255, 0, 0, 255);

        //        if (i == 0 && j == 0)
        //        {//3
        //            triangles[6 * ((i) * (Heatmapsize_column - 1) + j )] = i + i * columnmaxindex + j;//(left-buttom)
        //            triangles[6 * ((i) * (Heatmapsize_column - 1) + j ) + 5] = i + i * columnmaxindex + j;//(left-buttom)
        //        }

        //        if (i >0 && i < rowmaxindex-1 && j>0 && j< columnmaxindex-1)
        //        {//1
        //            triangles[6 * ((i - 1) * (Heatmapsize_column - 1) + (j - 1)) + 2] = i * columnmaxindex + j;//(right-buttom)
        //            triangles[6 * ((i - 1) * (Heatmapsize_column - 1) + (j - 1)) + 3] = i * columnmaxindex + j;//(right-buttom)
        //            triangles[6 * ((i - 1) * (Heatmapsize_column - 1) + j ) + 1] = i * columnmaxindex + j;//(right-top)
        //            triangles[6 * ((i) * (Heatmapsize_column - 1) + j )] = i * columnmaxindex + j;//(left-buttom)
        //            triangles[6 * ((i) * (Heatmapsize_column - 1) + j ) +5] = i * columnmaxindex + j;//(left-buttom)
        //            triangles[6 * ((i) * (Heatmapsize_column - 1) + (j-1) ) +4] = i * columnmaxindex + j;//()
        //        }

        //        if (i == 0 && j > 0 && j < columnmaxindex-1)
        //        {//2
        //            triangles[6 * j] = i * columnmaxindex + j;
        //            triangles[6 * j + 5] = i * columnmaxindex + j;
        //            triangles[6 * (j - 1) + 4] = i * columnmaxindex + j;
        //        }

        //        if (i == 0 && j == columnmaxindex-1)
        //        {//4
        //            triangles[6 * ((i) * (Heatmapsize_column - 1) + j - 1) + 4] = i * columnmaxindex + j;//()
        //        }

        //        if (i == rowmaxindex-1 && j > 0 && j < columnmaxindex-1)
        //        {//5
        //            triangles[6 * ((i - 1)* (Heatmapsize_column - 1) + (j - 1)) + 2] = i * columnmaxindex + j;//(right-buttom)
        //            triangles[6 * ((i - 1)* (Heatmapsize_column - 1) + (j - 1)) + 3] = i * columnmaxindex + j;//(right-buttom)
        //            triangles[6 * ((i - 1)* (Heatmapsize_column - 1) + j) + 1] = i * columnmaxindex + j;//(right-top)
        //        }

        //        if (i == rowmaxindex - 1 && j == columnmaxindex - 1)
        //        {//6
        //            triangles[6 * ((i - 1) * (Heatmapsize_column - 1) + (j - 1) ) + 2] = i * columnmaxindex + j;//(right-buttom)
        //            triangles[6 * ((i - 1) * (Heatmapsize_column - 1) + (j - 1)) + 3] = i * columnmaxindex + j;//(right-buttom)
        //        }

        //        if (i > 0 && i < rowmaxindex-1 && j ==  columnmaxindex-1)
        //        {//7
        //            triangles[6 * ((i - 1) * (Heatmapsize_column - 1) + (j - 1)) + 2] = i * columnmaxindex + j;//(right-buttom)
        //            triangles[6 * ((i - 1) * (Heatmapsize_column - 1) + (j - 1)) + 3] = i * columnmaxindex + j;//(right-buttom)
        //            triangles[6 * ((i) * (Heatmapsize_column - 1) + (j - 1)) + 4] = i * columnmaxindex + j;//()
        //        }

        //        if (i > 0 && i < rowmaxindex - 1 && j == 0)
        //        {//8
        //            triangles[6 * ((i - 1) * (Heatmapsize_column - 1) + j) + 1] = i * columnmaxindex + j;//(right-top)
        //            triangles[6 * ((i) * (Heatmapsize_column - 1) + j)] = i * columnmaxindex + j;//(left-buttom)
        //            triangles[6 * ((i) * (Heatmapsize_column - 1) + j) + 5] = i * columnmaxindex + j;//(left-buttom)
        //        }

        //        if (i == rowmaxindex - 1 && j == 0)
        //        {//9
        //            triangles[6 * ((i - 1) * (Heatmapsize_column - 1) + j ) + 1] = i * columnmaxindex + j;//(right-top)
        //        }
        //    }
        //}

        /////////////////////////////////////
        Vector3[] vertices4 = new Vector3[4];
        int[] triangles6 = new int[6]; 
        Vector2[] uvs4 = new Vector2[4]; 
        Vector3[] normals4 = new Vector3[4];

        vertices4[0] = new Vector3(-(float)Heatmapsize_row * 0.5f * (float)Heatmapwidth_row, 0.0f, (float)Heatmapsize_row * 0.5f * (float)Heatmapwidth_row);
        vertices4[1] = new Vector3(-(float)Heatmapsize_row * 0.5f * (float)Heatmapwidth_row, 0.0f, -(float)Heatmapsize_row * 0.5f * (float)Heatmapwidth_row);
        vertices4[2] = new Vector3((float)Heatmapsize_row * 0.5f * (float)Heatmapwidth_row, 0.0f, -(float)Heatmapsize_row * 0.5f * (float)Heatmapwidth_row);
        vertices4[3] = new Vector3((float)Heatmapsize_row * 0.5f * (float)Heatmapwidth_row, 0.0f, (float)Heatmapsize_row * 0.5f * (float)Heatmapwidth_row);

        triangles6[0] = 0;
        triangles6[1] = 3;
        triangles6[2] = 2;
        triangles6[3] = 2;
        triangles6[4] = 1;
        triangles6[5] = 0;


        uvs4[3] = new Vector2(0, 1);
        uvs4[0] = new Vector2(0, 0);
        uvs4[1] = new Vector2(1, 0);
        uvs4[2] = new Vector2(1, 1);


        normals4[0] = new Vector3(0,1, 0);
        normals4[1] = new Vector3(0,1, 0);
        normals4[2] = new Vector3(0,1, 0);
        normals4[3] = new Vector3(0,1, 0);

        /////////////////////////////////////

        mesh = new Mesh();

        mesh.hideFlags = HideFlags.DontSave;
        mesh.vertices = vertices4;
        mesh.triangles = triangles6;
        //mesh.colors32 = colors32;
        mesh.uv = uvs4;
        mesh.normals = normals4;
        //mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
        mFilter.mesh = mesh;

    }

    public void InitCarray()
    {
 
        ////////////////////////////////////////Initialize the C_array
        c_initializing_array = new float[Heatmapsize_row][];
        c_array = new float[Heatmapsize_row][];
        for (int i = 0; i < Heatmapsize_row; i++)
        {
            c_initializing_array[i] = new float[Heatmapsize_column];
            c_array[i] = new float[Heatmapsize_column];
            for (int j = 0; j < Heatmapsize_column; j++)
            {
                c_array[i][j] = new float();
                c_array[i][j] = 1;
                c_initializing_array[i][j] = new float();
                c_initializing_array[i][j] = 1;
                if (wallmap.wallmap != null)
                {
                    if(wallmap.wallmap[i][j] != 1)
                       c_initializing_array[i][j] = wallmap.wallmap[i][j];
                }
            }
        }



        double[][] localfilter = null;
        //malloc2DArray(localfilter, 5, 5);
        localfilter = new double[5][];
        for (int i = 0; i < 5; i++)
        {
            localfilter[i] = new double[5];
        }
        makeGaussianFilter(localfilter, 5, 0.5);
        imfilter2(c_initializing_array, localfilter, c_array, Heatmapsize_row, Heatmapsize_column, 5);
    }

    public Texture2D GenerateParabola()
    {
        //创建一个新的2D贴图

        //Material amaterial2 = myTextObject.GetComponent<MeshRenderer>().material;
        //var proceduralTexture = Instantiate(amaterial2.mainTexture) as Texture2D;
        int widthlong = ((int)(Heatmapwidth_row*10*Heatmapsize_row)); 
        int heightlong = ((int)(Heatmapwidth_column * 10 * Heatmapsize_column));
        Texture2D proceduralTexture = new Texture2D(widthlong, heightlong, TextureFormat.ARGB32, true);

        //获得贴图的中心点
        Vector2 centerPixelPosition = new Vector2(0,0);

        //遍历贴图上的每一个像素点 
        for (int i = 0; i < Heatmapsize_row; i = i+1)
        {
            for (int j = 0; j < Heatmapsize_column; j=j+1)
            {
                //Color pixelColor = Color.white;
                Color pixelColor;/* = Heatmap[i][j].GetHeat() / 8.0f * Color.red + (8.0f - Heatmap[i][j].GetHeat()) / 8.0f * Color.blue+ 0.5f*Color.green;*/
                if(Heatmap[i][j].GetHeat() >=0 && Heatmap[i][j].GetHeat() <4)
                {
                    pixelColor = (float)(4.0f-0.5f*Heatmap[i][j].GetHeat()) / 4.0f * Color.blue + (float)(0.5 * Heatmap[i][j].GetHeat()) / 4.0f * Color.green;
                }
                else
                {
                    pixelColor = (float)(4.0f - 0.5f * Heatmap[i][j].GetHeat()) / 4.0f * Color.green + (float)(0.5 * (Heatmap[i][j].GetHeat()-4.0f)) / 4.0f * Color.red;
                }

                pixelColor.a = 0.8f;
                //if (Heatmap[i][j].GetHeat() > 5)
                //    pixelColor =  Heatmap[i][j].GetHeat() / 8.0f * Color.red;
                //else if (Heatmap[i][j].GetHeat() > 3)
                //    pixelColor = Heatmap[i][j].GetHeat() / 8.0f * Color.white;
                //else
                //    pixelColor = Heatmap[i][j].GetHeat() / 8.0f * Color.white;

                for (int k = 0; k <10.0* Heatmapwidth_row; k++)
                {
                    for(int m = 0; m < 10.0 * Heatmapwidth_column; m++)
                    {
                        int m_indexrow = i * (int)(Heatmapwidth_row * 10) + k;
                        int m_indexcol = j * (int)(Heatmapwidth_column * 10) + m;
                        proceduralTexture.SetPixel(m_indexrow, m_indexcol, pixelColor );
                    }
                }

            }
        }
        //将像素应用到贴图
        proceduralTexture.Apply(true);
        //proceduralTexture = null;

        //return the texture to the main program.
        return proceduralTexture;
    }

    public void ChangeHeatMapTexture()
    {

        Material amaterial2 = myTextObject.GetComponent<MeshFilter>().GetComponent<Renderer>().material;
        //Material amaterial2 = myTextObject.GetComponent<MeshRenderer>().material;
        generatedTexture = GenerateParabola();
        amaterial2.SetTexture("_MainTex", generatedTexture);
    }

    public void ResetHeatMap()
    {
        for (int i = 0; i < Heatmapsize_row ; i++)
        {
            for (int j = 0; j < Heatmapsize_column; j++)
            {
                Heatmap[i][j].SetHeat(Heatmap[i][j].defaultheatvalue);
            }
        }
    }

    static public void makeGaussianFilter(double[][] lfliter, int iSize, double sigma)//arry is the the original matrix c
    {
        //StreamWriter write = new StreamWriter("D:\\1.txt");
       // write.Write("gaosi\n");
        int center = (iSize - 1) / 2;
        double sum = 0;
        double x2 = 0;
        double y2 = 0;
        for (int i = 0; i < iSize; i++)
        {
            x2 = Math.Pow((double)(i - center), 2);
            for (int j = 0; j < iSize; j++)
            {
                y2 = Math.Pow((double)(j - center), 2);
                lfliter[i][j] = Math.Exp(-(x2 + y2) / (2 * sigma * sigma));
                sum += lfliter[i][j];
            }
        }
        if (sum != 1 && sum != 0)
        {
            //归一化  
            for (int i = 0; i < iSize; i++)
            {
                for (int j = 0; j < iSize; j++)
                {
                    lfliter[i][j] = lfliter[i][j]*Math.Pow(sum, -1);
                }
            }
        }
       // write.Close();
        //return array;  
    }

  static public void malloc2DArray(double[][] filter,int iRow, int iCol)
  {  
      filter = new double[iRow][];  
      for (int i = 0; i<iRow; i++)  
      {  
          filter[i] = new double[iCol];  
      }  
      //return filter;  
  }


    static void imfilter2(float[][] src_image, double[][] filter, float[][] dst_image,int rowsize, int columnsize,int flitersize)
    {
       // StreamWriter write = new StreamWriter("D:\\1.txt");
        int start_row = (int)(flitersize / 2);
        int start_col = (int)(flitersize / 2);
        float[][] Mid_Matrix = new float[rowsize + 2 * start_row][];
        for(int i = 0; i< rowsize + 2 * start_row; i++)
        {
            Mid_Matrix[i] = new float[columnsize + 2 * start_col];
            for (int j = 0; j< columnsize + 2 * start_col; j++)
            {
                Mid_Matrix[i][j] = new float();
                Mid_Matrix[i][j] = 0;
            }
        }
        for (int i = 0; i < rowsize; i++)
        {
            for (int j = 0; j < columnsize; j++)
            {
                Mid_Matrix[i + start_row][ j + start_col] = src_image[i][j];
            }
        }
        int end_row = rowsize - 1 - start_row;
        int end_col = columnsize - 1 - start_col;
        int filter_row = flitersize;
        int filter_col = flitersize;
        float m_max = 0;
        for (int i = start_row; i <= end_row; i++)
        {
            for (int j = start_col; j <= end_col; j++)
            {
                int tmp_row = i - start_row;
                int tmp_col = j - start_col;
                for (int m = 0; m < filter_row; m++)
                {
                    for (int n = 0; n < filter_col; n++)
                    {
                        dst_image[tmp_row][tmp_col] += Mid_Matrix[tmp_row + m][tmp_col + n] * (float)filter[m][n];
                    }
                }
                if(m_max <= dst_image[tmp_row][tmp_col])
                {
                    m_max = dst_image[tmp_row][tmp_col];
                }
            }
        }
        for (int i = 0; i < rowsize; i++)
        {
            for (int j = 0; j < columnsize; j++)
            {
                dst_image[i][j] = dst_image[i][j] / m_max;
            }
        }

    }

    public static float GetHeatValueByPos(Vector2 m_pos)
    {
        int rowindex = 0;
        int columnindex = 0;
        rowindex = (int)((float)Instance.Heatmapsize_row / 2.0f - (Instance.center_x - m_pos.x) / (float)Heatmapwidth_row);
        columnindex = (int)((float)Instance.Heatmapsize_column / 2.0f - (Instance.center_y - m_pos.y) / (float)Heatmapwidth_column);

        if(rowindex>=0 && rowindex<= Instance.Heatmapsize_row && columnindex >= 0 && columnindex <= Instance.Heatmapsize_column)
        {
            return Instance.Heatmap[rowindex][columnindex].selfheatvalue;
        }
        return 0;
    }
}
