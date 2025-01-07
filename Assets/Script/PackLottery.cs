using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:家豪
//  創建日期:2025/01/07
//  創建用途: 抽獎
//==========================================

/// <summary>
/// 卡牌資料
/// </summary>
[Serializable]
public class PackData
{
    public string PackID;
    public Sprite PackSprite;
}

public class PackLottery : MonoBehaviour
{
    [Header("遊戲物件")]
    [SerializeField] private List<Image> packimages = new List<Image>();
    [SerializeField] private Sprite cardBackground;
    [SerializeField] private GameObject shuffleBtn;
    [SerializeField] private GameObject againBtn;


    [Header("遊戲資料")]
    [SerializeField] private List<PackData> packDatas = new List<PackData>();
    private List<PackData> tempSps = new List<PackData>();   //暫存此次擁有的牌組圖片
    public bool Mode { get; set; } = false;      // 結果模式 False:正常抽獎 True:內定效果
    //內定型 紀錄
    private PackData tempPackData;

    private void Start()
    {
        SettingPack();
    }

    /// <summary>
    /// 公開結果
    /// </summary>
    public void Result(Image packImg)
    {
        StartCoroutine(ResultCoroutine(packImg));
    }

    private IEnumerator ResultCoroutine(Image packImg)
    {
        if (!Mode)
        {
            for (int i = 0; i < packimages.Count; i++)
            {
                packimages[i].sprite = tempSps[i].PackSprite;
                yield return new WaitForSeconds(0.25f);
            }
        }
        else
        {
            var finalPacks = packimages.Where(x => x != packImg).ToList();
            var othertempDatas = tempSps.Where(x => x != tempPackData).ToList();
            packImg.sprite = tempPackData.PackSprite;

            for (int i = 0; i < finalPacks.Count; i++)
            {
                finalPacks[i].sprite = othertempDatas[i].PackSprite;
                yield return new WaitForSeconds(0.25f);
            }
        }
        againBtn.SetActive(true);
    }

    public void SettingPack()
    {
        tempSps.Clear();
        tempSps = packDatas.OrderBy(x => Guid.NewGuid()).Take(5).ToList();

        for (int i = 0; i < packimages.Count; i++)
        {
            packimages[i].sprite = tempSps[i].PackSprite;
        }
        packimages.ForEach(x => x.GetComponent<Button>().enabled = false);
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    public void Shuffle()
    {
        tempPackData = tempSps.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        StartCoroutine(ShuffleCoroutine());
    }

    private IEnumerator ShuffleCoroutine()
    {
        shuffleBtn.SetActive(false);
        //每張卡牌 換成 卡背
        packimages.ForEach(x => x.sprite = cardBackground);
        //儲存原始座標
        List<Vector2> tempPosition = packimages.Select(x => new Vector2(x.GetComponent<RectTransform>().anchoredPosition.x, x.GetComponent<RectTransform>().anchoredPosition.y)).ToList();
        yield return new WaitForSeconds(0.5f);
        //執行 洗牌動畫
        packimages.ForEach(x => x.GetComponent<RectTransform>().position = Vector2.zero);
        yield return new WaitForSeconds(0.5f);
        //洗牌洗完歸位
        for (int i = 0; i < packimages.Count; i++)
        {
            packimages[i].GetComponent<RectTransform>().anchoredPosition = tempPosition[i];
        }
        packimages.ForEach(x => x.GetComponent<Button>().enabled = true);
        tempSps = tempSps.OrderBy(x => Guid.NewGuid()).ToList();
    }
}
