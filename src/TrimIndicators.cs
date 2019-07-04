/*
 * Trim Indicators based on an idea by dazoe 
 * 
 * */

using System;
using UnityEngine;
using KSP.UI.Screens.Flight;

namespace TrimIndicators
{

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class TrimIndicatorsMono : MonoBehaviour
    {
        // the scaled size of the trim indicators
        private const float SIZE = 0.75f;

        // the new trim gauges
        private KSP.UI.Screens.LinearGauge trimRollGauge;
        private KSP.UI.Screens.LinearGauge trimPitchGauge;
        private KSP.UI.Screens.LinearGauge trimYawGauge;

        // the trim gauges get globally disabled if any exception is thrown
        private static bool disabled = false;

        private KSP.UI.Screens.LinearGauge CreateGauge(String name, KSP.UI.Screens.LinearGauge component)
        {
            GameObject indicator = (GameObject)MonoBehaviour.Instantiate(component.gameObject, Vector3.zero, Quaternion.identity);

            indicator.name = name;
            // set parent
            indicator.transform.SetParent(component.transform.parent);
            // set rotations
            indicator.transform.rotation = component.transform.rotation;
            indicator.transform.position = component.transform.position;
            indicator.transform.localPosition = component.transform.localPosition;
            indicator.transform.localScale = component.transform.localScale;
            indicator.transform.localRotation = component.transform.localRotation;

            KSP.UI.Screens.LinearGauge gauge = indicator.GetComponent<KSP.UI.Screens.LinearGauge>();
            indicator.transform.localScale = Vector3.Scale(component.transform.localScale, new Vector3(-SIZE, SIZE, SIZE));

            if (name == "GaugePitchTrimPointer")
            {
                // set size of the indicator by scaling them down and inverting the direction
                float origWidth = gauge.pointer.rect.width / SIZE + 2;
                indicator.transform.localPosition += new Vector3(gauge.pointer.rect.width, 0);

                gauge.minValuePosition -= new Vector2(origWidth, 0);
                gauge.maxValuePosition -= new Vector2(origWidth, 0);
            }
            else
            {
                // set size of the indicator by scaling them down and inverting the direction
                float origHeight = gauge.pointer.rect.height + 2;

                gauge.minValuePosition -= new Vector2(0, origHeight);
                gauge.maxValuePosition -= new Vector2(0, origHeight);
            }
            //return gauge; 
            return gauge;
        }

        public void Start()
        {
            // try to catch any exception if compatibility issues with KSP occur
            try
            {
                var gaugesObject = (GameObject)MonoBehaviour.FindObjectOfType<LinearControlGauges>().gameObject;

                var gaugesClass = gaugesObject.GetComponent<LinearControlGauges>();

                KSP.UI.Screens.LinearGauge roll = gaugesClass.roll;
                KSP.UI.Screens.LinearGauge pitch = gaugesClass.pitch;
                KSP.UI.Screens.LinearGauge yaw = gaugesClass.yaw;

                // create gauges
                this.trimRollGauge = CreateGauge("GaugeRollTrimPointer", roll);
                this.trimPitchGauge = CreateGauge("GaugePitchTrimPointer", pitch);
                this.trimYawGauge = CreateGauge("GaugeYawTrimPointerw", yaw);
            }
            catch (Exception e)
            {
                Debug.Log("exception caught in trim indicator init");
                disabled = true;
                throw e;
            }
        }

        public void Update()
        {
            if (disabled) return;
            // try to catch any exception if compatibility issues with KSP occur
            try
            {
                // update the trim gauges
                FlightCtrlState ctrlState = FlightGlobals.ActiveVessel.ctrlState;

                trimRollGauge.SetValue(ctrlState.rollTrim);
                trimPitchGauge.SetValue(-ctrlState.pitchTrim);
                trimYawGauge.SetValue(ctrlState.yawTrim);

            }
            catch
            {
                Debug.Log("exception caught in trim indicator update");
                disabled = true;
                Destroy(this);
            }
        }
    }
}
