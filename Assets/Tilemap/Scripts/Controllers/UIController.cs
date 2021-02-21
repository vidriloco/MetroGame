using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    private ResourceManager resourceManager;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        resourceManager = GameObject.FindObjectOfType<ResourceManager>();
        gameManager = GameManager.manager.SetResourcesObject(resourceManager);

        SetCurrentStationInformation();
        SetBoundStationInformation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCoinStats(int stats)
    {
        GameManager.manager.stats.PassengerMoved(stats);
        var coinsCounterText = GameObject.FindGameObjectWithTag(Tags.CoinsCounter).GetComponent<TMPro.TextMeshProUGUI>();
        var coins = GameManager.manager.stats.coins.ToString();
        coinsCounterText.text = coins;

        if (stats > 0)
        {
            CMDebug.TextPopupMouse("+ " + stats);
        }
        else
        {
            CMDebug.TextPopupMouse("" + stats);
        }

        var coinIcon = GameObject.FindGameObjectWithTag(Tags.CoinsIcon);

        LeanTween.scale(coinIcon, new Vector3(1.2f, 1.2f, 1.2f), 0.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            LeanTween.scale(coinIcon, new Vector3(0.8f, 0.8f, 0.8f), 0.2f);
        });
        LeanTween.rotateAroundLocal(coinIcon, new Vector3(0, 1, 0), 360, 0.5f);
    }

    void SetCurrentStationInformation()
    {
        var stationImage = GameObject.FindGameObjectWithTag(Tags.CurrentStationIcon).GetComponent<Image>();
        stationImage.sprite = gameManager.currentStation.bigIcon;
        var stationNameLabel = GameObject.FindGameObjectWithTag(Tags.CurrentStationNameLabel).GetComponent<TMPro.TextMeshProUGUI>();
        stationNameLabel.text = gameManager.currentStation.name;
    }

    void SetBoundStationInformation()
    {
        var stationImage = GameObject.FindGameObjectWithTag(Tags.BoundStationIcon).GetComponent<Image>();
        stationImage.sprite = gameManager.boundStation.bigIcon;
        var stationNameLabel = GameObject.FindGameObjectWithTag(Tags.BoundStationNameLabel).GetComponent<TMPro.TextMeshProUGUI>();
        stationNameLabel.text = gameManager.boundStation.name;
    }
}
