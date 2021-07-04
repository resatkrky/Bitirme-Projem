using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class languageManager : MonoBehaviour
{
    //ana menü
    public Text mHeader;
    public TMPro.TextMeshProUGUI startGame;
    public TMPro.TextMeshProUGUI settings;
    public TMPro.TextMeshProUGUI credits;
    public TMPro.TextMeshProUGUI exit;

    //settings
    public Text sHeader;
    public TMPro.TextMeshProUGUI graphic;
    public TMPro.TextMeshProUGUI resolution;
    public TMPro.TextMeshProUGUI textureQuality;
    public TMPro.TextMeshProUGUI antiAlaising;
    public TMPro.TextMeshProUGUI audio;
    public TMPro.TextMeshProUGUI backSettings;
    public Text apply;
    public Text fullScreen;

    //credits
    public Text cHeader;
    public TMPro.TextMeshProUGUI gameDeveloper;
    public TMPro.TextMeshProUGUI tutorialText;
    public TMPro.TextMeshProUGUI tutorialExplain;
    public TMPro.TextMeshProUGUI backCredits;



    languageSettings languageSettings;
    void Start()
    {
        
    }

    
    void Update()
    {
        //languageSettings = new languageSettings();
        //languageSettings = JsonUtility.FromJson<languageSettings>(File.ReadAllText(Application.persistentDataPath + "/langSet.json"));

        //if(languageSettings.dilSecim == 0)
        //{
        //    onClickTr();
        //}
        //else
        //{
        //    onClickEn();
        //}
    }

    public void onClickTr()
    {
        //languageSettings = new languageSettings();
        //languageSettings.dilSecim = 0;
        //string jsonData = JsonUtility.ToJson(languageSettings,true);
        //File.WriteAllText(Application.persistentDataPath + "/langSet.json", jsonData);

        mHeader.text = "CANAVAR AVLAMA";
        startGame.text = "Oyuna Basla";
        settings.text = "Ayarlar";
        credits.text = "Extralar";
        exit.text = "Çıkış";

        sHeader.text = "AYARLAR";
        graphic.text = "Grafik";
        fullScreen.text = "Tam Ekran";
        audio.text = "Ses";
        resolution.text = "Çözünme";
        textureQuality.text = "Doku Kalitesi";
        antiAlaising.text = "Kenar Yumuşatma";
        backSettings.text = "Geri";
        apply.text = "Uygula";

        cHeader.text = "EXTRALAR";
        gameDeveloper.text = "Oyun Geliştirici";
        tutorialText.text = "İpucu";
        tutorialExplain.text = "Eğer Boss'u bulmak istiyorsan Normal Canavarları öldürmelisin ama bu çok zor çünkü Boss daha güçlü ve Normal Canavarlar onu korur.";
        backCredits.text = "Geri";
    }
    public void onClickEn()
    {
        //languageSettings = new languageSettings();
        //languageSettings.dilSecim = 1;
        //string jsonData = JsonUtility.ToJson(languageSettings, true);
        //File.WriteAllText(Application.persistentDataPath + "/langSet.json", jsonData);

        mHeader.text = "HUNTING MONSTER";
        startGame.text = "Start Game";
        settings.text = "Settings";
        credits.text = "Credits";
        exit.text = "Exit";

        sHeader.text = "SETTINGS";
        graphic.text = "Graphic";
        fullScreen.text = "Full Screen";
        audio.text = "Audio";
        resolution.text = "Resolution";
        textureQuality.text = "Texture Quality";
        antiAlaising.text = "AntiAlaising";
        backSettings.text = "Back";
        apply.text = "Apply";

        cHeader.text = "CREDITS";
        gameDeveloper.text = "Game Developer";
        tutorialText.text = "Tutorial";
        tutorialExplain.text = "If you find the Boss Monter, you must kill Normal Monters but it's so hard because the Boss is stronger than Normal Monters and Normal Monsters product it.";
        backCredits.text = "Back";
    }
}
