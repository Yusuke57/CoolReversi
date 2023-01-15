using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common
{
    public class SEPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<SEClipData> clipDataList;
        
        public static SEPlayer I { get; private set; }

        private void Awake()
        {
            I = this;
        }

        public void Play(SEName seName)
        {
            var data = clipDataList.FirstOrDefault(data => data.name == seName);
            if (data == null)
            {
                return;
            }
            
            audioSource.PlayOneShot(data.clip, data.volume);
        }

        public enum SEName
        {
            CreateBoard = 10,
            
            PutStone = 100,
            ReverseStone = 110,
            
            Win = 500,
            Lose = 510,
            
            Retry = 600,
        }
        
        [Serializable]
        public class SEClipData
        {
            public SEName name;
            public AudioClip clip;
            public float volume;
        }
    }
}