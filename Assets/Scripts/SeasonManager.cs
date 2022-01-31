using UnityEngine;
using UnityEngine.UI;

public enum Season
{
    SPRING,
    SUMMER,
    AUTOMN,
    WINTER
}

public class SeasonManager : MonoBehaviour
{
    public static SeasonManager instance = null;

    private float currentTime = 0;
    private float seasonSpeed = 1;

    private Season currentSeason = Season.SPRING;

    public Text seasonText = null;
    public Slider seasonSlider = null;

    private float seasonLegth = 30;

    private void Awake()
    {
        instance = this;
        seasonSlider.value = currentTime / seasonLegth;
    }

    public void MoveForward()
    {
        currentTime += Time.deltaTime * seasonSpeed;
        seasonSlider.value = currentTime / seasonLegth;
        if (currentTime >= seasonLegth)
        {
            currentTime = 0;
            switch (currentSeason)
            {
                case Season.SPRING:
                    {
                        currentSeason = Season.SUMMER;
                        break;
                    }
                case Season.SUMMER:
                    {
                        currentSeason = Season.AUTOMN;
                        break;
                    }
                case Season.AUTOMN:
                    {
                        currentSeason = Season.WINTER;
                        break;
                    }
                case Season.WINTER:
                    {
                        currentSeason = Season.SPRING;
                        break;
                    }
            }
            seasonText.text = "Season : " + currentSeason;
        }
    }

    private void Update()
    {
        
    }
}
