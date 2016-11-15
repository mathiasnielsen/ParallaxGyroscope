using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Runtime;
using System;
using Android.Content;
using System.Collections.Generic;
using System.Linq;

namespace ParallaxGyroscope
{
    [Activity(Label = "ParallaxGyroscope", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity, ISensorEventListener
    {
        static readonly object _syncLock = new object();
        SensorManager _sensorManager;

        TextView _sensorTextView;
        ImageView _parallaxImageView;

        private float ALPHA = 0.50f;

        private float[] grav = new float[3];
        private float[] mag = new float[3];
        private float[] acc = new float[3];
        private float[] gyro = new float[3];

        protected float[] gravSensorVals;
        protected float[] accSensorVals;
        protected float[] magSensorVals;
        protected float[] gyroSensorVals;

        public MainActivity()
        {
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
        }

        public void OnSensorChanged(SensorEvent evt)
        {
            lock (_syncLock)
            {
                var rawData = string.Format("x={0:f}, y={1:f}, y={2:f}", evt.Values[0], evt.Values[1], evt.Values[2]);
                var valuesAsArray = evt.Values.ToArray();
                var valuesToUse = valuesAsArray;

                if (evt.Sensor.Type == SensorType.Gravity)
                {
                    var gravValues = LowPass(valuesAsArray, gravSensorVals);
                    grav[0] = gravValues[0];
                    grav[1] = gravValues[1];
                    grav[2] = gravValues[2];

                    valuesToUse = grav;
                }

                if (evt.Sensor.Type == SensorType.Accelerometer)
                {
                    var accValues = LowPass(valuesAsArray, accSensorVals);
                    acc[0] = accValues[0];
                    acc[1] = accValues[1];
                    acc[2] = accValues[2];

                    valuesToUse = acc;
                }

                if (evt.Sensor.Type == SensorType.MagneticField)
                {
                    var magValues = LowPass(valuesAsArray, magSensorVals);
                    mag[0] = magValues[0];
                    mag[1] = magValues[1];
                    mag[2] = magValues[2];

                    valuesToUse = mag;
                }

                if (evt.Sensor.Type == SensorType.Gyroscope)
                {
                    var gyroValues = LowPass(valuesAsArray, gyroSensorVals);
                    gyro[0] = gyroValues[0];
                    gyro[1] = gyroValues[1];
                    gyro[2] = gyroValues[2];

                    valuesToUse = gyro;
                }

                var valuesAsText = string.Format("x={0:f}, y={1:f}, y={2:f}", valuesToUse[0], valuesToUse[1], valuesToUse[2]);
                _sensorTextView.Text = valuesAsText;

                var scale = 20;
                _parallaxImageView.TranslationX = valuesToUse[0] * scale;
                _parallaxImageView.TranslationY = valuesToUse[1] * scale;
            }
        }

        /// <summary>
        /// https://www.built.io/blog/applying-low-pass-filter-to-android-sensor-s-readings#sthash.Ai9FuXRB.dpuf
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        protected float[] LowPass(float[] input, float[] output)
        {
            if (output == null)
            {
                return input;
            }

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = output[i] + ALPHA * (input[i] - output[i]);
            }

            return output;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            _sensorManager = (SensorManager)GetSystemService(Context.SensorService);

            _sensorTextView = FindViewById<TextView>(Resource.Id.accelerometer_text);

            _parallaxImageView = FindViewById<ImageView>(Resource.Id.parallaxImageView);
        }

        protected override void OnResume()
        {
            base.OnResume();
            ////_sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.MagneticField), SensorDelay.Ui);
            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Gravity), SensorDelay.Ui);
            ////_sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Ui);
            ////_sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Gyroscope), SensorDelay.Ui);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _sensorManager.UnregisterListener(this);
        }
    }
}

