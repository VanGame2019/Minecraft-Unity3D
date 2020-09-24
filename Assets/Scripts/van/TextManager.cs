using System;
using TMPro;
using UnityEngine;

namespace van
{
    public class TextManager:MonoBehaviour
    {
        public static TextManager instance;
        
        public TextMeshPro textMeshPro;
        public GameObject rect;
        
        
        private void Awake()
        {
            instance = this;
        }

        public void createText(string text,float size,Vector2 position,Vector2 realSize)
        {
            var t = Instantiate(textMeshPro);
            t.transform.SetParent(this.transform);
            
            t.text = text;
            t.fontSize = Mathf.Lerp(1f,3f,size/20000f) ;
            t.transform.localPosition = position / 50f;


            var r = Instantiate(this.rect);
            r.transform.SetParent(this.transform);
            r.transform.localPosition = position / 50f;
            r.GetComponent<SpriteRenderer>().size = realSize / 50f;

        }
    }
}