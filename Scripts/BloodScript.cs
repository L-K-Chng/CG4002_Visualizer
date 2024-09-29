using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bloodSplatter;

    void Start()
    {
        bloodSplatter.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BloodSplatterEffect()
    {
        StartCoroutine(BloodSplatterDuration());
    }

    private IEnumerator BloodSplatterDuration()
    {
        bloodSplatter.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        bloodSplatter.SetActive(false);
    }
}
