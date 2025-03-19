using UnityEngine;

public class AttributeTransformerGradient : MonoBehaviour
{
    [SerializeField]
    private AbstractDataProvider dataSource;

    [SerializeField]
    private MeshRenderer blossom;

    [SerializeField]
    private Gradient focusGradient;


    private void Awake(){
        if (dataSource != null){
            dataSource.OnDataReceived += ReceiveDataSample;
        }
    }

    private void OnDestroy() {
        if (dataSource != null) {
            dataSource.OnDataReceived -= ReceiveDataSample;
        }
    }

    private void ReceiveDataSample(float[] sample, double timestamp){
            float focusValue = sample[0];

        Debug.Log("Received data sample: " + string.Join(", ", sample)); 
        
        float mappedFocusValue = Mathf.Clamp01((focusValue - 0.2f) / 0.6f);

        Color focusColor = focusGradient.Evaluate(focusValue);

        blossom.material.color = focusColor;
        
        }

}