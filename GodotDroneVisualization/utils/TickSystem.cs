namespace GodotDroneVisualization.Entities;

using System;
using System.Collections.Generic;
using Godot;

public class TickSystem
{
    private readonly Dictionary<string, TickTimer> _timers = new();
    
    public void CreateTimer(string name, float interval, bool autoReset = true)
    {
        _timers[name] = new TickTimer(interval, autoReset);
    }
    
    public void Update(float delta)
    {
        foreach (var timer in _timers.Values)
        {
            timer.Update(delta);
        }
    }
    
    public bool IsReady(string name)
    {
        return _timers.TryGetValue(name, out var timer) && timer.IsReady();
    }
    
    public void Reset(string name)
    {
        if (_timers.TryGetValue(name, out var timer))
        {
            timer.Reset();
        }
    }
    
    public void SetInterval(string name, float interval)
    {
        if (_timers.TryGetValue(name, out var timer))
        {
            timer.SetInterval(interval);
        }
    }
    
    private class TickTimer
    {
        private float _interval;
        private float _elapsed;
        private bool _autoReset;
        private bool _isReady;
        
        public TickTimer(float interval, bool autoReset)
        {
            _interval = interval;
            _autoReset = autoReset;
            _elapsed = 0f;
            _isReady = false;
        }
        
        public void Update(float delta)
        {
            _elapsed += delta;
        
            if (_elapsed >= _interval)
            {
                _isReady = true;
            
                if (_autoReset)
                {
                    _elapsed = 0f; // Reset for next cycle
                }
            }
        }
        
        public bool IsReady()
        {
            if (_isReady)
            {
                // Always reset _isReady to false after checking
                _isReady = false;
                return true;
            }
            return false;
        }
        
        public void Reset()
        {
            _elapsed = 0f;
            _isReady = false;
        }
        
        public void SetInterval(float interval)
        {
            _interval = interval;
        }
    }
}