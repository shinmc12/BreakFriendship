using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class timer : MonoBehaviour
{

    [SerializeField] float setTime = 4.0f;
    [SerializeField] Text countdownText;
    public GameObject round1;
    public GameObject time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (setTime > 0)
            setTime -= Time.deltaTime;
        else if (setTime <= 0)
            Time.timeScale = 0.0f;


        if (Mathf.Round(setTime).ToString() == "0")
        {
            time.SetActive(false);
            round1.SetActive(true);
        }
        countdownText.text = (Mathf.Round(setTime)).ToString();
    }
}
