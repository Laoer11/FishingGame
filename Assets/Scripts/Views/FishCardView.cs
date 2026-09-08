using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using FishingGame.Models;

namespace FishingGame.Views
{
    /// <summary>结算鱼种卡片:鱼图+星级+奖励+数量。星星用"隐藏模板+克隆"模式</summary>
    public class FishCardView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _starTemplate;
        [SerializeField] private Text _priceText;
        [SerializeField] private Text _shellText;
        [SerializeField] private Text _countText;

        private readonly List<Image> _stars = new List<Image>();

        /// <summary>填充卡片数据(由 ResultView 在克隆后调用)</summary>
        public void SetData(FishDef def, int count)
        {
            if(def == null) return;
            if(_icon != null) _icon.sprite = def.Sprite;

            BuildStars(def.StarLevel);

            if(_priceText != null) _priceText.text = "金币" + "  " + def.CoinReward;
            if(_shellText != null) _shellText.text = "贝壳" + "  " + def.ShellReward;
            if(_countText != null) _countText.text = "×" + count;
        }

        /// <summary>星级构建:先复用已克隆星星(多余隐藏)→不足补克隆→按模板位置等距排列(间距=星宽+8)。</summary>
        private void BuildStars(int level)
        {
            if(_starTemplate == null) return;

            int need = Mathf.Clamp(level, 0, 5);
            for(int i = 0; i < _stars.Count; i++)
            {
                if(_stars[i] != null) _stars[i].gameObject.SetActive(i < need);
            }

            for(int i = _stars.Count; i < need; i++)
            {
                Image star = Instantiate(_starTemplate, _starTemplate.transform.parent);
                star.gameObject.SetActive(true);
                _stars.Add(star);
            }

            RectTransform tpl = (RectTransform)_starTemplate.transform;
            float spacing = tpl.rect.width + 8f;
            for(int i = 0; i < need; i++)
            {
                RectTransform rt = (RectTransform)_stars[i].transform;
                rt.anchoredPosition = new Vector2(
                    tpl.anchoredPosition.x + spacing * i,
                    tpl.anchoredPosition.y);
            }

            _starTemplate.gameObject.SetActive(false);
        }
    }
}