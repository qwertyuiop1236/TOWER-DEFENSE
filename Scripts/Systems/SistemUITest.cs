using System.Drawing;
using UnityEngine;
using TMPro;

public class SistemUITest : MonoBehaviour
{
    [SerializeField] public static int _xpPoint=20;
    [SerializeField] private int _koin=100;
    [SerializeField] private int _Point=0;
    [SerializeField] private float _time=180;


    //Text Pro
    [SerializeField] private TMP_Text XPText;
    [SerializeField] private TMP_Text KoinText;
    [SerializeField] private TMP_Text PointText;
    [SerializeField] private TMP_Text TimeText;

    private void Start() 
    {
        TimeText.text=_time.ToString();
    }

    private void Update()
    {
        _time-=Time.deltaTime;
        TimeText.text=_time.ToString();
        if (_time <= 0)
        {
            _time=180;
        }
        XPText.text=_xpPoint.ToString();
    }


    public void Damage(int damage)
    {
        if (_xpPoint - damage <= 0)
        {
            _xpPoint = 0;
        }
        else
        {
            _xpPoint -= damage;
        }
    }

    public void XPXil(int xil)
    {
        if(!(_xpPoint+xil >= _xpPoint))
        {
            _xpPoint+=xil;
        }
    }

    public void koinSum( int Koin1)
    {
        _koin += Koin1;
        _Point += Koin1;
    }

    public void PointSum(int point)
    {
        _Point +=point;
    }



}