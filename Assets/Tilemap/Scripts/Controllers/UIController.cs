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
        SetStationListInformation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCoinStats(int stats, Vector2 position)
    {
        GameManager.manager.stats.PassengerMoved(stats);
        var coinsCounterText = GameObject.FindGameObjectWithTag(Tags.CoinsCounter).GetComponent<TMPro.TextMeshProUGUI>();
        var coins = GameManager.manager.stats.coins.ToString();
        coinsCounterText.text = coins;

        if (stats > 0)
        {
            CMDebug.TextPopup("+ " + stats, position, Color.yellow);
        }
        else
        {
            CMDebug.TextPopup("" + stats, position, Color.yellow);
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

    void SetStationListInformation()
    {
        var stationList = GameObject.FindGameObjectWithTag(Tags.StationList);
        var baseStation = GameObject.FindGameObjectWithTag(Tags.StationListImage);

        var orderedList = gameManager.coveredStationList;
        orderedList.Reverse();

        for (var i = 0; i < orderedList.Count-1; i++)
        {
            Station station = (Station)orderedList[i];
            Debug.Log(station.name);
            var stationImageObject = GameObject.Instantiate(baseStation, stationList.transform);
            stationImageObject.GetComponent<Image>().rectTransform.localScale = new Vector3(1, 1, 1);
            stationImageObject.GetComponent<Image>().sprite = station.bigIcon;
        }

        GameObject.DestroyImmediate(baseStation);
    }
}
