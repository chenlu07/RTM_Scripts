﻿#region License
/*
* Agent Development and Prototyping Testbed
* https://github.com/ashoulson/ADAPT
* 
* Copyright (C) 2011-2015 Alexander Shoulson - ashoulson@gmail.com
*
* This file is part of ADAPT.
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

public class IdleBehavior: Behavior
{
	protected Node BuildTreeRoot()
	{
		return
			new DecoratorLoop(
				new Sequence(
				new LeafWait(6000),
				this.Node_Gesture("relieved_sigh")));
	}
	
	void Start()
	{
		base.StartTree(this.BuildTreeRoot());
	}
}
