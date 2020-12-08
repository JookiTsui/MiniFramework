﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniFramework {
		private static Dictionary<EventType, Delegate> _eventTable = new Dictionary<EventType, Delegate>();
		public static void AddListener(EventType eventType, Action callBack) {
		public static void AddListener<T>(EventType eventType, Action<T> callBack) {
		public static void AddListener<T1, T2>(EventType eventType, Action<T1, T2> callBack) {
		public static void AddListener<T1, T2, T3>(EventType eventType, Action<T1, T2, T3> callBack) {
		public static void AddListener<T1, T2, T3, T4>(EventType eventType, Action<T1, T2, T3, T4> callBack) {
		public static void RemoveListener(EventType eventType, Action callBack) {
		public static void RemoveListener<T>(EventType eventType, Action<T> callBack) {
		public static void RemoveListener<T1, T2>(EventType eventType, Action<T1, T2> callBack) {
		public static void RemoveListener<T1, T2, T3>(EventType eventType, Action<T1, T2, T3> callBack) {
		public static void RemoveListener<T1, T2, T3, T4>(EventType eventType, Action<T1, T2, T3, T4> callBack) {
		public static void BroadcastEvent(EventType eventType) {
		public static void BroadcastEvent<T>(EventType eventType, T arg) {
		public static void BroadcastEvent<T1, T2>(EventType eventType, T1 arg1, T2 arg2) {
		public static void BroadcastEvent<T1, T2, T3>(EventType eventType, T1 arg1, T2 arg2, T3 arg3) {
		public static void BroadcastEvent<T1, T2, T3, T4>(EventType eventType, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {