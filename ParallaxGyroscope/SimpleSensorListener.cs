using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ParallaxGyroscope
{
    public class SimpleSensorListener : Java.Lang.Object, ISensorEventListener
    {
        public SimpleSensorListener()
        {

        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            throw new NotImplementedException();
        }

        public void OnSensorChanged(SensorEvent e)
        {

        }
    }
}