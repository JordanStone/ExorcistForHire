using System.Collections.Generic;
using UnityEngine;

public class PathDefinition : MonoBehaviour {
	public Transform[] Points;
	public enum PatrolType 
	{
		Line,
		Circular
	}
	public PatrolType Type = PatrolType.Line;
	public IEnumerator<Transform> GetPathEnumerator() {
		if(Points == null || Points.Length < 1)
			yield break;
		var direction = 1;
		var index = 0;
		while(true) {
			yield return Points[index];
			if(Points.Length == 1)
				continue;
			if(Type == PatrolType.Line) {
				if(index <= 0)
					direction = 1;
				else if(index >= Points.Length -1)
					direction = -1;
				index = index + direction;
			} else {
				if(Type == PatrolType.Circular) {
					if(index >= Points.Length -1) {
						index = 0;
					} else {
						index = index + 1;
					}
				}
			}
		}
	}
	public void OnDrawGizmos() {
		if(Points == null || Points.Length < 2)
			return;
		for(var i = 1; i < Points.Length; i++) {
			Gizmos.DrawLine(Points[i-1].position, Points[i].position);
		}
	}

}
