using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler ,IBeginDragHandler{

    public InventoryItemBase Item { get; set; }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
       
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
    }

	public void OnBeginDrag(PointerEventData eventData)
	{
		transform.position = Input.mousePosition;
	}
}
