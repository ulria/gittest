﻿using System.Collections.Generic;
using UnityEngine;
using static Core.ResourcesHelper;

namespace LowPop
{
    public class DoneTutorialStep : TutorialStep
    {
        private GameObject _directivesGO;

        private bool _loaded = false;
        private bool _wasLoaded = false;

        public override void Load()
        {
            Debug.Log($"{System.DateTime.Now} DoneTutorialStep loaded.");
            if (_wasLoaded)
            {
                Reload();
            }
            else
            {
                InternalLoad();
            }
        }

        private void InternalLoad()
        {
            // Display generic consignes
            var middlegroundGO = GameObject.Find("1-Middleground");
            _directivesGO = new GameObject("directivesGO");
            _directivesGO.transform.parent = middlegroundGO.transform;
            var directivesBackgroundGO = new GameObject("directivesBackgroundGO");
            directivesBackgroundGO.transform.parent = _directivesGO.transform;
            var directivesBackgroundRectTransform = directivesBackgroundGO.AddComponent<RectTransform>();
            directivesBackgroundRectTransform.SetPositionAndRotation(new Vector3(-250, 250, 0), new Quaternion());
            directivesBackgroundRectTransform.sizeDelta = new Vector2(155, 155);
            directivesBackgroundRectTransform.localScale = new Vector3(6.5f, 3.25f, 1f);
            var sr = directivesBackgroundGO.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>(Resource.LowPop.Square);
            sr.material = Resources.Load<Material>(Resource.Core.GradientAlphaMaterial);

            sr.material.SetColor("_FirstColor", new Color(50f / 255f, 50f / 255f, 50f / 255f, 1));
            sr.material.SetColor("_SecondColor", new Color(120f / 255f, 120f / 255f, 120f / 255f, 1));
            sr.material.SetFloat("_Opacity", 0.6f);

            var directivesTextGO = new GameObject("directivesTextGO");
            directivesTextGO.transform.parent = directivesBackgroundGO.transform;
            var directivesTextMesh = directivesTextGO.AddComponent<TMPro.TextMeshPro>();
            directivesTextMesh.text = LabelHelper.ResolveLabel(LabelHelper.Label.LowPopTutorialDone);
            var directivesTextGORect = directivesTextGO.GetComponent<RectTransform>();
            directivesTextGORect.sizeDelta = new Vector2(155, 155);
            directivesTextGORect.localScale = new Vector3(1, 1, 1);
            directivesTextMesh.fontSizeMin = 100;
            directivesTextMesh.fontSizeMax = 500;
            directivesTextMesh.color = new Color(225f / 255f, 225f / 255f, 225f / 255f, 1);
            directivesTextMesh.alignment = TMPro.TextAlignmentOptions.Center;
            directivesTextMesh.transform.localPosition = new Vector3(0, 0, -0.1f);
            directivesTextMesh.enableAutoSizing = true;
            // Left, Top, Right, Bottom            
            directivesTextMesh.margin = new Vector4(10, 20, 10, 20);

            // Add onClick event listener
            directivesBackgroundGO.AddComponent<PolygonCollider2D>();
            var mouseHandler = directivesBackgroundGO.AddComponent<MouseHandler>();
            mouseHandler.AddOnMouseDownEvent(OnClick);

            _loaded = true;
        }

        private void Reload()
        {
            if (_directivesGO)
            {
                _directivesGO.SetActive(true);
            }
        }

        public void OnClick()
        {
            _directivesGO.SetActive(false);

            // Complete step
            _tutorialManager.CompleteStep();

            Debug.Log($"{System.DateTime.Now} Completed DoneTutorialStep");
        }

        public override void Update()
        {
            
        }

        public override void Unload()
        {
            Debug.Log($"{System.DateTime.Now} DoneTutorialStep unloaded.");
            if (_directivesGO)
            {
                _directivesGO.SetActive(false);
            }
            _loaded = false;
        }
    }
}