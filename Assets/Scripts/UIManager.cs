using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject turnOrder;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Unit unit in GameManager.instance.unitsList)
        {
            Image image = Instantiate(unit.image, turnOrder.transform);
        }
        GameManager.instance.OrderChanged.AddListener(ChangeOrder);
    }

    private void ChangeOrder()
    {
        turnOrder.transform.GetChild(0).SetAsLastSibling();
    }
}
