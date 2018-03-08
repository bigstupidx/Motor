using UnityEngine;
using System.Collections;

public class EasyQRCode : MonoBehaviour {

    public string textToEncode = "wang hui!";
    public Color darkColor = Color.black;
    public Color lightColor = Color.white;

    void Start()
    {
        // Example usage of QR Generator
        // The text can be any string, link or other QR Code supported string

        Texture2D qrTexture = QRGenerator.EncodeString(textToEncode, darkColor, lightColor);

        // Set the generated texture as the mainTexture on the quad
        GetComponent<Renderer>().material.mainTexture = qrTexture;
    }
}
