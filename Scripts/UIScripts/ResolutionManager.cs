using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResolutionManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public TMP_Text label;
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private double currentRefreshRate;
    void Awake()
    {
        //Almaceno todas las resoluciones disponibles por el monitor
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        //Eliminamos los valores por defecto del dropdown
        resolutionDropdown.ClearOptions();
        //Almacenamos el valor de la tasa de refresco del monitor para filtrar las resoluciones a esa tasa de refresco
        currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;
        Application.targetFrameRate = Mathf.RoundToInt((float)currentRefreshRate);
        //Filtramos las resoluciones
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height;
            options.Add(resolutionOption);
        }

        resolutionDropdown.AddOptions(options);

        if (!PlayerPrefs.HasKey("ResolutionIndex"))
        {
            Resolution maxResolution = resolutions[resolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, FullScreenMode.FullScreenWindow);
            resolutionDropdown.value = 0;
            PlayerPrefs.SetInt("ResolutionIndex", 0);
        }
        else
        {
            int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            setResolution(savedResolutionIndex);
            resolutionDropdown.value = savedResolutionIndex;
        }
        resolutionDropdown.RefreshShownValue();
    }

    public void setResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    } 

}
