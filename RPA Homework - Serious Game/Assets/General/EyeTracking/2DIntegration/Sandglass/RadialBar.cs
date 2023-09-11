using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadialBar : MonoBehaviour
{
    public Image fill;
    public TextMeshProUGUI amount;
    private float timer;
    private float maxTime;
    private bool started;

    // Start is called before the first frame update
    public void Start()
    {

    }

    private void Update()
    {
        if (started)
        {
            timer += Time.deltaTime;
            if (timer > maxTime) timer = maxTime;
            fill.fillAmount = Normalize();
            amount.text = $"{timer.ToString("0.00")}";
        }
    }

    private float Normalize()
    {
        return (float)timer / maxTime;
    }

    public void StartTimer(float maxTime)
    {
        timer = 0;
        this.maxTime = maxTime;
        fill.fillAmount = Normalize();
        amount.text = $"{timer.ToString("0.00")}";
        started = true;
    }
    public void StopTimer()
    {
        started = false;
    }

    public bool IsRunning()
    {
        return started;
    }
}
