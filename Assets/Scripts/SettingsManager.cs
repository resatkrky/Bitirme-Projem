using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public Toggle _fullScreen;

    public Dropdown _resolutionIndex; //cozunurluk indexleri ayarlamak icin dropdown
    public Dropdown _textureQuality; //ekranda ne kadar alan kaplayacağı ayarlamak icin dropdown
    public Dropdown _antiAlaising; //kenar yumuşatma ayarlamak icin dropdown

    public Slider _menuVoice; //menu sesi değerini ayarlamak için slider

    public Button applyButton; //onaylama butonu

    public Resolution[] resolution; //cozunurluk değerleri için dizi ataması
    public GameSettings gameSettings; //GameSettings scriptinden değer çekmek için

    public AudioSource menuAudioSource; //menu sesi eklenmesi için audiosource

    private void OnEnable() //tanımlanan değerleri atamak yani açmak
    {
        gameSettings = new GameSettings();

        _fullScreen.onValueChanged.AddListener(delegate { onFullScreenToggle(); });
        _resolutionIndex.onValueChanged.AddListener(delegate { onResolutionChange(); });
        _textureQuality.onValueChanged.AddListener(delegate { onTextureQuality(); });
        _menuVoice.onValueChanged.AddListener(delegate { onMenuVoiceChange(); });
        _antiAlaising.onValueChanged.AddListener(delegate { onAntiAlaisingChange(); });

        resolution = Screen.resolutions;

        foreach(Resolution res in resolution)
        {
            _resolutionIndex.options.Add(new Dropdown.OptionData(res.ToString()));
        }

        //LoadSettings();
    }

    public void onApplyButton() //onaylama butonu için fonk
    {
        SaveSettings();
    }

    public void onFullScreenToggle() //tam ekran ayarlamak için fonk
    {
        gameSettings.fullScreen = Screen.fullScreen = _fullScreen.isOn;
    }

    public void onResolutionChange() //cozunurluk ayarlamak içic fonk
    {
        Screen.SetResolution(resolution[_resolutionIndex.value].width, resolution[_resolutionIndex.value].height,Screen.fullScreen);
    }

    public void onTextureQuality() //ekranda kaplanacak yer için fonk
    {
        QualitySettings.masterTextureLimit = gameSettings.textureQuality = _textureQuality.value;
    }

    public void onAntiAlaisingChange() //kenar yumuşatma fonk
    {
        QualitySettings.antiAliasing = gameSettings.antiAlaising = _antiAlaising.value;
    }

    public void onMenuVoiceChange() //menu sesini azaltıp çoğaltmak için
    {
        menuAudioSource.volume = gameSettings.menuVoice = _menuVoice.value;
    }

    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(gameSettings, true); //jsondata stringine gameSettings'deki seçilen verileri aktardık
        File.WriteAllText(Application.persistentDataPath + "/gameSettins.json", jsonData); //daha sonra sisteme(file) jsondata'daki verileri kaydetti
    }

    public void LoadSettings()
    {
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gameSettins.json")); //verileri file'dan okudu

        _fullScreen.isOn = gameSettings.fullScreen; //okunan tam ekran verisi oyundaki tam ekran verisine aktarıldı 
        _resolutionIndex.value = gameSettings.resolutionIndex;//okunan çözünürlük verisi oyundaki çözünürlük verisine aktarıldı 
        _textureQuality.value = gameSettings.textureQuality;//okunan yer kaplama verisi oyundaki yer kaplama verisine aktarıldı 
        _antiAlaising.value = gameSettings.antiAlaising;//okunan kenar yumuşatma verisi oyundaki kenar yumuşatma verisine aktarıldı 
        _menuVoice.value = gameSettings.menuVoice;//okunan menü ses verisi oyundaki menu ses verisine aktarıldı 
    }

}
