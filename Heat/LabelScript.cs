#region License
/*
* ADAPT: Reading Terminal Market
*
* This file is part of the ADAPT for Reading Terminal Market.
* 
* cg.cis.upenn.edu/hms/research/ADAPT/Reading
* 
* ADAPT is free software: you can redistribute it and/or modify
* it under the terms of the GNU Lesser General Public License as published
* by the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
* 
* ADAPT is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU Lesser General Public License for more details.
* 
* You should have received a copy of the GNU Lesser General Public License
* along with ADAPT.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//attached to new label in CrowdManager
public class LabelScript : MonoBehaviour
{

    protected Text editable;
    protected GameObject character; //parent in hierarchy
    protected float heat;

    protected bool active = true; //if heat label should be shown

    // Use this for initialization
    void Start()
    {
        active = true;

        editable = this.transform.Find("Text").GetComponent<Text>();
        character = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //RenderHeatValue();
        RenderComfortValue();
    }

    void RenderHeatValue()//The rendering code by Charles
    {
        heat = character.GetComponent<HeatReceiver>().heat;
        float adjustedheat = heat + 73.4f;
        char degree = (char)176;
        if (Input.GetKeyDown(KeyCode.M))
        {//if pushed, reverse bool
            active = !active;
        }
        if (active)
        {
            //if active, show data
            editable.text = "Rel. heat = " + adjustedheat.ToString("#.00") + degree.ToString() + "F";//convert heat to string with only 2 decimal places
        }
        else
        {
            //if off, show character name
            editable.text = character.name;
        }

    }
    void RenderComfortValue()//The rendering code by Charles
    {
        heat = (float)character.GetComponent<ComfortJudgment>().heatcomfortablelevel;
        float m_tep = (float)character.GetComponent<ComfortJudgment>().centertemp;
        float m_densitycomfort = (float)character.GetComponent<ComfortJudgment>().densitycomfortablelevel;

        if (Input.GetKeyDown(KeyCode.M))
        {//if pushed, reverse bool
            active = !active;
        }
        if (active)
        {
            //if active, show data
            editable.text = "Heat Comfort = " + heat.ToString("#.00") +"\n" + "Density Comfort " + m_densitycomfort.ToString("#.00") + "\n" + "Tem = " + m_tep.ToString("#.00");//convert heat to string with only 2 decimal places

            heat = heat - 16.99f;
            Color pixelColor;/* = Heatmap[i][j].GetHeat() / 8.0f * Color.red + (8.0f - Heatmap[i][j].GetHeat()) / 8.0f * Color.blue+ 0.5f*Color.green;*/
            if (heat < 1f)
            {
                pixelColor = (float)(1f - 0.5f * heat) / 1f * Color.blue + (float)(0.5 * heat) / 1f * Color.green;
            }
            else
            {
                pixelColor = (float)(1f - 0.5f * heat) / 1f * Color.green + (float)(0.5 * (heat - 1f)) / 1f * Color.red;
            }

            pixelColor.a = 1.0f;
            editable.color = pixelColor;
        }
        else
        {
            //if off, show character name
            editable.text = character.name;
        }
    }
}
