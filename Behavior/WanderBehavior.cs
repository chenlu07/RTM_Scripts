#region License
/*
* ADAPT: Reading Terminal Market
*
* This file is part of the ADAPT for Reading Terminal Market.
* 
* cg.cis.upenn.edu/hms/research/ADAPT/Reading
* 
* Originally authored by Alexander Shoulson - ashoulson@gmail.com for ADAPT.
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

using System;
using UnityEngine;
using TreeSharpPlus;
using System.Collections;
using System.Collections.Generic;

public class WanderBehavior : Behavior
{
    // The locations we want to randomly patrol to

	public Transform wander1;
	public Transform wander2;
	public Transform wander3;

	protected Node ST_ApproachAndWait(Transform target)
	{
		Val<Vector3> position = Val.Value(() => target.position);
		
		return new Sequence(
			//new LeafTrace("Going to: " + position.Value),
			this.Node_GoTo(position),
			new LeafWait(1000));
	}

	protected Node BuildTreeRoot(){

		//ForceStatus to avoid exceptions when agent can't reach direct center of waypoint (another character or unaccessible by current route). Orig with just DecoratorLoop
		return
			new DecoratorLoop(
				new DecoratorForceStatus(RunStatus.Success,
			    	new SequenceShuffle(
						ST_ApproachAndWait(this.wander1),
						ST_ApproachAndWait(this.wander2),
						ST_ApproachAndWait(this.wander3))));
	}


	// Use this for initialization
	void Start() 
	{
		base.StartTree(this.BuildTreeRoot());
	}
	
}
