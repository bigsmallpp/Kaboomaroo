using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BombCountScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    // Start is called before the first frame update

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateBombCount(int count)
    {
        _text.text = count.ToString();
    }
}
