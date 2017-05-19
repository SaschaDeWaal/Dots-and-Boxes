using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollListPosition : MonoBehaviour {

	private const float height = 0.08f;

	private void Start () {
		RectTransform rectTransform = GetComponent<RectTransform>();
		int index = rectTransform.GetSiblingIndex();

		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.anchoredPosition = new Vector2(0, (Screen.width * height) * index);	
	}
	
}
