using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class LocationManage : MonoBehaviour {

  public Text txtLocation;
  public Text txtInfo;
  private AmapEvent amap;
  private AndroidJavaClass jcu;
  private AndroidJavaObject jou;
  private AndroidJavaObject mLocationClient;
  private AndroidJavaObject mLocationOption;

  public void StartLocation() {
    try {
      txtInfo.text = "start location...";

      txtInfo.text = txtInfo.text + "\r\n";
      jcu = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
      jou = jcu.GetStatic<AndroidJavaObject>("currentActivity");
      txtInfo.text = txtInfo.text + "currentActivity get...";

      txtInfo.text = txtInfo.text + "\r\n";
      mLocationClient = new AndroidJavaObject("com.amap.api.location.AMapLocationClient", jou);
      txtInfo.text = txtInfo.text + "AMapLocationClient get...";

      txtInfo.text = txtInfo.text + "\r\n";
      mLocationOption = new AndroidJavaObject("com.amap.api.location.AMapLocationClientOption");
      txtInfo.text = txtInfo.text + "AMapLocationClientOption get...";

      txtInfo.text = txtInfo.text + "\r\n";
      mLocationClient.Call("setLocationOption", mLocationOption);
      txtInfo.text = txtInfo.text + "setLocationOption...";

      amap = new AmapEvent();
      amap.locationChanged += OnLocationChanged;

      txtInfo.text = txtInfo.text + "\r\n";
      mLocationClient.Call("setLocationListener", amap);
      txtInfo.text = txtInfo.text + "setLocationListener...";

      txtInfo.text = txtInfo.text + "\r\n";
      mLocationClient.Call("startLocation");
      txtInfo.text = txtInfo.text + "startLocation...";

    } catch (Exception ex) {
      txtInfo.text = txtInfo.text + "\r\n";
      txtInfo.text = txtInfo.text + "--------------------";
      txtInfo.text = txtInfo.text + ex.Message;

      EndLocation();
    }
  }

  public void EndLocation() {
    if (amap != null) {
      amap.locationChanged -= OnLocationChanged;
    }

    if (mLocationClient != null) {
      mLocationClient.Call("stopLocation");
      mLocationClient.Call("onDestroy");
    }

    txtLocation.text = "";
  }

  private void OnLocationChanged(AndroidJavaObject amapLocation) {
    if (amapLocation != null) {
      if (amapLocation.Call<int>("getErrorCode") == 0) {
        txtLocation.text = ">>success:";

        try {
          txtLocation.text = txtLocation.text + "\r\n>>��λ�����Դ:" + amapLocation.Call<int>("getLocationType").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>γ��:" + amapLocation.Call<double>("getLatitude").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>����:" + amapLocation.Call<double>("getLongitude").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>������Ϣ:" + amapLocation.Call<float>("getAccuracy").ToString();
          //txtLocation.text = txtLocation.text + "\r\n>>��λʱ��:" + amapLocation.Call<AndroidJavaObject> ("getTime").ToString ();  
          txtLocation.text = txtLocation.text + "\r\n>>��ַ:" + amapLocation.Call<string>("getAddress").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>����:" + amapLocation.Call<string>("getCountry").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>ʡ:" + amapLocation.Call<string>("getProvince").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>����:" + amapLocation.Call<string>("getCity").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>����:" + amapLocation.Call<string>("getDistrict").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>�ֵ�:" + amapLocation.Call<string>("getStreet").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>����:" + amapLocation.Call<string>("getStreetNum").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>���б���:" + amapLocation.Call<string>("getCityCode").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>��������:" + amapLocation.Call<string>("getAdCode").ToString();

          txtLocation.text = txtLocation.text + "\r\n>>����:" + amapLocation.Call<double>("getAltitude").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>�����:" + amapLocation.Call<float>("getBearing").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>��λ��Ϣ����:" + amapLocation.Call<string>("getLocationDetail").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>��Ȥ��:" + amapLocation.Call<string>("getPoiName").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>�ṩ��:" + amapLocation.Call<string>("getProvider").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>��������:" + amapLocation.Call<int>("getSatellites").ToString();
          //txtLocation.text = txtLocation.text + "\r\n>>��ǰ�ٶ�:" + amapLocation.Call<string> ("getSpeed").ToString ();  

        } catch (Exception ex) {
          txtLocation.text = txtLocation.text + "\r\n--------------ex-------------:";
          txtLocation.text = txtLocation.text + "\r\n" + ex.Message;
        }

      } else {
        txtLocation.text = ">>amaperror:";
        txtLocation.text = txtLocation.text + ">>getErrorCode:" + amapLocation.Call<int>("getErrorCode").ToString();
        txtLocation.text = txtLocation.text + ">>getErrorInfo:" + amapLocation.Call<string>("getErrorInfo");
      }
    } else {
      txtInfo.text = "amaplocation is null.";
    }
  }
}