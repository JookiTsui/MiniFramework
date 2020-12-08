﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniFramework {
    [XLua.LuaCallCSharp]
    public class AudioMgr {
        private static bool _isAudioSourceExits = false;
        private static AudioSource _audioSource;
        public static void PlayMusic(string audioClipName, string abName, float volume) {
        public static void StopMusic() {
        public static void PlaySound(string audioClipName, string abName, float volume) {
        public static void AdjustVolume(float volume) {
        public static void Mute() {
        private static void CheckAudioSource() {