﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace LOP.Core
{
    public class MainUpdater : IUpdatable
    {
        private static MainUpdater instance;
        public static MainUpdater Instance
        {
            get { return instance ??= new MainUpdater(); }
        }
        private List<Script> scripts = new List<Script>();
        public static void AddScript(Script script)
        {
            if (Instance.scripts.Contains(script)) return;
            Instance.scripts.Add(script);
        }
        public static void RemoveScript(Script script)
        {
            if (Instance.scripts.Contains(script))
            {
                script.OnDestroy();
                Instance.scripts.Remove(script);
            }
        }
        public static T? FindScript<T>() where T : Script
        {
            foreach(var script in Instance.scripts)
            {
                if(script.GetType() == typeof(T))
                {
                    return script as T;
                }
            }
            return null;
        }
        private MainUpdater()
        {
            scripts = new List<Script>();
        }
        public void Init()
        {
            Type scriptType = typeof(Script);
            var types = Assembly.GetCallingAssembly().GetTypes();
            foreach (var type in types)
            {
                var parent = type.BaseType;
                if (parent != null && parent.FullName == scriptType.FullName)
                {
                    Console.WriteLine("Init: " + type.FullName);
                    Script script = Activator.CreateInstance(type) as Script;
                    if (script != null) AddScript(script);
                }
            }
        }
        public void Start()
        {
            Time.Start();
            foreach (var script in scripts)
            {
                script.Start();
            }
        }
        public void Update()
        {
            Time.Update();
            foreach (var script in scripts)
            {
                script.Update();
            }
        }
    }
    public abstract class Script : IUpdatable
    {
        public Script() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void OnDestroy() { }
        public virtual void Destroy()
        {
            MainUpdater.RemoveScript(this);
        }
        public static void Destroy(Script script)
        {
            script.Destroy();
        }
        public static T Instantiate<T>() where T : Script , new()
        {
            T newt = new();
            newt.Start();
            MainUpdater.AddScript(newt);
            return newt;
        }
        public static T? FindScript<T>() where T : Script
        {
            return MainUpdater.FindScript<T>();
        }
    }
}
