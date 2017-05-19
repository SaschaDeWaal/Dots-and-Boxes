
using UnityEngine;

public enum RelativePosition {
	up = 0,
	left = 1,
	down = 2,
	right = 3
}

public static class RelativePositionTools{

	public static RelativePosition Mirror(this RelativePosition pos) {
		return (RelativePosition)(((int)pos+2)%4);
	}

}