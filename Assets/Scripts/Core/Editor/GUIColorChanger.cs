using System;
using UnityEngine;

namespace ilsFramework.Core.Editor
{
    public class GUIColorChanger : IDisposable
    {
        public Color newColor;
        public Color orginalColor;

        public GUIColorChanger(Color color)
        {
            newColor = color;
            orginalColor= GUI.color;
            GUI.color = newColor;
        }
        
        public void Dispose()
        {
            GUI.color = orginalColor;   
        }
    }
    
    public class GUIBackgroundColorChanger : IDisposable
    {
        public Color newColor;
        public Color orginalColor;

        public GUIBackgroundColorChanger(Color color)
        {
            newColor = color;
            orginalColor= GUI.backgroundColor;
            GUI.backgroundColor = newColor;
        }
        
        public void Dispose()
        {
            GUI.backgroundColor = orginalColor;   
        }
    }
    
    public class GUIContentColorChanger : IDisposable
    {
        public Color newColor;
        public Color orginalColor;

        public GUIContentColorChanger(Color color)
        {
            newColor = color;
            orginalColor= GUI.contentColor;
            GUI.contentColor = newColor;
        }
        
        public void Dispose()
        {
            GUI.contentColor = orginalColor;   
        }
    }
}