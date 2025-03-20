using UnityEngine;

public class AttributeTransformer : MonoBehaviour
{
    [SerializeField]
    private AbstractDataProvider dataSource;

    [SerializeField]
    private MeshRenderer blossom;

    [SerializeField]
    private Material FocusLevel1;

    [SerializeField]
    private Material FocusLevel2;

    [SerializeField]
    private Material FocusLevel3;

    [SerializeField]
    private Material FocusLevel4;

[SerializeField]
    private Material FocusLevel5;



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

            if (focusValue >= 0 && focusValue < 0.3f) {
                blossom.GetComponent<Renderer>().material = FocusLevel1;
            } else if (focusValue >= 0.2f && focusValue < 0.27f) {
                blossom.GetComponent<Renderer>().material = FocusLevel2;
            } else if (focusValue >= 0.27f && focusValue <= 0.34f) {
                blossom.GetComponent<Renderer>().material = FocusLevel3;
            } else if (focusValue >= 0.34f && focusValue <= 0.43f) {
                blossom.GetComponent<Renderer>().material = FocusLevel3;
            } else if (focusValue >= 0.43f && focusValue <= 1f) {
                blossom.GetComponent<Renderer>().material = FocusLevel3;
            }

        }

}