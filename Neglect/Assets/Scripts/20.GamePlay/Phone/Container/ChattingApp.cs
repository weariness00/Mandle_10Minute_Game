﻿using GamePlay.Chatting;
using UnityEngine;

namespace GamePlay.Phone
{
    public partial class ChattingApp : MonoBehaviour
    {
        [Header("채팅 관련")] 
        [SerializeField] private Conversation chatting;
    }

    public partial class ChattingApp : IPhoneApplication
    {
        [Header("Phone 관련")]
        [SerializeField] private string appName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector2Int verticalResolution;
        [SerializeField] private PhoneControl _phone;
        public AppState AppState { get; set; }

        public string AppName => appName;
        public Sprite AppIcon { get => icon; set => icon =value; }
        public Vector2Int VerticalResolution { get => verticalResolution; set => verticalResolution = value; }
        public PhoneControl Phone => _phone;

        public void SetActiveBackground(bool value)
        {
            chatting.gameObject.SetActive(value);
        }

        public void AppInstall(PhoneControl phone)
        {
            _phone = phone;

            chatting.canvas.worldCamera = _phone.phoneCamera;
            chatting.backButton.onClick.AddListener(() => phone.applicationControl.CloseApp(this));
            
            SetActiveBackground(false);
        }

        public void AppPlay(PhoneControl phone)
        {
            SetActiveBackground(true);
            chatting.Init();
        }

        public void AppResume(PhoneControl phone)
        {
            SetActiveBackground(true);
            chatting.Init();
        }

        public void AppPause(PhoneControl phone)
        {
            SetActiveBackground(false);
        }

        public void AppExit(PhoneControl phone)
        {
            SetActiveBackground(false);
        }

        public void AppUnInstall(PhoneControl phone)
        {
        }
    }
}

